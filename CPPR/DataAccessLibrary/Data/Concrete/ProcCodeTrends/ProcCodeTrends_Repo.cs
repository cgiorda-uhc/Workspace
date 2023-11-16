using Dapper;
using DataAccessLibrary.Data.Abstract;
using DataAccessLibrary.DataAccess;
using Microsoft.Extensions.Primitives;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
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
            string strSQL = "SELECT DISTINCT year, quarter  FROM [pct].[CLM_OP] ORDER BY year ASC, quarter ASC";

            var results = _db.LoadData<DateSpan_Model>(sql: strSQL, token, connectionId: "VCT_DB");

            return results;
        }



        public async Task<CLM_OP_Report_Model> GetMainPCTReport_Async(ProcCodeTrends_Parameters pct_param,  CancellationToken token)
        {



            string filters = getFilterString(pct_param);
            StringBuilder sbSQL = new StringBuilder();

            //CREATE RANK TMP TABLE
            //CREATE RANK TMP TABLE
            //CREATE RANK TMP TABLE
            sbSQL.Append("IF OBJECT_ID('tempdb..#Rank') IS NOT NULL DROP TABLE #Rank SELECT t.px, t.px_desc, t.Y1Q1_allw_amt, t.Y2Q1_allw_amt, (t.Y2Q1_allw_amt - t.Y1Q1_allw_amt) as Y1Q1_Y2Q1_diff INTO #Rank FROM ( select px ,px_desc ,sum(case when year = 2021 and quarter = 1 then allw_amt end) as Y1Q1_allw_amt ,sum(case when year = 2022 and quarter = 1 then allw_amt end) as Y2Q1_allw_amt from pct.CLM_OP where op_phys_bucket = 'OP' "+ filters + " group by px, px_desc ) t; ");



            //unique individual start
            //unique individual start
            //unique individual start
            sbSQL.Append("SELECT DISTINCT TOP 10 t.px, t.px_desc ");

            //LOOP DSM!!!
            for (int i = 0;i < pct_param.DateSpanList.Count; i++)
            {
                sbSQL.Append(",t.Y"+ ((i +1) % 2 == 0 ? "2" : "1") + "Q" + pct_param.DateSpanList[i].quarter + "_indv ");
            }

            //LOOP DSM!!
            foreach (var ds in pct_param.DateSpanList)
            {
                sbSQL.Append(", CASE WHEN t.Y1Q" + ds.quarter + "_indv = 0 THEN 'N/A' ELSE  CAST(((t.Y2Q" + ds.quarter + "_indv - t.Y1Q" + ds.quarter + "_indv)/t.Y1Q" + ds.quarter + "_indv) as varchar)  END as Y1Q" + ds.quarter + "_Y2Q" + ds.quarter + "_trend ");
            }
            sbSQL.Append(" ,t.rank FROM ( select a.px ,a.px_desc ");

            //LOOP DSM!!!
            for (int i = 0; i < pct_param.DateSpanList.Count; i++)
            {
                sbSQL.Append(",t.Y" + ((i + 1) % 2 == 0 ? "2" : "1") + "Q" + pct_param.DateSpanList[i].quarter + "_indv ");
                sbSQL.Append(",sum(case when a.year = " + pct_param.DateSpanList[i].year + " and a.quarter = " + pct_param.DateSpanList[i].quarter + " then indv end) as Y" + ((i + 1) % 2 == 0 ? "2" : "1") + "Q" + pct_param.DateSpanList[i].quarter + "_indv ");
            }

            sbSQL.Append(",b.Y1Q1_Y2Q1_diff as rank from pct.CLM_OP a left join #Rank b on a.px = b.px and a.px_desc = b.px_desc where a.op_phys_bucket = 'OP'  " + filters + " group by b.Y1Q1_Y2Q1_diff,a.px, a.px_desc ) t order by t.rank DESC; ");
            //unique individual end
            //unique individual end
            //unique individual end




            //events start
            //events start
            //events start
            sbSQL.Append("SELECT DISTINCT TOP 10 t.px ,t.px_desc ");
            //LOOP DSM!!!
            for (int i = 0; i < pct_param.DateSpanList.Count; i++)
            {
                sbSQL.Append(",t.Y" + ((i + 1) % 2 == 0 ? "2" : "1") + "Q" + pct_param.DateSpanList[i].quarter + "_events ");
            }
            //LOOP DSM!!
            foreach (var ds in pct_param.DateSpanList)
            {
                sbSQL.Append(", CASE WHEN t.Y1Q" + ds.quarter + "_events = 0 THEN 'N/A' ELSE  CAST(((t.Y2Q" + ds.quarter + "_events - t.Y1Q" + ds.quarter + "_events)/t.Y1Q" + ds.quarter + "_events) as varchar)  END as Y1Q" + ds.quarter + "_Y2Q" + ds.quarter + "_trend ");
            }
            sbSQL.Append(",t.rank FROM ( select distinct a.px ,a.px_desc ");
            //LOOP DSM!!!
            for (int i = 0; i < pct_param.DateSpanList.Count; i++)
            {
                sbSQL.Append(",sum(case when a.year = " + pct_param.DateSpanList[i].year + " and a.quarter = " + pct_param.DateSpanList[i].quarter + " then evnts end) as Y" + ((i + 1) % 2 == 0 ? "2" : "1") + "Q" + pct_param.DateSpanList[i].quarter + "_events ");
            }

            sbSQL.Append(",b.Y1Q1_Y2Q1_diff as rank from pct.CLM_OP a left join #Rank b on a.px = b.px and a.px_desc = b.px_desc where a.op_phys_bucket = 'OP' " + filters + " group by b.Y1Q1_Y2Q1_diff,a.px, a.px_desc ) t order by t.rank DESC; ");
            //events end
            //events end
            //events end












            var results = _db.LoadDataMultiple(sql: sbSQL.ToString(), token, gr => gr.Read<Unique_Individual_Model>(), gr => gr.Read<Events_Model>(), gr => gr.Read<String>(), gr => gr.Read<String>(), gr => gr.Read<String>(), gr => gr.Read<String>(), gr => gr.Read<String>(), gr => gr.Read<String>(), gr => gr.Read<String>(), gr => gr.Read<String>(), gr => gr.Read<String>(), gr => gr.Read<String>(), gr => gr.Read<String>(), gr => gr.Read<String>(), gr => gr.Read<String>(), gr => gr.Read<String>(), gr => gr.Read<String>(), gr => gr.Read<String>(), gr => gr.Read<String>(), gr => gr.Read<String>(), gr => gr.Read<String>(), gr => gr.Read<String>(), gr => gr.Read<String>(), gr => gr.Read<String>(), gr => gr.Read<String>(), gr => gr.Read<String>(), gr => gr.Read<String>(), gr => gr.Read<String>(), gr => gr.Read<String>(), gr => gr.Read<String>(), gr => gr.Read<String>(), gr => gr.Read<String>(), gr => gr.Read<String>(), gr => gr.Read<String>(), gr => gr.Read<String>(), gr => gr.Read<String>(), gr => gr.Read<String>(), gr => gr.Read<String>(), gr => gr.Read<String>(), gr => gr.Read<String>(), "VCT_DB");

            CLM_OP_Report_Model claims_final = new CLM_OP_Report_Model();
            claims_final.unique_individual = ( results[0] as List<Unique_Individual_Model>);
            claims_final.events = (results[1] as List<Events_Model>);




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
