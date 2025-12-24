-- Fixes for FootballDB: ALTER AddReservation to accept @FieldID and CREATE dbo.SearchReservations.
-- Run this script in the context of the FootballDB database.
-- This script does NOT drop tables or data.

SET NOCOUNT ON;
GO

/***** ALTER AddReservation to accept @FieldID (and use dbo schema) *****/
-- We use ALTER PROCEDURE because AddReservation already exists in your DB.
ALTER PROCEDURE dbo.AddReservation
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

        -- Use Customers.FullName (existing schema) to find customer
        SELECT @CustomerID = CustomerID
        FROM dbo.Customers
        WHERE FullName = @CustomerName
          AND (@Phone IS NULL OR Phone = @Phone);

        IF @CustomerID IS NULL
        BEGIN
            INSERT INTO dbo.Customers (FullName, Phone, CreatedAt)
            VALUES (@CustomerName, @Phone, GETDATE());

            SET @CustomerID = SCOPE_IDENTITY();
        END

        -- Overlap check for same field and active reservations
        IF EXISTS (
            SELECT 1 FROM dbo.Reservations
            WHERE FieldID = @FieldID
              AND Status = 'Confirmed'
              AND NOT (EndTime <= @StartTime OR StartTime >= @EndTime)
        )
        BEGIN
            RAISERROR('Time slot is already booked for the selected field.', 16, 1);
            ROLLBACK TRAN;
            RETURN;
        END

        INSERT INTO dbo.Reservations (CustomerID, CustomerName, Phone, FieldID, StartTime, EndTime, TotalPrice, Status, CreatedAt)
        VALUES (@CustomerID, @CustomerName, @Phone, @FieldID, @StartTime, @EndTime, @TotalPrice, 'Confirmed', GETDATE());

        COMMIT TRAN;
    END TRY
    BEGIN CATCH
        IF XACT_STATE() <> 0 ROLLBACK TRAN;
        DECLARE @ErrMsg NVARCHAR(4000) = ERROR_MESSAGE();
        RAISERROR(@ErrMsg, 16, 1);
    END CATCH
END
GO

/***** Ensure ViewReservations exists under dbo. Do not DROP existing data. *****/
-- If ViewReservations does not exist you should create it; if it exists leave as-is.
-- Below is a safe CREATE VIEW IF NOT EXISTS pattern for older SQL Server: drop and create in separate batches.
IF OBJECT_ID('dbo.ViewReservations', 'V') IS NULL
BEGIN
    -- Create the view if it does not exist. If it does exist, this block is skipped.
    EXEC('
    CREATE VIEW dbo.ViewReservations AS
    SELECT
        r.ReservationID,
        r.CustomerID,
        r.CustomerName,
        r.Phone,
        r.FieldID,
        f.FieldName,
        r.StartTime,
        r.EndTime,
        r.TotalPrice,
        r.Status,
        r.CreatedAt
    FROM dbo.Reservations r
    LEFT JOIN dbo.Fields f ON r.FieldID = f.FieldID;
    ');
END
GO

/***** Create unified search procedure dbo.SearchReservations *****/
IF OBJECT_ID('dbo.SearchReservations', 'P') IS NOT NULL
    DROP PROCEDURE dbo.SearchReservations;
GO

CREATE PROCEDURE dbo.SearchReservations
    @Date DATETIME = NULL,                 -- optional, match on StartTime date
    @FieldID INT = NULL,                   -- optional
    @CustomerName NVARCHAR(200) = NULL     -- optional, caller should pass '%name%' for partial searches
AS
BEGIN
    SET NOCOUNT ON;
    BEGIN TRY
        DECLARE @StartOfDay DATETIME = NULL, @EndOfDay DATETIME = NULL;
        IF @Date IS NOT NULL
        BEGIN
            SET @StartOfDay = CONVERT(DATE, @Date);
            SET @EndOfDay = DATEADD(DAY, 1, @StartOfDay);
        END

        SELECT
            vr.ReservationID,
            vr.CustomerID,
            vr.CustomerName,
            vr.Phone,
            vr.FieldID,
            vr.FieldName,
            vr.StartTime,
            vr.EndTime,
            vr.TotalPrice,
            vr.Status,
            vr.CreatedAt
        FROM dbo.ViewReservations vr
        WHERE
            (@Date IS NULL OR (vr.StartTime >= @StartOfDay AND vr.StartTime < @EndOfDay))
            AND (@FieldID IS NULL OR vr.FieldID = @FieldID)
            AND (@CustomerName IS NULL OR vr.CustomerName LIKE @CustomerName)
        ORDER BY vr.StartTime;
    END TRY
    BEGIN CATCH
        DECLARE @ErrMsg NVARCHAR(4000) = ERROR_MESSAGE();
        RAISERROR(@ErrMsg, 16, 1);
    END CATCH
END
GO