CREATE PROCEDURE [api].[spEmployeeEventsDelete] (
	@EmployeeId int,
	@EventId int
)
AS
BEGIN
	DELETE FROM [dbo].[EmployeeEvents]
	WHERE EmployeeId = @EmployeeId AND EventId = @EventId;
END
