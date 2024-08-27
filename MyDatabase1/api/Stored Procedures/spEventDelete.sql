
CREATE   PROC [api].[spEventDelete]
(
    @Id [int]
)
AS
BEGIN
    DELETE [E]
    FROM    [dbo].[Event] AS [E]
    WHERE   [E].[Id] = @Id;

	DELETE [EE] FROM [dbo].[EmployeeEvents] AS [EE]
	WHERE [EE].[EventId] = @Id;
END;