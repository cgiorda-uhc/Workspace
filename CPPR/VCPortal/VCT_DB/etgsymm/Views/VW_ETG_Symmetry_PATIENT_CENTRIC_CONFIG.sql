CREATE VIEW [etgsymm].[VW_ETG_Symmetry_PATIENT_CENTRIC_CONFIG] as

SELECT 

   f.[ETG_Base_Class] AS [Base_ETG],
  m.[ETG_Description] AS [ETG_Base_Class_Description],
  p.[Premium_Specialty] as [Premium_Specialty],




    CASE WHEN f.[ETG_Base_Class] in (162400, 163100, 163300, 163700, 164100, 207200, 207300, 207600, 207800, 207900, 315300, 315400, 350800, 350900, 404300, 440000, 440100, 474000, 475400, 475500, 477800, 522400, 522500, 588200, 588600, 634400, 634500, 634600, 635600, 668000, 668100, 713600, 713800, 713900) THEN '0' ELSE 'All' END  as  TRT_CD,


CASE WHEN f.[Has_RX] = 1 AND f.[Has_NRX] = 1 THEN 'Rx: Y / NRx: Y' ELSE 
CASE WHEN f.[Has_RX] = 1 AND f.[Has_NRX] = 0 THEN 'Rx: Y / NRx: N' ELSE 
CASE WHEN f.[Has_RX] = 0 AND f.[Has_NRX] = 1 THEN 'Rx: N / NRx: Y' ELSE 
CASE WHEN f.[Has_RX] = 0 AND f.[Has_NRX] = 0 THEN 'Rx: N / NRx: N' ELSE 
NULL  END END END END as [Current_Rx_NRx],

	lob.title as Risk_Model,

	 f.Patient_Centric_Mapping as Current_Pt_Centric_Mapping,
	f.Patient_Centric_Change_Comments as Pt_Centric_Change_Comments
	
FROM  etgsymm.ETG_Fact_Symmetry AS f 

INNER JOIN (SELECT ETG_Base_Class,Premium_Specialty_id ,'Commercial'as title  FROM etgsymm.ETG_Fact_Symmetry  UNION SELECT ETG_Base_Class,Premium_Specialty_id ,'Medicare'as title  FROM etgsymm.ETG_Fact_Symmetry UNION SELECT ETG_Base_Class,Premium_Specialty_id ,'Medicaid'as title  FROM etgsymm.ETG_Fact_Symmetry ) AS lob ON f.ETG_Base_Class = lob.ETG_Base_Class AND   f.Premium_Specialty_id = lob.Premium_Specialty_id 

LEFT OUTER JOIN vct.ETG_Dim_Master AS m ON f.ETG_Base_Class = m.ETG_Base_Class 
LEFT OUTER JOIN vct.ETG_Dim_Premium_Spec_Master AS p ON f.Premium_Specialty_id = p.Premium_Specialty_id
 WHERE f.[PC_Attribution] not in ( 'Not Mapped', 'Not Present')
AND f.[Data_Date] = (SELECT max(data_Date) FROM etgsymm.ETG_Fact_Symmetry)
AND f.[EC_Mapping]  = 'Mapped'
AND f.[Patient_Centric_Mapping] = 'Yes'
