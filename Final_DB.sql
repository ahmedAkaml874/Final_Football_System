/*=====================================================
  FOOTBALL RESERVATION SYSTEM - FINAL SQL SCRIPT
=====================================================*/

SET NOCOUNT ON;
GO

/*==============================
  1) DATABASE
==============================*/
IF DB_ID('FootballDB') IS NULL
    CREATE DATABASE FootballDB;
GO

USE FootballDB;
GO

/*==============================
  2) DROP OBJECTS (SAFE ORDER)
==============================*/
IF OBJECT_ID('dbo.SearchReservations', 'P') IS NOT NULL DROP PROCEDURE dbo.SearchReservations;
IF OBJECT_ID('dbo.CancelReservation', 'P') IS NOT NULL DROP PROCEDURE dbo.CancelReservation;
IF OBJECT_ID('dbo.UpdateReservation', 'P') IS NOT NULL DROP PROCEDURE dbo.UpdateReservation;
IF OBJECT_ID('dbo.AddReservation', 'P') IS NOT NULL DROP PROCEDURE dbo.AddReservation;
IF OBJECT_ID('dbo.ViewReservations', 'V') IS NOT NULL DROP VIEW dbo.ViewReservations;

IF OBJECT_ID('dbo.Reservations', 'U') IS NOT NULL DROP TABLE dbo.Reservations;
IF OBJECT_ID('dbo.Customers', 'U') IS NOT NULL DROP TABLE dbo.Customers;
IF OBJECT_ID('dbo.Fields', 'U') IS NOT NULL DROP TABLE dbo.Fields;
GO

/*==============================
  3) TABLES
==============================*/
CREATE TABLE dbo.Fields (
    FieldID INT IDENTITY PRIMARY KEY,
    FieldName VARCHAR(100) NOT NULL,
    Location VARCHAR(200) NOT NULL,
    PricePerHour DECIMAL(10,2) NOT NULL CHECK (PricePerHour > 0),
    IsActive BIT NOT NULL DEFAULT 1
);

CREATE TABLE dbo.Customers (
    CustomerID INT IDENTITY PRIMARY KEY,
    FullName NVARCHAR(150) NOT NULL,
    Phone VARCHAR(30),
    CreatedAt DATETIME NOT NULL DEFAULT GETDATE()
);

CREATE TABLE dbo.Reservations (
    ReservationID INT IDENTITY PRIMARY KEY,
    CustomerID INT NOT NULL,
    CustomerName NVARCHAR(150) NOT NULL,
    Phone VARCHAR(30),
    FieldID INT NOT NULL,
    StartTime DATETIME NOT NULL,
    EndTime DATETIME NOT NULL,
    TotalPrice DECIMAL(10,2) NOT NULL,
    Status VARCHAR(20) NOT NULL DEFAULT 'Confirmed',
    CreatedAt DATETIME NOT NULL DEFAULT GETDATE(),

    CONSTRAINT FK_Res_Field FOREIGN KEY (FieldID) REFERENCES dbo.Fields(FieldID),
    CONSTRAINT FK_Res_Customer FOREIGN KEY (CustomerID) REFERENCES dbo.Customers(CustomerID),
    CONSTRAINT CK_Time CHECK (StartTime < EndTime)
);
GO

/*==============================
  4) SEED FIELDS
==============================*/
INSERT INTO dbo.Fields (FieldName, Location, PricePerHour) VALUES
('Green Field 1','University Street',200),
('Green Field 2','University Street',200),
('City Stadium 1','King Street',220),
('City Stadium 2','King Street',220),
('Youth Arena 1','Sports Street',260),
('Youth Arena 2','Sports Street',260);
GO

/*==============================
  5) VIEW
==============================*/
CREATE VIEW dbo.ViewReservations AS
SELECT
    r.ReservationID,
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
JOIN dbo.Fields f ON r.FieldID = f.FieldID;
GO

/*==============================
  6) ADD RESERVATION
==============================*/
CREATE PROCEDURE dbo.AddReservation
    @CustomerName NVARCHAR(150),
    @Phone VARCHAR(30),
    @FieldID INT,
    @StartTime DATETIME,
    @EndTime DATETIME,
    @TotalPrice DECIMAL(10,2)
AS
BEGIN
    SET NOCOUNT ON;

    DECLARE @CustomerID INT;

    SELECT @CustomerID = CustomerID
    FROM dbo.Customers
    WHERE FullName = @CustomerName AND (@Phone IS NULL OR Phone = @Phone);

    IF @CustomerID IS NULL
    BEGIN
        INSERT INTO dbo.Customers (FullName, Phone)
        VALUES (@CustomerName, @Phone);

        SET @CustomerID = SCOPE_IDENTITY();
    END

    IF EXISTS (
        SELECT 1 FROM dbo.Reservations
        WHERE FieldID = @FieldID
          AND Status = 'Confirmed'
          AND NOT (EndTime <= @StartTime OR StartTime >= @EndTime)
    )
    BEGIN
        RAISERROR('Time slot already booked.',16,1);
        RETURN;
    END

    INSERT INTO dbo.Reservations
    (CustomerID, CustomerName, Phone, FieldID, StartTime, EndTime, TotalPrice)
    VALUES
    (@CustomerID, @CustomerName, @Phone, @FieldID, @StartTime, @EndTime, @TotalPrice);
END;
GO

/*==============================
  7) CANCEL RESERVATION
==============================*/
CREATE PROCEDURE dbo.CancelReservation
    @ReservationID INT
AS
BEGIN
    UPDATE dbo.Reservations
    SET Status = 'Canceled'
    WHERE ReservationID = @ReservationID;
END;
GO

/*==============================
  8) SEARCH RESERVATIONS
==============================*/
CREATE PROCEDURE dbo.SearchReservations
    @Date DATE = NULL,
    @FieldID INT = NULL,
    @CustomerName NVARCHAR(150) = NULL
AS
BEGIN
    SELECT *
    FROM dbo.ViewReservations
    WHERE
        (@Date IS NULL OR CAST(StartTime AS DATE) = @Date)
        AND (@FieldID IS NULL OR FieldID = @FieldID)
        AND (@CustomerName IS NULL OR CustomerName LIKE '%' + @CustomerName + '%')
    ORDER BY StartTime;
END;
GO

PRINT 'FootballDB created successfully with no errors';
GO
