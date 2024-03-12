--SELECT * INTO  [VCT_DB].[vct].[ETG_Dim_Master_BAK31224]
--FROM  [VCT_DB].[vct].[ETG_Dim_Master]


TRUNCATE TABLE [VCT_DB].[vct].[ETG_Dim_Master];



TRUNCATE TABLE [VCT_DB].[vct].[ETG_Dim_Master];



INSERT INTO [VCT_DB].[vct].[ETG_Dim_Master]
           ([ETG_Base_Class]
           ,[ETG_Description]
           ,[ETG_Display])


SELECT distinct [ETG_Base_Class], MAX([ETG_Description]), MAX([ETG_Description]) FROM [VCPostDeploy].[dbo].[PD18_Mapping_Symmetry]
GROUP BY [ETG_Base_Class]

UNION

SELECT distinct [ETG_Base_Class], [ETG_Description], [ETG_Description] FROM [VCT_DB].[vct].[ETG_Dim_Master_BAK31224] 
WHERE [ETG_Description] IS NOT NULL 
AND [ETG_Base_Class] NOT IN (SELECT distinct [ETG_Base_Class] FROM [VCPostDeploy].[dbo].[PD18_Mapping_Symmetry])

