USE [VCT_DB]
GO

INSERT INTO [etgsymm].[ETG_Fact_Symmetry]
           ([ETG_Base_Class]
           ,[Premium_Specialty_Id]
		   ,[Never_Mapped]

		,[Has_RX]
		,[Has_NRX]

           ,[Has_Commercial]
           ,[Has_Medicare]
           ,[Has_Medicaid]


           ,[PC_Treatment_Indicator]
          
           
           ,[PC_Attribution]

           ,[PC_Change_Comments]

		     ,[PD_Version]
          
           ,[Symmetry_Version]

         )

SELECT  [ETG_Base_Class],
      p.Premium_Specialty_Id,

 CASE WHEN m.[Never_Map] = 'Y' THEN 1 ELSE 0 END AS [Never_Map], 

CASE WHEN LOWER(REPLACE(m.[Current_Rx_NRx], ' ', '')) LIKE  'rx:y/%'  THEN 1 ELSE 0 END AS [Has_RX] , 

 CASE WHEN LOWER(REPLACE(m.[Current_Rx_NRx], ' ', '')) LIKE '%/nrx:y'  THEN 1 ELSE 0 END AS [Has_NRX], 


CASE WHEN LOWER(REPLACE(m.[Current_LOB], ' ', ''))  = 'notmapped' then NULL ELSE 
CASE WHEN LOWER(REPLACE(m.[Current_LOB], ' ', '')) LIKE  '%commercial%' OR  LOWER(REPLACE(m.[Current_LOB], ' ', '')) = 'all' THEN 1 
ELSE 0 END END AS [Has_Commercial], 


CASE WHEN LOWER(REPLACE(m.[Current_LOB], ' ', ''))  = 'notmapped' then NULL ELSE 
CASE WHEN LOWER(REPLACE(m.[Current_LOB], ' ', '')) LIKE  '%medicare%' OR  LOWER(REPLACE(m.[Current_LOB], ' ', '')) = 'all' THEN 1 
ELSE 0 END END AS [Has_Medicare], 


CASE WHEN LOWER(REPLACE(m.[Current_LOB], ' ', ''))  = 'notmapped' then NULL ELSE 
CASE WHEN LOWER(REPLACE(m.[Current_LOB], ' ', '')) LIKE  '%medicaid%' OR  LOWER(REPLACE(m.[Current_LOB], ' ', '')) = 'all' THEN 1 
ELSE 0 END END AS [Has_Medicaid], 



      [PC_Current_Treatment_Indicator],

      [PC_Current_Attribution],


      [PC_Change_Comments]
	  , 18 as [PD_Version]
	  , 13 as [Symmetry_Version]
  FROM [VCPostDeploy].[dbo].[PD18_Mapping_Symmetry] m
  INNER JOIN [VCT_DB].[vct].[ETG_Dim_Premium_Spec_Master] p ON p.Premium_Specialty = m.Premium_Specialty
