CREATE VIEW [etg].[VW_ETG_Dataload_EC_AGG]
	AS select e5.PREM_SPCL_CD as Premium_Specialty,
	e5.ETG_BAS_CLSS_NBR as ETG_Base_Class,
	e5.ETG_TX_IND as EC_Treatment_Indicator,
	e5.EPSD_Epsd_Cnt as EC_Episode_Count,
	Cast(e5.EPSD_Tot_Cost as numeric(19, 2)) as EC_Total_Cost,
	Cast(e5.EPSD_Average_Cost as numeric(19, 2)) as EC_Average_Cost,
	Cast(e8.EPSD_CV as numeric(9, 4)) as EC_Coefficients_of_Variation,
	e6.EPSD_NP_Epsd_Cnt as EC_Normalized_Pricing_Episode_Count,
	Cast(e6.EPSD_NP_Tot_Cost as numeric(19, 2)) as EC_Normalized_Pricing_Total_Cost,
	e9.spcl_Epsd_Cnt as EC_Spec_Episode_Count,
	Cast(e9.spcl_Tot_Cost as numeric(19, 2)) as EC_Spec_Total_Cost,
	Cast(e9.spcl_Average_Cost as numeric(19, 2)) as EC_Spec_Average_Cost,
	Cast(e12.spcl_EPSD_CV as numeric(9, 4)) as EC_Spec_Coefficients_of_Variation,
	Cast(
		case 
				when e9.spcl_Epsd_Cnt is NULL then 0 
				else e9.spcl_Epsd_Cnt * 100.0 / e13.spcl_TOT_Epsd_Cnt 
			end as numeric(6, 2)) as EC_Spec_Percent_of_Episodes,
	e10.spcl_EPSD_NP_Epsd_Cnt as EC_Spec_Normalized_Pricing_Episode_Count,
	Cast(e10.Spcl_EPSD_NP_Tot_Cost as numeric(19, 2)) as EC_Spec_Normalized_Pricing_Total_Cost,
	case 
			when e12.spcl_EPSD_CV >= 3 then 'N' 
			when e12.spcl_EPSD_CV < 3 then 'Y' 
			else '' 
		end as EC_CV3,
	case 
			when e5.PREM_SPCL_CD in ('FAMED', 'INTMD', 'PEDS') and e9.spcl_Epsd_Cnt > 1000 then 'Y' 
			when e5.PREM_SPCL_CD in ('FAMED', 'INTMD', 'PEDS') and e9.spcl_Epsd_Cnt <= 1000 then 'N' 
			when e5.PREM_SPCL_CD not in ('FAMED', 'INTMD', 'PEDS') and e9.spcl_Epsd_Cnt > 500 then 'Y' 
			when e5.PREM_SPCL_CD not in ('FAMED', 'INTMD', 'PEDS') and e9.spcl_Epsd_Cnt <= 500 then 'N' 
			else '' 
		end as EC_Spec_Episode_Volume,
	e9.PD_Mapped 
from 
	(
	select spcl.PREM_SPCL_CD,
		e4.ETG_BAS_CLSS_NBR,
		e4.ETG_TX_IND,
		e4.EPSD_Epsd_Cnt,
		e4.EPSD_Tot_Cost,
		e4.EPSD_Average_Cost 
	from 
		(
		select Count(Distinct temptab.EPSD_NBR) as EPSD_Epsd_Cnt,
			Sum(temptab.TOT_ALLW_AMT) as EPSD_Tot_Cost,
			temptab.ETG_BAS_CLSS_NBR,
			temptab.ETG_TX_IND,
			Avg(temptab.TOT_ALLW_AMT) as EPSD_Average_Cost 
		from etg.VW_ETG_Dataload_PC_EC_Master as temptab 
		where temptab.ETG_TX_IND = 0 
			and temptab.PREM_SPCL_CD not in ('', 'GENSURG', 'GERIA', 'HEMAONC', 'PLASTIC', 'VASC', 'NONE', 'NONPAR') 
			and temptab.LOB_ID = 1 
		group by temptab.ETG_BAS_CLSS_NBR, temptab.ETG_TX_IND
		) e4, 
		(
		select trim(
				case 
						when b.PREM_SPCL_CD is NULL then 'NONE' 
						else b.PREM_SPCL_CD 
					end) as PREM_SPCL_CD 
		from etg.PrimarySpecWithCode_PDNDB_SOURCE b 
		where b.PREM_SPCL_CD not in ('', 'GENSURG', 'GERIA', 'HEMAONC', 'PLASTIC', 'VASC', 'NONE', 'NONPAR') 
		group by b.PREM_SPCL_CD
		) spcl
	) e5 
	left join 
	(
	select Count(Distinct temptab.EPSD_NBR) as EPSD_NP_Epsd_Cnt,
		Sum(temptab.TOT_NP_ALLW_AMT) as EPSD_NP_Tot_Cost,
		temptab.ETG_BAS_CLSS_NBR,
		temptab.ETG_TX_IND 
	from etg.VW_ETG_Dataload_PC_EC_Master as temptab 
	where temptab.ETG_TX_IND = 0 
		and temptab.PREM_SPCL_CD not in ('', 'GENSURG', 'GERIA', 'HEMAONC', 'PLASTIC', 'VASC', 'NONE', 'NONPAR') 
	group by temptab.ETG_BAS_CLSS_NBR, temptab.ETG_TX_IND
	) e6 on e5.ETG_BAS_CLSS_NBR = e6.ETG_BAS_CLSS_NBR and e5.ETG_TX_IND = e6.ETG_TX_IND 
	left join 
	(
	select E7.ETG_BAS_CLSS_NBR,
		E7.ETG_TX_IND,
		Sum(E7.TOT_CV) / Sum(E7.Epsd_Cnt) as EPSD_CV 
	from 
		(
		select Count(Distinct temptab.EPSD_NBR) as Epsd_Cnt,
			temptab.ETG_BAS_CLSS_NBR,
			temptab.ETG_TX_IND,
			((
					case 
							when StDev(temptab.TOT_ALLW_AMT) is NULL then 0 
							else StDev(temptab.TOT_ALLW_AMT) / Avg(temptab.TOT_ALLW_AMT) 
						end) * Count(Distinct temptab.EPSD_NBR)) as TOT_CV,
			temptab.PREM_SPCL_CD,
			temptab.SVRTY 
		from etg.VW_ETG_Dataload_PC_EC_Master as temptab 
		where temptab.ETG_TX_IND = 0 
			and temptab.PREM_SPCL_CD not in ('', 'GENSURG', 'GERIA', 'HEMAONC', 'PLASTIC', 'VASC', 'NONE', 'NONPAR') 
			and temptab.LOB_ID = 1 
		group by temptab.ETG_BAS_CLSS_NBR, temptab.ETG_TX_IND, temptab.PREM_SPCL_CD, temptab.SVRTY
		) E7 
	group by E7.ETG_BAS_CLSS_NBR, E7.ETG_TX_IND
	) e8 on e5.ETG_BAS_CLSS_NBR = e8.ETG_BAS_CLSS_NBR and e5.ETG_TX_IND = e8.ETG_TX_IND 
	left join 
	(
	select Count(Distinct temptab.EPSD_NBR) as spcl_Epsd_Cnt,
		Sum(temptab.TOT_ALLW_AMT) as spcl_Tot_Cost,
		temptab.ETG_BAS_CLSS_NBR,
		temptab.ETG_TX_IND,
		Avg(temptab.TOT_ALLW_AMT) as spcl_Average_Cost,
		temptab.PREM_SPCL_CD,
		temptab.PD_Mapped 
	from etg.VW_ETG_Dataload_PC_EC_Master as temptab 
	where temptab.ETG_TX_IND = 0 
		and temptab.PREM_SPCL_CD not in ('', 'GENSURG', 'GERIA', 'HEMAONC', 'PLASTIC', 'VASC', 'NONE', 'NONPAR') 
		and temptab.LOB_ID = 1 
	group by temptab.ETG_BAS_CLSS_NBR, temptab.ETG_TX_IND, temptab.PREM_SPCL_CD, temptab.PD_Mapped
	) e9 on e5.ETG_BAS_CLSS_NBR = e9.ETG_BAS_CLSS_NBR and e5.ETG_TX_IND = e9.ETG_TX_IND and e5.PREM_SPCL_CD = e9.PREM_SPCL_CD 
	left join 
	(
	select Count(Distinct temptab.EPSD_NBR) as spcl_EPSD_NP_Epsd_Cnt,
		Sum(temptab.TOT_NP_ALLW_AMT) as Spcl_EPSD_NP_Tot_Cost,
		temptab.ETG_BAS_CLSS_NBR,
		temptab.ETG_TX_IND,
		temptab.PREM_SPCL_CD 
	from etg.VW_ETG_Dataload_PC_EC_Master as temptab 
	where temptab.ETG_TX_IND = 0 
		and temptab.PREM_SPCL_CD not in ('', 'GENSURG', 'GERIA', 'HEMAONC', 'PLASTIC', 'VASC', 'NONE', 'NONPAR') 
	group by temptab.ETG_BAS_CLSS_NBR, temptab.ETG_TX_IND, temptab.PREM_SPCL_CD
	) e10 on e5.ETG_BAS_CLSS_NBR = e10.ETG_BAS_CLSS_NBR and e5.ETG_TX_IND = e10.ETG_TX_IND and e5.PREM_SPCL_CD = e10.PREM_SPCL_CD 
	left join 
	(
	select E11.ETG_BAS_CLSS_NBR,
		E11.ETG_TX_IND,
		Sum(E11.TOT_CV) / Sum(E11.Epsd_Cnt) as spcl_EPSD_CV,
		E11.PREM_SPCL_CD 
	from 
		(
		select Count(Distinct temptab.EPSD_NBR) as Epsd_Cnt,
			temptab.ETG_BAS_CLSS_NBR,
			temptab.ETG_TX_IND,
			((
					case 
							when StDev(temptab.TOT_ALLW_AMT) is NULL then 0 
							else StDev(temptab.TOT_ALLW_AMT) / Avg(temptab.TOT_ALLW_AMT) 
						end) * Count(Distinct temptab.EPSD_NBR)) as TOT_CV,
			temptab.PREM_SPCL_CD,
			temptab.SVRTY 
		from etg.VW_ETG_Dataload_PC_EC_Master as temptab 
		where temptab.ETG_TX_IND = 0 
			and temptab.PREM_SPCL_CD not in ('', 'GENSURG', 'GERIA', 'HEMAONC', 'PLASTIC', 'VASC', 'NONE', 'NONPAR') 
			and temptab.LOB_ID = 1 
		group by temptab.ETG_BAS_CLSS_NBR, temptab.ETG_TX_IND, temptab.PREM_SPCL_CD, temptab.SVRTY
		) E11 
	group by E11.ETG_BAS_CLSS_NBR, E11.ETG_TX_IND, E11.PREM_SPCL_CD
	) e12 on e5.ETG_BAS_CLSS_NBR = e12.ETG_BAS_CLSS_NBR and e5.ETG_TX_IND = e12.ETG_TX_IND and e5.PREM_SPCL_CD = e12.PREM_SPCL_CD 
	left join 
	(
	select Count(Distinct temptab.EPSD_NBR) as spcl_TOT_Epsd_Cnt,
		temptab.PREM_SPCL_CD 
	from etg.VW_ETG_Dataload_PC_EC_Master as temptab 
	where temptab.PREM_SPCL_CD not in ('', 'GENSURG', 'GERIA', 'HEMAONC', 'PLASTIC', 'VASC', 'NONE', 'NONPAR') 
		and temptab.ETG_TX_IND = 0 
		and temptab.LOB_ID = 1 
	group by temptab.PREM_SPCL_CD
	) e13 on e5.PREM_SPCL_CD = e13.PREM_SPCL_CD

