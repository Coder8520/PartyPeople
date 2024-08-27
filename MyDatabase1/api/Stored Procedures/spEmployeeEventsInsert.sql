CREATE PROCEDURE [api].[spEmployeeEventsInsert] (
	@EmployeeId int,
	@EventId int
)
AS
	INSERT INTO [dbo].[EmployeeEvents]
	VALUES (@EmployeeId, @EventId);
