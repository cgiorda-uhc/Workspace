CREATE PROCEDURE [chemopx].[sp_Source_GetAll]
AS
	SELECT DISTINCT 
      [SOURCE]
  FROM [chemopx].[ChemotherapyPX]

