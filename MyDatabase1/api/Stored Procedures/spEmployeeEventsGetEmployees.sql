CREATE PROCEDURE [api].[spEmployeeEventsGetEmployees] 
	@Id int
AS
	SELECT * FROM dbo.Employee emp WHERE emp.Id IN ( 
		SELECT EmployeeId FROM dbo.EmployeeEvents WHERE EventId = @Id
	);
