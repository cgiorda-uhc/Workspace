using DataAccessLibrary.Data.Abstract;
using DataAccessLibrary.DataAccess;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VCPortal_Models.Models.ProcCodeTrends;
using VCPortal_Models.Parameters.MHP;

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


    }
}
