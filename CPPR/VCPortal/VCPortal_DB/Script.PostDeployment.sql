
IF NOT EXISTS (SELECT 1 from  [dbo].[Proc_Codes])
BEGIN 
INSERT INTO [dbo].[Proc_Codes] ( [Proc_Cd], [Proc_Desc],[Proc_Cd_Type], [Proc_Cd_Date]) 
SELECT [PROC_CD],[PROC_DESC],[PROC_CD_Type], [Proc_CD_Date]  FROM [VCPostDeploy].[dbo].[Proc_Codes]
END



IF NOT EXISTS (SELECT 1 from  [dbo].[Code_Category])
BEGIN 
	INSERT INTO [dbo].[Code_Category] ( [Code_Category]) 
	SELECT DISTINCT Code_Category  FROM [VCPostDeploy].[dbo].[ChemotherapyPXCodes]
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


IF NOT EXISTS (SELECT 1 from  [dbo].[ASP_Category])
BEGIN 
	INSERT INTO [dbo].[ASP_Category] ( [ASP_Category]) 
	SELECT DISTINCT ASP_Category  FROM [VCPostDeploy].[dbo].[ChemotherapyPXCodes]
	WHERE  ISNULL(ASP_Category,'') <> ''
	--VALUES
	--('INJECTABLES/OTHER DRUGS'),
	--('INJECTABLES-ONCOLOGY/THERAPEUTIC CHEMO DRUGS')
END



IF NOT EXISTS (SELECT 1 from  [dbo].[Drug_Adm_Mode])
BEGIN 
	INSERT INTO [dbo].[Drug_Adm_Mode] ( [DRUG_ADM_MODE]) 
	SELECT DISTINCT Drug_Adm_Mode  FROM [VCPostDeploy].[dbo].[ChemotherapyPXCodes]
	WHERE  ISNULL(Drug_Adm_Mode,'') <> ''
--	VALUES
--	('INSTILLATION'),
--('N/A'),
--('ORAL'),
--('PARENTERAL')

END

IF NOT EXISTS (SELECT 1 from  [dbo].[PA_Drugs])
BEGIN 
	INSERT INTO [dbo].[PA_Drugs] ( [PA_Drugs]) 
	SELECT DISTINCT PA_Drugs  FROM [VCPostDeploy].[dbo].[ChemotherapyPXCodes]
	WHERE  ISNULL(PA_Drugs,'') <> ''
--	VALUES
--	('ACTIVE'),
--('ACTIVE RADIO'),
--('ACTIVE: SUPPORTIVE'),
--('INACTIVE'),
--('N/A')

END


IF NOT EXISTS (SELECT 1 from  [dbo].[CEP_Pay_Cd])
BEGIN 
	INSERT INTO [dbo].[CEP_Pay_Cd] ( [CEP_Pay_Cd]) 
	SELECT DISTINCT CEP_Pay_Cd  FROM [VCPostDeploy].[dbo].[ChemotherapyPXCodes]
	WHERE  ISNULL(CEP_Pay_Cd,'') <> ''
--	VALUES
--	('EXCLUDE'),
--('REPRICED AT 100% of ASP')

END


IF NOT EXISTS (SELECT 1 from  [dbo].[CEP_Enroll_Cd])
BEGIN 
	INSERT INTO [dbo].[CEP_Enroll_Cd] ( [CEP_Enroll_Cd]) 
	SELECT DISTINCT CEP_Enroll_Cd FROM [VCPostDeploy].[dbo].[ChemotherapyPXCodes]
	WHERE  ISNULL(CEP_Enroll_Cd,'') <> ''
--	VALUES
--('ENROLL'),
--	('EXCLUDE')


END


IF NOT EXISTS (SELECT 1 from  [dbo].[ChemotherapyPX])
BEGIN 
INSERT INTO [dbo].[ChemotherapyPX] ( [CODE], [CODE_DESC], [GENERIC_NAME], [TRADE_NAME], [CKPT_INHIB_IND], [ANTI_EMETIC_IND], [CODE_TYPE], [CODE_EFF_DT], [CODE_END_DT], [NHNR_CANCER_THERAPY], [CODE_CATEGORY_ID],[ASP_CATEGORY_ID],[DRUG_ADM_MODE_ID], [PA_DRUGS_ID] ,[PA_EFF_DT], [PA_END_DT], [CEP_PAY_CD_ID],[CEP_ENROLL_CD_ID], [CEP_ENROLL_EXCL_DESC], [NOVEL_STATUS_IND], [FIRST_NOVEL_MNTH], [SOURCE],[UPDATE_DT]) 

SELECT px.[CODE], px.[CODE_DESC], px.[GENERIC_NAME], px.[TRADE_NAME], CASE WHEN px.[CKPT_INHIB_IND] = 'Y' THEN 1 ELSE 0 END, CASE WHEN px.[ANTI_EMETIC_IND] = 'Y' THEN 1 ELSE 0 END, px.[CODE_TYPE], px.[CODE_EFF_DT], px.[CODE_END_DT], CASE WHEN px.[NHNR_CANCER_THERAPY] = 'Y' THEN 1 ELSE 0 END, cc.[CODE_CATEGORY_ID],  asp.[ASP_CATEGORY_ID], da.[DRUG_ADM_MODE_ID],  pa.[PA_DRUGS_ID], px.[PA_EFF_DT], px.[PA_END_DT], cp.[CEP_PAY_CD_ID],  ce.[CEP_ENROLL_CD_ID], px.[CEP_ENROLL_EXCL_DESC], CASE WHEN px.[NOVEL_STATUS_IND] = 'Y' THEN 1 ELSE 0 END, px.[FIRST_NOVEL_MNTH], px.[SOURCE], px.[UPDATE_DT] FROM [VCPostDeploy].[dbo].[ChemotherapyPXCodes2023] px LEFT JOIN [dbo].[Code_Category] cc ON cc.Code_Category = px.CODE_CATEGORY LEFT JOIN [dbo].[ASP_Category] asp ON asp.ASP_CATEGORY = px.[ASP_CATEGORY] LEFT JOIN [dbo].[Drug_Adm_Mode] da ON da.DRUG_ADM_MODE = px.[DRUG_ADM_MODE] LEFT JOIN [dbo].[PA_Drugs] pa ON pa.PA_DRUGS = px.[PA_DRUGS] LEFT JOIN [dbo].[CEP_Pay_Cd] cp ON cp.CEP_PAY_CD = px.[CEP_PAY_CD] LEFT JOIN [dbo].[CEP_Enroll_Cd] ce ON ce.CEP_Enroll_Cd = px.[CEP_ENROLL_CD]
END


Truncate table [dbo].[ChemotherapyPX_Tracking];