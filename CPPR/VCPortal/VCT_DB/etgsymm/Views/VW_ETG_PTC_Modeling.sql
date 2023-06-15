CREATE VIEW [etgsymm].[VW_ETG_PTC_Modeling]
	AS 

	SELECT 

   f.[ETG_Base_Class] AS [ETG_Base_Class],
  m.[ETG_Description] AS [ETG_Description],
  p.[Premium_Specialty] as [Premium_Specialty],
  pm.[PREMIUM_ABBRV] as [PC_Modeling_Specialty],
  
    CASE  WHEN f.[PC_Attribution] = 'Always Attributed'  THEN 'Y' ELSE 'N' END as [Always_Attributed],


    CASE  WHEN f.[PC_Attribution] = 'If Involved'  THEN 'Y' ELSE 'N' END as [If_Attributed],


	    CASE WHEN f.[ETG_Base_Class] in (162400, 163100, 163300, 163700, 164100, 207200, 207300, 207600, 207800, 207900, 315300, 315400, 350800, 350900, 404300, 440000, 440100, 474000, 475400, 475500, 477800, 522400, 522500, 588200, 588600, 634400, 634500, 634600, 635600, 668000, 668100, 713600, 713800, 713900) THEN '0' ELSE 'All' END  as  TRT_CD,




CASE WHEN f.[Has_RX] = 1 THEN 'Y' ELSE 'N' END as [Rx],


CASE WHEN f.[Has_NRX] = 1 THEN 'Y' ELSE  'N'END as  [NRx],


	lob.title as Risk_Model
	
FROM  etgsymm.ETG_Fact_Symmetry AS f 

INNER JOIN (SELECT ETG_Base_Class,Premium_Specialty_id ,'Commercial'as title  FROM etgsymm.ETG_Fact_Symmetry  UNION SELECT ETG_Base_Class,Premium_Specialty_id ,'Medicaid'as title  FROM etgsymm.ETG_Fact_Symmetry ) AS lob ON f.ETG_Base_Class = lob.ETG_Base_Class AND   f.Premium_Specialty_id = lob.Premium_Specialty_id 

LEFT OUTER JOIN vct.ETG_Dim_Master AS m ON f.ETG_Base_Class = m.ETG_Base_Class 
LEFT OUTER JOIN vct.ETG_Dim_Premium_Spec_Master AS p ON f.Premium_Specialty_id = p.Premium_Specialty_id
LEFT OUTER JOIN vct.Premium_Mapping AS pm ON pm.PREMIUM= p.Premium_Specialty
 WHERE 
 
 --f.[PC_Attribution] not in ( 'Not Mapped', 'Not Present')
f.[PD_Version] = (SELECT max(PD_Version) FROM etgsymm.ETG_Fact_Symmetry)
AND f.[PC_Attribution]in ('Always Attributed', 'If Involved')
--AND f.[Patient_Centric_Mapping]  = 'Mapped'