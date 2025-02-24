﻿using Microsoft.AspNetCore.Mvc;
using VCPortal_Models.Parameters.MHP;
using VCPortal_Models.Shared;

namespace VCPortal_API.Api_Calls;

public static class MHPUniverse_Calls
{

    /* TEST
   {
      "state": "'CA'",
      "startDate": "01/01/2021",
      "endDate": "12/31/2021",
      "finC_ARNG_DESC": "'ASO','FULLY INSURED'",
      "mkT_SEG_RLLP_DESC": "'ALL OTHER','KEY ACCOUNTS','MAJOR ACCOUNTS','PUBLIC SECTOR','SMALL BUSINESS 2-50','SMALL BUSINESS 51-99','STATE EXCHANGE','USS'",
      "legalEntities":  ["30100 - UNITEDHEALTHCARE INSURANCE COMPANY","30310 - UNITEDHEALTHCARE SERVICE LLC"],
      "mkT_TYP_DESC": null
    }                    

   {
      "state": "'CA'",
      "startDate": "01/01/2021",
      "endDate": "12/31/2021",
      "cs_TADM_PRDCT_MAP": "'MEDICAID','MMP'"
    }                    
*/

    private static readonly Serilog.ILogger _log = Serilog.Log.ForContext(typeof(MHPUniverse_Calls));


    //STATIC EXTENSION FUNCTION ACT AS CONSTRUCTOR 
    public static void ConfigureMHPApi(this WebApplication app)
    {

        //ALL OF MY API ENDPOINT MAPPING
        //app.MapGet(pattern: "/mhpstates", GetStates).Produces<IEnumerable<string>>(StatusCodes.Status200OK, "application/json").Produces(StatusCodes.Status404NotFound);
        //app.MapGet(pattern: "/mhpstates", async ([FromBody] IMHPData_Repo repo, CancellationToken token) =>
        //{
        //    try
        //    {
        //        ////RETURN HTTP 200
        //        return Results.Ok(await repo.GetStatesAsync(token));//200 SUCCESS
        //    }
        //    catch (Exception ex)
        //    {
        //        //RETURN ERROR
        //        return Results.Problem(ex.Message);

        //    }
        //});

        //ALL OF MY API ENDPOINT MAPPING
        //app.MapGet(pattern: "/mhp_ei", async ([FromQuery] IMHPUniverse_Repo repo,  MHP_EI_Parameters param, CancellationToken token)  => 
        app.MapPost(pattern: "/mhp_ei", async ([FromBody] MHP_EI_Parameters param, IMHPUniverse_Repo repo, CancellationToken token) =>
        {
            try
            {
                ////RETURN HTTP 200
                var results  = await repo.GetMHP_EI_Async(param.State, param.StartDate, param.EndDate, param.Finc_Arng_Desc, param.Mkt_Seg_Rllp_Desc, param.LegalEntities, param.Mkt_Typ_Desc, param.Cust_Seg, token);//200 SUCCESS

                if (results != null)
                {
                    return Results.Ok(results);//200 SUCCESS

                }
                _log.Warning("API GetMHP_EI_Async 404, not found");
                return Results.NotFound(); //404


            }
            catch (Exception ex)
            {

                _log.Error(ex, "API GetMHP_EI_Async threw an error");
                //RETURN ERROR
                return Results.Problem(ex.Message);

            }
        });




        //ALL OF MY API ENDPOINT MAPPING
        //app.MapGet(pattern: "/mhp_ei", async ([FromQuery] IMHPUniverse_Repo repo,  MHP_EI_Parameters param, CancellationToken token)  => 
        app.MapPost(pattern: "/mhp_ei_all", async ([FromBody] MHP_EI_Parameters_All param, IMHPUniverse_Repo repo, CancellationToken token) =>
        {
            try
            {
                ////RETURN HTTP 200
                var results = await repo.GetMHP_EI_ALL_Async(param.State, param.StartDate, param.EndDate, param.Finc_Arng_Desc, param.Mkt_Seg_Rllp_Desc, param.LegalEntities, param.Mkt_Typ_Desc, param.Cust_Seg, token);//200 SUCCESS

                if (results != null)
                {
                    return Results.Ok(results);//200 SUCCESS

                }
                _log.Warning("API GetMHP_EI_Async 404, not found");
                return Results.NotFound(); //404


            }
            catch (Exception ex)
            {

                _log.Error(ex, "API GetMHP_EI_Async threw an error");
                //RETURN ERROR
                return Results.Problem(ex.Message);

            }
        });



        app.MapPost(pattern: "/mhp_ifp", async ([FromBody] MHP_IFP_Parameters param, IMHPUniverse_Repo repo, CancellationToken token) =>
        {
            try
            {
                ////RETURN HTTP 200
                var results = await repo.GetMHP_IFP_Async(param.State, param.StartDate, param.EndDate, param.ProductCodes, token);//200 SUCCESS

                if (results != null)
                {
                    return Results.Ok(results);//200 SUCCESS

                }
                _log.Warning("API GetMHP_IFP_Async 404, not found");
                return Results.NotFound(); //404
            }
            catch (Exception ex)
            {

                _log.Error(ex, "API GetMHP_IFP_Async threw an error");

                //RETURN ERROR
                return Results.Problem(ex.Message);

            }
        });


        app.MapPost(pattern: "/mhp_cs", async ([FromBody] MHP_CS_Parameters param, IMHPUniverse_Repo repo, CancellationToken token) =>
        {
            try
            {
            ////RETURN HTTP 200
            var results = await repo.GetMHP_CS_Async(param.State, param.StartDate, param.EndDate, param.CS_Tadm_Prdct_Map , param.GroupNumbers, token);//200 SUCCESS

                if (results != null)
                {
                    return Results.Ok(results);//200 SUCCESS

                }
                _log.Warning("API GetMHP_CS_Async 404, not found");
                return Results.NotFound(); //404

            }
            catch (Exception ex)
            {

                _log.Error(ex, "API GetMHP_CS_Async threw an error");

                //RETURN ERROR
                return Results.Problem(ex.Message);

            }
        });


        app.MapPost(pattern: "/mhp_ei_details", async ([FromBody] MHP_EI_Parameters param, IMHPUniverse_Repo repo, CancellationToken token) =>
        {
            try
            {
            ////RETURN HTTP 200
            var results = await repo.GetMHPEIDetailsAsync(param.State, param.StartDate, param.EndDate, param.Finc_Arng_Desc, param.Mkt_Seg_Rllp_Desc, param.LegalEntities, param.Mkt_Typ_Desc, param.Cust_Seg,token);//200 SUCCESS


                if (results != null)
                {
                    return Results.Ok(results);//200 SUCCESS

                }
                _log.Warning("API GetMHPEIDetailsAsync 404, not found");
                return Results.NotFound(); //404
            }
            catch (Exception ex)
            {
                _log.Error(ex, "API GetMHPEIDetailsAsync threw an error");

                //RETURN ERROR
                return Results.Problem(ex.Message);

            }
        });

        app.MapPost(pattern: "/mhp_ei_details_all", async ([FromBody] MHP_EI_Parameters_All param, IMHPUniverse_Repo repo, CancellationToken token) =>
        {
            try
            {
                ////RETURN HTTP 200
                var results = await repo.GetMHPEIDetailsAllAsync(param.State, param.StartDate, param.EndDate, param.Finc_Arng_Desc, param.Mkt_Seg_Rllp_Desc, param.LegalEntities, param.Mkt_Typ_Desc, param.Cust_Seg, token);//200 SUCCESS

                if (results != null)
                {
                    return Results.Ok(results);//200 SUCCESS

                }
                _log.Warning("API GetMHPEIDetailsAllAsync 404, not found");
                return Results.NotFound(); //404
            }
            catch (Exception ex)
            {

                _log.Error(ex, "API GetMHPEIDetailsAllAsync threw an error");

                //RETURN ERROR
                return Results.Problem(ex.Message);

            }
        });





        app.MapPost(pattern: "/mhp_cs_details", async ([FromBody] MHP_CS_Parameters param, IMHPUniverse_Repo repo, CancellationToken token) =>
        {
            try
            {
            ////RETURN HTTP 200
            var results = await repo.GetMHPCSDetailsAsync(param.State, param.StartDate, param.EndDate, param.CS_Tadm_Prdct_Map, param.GroupNumbers, token);//200 SUCCESS


                if (results != null)
                {
                    return Results.Ok(results);//200 SUCCESS

                }
                _log.Warning("API GetMHPCSDetailsAsync 404, not found");
                return Results.NotFound(); //404

            }
            catch (Exception ex)
            {
                _log.Error(ex, "API GetMHPCSDetailsAsync threw an error");

                //RETURN ERROR
                return Results.Problem(ex.Message);

            }
        });

        app.MapPost(pattern: "/mhp_ifp_details", async ([FromBody] MHP_IFP_Parameters param, IMHPUniverse_Repo repo, CancellationToken token) =>
        {
            try
            {
            ////RETURN HTTP 200
            var results = await repo.GetMHPIFPDetailsAsync(param.State, param.StartDate, param.EndDate, param.ProductCodes, token);//200 SUCCESS


                if (results != null)
                {
                    return Results.Ok(results);//200 SUCCESS

                }
                _log.Warning("API GetMHPIFPDetailsAsync 404, not found");
                return Results.NotFound(); //404
            }
            catch (Exception ex)
            {
                _log.Error(ex, "API GetMHPIFPDetailsAsync threw an error");

                //RETURN ERROR
                return Results.Problem(ex.Message);

            }
        });

        app.MapGet(pattern: "/mhp_filters", async ( IMHPUniverse_Repo repo, CancellationToken token) =>
        {
            try
            {
            ////RETURN HTTP 200
            var results = await repo.GetMHP_Filters_Async(token);//200 SUCCESS

                if (results != null)
                {
                    return Results.Ok(results);//200 SUCCESS

                }
                _log.Warning("API GetMHP_Filters_Async 404, not found");
                return Results.NotFound(); //404

            }
            catch (Exception ex)
            {

                _log.Error(ex, "API GetMHP_Filters_Async threw an error");

                //RETURN ERROR
                return Results.Problem(ex.Message);

            }
        });

        app.MapGet(pattern: "/mhp_groupstate", async (IMHPUniverse_Repo repo, CancellationToken token) =>
        {
            try
            {

            ////RETURN HTTP 200
            var results = await repo.GetMHP_Group_State_Async(token);//200 SUCCESS

                if (results != null)
                {
                    return Results.Ok(results);//200 SUCCESS

                }
                _log.Warning("API GetMHP_Group_State_Async 404, not found");
                return Results.NotFound(); //404


            }
            catch (Exception ex)
            {
                _log.Error(ex, "API GetMHP_Group_State_Async threw an error");

                //RETURN ERROR
                return Results.Problem(ex.Message);

            }
        });


    }


    //CANT PLUG IN CANCELLATION TOKEN VIA THIS ROUTE. 'SHORTHAND' ABOVE
    private static async Task<IResult> GetMHP_EI(IMHPUniverse_Repo repo, [FromBody] MHP_EI_Parameters param, CancellationToken token)
    {
        try
        {
        ////RETURN HTTP 200
            var results = await repo.GetMHP_EI_Async(param.State, param.StartDate, param.EndDate, param.Finc_Arng_Desc, param.Mkt_Seg_Rllp_Desc, param.LegalEntities, param.Mkt_Typ_Desc, param.Cust_Seg, token);//200 SUCCESS

            if (results != null)
            {
                return Results.Ok(results);//200 SUCCESS

            }
            _log.Warning("API GetMHP_EI_Async 404, not found");
            return Results.NotFound(); //404

        }
        catch (Exception ex)
        {
            _log.Error(ex, "API GetMHP_EI_Async threw an error");


            //RETURN ERROR
            return Results.Problem(ex.Message);

        }
    }

}
