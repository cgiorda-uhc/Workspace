CREATE PROCEDURE [dbo].[sp_CEP_Pay_Cd_GetAll]
AS
	BEGIN
	SELECT
	[CEP_Pay_Cd_Id], 
    [CEP_Pay_Cd]
	FROM [dbo].[CEP_Pay_Cd] WHERE  isnull([Is_Archived],0) <> 1
	ORDER BY [CEP_Pay_Cd];
END
