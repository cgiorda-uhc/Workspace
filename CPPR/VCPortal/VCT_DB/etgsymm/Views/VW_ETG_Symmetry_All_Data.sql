CREATE VIEW [etgsymm].[VW_ETG_Symmetry_All_Data] as

SELECT 
f.ETG_Fact_Symmetry_id,
f.ETG_Base_Class,
m.ETG_Description,
f.Premium_Specialty_id,
p.Premium_Specialty,
CASE WHEN f.PC_Treatment_Indicator IS NULL THEN 'Not Selected' ELSE f.PC_Treatment_Indicator END  as PC_Treatment_Indicator,


CASE WHEN f.PC_Attribution IS NULL THEN 'Not Selected' ELSE f.PC_Attribution END  as PC_Attribution,



CASE WHEN f.EC_Treatment_Indicator IS NULL THEN 'Not Selected' ELSE f.EC_Treatment_Indicator END  as EC_Treatment_Indicator,


f.PC_Episode_Count,
f.PC_Total_Cost,
f.PC_Average_Cost,
f.PC_Coefficients_of_Variation,
f.PC_Normalized_Pricing_Episode_Count,
f.PC_Normalized_Pricing_Total_Cost,
f.PC_Spec_Episode_Count,
f.EC_Episode_Count, 


CASE WHEN f.[Has_NRX] = 1 THEN 'Y' ELSE 
CASE WHEN f.[Has_NRX] = 0 THEN 'N' ELSE NULL END END as [Has_NRX],

CASE WHEN f.[Has_RX] = 1 THEN 'Y' ELSE 
CASE WHEN f.[Has_RX] = 0 THEN 'N' ELSE NULL END END as [Has_RX],



CASE WHEN f.EC_Mapping IS NULL THEN 'Not Selected' ELSE f.EC_Mapping END  as Mapping,



f.EC_Change_Comments,


CASE WHEN f.Patient_Centric_Mapping IS NULL THEN 'Not Selected' ELSE f.Patient_Centric_Mapping END  as Patient_Centric_Mapping,



f.PC_Change_Comments,


CASE WHEN f.Has_Commercial is null AND f.Has_Medicare is null AND f.Has_Medicaid is null THEN 'Not Selected' ELSE 
CASE WHEN f.Has_Commercial = 1 AND f.Has_Medicare = 1 AND f.Has_Medicaid = 1 THEN 'All' ELSE 
CASE WHEN f.Has_Commercial = 1 AND f.Has_Medicare = 1 THEN 'Commercial + Medicare' ELSE
CASE WHEN f.Has_Commercial = 1 AND f.Has_Medicaid = 1 THEN 'Commercial + Medicaid' ELSE
CASE WHEN f.Has_Medicare = 1 AND f.Has_Medicaid = 1 THEN 'Medicare + Medicaid' ELSE
CASE WHEN f.Has_Commercial = 1 THEN 'Commercial Only' ELSE
CASE WHEN f.Has_Medicare = 1 THEN 'Medicare Only' ELSE
CASE WHEN f.Has_Medicaid = 1 THEN 'Medicaid Only' ELSE 'Not Selected' 
END END END END END END END END as LOBString,


f.Data_Period as Data_Period,


 f.Symmetry_Version as Symmetry_Version
FROM  etgsymm.ETG_Fact_Symmetry AS f 
LEFT OUTER JOIN vct.ETG_Dim_Master AS m ON f.ETG_Base_Class = m.ETG_Base_Class 
LEFT OUTER JOIN vct.ETG_Dim_Premium_Spec_Master AS p ON f.Premium_Specialty_id = p.Premium_Specialty_id


