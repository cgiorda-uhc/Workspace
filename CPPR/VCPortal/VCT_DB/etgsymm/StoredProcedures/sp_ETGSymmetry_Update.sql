

CREATE PROCEDURE [etgsymm].[sp_ETGSymmetry_Update]
@Update_Date as Date,
@User  [varchar](10)
AS
BEGIN


	UPDATE f
	SET 
	f.[has_Commercial] = u.Has_Commercial,
	f.[has_Medicare] = u.Has_Medicare,
	f.[has_Medicaid] =u.Has_Medicaid ,
	--f.[PC_Treatment_Indicator] = u.Pop_Cost_Current_Treatment_Indicator,
	--f.[PC_Attribution] =u.Current_Attribution,
	--f.[EC_Treatment_Indicator] = u.Current_Episode_Cost_Treatment_Indicator,
	--f.[EC_Mapping] = u.Current_Mapping,
	--f.[Patient_Centric_Mapping] = u.Current_Patient_Centric_Mapping,
	--f.[PC_Change_Comments] = u.Pop_Cost_Change_Comments,
	--f.[EC_Change_Comments] = u.Episode_Cost_Change_Comments,
	f.[Patient_Centric_Change_Comments] =u.Patient_Centric_Change_Comments
	FROM [etgsymm].[ETG_Fact_Symmetry] f
	INNER JOIN
	[etgsymm].[ETG_Fact_Symmetry_Update_Tracker] u
	ON f.ETG_Fact_Symmetry_id = u.ETG_Fact_Symmetry_id
	WHERE u.username = @User AND u.update_date = @Update_Date
END




