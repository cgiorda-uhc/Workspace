CREATE VIEW [etgsymm].[VW_ETG_POP_EPSD_NRX] AS
	--SELECT EPSD_NBR, TOT_ALLW_AMT, SVRTY,LOB ,TOT_NP_ALLW_AMT,ETG_BAS_CLSS_NBR, ETG_Description ,ETG_TX_IND, PREM_SPCL_CD, PD_SPCL,PD14_Mapped
	SELECT e.EPSD_NBR,[TOT_ALLW_AMT], [SVRTY], 

CASE WHEN [LOB_ID] = 1 THEN 'COMMERCIAL' ELSE CASE WHEN [LOB_ID] = 2 THEN 'MEDICARE' ELSE  'MEDICAID' END END as LOB ,

[TOT_NP_ALLW_AMT],

e.ETG_BAS_CLSS_NBR, d.[ETG_Description] ,e.ETG_TX_IND,

CASE WHEN e.[PROV_MPIN] = 0 THEN 'NONPAR' ELSE CASE WHEN e.[PROV_MPIN]  <> 0 AND t.PREM_SPCL_CD IS NULL THEN 'NONE' ELSE t.PREM_SPCL_CD END END  as PREM_SPCL_CD, 

--Create PD_SPCL flag as
CASE WHEN t.PREM_SPCL_CD IN ('CARDVS','DERMA','GERIA','HEMAONC','VASC','PLASTIC','NONE','NONPAR') THEN 'N' ELSE 'Y' END as PD_SPCL,

CASE WHEN m.TRT_CD  IS NOT NULL and t.PREM_SPCL_CD IS NOT NULL AND m.ETG_BASE_CLASS IS NOT NULL  THEN 'Y' ELSE'N' END as PD14_Mapped 


FROM (


SELECT   prim.MPIN,
         CASE 
              WHEN prim.[PREM_SPCL_CD] ='CARDCD' AND sec.[secondary_spec] = 'CARDEP' THEN 'CARDEP' 
              ELSE CASE 
                        WHEN prim.[PREM_SPCL_CD] in ('NS', 'ORTHO') THEN 'NOS' 
                        ELSE [PREM_SPCL_CD] 
                   END 
         END as [PREM_SPCL_CD]
FROM     (SELECT   [PREM_SPCL_CD],
                   [MPIN]
				   --Step 2: UHN data Query (NDB data) server -WP000074441CLS + step 4
          FROM     [vct].[PrimarySpecWithCode] 
          GROUP BY [PREM_SPCL_CD], [MPIN]) prim 
         LEFT JOIN (SELECT [Secondary_Spec], [MPIN] FROM [vct].[PrimarySpecWithCode] GROUP BY [Secondary_Spec], [MPIN]) sec ON prim.MPIN = sec.MPIN

) t
--Step 1: UGAP data Query
LEFT JOIN [vct].[ETG_Episodes_UGAP] e ON t.MPIN = e.PROV_MPIN 
-- Step 3 : Premium Specialties query ( server DBSWP0662)
LEFT JOIN [vct].[ETG_Mapped_PD] m ON m.PREM_SPCL_CD = t.PREM_SPCL_CD AND m.TRT_CD = e.ETG_TX_IND  AND m.ETG_BASE_CLASS = e.ETG_BAS_CLSS_NBR 
--ETG DESCRIPTION
LEFT JOIN [vct].[ETG_Dim_Master] d ON d.ETG_BASE_CLASS = e.ETG_BAS_CLSS_NBR
GO

