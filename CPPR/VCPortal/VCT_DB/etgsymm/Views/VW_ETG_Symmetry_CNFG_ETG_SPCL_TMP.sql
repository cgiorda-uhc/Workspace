CREATE VIEW [etgsymm].[VW_ETG_Symmetry_CNFG_ETG_SPCL_TMP]
	AS 

	
	

	

	SELECT tmp.CNFG_ETG_SPCL_SYS_ID, tmp.[ETG_BASE_CLASS], tmp.TRT_CD,tmp.PREM_DESG_VER_NBR,
	
	
	--CASE WHEN tmp.PREM_SPCL_CD = 'CRD2' THEN  'CARDCD' ELSE tmp.PREM_SPCL_CD END as PREM_SPCL_CD ,
	tmp.PREM_SPCL_CD as PREM_SPCL_CD ,
	
	tmp.[NOTES] FROM (
	


	
		
	SELECT 


NULL as CNFG_ETG_SPCL_SYS_ID,

  f.[ETG_Base_Class] AS [ETG_BASE_CLASS],
 sf.[EC_Current_Treatment_Indicator] as TRT_CD,


CAST(f.PD_Version AS DECIMAL(10,2))  as PREM_DESG_VER_NBR,
p.Premium_Specialty as PREM_SPCL_CD,
 
NULL as [NOTES]
	
FROM  etgsymm.ETG_Fact_Symmetry AS f 
INNER JOIN [etgsymm].[VW_ETG_Summary_Final] sf ON f.ETG_Fact_Symmetry_Id = sf.ETG_Fact_Symmetry_Id 
LEFT OUTER JOIN vct.ETG_Dim_Premium_Spec_Master AS p ON f.Premium_Specialty_id = p.Premium_Specialty_id
 WHERE 
f.[Symmetry_Version] = (SELECT MAX(Symmetry_Version) FROM etgsymm.ETG_Fact_Symmetry)
 AND  f.ETG_Base_Class <> 000000
AND f.Premium_Specialty_Id IS NOT NULL
AND f.ETG_Fact_Symmetry_Id IN  (SELECT ETG_Fact_Symmetry_Id FROM [etgsymm].[VW_ETG_Summary_Final] WHERE  EC_Current_Mapping = 'Mapped')
--AND f.ETG_Base_Class + p.[Premium_Specialty] IN  (SELECT ETG_Base_Class + [Premium_Specialty] FROM [etgsymm].[VW_ETG_Summary_Final] WHERE  EC_Current_Mapping = 'Mapped')
AND sf.[EC_Current_Treatment_Indicator] <> '0 & 1'

UNION ALL


	
	SELECT 


NULL as CNFG_ETG_SPCL_SYS_ID,

  f.[ETG_Base_Class] AS [ETG_BASE_CLASS],
0 as TRT_CD,


CAST(f.PD_Version AS DECIMAL(10,2))  as PREM_DESG_VER_NBR,
p.Premium_Specialty as PREM_SPCL_CD,
 
NULL as [NOTES]
	
FROM  etgsymm.ETG_Fact_Symmetry AS f 
INNER JOIN [etgsymm].[VW_ETG_Summary_Final] sf ON f.ETG_Fact_Symmetry_Id = sf.ETG_Fact_Symmetry_Id 
LEFT OUTER JOIN vct.ETG_Dim_Premium_Spec_Master AS p ON f.Premium_Specialty_id = p.Premium_Specialty_id
 WHERE 
f.[Symmetry_Version] = (SELECT MAX(Symmetry_Version) FROM etgsymm.ETG_Fact_Symmetry)
 AND  f.ETG_Base_Class <> 000000
AND f.Premium_Specialty_Id IS NOT NULL
AND f.ETG_Fact_Symmetry_Id IN  (SELECT ETG_Fact_Symmetry_Id FROM [etgsymm].[VW_ETG_Summary_Final] WHERE  EC_Current_Mapping = 'Mapped')
--AND f.ETG_Base_Class + p.[Premium_Specialty] IN  (SELECT ETG_Base_Class + [Premium_Specialty] FROM [etgsymm].[VW_ETG_Summary_Final] WHERE  EC_Current_Mapping = 'Mapped')
AND sf.[EC_Current_Treatment_Indicator] = '0 & 1'


UNION ALL


	
	SELECT 


NULL as CNFG_ETG_SPCL_SYS_ID,

  f.[ETG_Base_Class] AS [ETG_BASE_CLASS],
 1 as TRT_CD,


CAST(f.PD_Version AS DECIMAL(10,2))  as PREM_DESG_VER_NBR,
p.Premium_Specialty as PREM_SPCL_CD,
 
NULL as [NOTES]
	
FROM  etgsymm.ETG_Fact_Symmetry AS f 
INNER JOIN [etgsymm].[VW_ETG_Summary_Final] sf ON f.ETG_Fact_Symmetry_Id = sf.ETG_Fact_Symmetry_Id 
LEFT OUTER JOIN vct.ETG_Dim_Premium_Spec_Master AS p ON f.Premium_Specialty_id = p.Premium_Specialty_id
 WHERE 
f.[Symmetry_Version] = (SELECT MAX(Symmetry_Version) FROM etgsymm.ETG_Fact_Symmetry)
 AND  f.ETG_Base_Class <> 000000
AND f.Premium_Specialty_Id IS NOT NULL
AND f.ETG_Fact_Symmetry_Id IN  (SELECT ETG_Fact_Symmetry_Id FROM [etgsymm].[VW_ETG_Summary_Final] WHERE  EC_Current_Mapping = 'Mapped')
--AND f.ETG_Base_Class + p.[Premium_Specialty] IN  (SELECT ETG_Base_Class + [Premium_Specialty] FROM [etgsymm].[VW_ETG_Summary_Final] WHERE  EC_Current_Mapping = 'Mapped')
AND sf.[EC_Current_Treatment_Indicator] = '0 & 1'


) tmp


WHERE 

tmp.PREM_SPCL_CD IN 
(

'ALRGY',   
'CRD2',  
'EDCRN',   
'ENT',     
'FAMED',   
'GI',      
'INTMD',   
'NEPHR',   
'NEURO',   
'NOS',     
'OBGYN',   
'PEDS',    
'PULMN',   
'RHEUM',   
'UROL'  
)

