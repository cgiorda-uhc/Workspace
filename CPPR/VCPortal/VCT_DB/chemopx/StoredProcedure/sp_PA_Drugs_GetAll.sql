CREATE PROCEDURE [chemopx].[sp_PA_Drugs_GetAll]
AS
	BEGIN
	SELECT
	[PA_Drugs_Id], 
    [PA_Drugs]
	FROM [chemopx].[PA_Drugs] WHERE  isnull([Is_Archived],0) <> 1
	ORDER BY [PA_Drugs];
END