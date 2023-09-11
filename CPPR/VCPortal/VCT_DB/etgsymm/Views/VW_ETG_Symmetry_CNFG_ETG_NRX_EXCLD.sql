CREATE VIEW etgsymm.[VW_ETG_Symmetry_CNFG_ETG_NRX_EXCLD]
	AS 

	SELECT distinct


NULL as CNFG_ETG_NRX_EXCLD_SYS_ID,

CAST(f.PD_Version AS DECIMAL(10,2))  as PREM_DESG_VER_NBR,

   f.[ETG_Base_Class] AS [ETG_BASE_CLASS],
NULL as [NOTES]
	
FROM  etgsymm.ETG_Fact_Symmetry AS f 

 WHERE 
 f.PD_Version = (SELECT max(PD_Version) FROM etgsymm.ETG_Fact_Symmetry)
 --AND [Has_NRX] = 0
  AND  f.ETG_Base_Class <> 000000
AND f.Premium_Specialty_Id IS NOT NULL

AND f.EC_Mapping = 'Mapped'
AND f.Has_NRX = 0
and f.Has_RX  =1

--AND f.ETG_Fact_Symmetry_Id IN  (SELECT ETG_Fact_Symmetry_Id FROM [etgsymm].[VW_ETG_Summary_Final] WHERE [UGAP_Changes] <> 'Not Mapped')

