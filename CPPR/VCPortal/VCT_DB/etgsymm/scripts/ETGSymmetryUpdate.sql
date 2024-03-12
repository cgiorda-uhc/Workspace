
--SELECT * INTO  [VCT_DB].[vct].[ETG_Fact_Symmetry_BAK31224]
--FROM  [VCT_DB].[etgsymm].[ETG_Fact_Symmetry]


UPDATE
   Table_A
SET

Table_A.[Never_Mapped] = CASE WHEN Table_B.[Never_Map_Previous] = 'Y' THEN 1 ELSE 0 END, 
Table_A.[Has_RX] = CASE WHEN LOWER(REPLACE(Table_B.[Previous_Rx_NRx], ' ', '')) LIKE  'rx:y/%'  THEN 1 ELSE 0 END, 
Table_A.[Has_NRX] = CASE WHEN LOWER(REPLACE(Table_B.[Previous_Rx_NRx], ' ', '')) LIKE '%/nrx:y'  THEN 1 ELSE 0 END, 


Table_A.[Has_Commercial] = CASE WHEN LOWER(REPLACE(Table_B.[Previous_LOB], ' ', ''))  = 'notmapped' then NULL ELSE 
CASE WHEN LOWER(REPLACE(Table_B.[Previous_LOB], ' ', '')) LIKE  '%commercial%' OR  LOWER(REPLACE(Table_B.[Previous_LOB], ' ', '')) = 'all' THEN 1 
ELSE 0 END END, 


Table_A.[Has_Medicare] = CASE WHEN LOWER(REPLACE(Table_B.[Previous_LOB], ' ', ''))  = 'notmapped' then NULL ELSE 
CASE WHEN LOWER(REPLACE(Table_B.[Previous_LOB], ' ', '')) LIKE  '%medicare%' OR  LOWER(REPLACE(Table_B.[Previous_LOB], ' ', '')) = 'all' THEN 1 
ELSE 0 END END, 


Table_A.[Has_Medicaid] = CASE WHEN LOWER(REPLACE(Table_B.[Previous_LOB], ' ', ''))  = 'notmapped' then NULL ELSE 
CASE WHEN LOWER(REPLACE(Table_B.[Previous_LOB], ' ', '')) LIKE  '%medicaid%' OR  LOWER(REPLACE(Table_B.[Previous_LOB], ' ', '')) = 'all' THEN 1 
ELSE 0 END END, 



Table_A.[PC_Treatment_Indicator] = Table_B.[PC_Previous_Treatment_Indicator],

Table_A.[PC_Attribution] = Table_B.[PC_Previous_Attribution]


FROM
    [VCT_DB].[etgsymm].[ETG_Fact_Symmetry] AS Table_A
       INNER JOIN  [VCT_DB].vct.ETG_Dim_Premium_Spec_Master AS Table_C 
       ON Table_A.Premium_Specialty_id = Table_C.Premium_Specialty_id
    INNER JOIN [VCPostDeploy].[dbo].[PD18_Mapping_Symmetry] AS Table_B 
       ON Table_A.[ETG_Base_Class] = Table_B.[ETG_Base_Class] AND Table_C.[Premium_Specialty] = Table_B.[Premium_Specialty]
       WHERE Table_A.PD_Version = 17
