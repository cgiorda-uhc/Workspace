using Dapper;
using DataAccessLibrary.Data.Abstract;
using DataAccessLibrary.DataAccess;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.Extensions.Primitives;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.Intrinsics.X86;
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



        public Task<IEnumerable<string>> GetPROC_CD_Async(CancellationToken token)
        {

            string strSQL = "SELECT px + ' - ' + px_desc FROM [VCT_DB].[pct].[PROC_CD];";

            var results = _db.LoadData<string>(sql: strSQL, token, connectionId: "VCT_DB");

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
            sbSQL.Append("IF OBJECT_ID('tempdb..#Rank') IS NOT NULL DROP TABLE #Rank SELECT t.px, t.px_desc, t.Y1Q1_allw_amt, t.Y2Q1_allw_amt, (t.Y2Q1_allw_amt - t.Y1Q1_allw_amt) as Y1Q1_Y2Q1_diff INTO #Rank FROM ( select px ,px_desc ,sum(case when year = "+ pct_param.DateSpanList[0].year + " and quarter = "+ pct_param.DateSpanList[0].quarter + " then allw_amt end) as Y1Q1_allw_amt ,sum(case when year = "+ pct_param.DateSpanList[4].year + " and quarter = "+ pct_param.DateSpanList[4].quarter + " then allw_amt end) as Y2Q1_allw_amt from pct.CLM_OP a where a.op_phys_bucket = 'OP' " + filters + " group by px, px_desc ) t; ");


            //CREATE MemberMonth TMP TABLE START
            //CREATE MemberMonth TMP TABLE START
            //CREATE MemberMonth TMP TABLE START
            sbSQL.Append("IF OBJECT_ID('tempdb..#MemberMonth') IS  NOT NULL DROP TABLE #MemberMonth ");
            sbSQL.Append("SELECT DISTINCT TOP "+ pct_param.RowCount +" t.Metric ");

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

            sbSQL.Append("from pct.MM_FINAL a where 1 = 1  " + _no_proc_codes_filters + ") t; "); //NO PX IN pct.MM_FINAL TABLE

            //CREATE MemberMonth TMP TABLE END
            //CREATE MemberMonth TMP TABLE END
            //CREATE MemberMonth TMP TABLE END


            //Year Quarter Start
            //Year Quarter Start
            //Year Quarter Start
            sbSQL.Append("SELECT DISTINCT [year], [quarter] from pct.CLM_OP ORDER BY [year], [quarter];");
            //Year Quarter End
            //Year Quarter End
            //Year Quarter End



            //unique individual start
            //unique individual start
            //unique individual start
            var sql = generateGenericSQL("indv", "indv", pct_param.RowCount, filters, pct_param.DateSpanList);
            sbSQL.Append(sql);
            //unique individual end
            //unique individual end
            //unique individual end



            //events start
            //events start
            //events start
            sql = generateGenericSQL("evnts", "events", pct_param.RowCount, filters, pct_param.DateSpanList);
            sbSQL.Append(sql);
            //events end
            //events end
            //events end



            //claims start
            //claims start
            //claims start
            sbSQL.Append("SELECT DISTINCT TOP "+ pct_param.RowCount +" t.px ,t.px_desc ");
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
    
                sbSQL.Append(",t.Y" + year + "Q" + quarter + "_claims ");
                sbSQL.Append(",t.Y" + year + "Q" + quarter + "_fac_claims ");
                sbSQL.Append(",t.Y" + year + "Q" + quarter + "_phy_claims ");
                sbSQL.Append(",t.Y" + year + "Q" + quarter + "_oth_claims ");
            }

            //LOOP DSM!!
            for (int i = 1; i < 5; i++)
            {

                sbSQL.Append(",CASE WHEN t.Y1Q" + i + "_claims = 0 THEN 'N/A' ELSE  CAST(FORMAT(((t.Y2Q" + i + "_claims-t.Y1Q" + i + "_claims)/t.Y1Q" + i + "_claims),'P') as varchar) END as Y1Q" + i + "_Y2Q" + i + "_trend_claims ");
                sbSQL.Append(",CASE WHEN t.Y1Q" + i + "_fac_claims = 0 THEN 'N/A' ELSE  CAST(FORMAT(((t.Y2Q" + i + "_fac_claims-t.Y1Q" + i + "_fac_claims)/t.Y1Q" + i + "_fac_claims),'P') as varchar)  END as Y1Q" + i + "_Y2Q" + i + "_trend_fac_claims ");
                sbSQL.Append(",CASE WHEN t.Y1Q" + i + "_phy_claims = 0 THEN 'N/A' ELSE  CAST(FORMAT(((t.Y2Q" + i + "_fac_claims-t.Y1Q" + i + "_phy_claims)/t.Y1Q" + i + "_phy_claims),'P') as varchar) END Y1Q" + i + "_Y2Q" + i + "_trend_phy_claims ");
                sbSQL.Append(",CASE WHEN t.Y1Q" + i + "_oth_claims = 0 THEN 'N/A' ELSE  CAST(FORMAT(((t.Y2Q" + i + "_oth_claims-t.Y1Q" + i + "_oth_claims)/t.Y1Q" + i + "_oth_claims),'P') as varchar)  END Y1Q" + i + "_Y2Q" + i + "_trend_oth_claims ");

            }


            sbSQL.Append(",t.rank FROM ( select distinct a.px ,a.px_desc ");
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

                var year_full = pct_param.DateSpanList[i].year;
                var quarter_actual = pct_param.DateSpanList[i].quarter;

                sbSQL.Append(",sum(case when a.year = " + year_full + " and a.quarter = " + quarter_actual + " then claims end) as Y" + year + "Q" + quarter + "_claims ");
                sbSQL.Append(",sum(case when a.year = " + year_full + " and a.quarter = " + quarter_actual + " then fac_clms end) as Y" + year + "Q" + quarter + "_fac_claims ");
                sbSQL.Append(",sum(case when a.year = " + year_full + " and a.quarter = " + quarter_actual + " then phy_clms end) as Y" + year + "Q" + quarter + "_phy_claims ");
                sbSQL.Append(",sum(case when a.year = " + year_full + " and a.quarter = " + quarter_actual + " then oth_clms end) as Y" + year + "Q" + quarter + "_oth_claims ");

            }

            sbSQL.Append(",b.Y1Q1_Y2Q1_diff as rank from pct.CLM_OP a left join #Rank b on a.px = b.px and a.px_desc = b.px_desc where a.op_phys_bucket = 'OP' " + filters + " group by b.Y1Q1_Y2Q1_diff,a.px, a.px_desc ) t order by t.rank DESC; ");
            //claims end
            //claims end
            //claims end


            //allowed start
            //allowed start
            //allowed start
             sql = generateGenericSQL("allw_amt", "allw_amt", pct_param.RowCount, filters, pct_param.DateSpanList);
            sbSQL.Append(sql);
            //allowed end
            //allowed end
            //allowed end


            ////member month start
            ////member month start
            ////member month start
            sbSQL.Append("SELECT * FROM #MemberMonth; ");
            ////member month end
            ////member month end
            ////member month end


            //Allowed PMPM start
            //Allowed PMPM start
            //Allowed PMPM start 

            sql = generateGenericMemberMonthSQL("allw_amt", "allw_PMPM", pct_param.RowCount, filters, pct_param.DateSpanList);
            sbSQL.Append(sql);

            //Allowed PMPM end
            //Allowed PMPM end
            //Allowed PMPM end



            ////Utilization/000 start
            ////Utilization/000 start
            ////Utilization/000 start 

            sql = generateGenericMemberMonthSQL("px_cnt", "util000", pct_param.RowCount, filters, pct_param.DateSpanList);
            sbSQL.Append(sql);

            ////Utilization/000 end
            ////Utilization/000 end
            ////Utilization/000 end



            //Unit Cost 1 start
            //Unit Cost 1 start
            //Unit Cost 1 start 
            sbSQL.Append("SELECT TOP "+ pct_param.RowCount +" t1.px ,t1.px_desc ");
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

                sbSQL.Append(",t1.Y" + year + "Q" + quarter + "_Unit_Cost1 ");
            }
            //LOOP DSM!!
            for (int i = 1; i < 5; i++)
            {

                sbSQL.Append(", CASE WHEN t1.Y1Q" + i + "_Unit_Cost1 = 0 THEN 'N/A' ELSE  CAST(FORMAT(((t1.Y2Q" + i + "_Unit_Cost1 - t1.Y1Q" + i + "_Unit_Cost1)/t1.Y1Q" + i + "_Unit_Cost1),'P') as varchar)  END as Y1Q" + i + "_Y2Q" + i + "_trend ");

            }

            sbSQL.Append("FROM ( select distinct t.px ,t.px_desc ");

            for (int i = 1; i < 5; i++)
            {

                sbSQL.Append(",CASE WHEN t.Y1Q" + i + "_events  = 0 THEN NULL ELSE t.Y1Q" + i + "_allw_amt/t.Y1Q" + i + "_events END as Y1Q" + i + "_Unit_Cost1 ");
                sbSQL.Append(",CASE WHEN t.Y2Q" + i + "_events = 0 THEN NULL ELSE t.Y2Q" + i + "_allw_amt/t.Y2Q" + i + "_events END as Y2Q" + i + "_Unit_Cost1 ");

            }

            sbSQL.Append(",y.Y1Q1_Y2Q1_diff  as rank FROM ( select distinct px ,px_desc ");

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

                var year_full = pct_param.DateSpanList[i].year;
                var quarter_actual = pct_param.DateSpanList[i].quarter;


                sbSQL.Append(",sum(case when year = " + year_full + " and quarter = " + quarter_actual + " then allw_amt end) as Y" + year + "Q" + quarter + "_allw_amt ");

                sbSQL.Append(",sum(case when year = " + year_full + " and quarter = " + quarter_actual + " then evnts end) as Y" + year + "Q" + quarter + "_events ");
            }

            sbSQL.Append("from pct.CLM_OP where op_phys_bucket = 'OP' " + filters.Replace("a.","") + " group by px, px_desc ) t   ");
            sbSQL.Append("left join #Rank y   on t.px = y.px and t.px_desc = y.px_desc ) t1  ");
            sbSQL.Append("order by t1.rank DESC; ");
            //Unit Cost 1 end
            //Unit Cost 1 end
            //Unit Cost 1 end





            //Unit Cost 2 start
            //Unit Cost 2 start
            //Unit Cost 2 start 
            sbSQL.Append("SELECT TOP " + pct_param.RowCount + " t1.px ,t1.px_desc ");
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
          

                sbSQL.Append(",t1.Y" + year + "Q" + quarter + "_Unit_Cost2 ");
            }
            //LOOP DSM!!
            for (int i = 1; i < 5; i++)
            {

                sbSQL.Append(", CASE WHEN t1.Y1Q" + i + "_Unit_Cost2 = 0 THEN 'N/A' ELSE  CAST(FORMAT(((t1.Y2Q" + i + "_Unit_Cost2 - t1.Y1Q" + i + "_Unit_Cost2)/t1.Y1Q" + i + "_Unit_Cost2),'P') as varchar)  END as Y1Q" + i + "_Y2Q" + i + "_trend ");

            }

            sbSQL.Append("FROM ( select distinct t.px ,t.px_desc ");

            for (int i = 1; i < 5; i++)
            {


                sbSQL.Append(",CASE WHEN t.Y1Q" + i + "_adj_srv_uni  = 0 THEN NULL ELSE t.Y1Q" + i + "_allw_amt/t.Y1Q" + i + "_adj_srv_uni END as Y1Q" + i + "_Unit_Cost2 ");
                sbSQL.Append(",CASE WHEN t.Y2Q" + i + "_adj_srv_uni = 0 THEN NULL ELSE t.Y2Q" + i + "_allw_amt/t.Y2Q" + i + "_adj_srv_uni END as Y2Q" + i + "_Unit_Cost2 ");

            }
            

            sbSQL.Append(",y.Y1Q1_Y2Q1_diff  as rank FROM ( select distinct px ,px_desc ");

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

                var year_full = pct_param.DateSpanList[i].year;
                var quarter_actual = pct_param.DateSpanList[i].quarter;


                sbSQL.Append(",sum(case when year = " + year_full + " and quarter = " + quarter_actual + " then allw_amt end) as Y" + year + "Q" + quarter + "_allw_amt ");

                sbSQL.Append(",sum(case when year = " + year_full + " and quarter = " + quarter_actual + "then adj_srv_uni end) as Y" + year + "Q" + quarter + "_adj_srv_uni ");
            }

            sbSQL.Append("from pct.CLM_OP where op_phys_bucket = 'OP' " + filters.Replace("a.", "") + " group by px, px_desc ) t   ");
            sbSQL.Append("left join #Rank y   on t.px = y.px and t.px_desc = y.px_desc ) t1  ");
            sbSQL.Append("order by t1.rank DESC; ");
            //Unit Cost 2 end
            //Unit Cost 2 end
            //Unit Cost 2 end



            var results = _db.LoadDataMultiple(sql: sbSQL.ToString(), token, gr => gr.Read<YearQuarter_Model>(), gr => gr.Read<Unique_Individual_Model>(), gr => gr.Read<Events_Model>(), gr => gr.Read<Claims_Model>(), gr => gr.Read<Allowed_Model>(), gr => gr.Read<Member_Month_Model>(), gr => gr.Read<Allowed_PMPM_Model>(), gr => gr.Read<Utilization000_Model>(), gr => gr.Read<Unit_Cost1_Model>(), gr => gr.Read<Unit_Cost2_Model>(), gr => gr.Read<String>(), gr => gr.Read<String>(), gr => gr.Read<String>(), gr => gr.Read<String>(), gr => gr.Read<String>(), gr => gr.Read<String>(), gr => gr.Read<String>(), gr => gr.Read<String>(), gr => gr.Read<String>(), gr => gr.Read<String>(), gr => gr.Read<String>(), gr => gr.Read<String>(), gr => gr.Read<String>(), gr => gr.Read<String>(), gr => gr.Read<String>(), gr => gr.Read<String>(), gr => gr.Read<String>(), gr => gr.Read<String>(), gr => gr.Read<String>(), gr => gr.Read<String>(), gr => gr.Read<String>(), gr => gr.Read<String>(), gr => gr.Read<String>(), gr => gr.Read<String>(), gr => gr.Read<String>(), gr => gr.Read<String>(), gr => gr.Read<String>(), gr => gr.Read<String>(), gr => gr.Read<String>(), gr => gr.Read<String>(), "VCT_DB");

            CLM_OP_Report_Model claims_final = new CLM_OP_Report_Model();
            claims_final.year_quarter = (results[0] as List<YearQuarter_Model>);
            claims_final.unique_individual = (results[1] as List<Unique_Individual_Model>);
            claims_final.events = (results[2] as List<Events_Model>);
            claims_final.claims = (results[3] as List<Claims_Model>);
            claims_final.allowed = (results[4] as List<Allowed_Model>);
            claims_final.member_month = (results[5] as List<Member_Month_Model>);
            claims_final.allowed_pmpm = (results[6] as List<Allowed_PMPM_Model>);
            claims_final.utilization000= (results[7] as List<Utilization000_Model>);
            claims_final.unit_cost1 = (results[8] as List<Unit_Cost1_Model>);
            claims_final.unit_cost2 = (results[9] as List<Unit_Cost2_Model>);


            return claims_final;
    
        }


        private string generateGenericSQL(string columnName, string displayName, int RowCnt, string filters, List<DateSpan_Model> DateSpanList)
        {


            string year = "";
            string quarter = "";

            StringBuilder sbSQL = new StringBuilder();
            sbSQL.Append("SELECT DISTINCT TOP " + RowCnt + " t.px ,t.px_desc ");
            //LOOP DSM!!!
            for (int i = 0; i < DateSpanList.Count; i++)
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

                sbSQL.Append(",t.Y" + year + "Q" + quarter + "_" + displayName);
            }
            //LOOP DSM!!
            for (int i = 1; i < 5; i++)
            {

                sbSQL.Append(", CASE WHEN t.Y1Q" + i + "_" + displayName + " = 0 THEN 'N/A' ELSE  CAST(FORMAT(((t.Y2Q" + i + "_" +  displayName +" - t.Y1Q" + i + "_" + displayName + ")/t.Y1Q" + i + "_" + displayName + "),'P') as varchar) END as Y1Q" + i + "_Y2Q" + i + "_trend ");

            }


            sbSQL.Append(",t.rank FROM ( select distinct a.px ,a.px_desc ");
            //LOOP DSM!!!
            for (int i = 0; i < DateSpanList.Count; i++)
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

                var year_full = DateSpanList[i].year;
                var quarter_actual = DateSpanList[i].quarter;

                sbSQL.Append(",sum(case when a.year = " + year_full + " and a.quarter = " + quarter_actual + " then "+ columnName + " end) as Y" + year + "Q" + quarter + "_" + displayName);
            }

            sbSQL.Append(",b.Y1Q1_Y2Q1_diff as rank from pct.CLM_OP a left join #Rank b on a.px = b.px and a.px_desc = b.px_desc where a.op_phys_bucket = 'OP' " + filters + " group by b.Y1Q1_Y2Q1_diff,a.px, a.px_desc ) t order by t.rank DESC; ");


            return sbSQL.ToString();
        }


        private string generateGenericMemberMonthSQL(string columnName, string displayName, int RowCnt, string filters, List<DateSpan_Model> DateSpanList)
        {


            string year = "";
            string quarter = "";

            StringBuilder sbSQL = new StringBuilder();
            sbSQL.Append("SELECT DISTINCT TOP " + RowCnt + " x.px ,x.px_desc ");
            //LOOP DSM!!!
            for (int i = 0; i < DateSpanList.Count; i++)
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

                sbSQL.Append(",x.Y" + year + "Q" + quarter + "_" + displayName + " ");
            }

            //LOOP DSM!!
            for (int i = 1; i < 5; i++)
            {

                sbSQL.Append(", CASE WHEN x.Y1Q" + i + "_" + displayName + " = 0 THEN 'N/A' ELSE  CAST(FORMAT(((x.Y2Q" + i + "_" + displayName + " - x.Y1Q" + i + "_" + displayName + ")/x.Y1Q" + i + "_" + displayName + "),'P') as varchar) END as Y1Q" + i + "_Y2Q" + i + "_trend ");
            }

            sbSQL.Append(",y.Y1Q1_Y2Q1_diff  as rank FROM ( select distinct a.px ,a.px_desc ");

            for (int i = 1; i < 5; i++)
            {

                sbSQL.Append(",a.Y1Q" + i + "_" + columnName + "/(SELECT Y1Q" + i + "_Mbr_Month FROM #MemberMonth) as Y1Q" + i + "_" + displayName + " ");
                sbSQL.Append(",a.Y2Q" + i + "_" + columnName + "/(SELECT Y2Q" + i + "_Mbr_Month FROM #MemberMonth) as Y2Q" + i + "_" + displayName + " ");

            }

            sbSQL.Append(" FROM ( select distinct px ,px_desc ");
            //LOOP DSM!!!
            for (int i = 0; i < DateSpanList.Count; i++)
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

                var year_full = DateSpanList[i].year;
                var quarter_actual = DateSpanList[i].quarter;


                sbSQL.Append(",sum(case when year = " + year_full + " and quarter = " + quarter_actual + " then "+ columnName + " end) as Y" + year + "Q" + quarter +"_" + columnName + " ");
            }

            sbSQL.Append("from pct.CLM_OP where op_phys_bucket = 'OP' " + filters.Replace("a.", "") + " group by px, px_desc ) a ) x  ");
            sbSQL.Append("left join #Rank y   on x.px = y.px and x.px_desc = y.px_desc  ");
            sbSQL.Append("order by y.Y1Q1_Y2Q1_diff DESC; ");


            return sbSQL.ToString();
        }


        //THIS FILTER IS USE TO ALLOW DYNAMIC FILTERS WHEN HAS NOT PX 
        private string _no_proc_codes_filters;
        private string getFilterString(ProcCodeTrends_Parameters pct_param)
        {
            StringBuilder sbFilters = new StringBuilder();
            StringBuilder sbFiltersNoProc = new StringBuilder();

            PropertyInfo[] rootProperties = typeof(ProcCodeTrends_Parameters).GetProperties();
            foreach (PropertyInfo property in rootProperties)
            {
                if(property.PropertyType == typeof(string))
                {
                    var value = property.GetValue(pct_param);
                    if (value != null)
                    {

                        if(property.Name != "px") //THIS FILTER IS USE TO ALLOW DYNAMIC FILTERS WHEN HAS NOT PX 
                        {
                            sbFiltersNoProc.Append("AND a." + property.Name + " in (" + value + ") ");
                        }

                        sbFilters.Append("AND a." + property.Name + " in (" + value + ") ");

                    }
                        
                }
                
 
            }

            _no_proc_codes_filters = sbFiltersNoProc.ToString();
            return sbFilters.ToString();
        }



    }
}
