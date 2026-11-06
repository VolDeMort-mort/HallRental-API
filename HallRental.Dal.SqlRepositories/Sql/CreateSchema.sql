IF OBJECT_ID(N'dbo.Halls', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.Halls (
        Id uniqueidentifier NOT NULL,
        Name nvarchar(100) NOT NULL,
        Capacity int NOT NULL,
        PricePerHour decimal(18,2) NOT NULL,
        IsActive bit NOT NULL,
        CONSTRAINT PK_Halls PRIMARY KEY (Id)
    );
END;

IF OBJECT_ID(N'dbo.HoursPricings', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.HoursPricings (
        Id uniqueidentifier NOT NULL,
        Name nvarchar(100) NOT NULL,
        [From] time NOT NULL,
        [To] time NOT NULL,
        Multiplier decimal(18,2) NOT NULL,
        CONSTRAINT PK_HoursPricings PRIMARY KEY (Id)
    );
END;

IF OBJECT_ID(N'dbo.Services', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.Services (
        Id uniqueidentifier NOT NULL,
        Name nvarchar(100) NOT NULL,
        Price decimal(18,2) NOT NULL,
        HallId uniqueidentifier NOT NULL,
        CONSTRAINT PK_Services PRIMARY KEY (Id),
        CONSTRAINT FK_Services_Halls_HallId FOREIGN KEY (HallId) REFERENCES dbo.Halls (Id) ON DELETE CASCADE
    );
END;

IF OBJECT_ID(N'dbo.Bookings', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.Bookings (
        Id uniqueidentifier NOT NULL,
        HallId uniqueidentifier NOT NULL,
        ClientId nvarchar(200) NOT NULL,
        Start datetime2 NOT NULL,
        [End] datetime2 NOT NULL,
        RentPrice decimal(18,2) NOT NULL,
        ServicesPrice decimal(18,2) NOT NULL,
        CONSTRAINT PK_Bookings PRIMARY KEY (Id),
        -- Bookings are report history, so deleting a hall row must not take them with it
        CONSTRAINT FK_Bookings_Halls_HallId FOREIGN KEY (HallId) REFERENCES dbo.Halls (Id) ON DELETE NO ACTION
    );
END;

IF OBJECT_ID(N'dbo.BookedServices', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.BookedServices (
        BookingId uniqueidentifier NOT NULL,
        Id int NOT NULL IDENTITY,
        ServiceId uniqueidentifier NOT NULL,
        Name nvarchar(100) NOT NULL,
        Price decimal(18,2) NOT NULL,
        CONSTRAINT PK_BookedServices PRIMARY KEY (BookingId, Id),
        CONSTRAINT FK_BookedServices_Bookings_BookingId FOREIGN KEY (BookingId) REFERENCES dbo.Bookings (Id) ON DELETE CASCADE
    );
END;

IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = N'IX_Services_HallId_Name' AND object_id = OBJECT_ID(N'dbo.Services'))
    CREATE UNIQUE INDEX IX_Services_HallId_Name ON dbo.Services (HallId, Name);

IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = N'IX_Bookings_HallId' AND object_id = OBJECT_ID(N'dbo.Bookings'))
    CREATE INDEX IX_Bookings_HallId ON dbo.Bookings (HallId);

IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = N'IX_Bookings_Start' AND object_id = OBJECT_ID(N'dbo.Bookings'))
    CREATE INDEX IX_Bookings_Start ON dbo.Bookings (Start);

IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = N'IX_Bookings_ClientId' AND object_id = OBJECT_ID(N'dbo.Bookings'))
    CREATE INDEX IX_Bookings_ClientId ON dbo.Bookings (ClientId);
