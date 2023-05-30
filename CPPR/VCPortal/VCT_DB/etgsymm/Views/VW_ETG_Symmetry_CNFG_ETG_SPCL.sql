CREATE VIEW etgsymm.[VW_ETG_Symmetry_CNFG_ETG_SPCL]
	AS 
	
	
	SELECT 


NULL as CNFG_ETG_SPCL_SYS_ID,

  f.[ETG_Base_Class] AS [ETG_BASE_CLASS],
 f.[PC_Treatment_Indicator]  as TRT_CD,


CAST(f.PD_Version AS DECIMAL(10,2))  as PREM_DESG_VER_NBR,
p.Premium_Specialty as PREM_SPCL_CD,
 
NULL as [NOTES]
	
FROM  etgsymm.ETG_Fact_Symmetry AS f 
LEFT OUTER JOIN vct.ETG_Dim_Premium_Spec_Master AS p ON f.Premium_Specialty_id = p.Premium_Specialty_id
 WHERE 
 f.PD_Version = (SELECT max(PD_Version) FROM etgsymm.ETG_Fact_Symmetry)
 AND  f.[PC_Treatment_Indicator] = '0'
 AND  f.ETG_Base_Class <> 000000
AND f.Premium_Specialty_Id IS NOT NULL
