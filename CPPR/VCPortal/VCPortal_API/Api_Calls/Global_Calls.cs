using AutoMapper;
using Microsoft.AspNetCore.Http;
using System.Collections.Generic;
using VCPortal_API.MapperProfiles.ChemoPx;
using VCPortal_Models.Models.Shared;
using VCPortal_Models.Shared;

namespace VCPortal_API.Api_Calls;

public static class Global_Calls
{

    //STATIC EXTENSION FUNCTION ACT AS CONSTRUCTOR 
    public static void ConfigureGlobalApi(this WebApplication app)
    {

        //ALL OF MY API ENDPOINT MAPPING
        app.MapPost(pattern: "/log", InsertLog);
    }



    private static async Task<IResult> InsertLog(VCLog log, ILog_Repo repo)
    {
        try
        {

            await repo.InsertLog(log);

            return Results.Ok();  ////RETURN HTTP 200

        }
        catch (Exception ex)
        {
            //RETURN ERROR
            return Results.Problem(ex.Message);

        }
    }

}
