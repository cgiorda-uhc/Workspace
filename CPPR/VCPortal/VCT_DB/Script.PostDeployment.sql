
--ETG LOADS
IF '$(ETGSymmRefresh)' = '1'
BEGIN 
    TRUNCATE TABLE  [vct].[ETG_Dim_Master];
    TRUNCATE TABLE  [vct].[ETG_Dim_Premium_Spec_Master];
    TRUNCATE TABLE  [etgsymm].[ETG_Fact_Symmetry];
    Truncate table [etgsymm].[ETG_Fact_Symmetry_Update_Tracker];
END

IF NOT EXISTS (SELECT 1 from  [vct].[ETG_Dim_Master]) 
BEGIN 
INSERT INTO [vct].[ETG_Dim_Master]
           ([ETG_Base_Class]
           ,[ETG_Description]
           ,[ETG_Display])
SELECT  [ETG_Base_Class]
      ,[Description]
      ,[Short_Description]
  FROM [deploy].[ETG_Base_Class]
END



IF NOT EXISTS (SELECT 1 from  [vct].[ETG_Dim_Premium_Spec_Master]) 
BEGIN 
INSERT INTO [vct].[ETG_Dim_Premium_Spec_Master]
           ([Premium_Specialty_id]
           ,[Premium_Specialty])
SELECT  [Premium_Specialty_id]
      ,[Premium_Specialty]
  FROM [deploy].[ETG_Dim_Premium_Spec_Master]
END


IF NOT EXISTS (SELECT 1 from  [etgsymm].[ETG_Fact_Symmetry]) 
BEGIN 
INSERT INTO [etgsymm].[ETG_Fact_Symmetry]
           (
           [ETG_Base_Class]
           ,[Premium_Specialty_id]
           ,[Has_Commercial]
           ,[Has_Medicare]
           ,[Has_Medicaid]
           ,[Has_NRX]
           ,[Has_RX]
		   ,[Never_Mapped]
           ,[PC_Treatment_Indicator]

		         ,[PC_Spec_Episode_Count]

				            ,[PC_Spec_Episode_Distribution]
       ,[PC_Spec_Percent_of_Episodes]

	   ,[PC_Spec_Total_Cost]
           ,[PC_Spec_Average_Cost]
		     ,[PC_Normalized_Pricing_Total_Cost]
           ,[PC_Spec_CV]
           ,[PC_Attribution]

		   
           ,[PC_Change_Comments]


    
       
           ,[EC_Treatment_Indicator]
		   ,[EC_Spec_Episode_Count]

		              ,[EC_Spec_Episode_Distribution]
           ,[EC_Spec_Percent_of_Episodes]
           ,[EC_Spec_Total_Cost]
           ,[EC_Spec_Average_Cost]


           ,[EC_Normalized_Pricing_Total_Cost]


           ,[EC_Spec_CV]
           ,[EC_Mapping]
           ,[EC_Change_Comments]
		   ,[Measure_Status]

          
           ,[Symmetry_id]
           ,[Symmetry_Version], PD_Version)
   SELECT [ETG_Base_Class]
      ,[Premium_Specialty_id]
	  ,[Has_Commercial]
      ,[Has_Medicare]
      ,[Has_Medicaid]
      ,[Has_NRX]
      ,[Has_RX]
      ,[Never_Map]
      ,[PC_Current_Treatment_Indicator]
      ,[PC_Spec_Episode_Cnt]
      ,[PC_Spec_Episode_Distribution]
      ,[PC_Spec_Perc_of_Episodes]
      ,[PC_Spec_Tot_Cost]
      ,[PC_Spec_Avg_Cost]
      ,[PC_Spec_Normalized_Pricing]
      ,[PC_Spec_CV]
      ,[PC_Current_Attribution]
      ,[PC_Change_Comments]

      ,[EC_Current_Treatment_Indicator]
      ,[EC_Spec_Episode_Cnt]
      ,[EC_Spec_Episode_Distribution]
      ,[EC_Spec_Perc_of_Episodes]
      ,[EC_Spec_Tot_Cost]
      ,[EC_Spec_Avg_Cost]
      ,[EC_Spec_Normalized_Pricing]
      ,[EC_Spec_CV]
      ,[EC_Current_Mapping]
      ,[EC_Change_Comments]
      ,[Measure_Status]
      ,[PD_Version]
      ,[PD_Version]
      ,[Symmetry_Version]
  FROM [deploy].[ETG_Fact_Symmetry_Final]
END



--CHEMO PX LOADS
IF '$(ChemoPXRefresh)' = '1'
BEGIN 
    TRUNCATE TABLE  [chemopx].[Code_Category];
    TRUNCATE TABLE [chemopx].[ASP_Category];
    TRUNCATE TABLE  [chemopx].[Drug_Adm_Mode];
    TRUNCATE TABLE  [chemopx].[PA_Drugs];
    TRUNCATE TABLE  [chemopx].[CEP_Pay_Cd];
    TRUNCATE TABLE  [chemopx].[CEP_Enroll_Cd];
    TRUNCATE TABLE  [chemopx].[ChemotherapyPX];
    Truncate table [chemopx].[ChemotherapyPX_Tracking];
    Truncate table [vct].[Proc_Codes];
END

IF NOT EXISTS (SELECT 1 from  [vct].[Proc_Codes]) 
BEGIN 
INSERT INTO [vct].[Proc_Codes] ( [Proc_Cd], [Proc_Desc],[Proc_Cd_Type], [Proc_Cd_Date]) 
SELECT [PROC_CD],[PROC_DESC],[PROC_CD_Type], [Proc_CD_Date]  FROM [deploy].[Proc_Codes]
END


IF NOT EXISTS (SELECT 1 from  [chemopx].[Code_Category]) 
BEGIN 
	INSERT INTO [chemopx].[Code_Category] ( [Code_Category]) 
	SELECT DISTINCT Code_Category  FROM [deploy].[ChemotherapyPXCodes]
	WHERE  ISNULL(Code_Category,'') <> ''
	--VALUES
	--('ACTIVE TREATMENT: ADOPTIVE IMMUNOTHERAPY'),
	--('ACTIVE TREATMENT: BIOLOGIC'),
	--('ACTIVE TREATMENT: CAR-T'),
	--('ACTIVE TREATMENT: CHEMOTHERAPY'),
	--('ACTIVE TREATMENT: HORMONAL'),
	--('ACTIVE TREATMENT: OTHER DRUG'),
	--('ACTIVE TREATMENT: RADIOPHARMACEUTICAL'),
	--('ADMINISTRATION'),
	--('ADMINISTRATION: CAR-T'),
	--('ADMINISTRATION: RADIOPHARMACEUTICAL'),
	--('EOC EPISODE FEE'),
	--('NON-CANCER DRUG'),
	--('PROTECTIVE AND SUPPORTIVE CARE')
END


IF NOT EXISTS (SELECT 1 from  [chemopx].[ASP_Category]) 
BEGIN 
	INSERT INTO [chemopx].[ASP_Category] ( [ASP_Category]) 
	SELECT DISTINCT ASP_Category  FROM [deploy].[ChemotherapyPXCodes]
	WHERE  ISNULL(ASP_Category,'') <> ''
	--VALUES
	--('INJECTABLES/OTHER DRUGS'),
	--('INJECTABLES-ONCOLOGY/THERAPEUTIC CHEMO DRUGS')
END



IF NOT EXISTS (SELECT 1 from  [chemopx].[Drug_Adm_Mode])
BEGIN 
	INSERT INTO [chemopx].[Drug_Adm_Mode] ( [DRUG_ADM_MODE]) 
	SELECT DISTINCT Drug_Adm_Mode  FROM [deploy].[ChemotherapyPXCodes]
	WHERE  ISNULL(Drug_Adm_Mode,'') <> ''
--	VALUES
--	('INSTILLATION'),
--('N/A'),
--('ORAL'),
--('PARENTERAL')

END


IF NOT EXISTS (SELECT 1 from  [chemopx].[PA_Drugs]) 
BEGIN 
	INSERT INTO [chemopx].[PA_Drugs] ( [PA_Drugs]) 
	SELECT DISTINCT PA_Drugs  FROM [deploy].[ChemotherapyPXCodes]
	WHERE  ISNULL(PA_Drugs,'') <> ''
--	VALUES
--	('ACTIVE'),
--('ACTIVE RADIO'),
--('ACTIVE: SUPPORTIVE'),
--('INACTIVE'),
--('N/A')

END


IF NOT EXISTS (SELECT 1 from  [chemopx].[CEP_Pay_Cd]) 
BEGIN 
	INSERT INTO [chemopx].[CEP_Pay_Cd] ( [CEP_Pay_Cd]) 
	SELECT DISTINCT CEP_Pay_Cd  FROM [deploy].[ChemotherapyPXCodes]
	WHERE  ISNULL(CEP_Pay_Cd,'') <> ''
--	VALUES
--	('EXCLUDE'),
--('REPRICED AT 100% of ASP')

END


IF NOT EXISTS (SELECT 1 from  [chemopx].[CEP_Enroll_Cd]) 
BEGIN 
	INSERT INTO [chemopx].[CEP_Enroll_Cd] ( [CEP_Enroll_Cd]) 
	SELECT DISTINCT CEP_Enroll_Cd FROM [deploy].[ChemotherapyPXCodes]
	WHERE  ISNULL(CEP_Enroll_Cd,'') <> ''
--	VALUES
--('ENROLL'),
--	('EXCLUDE')


END


IF NOT EXISTS (SELECT 1 from  [chemopx].[ChemotherapyPX])
BEGIN 
INSERT INTO [chemopx].[ChemotherapyPX] ( [CODE], [CODE_DESC], [GENERIC_NAME], [TRADE_NAME], [CKPT_INHIB_IND], [ANTI_EMETIC_IND], [CODE_TYPE], [CODE_EFF_DT], [CODE_END_DT], [NHNR_CANCER_THERAPY], [CODE_CATEGORY_ID],[ASP_CATEGORY_ID],[DRUG_ADM_MODE_ID], [PA_DRUGS_ID] ,[PA_EFF_DT], [PA_END_DT], [CEP_PAY_CD_ID],[CEP_ENROLL_CD_ID], [CEP_ENROLL_EXCL_DESC], [NOVEL_STATUS_IND], [FIRST_NOVEL_MNTH], [SOURCE],[UPDATE_DT]) 

SELECT px.[CODE], px.[CODE_DESC], px.[GENERIC_NAME], px.[TRADE_NAME], CASE WHEN px.[CKPT_INHIB_IND] = 'Y' THEN 1 ELSE 0 END, CASE WHEN px.[ANTI_EMETIC_IND] = 'Y' THEN 1 ELSE 0 END, px.[CODE_TYPE], px.[CODE_EFF_DT], px.[CODE_END_DT], CASE WHEN px.[NHNR_CANCER_THERAPY] = 'Y' THEN 1 ELSE 0 END, cc.[CODE_CATEGORY_ID],  asp.[ASP_CATEGORY_ID], da.[DRUG_ADM_MODE_ID],  pa.[PA_DRUGS_ID], px.[PA_EFF_DT], px.[PA_END_DT], cp.[CEP_PAY_CD_ID],  ce.[CEP_ENROLL_CD_ID], px.[CEP_ENROLL_EXCL_DESC], CASE WHEN px.[NOVEL_STATUS_IND] = 'Y' THEN 1 ELSE 0 END, px.[FIRST_NOVEL_MNTH], px.[SOURCE], px.[UPDATE_DT] FROM [deploy].[ChemotherapyPXCodes] px LEFT JOIN [chemopx].[Code_Category] cc ON cc.Code_Category = px.CODE_CATEGORY LEFT JOIN [chemopx].[ASP_Category] asp ON asp.ASP_CATEGORY = px.[ASP_CATEGORY] LEFT JOIN [chemopx].[Drug_Adm_Mode] da ON da.DRUG_ADM_MODE = px.[DRUG_ADM_MODE] LEFT JOIN [chemopx].[PA_Drugs] pa ON pa.PA_DRUGS = px.[PA_DRUGS] LEFT JOIN [chemopx].[CEP_Pay_Cd] cp ON cp.CEP_PAY_CD = px.[CEP_PAY_CD] LEFT JOIN [chemopx].[CEP_Enroll_Cd] ce ON ce.CEP_Enroll_Cd = px.[CEP_ENROLL_CD]
END


