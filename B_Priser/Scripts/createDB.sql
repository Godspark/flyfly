CREATE DATABASE FlyFlyDB
CREATE TABLE [dbo].[Flight]
(
	[Id] INT NOT NULL PRIMARY KEY IDENTITY, 
    [DeparturePlace] NCHAR(100) NULL, 
    [DestinationPlace] NCHAR(100) NULL, 
    [DepartureTime] DATETIME2 NULL, 
    [DestinationTime] DATETIME2 NULL, 
    [FlightTime] TIME NULL, 
    [Price] INT NULL
)