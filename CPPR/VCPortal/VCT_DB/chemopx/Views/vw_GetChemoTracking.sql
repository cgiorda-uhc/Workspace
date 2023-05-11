CREATE VIEW [chemopx].[vw_GetChemoTracking]
	AS 
    SELECT   px.[Id] as Tracking_Id,
         px.[CODE],
         pc.Proc_Desc as [CODE_DESC],
         pc.Proc_Cd_Date as [CODE_END_DT],
         pc.Proc_Cd_Type as [CODE_TYPE],
         px.[GENERIC_NAME],
         px.[GENERIC_NAME_PREVIOUS],
         px.[TRADE_NAME],
         px.[TRADE_NAME_PREVIOUS],
         px.[CKPT_INHIB_IND],
         px.[CKPT_INHIB_IND_PREVIOUS],
         px.[ANTI_EMETIC_IND],
         px.[ANTI_EMETIC_IND_PREVIOUS],
         px.[CODE_EFF_DT],
         px.[CODE_EFF_DT_PREVIOUS],
         px.[NHNR_CANCER_THERAPY],
         px.[NHNR_CANCER_THERAPY_PREVIOUS],
         cc.[Code_Category] as CODE_CATEGORY,
         ccp.[Code_Category] as CODE_CATEGORY_PREVIOUS,
         asp.[ASP_CATEGORY] as ASP_CATEGORY,
         aspp.[ASP_CATEGORY] as ASP_CATEGORY_PREVIOUS,
         da.[DRUG_ADM_MODE] as DRUG_ADM_MODE,
         dap.[DRUG_ADM_MODE] as DRUG_ADM_MODE_PREVIOUS,
         pa.[PA_DRUGS] as PA_DRUGS,
         pap.[PA_DRUGS] as PA_DRUGS_PREVIOUS,
         px.[PA_EFF_DT],
         px.[PA_EFF_DT_PREVIOUS],
         px.[PA_END_DT],
         px.[PA_END_DT_PREVIOUS],
         cp.[CEP_PAY_CD] as CEP_PAY_CD,
         cpp.[CEP_PAY_CD] as CEP_PAY_CD_PREVIOUS,
         ce.[CEP_ENROLL_CD] as CEP_ENROLL_CD,
         cep.[CEP_ENROLL_CD] as CEP_ENROLL_CD_PREVIOUS,
         px.[CEP_ENROLL_EXCL_DESC],
         px.[CEP_ENROLL_EXCL_DESC_PREVIOUS],
         px.[NOVEL_STATUS_IND],
         px.[NOVEL_STATUS_IND_PREVIOUS],
         px.[FIRST_NOVEL_MNTH],
         px.[FIRST_NOVEL_MNTH_PREVIOUS],
         px.[SOURCE],
         px.[SOURCE_PREVIOUS],
         px.[UPDATE_DT],
         px.[UPDATE_USER],
         px.[UPDATE_ACTION]
FROM     [chemopx].[ChemotherapyPX_Tracking] px 
         LEFT JOIN [chemopx].[Code_Category] cc ON cc.CODE_CATEGORY_ID = px.CODE_CATEGORY_ID 
         LEFT JOIN [chemopx].[ASP_Category] asp ON asp.ASP_CATEGORY_ID = px.[ASP_CATEGORY_ID] 
         LEFT JOIN [chemopx].[Drug_Adm_Mode] da ON da.DRUG_ADM_MODE_ID = px.[DRUG_ADM_MODE_ID] 
         LEFT JOIN [chemopx].[PA_Drugs] pa ON pa.PA_DRUGS_ID = px.[PA_DRUGS_ID] 
         LEFT JOIN [chemopx].[CEP_Pay_Cd] cp ON cp.CEP_PAY_CD_ID = px.[CEP_PAY_CD_ID] 
         LEFT JOIN [chemopx].[CEP_Enroll_Cd] ce ON ce.CEP_ENROLL_CD_ID = px.[CEP_ENROLL_CD_ID] 
         LEFT JOIN [vct].[Proc_Codes] pc ON pc.Proc_Cd = px.CODE 
         LEFT JOIN [chemopx].[Code_Category] ccp ON ccp.CODE_CATEGORY_ID = px.CODE_CATEGORY_ID_PREVIOUS 
         LEFT JOIN [chemopx].[ASP_Category] aspp ON aspp.ASP_CATEGORY_ID = px.[ASP_CATEGORY_ID_PREVIOUS] 
         LEFT JOIN [chemopx].[Drug_Adm_Mode] dap ON dap.DRUG_ADM_MODE_ID = px.[DRUG_ADM_MODE_ID_PREVIOUS] 
         LEFT JOIN [chemopx].[PA_Drugs] pap ON pap.PA_DRUGS_ID = px.[PA_DRUGS_ID_PREVIOUS] 
         LEFT JOIN [chemopx].[CEP_Pay_Cd] cpp ON cpp.CEP_PAY_CD_ID = px.[CEP_PAY_CD_ID_PREVIOUS] 
         LEFT JOIN [chemopx].[CEP_Enroll_Cd] cep ON cep.CEP_ENROLL_CD_ID = px.[CEP_ENROLL_CD_ID_PREVIOUS]

