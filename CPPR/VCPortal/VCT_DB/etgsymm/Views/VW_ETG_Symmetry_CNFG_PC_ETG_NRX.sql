CREATE VIEW etgsymm.[VW_ETG_Symmetry_CNFG_PC_ETG_NRX]
	AS
	

				SELECT 
	DISTINCT

NULL as CNFG_PC_ETG_NRX_SYS_ID,

  f.[ETG_Base_Class] AS [ETG_BASE_CLASS],


    m.[TRT_CD] as  TRT_CD,
--CASE WHEN f.[PC_Treatment_Indicator] = 'All' THEN  m.[TRT_CD] ELSE f.[PC_Treatment_Indicator] END  as TRT_CD,
 'Y' as NRX,

CAST(f.PD_Version AS DECIMAL(10,2))  as PREM_DESG_VER_NBR,

NULL as [NOTES]
	
FROM  etgsymm.ETG_Fact_Symmetry AS f 

INNER JOIN (SELECT DISTINCT[ETG_BAS_CLSS_NBR],[TRT_CD],[NRX] FROM [etgsymm].[VW_ETG_Symmetry_UGAP CNFG] WHERE NRX = 'Y') m ON f.ETG_Base_Class = m.[ETG_BAS_CLSS_NBR]



 WHERE 
 f.PD_Version = (SELECT max(PD_Version) FROM etgsymm.ETG_Fact_Symmetry)
 AND  f.Has_NRX = 1
 AND  f.ETG_Base_Class <> 000000
AND f.Premium_Specialty_Id IS NOT NULL
AND f.ETG_Fact_Symmetry_Id IN  (SELECT ETG_Fact_Symmetry_Id FROM [etgsymm].[VW_ETG_Summary_Final_PTC] WHERE [UGAP_Changes] <> 'Not Mapped' AND PC_Current_Attribution in ('If Involved', 'Always Attributed'))