
using DataAccessLibrary.Data.Abstract;
using DataAccessLibrary.DataAccess;
using System.Diagnostics;
using VCPortal_Models.Models.ChemoPx;
using VCPortal_Models.Models.Shared;

namespace DataAccessLibrary.Data.Concrete.Shared;
public class Logs_Repo : ILog_Repo
{
    private readonly IRelationalDataAccess _db;

    public Logs_Repo(IRelationalDataAccess db)
    {
        _db = db;
    }


    public async Task InsertLog(VCLog log)
    {
        var result = await _db.ExecuteScalar(storedProcedure: "dbo.sp_Log_Insert",
        new
        {
            log.log_level,
            log.event_name,
            log.source,
            log.exception_message,
            log.stack_trace,
            log.state
        });

    }


}
