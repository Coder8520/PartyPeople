
CREATE   PROC [api].[spEmployeeGet]
(
    @Id [int]
)
AS
BEGIN
    SELECT *
    FROM    [dbo].[Employee] AS [E]
    WHERE   [E].[Id] = @Id;
END;