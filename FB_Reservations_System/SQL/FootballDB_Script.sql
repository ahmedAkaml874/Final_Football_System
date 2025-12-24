-- SQL script: create/replace view and stored procedures for FootballDB.
-- IMPORTANT: This script does not drop existing tables or data.
-- Run in the context of FootballDB.

SET NOCOUNT ON;
GO

-- Ensure database objects are created in dbo schema
-- === ReservationsView ===
IF OBJECT_ID('dbo.ReservationsView', 'V') IS NOT NULL
    DROP VIEW dbo.ReservationsView;
GO

CREATE VIEW dbo.ReservationsView AS
SELECT
    r.ReservationID,
    c.CustomerName,
    c.Phone,
    f.FieldName,
    r.StartTime,
    r.EndTime,
    r.TotalPrice,
    r.Status
FROM dbo.Reservations r
JOIN dbo.Customers c ON r.CustomerID = c.CustomerID
JOIN dbo.Fields f ON r.FieldID = f.FieldID;
GO

-- === AddReservation ===
IF OBJECT_ID('dbo.AddReservation', 'P') IS NOT NULL
    DROP PROCEDURE dbo.AddReservation;
GO

CREATE PROCEDURE dbo.AddReservation
    @CustomerName NVARCHAR(200),
    @Phone NVARCHAR(50) = NULL,
    @FieldID INT,
    @StartTime DATETIME,
    @EndTime DATETIME,
    @TotalPrice DECIMAL(10,2)
AS
BEGIN
    SET NOCOUNT ON;
    BEGIN TRY
        BEGIN TRAN;

        DECLARE @CustomerID INT;

        SELECT @CustomerID = CustomerID
        FROM dbo.Customers
        WHERE CustomerName = @CustomerName
          AND (@Phone IS NULL OR Phone = @Phone);

        IF @CustomerID IS NULL
        BEGIN
            INSERT INTO dbo.Customers (CustomerName, Phone)
            VALUES (@CustomerName, @Phone);
            SET @CustomerID = SCOPE_IDENTITY();
        END

        -- Overlap check for same field and active reservations
        IF EXISTS (
            SELECT 1 FROM dbo.Reservations
            WHERE FieldID = @FieldID
              AND Status = 'Active'
              AND NOT (EndTime <= @StartTime OR StartTime >= @EndTime)
        )
        BEGIN
            RAISERROR('Time slot is already booked for the selected field.', 16, 1);
            ROLLBACK TRAN;
            RETURN;
        END

        INSERT INTO dbo.Reservations (CustomerID, FieldID, StartTime, EndTime, TotalPrice, Status)
        VALUES (@CustomerID, @FieldID, @StartTime, @EndTime, @TotalPrice, 'Active');

        COMMIT TRAN;
    END TRY
    BEGIN CATCH
        IF XACT_STATE() <> 0 ROLLBACK TRAN;
        DECLARE @ErrMsg NVARCHAR(4000) = ERROR_MESSAGE();
        RAISERROR(@ErrMsg, 16, 1);
    END CATCH
END
GO

-- === CancelReservation ===
IF OBJECT_ID('dbo.CancelReservation', 'P') IS NOT NULL
    DROP PROCEDURE dbo.CancelReservation;
GO

CREATE PROCEDURE dbo.CancelReservation
    @ReservationID INT
AS
BEGIN
    SET NOCOUNT ON;
    BEGIN TRY
        UPDATE dbo.Reservations
        SET Status = 'Canceled'
        WHERE ReservationID = @ReservationID;
    END TRY
    BEGIN CATCH
        DECLARE @ErrMsg NVARCHAR(4000) = ERROR_MESSAGE();
        RAISERROR(@ErrMsg, 16, 1);
    END CATCH
END
GO

-- === UpdateReservation ===
-- Updates reservation times, price and/or status. Keeps overlap check.
IF OBJECT_ID('dbo.UpdateReservation', 'P') IS NOT NULL
    DROP PROCEDURE dbo.UpdateReservation;
GO

CREATE PROCEDURE dbo.UpdateReservation
    @ReservationID INT,
    @StartTime DATETIME,
    @EndTime DATETIME,
    @TotalPrice DECIMAL(10,2),
    @Status NVARCHAR(50) = NULL  -- optional, pass NULL to keep current
AS
BEGIN
    SET NOCOUNT ON;
    BEGIN TRY
        BEGIN TRAN;

        DECLARE @FieldID INT;
        SELECT @FieldID = FieldID FROM dbo.Reservations WHERE ReservationID = @ReservationID;

        IF @FieldID IS NULL
        BEGIN
            RAISERROR('Reservation not found.', 16, 1);
            ROLLBACK TRAN;
            RETURN;
        END

        -- Overlap check excluding current reservation
        IF EXISTS (
            SELECT 1 FROM dbo.Reservations
            WHERE FieldID = @FieldID
              AND ReservationID <> @ReservationID
              AND Status = 'Active'
              AND NOT (EndTime <= @StartTime OR StartTime >= @EndTime)
        )
        BEGIN
            RAISERROR('Time slot is already booked for the selected field.', 16, 1);
            ROLLBACK TRAN;
            RETURN;
        END

        UPDATE dbo.Reservations
        SET StartTime = @StartTime,
            EndTime = @EndTime,
            TotalPrice = @TotalPrice,
            Status = COALESCE(@Status, Status)
        WHERE ReservationID = @ReservationID;

        COMMIT TRAN;
    END TRY
    BEGIN CATCH
        IF XACT_STATE() <> 0 ROLLBACK TRAN;
        DECLARE @ErrMsg NVARCHAR(4000) = ERROR_MESSAGE();
        RAISERROR(@ErrMsg, 16, 1);
    END CATCH
END
GO

-- === Unified SearchReservations ===
-- Supports optional filters: @Day, @FieldName, @CustomerName
-- Kept optional @FieldID for backward compatibility with existing UI.
IF OBJECT_ID('dbo.SearchReservations', 'P') IS NOT NULL
    DROP PROCEDURE dbo.SearchReservations;
GO

CREATE PROCEDURE dbo.SearchReservations
    @Day DATETIME = NULL,                -- optional: filter by date (StartTime date)
    @FieldID INT = NULL,                 -- optional: filter by FieldID (keeps compatibility)
    @FieldName NVARCHAR(100) = NULL,     -- optional: filter by FieldName (preferred)
    @CustomerName NVARCHAR(200) = NULL   -- optional: partial match (use %...% from caller)
AS
BEGIN
    SET NOCOUNT ON;
    BEGIN TRY
        DECLARE @DayStart DATETIME = NULL, @DayEnd DATETIME = NULL;
        IF @Day IS NOT NULL
        BEGIN
            SET @DayStart = CONVERT(DATE, @Day);
            SET @DayEnd = DATEADD(DAY, 1, @DayStart);
        END

        SELECT
            r.ReservationID,
            c.CustomerName,
            c.Phone,
            f.FieldName,
            r.StartTime,
            r.EndTime,
            r.TotalPrice,
            r.Status
        FROM dbo.Reservations r
        JOIN dbo.Customers c ON r.CustomerID = c.CustomerID
        JOIN dbo.Fields f ON r.FieldID = f.FieldID
        WHERE
            (@Day IS NULL OR (r.StartTime >= @DayStart AND r.StartTime < @DayEnd))
            AND (@FieldID IS NULL OR r.FieldID = @FieldID)
            AND (@FieldName IS NULL OR f.FieldName = @FieldName)
            AND (@CustomerName IS NULL OR c.CustomerName LIKE @CustomerName)
        ORDER BY r.StartTime;
    END TRY
    BEGIN CATCH
        DECLARE @ErrMsg NVARCHAR(4000) = ERROR_MESSAGE();
        RAISERROR(@ErrMsg, 16, 1);
    END CATCH
END
GO

-- Keep seed data for fields if empty (no change to existing data)
IF NOT EXISTS (SELECT 1 FROM dbo.Fields)
BEGIN
    INSERT INTO dbo.Fields (FieldName, PricePerHour, IsActive) VALUES
    ('Field A', 30.00, 1),
    ('Field B', 25.00, 1),
    ('Field C', 20.00, 0); -- inactive
END
GO