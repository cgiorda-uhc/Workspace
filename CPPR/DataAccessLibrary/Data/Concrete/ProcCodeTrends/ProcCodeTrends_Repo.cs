using DataAccessLibrary.Data.Abstract;
using DataAccessLibrary.DataAccess;
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


        public Task<IEnumerable<MM_FINAL_Model>> GetCLM_OP_Async(string LOB, string Region, string State, string Product, string CSProduct, string FundingType, string LegalEntity, string Source, string CSDualIndicator, string MRDualIndicator, CancellationToken token)
        {

            string strSQL = "SELECT * FROM [VCT_DB].[pct].[CLM_OP];";

            var results = _db.LoadData<MM_FINAL_Model>(sql: strSQL, token, connectionId: "VCT_DB");

            return results;
        }
    }
}
