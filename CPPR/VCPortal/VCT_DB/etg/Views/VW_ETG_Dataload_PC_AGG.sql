CREATE VIEW [etg].[VW_ETG_Dataload_PC_AGG]
	AS select e5.PREM_SPCL_CD as Premium_Specialty,
	e5.ETG_BAS_CLSS_NBR as ETG_Base_Class,
	e5.Pop_Epsd_Cnt as PC_Episode_Count,
	Cast(e5.Pop_Tot_Cost as numeric(19, 2)) as PC_Total_Cost,
	Cast(e5.Pop_Average_Cost as numeric(19, 2)) as PC_Average_Cost,
	Cast(e8.Pop_CV as numeric(9, 4)) as PC_Coefficients_of_Variation,
	e6.Pop_NP_Epsd_Cnt as PC_Normalized_Pricing_Episode_Count,
	Cast(e6.Pop_NP_Tot_Cost as numeric(19, 2)) as PC_Normalized_Pricing_Total_Cost,
	e9.spcl_Pop_Epsd_Cnt as PC_Spec_Episode_Count,
	Cast(e9.spcl_Pop_Tot_Cost as numeric(19, 2)) as PC_Spec_Total_Cost,
	Cast(e9.spcl_Pop_Average_Cost as numeric(19, 2)) as PC_Spec_Average_Cost,
	Cast(e12.spcl_pop_EPSD_CV as numeric(9, 4)) as PC_Spec_CV,
	Cast(
		case 
				when e9.spcl_Pop_Epsd_Cnt is NULL then 0 
				else e9.spcl_Pop_Epsd_Cnt * 100.0 / e13.spcl_Pop_TOT_Epsd_Cnt 
			end as numeric(6, 2)) as PC_Spec_Percent_of_Episodes,
	e10.spcl_Pop_NP_Epsd_Cnt as PC_Spec_Normalized_Pricing_Episode_Count,
	Cast(e10.spcl_Pop_NP_Tot_Cost as numeric(19, 2)) as PC_Spec_Normalized_Pricing_Total_Cost,
	case 
			when e12.spcl_pop_EPSD_CV >= 3 then 'N' 
			when e12.spcl_pop_EPSD_CV < 3 then 'Y' 
			else '' 
		end as PC_CV3,
	case 
			when e5.PREM_SPCL_CD in ('FAMED', 'INTMD', 'PEDS') and e9.spcl_Pop_Epsd_Cnt > 1000 then 'Y' 
			when e5.PREM_SPCL_CD in ('FAMED', 'INTMD', 'PEDS') and e9.spcl_Pop_Epsd_Cnt <= 1000 then 'N' 
			when e5.PREM_SPCL_CD not in ('FAMED', 'INTMD', 'PEDS') and e9.spcl_Pop_Epsd_Cnt > 500 then 'Y' 
			when e5.PREM_SPCL_CD not in ('FAMED', 'INTMD', 'PEDS') and e9.spcl_Pop_Epsd_Cnt <= 500 then 'N' 
			else '' 
		end as PC_Spec_Epsd_Volume 
from 
	(
	select spcl.PREM_SPCL_CD,
		e4.ETG_BAS_CLSS_NBR,
		e4.Pop_Epsd_Cnt,
		e4.Pop_Tot_Cost,
		e4.Pop_Average_Cost,
		e4.totEpsd_Cnt 
	from 
		(
		select Count(Distinct temptab.EPSD_NBR) as Pop_Epsd_Cnt,
			Sum(temptab.TOT_ALLW_AMT) as Pop_Tot_Cost,
			temptab.ETG_BAS_CLSS_NBR,
			Avg(temptab.TOT_ALLW_AMT) as Pop_Average_Cost,
			Count(temptab.EPSD_NBR) as totEpsd_Cnt 
		from etg.VW_ETG_Dataload_PC_EC_Master as temptab 
		where temptab.TRT_CD = 0 
			and temptab.PREM_SPCL_CD not in ('', 'NONE') 
			and temptab.LOB_ID = 1 
		group by temptab.ETG_BAS_CLSS_NBR
		) e4, 
		(
		select trim(
				case 
						when b.PREM_SPCL_CD is NULL then 'NONE' 
						else b.PREM_SPCL_CD 
					end) as PREM_SPCL_CD 
		from etg.PrimarySpecWithCode_PDNDB_SOURCE b 
		where b.PREM_SPCL_CD not in ('', 'NONE') 
		group by b.PREM_SPCL_CD
		) spcl
	) e5 
	left join 
	(
	select Count(Distinct temptab.EPSD_NBR) as Pop_NP_Epsd_Cnt,
		Sum(temptab.TOT_NP_ALLW_AMT) as Pop_NP_Tot_Cost,
		temptab.ETG_BAS_CLSS_NBR 
	from etg.VW_ETG_Dataload_PC_EC_Master as temptab 
	where temptab.TRT_CD = 0 
		and temptab.PREM_SPCL_CD not in ('', 'NONE') 
	group by temptab.ETG_BAS_CLSS_NBR
	) e6 on e5.ETG_BAS_CLSS_NBR = e6.ETG_BAS_CLSS_NBR 
	left join 
	(
	select E7.ETG_BAS_CLSS_NBR,
		Sum(E7.TOT_CV) / Sum(E7.Epsd_Cnt) as Pop_CV 
	from 
		(
		select Count(Distinct temptab.EPSD_NBR) as Epsd_Cnt,
			temptab.ETG_BAS_CLSS_NBR,
			((
					case 
							when StDev(temptab.TOT_ALLW_AMT) is NULL then 0 
							else StDev(temptab.TOT_ALLW_AMT) / Avg(temptab.TOT_ALLW_AMT) 
						end) * Count(Distinct temptab.EPSD_NBR)) as TOT_CV,
			temptab.PREM_SPCL_CD 
		from etg.VW_ETG_Dataload_PC_EC_Master as temptab 
		where temptab.PREM_SPCL_CD not in ('', 'NONE') 
			and temptab.TRT_CD = 0 
			and temptab.LOB_ID = 1 
		group by temptab.ETG_BAS_CLSS_NBR, temptab.PREM_SPCL_CD
		) E7 
	group by E7.ETG_BAS_CLSS_NBR
	) e8 on e5.ETG_BAS_CLSS_NBR = e8.ETG_BAS_CLSS_NBR 
	left join 
	(
	select Count(Distinct temptab.EPSD_NBR) as spcl_Pop_Epsd_Cnt,
		Sum(temptab.TOT_ALLW_AMT) as spcl_Pop_Tot_Cost,
		temptab.ETG_BAS_CLSS_NBR,
		Avg(temptab.TOT_ALLW_AMT) as spcl_Pop_Average_Cost,
		temptab.PREM_SPCL_CD,
		Count(temptab.EPSD_NBR) as spcl_tot_Epsd_Cnt 
	from etg.VW_ETG_Dataload_PC_EC_Master as temptab 
	where temptab.PREM_SPCL_CD not in ('', 'NONE') 
		and temptab.TRT_CD = 0 
		and temptab.LOB_ID = 1 
	group by temptab.ETG_BAS_CLSS_NBR, temptab.PREM_SPCL_CD
	) e9 on e5.ETG_BAS_CLSS_NBR = e9.ETG_BAS_CLSS_NBR and e5.PREM_SPCL_CD = e9.PREM_SPCL_CD 
	left join 
	(
	select Count(Distinct temptab.EPSD_NBR) as spcl_Pop_NP_Epsd_Cnt,
		Sum(temptab.TOT_NP_ALLW_AMT) as spcl_Pop_NP_Tot_Cost,
		temptab.ETG_BAS_CLSS_NBR,
		temptab.PREM_SPCL_CD 
	from etg.VW_ETG_Dataload_PC_EC_Master as temptab 
	where temptab.PREM_SPCL_CD not in ('', 'NONE') 
		and temptab.TRT_CD = 0 
	group by temptab.ETG_BAS_CLSS_NBR, temptab.PREM_SPCL_CD
	) e10 on e5.ETG_BAS_CLSS_NBR = e10.ETG_BAS_CLSS_NBR and e5.PREM_SPCL_CD = e10.PREM_SPCL_CD 
	left join 
	(
	select E11.ETG_BAS_CLSS_NBR,
		Sum(E11.TOT_CV) / Sum(E11.Epsd_Cnt) as spcl_pop_EPSD_CV,
		E11.PREM_SPCL_CD 
	from 
		(
		select Count(Distinct temptab.EPSD_NBR) as Epsd_Cnt,
			temptab.ETG_BAS_CLSS_NBR,
			((
					case 
							when StDev(temptab.TOT_ALLW_AMT) is NULL then 0 
							else StDev(temptab.TOT_ALLW_AMT) / Avg(temptab.TOT_ALLW_AMT) 
						end) * Count(Distinct temptab.EPSD_NBR)) as TOT_CV,
			temptab.PREM_SPCL_CD 
		from etg.VW_ETG_Dataload_PC_EC_Master as temptab 
		where temptab.PREM_SPCL_CD not in ('', 'NONE') 
			and temptab.TRT_CD = 0 
			and temptab.LOB_ID = 1 
		group by temptab.ETG_BAS_CLSS_NBR, temptab.PREM_SPCL_CD
		) E11 
	group by E11.ETG_BAS_CLSS_NBR, E11.PREM_SPCL_CD
	) e12 on e5.ETG_BAS_CLSS_NBR = e12.ETG_BAS_CLSS_NBR and e5.PREM_SPCL_CD = e12.PREM_SPCL_CD 
	left join 
	(
	select Count(Distinct temptab.EPSD_NBR) as spcl_Pop_TOT_Epsd_Cnt,
		temptab.PREM_SPCL_CD 
	from etg.VW_ETG_Dataload_PC_EC_Master as temptab 
	where temptab.PREM_SPCL_CD not in ('', 'NONE') 
		and temptab.TRT_CD = 0 
		and temptab.LOB_ID = 1 
	group by temptab.PREM_SPCL_CD
	) e13 on e5.PREM_SPCL_CD = e13.PREM_SPCL_CD
