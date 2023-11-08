CREATE VIEW [etgsymm].[VW_ETG_Symmetry_CNFG_ETG_NRX_COMPARE]
	AS SELECT 
      n.[ETG_Base_Class] as ETG_BAS_CLSS_NBR
	  ,e.[ETG_Description] as ETG_BASE_CLS_TRT_RPT_DESC

      ,a.[RX_RATE] as [Prior_RX_Rate]
	  ,a.[RX_NRX] as [Prior_RX_NRX]

	  ,n.[RX_RATE] as [Current_RX_Rate]
	  ,n.[RX_NRX] as [Current_RX_NRX]

      ,CASE WHEN a.[RX_NRX] = n.[RX_NRX] THEN 'Y' ELSE 'N' END as [Change]

  FROM [etg].[ETG_Dataload_NRX_AGG] n
LEFT OUTER JOIN [etg].[ETG_Dataload_NRX_AGG_ARCHIVE] a on a.[ETG_Base_Class] = n.[ETG_Base_Class]
INNER JOIN [vct].[ETG_Dim_Master] e on e.ETG_Base_Class = n.ETG_Base_Class
  WHERE a.[PD_Version] = (SELECT MAX(PD_Version) FROM [etg].[ETG_Dataload_NRX_AGG_ARCHIVE]) 
