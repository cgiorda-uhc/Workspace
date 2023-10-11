using DataAccessLibrary.Data.Abstract;
using DataAccessLibrary.DataAccess;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VCPortal_Models.Parameters.MHP;

namespace DataAccessLibrary.Data.Concrete.EDCAdhoc
{
    public class EDCAdhoc_Repo : IEDCAdhoc_Repo
    {
        private readonly IRelationalDataAccess _db;

        public EDCAdhoc_Repo(IRelationalDataAccess db)
        {
            _db = db;
        }

        public Task<IEnumerable<MHP_Group_State_Model>> GetMHP_Group_State_Async(CancellationToken token)
        {

            string strSQL = "SELECT [State_of_Issue],[Group_Number] FROM [VCT_DB].[mhp].[MHP_Group_State];";

            var results = _db.LoadData<MHP_Group_State_Model>(sql: strSQL, token, connectionId: "VCT_DB");

            return results;
        }

    }
}
