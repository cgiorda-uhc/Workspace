using Dapper;
using DataAccessLibrary.Data.Abstract;
using DataAccessLibrary.DataAccess;
using Microsoft.Extensions.Primitives;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using VCPortal_Models.Models.ProcCodeTrends;
using VCPortal_Models.Parameters.MHP;
using VCPortal_Models.Parameters.ProcCodeTrends;

namespace DataAccessLibrary.Data.Concrete.ProcCodeTrends
{
    public class ProcCodeTrends_Repo : IProcCodeTrends_Repo
    {
        private readonly IRelationalDataAccess _db;

        public ProcCodeTrends_Repo(IRelationalDataAccess db)
        {
            _db = db;
        }

        public Task<IEnumerable<MM_FINAL_Model>> GetMM_FINAL_Async(CancellationToken token)
        {

            string strSQL = "SELECT * FROM [VCT_DB].[pct].[MM_FINAL];";

            var results = _db.LoadData<MM_FINAL_Model>(sql: strSQL, token, connectionId: "VCT_DB");

            return results;
        }

        public Task<IEnumerable<CLM_PHYS_Model>> GetCLM_PHYS_Async(ProcCodeTrends_Parameters pct_param, CancellationToken token)
        {

            string strSQL = "SELECT * FROM [VCT_DB].[pct].[CLM_PHYS];";

            var results = _db.LoadData<CLM_PHYS_Model>(sql: strSQL, token, connectionId: "VCT_DB");

            return results;
        }


        public Task<IEnumerable<CLM_OP_Model>> GetCLM_OP_Async(ProcCodeTrends_Parameters pct_param, CancellationToken token)
        {

            string strSQL = "SELECT * FROM [VCT_DB].[pct].[CLM_OP];";

            var results = _db.LoadData<CLM_OP_Model>(sql: strSQL, token, connectionId: "VCT_DB");

            return results;
        }



        public Task<IEnumerable<DateSpan_Model>> GetDateSpan_Async(CancellationToken token)
        {

            //string strSQL = "SELECT MIN(year) as Year1, MAX(year) as Year2, (SELECT MIN(quarter) FROM [pct].[CLM_OP] WHERE year = (SELECT MIN(year) FROM [pct].[CLM_OP])) as First_Quarter FROM [pct].[CLM_OP];";
            string strSQL = "SELECT DISTINCT year, quarter  FROM [pct].[CLM_OP] ORDER BY year ASC, quarter asc";
            //string strSQL = "SELECT DISTINCT year, quarter  FROM [pct].[CLM_OP] ORDER BY  quarter asc, year asc";
            var results = _db.LoadData<DateSpan_Model>(sql: strSQL, token, connectionId: "VCT_DB");

            return results;
        }



        public async Task<CLM_OP_Report_Model> GetMainPCTReport_Async(ProcCodeTrends_Parameters pct_param,  CancellationToken token)
        {



            string filters = getFilterString(pct_param);
            StringBuilder sbSQL = new StringBuilder();


            string year = "";
            string quarter = "";


        

            //CREATE RANK TMP TABLE
            //CREATE RANK TMP TABLE
            //CREATE RANK TMP TABLE
            sbSQL.Append("IF OBJECT_ID('tempdb..#Rank') IS NOT NULL DROP TABLE #Rank SELECT t.px, t.px_desc, t.Y1Q1_allw_amt, t.Y2Q1_allw_amt, (t.Y2Q1_allw_amt - t.Y1Q1_allw_amt) as Y1Q1_Y2Q1_diff INTO #Rank FROM ( select px ,px_desc ,sum(case when year = "+ pct_param.DateSpanList[0].year + " and quarter = "+ pct_param.DateSpanList[0].quarter + " then allw_amt end) as Y1Q1_allw_amt ,sum(case when year = "+ pct_param.DateSpanList[4].year + " and quarter = "+ pct_param.DateSpanList[4].quarter + " then allw_amt end) as Y2Q1_allw_amt from pct.CLM_OP where op_phys_bucket = 'OP' " + filters + " group by px, px_desc ) t; ");


            //CREATE MemberMonth TMP TABLE START
            //CREATE MemberMonth TMP TABLE START
            //CREATE MemberMonth TMP TABLE START
            sbSQL.Append("IF OBJECT_ID('tempdb..#MemberMonth') IS  NOT NULL DROP TABLE #MemberMonth ");
            sbSQL.Append("SELECT DISTINCT TOP 10 t.Metric ");

            //LOOP DSM!!!
            for (int i = 0; i < pct_param.DateSpanList.Count; i++)
            {
                if (i <= 3)
                {
                    year = "1";
                    quarter = (i + 1).ToString();
                }
                else
                {
                    year = "2";
                    quarter = ((i + 1) - 4).ToString();
                }


                //sbSQL.Append(",t.Y" + ((i + 1) % 2 == 0 ? "2" : "1") + "Q" + pct_param.DateSpanList[i].quarter + "_Mbr_Month ");
                sbSQL.Append(",t.Y" + year + "Q" + quarter + "_Mbr_Month ");
            }

            //LOOP DSM!!
            for (int i = 1; i < 5; i++)
            {

                sbSQL.Append(", CASE WHEN t.Y1Q" + i+ "_Mbr_Month = 0 THEN 'N/A' ELSE  CAST(FORMAT(((t.Y2Q" + i + "_Mbr_Month - t.Y1Q" + i + "_Mbr_Month)/t.Y1Q" + i + "_Mbr_Month),'P') as varchar)  END as Y1Q" + i + "_Y2Q" + i + "_trend ");
     
            }
            sbSQL.Append("INTO #MemberMonth FROM ( select distinct 'Member Month' as Metric ");

            //LOOP DSM!!!
            for (int i = 0; i < pct_param.DateSpanList.Count; i++)
            {

                if(i <= 3)
                {
                    year = "1";
                    quarter = (i + 1).ToString();
                }
                else
                {
                    year = "2";
                    quarter = ((i + 1) - 4).ToString();
                }
               
                var year_full = pct_param.DateSpanList[i].year;
                var quarter_actual = pct_param.DateSpanList[i].quarter;

                sbSQL.Append(",sum(case when a.year = " + year_full + " and a.quarter = " + quarter_actual + " then Mbr_Month end) as Y" + year + "Q" + quarter + "_Mbr_Month ");
            }

            sbSQL.Append("from pct.MM_FINAL a where 1 = 1  " + filters + ") t; ");

            //CREATE MemberMonth TMP TABLE END
            //CREATE MemberMonth TMP TABLE END
            //CREATE MemberMonth TMP TABLE END



            //unique individual start
            //unique individual start
            //unique individual start
            sbSQL.Append("SELECT DISTINCT TOP 10000 t.px, t.px_desc ");

            //LOOP DSM!!!
            for (int i = 0; i < pct_param.DateSpanList.Count; i++)
            {

                if (i <= 3)
                {
                    year = "1";
                    quarter = (i + 1).ToString();
                }
                else
                {
                    year = "2";
                    quarter = ((i + 1) - 4).ToString();
                }

                // sbSQL.Append(",t.Y" + ((i + 1) % 2 == 0 ? "2" : "1") + "Q" + pct_param.DateSpanList[i].quarter + "_indv ");
                sbSQL.Append(",t.Y" + year + "Q" + quarter + "_indv ");

            }

            ////LOOP DSM!!
            //for (int i = 0; i < pct_param.DateSpanList.Count; i++)
            //{

            //    if (i % 2 != 0)
            //    {
            //        continue;
            //    }

            //    var quarter = pct_param.DateSpanList[i].quarter;

            //    sbSQL.Append(", CASE WHEN t.Y1Q" + quarter + "_indv = 0 THEN 'N/A' ELSE  CAST(((t.Y2Q" + quarter + "_indv - t.Y1Q" + quarter + "_indv)/t.Y1Q" + quarter + "_indv) as varchar)  END as Y1Q" + quarter + "_Y2Q" + quarter + "_trend ");
            //}
            //LOOP DSM!!
            for (int i = 1; i < 5; i++)
            {

                sbSQL.Append(", CASE WHEN t.Y1Q" + i + "_indv = 0 THEN 'N/A' ELSE  CAST(FORMAT(((t.Y2Q" + i + "_indv - t.Y1Q" + i + "_indv)/t.Y1Q" + i + "_indv),'P') as varchar)  END as Y1Q" + i + "_Y2Q" + i + "_trend ");

            }



            sbSQL.Append(" ,t.rank FROM ( select a.px ,a.px_desc ");

            //LOOP DSM!!!
            for (int i = 0; i < pct_param.DateSpanList.Count; i++)
            {

                //var year = ((i + 1) % 2 == 0 ? "2" : "1");
                //var quarter = pct_param.DateSpanList[i].quarter;
                //var year_full = pct_param.DateSpanList[i].year;

                //sbSQL.Append(",sum(case when a.year = " + year_full + " and a.quarter = " + quarter + " then indv end) as Y" + year + "Q" + quarter + "_indv ");

                if (i <= 3)
                {
                    year = "1";
                    quarter = (i + 1).ToString();
                }
                else
                {
                    year = "2";
                    quarter = ((i + 1) - 4).ToString();
                }

                var year_full = pct_param.DateSpanList[i].year;
                var quarter_actual = pct_param.DateSpanList[i].quarter;

                sbSQL.Append(",sum(case when a.year = " + year_full + " and a.quarter = " + quarter_actual + " then indv  end) as Y" + year + "Q" + quarter + "_indv ");


            }

            sbSQL.Append(",b.Y1Q1_Y2Q1_diff as rank from pct.CLM_OP a left join #Rank b on a.px = b.px and a.px_desc = b.px_desc where a.op_phys_bucket = 'OP'  " + filters + " group by b.Y1Q1_Y2Q1_diff,a.px, a.px_desc ) t order by t.rank DESC; ");
            //unique individual end
            //unique individual end
            //unique individual end


            ////events start
            ////events start
            ////events start
            //sbSQL.Append("SELECT DISTINCT TOP 10 t.px ,t.px_desc ");
            ////LOOP DSM!!!
            //for (int i = 0; i < pct_param.DateSpanList.Count; i++)
            //{
            //    if (i <= 3)
            //    {
            //        year = "1";
            //        quarter = (i + 1).ToString();
            //    }
            //    else
            //    {
            //        year = "2";
            //        quarter = ((i + 1) - 4).ToString();
            //    }


            //    //sbSQL.Append(",t.Y" + ((i + 1) % 2 == 0 ? "2" : "1") + "Q" + pct_param.DateSpanList[i].quarter + "_Mbr_Month ");
            //    sbSQL.Append(",t.Y" + year + "Q" + quarter + "_events ");
            //}


            ////LOOP DSM!!
            //for (int i = 1; i < 5; i++)
            //{

            //    sbSQL.Append(", CASE WHEN t.Y1Q" + i + "_events = 0 THEN 'N/A' ELSE  CAST(((t.Y2Q" + i + "_events - t.Y1Q" + i + "_events)/t.Y1Q" + i + "_events) as varchar)  END as Y1Q" + i + "_Y2Q" + i + "_trend ");

            //}


            //sbSQL.Append(",t.rank FROM ( select distinct a.px ,a.px_desc ");
            ////LOOP DSM!!!
            //for (int i = 0; i < pct_param.DateSpanList.Count; i++)
            //{


            //    if (i <= 3)
            //    {
            //        year = "1";
            //        quarter = (i + 1).ToString();
            //    }
            //    else
            //    {
            //        year = "2";
            //        quarter = ((i + 1) - 4).ToString();
            //    }

            //    var year_full = pct_param.DateSpanList[i].year;
            //    var quarter_actual = pct_param.DateSpanList[i].quarter;

            //    sbSQL.Append(",sum(case when a.year = " + year_full + " and a.quarter = " + quarter_actual + " then evnts end) as Y" + year + "Q" + quarter + "_events ");

            //}

            //sbSQL.Append(",b.Y1Q1_Y2Q1_diff as rank from pct.CLM_OP a left join #Rank b on a.px = b.px and a.px_desc = b.px_desc where a.op_phys_bucket = 'OP' " + filters + " group by b.Y1Q1_Y2Q1_diff,a.px, a.px_desc ) t order by t.rank DESC; ");
            ////events end
            ////events end
            ////events end



            ////claims start
            ////claims start
            ////claims start
            //sbSQL.Append("SELECT DISTINCT TOP 10 t.px ,t.px_desc ");
            ////LOOP DSM!!!
            //for (int i = 0; i < pct_param.DateSpanList.Count; i++)
            //{
            //    var year = ((i + 1) % 2 == 0 ? "2" : "1");
            //    var quarter = pct_param.DateSpanList[i].quarter;

            //    sbSQL.Append(",t.Y" + year + "Q" + quarter + "_claims ");
            //    sbSQL.Append(",t.Y" + year + "Q" + quarter + "_fac_claims ");
            //    sbSQL.Append(",t.Y" + year + "Q" + quarter + "_phy_claims ");
            //    sbSQL.Append(",t.Y" + year + "Q" + quarter + "_oth_claims ");
            //}
            ////LOOP DSM!!
            //for (int i = 0; i < pct_param.DateSpanList.Count; i++)
            //{
            //    if (i % 2 != 0)
            //    {
            //        continue;
            //    }

            //    //var year = ((i + 1) % 2 == 0 ? "2" : "1");
            //    var quarter = pct_param.DateSpanList[i].quarter;

            //    sbSQL.Append(",CASE WHEN t.Y1Q" + quarter + "_claims = 0 THEN 'N/A' ELSE  CAST(((t.Y2Q" + quarter + "_claims-t.Y1Q" + quarter + "_claims)/t.Y1Q" + quarter + "_claims) as varchar) END as Y1Q" + quarter + "_Y2Q" + quarter + "_trend_claims ");
            //    sbSQL.Append(",CASE WHEN t.Y1Q" + quarter + "_fac_claims = 0 THEN 'N/A' ELSE  CAST((t.Y2Q" + quarter + "_fac_claims-t.Y1Q" + quarter + "_fac_claims)/t.Y1Q" + quarter + "_fac_claims as varchar)  END as Y1Q" + quarter + "_Y2Q" + quarter + "_trend_fac_claims ");
            //    sbSQL.Append(",CASE WHEN t.Y1Q" + quarter + "_phy_claims = 0 THEN 'N/A' ELSE  CAST((t.Y2Q" + quarter + "_fac_claims-t.Y1Q" + quarter + "_phy_claims)/t.Y1Q" + quarter + "_phy_claims  as varchar) END Y1Q" + quarter + "_Y2Q" + quarter + "_trend_phy_claims ");
            //    sbSQL.Append(",CASE WHEN t.Y1Q" + quarter + "_oth_claims = 0 THEN 'N/A' ELSE  CAST((t.Y2Q" + quarter + "_oth_claims-t.Y1Q" + quarter + "_oth_claims)/t.Y1Q" + quarter + "_oth_claims as varchar)  END Y1Q" + quarter + "_Y2Q" + quarter + "_trend_oth_claims ");
            //}


            //sbSQL.Append(",t.rank FROM ( select distinct a.px ,a.px_desc ");
            ////LOOP DSM!!!
            //for (int i = 0; i < pct_param.DateSpanList.Count; i++)
            //{

            //    var year = ((i + 1) % 2 == 0 ? "2" : "1");
            //    var quarter = pct_param.DateSpanList[i].quarter;
            //    var year_full = pct_param.DateSpanList[i].year;

            //    sbSQL.Append(",sum(case when a.year = " + year_full + " and a.quarter = " + quarter + " then claims end) as Y" + year + "Q" + quarter + "_claims ");
            //    sbSQL.Append(",sum(case when a.year = " + year_full + " and a.quarter = " + quarter + " then fac_clms end) as Y" + year + "Q" + quarter + "_fac_claims ");
            //    sbSQL.Append(",sum(case when a.year = " + year_full + " and a.quarter = " + quarter + " then phy_clms end) as Y" + year + "Q" + quarter + "_phy_claims ");
            //    sbSQL.Append(",sum(case when a.year = " + year_full + " and a.quarter = " + quarter + " then oth_clms end) as Y" + year + "Q" + quarter + "_oth_claims ");

            //}

            //sbSQL.Append(",b.Y1Q1_Y2Q1_diff as rank from pct.CLM_OP a left join #Rank b on a.px = b.px and a.px_desc = b.px_desc where a.op_phys_bucket = 'OP' " + filters + " group by b.Y1Q1_Y2Q1_diff,a.px, a.px_desc ) t order by t.rank DESC; ");
            ////claims end
            ////claims end
            ////claims end




            ////allowed start
            ////allowed start
            ////allowed start
            //sbSQL.Append("SELECT DISTINCT TOP 10 t.px ,t.px_desc ");
            ////LOOP DSM!!!
            //for (int i = 0; i < pct_param.DateSpanList.Count; i++)
            //{
            //    var year = ((i + 1) % 2 == 0 ? "2" : "1");
            //    var quarter = pct_param.DateSpanList[i].quarter;

            //    sbSQL.Append(",t.Y" + year + "Q" + quarter + "_allw_amt ");
            //}
            ////LOOP DSM!!
            //for (int i = 0; i < pct_param.DateSpanList.Count; i++)
            //{

            //    if (i % 2 != 0)
            //    {
            //        continue;
            //    }
            //    var quarter = pct_param.DateSpanList[i].quarter;

            //    sbSQL.Append(", CASE WHEN t.Y1Q" + quarter + "_allw_amt = 0 THEN 'N/A' ELSE  CAST(((t.Y2Q" + quarter + "_allw_amt - t.Y1Q" + quarter + "_allw_amt)/t.Y1Q" + quarter + "_allw_amt) as varchar)  END as Y1Q" + quarter + "_Y2Q" + quarter + "_trend ");
            //}
            //sbSQL.Append(",t.rank FROM ( select distinct a.px ,a.px_desc ");
            ////LOOP DSM!!!
            //for (int i = 0; i < pct_param.DateSpanList.Count; i++)
            //{
            //    var year = ((i + 1) % 2 == 0 ? "2" : "1");
            //    var quarter = pct_param.DateSpanList[i].quarter;
            //    var year_full = pct_param.DateSpanList[i].year;

            //    sbSQL.Append(",sum(case when a.year = " + year_full + " and a.quarter = " + quarter + " then allw_amt end) as Y" + year + "Q" + quarter + "_allw_amt ");
            //}

            //sbSQL.Append(",b.Y1Q1_Y2Q1_diff as rank from pct.CLM_OP a left join #Rank b on a.px = b.px and a.px_desc = b.px_desc where a.op_phys_bucket = 'OP' " + filters + " group by b.Y1Q1_Y2Q1_diff,a.px, a.px_desc ) t order by t.rank DESC; ");
            ////allowed end
            ////allowed end
            ////allowed end


            ////member month start
            ////member month start
            ////member month start
            //sbSQL.Append("SELECT * FROM #MemberMonth; ");
            ////member month end
            ////member month end
            ////member month end


            ////Allowed PMPM start
            ////Allowed PMPM start
            ////Allowed PMPM start 
            //sbSQL.Append("SELECT DISTINCT TOP 10 x.px ,x.px_desc ");
            ////LOOP DSM!!!
            //for (int i = 0; i < pct_param.DateSpanList.Count; i++)
            //{
            //    var year = ((i + 1) % 2 == 0 ? "2" : "1");
            //    var quarter = pct_param.DateSpanList[i].quarter;

            //    sbSQL.Append(",x.Y" + year + "Q" + quarter + "_allw_PMPM ");
            //}
            ////LOOP DSM!!
            //for (int i = 0; i < pct_param.DateSpanList.Count; i++)
            //{

            //    if (i % 2 != 0)
            //    {
            //        continue;
            //    }
            //    var quarter = pct_param.DateSpanList[i].quarter;

            //    sbSQL.Append(", CASE WHEN x.Y1Q" + quarter + "_allw_PMPM = 0 THEN 'N/A' ELSE  CAST(((x.Y2Q" + quarter + "_allw_PMPM - x.Y1Q" + quarter + "_allw_PMPM)/x.Y1Q" + quarter + "_allw_PMPM) as varchar)  END as Y1Q" + quarter + "_Y2Q" + quarter + "_trend ");
            //}

            //sbSQL.Append(",y.Y1Q1_Y2Q1_diff  as rank FROM ( select distinct a.px ,a.px_desc ");


            //for (int i = 0; i < pct_param.DateSpanList.Count; i++)
            //{

            //    if (i % 2 != 0)
            //    {
            //        continue;
            //    }
            //    var quarter = pct_param.DateSpanList[i].quarter;

            //    sbSQL.Append(",a.Y1Q" + quarter + "_allw_amt/(SELECT Y1Q" + quarter + "_Mbr_Month FROM #MemberMonth) as Y1Q" + quarter + "_allw_PMPM ");
            //    sbSQL.Append(",a.Y2Q" + quarter + "_allw_amt/(SELECT Y2Q" + quarter + "_Mbr_Month FROM #MemberMonth) as Y2Q" + quarter + "_allw_PMPM ");
            //}


            //sbSQL.Append(" FROM ( select distinct px ,px_desc ");
            ////LOOP DSM!!!
            //for (int i = 0; i < pct_param.DateSpanList.Count; i++)
            //{
            //    var year = ((i + 1) % 2 == 0 ? "2" : "1");
            //    var quarter = pct_param.DateSpanList[i].quarter;
            //    var year_full = pct_param.DateSpanList[i].year;

            //    sbSQL.Append(",sum(case when year = " + year_full + " and quarter = " + quarter + " then allw_amt end) as Y" + year + "Q" + quarter + "_allw_amt ");
            //}

            //sbSQL.Append("from pct.CLM_OP where op_phys_bucket = 'OP' " + filters + " group by px, px_desc ) a ) x  ");
            //sbSQL.Append("left join #Rank y   on x.px = y.px and x.px_desc = y.px_desc  ");
            //sbSQL.Append("order by y.Y1Q1_Y2Q1_diff DESC; ");
            ////Allowed PMPM end
            ////Allowed PMPM end
            ////Allowed PMPM end



            ////Utilization/000 start
            ////Utilization/000 start
            ////Utilization/000 start 
            //sbSQL.Append("SELECT DISTINCT TOP 10 x.px ,x.px_desc ");
            ////LOOP DSM!!!
            //for (int i = 0; i < pct_param.DateSpanList.Count; i++)
            //{
            //    var year = ((i + 1) % 2 == 0 ? "2" : "1");
            //    var quarter = pct_param.DateSpanList[i].quarter;

            //    sbSQL.Append(",x.Y" + year + "Q" + quarter + "_util000 ");
            //}
            ////LOOP DSM!!
            //for (int i = 0; i < pct_param.DateSpanList.Count; i++)
            //{

            //    if (i % 2 != 0)
            //    {
            //        continue;
            //    }
            //    var quarter = pct_param.DateSpanList[i].quarter;

            //    sbSQL.Append(", CASE WHEN x.Y1Q" + quarter + "_util000 = 0 THEN 'N/A' ELSE  CAST(((x.Y2Q" + quarter + "_util000 - x.Y1Q" + quarter + "_util000)/x.Y1Q" + quarter + "_util000) as varchar)  END as Y1Q" + quarter + "_Y2Q" + quarter + "_trend ");
            //}

            //sbSQL.Append(",y.Y1Q1_Y2Q1_diff  as rank FROM ( select distinct a.px ,a.px_desc ");


            //for (int i = 0; i < pct_param.DateSpanList.Count; i++)
            //{

            //    if (i % 2 != 0)
            //    {
            //        continue;
            //    }
            //    var quarter = pct_param.DateSpanList[i].quarter;

            //    sbSQL.Append(",a.Y1Q" + quarter + "_px_cnt/(SELECT Y1Q" + quarter + "_Mbr_Month FROM #MemberMonth) as Y1Q" + quarter + "_util000 ");
            //    sbSQL.Append(",a.Y2Q" + quarter + "_px_cnt/(SELECT Y2Q" + quarter + "_Mbr_Month FROM #MemberMonth) as Y2Q" + quarter + "_util000 ");
            //}


            //sbSQL.Append(" FROM ( select distinct px ,px_desc ");
            ////LOOP DSM!!!
            //for (int i = 0; i < pct_param.DateSpanList.Count; i++)
            //{
            //    var year = ((i + 1) % 2 == 0 ? "2" : "1");
            //    var quarter = pct_param.DateSpanList[i].quarter;
            //    var year_full = pct_param.DateSpanList[i].year;

            //    sbSQL.Append(",sum(case when year = " + year_full + " and quarter = " + quarter + " then px_cnt end) as Y" + year + "Q" + quarter + "_px_cnt ");
            //}

            //sbSQL.Append("from pct.CLM_OP where op_phys_bucket = 'OP' " + filters + " group by px, px_desc ) a ) x  ");
            //sbSQL.Append("left join #Rank y   on x.px = y.px and x.px_desc = y.px_desc  ");
            //sbSQL.Append("order by y.Y1Q1_Y2Q1_diff DESC; ");
            ////Utilization/000 end
            ////Utilization/000 end
            ////Utilization/000 end



            ////Unit Cost 1 start
            ////Unit Cost 1 start
            ////Unit Cost 1 start 
            //sbSQL.Append("SELECT TOP 10 t1.px ,t1.px_desc ");
            ////LOOP DSM!!!
            //for (int i = 0; i < pct_param.DateSpanList.Count; i++)
            //{
            //    var year = ((i + 1) % 2 == 0 ? "2" : "1");
            //    var quarter = pct_param.DateSpanList[i].quarter;

            //    sbSQL.Append(",t1.Y" + year + "Q" + quarter + "_Unit_Cost1 ");
            //}
            ////LOOP DSM!!
            //for (int i = 0; i < pct_param.DateSpanList.Count; i++)
            //{

            //    if (i % 2 != 0)
            //    {
            //        continue;
            //    }
            //    var quarter = pct_param.DateSpanList[i].quarter;

            //    sbSQL.Append(", CASE WHEN t1.Y1Q" + quarter + "_Unit_Cost1 = 0 THEN 'N/A' ELSE  CAST(((t1.Y2Q" + quarter + "_Unit_Cost1 - t1.Y1Q" + quarter + "_Unit_Cost1)/t1.Y1Q" + quarter + "_Unit_Cost1) as varchar)  END as Y1Q" + quarter + "_Y2Q" + quarter + "_trend ");
            //}

            //sbSQL.Append("FROM ( select distinct t.px ,t.px_desc ");


            //for (int i = 0; i < pct_param.DateSpanList.Count; i++)
            //{

            //    if (i % 2 != 0)
            //    {
            //        continue;
            //    }
            //    var quarter = pct_param.DateSpanList[i].quarter;

            //    sbSQL.Append(",CASE WHEN t.Y1Q" + quarter + "_events  = 0 THEN NULL ELSE t.Y1Q" + quarter + "_allw_amt/t.Y1Q" + quarter + "_events END as Y1Q" + quarter + "_Unit_Cost1 ");
            //    sbSQL.Append(",CASE WHEN t.Y2Q" + quarter + "_events = 0 THEN NULL ELSE t.Y2Q" + quarter + "_allw_amt/t.Y2Q" + quarter + "_events END as Y2Q" + quarter + "_Unit_Cost1 ");
            //}

            //sbSQL.Append(",y.Y1Q1_Y2Q1_diff  as rank FROM ( select distinct px ,px_desc ");

            ////LOOP DSM!!!
            //for (int i = 0; i < pct_param.DateSpanList.Count; i++)
            //{
            //    var year = ((i + 1) % 2 == 0 ? "2" : "1");
            //    var quarter = pct_param.DateSpanList[i].quarter;
            //    var year_full = pct_param.DateSpanList[i].year;


            //    sbSQL.Append(",sum(case when year = " + year_full + " and quarter = " + quarter + " then allw_amt end) as Y" + year + "Q" + quarter + "_allw_amt ");

            //    sbSQL.Append(",sum(case when year = " + year_full + " and quarter = " + quarter + " then evnts end) as Y" + year + "Q" + quarter + "_events ");
            //}

            //sbSQL.Append("from pct.CLM_OP where op_phys_bucket = 'OP' " + filters + " group by px, px_desc ) t   ");
            //sbSQL.Append("left join #Rank y   on t.px = y.px and t.px_desc = y.px_desc ) t1  ");
            //sbSQL.Append("order by t1.rank DESC; ");
            ////Unit Cost 1 end
            ////Unit Cost 1 end
            ////Unit Cost 1 end





            ////Unit Cost 2 start
            ////Unit Cost 2 start
            ////Unit Cost 2 start 
            //sbSQL.Append("SELECT TOP 10 t1.px ,t1.px_desc ");
            ////LOOP DSM!!!
            //for (int i = 0; i < pct_param.DateSpanList.Count; i++)
            //{
            //    var year = ((i + 1) % 2 == 0 ? "2" : "1");
            //    var quarter = pct_param.DateSpanList[i].quarter;

            //    sbSQL.Append(",t1.Y" + year + "Q" + quarter + "_Unit_Cost2 ");
            //}
            ////LOOP DSM!!
            //for (int i = 0; i < pct_param.DateSpanList.Count; i++)
            //{

            //    if (i % 2 != 0)
            //    {
            //        continue;
            //    }
            //    var quarter = pct_param.DateSpanList[i].quarter;

            //    sbSQL.Append(", CASE WHEN t1.Y1Q" + quarter + "_Unit_Cost2 = 0 THEN 'N/A' ELSE  CAST(((t1.Y2Q" + quarter + "_Unit_Cost2 - t1.Y1Q" + quarter + "_Unit_Cost2)/t1.Y1Q" + quarter + "_Unit_Cost2) as varchar)  END as Y1Q" + quarter + "_Y2Q" + quarter + "_trend ");
            //}

            //sbSQL.Append("FROM ( select distinct t.px ,t.px_desc ");


            //for (int i = 0; i < pct_param.DateSpanList.Count; i++)
            //{

            //    if (i % 2 != 0)
            //    {
            //        continue;
            //    }
            //    var quarter = pct_param.DateSpanList[i].quarter;

            //    sbSQL.Append(",CASE WHEN t.Y1Q" + quarter + "_adj_srv_uni  = 0 THEN NULL ELSE t.Y1Q" + quarter + "_allw_amt/t.Y1Q" + quarter + "_adj_srv_uni END as Y1Q" + quarter + "_Unit_Cost2 ");
            //    sbSQL.Append(",CASE WHEN t.Y2Q" + quarter + "_adj_srv_uni = 0 THEN NULL ELSE t.Y2Q" + quarter + "_allw_amt/t.Y2Q" + quarter + "_adj_srv_uni END as Y2Q" + quarter + "_Unit_Cost2 ");
            //}

            //sbSQL.Append(",y.Y1Q1_Y2Q1_diff  as rank FROM ( select distinct px ,px_desc ");

            ////LOOP DSM!!!
            //for (int i = 0; i < pct_param.DateSpanList.Count; i++)
            //{
            //    var year = ((i + 1) % 2 == 0 ? "2" : "1");
            //    var quarter = pct_param.DateSpanList[i].quarter;
            //    var year_full = pct_param.DateSpanList[i].year;


            //    sbSQL.Append(",sum(case when year = " + year_full + " and quarter = " + quarter + " then allw_amt end) as Y" + year + "Q" + quarter + "_allw_amt ");

            //    sbSQL.Append(",sum(case when year = " + year_full + " and quarter = " + quarter + "then adj_srv_uni end) as Y" + year + "Q" + quarter + "_adj_srv_uni ");
            //}

            //sbSQL.Append("from pct.CLM_OP where op_phys_bucket = 'OP' " + filters + " group by px, px_desc ) t   ");
            //sbSQL.Append("left join #Rank y   on t.px = y.px and t.px_desc = y.px_desc ) t1  ");
            //sbSQL.Append("order by t1.rank DESC; ");
            ////Unit Cost 2 end
            ////Unit Cost 2 end
            ////Unit Cost 2 end


            //Year Quarter Start
            //Year Quarter Start
            //Year Quarter Start
            sbSQL.Append("SELECT DISTINCT [year], [quarter] from pct.CLM_OP ORDER BY [year], [quarter];");
            //Year Quarter End
            //Year Quarter End
            //Year Quarter End



            var results = _db.LoadDataMultiple(sql: sbSQL.ToString(), token, gr => gr.Read<Unique_Individual_Model>(), gr => gr.Read<YearQuarter_Model>(), gr => gr.Read<String>(), gr => gr.Read<String>(), gr => gr.Read<String>(), gr => gr.Read<String>(), gr => gr.Read<String>(), gr => gr.Read<String>(), gr => gr.Read<String>(), gr => gr.Read<String>(), gr => gr.Read<String>(), gr => gr.Read<String>(), gr => gr.Read<String>(), gr => gr.Read<String>(), gr => gr.Read<String>(), gr => gr.Read<String>(), gr => gr.Read<String>(), gr => gr.Read<String>(), gr => gr.Read<String>(), gr => gr.Read<String>(), gr => gr.Read<String>(), gr => gr.Read<String>(), gr => gr.Read<String>(), gr => gr.Read<String>(), gr => gr.Read<String>(), gr => gr.Read<String>(), gr => gr.Read<String>(), gr => gr.Read<String>(), gr => gr.Read<String>(), gr => gr.Read<String>(), gr => gr.Read<String>(), gr => gr.Read<String>(), gr => gr.Read<String>(), gr => gr.Read<String>(), gr => gr.Read<String>(), gr => gr.Read<String>(), gr => gr.Read<String>(), gr => gr.Read<String>(), gr => gr.Read<String>(), gr => gr.Read<String>(), "VCT_DB");

            CLM_OP_Report_Model claims_final = new CLM_OP_Report_Model();
            claims_final.unique_individual = (results[0] as List<Unique_Individual_Model>);
            claims_final.year_quarter = (results[1] as List<YearQuarter_Model>);




            //var results = _db.LoadDataMultiple(sql: sbSQL.ToString(), token, gr => gr.Read<Unique_Individual_Model>(), gr => gr.Read<Events_Model>(), gr => gr.Read<Claims_Model>(), gr => gr.Read<Allowed_Model>(), gr => gr.Read<Member_Month_Model>(), gr => gr.Read<Allowed_PMPM_Model>(), gr => gr.Read<Utilization000_Model>(), gr => gr.Read<Unit_Cost1_Model>(), gr => gr.Read<Unit_Cost2_Model>(), gr => gr.Read<YearQuarter_Model>(), gr => gr.Read<String>(), gr => gr.Read<String>(), gr => gr.Read<String>(), gr => gr.Read<String>(), gr => gr.Read<String>(), gr => gr.Read<String>(), gr => gr.Read<String>(), gr => gr.Read<String>(), gr => gr.Read<String>(), gr => gr.Read<String>(), gr => gr.Read<String>(), gr => gr.Read<String>(), gr => gr.Read<String>(), gr => gr.Read<String>(), gr => gr.Read<String>(), gr => gr.Read<String>(), gr => gr.Read<String>(), gr => gr.Read<String>(), gr => gr.Read<String>(), gr => gr.Read<String>(), gr => gr.Read<String>(), gr => gr.Read<String>(), gr => gr.Read<String>(), gr => gr.Read<String>(), gr => gr.Read<String>(), gr => gr.Read<String>(), gr => gr.Read<String>(), gr => gr.Read<String>(), gr => gr.Read<String>(), gr => gr.Read<String>(), "VCT_DB");

            //CLM_OP_Report_Model claims_final = new CLM_OP_Report_Model();
            //claims_final.unique_individual = ( results[0] as List<Unique_Individual_Model>);
            //claims_final.events = (results[1] as List<Events_Model>);
            //claims_final.claims = (results[2] as List<Claims_Model>);
            //claims_final.allowed = (results[3] as List<Allowed_Model>);
            //claims_final.member_month = (results[4] as List<Member_Month_Model>);
            //claims_final.allowed_pmpm = (results[5] as List<Allowed_PMPM_Model>);
            //claims_final.utilization000 = (results[6] as List<Utilization000_Model>);
            //claims_final.unit_cost1 = (results[7] as List<Unit_Cost1_Model>);
            //claims_final.unit_cost2 = (results[8] as List<Unit_Cost2_Model>);
            //claims_final.year_quarter = (results[9] as List<YearQuarter_Model>);


            return claims_final;
    
        }


        private string getFilterString(ProcCodeTrends_Parameters pct_param)
        {
            StringBuilder sbFilters = new StringBuilder();


            PropertyInfo[] rootProperties = typeof(ProcCodeTrends_Parameters).GetProperties();
            foreach (PropertyInfo property in rootProperties)
            {
                if(property.PropertyType == typeof(string))
                {
                    var value = property.GetValue(pct_param);
                    if (value != null)
                    {

                        sbFilters.Append("AND " + property.Name + " in ("+ value + ") ");

                    }
                        
                }
                
 
            }

            return sbFilters.ToString();
        }



    }
}
