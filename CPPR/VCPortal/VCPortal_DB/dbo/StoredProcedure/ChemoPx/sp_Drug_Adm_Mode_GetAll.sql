CREATE PROCEDURE [dbo].[sp_Drug_Adm_Mode_GetAll]
AS
	BEGIN
	SELECT
	[Drug_Adm_Mode_Id], 
    [Drug_Adm_Mode]
	FROM [dbo].[Drug_Adm_Mode] WHERE  isnull([Is_Archived],0) <> 1
	ORDER BY [Drug_Adm_Mode];
END
