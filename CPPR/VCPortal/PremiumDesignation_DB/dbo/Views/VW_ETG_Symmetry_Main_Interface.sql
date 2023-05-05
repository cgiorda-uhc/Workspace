CREATE VIEW [dbo].[VW_ETG_Symmetry_Main_Interface] as

SELECT 
f.ETG_Fact_Symmetry_Id,
fp.ETG_Fact_Symmetry_Id as ETG_Fact_Symmetry_Id_Previous,
f.ETG_Base_Class,
m.ETG_Description,
f.Premium_Specialty_Id,
p.Premium_Specialty,


f.Has_Commercial,
f.Has_Medicare,
f.Has_Medicaid, 

CASE WHEN f.Has_Commercial is null AND f.Has_Medicare is null AND f.Has_Medicaid is null THEN 'Not Mapped' ELSE 
CASE WHEN f.Has_Commercial = 1 AND f.Has_Medicare = 1 AND f.Has_Medicaid = 1 THEN 'All' ELSE 
CASE WHEN f.Has_Commercial = 1 AND f.Has_Medicare = 1 THEN 'Commercial + Medicare' ELSE
CASE WHEN f.Has_Commercial = 1 AND f.Has_Medicaid = 1 THEN 'Commercial + Medicaid' ELSE
CASE WHEN f.Has_Medicare = 1 AND f.Has_Medicaid = 1 THEN 'Medicare + Medicaid' ELSE
CASE WHEN f.Has_Commercial = 1 THEN 'Commercial Only' ELSE
CASE WHEN f.Has_Medicare = 1 THEN 'Medicare Only' ELSE
CASE WHEN f.Has_Medicaid = 1 THEN 'Medicaid Only' ELSE 'Not Mapped' 
END END END END END END END END as LOB,


fp.Has_Commercial as Has_Commercial_Previous,
fp.Has_Medicare as Has_Medicare_Previous,
fp.Has_Medicaid as Has_Medicaid_Previous, 



CASE WHEN fp.Has_Commercial is null AND fp.Has_Medicare is null AND fp.Has_Medicaid is null THEN 'Not Mapped' ELSE 
CASE WHEN fp.Has_Commercial = 1 AND fp.Has_Medicare = 1 AND fp.Has_Medicaid = 1 THEN 'All' ELSE 
CASE WHEN fp.Has_Commercial = 1 AND fp.Has_Medicare = 1 THEN 'Commercial + Medicare' ELSE
CASE WHEN fp.Has_Commercial = 1 AND fp.Has_Medicaid = 1 THEN 'Commercial + Medicaid' ELSE
CASE WHEN fp.Has_Medicare = 1 AND fp.Has_Medicaid = 1 THEN 'Medicare + Medicaid' ELSE
CASE WHEN fp.Has_Commercial = 1 THEN 'Commercial Only' ELSE
CASE WHEN fp.Has_Medicare = 1 THEN 'Medicare Only' ELSE
CASE WHEN fp.Has_Medicaid = 1 THEN 'Medicaid Only' ELSE 'Not Mapped' 
END END END END END END END END as LOBPrevious,

CASE WHEN f.[Has_RX] = 1 AND f.[Has_NRX] = 1 THEN 'Rx: Y / NRx: Y' ELSE 
CASE WHEN f.[Has_RX] = 1 AND f.[Has_NRX] = 0 THEN 'Rx: Y / NRx: N' ELSE 
CASE WHEN f.[Has_RX] = 0 AND f.[Has_NRX] = 1 THEN 'Rx: N / NRx: Y' ELSE 
CASE WHEN f.[Has_RX] = 0 AND f.[Has_NRX] = 0 THEN 'Rx: N / NRx: N' ELSE 
'Not Mapped'  END END END END as RX_NRX,


f.[Has_RX],
f.[Has_NRX],



CASE WHEN fp.[Has_NRX] = 1 AND fp.[Has_RX] = 0 THEN 'Rx: N / NRx: Y' ELSE 
CASE WHEN fp.[Has_NRX] = 1 AND fp.[Has_RX] = 1 THEN 'Rx: Y / NRx: Y' ELSE 
CASE WHEN fp.[Has_NRX] = 0 AND fp.[Has_RX] = 0 THEN 'Rx: N / NRx: N' ELSE 
CASE WHEN fp.[Has_NRX] = 0 AND fp.[Has_RX] = 1 THEN 'Rx: Y / NRx: N' ELSE 
NULL END END END END as RX_NRXPrevious,


fp.[Has_RX] as Has_RX_Previous,
fp.[Has_NRX]as Has_NRX_Previous,


 CASE WHEN f.[PC_Attribution] not in ( 'Not Mapped', 'Not Present')
AND  (f.Has_Commercial is not null or f.Has_Medicare is not null or f.Has_Medicaid is not null)
AND f.[PC_Treatment_Indicator] in ('All', '0')
AND f.[PC_Attribution] in ('Always Attributed', 'If Involved')
AND f.[EC_Treatment_Indicator] <> 'Not Mapped' AND f.[EC_Treatment_Indicator] IS NOT NULL
AND f.[EC_Mapping]  = 'Mapped' THEN 'Y' ELSE 'N' END as Is_Config,


CASE WHEN f.[ETG_Base_Class] in (162400, 163100, 163300, 163700, 164100, 207200, 207300, 207600, 207800, 207900, 315300, 315400, 350800, 350900, 404300, 440000, 440100, 474000, 475400, 475500, 477800, 522400, 522500, 588200, 588600, 634400, 634500, 634600, 635600, 668000, 668100, 713600, 713800, 713900) THEN '0' ELSE 'All' END  as  PC_Treatment_Indicator,



CASE WHEN fp.[ETG_Base_Class] in (162400, 163100, 163300, 163700, 164100, 207200, 207300, 207600, 207800, 207900, 315300, 315400, 350800, 350900, 404300, 440000, 440100, 474000, 475400, 475500, 477800, 522400, 522500, 588200, 588600, 634400, 634500, 634600, 635600, 668000, 668100, 713600, 713800, 713900) THEN '0' ELSE 'All' END  as  PC_Treatment_Indicator_Previous,


CASE WHEN f.PC_Attribution IS NULL THEN 'Not Mapped' ELSE f.PC_Attribution END  as PC_Attribution,
CASE WHEN fp.PC_Attribution IS NULL THEN 'Not Mapped' ELSE fp.PC_Attribution END as PC_Attribution_Previous,



f.PC_Episode_Count,
f.PC_Total_Cost,
f.PC_Average_Cost,
f.PC_Coefficients_of_Variation,

CASE WHEN f.PC_Normalized_Pricing_Episode_Count IS NOT NULL THEN f.PC_Normalized_Pricing_Episode_Count ELSE 0 END as PC_Normalized_Pricing_Episode_Count,
f.PC_Normalized_Pricing_Total_Cost,
CASE WHEN f.PC_Spec_Episode_Count IS NOT NULL THEN f.PC_Spec_Episode_Count ELSE 0 END as PC_Spec_Episode_Count,

f.[PC_Spec_Episode_Distribution],
f.[PC_Spec_Percent_of_Episodes],

f.[PC_Spec_Total_Cost],
f.[PC_Spec_Average_Cost],
f.[PC_Spec_CV],

 
CASE WHEN fp.PC_Attribution= 'Not Mapped' AND f.PC_Attribution  <> 'Not Mapped' AND fp.Patient_Centric_Mapping= 'Not Mapped' AND f.Patient_Centric_Mapping = 'Mapped' THEN 'Added'
ELSE
CASE WHEN fp.PC_Attribution <> 'Not Mapped' AND f.PC_Attribution  = 'Not Mapped' AND fp.Patient_Centric_Mapping= 'Mapped' AND f.Patient_Centric_Mapping = 'Not Mapped' THEN 'Removed'
ELSE
CASE WHEN (f.PC_Attribution  <> 'Not Mapped'AND f.PC_Attribution = 'Not Mapped') OR (f.Patient_Centric_Mapping = 'Not Mapped'  AND f.Patient_Centric_Mapping= 'Mapped')  THEN 'Inconsistent Mapping'
ELSE
CASE WHEN fp.PC_Attribution= f.PC_Attribution   AND fp.Patient_Centric_Mapping=  f.Patient_Centric_Mapping THEN 'No Change'
END END END END as PC_Measure_Status,


CASE WHEN f.[Patient_Centric_Mapping] = 'Always Attributed' AND fp.[Patient_Centric_Mapping] = 'Always Attributed' THEN 'No Change' ELSE
CASE WHEN f.[Patient_Centric_Mapping] = 'If Involved' AND fp.[Patient_Centric_Mapping] = 'Always Attributed' THEN 'Attribution Altered' ELSE
CASE WHEN f.[Patient_Centric_Mapping] = 'Not Mapped' AND fp.[Patient_Centric_Mapping] = 'Always Attributed' THEN 'Attribution Removed' ELSE
CASE WHEN f.[Patient_Centric_Mapping] = 'Always Attributed' AND fp.[Patient_Centric_Mapping] = 'If Involved' THEN 'Attribution Altered' ELSE
CASE WHEN f.[Patient_Centric_Mapping] = 'If Involved' AND fp.[Patient_Centric_Mapping] = 'If Involved' THEN 'No Change' ELSE
CASE WHEN f.[Patient_Centric_Mapping] = 'Not Mapped' AND fp.[Patient_Centric_Mapping] = 'If Involved' THEN 'Attribution Removed' ELSE
CASE WHEN f.[Patient_Centric_Mapping] = 'Always Attributed' AND fp.[Patient_Centric_Mapping] = 'Not Mapped' THEN 'Attribution Added' ELSE 
CASE WHEN f.[Patient_Centric_Mapping] = 'If Involved' AND fp.[Patient_Centric_Mapping] = 'Not Mapped' THEN 'Attribution Added' ELSE 
CASE WHEN f.[Patient_Centric_Mapping] = 'Not Mapped' AND fp.[Patient_Centric_Mapping] = 'Not Mapped' THEN 'No Change' ELSE NULL
END  END  END  END  END  END  END END END as [PC_Changes_Made],


f.PC_Change_Comments,



CASE WHEN f.Patient_Centric_Mapping IS NULL THEN 'No' ELSE f.Patient_Centric_Mapping END  as Patient_Centric_Mapping,
CASE WHEN fp.Patient_Centric_Mapping IS NULL THEN 'No' ELSE fp.Patient_Centric_Mapping END as Patient_Centric_Mapping_Previous,

f.Patient_Centric_Change_Comments,




 CASE WHEN f.[ETG_Base_Class] in (668200, 601100) THEN '0 & 1' ELSE '0' END  as  EC_Treatment_Indicator,
 CASE WHEN fp.[ETG_Base_Class] in (668200, 601100) THEN '0 & 1' ELSE '0' END  as  EC_Treatment_Indicator_Previous,

f.[EC_Spec_Episode_Distribution],
f.[EC_Spec_Percent_of_Episodes],

f.[EC_Spec_Total_Cost], 
f.[EC_Spec_Average_Cost], 
f.[EC_Coefficients_of_Variation],

f.[EC_Normalized_Pricing_Episode_Count] as EC_Episode_Count,
f.[EC_Normalized_Pricing_Total_Cost],

CASE WHEN f.EC_Episode_Count IS NOT NULL THEN f.EC_Episode_Count ELSE 0 END as EC_Spec_Episode_Count,
f.[EC_Total_Cost],
f.[EC_Average_Cost],
f.[EC_Spec_CV],



CASE WHEN f.[EC_Mapping] = 'Mapped' AND fp.[EC_Mapping] = 'ADD' THEN 'No Change' ELSE
CASE WHEN f.[EC_Mapping] = 'Mapped' AND fp.[EC_Mapping] = 'KEEP' THEN 'No Change' ELSE
CASE WHEN f.[EC_Mapping] = 'Mapped' AND fp.[EC_Mapping] = 'DROP' THEN 'Added' ELSE
CASE WHEN f.[EC_Mapping] = 'Mapped' AND fp.[EC_Mapping] = 'Not Mapped' THEN 'Added' ELSE
CASE WHEN f.[EC_Mapping] = 'Not Mapped' AND fp.[EC_Mapping] = 'ADD' THEN 'Removed' ELSE
CASE WHEN f.[EC_Mapping] = 'Not Mapped' AND fp.[EC_Mapping] = 'KEEP' THEN 'Removed' ELSE
CASE WHEN f.[EC_Mapping] = 'Not Mapped' AND fp.[EC_Mapping] = 'DROP' THEN 'No Change' ELSE 
CASE WHEN f.[EC_Mapping] = 'Not Mapped' AND fp.[EC_Mapping] = 'Not Mapped' THEN 'No Change' ELSE NULL
END  END  END  END  END  END  END END as [EC_Changes_Made],


CASE WHEN f.EC_Mapping IS NULL THEN 'Not Mapped' ELSE f.EC_Mapping END  as EC_Mapping,
CASE WHEN fp.EC_Mapping IS NULL THEN 'Not Mapped' ELSE fp.EC_Mapping END as EC_Mapping_Previous,

--CASE WHEN f.EC_Mapping IS NULL THEN 'Not Mapped' ELSE f.EC_Mapping END  as Current_Mapping_Original,
--CASE WHEN fp.EC_Mapping IS NULL THEN 'Not Mapped' ELSE fp.EC_Mapping END as Previous_Mapping_Original,

f.EC_Change_Comments,



f.Data_Period as Data_Period,
fp.Data_Period as Data_Period_Previous,
 f.Symmetry_Version as Symmetry_Version,
  fp.Symmetry_Version as Symmetry_Version_Previous



FROM  (SELECT * FROM dbo.ETG_Fact_Symmetry WHERE Symmetry_Version = (SELECT max(Symmetry_Version) FROM dbo.ETG_Fact_Symmetry)) AS f 
LEFT OUTER JOIN (SELECT * FROM dbo.ETG_Fact_Symmetry WHERE Symmetry_Version = (SELECT max(Symmetry_Version) FROM dbo.ETG_Fact_Symmetry WHERE Symmetry_Version<(SELECT MAX(Symmetry_Version) FROM dbo.ETG_Fact_Symmetry))) AS fp ON f.ETG_Base_Class = fp.ETG_Base_Class AND f.Premium_Specialty_id  = fp.Premium_Specialty_id 
LEFT OUTER JOIN dbo.ETG_Dim_Master AS m ON f.ETG_Base_Class = m.ETG_Base_Class 
LEFT OUTER JOIN dbo.ETG_Dim_Premium_Spec_Master AS p ON f.Premium_Specialty_id = p.Premium_Specialty_id


WHERE f.ETG_Base_Class <> 000000
