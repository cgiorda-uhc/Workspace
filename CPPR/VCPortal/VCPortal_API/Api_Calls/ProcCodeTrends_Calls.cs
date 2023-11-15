using Microsoft.AspNetCore.Mvc;
using VCPortal_Models.Models.ProcCodeTrends;
using VCPortal_Models.Parameters.MHP;
using VCPortal_Models.Parameters.ProcCodeTrends;

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

        app.MapPost(pattern: "pct_clmphys", async ([FromBody] ProcCodeTrends_Parameters param, IProcCodeTrends_Repo repo, CancellationToken token) =>
        {
            try
            {
                ////RETURN HTTP 200
                var results = await repo.GetCLM_PHYS_Async(param, token);//200 SUCCESS

                if (results != null)
                {
                    return Results.Ok(results);//200 SUCCESS

                }
                _log.Warning("API GetCLM_PHYS_Async 404, not found");
                return Results.NotFound(); //404


            }
            catch (Exception ex)
            {

                _log.Error(ex, "API GetCLM_PHYS_Async threw an error");
                //RETURN ERROR
                return Results.Problem(ex.Message);

            }
        });


        app.MapPost(pattern: "pct_clmop", async ([FromBody] ProcCodeTrends_Parameters param, IProcCodeTrends_Repo repo, CancellationToken token) =>
        {
            try
            {
                ////RETURN HTTP 200
                var results = await repo.GetCLM_PHYS_Async(param, token);//200 SUCCESS

                if (results != null)
                {
                    return Results.Ok(results);//200 SUCCESS

                }
                _log.Warning("API GetCLM_OP_Async 404, not found");
                return Results.NotFound(); //404


            }
            catch (Exception ex)
            {

                _log.Error(ex, "API GetCLM_OP_Async threw an error");
                //RETURN ERROR
                return Results.Problem(ex.Message);

            }
        });

        app.MapPost(pattern: "pct_datespan", async (IProcCodeTrends_Repo repo, CancellationToken token) =>
        {
            try
            {
                ////RETURN HTTP 200
                var results = await repo.GetDateSpan_Async(token);//200 SUCCESS

                if (results != null)
                {
                    return Results.Ok(results);//200 SUCCESS

                }
                _log.Warning("API GetDateSpan_Async 404, not found");
                return Results.NotFound(); //404


            }
            catch (Exception ex)
            {

                _log.Error(ex, "API GetDateSpan_Async threw an error");
                //RETURN ERROR
                return Results.Problem(ex.Message);

            }
        });


        app.MapPost(pattern: "pct_mainreport", async ([FromBody] ProcCodeTrends_Parameters param, [FromBody] List<DateSpan_Model> dsm, IProcCodeTrends_Repo repo, CancellationToken token) =>
        {
            try
            {
                ////RETURN HTTP 200
                var results = await repo.GetMainPCTReport_Async(param, dsm, token);//200 SUCCESS

                if (results != null)
                {
                    return Results.Ok(results);//200 SUCCESS

                }
                _log.Warning("API GetMainPCTReport_Async 404, not found");
                return Results.NotFound(); //404


            }
            catch (Exception ex)
            {

                _log.Error(ex, "API GetMainPCTReport_Async threw an error");
                //RETURN ERROR
                return Results.Problem(ex.Message);

            }
        });

    }


}
