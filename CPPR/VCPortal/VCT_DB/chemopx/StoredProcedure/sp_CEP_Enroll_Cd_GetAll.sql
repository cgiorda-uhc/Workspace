CREATE PROCEDURE [chemopx].[sp_CEP_Enroll_Cd_GetAll]
AS
	BEGIN
	SELECT
	[CEP_Enroll_Cd_Id], 
    [CEP_Enroll_Cd]
	FROM [chemopx].[CEP_Enroll_Cd] WHERE  isnull([Is_Archived],0) <> 1 
	ORDER BY [CEP_Enroll_Cd];
END
