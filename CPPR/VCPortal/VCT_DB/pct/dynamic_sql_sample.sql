﻿
--CREATE RANK TMP TABLE
--CREATE RANK TMP TABLE
--CREATE RANK TMP TABLE
IF OBJECT_ID('tempdb..#Rank') IS NOT NULL DROP TABLE #Rank 
SELECT   t.px,
         t.px_desc,
         t.Y1Q1_allw_amt,
         t.Y2Q1_allw_amt,
         (t.Y2Q1_allw_amt - t.Y1Q1_allw_amt) as Y1Q1_Y2Q1_diff
INTO     #Rank
FROM     (SELECT   px,
                   px_desc,
                   sum(case when year = 2021 and quarter = 2 then allw_amt end) as Y1Q1_allw_amt,
                   sum(case when year = 2022 and quarter = 2 then allw_amt end) as Y2Q1_allw_amt
          FROM     pct.CLM_OP a
          WHERE    a.op_phys_bucket = 'OP'
          AND      a.LOB in ('EI')
          AND      a.mapping_state in ('NEW JERSEY')
          GROUP BY px, px_desc) t;




--CREATE MEMBER MONTH TMP TABLE
--CREATE MEMBER MONTH TMP TABLE
--CREATE MEMBER MONTH TMP TABLE
IF OBJECT_ID('tempdb..#MemberMonth') IS  NOT NULL DROP TABLE #MemberMonth 
SELECT   Distinct TOP 100 t.Metric,
         t.Y1Q1_Mbr_Month,
         t.Y1Q2_Mbr_Month,
         t.Y1Q3_Mbr_Month,
         t.Y1Q4_Mbr_Month,
         t.Y2Q1_Mbr_Month,
         t.Y2Q2_Mbr_Month,
         t.Y2Q3_Mbr_Month,
         t.Y2Q4_Mbr_Month,
         CASE 
              WHEN t.Y1Q1_Mbr_Month = 0 THEN 'N/A' 
              ELSE CAST(FORMAT(((t.Y2Q1_Mbr_Month - t.Y1Q1_Mbr_Month)/t.Y1Q1_Mbr_Month),'P') as varchar) 
         END as Y1Q1_Y2Q1_trend,
         CASE 
              WHEN t.Y1Q2_Mbr_Month = 0 THEN 'N/A' 
              ELSE CAST(FORMAT(((t.Y2Q2_Mbr_Month - t.Y1Q2_Mbr_Month)/t.Y1Q2_Mbr_Month),'P') as varchar) 
         END as Y1Q2_Y2Q2_trend,
         CASE 
              WHEN t.Y1Q3_Mbr_Month = 0 THEN 'N/A' 
              ELSE CAST(FORMAT(((t.Y2Q3_Mbr_Month - t.Y1Q3_Mbr_Month)/t.Y1Q3_Mbr_Month),'P') as varchar) 
         END as Y1Q3_Y2Q3_trend,
         CASE 
              WHEN t.Y1Q4_Mbr_Month = 0 THEN 'N/A' 
              ELSE CAST(FORMAT(((t.Y2Q4_Mbr_Month - t.Y1Q4_Mbr_Month)/t.Y1Q4_Mbr_Month),'P') as varchar) 
         END as Y1Q4_Y2Q4_trend
INTO     #MemberMonth
FROM     (SELECT   Distinct 'Member Month' as Metric,
                   sum(case when a.year = 2021 and a.quarter = 2 then Mbr_Month end) as Y1Q1_Mbr_Month,
                   sum(case when a.year = 2021 and a.quarter = 3 then Mbr_Month end) as Y1Q2_Mbr_Month,
                   sum(case when a.year = 2021 and a.quarter = 4 then Mbr_Month end) as Y1Q3_Mbr_Month,
                   sum(case when a.year = 2022 and a.quarter = 1 then Mbr_Month end) as Y1Q4_Mbr_Month,
                   sum(case when a.year = 2022 and a.quarter = 2 then Mbr_Month end) as Y2Q1_Mbr_Month,
                   sum(case when a.year = 2022 and a.quarter = 3 then Mbr_Month end) as Y2Q2_Mbr_Month,
                   sum(case when a.year = 2022 and a.quarter = 4 then Mbr_Month end) as Y2Q3_Mbr_Month,
                   sum(case when a.year = 2023 and a.quarter = 1 then Mbr_Month end) as Y2Q4_Mbr_Month
          FROM     pct.MM_FINAL a
          WHERE    1 = 1
          AND      a.LOB in ('EI')
          AND      a.mapping_state in ('NEW JERSEY')) t;




--Unique Individual
--Unique Individual
--Unique Individual
SELECT   Distinct TOP 100 t.px,
         t.px_desc,
         t.Y1Q1_indv,
         t.Y1Q2_indv,
         t.Y1Q3_indv,
         t.Y1Q4_indv,
         t.Y2Q1_indv,
         t.Y2Q2_indv,
         t.Y2Q3_indv,
         t.Y2Q4_indv,
         CASE 
              WHEN t.Y1Q1_indv = 0 THEN 'N/A' 
              ELSE CAST(FORMAT(((t.Y2Q1_indv - t.Y1Q1_indv)/t.Y1Q1_indv),'P') as varchar) 
         END as Y1Q1_Y2Q1_trend,
         CASE 
              WHEN t.Y1Q2_indv = 0 THEN 'N/A' 
              ELSE CAST(FORMAT(((t.Y2Q2_indv - t.Y1Q2_indv)/t.Y1Q2_indv),'P') as varchar) 
         END as Y1Q2_Y2Q2_trend,
         CASE 
              WHEN t.Y1Q3_indv = 0 THEN 'N/A' 
              ELSE CAST(FORMAT(((t.Y2Q3_indv - t.Y1Q3_indv)/t.Y1Q3_indv),'P') as varchar) 
         END as Y1Q3_Y2Q3_trend,
         CASE 
              WHEN t.Y1Q4_indv = 0 THEN 'N/A' 
              ELSE CAST(FORMAT(((t.Y2Q4_indv - t.Y1Q4_indv)/t.Y1Q4_indv),'P') as varchar) 
         END as Y1Q4_Y2Q4_trend,
         t.rank
FROM     ( SELECT   Distinct a.px,
         a.px_desc,
         sum(case when a.year = 2021 and a.quarter = 2 then indv end) as Y1Q1_indv,
         sum(case when a.year = 2021 and a.quarter = 3 then indv end) as Y1Q2_indv,
         sum(case when a.year = 2021 and a.quarter = 4 then indv end) as Y1Q3_indv,
         sum(case when a.year = 2022 and a.quarter = 1 then indv end) as Y1Q4_indv,
         sum(case when a.year = 2022 and a.quarter = 2 then indv end) as Y2Q1_indv,
         sum(case when a.year = 2022 and a.quarter = 3 then indv end) as Y2Q2_indv,
         sum(case when a.year = 2022 and a.quarter = 4 then indv end) as Y2Q3_indv,
         sum(case when a.year = 2023 and a.quarter = 1 then indv end) as Y2Q4_indv,
         b.Y1Q1_Y2Q1_diff as rank
FROM     pct.CLM_OP a 
         left join #Rank b on a.px = b.px and a.px_desc = b.px_desc
WHERE    a.op_phys_bucket = 'OP'
AND      a.LOB in ('EI')
AND      a.mapping_state in ('NEW JERSEY')
GROUP BY b.Y1Q1_Y2Q1_diff, a.px, a.px_desc) t
ORDER BY t.rank DESC;



--Events
--Events
--Events
SELECT   Distinct TOP 100 t.px,
         t.px_desc,
         t.Y1Q1_events,
         t.Y1Q2_events,
         t.Y1Q3_events,
         t.Y1Q4_events,
         t.Y2Q1_events,
         t.Y2Q2_events,
         t.Y2Q3_events,
         t.Y2Q4_events,
         CASE 
              WHEN t.Y1Q1_events = 0 THEN 'N/A' 
              ELSE CAST(FORMAT(((t.Y2Q1_events - t.Y1Q1_events)/t.Y1Q1_events),'P') as varchar) 
         END as Y1Q1_Y2Q1_trend,
         CASE 
              WHEN t.Y1Q2_events = 0 THEN 'N/A' 
              ELSE CAST(FORMAT(((t.Y2Q2_events - t.Y1Q2_events)/t.Y1Q2_events),'P') as varchar) 
         END as Y1Q2_Y2Q2_trend,
         CASE 
              WHEN t.Y1Q3_events = 0 THEN 'N/A' 
              ELSE CAST(FORMAT(((t.Y2Q3_events - t.Y1Q3_events)/t.Y1Q3_events),'P') as varchar) 
         END as Y1Q3_Y2Q3_trend,
         CASE 
              WHEN t.Y1Q4_events = 0 THEN 'N/A' 
              ELSE CAST(FORMAT(((t.Y2Q4_events - t.Y1Q4_events)/t.Y1Q4_events),'P') as varchar) 
         END as Y1Q4_Y2Q4_trend,
         t.rank
FROM     ( SELECT   Distinct a.px,
         a.px_desc,
         sum(case when a.year = 2021 and a.quarter = 2 then evnts end) as Y1Q1_events,
         sum(case when a.year = 2021 and a.quarter = 3 then evnts end) as Y1Q2_events,
         sum(case when a.year = 2021 and a.quarter = 4 then evnts end) as Y1Q3_events,
         sum(case when a.year = 2022 and a.quarter = 1 then evnts end) as Y1Q4_events,
         sum(case when a.year = 2022 and a.quarter = 2 then evnts end) as Y2Q1_events,
         sum(case when a.year = 2022 and a.quarter = 3 then evnts end) as Y2Q2_events,
         sum(case when a.year = 2022 and a.quarter = 4 then evnts end) as Y2Q3_events,
         sum(case when a.year = 2023 and a.quarter = 1 then evnts end) as Y2Q4_events,
         b.Y1Q1_Y2Q1_diff as rank
FROM     pct.CLM_OP a 
         left join #Rank b on a.px = b.px and a.px_desc = b.px_desc
WHERE    a.op_phys_bucket = 'OP'
AND      a.LOB in ('EI')
AND      a.mapping_state in ('NEW JERSEY')
GROUP BY b.Y1Q1_Y2Q1_diff, a.px, a.px_desc ) t
ORDER BY t.rank DESC;




--Claims
--Claims
--Claims
SELECT   Distinct TOP 100 t.px,
         t.px_desc,
         t.Y1Q1_claims,
         t.Y1Q1_fac_claims,
         t.Y1Q1_phy_claims,
         t.Y1Q1_oth_claims,
         t.Y1Q2_claims,
         t.Y1Q2_fac_claims,
         t.Y1Q2_phy_claims,
         t.Y1Q2_oth_claims,
         t.Y1Q3_claims,
         t.Y1Q3_fac_claims,
         t.Y1Q3_phy_claims,
         t.Y1Q3_oth_claims,
         t.Y1Q4_claims,
         t.Y1Q4_fac_claims,
         t.Y1Q4_phy_claims,
         t.Y1Q4_oth_claims,
         t.Y2Q1_claims,
         t.Y2Q1_fac_claims,
         t.Y2Q1_phy_claims,
         t.Y2Q1_oth_claims,
         t.Y2Q2_claims,
         t.Y2Q2_fac_claims,
         t.Y2Q2_phy_claims,
         t.Y2Q2_oth_claims,
         t.Y2Q3_claims,
         t.Y2Q3_fac_claims,
         t.Y2Q3_phy_claims,
         t.Y2Q3_oth_claims,
         t.Y2Q4_claims,
         t.Y2Q4_fac_claims,
         t.Y2Q4_phy_claims,
         t.Y2Q4_oth_claims,
         CASE 
              WHEN t.Y1Q1_claims = 0 THEN 'N/A' 
              ELSE CAST(FORMAT(((t.Y2Q1_claims-t.Y1Q1_claims)/t.Y1Q1_claims),'P') as varchar) 
         END as Y1Q1_Y2Q1_trend_claims,
         CASE 
              WHEN t.Y1Q1_fac_claims = 0 THEN 'N/A' 
              ELSE CAST(FORMAT(((t.Y2Q1_fac_claims-t.Y1Q1_fac_claims)/t.Y1Q1_fac_claims),'P') as varchar) 
         END as Y1Q1_Y2Q1_trend_fac_claims,
         CASE 
              WHEN t.Y1Q1_phy_claims = 0 THEN 'N/A' 
              ELSE CAST(FORMAT(((t.Y2Q1_fac_claims-t.Y1Q1_phy_claims)/t.Y1Q1_phy_claims),'P') as varchar) 
         END Y1Q1_Y2Q1_trend_phy_claims,
         CASE 
              WHEN t.Y1Q1_oth_claims = 0 THEN 'N/A' 
              ELSE CAST(FORMAT(((t.Y2Q1_oth_claims-t.Y1Q1_oth_claims)/t.Y1Q1_oth_claims),'P') as varchar) 
         END Y1Q1_Y2Q1_trend_oth_claims,
         CASE 
              WHEN t.Y1Q2_claims = 0 THEN 'N/A' 
              ELSE CAST(FORMAT(((t.Y2Q2_claims-t.Y1Q2_claims)/t.Y1Q2_claims),'P') as varchar) 
         END as Y1Q2_Y2Q2_trend_claims,
         CASE 
              WHEN t.Y1Q2_fac_claims = 0 THEN 'N/A' 
              ELSE CAST(FORMAT(((t.Y2Q2_fac_claims-t.Y1Q2_fac_claims)/t.Y1Q2_fac_claims),'P') as varchar) 
         END as Y1Q2_Y2Q2_trend_fac_claims,
         CASE 
              WHEN t.Y1Q2_phy_claims = 0 THEN 'N/A' 
              ELSE CAST(FORMAT(((t.Y2Q2_fac_claims-t.Y1Q2_phy_claims)/t.Y1Q2_phy_claims),'P') as varchar) 
         END Y1Q2_Y2Q2_trend_phy_claims,
         CASE 
              WHEN t.Y1Q2_oth_claims = 0 THEN 'N/A' 
              ELSE CAST(FORMAT(((t.Y2Q2_oth_claims-t.Y1Q2_oth_claims)/t.Y1Q2_oth_claims),'P') as varchar) 
         END Y1Q2_Y2Q2_trend_oth_claims,
         CASE 
              WHEN t.Y1Q3_claims = 0 THEN 'N/A' 
              ELSE CAST(FORMAT(((t.Y2Q3_claims-t.Y1Q3_claims)/t.Y1Q3_claims),'P') as varchar) 
         END as Y1Q3_Y2Q3_trend_claims,
         CASE 
              WHEN t.Y1Q3_fac_claims = 0 THEN 'N/A' 
              ELSE CAST(FORMAT(((t.Y2Q3_fac_claims-t.Y1Q3_fac_claims)/t.Y1Q3_fac_claims),'P') as varchar) 
         END as Y1Q3_Y2Q3_trend_fac_claims,
         CASE 
              WHEN t.Y1Q3_phy_claims = 0 THEN 'N/A' 
              ELSE CAST(FORMAT(((t.Y2Q3_fac_claims-t.Y1Q3_phy_claims)/t.Y1Q3_phy_claims),'P') as varchar) 
         END Y1Q3_Y2Q3_trend_phy_claims,
         CASE 
              WHEN t.Y1Q3_oth_claims = 0 THEN 'N/A' 
              ELSE CAST(FORMAT(((t.Y2Q3_oth_claims-t.Y1Q3_oth_claims)/t.Y1Q3_oth_claims),'P') as varchar) 
         END Y1Q3_Y2Q3_trend_oth_claims,
         CASE 
              WHEN t.Y1Q4_claims = 0 THEN 'N/A' 
              ELSE CAST(FORMAT(((t.Y2Q4_claims-t.Y1Q4_claims)/t.Y1Q4_claims),'P') as varchar) 
         END as Y1Q4_Y2Q4_trend_claims,
         CASE 
              WHEN t.Y1Q4_fac_claims = 0 THEN 'N/A' 
              ELSE CAST(FORMAT(((t.Y2Q4_fac_claims-t.Y1Q4_fac_claims)/t.Y1Q4_fac_claims),'P') as varchar) 
         END as Y1Q4_Y2Q4_trend_fac_claims,
         CASE 
              WHEN t.Y1Q4_phy_claims = 0 THEN 'N/A' 
              ELSE CAST(FORMAT(((t.Y2Q4_fac_claims-t.Y1Q4_phy_claims)/t.Y1Q4_phy_claims),'P') as varchar) 
         END Y1Q4_Y2Q4_trend_phy_claims,
         CASE 
              WHEN t.Y1Q4_oth_claims = 0 THEN 'N/A' 
              ELSE CAST(FORMAT(((t.Y2Q4_oth_claims-t.Y1Q4_oth_claims)/t.Y1Q4_oth_claims),'P') as varchar) 
         END Y1Q4_Y2Q4_trend_oth_claims,
         t.rank
FROM     ( SELECT   Distinct a.px,
         a.px_desc,
         sum(case when a.year = 2021 and a.quarter = 2 then claims end) as Y1Q1_claims,
         sum(case when a.year = 2021 and a.quarter = 2 then fac_clms end) as Y1Q1_fac_claims,
         sum(case when a.year = 2021 and a.quarter = 2 then phy_clms end) as Y1Q1_phy_claims,
         sum(case when a.year = 2021 and a.quarter = 2 then oth_clms end) as Y1Q1_oth_claims,
         sum(case when a.year = 2021 and a.quarter = 3 then claims end) as Y1Q2_claims,
         sum(case when a.year = 2021 and a.quarter = 3 then fac_clms end) as Y1Q2_fac_claims,
         sum(case when a.year = 2021 and a.quarter = 3 then phy_clms end) as Y1Q2_phy_claims,
         sum(case when a.year = 2021 and a.quarter = 3 then oth_clms end) as Y1Q2_oth_claims,
         sum(case when a.year = 2021 and a.quarter = 4 then claims end) as Y1Q3_claims,
         sum(case when a.year = 2021 and a.quarter = 4 then fac_clms end) as Y1Q3_fac_claims,
         sum(case when a.year = 2021 and a.quarter = 4 then phy_clms end) as Y1Q3_phy_claims,
         sum(case when a.year = 2021 and a.quarter = 4 then oth_clms end) as Y1Q3_oth_claims,
         sum(case when a.year = 2022 and a.quarter = 1 then claims end) as Y1Q4_claims,
         sum(case when a.year = 2022 and a.quarter = 1 then fac_clms end) as Y1Q4_fac_claims,
         sum(case when a.year = 2022 and a.quarter = 1 then phy_clms end) as Y1Q4_phy_claims,
         sum(case when a.year = 2022 and a.quarter = 1 then oth_clms end) as Y1Q4_oth_claims,
         sum(case when a.year = 2022 and a.quarter = 2 then claims end) as Y2Q1_claims,
         sum(case when a.year = 2022 and a.quarter = 2 then fac_clms end) as Y2Q1_fac_claims,
         sum(case when a.year = 2022 and a.quarter = 2 then phy_clms end) as Y2Q1_phy_claims,
         sum(case when a.year = 2022 and a.quarter = 2 then oth_clms end) as Y2Q1_oth_claims,
         sum(case when a.year = 2022 and a.quarter = 3 then claims end) as Y2Q2_claims,
         sum(case when a.year = 2022 and a.quarter = 3 then fac_clms end) as Y2Q2_fac_claims,
         sum(case when a.year = 2022 and a.quarter = 3 then phy_clms end) as Y2Q2_phy_claims,
         sum(case when a.year = 2022 and a.quarter = 3 then oth_clms end) as Y2Q2_oth_claims,
         sum(case when a.year = 2022 and a.quarter = 4 then claims end) as Y2Q3_claims,
         sum(case when a.year = 2022 and a.quarter = 4 then fac_clms end) as Y2Q3_fac_claims,
         sum(case when a.year = 2022 and a.quarter = 4 then phy_clms end) as Y2Q3_phy_claims,
         sum(case when a.year = 2022 and a.quarter = 4 then oth_clms end) as Y2Q3_oth_claims,
         sum(case when a.year = 2023 and a.quarter = 1 then claims end) as Y2Q4_claims,
         sum(case when a.year = 2023 and a.quarter = 1 then fac_clms end) as Y2Q4_fac_claims,
         sum(case when a.year = 2023 and a.quarter = 1 then phy_clms end) as Y2Q4_phy_claims,
         sum(case when a.year = 2023 and a.quarter = 1 then oth_clms end) as Y2Q4_oth_claims,
         b.Y1Q1_Y2Q1_diff as rank
FROM     pct.CLM_OP a 
         left join #Rank b on a.px = b.px and a.px_desc = b.px_desc
WHERE    a.op_phys_bucket = 'OP'
AND      a.LOB in ('EI')
AND      a.mapping_state in ('NEW JERSEY')
GROUP BY b.Y1Q1_Y2Q1_diff, a.px, a.px_desc ) t
ORDER BY t.rank DESC;




--Allowed Amount
--Allowed Amount
--Allowed Amount
SELECT   Distinct TOP 100 t.px,
         t.px_desc,
         t.Y1Q1_allw_amt,
         t.Y1Q2_allw_amt,
         t.Y1Q3_allw_amt,
         t.Y1Q4_allw_amt,
         t.Y2Q1_allw_amt,
         t.Y2Q2_allw_amt,
         t.Y2Q3_allw_amt,
         t.Y2Q4_allw_amt,
         CASE 
              WHEN t.Y1Q1_allw_amt = 0 THEN 'N/A' 
              ELSE CAST(FORMAT(((t.Y2Q1_allw_amt - t.Y1Q1_allw_amt)/t.Y1Q1_allw_amt),'P') as varchar) 
         END as Y1Q1_Y2Q1_trend,
         CASE 
              WHEN t.Y1Q2_allw_amt = 0 THEN 'N/A' 
              ELSE CAST(FORMAT(((t.Y2Q2_allw_amt - t.Y1Q2_allw_amt)/t.Y1Q2_allw_amt),'P') as varchar) 
         END as Y1Q2_Y2Q2_trend,
         CASE 
              WHEN t.Y1Q3_allw_amt = 0 THEN 'N/A' 
              ELSE CAST(FORMAT(((t.Y2Q3_allw_amt - t.Y1Q3_allw_amt)/t.Y1Q3_allw_amt),'P') as varchar) 
         END as Y1Q3_Y2Q3_trend,
         CASE 
              WHEN t.Y1Q4_allw_amt = 0 THEN 'N/A' 
              ELSE CAST(FORMAT(((t.Y2Q4_allw_amt - t.Y1Q4_allw_amt)/t.Y1Q4_allw_amt),'P') as varchar) 
         END as Y1Q4_Y2Q4_trend,
         t.rank
FROM     ( SELECT   Distinct a.px,
         a.px_desc,
         sum(case when a.year = 2021 and a.quarter = 2 then allw_amt end) as Y1Q1_allw_amt,
         sum(case when a.year = 2021 and a.quarter = 3 then allw_amt end) as Y1Q2_allw_amt,
         sum(case when a.year = 2021 and a.quarter = 4 then allw_amt end) as Y1Q3_allw_amt,
         sum(case when a.year = 2022 and a.quarter = 1 then allw_amt end) as Y1Q4_allw_amt,
         sum(case when a.year = 2022 and a.quarter = 2 then allw_amt end) as Y2Q1_allw_amt,
         sum(case when a.year = 2022 and a.quarter = 3 then allw_amt end) as Y2Q2_allw_amt,
         sum(case when a.year = 2022 and a.quarter = 4 then allw_amt end) as Y2Q3_allw_amt,
         sum(case when a.year = 2023 and a.quarter = 1 then allw_amt end) as Y2Q4_allw_amt,
         b.Y1Q1_Y2Q1_diff as rank
FROM     pct.CLM_OP a 
         left join #Rank b on a.px = b.px and a.px_desc = b.px_desc
WHERE    a.op_phys_bucket = 'OP'
AND      a.LOB in ('EI')
AND      a.mapping_state in ('NEW JERSEY')
GROUP BY b.Y1Q1_Y2Q1_diff, a.px, a.px_desc) t
ORDER BY t.rank DESC;


--Member Month
--Member Month
--Member Month
SELECT * FROM #MemberMonth;


--Allowed Amount PMPM
--Allowed Amount PMPM
--Allowed Amount PMPM
SELECT   Distinct TOP 100 x.px,
         x.px_desc,
         x.Y1Q1_allw_PMPM,
         x.Y1Q2_allw_PMPM,
         x.Y1Q3_allw_PMPM,
         x.Y1Q4_allw_PMPM,
         x.Y2Q1_allw_PMPM,
         x.Y2Q2_allw_PMPM,
         x.Y2Q3_allw_PMPM,
         x.Y2Q4_allw_PMPM,
         CASE 
              WHEN x.Y1Q1_allw_PMPM = 0 THEN 'N/A' 
              ELSE CAST(FORMAT(((x.Y2Q1_allw_PMPM - x.Y1Q1_allw_PMPM)/x.Y1Q1_allw_PMPM),'P') as varchar) 
         END as Y1Q1_Y2Q1_trend,
         CASE 
              WHEN x.Y1Q2_allw_PMPM = 0 THEN 'N/A' 
              ELSE CAST(FORMAT(((x.Y2Q2_allw_PMPM - x.Y1Q2_allw_PMPM)/x.Y1Q2_allw_PMPM),'P') as varchar) 
         END as Y1Q2_Y2Q2_trend,
         CASE 
              WHEN x.Y1Q3_allw_PMPM = 0 THEN 'N/A' 
              ELSE CAST(FORMAT(((x.Y2Q3_allw_PMPM - x.Y1Q3_allw_PMPM)/x.Y1Q3_allw_PMPM),'P') as varchar) 
         END as Y1Q3_Y2Q3_trend,
         CASE 
              WHEN x.Y1Q4_allw_PMPM = 0 THEN 'N/A' 
              ELSE CAST(FORMAT(((x.Y2Q4_allw_PMPM - x.Y1Q4_allw_PMPM)/x.Y1Q4_allw_PMPM),'P') as varchar) 
         END as Y1Q4_Y2Q4_trend,
         y.Y1Q1_Y2Q1_diff as rank
FROM     (SELECT   Distinct a.px,
                   a.px_desc,
                   a.Y1Q1_allw_amt/(SELECT Y1Q1_Mbr_Month FROM #MemberMonth) as Y1Q1_allw_PMPM,
                   a.Y2Q1_allw_amt/(SELECT Y2Q1_Mbr_Month FROM #MemberMonth) as Y2Q1_allw_PMPM,
                   a.Y1Q2_allw_amt/(SELECT Y1Q2_Mbr_Month FROM #MemberMonth) as Y1Q2_allw_PMPM,
                   a.Y2Q2_allw_amt/(SELECT Y2Q2_Mbr_Month FROM #MemberMonth) as Y2Q2_allw_PMPM,
                   a.Y1Q3_allw_amt/(SELECT Y1Q3_Mbr_Month FROM #MemberMonth) as Y1Q3_allw_PMPM,
                   a.Y2Q3_allw_amt/(SELECT Y2Q3_Mbr_Month FROM #MemberMonth) as Y2Q3_allw_PMPM,
                   a.Y1Q4_allw_amt/(SELECT Y1Q4_Mbr_Month FROM #MemberMonth) as Y1Q4_allw_PMPM,
                   a.Y2Q4_allw_amt/(SELECT Y2Q4_Mbr_Month FROM #MemberMonth) as Y2Q4_allw_PMPM
          FROM     (SELECT   Distinct px,
                             px_desc,
                             sum(case when year = 2021 and quarter = 2 then allw_amt end) as Y1Q1_allw_amt,
                             sum(case when year = 2021 and quarter = 3 then allw_amt end) as Y1Q2_allw_amt,
                             sum(case when year = 2021 and quarter = 4 then allw_amt end) as Y1Q3_allw_amt,
                             sum(case when year = 2022 and quarter = 1 then allw_amt end) as Y1Q4_allw_amt,
                             sum(case when year = 2022 and quarter = 2 then allw_amt end) as Y2Q1_allw_amt,
                             sum(case when year = 2022 and quarter = 3 then allw_amt end) as Y2Q2_allw_amt,
                             sum(case when year = 2022 and quarter = 4 then allw_amt end) as Y2Q3_allw_amt,
                             sum(case when year = 2023 and quarter = 1 then allw_amt end) as Y2Q4_allw_amt
                    FROM     pct.CLM_OP
                    WHERE    op_phys_bucket = 'OP'
                    AND      LOB in ('EI')
                    AND      mapping_state in ('NEW JERSEY')
                    GROUP BY px, px_desc) a) x 
         left join #Rank y on x.px = y.px and x.px_desc = y.px_desc
ORDER BY y.Y1Q1_Y2Q1_diff DESC;




--Utilization000
--Utilization000
--Utilization000
SELECT   Distinct TOP 100 x.px,
         x.px_desc,
         x.Y1Q1_util000,
         x.Y1Q2_util000,
         x.Y1Q3_util000,
         x.Y1Q4_util000,
         x.Y2Q1_util000,
         x.Y2Q2_util000,
         x.Y2Q3_util000,
         x.Y2Q4_util000,
         CASE 
              WHEN x.Y1Q1_util000 = 0 THEN 'N/A' 
              ELSE CAST(FORMAT(((x.Y2Q1_util000 - x.Y1Q1_util000)/x.Y1Q1_util000),'P') as varchar) 
         END as Y1Q1_Y2Q1_trend,
         CASE 
              WHEN x.Y1Q2_util000 = 0 THEN 'N/A' 
              ELSE CAST(FORMAT(((x.Y2Q2_util000 - x.Y1Q2_util000)/x.Y1Q2_util000),'P') as varchar) 
         END as Y1Q2_Y2Q2_trend,
         CASE 
              WHEN x.Y1Q3_util000 = 0 THEN 'N/A' 
              ELSE CAST(FORMAT(((x.Y2Q3_util000 - x.Y1Q3_util000)/x.Y1Q3_util000),'P') as varchar) 
         END as Y1Q3_Y2Q3_trend,
         CASE 
              WHEN x.Y1Q4_util000 = 0 THEN 'N/A' 
              ELSE CAST(FORMAT(((x.Y2Q4_util000 - x.Y1Q4_util000)/x.Y1Q4_util000),'P') as varchar) 
         END as Y1Q4_Y2Q4_trend,
         y.Y1Q1_Y2Q1_diff as rank
FROM     (SELECT   Distinct a.px,
                   a.px_desc,
                   a.Y1Q1_px_cnt/(SELECT Y1Q1_Mbr_Month FROM #MemberMonth) as Y1Q1_util000,
                   a.Y2Q1_px_cnt/(SELECT Y2Q1_Mbr_Month FROM #MemberMonth) as Y2Q1_util000,
                   a.Y1Q2_px_cnt/(SELECT Y1Q2_Mbr_Month FROM #MemberMonth) as Y1Q2_util000,
                   a.Y2Q2_px_cnt/(SELECT Y2Q2_Mbr_Month FROM #MemberMonth) as Y2Q2_util000,
                   a.Y1Q3_px_cnt/(SELECT Y1Q3_Mbr_Month FROM #MemberMonth) as Y1Q3_util000,
                   a.Y2Q3_px_cnt/(SELECT Y2Q3_Mbr_Month FROM #MemberMonth) as Y2Q3_util000,
                   a.Y1Q4_px_cnt/(SELECT Y1Q4_Mbr_Month FROM #MemberMonth) as Y1Q4_util000,
                   a.Y2Q4_px_cnt/(SELECT Y2Q4_Mbr_Month FROM #MemberMonth) as Y2Q4_util000
          FROM     (SELECT   Distinct px,
                             px_desc,
                             sum(case when year = 2021 and quarter = 2 then px_cnt end) as Y1Q1_px_cnt,
                             sum(case when year = 2021 and quarter = 3 then px_cnt end) as Y1Q2_px_cnt,
                             sum(case when year = 2021 and quarter = 4 then px_cnt end) as Y1Q3_px_cnt,
                             sum(case when year = 2022 and quarter = 1 then px_cnt end) as Y1Q4_px_cnt,
                             sum(case when year = 2022 and quarter = 2 then px_cnt end) as Y2Q1_px_cnt,
                             sum(case when year = 2022 and quarter = 3 then px_cnt end) as Y2Q2_px_cnt,
                             sum(case when year = 2022 and quarter = 4 then px_cnt end) as Y2Q3_px_cnt,
                             sum(case when year = 2023 and quarter = 1 then px_cnt end) as Y2Q4_px_cnt
                    FROM     pct.CLM_OP
                    WHERE    op_phys_bucket = 'OP'
                    AND      LOB in ('EI')
                    AND      mapping_state in ('NEW JERSEY')
                    GROUP BY px, px_desc) a) x 
         left join #Rank y on x.px = y.px and x.px_desc = y.px_desc
ORDER BY y.Y1Q1_Y2Q1_diff DESC;



--Unit Cost 1
--Unit Cost 1
--Unit Cost 1
SELECT   t1.px,
         t1.px_desc,
         t1.Y1Q1_Unit_Cost1,
         t1.Y1Q2_Unit_Cost1,
         t1.Y1Q3_Unit_Cost1,
         t1.Y1Q4_Unit_Cost1,
         t1.Y2Q1_Unit_Cost1,
         t1.Y2Q2_Unit_Cost1,
         t1.Y2Q3_Unit_Cost1,
         t1.Y2Q4_Unit_Cost1,
         CASE 
              WHEN t1.Y1Q1_Unit_Cost1 = 0 THEN 'N/A' 
              ELSE CAST(FORMAT(((t1.Y2Q1_Unit_Cost1 - t1.Y1Q1_Unit_Cost1)/t1.Y1Q1_Unit_Cost1),'P') as varchar) 
         END as Y1Q1_Y2Q1_trend,
         CASE 
              WHEN t1.Y1Q2_Unit_Cost1 = 0 THEN 'N/A' 
              ELSE CAST(FORMAT(((t1.Y2Q2_Unit_Cost1 - t1.Y1Q2_Unit_Cost1)/t1.Y1Q2_Unit_Cost1),'P') as varchar) 
         END as Y1Q2_Y2Q2_trend,
         CASE 
              WHEN t1.Y1Q3_Unit_Cost1 = 0 THEN 'N/A' 
              ELSE CAST(FORMAT(((t1.Y2Q3_Unit_Cost1 - t1.Y1Q3_Unit_Cost1)/t1.Y1Q3_Unit_Cost1),'P') as varchar) 
         END as Y1Q3_Y2Q3_trend,
         CASE 
              WHEN t1.Y1Q4_Unit_Cost1 = 0 THEN 'N/A' 
              ELSE CAST(FORMAT(((t1.Y2Q4_Unit_Cost1 - t1.Y1Q4_Unit_Cost1)/t1.Y1Q4_Unit_Cost1),'P') as varchar) 
         END as Y1Q4_Y2Q4_trend
FROM     ( SELECT   Distinct t.px,
         t.px_desc,
         CASE 
              WHEN t.Y1Q1_events = 0 THEN NULL 
              ELSE t.Y1Q1_allw_amt/t.Y1Q1_events 
         END as Y1Q1_Unit_Cost1,
         CASE 
              WHEN t.Y2Q1_events = 0 THEN NULL 
              ELSE t.Y2Q1_allw_amt/t.Y2Q1_events 
         END as Y2Q1_Unit_Cost1,
         CASE 
              WHEN t.Y1Q2_events = 0 THEN NULL 
              ELSE t.Y1Q2_allw_amt/t.Y1Q2_events 
         END as Y1Q2_Unit_Cost1,
         CASE 
              WHEN t.Y2Q2_events = 0 THEN NULL 
              ELSE t.Y2Q2_allw_amt/t.Y2Q2_events 
         END as Y2Q2_Unit_Cost1,
         CASE 
              WHEN t.Y1Q3_events = 0 THEN NULL 
              ELSE t.Y1Q3_allw_amt/t.Y1Q3_events 
         END as Y1Q3_Unit_Cost1,
         CASE 
              WHEN t.Y2Q3_events = 0 THEN NULL 
              ELSE t.Y2Q3_allw_amt/t.Y2Q3_events 
         END as Y2Q3_Unit_Cost1,
         CASE 
              WHEN t.Y1Q4_events = 0 THEN NULL 
              ELSE t.Y1Q4_allw_amt/t.Y1Q4_events 
         END as Y1Q4_Unit_Cost1,
         CASE 
              WHEN t.Y2Q4_events = 0 THEN NULL 
              ELSE t.Y2Q4_allw_amt/t.Y2Q4_events 
         END as Y2Q4_Unit_Cost1,
         y.Y1Q1_Y2Q1_diff as rank
FROM     (SELECT   Distinct px,
                   px_desc,
                   sum(case when year = 2021 and quarter = 2 then allw_amt end) as Y1Q1_allw_amt,
                   sum(case when year = 2021 and quarter = 2 then evnts end) as Y1Q1_events,
                   sum(case when year = 2021 and quarter = 3 then allw_amt end) as Y1Q2_allw_amt,
                   sum(case when year = 2021 and quarter = 3 then evnts end) as Y1Q2_events,
                   sum(case when year = 2021 and quarter = 4 then allw_amt end) as Y1Q3_allw_amt,
                   sum(case when year = 2021 and quarter = 4 then evnts end) as Y1Q3_events,
                   sum(case when year = 2022 and quarter = 1 then allw_amt end) as Y1Q4_allw_amt,
                   sum(case when year = 2022 and quarter = 1 then evnts end) as Y1Q4_events,
                   sum(case when year = 2022 and quarter = 2 then allw_amt end) as Y2Q1_allw_amt,
                   sum(case when year = 2022 and quarter = 2 then evnts end) as Y2Q1_events,
                   sum(case when year = 2022 and quarter = 3 then allw_amt end) as Y2Q2_allw_amt,
                   sum(case when year = 2022 and quarter = 3 then evnts end) as Y2Q2_events,
                   sum(case when year = 2022 and quarter = 4 then allw_amt end) as Y2Q3_allw_amt,
                   sum(case when year = 2022 and quarter = 4 then evnts end) as Y2Q3_events,
                   sum(case when year = 2023 and quarter = 1 then allw_amt end) as Y2Q4_allw_amt,
                   sum(case when year = 2023 and quarter = 1 then evnts end) as Y2Q4_events
          FROM     pct.CLM_OP
          WHERE    op_phys_bucket = 'OP'
          AND      LOB in ('EI')
          AND      mapping_state in ('NEW JERSEY')
          GROUP BY px, px_desc) t 
         left join #Rank y on t.px = y.px and t.px_desc = y.px_desc ) t1
ORDER BY t1.rank DESC;


--Unit Cost 2
--Unit Cost 2
--Unit Cost 2
SELECT   t1.px,
         t1.px_desc,
         t1.Y1Q1_Unit_Cost2,
         t1.Y1Q2_Unit_Cost2,
         t1.Y1Q3_Unit_Cost2,
         t1.Y1Q4_Unit_Cost2,
         t1.Y2Q1_Unit_Cost2,
         t1.Y2Q2_Unit_Cost2,
         t1.Y2Q3_Unit_Cost2,
         t1.Y2Q4_Unit_Cost2,
         CASE 
              WHEN t1.Y1Q1_Unit_Cost2 = 0 THEN 'N/A' 
              ELSE CAST(FORMAT(((t1.Y2Q1_Unit_Cost2 - t1.Y1Q1_Unit_Cost2)/t1.Y1Q1_Unit_Cost2),'P') as varchar) 
         END as Y1Q1_Y2Q1_trend,
         CASE 
              WHEN t1.Y1Q2_Unit_Cost2 = 0 THEN 'N/A' 
              ELSE CAST(FORMAT(((t1.Y2Q2_Unit_Cost2 - t1.Y1Q2_Unit_Cost2)/t1.Y1Q2_Unit_Cost2),'P') as varchar) 
         END as Y1Q2_Y2Q2_trend,
         CASE 
              WHEN t1.Y1Q3_Unit_Cost2 = 0 THEN 'N/A' 
              ELSE CAST(FORMAT(((t1.Y2Q3_Unit_Cost2 - t1.Y1Q3_Unit_Cost2)/t1.Y1Q3_Unit_Cost2),'P') as varchar) 
         END as Y1Q3_Y2Q3_trend,
         CASE 
              WHEN t1.Y1Q4_Unit_Cost2 = 0 THEN 'N/A' 
              ELSE CAST(FORMAT(((t1.Y2Q4_Unit_Cost2 - t1.Y1Q4_Unit_Cost2)/t1.Y1Q4_Unit_Cost2),'P') as varchar) 
         END as Y1Q4_Y2Q4_trend
FROM     ( SELECT   Distinct t.px,
         t.px_desc,
         CASE 
              WHEN t.Y1Q1_adj_srv_uni = 0 THEN NULL 
              ELSE t.Y1Q1_allw_amt/t.Y1Q1_adj_srv_uni 
         END as Y1Q1_Unit_Cost2,
         CASE 
              WHEN t.Y2Q1_adj_srv_uni = 0 THEN NULL 
              ELSE t.Y2Q1_allw_amt/t.Y2Q1_adj_srv_uni 
         END as Y2Q1_Unit_Cost2,
         CASE 
              WHEN t.Y1Q2_adj_srv_uni = 0 THEN NULL 
              ELSE t.Y1Q2_allw_amt/t.Y1Q2_adj_srv_uni 
         END as Y1Q2_Unit_Cost2,
         CASE 
              WHEN t.Y2Q2_adj_srv_uni = 0 THEN NULL 
              ELSE t.Y2Q2_allw_amt/t.Y2Q2_adj_srv_uni 
         END as Y2Q2_Unit_Cost2,
         CASE 
              WHEN t.Y1Q3_adj_srv_uni = 0 THEN NULL 
              ELSE t.Y1Q3_allw_amt/t.Y1Q3_adj_srv_uni 
         END as Y1Q3_Unit_Cost2,
         CASE 
              WHEN t.Y2Q3_adj_srv_uni = 0 THEN NULL 
              ELSE t.Y2Q3_allw_amt/t.Y2Q3_adj_srv_uni 
         END as Y2Q3_Unit_Cost2,
         CASE 
              WHEN t.Y1Q4_adj_srv_uni = 0 THEN NULL 
              ELSE t.Y1Q4_allw_amt/t.Y1Q4_adj_srv_uni 
         END as Y1Q4_Unit_Cost2,
         CASE 
              WHEN t.Y2Q4_adj_srv_uni = 0 THEN NULL 
              ELSE t.Y2Q4_allw_amt/t.Y2Q4_adj_srv_uni 
         END as Y2Q4_Unit_Cost2,
         y.Y1Q1_Y2Q1_diff as rank
FROM     ( SELECT   Distinct px,
         px_desc,
         sum(case when year = 2021 and quarter = 2 then allw_amt end) as Y1Q1_allw_amt,
         sum(case when year = 2021 and quarter = 2then adj_srv_uni end) as Y1Q1_adj_srv_uni,
         sum(case when year = 2021 and quarter = 3 then allw_amt end) as Y1Q2_allw_amt,
         sum(case when year = 2021 and quarter = 3then adj_srv_uni end) as Y1Q2_adj_srv_uni,
         sum(case when year = 2021 and quarter = 4 then allw_amt end) as Y1Q3_allw_amt,
         sum(case when year = 2021 and quarter = 4then adj_srv_uni end) as Y1Q3_adj_srv_uni,
         sum(case when year = 2022 and quarter = 1 then allw_amt end) as Y1Q4_allw_amt,
         sum(case when year = 2022 and quarter = 1then adj_srv_uni end) as Y1Q4_adj_srv_uni,
         sum(case when year = 2022 and quarter = 2 then allw_amt end) as Y2Q1_allw_amt,
         sum(case when year = 2022 and quarter = 2then adj_srv_uni end) as Y2Q1_adj_srv_uni,
         sum(case when year = 2022 and quarter = 3 then allw_amt end) as Y2Q2_allw_amt,
         sum(case when year = 2022 and quarter = 3then adj_srv_uni end) as Y2Q2_adj_srv_uni,
         sum(case when year = 2022 and quarter = 4 then allw_amt end) as Y2Q3_allw_amt,
         sum(case when year = 2022 and quarter = 4then adj_srv_uni end) as Y2Q3_adj_srv_uni,
         sum(case when year = 2023 and quarter = 1 then allw_amt end) as Y2Q4_allw_amt,
         sum(case when year = 2023 and quarter = 1then adj_srv_uni end) as Y2Q4_adj_srv_uni
FROM     pct.CLM_OP
WHERE    op_phys_bucket = 'OP'
AND      LOB in ('EI')
AND      mapping_state in ('NEW JERSEY')
GROUP BY px, 
         px_desc ) t left join #Rank y on t.px = y.px and t.px_desc = y.px_desc ) t1
ORDER BY t1.rank DESC;

