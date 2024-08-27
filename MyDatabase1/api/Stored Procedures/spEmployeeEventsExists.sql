CREATE PROCEDURE [api].[spEmployeeEventsExists] (
	@EmployeeId int,
	@EventId int
)
AS
	    SELECT  CAST(CASE 
			WHEN EXISTS (
				SELECT 1
				FROM   [dbo].[EmployeeEvents] AS [EE]
				WHERE  [EE].[EmployeeId] = @EmployeeId AND [EE].[EventId] = @EventId
			) 
				THEN 1 ELSE 0
            END AS bit);
