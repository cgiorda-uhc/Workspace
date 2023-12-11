CREATE VIEW [etgsymm].[VW_ETG_Symmetry_CNFG_ETG_NRX_COMPARE]
	AS SELECT 
      e.[ETG_Base_Class] as ETG_BAS_CLSS_NBR
	  ,e.[ETG_Description] as ETG_BASE_CLS_TRT_RPT_DESC


	  ,n.MEMBER_COUNT as Current_MEMBER_COUNT
		,n.EPSD_COUNT as Current_EPSD_COUNT
		,n.ETGD_TOT_ALLW_AMT as Current_ETGD_TOT_ALLW_AMT
			,n.ETGD_RX_ALLW_AMT as Current_ETGD_RX_ALLW_AMT




      ,a.[RX_RATE] as [Prior_RX_Rate]
	  ,a.[RX_NRX] as [Prior_RX_NRX]

	  ,n.[RX_RATE] as [Current_RX_Rate]
	  ,n.[RX_NRX] as [Current_RX_NRX]

      ,CASE WHEN a.[RX_NRX] = n.[RX_NRX] OR a.[RX_NRX] IS NULL OR  n.[RX_NRX] IS NULL THEN 'N' ELSE 'Y' END as [Change]

  FROM [vct].[ETG_Dim_Master] e 

LEFT OUTER JOIN  [etg].[ETG_Dataload_NRX_AGG] n on e.ETG_Base_Class = n.ETG_Base_Class

LEFT OUTER JOIN (SELECT * FROM [etg].[ETG_Dataload_NRX_AGG_ARCHIVE] WHERE [PD_Version] = (SELECT MAX(PD_Version) FROM [etg].[ETG_Dataload_NRX_AGG_ARCHIVE])) a 



on a.[ETG_Base_Class] = n.[ETG_Base_Class]

