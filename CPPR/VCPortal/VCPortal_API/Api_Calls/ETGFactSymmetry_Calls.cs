using DataAccessLibrary.Data.Abstract;
using System.Web.Http;
using VCPortal_Models.Dtos.ETGFactSymmetry;
using VCPortal_Models.Models.ETGSymmetry;
using VCPortal_Models.Models.Shared;
using VCPortal_Models.Parameters.MHP;
using VCPortal_Models.Shared;

namespace VCPortal_API.Api_Calls;

public static class ETGFactSymmetry_Calls
{
    private static readonly Serilog.ILogger _log = Serilog.Log.ForContext(typeof(ETGFactSymmetry_Calls));

    public static void ConfigureETGFactSymmetryApi(this WebApplication app)
    {

        //ALL OF MY API ENDPOINT MAPPING
        app.MapGet(pattern: "/etgsymmetry", async (IETGFactSymmetry_Repo repo, CancellationToken token) =>
        {
            try
            {
                _log.Information("Requesting API GetETGFactSymmetryDisplay()...");
                ///RETURN HTTP 200
                ///
                var results = await repo.GetETGFactSymmetryDisplayAsync(token);

                if (results != null)
                {
                    return Results.Ok(results);//200 SUCCESS

                }
                _log.Warning("API GetETGFactSymmetryDisplay() 404, not found");
                return Results.NotFound(); //404
            }
            catch (Exception ex)
            {
                _log.Error(ex, "API GetETGFactSymmetryDisplay threw an error");
                //RETURN ERROR
                return Results.Problem(ex.Message);

            }
        });


        app.MapGet(pattern: "/etgsymmetrytracking", async (IETGFactSymmetry_Repo repo, CancellationToken token) =>
        {
            try
            {
                _log.Information("Requesting API GetETGTracking()...");
                ////RETURN HTTP 200
                ///
                var results = await repo.GetETGTrackingAsync(token);

                if (results != null)
                {
                    return Results.Ok(results);//200 SUCCESS

                }
                _log.Warning("API GetETGTracking() 404, not found");
                return Results.NotFound(); //404
            }
            catch (Exception ex)
            {
                _log.Error(ex, "API GetETGTracking threw an error");
                //RETURN ERROR
                return Results.Problem(ex.Message);

            }
        });



        app.MapGet(pattern: "/etgpcconfig", async (IETGFactSymmetry_Repo repo, CancellationToken token) =>
        {
            try
            {
                _log.Information("Requesting API GetETGPatientCentricConfig()...");
                ////RETURN HTTP 200
                ///
                var results = await repo.GetETGPatientCentricConfigAsync(token);

                if (results != null)
                {
                    return Results.Ok(results);//200 SUCCESS

                }
                _log.Warning("API GetETGPatientCentricConfig() 404, not found");
                return Results.NotFound(); //404
            }
            catch (Exception ex)
            {
                _log.Error(ex, "API GetETGPatientCentricConfig threw an error");
                //RETURN ERROR
                return Results.Problem(ex.Message);

            }
        });


        app.MapGet(pattern: "/etgpeconfig", async (IETGFactSymmetry_Repo repo, CancellationToken token) =>
        {
            try
            {
                _log.Information("Requesting API GetETGPopEpisodeConfig()...");
                ////RETURN HTTP 200
                ///
                var results = await repo.GetETGPopEpisodeConfigAsync(token);

                if (results != null)
                {
                    return Results.Ok(results);//200 SUCCESS

                }
                _log.Warning("API GetETGPopEpisodeConfig() 404, not found");
                return Results.NotFound(); //404
            }
            catch (Exception ex)
            {
                _log.Error(ex, "API GetETGPopEpisodeConfig threw an error");
                //RETURN ERROR
                return Results.Problem(ex.Message);

            }
        });


        app.MapGet(pattern: "/etgrxnrxconfig", async (IETGFactSymmetry_Repo repo, CancellationToken token) =>
        {
            try
            {
                _log.Information("Requesting API GetETGRxNrxConfig()...");
                ////RETURN HTTP 200
                ///
                var results = await repo.GetETGRxNrxConfigAsync(token);

                if (results != null)
                {
                    return Results.Ok(results);//200 SUCCESS

                }
                _log.Warning("API GetETGRxNrxConfig() 404, not found");
                return Results.NotFound(); //404
            }
            catch (Exception ex)
            {
                _log.Error(ex, "API GetETGRxNrxConfig threw an error");
                //RETURN ERROR
                return Results.Problem(ex.Message);

            }
        });



        app.MapPost(pattern: "/etginsert", async (List<ETGFactSymmetry_Tracking_UpdateDto> etg, IETGFactSymmetry_Repo repo) =>
        {
            try
            {
                _log.Information("Requesting API InsertETGFactSymmetry()...");
                await repo.InsertETGFactSymmetryTracking(etg, "VCT_DB");

                return Results.Ok();  ////RETURN HTTP 200

            }
            catch (Exception ex)
            {
                _log.Error(ex, "API InsertETGFactSymmetry threw an error");
                //RETURN ERROR
                return Results.Problem(ex.Message);

            }
        });


    }

    


}
