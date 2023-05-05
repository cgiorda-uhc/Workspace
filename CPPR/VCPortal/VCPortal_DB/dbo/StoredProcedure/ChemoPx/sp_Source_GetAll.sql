CREATE PROCEDURE [dbo].[sp_Source_GetAll]
AS
	SELECT DISTINCT 
      [SOURCE]
  FROM .[dbo].[ChemotherapyPX]

