
CREATE   PROC [api].[spEmployeeDelete]
(
    @Id [int]
)
AS
BEGIN
    DELETE [E]
    FROM    [dbo].[Employee] AS [E]
    WHERE   [E].[Id] = @Id;

	DELETE [EE] 
	FROM [dbo].[EmployeeEvents] AS [EE]
	WHERE [EE].[EmployeeId] = @Id;
END;