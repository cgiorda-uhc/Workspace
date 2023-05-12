CREATE PROCEDURE [etgsymm].[sp_ETGFactSymmetry_BulkUpdate]
	@username varchar(10),
	@update_date DATETIME
AS



UPDATE Track
SET 
Track.[Has_Commercial_Previous] = CASE WHEN ISNULL(Track.[Has_Commercial],'') <> ISNULL(ETG.[Has_Commercial],'') THEN ETG.[Has_Commercial] ELSE Track.[Has_Commercial_Previous] END,
Track.[Has_Medicare_Previous] = CASE WHEN ISNULL(Track.[Has_Medicare],'') <> ISNULL(ETG.[Has_Medicare],'') THEN ETG.[Has_Medicare] ELSE Track.[Has_Medicare_Previous] END,
Track.[Has_Medicaid_Previous] = CASE WHEN ISNULL(Track.[Has_Medicaid],'') <>  ISNULL(ETG.[Has_Medicaid],'') THEN ETG.[Has_Medicaid]  ELSE  Track.[Has_Medicaid_Previous]  END,
Track.[Has_NRX_Previous] = CASE WHEN ISNULL(Track.[Has_NRX],'') <> ISNULL(ETG.[Has_NRX],'') THEN ETG.[Has_NRX]  ELSE Track.[Has_NRX_Previous] END,
Track.[Has_RX_Previous] = CASE WHEN ISNULL(Track.[Has_RX],'') <> ISNULL(ETG.[Has_RX],'') THEN ETG.[Has_RX]  ELSE Track.[Has_RX_Previous]  END,
Track.[PC_Treatment_Indicator_Previous] = CASE WHEN ISNULL(Track.[PC_Treatment_Indicator],'') <> ISNULL(ETG.[PC_Treatment_Indicator],'') THEN ETG.[PC_Treatment_Indicator]  ELSE Track.[PC_Treatment_Indicator_Previous] END,
Track.[PC_Attribution_Previous] = CASE WHEN ISNULL(Track.[PC_Attribution],'') <> ISNULL(ETG.[PC_Attribution],'') THEN ETG.[PC_Attribution] ELSE Track.[PC_Attribution_Previous] END,
Track.[PC_Change_Comments_Previous] = CASE WHEN ISNULL(Track.[PC_Change_Comments],'') NOT LIKE ISNULL(ETG.[PC_Change_Comments],'') THEN ETG.[PC_Change_Comments] ELSE Track.[PC_Change_Comments_Previous] END,
Track.[Patient_Centric_Mapping_Previous] = CASE WHEN ISNULL(Track.[Patient_Centric_Mapping],'') <> ISNULL(ETG.[Patient_Centric_Mapping],'')  THEN ETG.[Patient_Centric_Mapping]  ELSE Track.[Patient_Centric_Mapping_Previous] END,
Track.[EC_Mapping_Previous] = CASE WHEN ISNULL(Track.[EC_Mapping],'') <> ISNULL(ETG.[EC_Mapping],'') THEN ISNULL(ETG.[EC_Mapping],'')  ELSE Track.[EC_Mapping_Previous] END,
Track.[EC_Treatment_Indicator_Previous] = CASE WHEN ISNULL(Track.[EC_Treatment_Indicator],'') <> ISNULL(ETG.[EC_Treatment_Indicator],'') THEN ETG.[EC_Treatment_Indicator]  ELSE  Track.[EC_Treatment_Indicator_Previous] END,
Track.[EC_Change_Comments_Previous] = CASE WHEN ISNULL(Track.[EC_Change_Comments],'') NOT LIKE ISNULL(ETG.[EC_Change_Comments],'')  THEN ETG.[EC_Change_Comments]   ELSE Track.[EC_Change_Comments_Previous] END,
Track.[Patient_Centric_Change_Comments_Previous] = CASE WHEN ISNULL(Track.[Patient_Centric_Change_Comments],'') NOT LIKE ISNULL(ETG.[Patient_Centric_Change_Comments],'') THEN ETG.[Patient_Centric_Change_Comments]  ELSE  Track.[Patient_Centric_Change_Comments_Previous] END
FROM [etgsymm].[ETG_Fact_Symmetry_Update_Tracker] Track
INNER JOIN 
[etgsymm].[ETG_Fact_Symmetry] ETG
ON ETG.[ETG_Fact_Symmetry_id] = Track.[ETG_Fact_Symmetry_id]
WHERE Track.[username] = @username AND Track.update_date =@update_date 





UPDATE ETG
SET 
ETG.[Has_Commercial] = CASE WHEN ISNULL(Track.[Has_Commercial],'') <> ISNULL(ETG.[Has_Commercial],'') THEN Track.[Has_Commercial] ELSE ETG.[Has_Commercial] END,
ETG.[Has_Medicare] = CASE WHEN ISNULL(Track.[Has_Medicare],'') <> ISNULL(ETG.[Has_Medicare],'') THEN Track.[Has_Medicare] ELSE ETG.[Has_Medicare] END,
ETG.[Has_Medicaid] = CASE WHEN ISNULL(Track.[Has_Medicaid],'') <>  ISNULL(ETG.[Has_Medicaid],'') THEN Track.[Has_Medicaid]  ELSE  ETG.[Has_Medicaid]  END,
ETG.[Has_NRX] = CASE WHEN ISNULL(Track.[Has_NRX],'') <> ISNULL(ETG.[Has_NRX],'') THEN Track.[Has_NRX]  ELSE ETG.[Has_NRX] END,
ETG.[Has_RX] = CASE WHEN ISNULL(Track.[Has_RX],'') <> ISNULL(ETG.[Has_RX],'') THEN Track.[Has_RX]  ELSE ETG.[Has_RX]  END,
ETG.[PC_Treatment_Indicator] = CASE WHEN ISNULL(Track.[PC_Treatment_Indicator],'') <> ISNULL(ETG.[PC_Treatment_Indicator],'') THEN Track.[PC_Treatment_Indicator]  ELSE ETG.[PC_Treatment_Indicator] END,
ETG.[PC_Attribution] = CASE WHEN ISNULL(Track.[PC_Attribution],'') <> ISNULL(ETG.[PC_Attribution],'') THEN Track.[PC_Attribution] ELSE Track.[PC_Attribution] END,
ETG.[PC_Change_Comments] = CASE WHEN ISNULL(Track.[PC_Change_Comments],'') NOT LIKE ISNULL(ETG.[PC_Change_Comments],'') THEN Track.[PC_Change_Comments] ELSE ETG.[PC_Change_Comments] END,
ETG.[Patient_Centric_Mapping] = CASE WHEN ISNULL(Track.[Patient_Centric_Mapping],'') <> ISNULL(ETG.[Patient_Centric_Mapping],'')  THEN Track.[Patient_Centric_Mapping]  ELSE ETG.[Patient_Centric_Mapping] END,
ETG.[EC_Mapping] = CASE WHEN ISNULL(Track.[EC_Mapping],'') <> ISNULL(ETG.[EC_Mapping],'') THEN Track.[EC_Mapping]  ELSE Track.[EC_Mapping] END,
ETG.[EC_Treatment_Indicator] = CASE WHEN ISNULL(Track.[EC_Treatment_Indicator],'') <> ISNULL(ETG.[EC_Treatment_Indicator],'') THEN Track.[EC_Treatment_Indicator]  ELSE  ETG.[EC_Treatment_Indicator] END,
ETG.[EC_Change_Comments] = CASE WHEN ISNULL(Track.[EC_Change_Comments],'') NOT LIKE ISNULL(ETG.[EC_Change_Comments],'')  THEN Track.[EC_Change_Comments]   ELSE ETG.[EC_Change_Comments] END,
ETG.[Patient_Centric_Change_Comments] = CASE WHEN ISNULL(Track.[Patient_Centric_Change_Comments],'') NOT LIKE ISNULL(ETG.[Patient_Centric_Change_Comments],'') THEN Track.[Patient_Centric_Change_Comments]  ELSE  ETG.[Patient_Centric_Change_Comments] END
FROM [etgsymm].[ETG_Fact_Symmetry] ETG
INNER JOIN 
[etgsymm].[ETG_Fact_Symmetry_Update_Tracker] Track
ON ETG.[ETG_Fact_Symmetry_id] = Track.[ETG_Fact_Symmetry_id]
WHERE Track.[username] = @username AND Track.update_date =@update_date 
GO


