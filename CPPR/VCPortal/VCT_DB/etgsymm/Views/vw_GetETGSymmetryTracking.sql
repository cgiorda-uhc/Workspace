CREATE VIEW [etgsymm].[vw_GetETGSymmetryTracking]
	AS 
	SELECT [ETG_Fact_Symmetry_UT_id] as Tracker_Id
,f.ETG_Base_Class
,m.[ETG_Description]
, p.Premium_Specialty
	,CASE WHEN t.[Has_Commercial] = 0 THEN 'N' ELSE CASE WHEN t.[Has_Commercial] = 1 THEN 'Y' ELSE NULL END END AS [Has_Commercial]
	,CASE WHEN t.[Has_Commercial_Previous] = 0 THEN 'N' ELSE CASE WHEN t.[Has_Commercial_Previous] = 1 THEN 'Y' ELSE NULL END END AS [Has_Commercial_Previous]
	,CASE WHEN t.[Has_Medicare] = 0 THEN 'N' ELSE CASE WHEN t.[Has_Medicare] = 1 THEN 'Y' ELSE NULL END END AS [Has_Medicare]
	,CASE WHEN t.[Has_Medicare_Previous] = 0 THEN 'N' ELSE CASE WHEN [Has_Medicare_Previous] = 1 THEN 'Y' ELSE NULL END END AS [Has_Medicare_Previous]
	,CASE WHEN t.[Has_Medicaid] = 0 THEN 'N' ELSE CASE WHEN t.[Has_Medicaid] = 1 THEN 'Y' ELSE NULL END END AS [Has_Medicaid]
	,CASE WHEN t.[Has_Medicaid_Previous] = 0 THEN 'N' ELSE CASE WHEN t.[Has_Medicaid_Previous] = 1 THEN 'Y' ELSE NULL END END AS [Has_Medicaid_Previous]
	,CASE WHEN t.[Has_NRX] = 0 THEN 'N' ELSE CASE WHEN t.[Has_NRX] = 1 THEN 'Y' ELSE NULL END END AS [Has_NRX]
	,CASE WHEN t.[Has_NRX_Previous] = 0 THEN 'N' ELSE CASE WHEN t.[Has_NRX] = 1 THEN 'Y' ELSE NULL END END AS [Has_NRX_Previous]
	,CASE WHEN t.[Has_RX] = 0 THEN 'N' ELSE CASE WHEN t.[Has_RX] = 1 THEN 'Y' ELSE NULL END END AS [Has_RX]
	,CASE WHEN t.[Has_RX_Previous] = 0 THEN 'N' ELSE CASE WHEN t.[Has_RX_Previous] = 1 THEN 'Y' ELSE NULL END END AS [Has_RX_Previous]
	,t.[PC_Treatment_Indicator]
	,t.[PC_Treatment_Indicator_Previous]
	,t.[PC_Attribution]
	,t.[PC_Attribution_Previous]
	,t.[PC_Change_Comments]
	,t.[PC_Change_Comments_Previous]
	,t.[Patient_Centric_Mapping]
	,t.[Patient_Centric_Mapping_Previous]
	,t.[EC_Mapping]
	,t.[EC_Mapping_Previous]
	,t.[EC_Treatment_Indicator]
	,t.[EC_Treatment_Indicator_Previous]
	,t.[EC_Change_Comments]
	,t.[EC_Change_Comments_Previous]
	,t.[Patient_Centric_Change_Comments]
	,t.[Patient_Centric_Change_Comments_Previous]
	,t.[username]
	,t.[update_date]
  FROM [etgsymm].[ETG_Fact_Symmetry_Update_Tracker] t
  INNER JOIN [etgsymm].[ETG_Fact_Symmetry] as f ON f.ETG_Fact_Symmetry_Id = t.ETG_Fact_Symmetry_id
  LEFT OUTER JOIN vct.ETG_Dim_Master AS m ON f.ETG_Base_Class = m.ETG_Base_Class 
LEFT OUTER JOIN vct.ETG_Dim_Premium_Spec_Master AS p ON f.Premium_Specialty_id = p.Premium_Specialty_id
