--CREATE PROCEDURE [dbo].[sp_ChemotherapyPX_GetAllFilters]
--AS
--BEGIN

--SELECT Filter_Id, Filter_Desc, Filter_Type
--FROM(

--	SELECT
--	[CODE_CATEGORY_ID] as Filter_Id, 
--    [Code_Category] as Filter_Desc,
--	'Code_Category' as Filter_Type,
--	Is_Archived
--	FROM [dbo].[Code_Category] 
	
--	UNION ALL 

--	SELECT
--	0 as Filter_Id, 
--    '--Select One--' as Filter_Desc,
--	'Code_Category' as Filter_Type,
--	0 as Is_Archived

	
--	UNION ALL 


	
--	SELECT
--	[Id] as Filter_Id, 
--    [ASP_Category_Desc] as Filter_Desc,
--	'ASP_Category' as Filter_Type,
--	Is_Archived
--	FROM [dbo].[ASP_Category] 

--		UNION ALL 

--	SELECT
--	0 as Filter_Id, 
--    '--Select One--' as Filter_Desc,
--	'ASP_Category' as Filter_Type,
--	0 as Is_Archived


--	UNION ALL 

--	SELECT
--	[Id] as Filter_Id, 
--    [Drug_Adm_Mode_Desc] as Filter_Desc,
--	'Drug_Adm_Mode' as Filter_Type,
--	Is_Archived
--	FROM [dbo].[Drug_Adm_Mode] 

--		UNION ALL 

--	SELECT
--	0 as Filter_Id, 
--    '--Select One--' as Filter_Desc,
--	'Drug_Adm_Mode' as Filter_Type,
--	0 as Is_Archived

	
--	UNION ALL 

--		SELECT
--	[Id] as Filter_Id, 
--    [PA_Drugs_Desc] as Filter_Desc,
--	'PA_Drugs' as Filter_Type,
--	Is_Archived
--	FROM [dbo].[PA_Drugs] 

--		UNION ALL 

--	SELECT
--	0 as Filter_Id, 
--    '--Select One--' as Filter_Desc,
--	'PA_Drugs' as Filter_Type,
--		0 as Is_Archived

	
	
		
--	UNION ALL 

--		SELECT
--	[Id] as Filter_Id, 
--    [CEP_Pay_Cd_Desc] as Filter_Desc,
--	'CEP_Pay_Cd' as Filter_Type,
--	Is_Archived
--	FROM [dbo].[CEP_Pay_Cd] 
	
--		UNION ALL 

--	SELECT
--	0 as Filter_Id, 
--    '--Select One--' as Filter_Desc,
--	'CEP_Pay_Cd' as Filter_Type,
--		0 as Is_Archived

	
	
	
		
--	UNION ALL 

--		SELECT
--	[Id] as Filter_Id, 
--    [CEP_Enroll_Cd_Desc] as Filter_Desc,
--	'CEP_Enroll_Cd' as Filter_Type,
--	Is_Archived
--	FROM [dbo].[CEP_Enroll_Cd] 

--		UNION ALL 

--	SELECT
--	0 as Filter_Id, 
--    '--Select One--' as Filter_Desc,
--	'CEP_Enroll_Cd' as Filter_Type,
--		0 as Is_Archived





--		UNION ALL 

--		SELECT DISTINCT 
--	NULL as Filter_Id, 
--    [SOURCE] as Filter_Desc,
--	'SOURCE' as Filter_Type,
--	0 as Is_Archived
--	FROM  [dbo].[ChemotherapyPX]
--		WHERE [SOURCE] IS NOT NULL

--			UNION ALL 

--		SELECT DISTINCT 
--	NULL as Filter_Id, 
--    [CEP_ENROLL_EXCL_DESC] as Filter_Desc,
--	'CEP_ENROLL_EXCL_DESC' as Filter_Type,
--	0 as Is_Archived
--	FROM  [dbo].[ChemotherapyPX]
--	WHERE [CEP_ENROLL_EXCL_DESC] IS NOT NULL


--	) tmp
	
	
	
--	WHERE  isnull(tmp.Is_Archived,0)  <> 1
--	ORDER BY tmp.Filter_Type, tmp.Filter_Desc;

--END
