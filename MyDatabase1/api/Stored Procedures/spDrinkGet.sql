CREATE PROCEDURE [api].[spDrinkGet] (
	@Id INT
)
AS
	SELECT * FROM [dbo].[Drinks] AS [D] WHERE D.Id = @Id;
