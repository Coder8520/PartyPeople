CREATE TABLE [dbo].[Employee] (
    [Id]          INT            IDENTITY (1, 1) NOT NULL,
    [FirstName]   NVARCHAR (255) NOT NULL,
    [LastName]    NVARCHAR (255) NOT NULL,
    [DateOfBirth] DATE           NOT NULL,
    [DrinkId]     INT            NULL,
    PRIMARY KEY CLUSTERED ([Id] ASC),
    FOREIGN KEY ([DrinkId]) REFERENCES [dbo].[Drinks] ([Id])
);

