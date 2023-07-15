CREATE VIEW [etg].[VW_ETG_Dataload_PC_EC_Master]
	AS select e1.EPSD_NBR,
		e1.TOT_ALLW_AMT,
		e1.SVRTY,
		e1.ETG_BAS_CLSS_NBR,
		e1.PROV_MPIN,
		e1.TOT_NP_ALLW_AMT,
		e1.LOB_ID,
		e1.PREM_SPCL_CD,
		case 
				when e2.PREM_SPCL_CD is NULL and e2.TRT_CD is NULL and e2.ETG_BASE_CLASS is NULL then 'N' 
				else 'Y' 
			end as PD_Mapped,
		case 
				when e1.PREM_SPCL_CD in ('CARDVS', 'DERMA', 'GERIA', 'HEMAONC', 'VASC', 'PLASTIC', 'NONE', 'NONPAR') then 'N' 
				else 'Y' 
			end as PD_SPCL,
		case 
				when e3.CNCR_IND = 'N' then 0 
				else e1.ETG_TX_IND 
			end as TRT_CD,
		e1.ETG_TX_IND 
	from 
		(
		select a.EPSD_NBR,
			a.TOT_ALLW_AMT,
			a.SVRTY,
			a.ETG_BAS_CLSS_NBR,
			a.ETG_TX_IND,
			a.PROV_MPIN,
			a.TOT_NP_ALLW_AMT,
			a.LOB_ID,
			case 
					when a.PROV_MPIN = 0 then 'NONPAR' 
					when a.PROV_MPIN <> 0 and b.PREM_SPCL_CD is NULL then 'NONE' 
					else b.PREM_SPCL_CD 
				end as PREM_SPCL_CD 
		from etg.ETG_Episodes_UGAP_SOURCE a 
			left join etg.PrimarySpecWithCode_PDNDB_SOURCE b on a.PROV_MPIN = b.MPIN 
		where a.SVRTY <> ''
		) e1 
		left join etg.ETG_Mapped_PD_SOURCE e2 on e1.PREM_SPCL_CD = e2.PREM_SPCL_CD and e1.ETG_TX_IND = e2.TRT_CD and e1.ETG_BAS_CLSS_NBR = e2.ETG_BASE_CLASS 
		left join etg.ETG_Cancer_Flag_PD_SOURCE e3 on e1.ETG_BAS_CLSS_NBR = e3.ETG_BASE_CLASS 
	group by e1.EPSD_NBR, e1.TOT_ALLW_AMT, e1.SVRTY, e1.ETG_BAS_CLSS_NBR, e1.PROV_MPIN, e1.TOT_NP_ALLW_AMT, e1.LOB_ID, e1.PREM_SPCL_CD, case 
				when e2.PREM_SPCL_CD is NULL and e2.TRT_CD is NULL and e2.ETG_BASE_CLASS is NULL then 'N' 
				else 'Y' 
			end, case 
				when e1.PREM_SPCL_CD in ('CARDVS', 'DERMA', 'GERIA', 'HEMAONC', 'VASC', 'PLASTIC', 'NONE', 'NONPAR') then 'N' 
				else 'Y' 
			end, case 
				when e3.CNCR_IND = 'N' then 0 
				else e1.ETG_TX_IND 
			end, e1.ETG_TX_IND
