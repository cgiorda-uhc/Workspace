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



        app.MapGet(pattern: "/etgnrxexclconfig", async (IETGFactSymmetry_Repo repo, CancellationToken token) =>
        {
            try
            {
                _log.Information("Requesting API GetETG_CNFG_ETG_NRX_EXCLD()...");
                ////RETURN HTTP 200
                ///
                var results = await repo.GetETG_CNFG_ETG_NRX_EXCLD(token);

                if (results != null)
                {
                    return Results.Ok(results);//200 SUCCESS

                }
                _log.Warning("API GetETG_CNFG_ETG_NRX_EXCLD() 404, not found");
                return Results.NotFound(); //404
            }
            catch (Exception ex)
            {
                _log.Error(ex, "API GetETG_CNFG_ETG_NRX_EXCLD threw an error");
                //RETURN ERROR
                return Results.Problem(ex.Message);

            }
        });


        app.MapGet(pattern: "/etgspclconfig", async (IETGFactSymmetry_Repo repo, CancellationToken token) =>
        {
            try
            {
                _log.Information("Requesting API GetETG_CNFG_ETG_SPCL()...");
                ////RETURN HTTP 200
                ///
                var results = await repo.GetETG_CNFG_ETG_SPCL(token);

                if (results != null)
                {
                    return Results.Ok(results);//200 SUCCESS

                }
                _log.Warning("API GetETG_CNFG_ETG_SPCL() 404, not found");
                return Results.NotFound(); //404
            }
            catch (Exception ex)
            {
                _log.Error(ex, "API GetETG_CNFG_ETG_SPCL threw an error");
                //RETURN ERROR
                return Results.Problem(ex.Message);

            }
        });

        app.MapGet(pattern: "/etgpcnrxconfig", async (IETGFactSymmetry_Repo repo, CancellationToken token) =>
        {
            try
            {
                _log.Information("Requesting API GetETG_CNFG_PC_ETG_NRX()...");
                ////RETURN HTTP 200
                ///
                var results = await repo.GetETG_CNFG_PC_ETG_NRX(token);

                if (results != null)
                {
                    return Results.Ok(results);//200 SUCCESS

                }
                _log.Warning("API GetETG_CNFG_PC_ETG_NRX() 404, not found");
                return Results.NotFound(); //404
            }
            catch (Exception ex)
            {
                _log.Error(ex, "API GetETG_CNFG_PC_ETG_NRX threw an error");
                //RETURN ERROR
                return Results.Problem(ex.Message);

            }
        });


        app.MapGet(pattern: "/etgptcmodelingcgf", async (IETGFactSymmetry_Repo repo, CancellationToken token) =>
        {
            try
            {
                _log.Information("Requesting API GetETG_PTC_Modeling_Model()...");
                ////RETURN HTTP 200
                ///
                var results = await repo.GetETG_PTC_Modeling_Model(token);

                if (results != null)
                {
                    return Results.Ok(results);//200 SUCCESS

                }
                _log.Warning("API GetETG_PTC_Modeling_Model() 404, not found");
                return Results.NotFound(); //404
            }
            catch (Exception ex)
            {
                _log.Error(ex, "API GetETG_PTC_Modeling_Model threw an error");
                //RETURN ERROR
                return Results.Problem(ex.Message);

            }
        });


        app.MapGet(pattern: "/etgugapcgf", async (IETGFactSymmetry_Repo repo, CancellationToken token) =>
        {
            try
            {
                _log.Information("Requesting API GetETG_UGAP_CFG_Model()...");
                ////RETURN HTTP 200
                ///
                var results = await repo.GetETG_UGAP_CFG_Model(token);

                if (results != null)
                {
                    return Results.Ok(results);//200 SUCCESS

                }
                _log.Warning("API GetETG_UGAP_CFG_Model() 404, not found");
                return Results.NotFound(); //404
            }
            catch (Exception ex)
            {
                _log.Error(ex, "API GetETG_UGAP_CFG_Model threw an error");
                //RETURN ERROR
                return Results.Problem(ex.Message);

            }
        });


        app.MapGet(pattern: "/etgsumfinal", async (IETGFactSymmetry_Repo repo, CancellationToken token) =>
        {
            try
            {
                _log.Information("Requesting API GetETGSummaryFinalAsync()...");
                ///RETURN HTTP 200
                ///
                var results = await repo.GetETGSummaryFinalAsync(token);

                if (results != null)
                {
                    return Results.Ok(results);//200 SUCCESS

                }
                _log.Warning("API GetETGSummaryFinalAsync() 404, not found");
                return Results.NotFound(); //404
            }
            catch (Exception ex)
            {
                _log.Error(ex, "API GetETGSummaryFinalAsync threw an error");
                //RETURN ERROR
                return Results.Problem(ex.Message);

            }
        });



        app.MapGet(pattern: "/etgpdversion", async (IETGFactSymmetry_Repo repo, CancellationToken token) =>
        {
            try
            {
                _log.Information("Requesting API GetPDVersionsAsync()...");
                ///RETURN HTTP 200
                ///
                var results = await repo.GetPDVersionsAsync(token);

                if (results != null)
                {
                    return Results.Ok(results);//200 SUCCESS

                }
                _log.Warning("API GetPDVersionsAsync() 404, not found");
                return Results.NotFound(); //404
            }
            catch (Exception ex)
            {
                _log.Error(ex, "API GetPDVersionsAsync threw an error");
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
