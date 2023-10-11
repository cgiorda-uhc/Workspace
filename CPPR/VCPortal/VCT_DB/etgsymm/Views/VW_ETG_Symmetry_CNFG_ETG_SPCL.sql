CREATE VIEW etgsymm.[VW_ETG_Symmetry_CNFG_ETG_SPCL]
	AS 
	
	
		
	SELECT [CNFG_ETG_SPCL_SYS_ID]
      ,[ETG_BASE_CLASS]
      ,[TRT_CD]
      ,[PREM_DESG_VER_NBR]
      ,[PREM_SPCL_CD]
      ,[NOTES]
  FROM [etgsymm].[VW_ETG_Symmetry_CNFG_ETG_SPCL_TMP]

  UNION ALL


  SELECT [CNFG_ETG_SPCL_SYS_ID]
      ,e.[ETG_BASE_CLASS]
      ,NEW_TRT_CD as [TRT_CD]
      ,[PREM_DESG_VER_NBR]
      ,[PREM_SPCL_CD]
      ,[NOTES]
  FROM [etgsymm].[VW_ETG_Symmetry_CNFG_ETG_SPCL_TMP]  e
  INNER JOIN [etg].[ETG_Spec_Bilateral] b ON b.[ETG_BASE_CLASS] = e.[ETG_BASE_CLASS] AND b.TRT_CD =  e.[TRT_CD]
