CREATE PROCEDURE [dbo].[sp_CEP_Enroll_Excl_Desc_GetAll]
AS
	SELECT DISTINCT 
      [CEP_ENROLL_EXCL_DESC]
  FROM .[dbo].[ChemotherapyPX]
