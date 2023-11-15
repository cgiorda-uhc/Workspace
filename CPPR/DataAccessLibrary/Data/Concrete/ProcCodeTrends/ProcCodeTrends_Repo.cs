using Dapper;
using DataAccessLibrary.Data.Abstract;
using DataAccessLibrary.DataAccess;
using Microsoft.Extensions.Primitives;
using System;
using System.Collections.Generic;
using System.Linq;
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



        public Task<SqlMapper.GridReader> GetMainPCTReport_Async(ProcCodeTrends_Parameters pct_param, List<DateSpan_Model> dsm, CancellationToken token)
        {


            StringBuilder sbFilters = new StringBuilder();
            StringBuilder sbSQL = new StringBuilder();

            //CREATE RANK TMP TABLE
            sbSQL.Append("IF OBJECT_ID('tempdb..#Rank') IS NOT NULL DROP TABLE #Rank SELECT t.px, t.px_desc, t.Y1Q1_allw_amt, t.Y2Q1_allw_amt, (t.Y2Q1_allw_amt - t.Y1Q1_allw_amt) as Y1Q1_Y2Q1_diff INTO #Rank FROM ( select px ,px_desc ,sum(case when year = 2021 and quarter = 1 then allw_amt end) as Y1Q1_allw_amt ,sum(case when year = 2022 and quarter = 1 then allw_amt end) as Y2Q1_allw_amt from pct.CLM_OP where op_phys_bucket = 'OP' "+ sbFilters.ToString() + " group by px, px_desc ) t");


            //unique individual
            //unique individual
            //unique individual
            sbSQL.Append("SELECT DISTINCT TOP 10 t.px, t.px_desc ");

            //LOOP DSM!!!
            //,t.Y1Q?_indv
            //,t.Y2Q?_indv

            //LOOP DSM!!
            //, CASE WHEN t.Y1Q?_indv = 0 THEN 'N/A' ELSE  CAST(((t.Y2Q?_indv-t.Y1Q?_indv)/t.Y1Q?_indv) as varchar)  END as Y1Q?_Y2Q?_trend

            sbSQL.Append(" ,t.rank FROM ( select a.px ,a.px_desc ");

            //LOOP DSM!!!
            //,sum(case when a.year = ? and a.quarter = ? then indv end) as Y1Q?_indv
            //,sum(case when a.year = ? and a.quarter = ? then indv end) as Y2Q?_indv

            sbSQL.Append(",b.Y1Q1_Y2Q1_diff as rank from pct.CLM_OP a left join #Rank b on a.px = b.px and a.px_desc = b.px_desc where a.op_phys_bucket = 'OP'  " + sbFilters.ToString() + " group by b.Y1Q1_Y2Q1_diff,a.px, a.px_desc ) t order by t.rank DESC");





            //events
            //events
            //events
            sbSQL.Append("SELECT DISTINCT TOP 10 t.px ,t.px_desc");
            //LOOP DSM!!!
            //,t.Y1Q?_events
            //,t.Y2Q?_events

            //LOOP DSM!!
            //, CASE WHEN t.Y1Q?_events = 0 THEN 'N/A' ELSE  CAST(((t.Y2Q?_events-t.Y1Q?_events)/t.Y1Q?_events) as varchar)  END as Y1Q?_Y2Q?_trend

            sbSQL.Append("FROM ( select distinct a.px ,a.px_desc");
            //LOOP DSM!!!
            //,sum(case when a.year = ? and a.quarter = ? then events end) as Y1Q?_events
            //,sum(case when a.year = ? and a.quarter = ? then events end) as Y2Q?_events

            sbSQL.Append(",b.Y1Q1_Y2Q1_diff as rank from pct.CLM_OP a left join #Rank b on a.px = b.px and a.px_desc = b.px_desc where a.op_phys_bucket = 'OP' " + sbFilters.ToString() + " group by b.Y1Q1_Y2Q1_diff,a.px, a.px_desc ) t order by t.rank DESC");

            var results = _db.LoadDataMultiple(sql: sbSQL.ToString(), token, connectionId: "VCT_DB");

            return results;
        }


        private string getFilterString()
        {
            StringBuilder sbFilters = new StringBuilder();

            return sbFilters.ToString();
        }



    }
}
