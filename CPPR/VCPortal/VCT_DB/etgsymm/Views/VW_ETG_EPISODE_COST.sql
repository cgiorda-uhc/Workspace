CREATE VIEW [etgsymm].[VW_ETG_EPISODE_COST]
	AS SELECT 
pdm.PREM_SPCL_CD,
ec1.ETG_BAS_CLSS_NBR,  
ec1.[ETG_TX_IND], 
ec1.NP_Tot_Cost,
ec1.NP_Epsd_Cnt,
ec2.Tot_Cost,
ec2.Average_Cost,
ec2.Epsd_Cnt,
ec3.PD_Epsd_Cnt,
ec3.PD_CV_TOT
FROM --step 11: Summarize NP cost and episodes (all LOB’s)
(
       
	   SELECT etg.[ETG_BAS_CLSS_NBR], etg.[ETG_TX_IND], 
       SUM (etg.[TOT_NP_ALLW_AMT]) as NP_Tot_Cost, 
       COUNT(distinct etg.[EPSD_NBR]) as NP_Epsd_Cnt
       FROM [etgsymm].[VW_ETG_POP_EPSD_NRX] etg --MAIN VIEW
         WHERE etg.[PREM_SPCL_CD] NOT IN ('','GENSURG','GERIA','HEMAONC','PLASTIC','VASC','NONE','NONPAR') --Step 10: Filter the data from step 9 on
       and etg.[ETG_TX_IND] = 0
       GROUP BY etg.[ETG_BAS_CLSS_NBR], etg.[ETG_TX_IND]


) ec1

LEFT JOIN --Step 13: Summarize cost and episodes for Commercial LOB only
(
      
	  SELECT etg.[ETG_BAS_CLSS_NBR], etg.[ETG_TX_IND], SUM(etg.[TOT_ALLW_AMT]) as Tot_Cost, 
       AVG(etg.[TOT_ALLW_AMT]) as Average_Cost,
       COUNT(DISTINCT etg.[EPSD_NBR]) as Epsd_Cnt
       FROM [etgsymm].[VW_ETG_POP_EPSD_NRX] etg --MAIN VIEW
       WHERE etg.[LOB] = 'COMMERCIAL' --step 12 : filter the data from Step 10 on 
       GROUP BY etg.[ETG_BAS_CLSS_NBR], etg.[ETG_TX_IND]


) ec2 ON ec1.[ETG_BAS_CLSS_NBR] = ec2.[ETG_BAS_CLSS_NBR] AND ec1.[ETG_TX_IND] = ec2.[ETG_TX_IND]

LEFT JOIN --Step 15: Attach PD specialties to data from step 5 
(
       
	   SELECT distinct [PREM_SPCL_CD],[ETG_BASE_CLASS]
       FROM [vct].[ETG_Mapped_PD] 
       WHERE [PREM_SPCL_CD] NOT IN ('','GENSURG','GERIA','HEMAONC','PLASTIC','VASC','NONE','NONPAR')

       
) pdm ON pdm.[ETG_BASE_CLASS] = ec1.[ETG_BAS_CLSS_NBR]

LEFT JOIN 
(
	   --Step 18: Calculate CV for  Commercial LOB  
        SELECT e.[ETG_BAS_CLSS_NBR], e.[ETG_TX_IND], e.[PREM_SPCL_CD],  e.Epsd_Cnt as PD_Epsd_Cnt,
        CASE WHEN e.SD/Average_Cost IS NULL THEN 0 ELSE e.SD/Average_Cost END as PD_CV_TOT --step 17: calculate Coefficient of variation
        FROM ( --Step 16: Summarize cost and episodes for Commercial LOB only by premium specialty and severity
       
            SELECT etg.[ETG_BAS_CLSS_NBR], etg.[ETG_TX_IND],[PREM_SPCL_CD],
            SUM (etg.[TOT_ALLW_AMT]) as Tot_Cost,
            AVG(etg.[TOT_ALLW_AMT])  as Average_Cost,
            COUNT(DISTINCT etg.[EPSD_NBR])  as Epsd_Cnt,
            STDEV(etg.[TOT_ALLW_AMT]) as SD
            FROM [etgsymm].[VW_ETG_POP_EPSD_NRX] etg --MAIN VIEW
            GROUP BY etg.[ETG_BAS_CLSS_NBR], etg.[ETG_TX_IND],[PREM_SPCL_CD],[SVRTY]
        ) e

) ec3 ON ec1.[ETG_BAS_CLSS_NBR] = ec3.[ETG_BAS_CLSS_NBR] AND ec1.[ETG_TX_IND] = ec3.[ETG_TX_IND] AND ec3.[PREM_SPCL_CD] = pdm.[PREM_SPCL_CD] --Step 19: Join CV to data from Step 15 
