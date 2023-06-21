CREATE VIEW [etgsymm].[VW_UGAPCFG_FINAL]
	AS 

	SELECT 

NULL as [MPC_NBR],
   t.[BASEETG] as [ETG_BAS_CLSS_NBR],
         t.[IF_ALWAYS] as [ALWAYS],
      t.[IF_ATTRIB] as [ATTRIBUTED],

p.[PREMIUM_ABBRV] as [ERG_SPCL_CATGY_CD],
   
      t.[TREATMENT_IND] as [TRT_CD],
	 t.[RX],
      t.[NRX],



      t.[RISK_MODEL] as [RISK_Model],

	  SUBSTRING(t.[BUCKET],1,CHARINDEX('_',t.[BUCKET])-1)  as LOW_MONTH,
	  SUBSTRING(t.[BUCKET],CHARINDEX('_',t.[BUCKET])+1,len(t.[BUCKET])) as HIGH_MONTH
FROM
(
SELECT [SPECIALTY]
      ,[BASEETG]
      ,[TREATMENT_IND]
      ,[ETG_DESC]
      ,[IF_ALWAYS]
      ,[IF_ATTRIB]
      ,[RISK_MODEL]
      ,[RX]
      ,[NRX]
      ,[BUCKET]
  FROM [vct].[UGAPCFG_ETG_TI_RX_NRX_COM]

  UNION ALL 

  SELECT [SPECIALTY]
      ,[BASEETG]
      ,[TREATMENT_IND]
      ,[ETG_DESC]
      ,[IF_ALWAYS]
      ,[IF_ATTRIB]
      , 'Exchange' as [RISK_MODEL]
      ,[RX]
      ,[NRX]
      ,[BUCKET]
  FROM [vct].[UGAPCFG_ETG_TI_RX_NRX_COM]

  UNION ALL

  SELECT  [SPECIALTY]
      ,[BASEETG]
      ,[TREATMENT_IND]
      ,[ETG_DESC]
      ,[IF_ALWAYS]
      ,[IF_ATTRIB]
      ,[RISK_MODEL]
      ,[RX]
      ,[NRX]
      ,[BUCKET]
  FROM [vct].[UGAPCFG_ETG_TI_RX_NRX_MCD]

  UNION ALL

  SELECT  [SPECIALTY]
      ,[BASEETG]
      ,[TREATMENT_IND]
      ,[ETG_DESC]
      ,[IF_ALWAYS]
      ,[IF_ATTRIB]
      ,[RISK_MODEL]
      ,[RX]
      ,[NRX]
      ,[BUCKET]
  FROM [vct].[UGAPCFG_ETG_TI_RX_NRX_MCR]

  ) t
  LEFT JOIN [vct].[Premium_Mapping] p ON p.[PREMIUM] = t.[SPECIALTY]
