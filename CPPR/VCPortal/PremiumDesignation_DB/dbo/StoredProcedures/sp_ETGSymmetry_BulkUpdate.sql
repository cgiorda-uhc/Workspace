CREATE PROCEDURE [dbo].[sp_ETGFactSymmetry_BulkUpdate]
	@username varchar(10),
	@update_date DATETIME
AS



UPDATE Track
SET 
[Has_Commercial_Previous] = CASE WHEN Track.[Has_Commercial] IS NULL THEN NULL ELSE ETG.[Has_Commercial] END,
[Has_Medicare_Previous] = CASE WHEN Track.[Has_Medicare] IS NULL THEN NULL ELSE ETG.[Has_Medicare] END,
[Has_Medicaid_Previous] = CASE WHEN Track.[Has_Medicaid] IS NULL THEN NULL  ELSE ETG.[Has_Medicaid] END,
[Has_NRX_Previous] = CASE WHEN Track.[Has_NRX] IS NULL THEN NULL  ELSE ETG.[Has_NRX] END,
[Has_RX_Previous] = CASE WHEN Track.[Has_RX] IS NULL THEN NULL  ELSE ETG.[Has_RX] END,
[PC_Treatment_Indicator_Previous] = CASE WHEN Track.[PC_Treatment_Indicator] IS NULL THEN NULL  ELSE ETG.[PC_Treatment_Indicator] END,
[PC_Attribution_Previous] = CASE WHEN Track.[PC_Attribution] IS NULL THEN NULL  ELSE ETG.[PC_Attribution] END,
[PC_Change_Comments_Previous] = CASE WHEN Track.[PC_Change_Comments] IS NULL THEN NULL ELSE ETG.[PC_Change_Comments] END,
[Patient_Centric_Mapping_Previous] = CASE WHEN Track.[Patient_Centric_Mapping] IS NULL THEN NULL  ELSE ETG.[Patient_Centric_Mapping] END,
[EC_Mapping_Previous] = CASE WHEN Track.[EC_Mapping] IS NULL THEN NULL  ELSE ETG.[EC_Mapping] END,
[EC_Treatment_Indicator_Previous] = CASE WHEN Track.[EC_Treatment_Indicator] IS NULL THEN NULL  ELSE ETG.[EC_Treatment_Indicator] END,
[EC_Change_Comments_Previous] = CASE WHEN Track.[EC_Change_Comments] IS NULL THEN NULL  ELSE ETG.[EC_Change_Comments] END,
[Patient_Centric_Change_Comments_Previous] = CASE WHEN Track.[Patient_Centric_Change_Comments] IS NULL THEN NULL  ELSE ETG.[Patient_Centric_Change_Comments] END
FROM [dbo].[ETG_Fact_Symmetry_Update_Tracker] Track
INNER JOIN 
[dbo].[ETG_Fact_Symmetry] ETG
ON ETG.[ETG_Fact_Symmetry_id] = Track.[ETG_Fact_Symmetry_id]
WHERE Track.[username] = @username AND Track.update_date =@update_date 





UPDATE ETG
SET 

[Has_Commercial] = CASE WHEN Track.[Has_Commercial] IS NULL THEN ETG.[Has_Commercial] ELSE Track.[Has_Commercial] END,
[Has_Medicare] = CASE WHEN Track.[Has_Medicare] IS NULL THEN ETG.[Has_Medicare] ELSE Track.[Has_Medicare] END,
[Has_Medicaid] = CASE WHEN Track.[Has_Medicaid] IS NULL THEN ETG.[Has_Medicaid] ELSE Track.[Has_Medicaid] END,
[Has_NRX] = CASE WHEN Track.[Has_NRX] IS NULL THEN ETG.[Has_NRX] ELSE Track.[Has_NRX] END,
[Has_RX] = CASE WHEN Track.[Has_RX] IS NULL THEN ETG.[Has_RX] ELSE Track.[Has_RX] END,
[PC_Treatment_Indicator] = CASE WHEN Track.[PC_Treatment_Indicator] IS NULL THEN ETG.[PC_Treatment_Indicator] ELSE Track.[PC_Treatment_Indicator] END,
[PC_Attribution] = CASE WHEN Track.[PC_Attribution] IS NULL THEN ETG.[PC_Attribution] ELSE Track.[PC_Attribution] END,
[PC_Change_Comments] = CASE WHEN Track.[PC_Change_Comments] IS NULL THEN ETG.[PC_Change_Comments] ELSE Track.[PC_Change_Comments] END,
[Patient_Centric_Mapping] = CASE WHEN Track.[Patient_Centric_Mapping] IS NULL THEN ETG.[Patient_Centric_Mapping] ELSE Track.[Patient_Centric_Mapping] END,
[EC_Mapping] = CASE WHEN Track.[EC_Mapping] IS NULL THEN ETG.[EC_Mapping] ELSE Track.[EC_Mapping] END,
[EC_Treatment_Indicator] = CASE WHEN Track.[EC_Treatment_Indicator] IS NULL THEN ETG.[EC_Treatment_Indicator] ELSE Track.[EC_Treatment_Indicator] END,
[EC_Change_Comments] = CASE WHEN Track.[EC_Change_Comments] IS NULL THEN ETG.[EC_Change_Comments] ELSE Track.[EC_Change_Comments] END,
[Patient_Centric_Change_Comments] = CASE WHEN Track.[Patient_Centric_Change_Comments] IS NULL THEN ETG.[Patient_Centric_Change_Comments] ELSE Track.[Patient_Centric_Change_Comments] END
FROM [dbo].[ETG_Fact_Symmetry] ETG
INNER JOIN 
[dbo].[ETG_Fact_Symmetry_Update_Tracker] Track
ON ETG.[ETG_Fact_Symmetry_id] = Track.[ETG_Fact_Symmetry_id]
WHERE Track.[username] = @username AND Track.update_date =@update_date 
GO


