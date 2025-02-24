﻿CREATE VIEW [etg].[VW_ETG_Dataload_NRX_AGG]
	AS

	select b.ETG_BAS_CLSS_NBR as ETG_Base_Class,CNCR_IND,
       Concat('Rx: ', b.RX, ' / NRx: ', b.NRX) as RX_NRX,
       case 
                    when b.RX = 'Y' then 'True' 
                    else 'False' 
             end as Has_RX,
       case 
                    when b.NRX = 'Y' then 'True' 
                    else 'False' 
             end as Has_NRX,
       b.RX_RATE,
       b.RX,
       b.NRX ,
                           b.MEMBER_COUNT,
                    b.EPSD_COUNT,
                    b.ETGD_TOT_ALLW_AMT,
                    b.ETGD_RX_ALLW_AMT
from 
       (
       select a.ETG_BAS_CLSS_NBR,CNCR_IND,
             case 
                           when Sum(a.ETGD_TOT_ALLW_AMT) <> 0 and Sum(a.ETGD_RX_ALLW_AMT) <> 0 then Sum(a.ETGD_RX_ALLW_AMT) * 1.0 / Sum(a.ETGD_TOT_ALLW_AMT) 
                           else 0 
                    end as RX_RATE,
             case 
                           when Sum(a.ETGD_RX_ALLW_AMT) * 1.0 / Sum(a.ETGD_TOT_ALLW_AMT) > 0.3 then 'N' 
                           else 'Y' 
                    end as NRX,
             'Y' as RX ,

                    SUM(a.MEMBER_COUNT) as MEMBER_COUNT,
                    SUM(a.EPSD_COUNT) as EPSD_COUNT,
                    SUM(a.ETGD_TOT_ALLW_AMT) as ETGD_TOT_ALLW_AMT,
                    SUM(a.ETGD_RX_ALLW_AMT) as ETGD_RX_ALLW_AMT


       from 
             (
             select NRX.ETG_BAS_CLSS_NBR,
                    NRX.MEMBER_COUNT,
                    NRX.EPSD_COUNT,
                    NRX.ETGD_TOT_ALLW_AMT,
                    NRX.ETGD_RX_ALLW_AMT,
                    CF.CNCR_IND
                    /*case 
                                 when CF.CNCR_IND = 'N' then 10 
                                 else NRX.TRT_CD 
                           end as TRT_CD*/ 
             from etg.NRX_Cost_UGAP_SOURCE NRX 
             inner join etg.ETG_Cancer_Flag_PD_SOURCE CF on NRX.ETG_BAS_CLSS_NBR = CF.ETG_BASE_CLASS--1063
                    
             ) a 
       --where a.TRT_CD = 10 
       group by a.ETG_BAS_CLSS_NBR,CNCR_IND
       ) b


