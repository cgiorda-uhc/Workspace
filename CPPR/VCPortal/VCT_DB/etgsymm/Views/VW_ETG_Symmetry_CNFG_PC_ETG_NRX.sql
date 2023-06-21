CREATE VIEW etgsymm.[VW_ETG_Symmetry_CNFG_PC_ETG_NRX]
	AS
	

		SELECT 
	DISTINCT

NULL as CNFG_PC_ETG_NRX_SYS_ID,

  f.[ETG_Base_Class] AS [ETG_BASE_CLASS],
CASE WHEN f.[PC_Treatment_Indicator] = 'All' THEN  m.[Treatment_Indicator] ELSE f.[PC_Treatment_Indicator] END  as TRT_CD,
 'Y' as NRX,

CAST(f.PD_Version AS DECIMAL(10,2))  as PREM_DESG_VER_NBR,

NULL as [NOTES]
	
FROM  etgsymm.ETG_Fact_Symmetry AS f 

LEFT JOIN [vct].[ETG_TI_Mapping] m ON f.ETG_Base_Class = m.[Base_ETG]  AND f.[PC_Treatment_Indicator] = 'All'



 WHERE 
 f.PD_Version = (SELECT max(PD_Version) FROM etgsymm.ETG_Fact_Symmetry)
 AND  f.Has_NRX = 1
 AND  f.ETG_Base_Class <> 000000
AND f.Premium_Specialty_Id IS NOT NULL
AND f.PC_Attribution in ('If Involved', 'Always Attributed')
