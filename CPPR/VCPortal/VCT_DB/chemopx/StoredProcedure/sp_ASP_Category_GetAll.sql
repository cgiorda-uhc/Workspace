CREATE PROCEDURE [chemopx].[sp_ASP_Category_GetAll]

AS
	BEGIN
	SELECT
	[ASP_Category_Id], 
    [ASP_Category]
	FROM [chemopx].[ASP_Category] WHERE  isnull([Is_Archived],0) <> 1
	ORDER BY [ASP_Category];
END