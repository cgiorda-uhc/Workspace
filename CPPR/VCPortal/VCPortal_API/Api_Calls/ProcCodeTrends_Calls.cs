using VCPortal_Models.Parameters.MHP;

namespace VCPortal_API.Api_Calls;

public static class ProcCodeTrends_Calls
{

    private static readonly Serilog.ILogger _log = Serilog.Log.ForContext(typeof(ProcCodeTrends_Calls));


    //STATIC EXTENSION FUNCTION ACT AS CONSTRUCTOR 
    public static void ConfigureProcCodeTrendsApi(this WebApplication app)
    {


        app.MapGet(pattern: "/pct_mmfinal", async (IProcCodeTrends_Repo repo, CancellationToken token) =>
        {
            try
            {

                ////RETURN HTTP 200
                var results = await repo.GetMM_FINAL_Async(token);//200 SUCCESS

                if (results != null)
                {
                    return Results.Ok(results);//200 SUCCESS

                }
                _log.Warning("API GetMM_FINAL_Async 404, not found");
                return Results.NotFound(); //404


            }
            catch (Exception ex)
            {
                _log.Error(ex, "API GetMM_FINAL_Async threw an error");

                //RETURN ERROR
                return Results.Problem(ex.Message);

            }
        });


    }


}
