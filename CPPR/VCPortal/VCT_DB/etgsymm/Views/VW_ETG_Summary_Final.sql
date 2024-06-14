CREATE VIEW [etgsymm].[VW_ETG_Summary_Final]
	AS 

	/****** Script for SelectTopNRows command from SSMS  ******/
/****** Script for SelectTopNRows command from SSMS  ******/
SELECT [ETG_Base_Class]
      ,[ETG_Description]
      ,[Premium_Specialty]
      ,[Never_Map]
      ,[Never_Map_Previous]
      ,[Current_Rx_NRx]
      ,[Previous_Rx_NRx]
      ,[Current_LOB]
      ,[LOB_UGAP]
      ,[Previous_LOB]
      ,[PC_Current_Treatment_Indicator]
      ,[PC_Previous_Treatment_Indicator]
      ,[PC_Spec_Episode_Cnt]
      ,[PC_Spec_Episode_Distribution]
      ,[PC_Spec_Perc_of_Episodes]
      ,[PC_Spec_Tot_Cost]
      ,[PC_Spec_Avg_Cost]
      ,[PC_Spec_Normalized_Pricing]
      ,[PC_Spec_CV]

      ,CASE WHEN [UGAP_Changes] = 'Drop' AND [LOB_UGAP] IS NULL THEN 'Not Mapped'  ELSE PC_Current_Attribution END AS [PC_Current_Attribution]


      ,[PC_Prev_Attribution]
      ,[PC_Change_Comments]
      ,[EC_Current_Treatment_Indicator]
      ,[EC_Previous_Treatment_Indicator]
      ,[EC_Spec_Episode_Cnt]
      ,[EC_Spec_Episode_Distribution]
      ,[EC_Spec_Perc_of_Episodes]
      ,[EC_Spec_Tot_Cost]
      ,[EC_Spec_Avg_Cost]
      ,[EC_Spec_Normalized_Pricing]
      ,[EC_Spec_CV]



      , CASE WHEN [UGAP_Changes] = 'Drop' AND [LOB_UGAP] IS NULL THEN 'Not Mapped'  ELSE EC_Current_Mapping END AS [EC_Current_Mapping]

      ,[EC_Previous_Mapping]

	  ,EC_Changes_Made

      ,[EC_Change_Comments]
      ,[PC_Measure_Status]
      ,[UGAP_Changes]
      ,[Is_Flagged]
	  ,ETG_Fact_Symmetry_Id
  FROM 
  (

  SELECT 
	[ETG_Base_Class]
	,[ETG_Description]
	,[Premium_Specialty]
	,CASE WHEN [Never_Mapped] = 0 THEN 'N' ELSE CASE WHEN [Never_Mapped] = 1 THEN 'Y' ELSE NULL END END as Never_Map
		,CASE WHEN [Never_Mapped_Previous] = 0 THEN 'N' ELSE CASE WHEN [Never_Mapped_Previous] = 1 THEN 'Y' ELSE NULL END END as  Never_Map_Previous
	,[RX_NRX] as Current_Rx_NRx
	,[RX_NRXPrevious] as Previous_Rx_NRx


,CASE WHEN Has_Commercial = 1 AND Has_Medicare = 1 AND Has_Medicaid = 1 THEN 'All' ELSE 
CASE WHEN Has_Commercial = 1 AND Has_Medicare = 1 THEN 'Commercial + Medicare' ELSE
CASE WHEN Has_Commercial = 1 AND Has_Medicaid = 1 THEN 'Commercial + Medicaid' ELSE
CASE WHEN Has_Medicare = 1 AND Has_Medicaid = 1 THEN 'Medicare + Medicaid' ELSE
CASE WHEN Has_Commercial = 1 THEN 'Commercial Only' ELSE
CASE WHEN Has_Medicare = 1 THEN 'Medicare Only' ELSE
CASE WHEN Has_Medicaid = 1 THEN 'Medicaid Only' ELSE 'Not Mapped' 
END END END END END END END  as Current_LOB




       ,CASE WHEN Has_Commercial_ugap is null AND Has_Medicare_ugap is null AND Has_Medicaid_ugap is null THEN NULL
       ELSE 
CASE WHEN Has_Commercial_ugap = 1 AND Has_Medicare_ugap = 1 AND Has_Medicaid_ugap = 1 THEN 'All' ELSE 
CASE WHEN Has_Commercial_ugap = 1 AND Has_Medicare_ugap = 1 THEN 'Commercial + Medicare' ELSE
CASE WHEN Has_Commercial_ugap = 1 AND Has_Medicaid_ugap = 1 THEN 'Commercial + Medicaid' ELSE
CASE WHEN Has_Medicare_ugap = 1 AND Has_Medicaid_ugap = 1 THEN 'Medicare + Medicaid' ELSE
CASE WHEN Has_Commercial_ugap = 1 THEN 'Commercial Only' ELSE
CASE WHEN Has_Medicare_ugap = 1 THEN 'Medicare Only' ELSE
CASE WHEN Has_Medicaid_ugap = 1 THEN 'Medicaid Only' ELSE 'Not Mapped' 
END END END END END END END END as LOB_UGAP




	,[LOBPrevious] as  Previous_LOB



	
	,[PC_Treatment_Indicator] as PC_Current_Treatment_Indicator
	,[PC_Treatment_Indicator_Previous] as PC_Previous_Treatment_Indicator
	,[PC_Spec_Episode_Count] as PC_Spec_Episode_Cnt
	,[PC_Spec_Episode_Distribution] 
	,[PC_Spec_Percent_of_Episodes] as PC_Spec_Perc_of_Episodes
	,[PC_Spec_Total_Cost] as PC_Spec_Tot_Cost
	,[PC_Spec_Average_Cost] as PC_Spec_Avg_Cost
	,PC_Normalized_Pricing_Episode_Count as PC_Spec_Normalized_Pricing
	,[PC_Spec_CV]


	,[PC_Attribution] as PC_Current_Attribution
	,[PC_Attribution_Previous] as PC_Prev_Attribution
	,[PC_Change_Comments]
	,[EC_Treatment_Indicator] as EC_Current_Treatment_Indicator
	,[EC_Treatment_Indicator_Previous] as EC_Previous_Treatment_Indicator
	,[EC_Spec_Episode_Count] as EC_Spec_Episode_Cnt
	,[EC_Spec_Episode_Distribution]  
	,[EC_Spec_Percent_of_Episodes] as EC_Spec_Perc_of_Episodes
	,[EC_Spec_Total_Cost] as EC_Spec_Tot_Cost
	,[EC_Spec_Average_Cost] as EC_Spec_Avg_Cost





	,CASE WHEN [EC_Normalized_Pricing_Total_Cost] IS NOT NULL THEN [EC_Normalized_Pricing_Total_Cost] ELSE 0 END as EC_Spec_Normalized_Pricing



	,[EC_Spec_CV]
	,[EC_Mapping] as EC_Current_Mapping
	,[EC_Mapping_Previous] as EC_Previous_Mapping
	,EC_Changes_Made
	,[EC_Change_Comments]
	,[PC_Measure_Status]


       --,CASE WHEN PC_Attribution = 'Not Mapped' OR (Has_Commercial_ugap is null AND Has_Medicare_ugap is null AND Has_Medicaid_ugap is null ) THEN 'Not Mapped' ELSE 
       --CASE WHEN ISNULL(Has_Commercial_ugap,'') <> ISNULL(Has_Commercial,'') OR ISNULL(Has_Medicare_ugap,'') <> ISNULL(Has_Medicare,'') OR  ISNULL(Has_Medicaid_ugap,'') <> ISNULL(Has_Medicaid,'') THEN 'Drop' ELSE 'Keep' END END as UGAP_Changes
	   ,CASE WHEN PC_Attribution = 'Not Mapped' THEN 'Not Mapped' ELSE 
       CASE WHEN ISNULL(Has_Commercial_ugap,'') <> ISNULL(Has_Commercial,'') OR ISNULL(Has_Medicare_ugap,'') <> ISNULL(Has_Medicare,'') OR  ISNULL(Has_Medicaid_ugap,'') <> ISNULL(Has_Medicaid,'') THEN 'Drop' ELSE 'Keep' END END as UGAP_Changes



    --   ,CASE WHEN Has_Commercial_ugap is null AND Has_Medicare_ugap is null AND Has_Medicaid_ugap is null AND  PC_Attribution <> 'Not Mapped' THEN 'Y' ELSE 
	   
	   --CASE WHEN (ISNULL(Has_Commercial_ugap,'') <> '' OR  ISNULL(Has_Medicare_ugap,'') <> '' OR ISNULL(Has_Medicaid_ugap,'') <> '')  AND  PC_Attribution = 'Not Mapped' THEN 'Y' ELSE 
	   --'N'
    --   END END as Is_Flagged

	   ,CASE WHEN (ISNULL(Has_Commercial_ugap,'') <> '' OR  ISNULL(Has_Medicare_ugap,'') <> '' OR ISNULL(Has_Medicaid_ugap,'') <> '')  AND  PC_Attribution = 'Not Mapped' THEN 'Y' ELSE 
	   'N'
       END as Is_Flagged  ,ETG_Fact_Symmetry_Id



  FROM [etgsymm].[VW_ETG_Symmetry_Main_Interface] v
  LEFT OUTER JOIN 
(

	SELECT DISTINCT 

	main.[BASEETG], 
	main.[SPECIALTY],

			 CASE WHEN comm.[RISK_MODEL] IS NULL THEN 0 ELSE 1 END as Has_Commercial_ugap,
			 CASE WHEN medcr.[RISK_MODEL] IS NULL THEN 0 ELSE 1 END as Has_Medicare_ugap,
			 CASE WHEN medcd.[RISK_MODEL] IS NULL THEN 0 ELSE 1 END as Has_Medicaid_ugap

			 FROM (

	SELECT [SPECIALTY]
		  ,[BASEETG]
	  FROM [vct].[UGAPCFG_ETG_TI_RX_NRX_COM]
	  UNION ALL
	  SELECT [SPECIALTY]
		  ,[BASEETG]
		FROM [vct].[UGAPCFG_ETG_TI_RX_NRX_MCD]
			 UNION ALL
		   SELECT [SPECIALTY]
		  ,[BASEETG]
		   FROM [vct].[UGAPCFG_ETG_TI_RX_NRX_MCR]

		   ) main
	LEFT JOIN (
	SELECT distinct [SPECIALTY]
		  ,[BASEETG]
		  ,1 as [RISK_MODEL]
	  FROM [vct].[UGAPCFG_ETG_TI_RX_NRX_COM]
	  ) comm ON main.BASEETG = comm.BASEETG AND main.SPECIALTY = comm.SPECIALTY 

	  LEFT JOIN (
	SELECT distinct [SPECIALTY]
		  ,[BASEETG]
		  ,1 as [RISK_MODEL]
	  FROM [vct].[UGAPCFG_ETG_TI_RX_NRX_MCR]
	  ) medcr ON main.BASEETG = medcr.BASEETG AND main.SPECIALTY = medcr.SPECIALTY 
	   LEFT JOIN (
	SELECT distinct [SPECIALTY]
		  ,[BASEETG]
		  ,1 as [RISK_MODEL]
	  FROM [vct].[UGAPCFG_ETG_TI_RX_NRX_MCD]
	  ) medcd ON main.BASEETG = medcd.BASEETG AND main.SPECIALTY = medcd.SPECIALTY 



)i ON i.[BASEETG] = v.ETG_Base_Class AND i.[SPECIALTY] = v.Premium_Specialty

  ) t