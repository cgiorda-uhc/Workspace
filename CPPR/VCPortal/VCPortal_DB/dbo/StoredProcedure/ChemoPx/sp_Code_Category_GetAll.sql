CREATE PROCEDURE [dbo].[sp_Code_Category_GetAll]
AS
	BEGIN
	SELECT
	[CODE_CATEGORY_ID], 
    [Code_Category]
	FROM [dbo].[Code_Category] WHERE  isnull([Is_Archived],0) <> 1
	ORDER BY [Code_Category];
END

