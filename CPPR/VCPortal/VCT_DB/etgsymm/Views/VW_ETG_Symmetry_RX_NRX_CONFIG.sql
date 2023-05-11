CREATE VIEW [etgsymm].[VW_ETG_Symmetry_RX_NRX_CONFIG] as
SELECT 

   f.[ETG_Base_Class] AS [Base_ETG],
  m.[ETG_Description] AS [ETG_Base_Class_Description],
  p.[Premium_Specialty] as [Premium_Specialty],


CASE WHEN f.[Has_RX] = 0 THEN 'Rx: Y / NRx: N' ELSE NULL END as [Rx],
CASE WHEN f.[Has_NRX] = 0 THEN 'Rx: Y / NRx: N' ELSE NULL END as [NRx]


	
FROM  etgsymm.ETG_Fact_Symmetry AS f 


INNER JOIN (SELECT ETG_Base_Class,Premium_Specialty_id ,'Commercial'as title  FROM etgsymm.ETG_Fact_Symmetry  UNION SELECT ETG_Base_Class,Premium_Specialty_id ,'Medicare'as title  FROM etgsymm.ETG_Fact_Symmetry UNION SELECT ETG_Base_Class,Premium_Specialty_id ,'Medicaid'as title  FROM etgsymm.ETG_Fact_Symmetry ) AS lob ON f.ETG_Base_Class = lob.ETG_Base_Class AND   f.Premium_Specialty_id = lob.Premium_Specialty_id 

LEFT OUTER JOIN vct.ETG_Dim_Master AS m ON f.ETG_Base_Class = m.ETG_Base_Class 
LEFT OUTER JOIN vct.ETG_Dim_Premium_Spec_Master AS p ON f.Premium_Specialty_id = p.Premium_Specialty_id
 WHERE f.[PC_Attribution] not in ( 'Not Mapped', 'Not Present')
AND f.[Data_Date] = (SELECT max(data_Date) FROM etgsymm.ETG_Fact_Symmetry)


AND f.[PC_Attribution]in ('Always Attributed', 'If Involved')
AND f.[Patient_Centric_Mapping]  = 'Mapped'








