using DataAccessLibrary.Data.Abstract;
using DataAccessLibrary.DataAccess;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VCPortal_Models.Dtos.ETGFactSymmetry;

namespace DataAccessLibrary.Data.Concrete.MHP;

public class MHPData_Repo : IMHPData_Repo
{
    private readonly IRelationalDataAccess _db;

    public MHPData_Repo(IRelationalDataAccess db)
    {
        _db = db;
    }


    public Task<IEnumerable<string>> GetStatesAsync(CancellationToken token)
    {

        string strSQL = "SELECT Filter_Value as State_of_Issue FROM stg.MHP_Universes_Filter_Cache  WHERE Filter_Type = 'State_of_Issue' AND Report_Type in ('EI','ALL') ORDER BY State_of_Issue ;";

        var results = _db.LoadData<string>(sql: strSQL, token, connectionId: "ILUCA");

        return results;
    }
}
