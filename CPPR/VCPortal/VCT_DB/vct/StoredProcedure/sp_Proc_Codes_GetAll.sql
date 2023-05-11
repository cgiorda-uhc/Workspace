CREATE PROCEDURE [vct].[sp_Proc_Codes_GetAll]

AS
BEGIN

SELECT Proc_Cd, Proc_Desc, Proc_Cd_Type, Proc_Cd_Date FROM vct.Proc_Codes 
WHERE Proc_Cd not in (SELECT CODE FROM chemopx.ChemotherapyPX WHERE isnull(is_archived,0) = 0)

ORDER BY Proc_Cd


END
