CREATE TABLE [dbo].[EmployeeEvents] (
    [EmployeeId] INT NOT NULL,
    [EventId]    INT NOT NULL,
    FOREIGN KEY ([EmployeeId]) REFERENCES [dbo].[Employee] ([Id]),
    FOREIGN KEY ([EventId]) REFERENCES [dbo].[Event] ([Id])
);

