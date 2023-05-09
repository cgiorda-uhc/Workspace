
using Serilog;
using Teradata.Net.Security.Mechanisms;
using VCPortal_Models.Models.Shared;
using VCPortal_Models.Shared;

namespace VCPortal_API.Api_Calls;
public static class ChemotherapyPX_Calls
{

    private static readonly Serilog.ILogger _log = Serilog.Log.ForContext(typeof(ChemotherapyPX_Calls));

    //STATIC EXTENSION FUNCTION ACT AS CONSTRUCTOR 
    public static void ConfigureChemoPXApi(this WebApplication app)
    {

        //ALL OF MY API ENDPOINT MAPPING
        app.MapGet(pattern: "/chemotherapypx", GetAllChemotherapyPX).Produces<IEnumerable<ChemotherapyPX_ReadDto>>(StatusCodes.Status200OK, "application/json").Produces(StatusCodes.Status404NotFound);
        app.MapGet(pattern: "/chemotherapypx/{id}", GetChemotherapyPX).Produces<ChemotherapyPX_ReadDto>(StatusCodes.Status200OK, "application/json").Produces(StatusCodes.Status404NotFound);
        app.MapPost(pattern: "/chemotherapypx", InsertChemotherapyPX).Produces(StatusCodes.Status200OK).Produces(StatusCodes.Status404NotFound);


        app.MapGet(pattern: "/chemopxtracking", GetChemotherapyPXTracking).Produces<IEnumerable<ChemotherapyPX_Tracking_ReadDto>>(StatusCodes.Status200OK, "application/json").Produces(StatusCodes.Status404NotFound);

        //app.MapGet(pattern: "/filters", GetAllFilters).Produces<IEnumerable<ChemotherapyPXFilters>>(StatusCodes.Status200OK).Produces(StatusCodes.Status404NotFound);
        app.MapGet(pattern: "/proc_codes", GetAllProcCodes).Produces<IEnumerable<ProcCodesModel>>(StatusCodes.Status200OK).Produces(StatusCodes.Status404NotFound);


        app.MapGet(pattern: "/codecategory", GetAllCodeCategory).Produces<IEnumerable<Code_Category_Model>>(StatusCodes.Status200OK).Produces(StatusCodes.Status404NotFound);
        app.MapGet(pattern: "/aspcategory", GetAllASPCategory).Produces<IEnumerable<ASP_Category_Model>>(StatusCodes.Status200OK).Produces(StatusCodes.Status404NotFound);
        app.MapGet(pattern: "/drugadmmode", GetAllDrugAdmMode).Produces<IEnumerable<Drug_Adm_Mode_Model>>(StatusCodes.Status200OK).Produces(StatusCodes.Status404NotFound);
        app.MapGet(pattern: "/padrugs", GetAllPADrugs).Produces<IEnumerable<PA_Drugs_Model>>(StatusCodes.Status200OK).Produces(StatusCodes.Status404NotFound);
        app.MapGet(pattern: "/ceppaycd", GetAllCEPPayCd).Produces<IEnumerable<CEP_Pay_Cd_Model>>(StatusCodes.Status200OK).Produces(StatusCodes.Status404NotFound);
        app.MapGet(pattern: "/cepenrolcd", GetAllCEPEnrollCd).Produces<IEnumerable<CEP_Enroll_Cd_Model>>(StatusCodes.Status200OK).Produces(StatusCodes.Status404NotFound);


        app.MapGet(pattern: "/source", GetSource).Produces<IEnumerable<string>>(StatusCodes.Status200OK).Produces(StatusCodes.Status404NotFound);
        app.MapGet(pattern: "/cepenrexcl", GetCEP_Enroll_Excl_Desc).Produces<IEnumerable<string>>(StatusCodes.Status200OK).Produces(StatusCodes.Status404NotFound);


        ////SETUP MAPPING FOR DTO
        //var mapperConfig = new MapperConfiguration(mc =>
        //{
        //    mc.AddProfile(new ChemotherapyPX_Profile());
        //});
        //_mapper = mapperConfig.CreateMapper();

    }

    //WRAP RESULTS IN PROPER HTTP CODES VIA IResult
    private static async Task<IResult> GetAllChemotherapyPX(IChemotherapyPX_Repo repo)
    {
        try
        {
            _log.Information("Requesting API GetAllChemotherapyPX()...");

            var results = await repo.GetAllChemotherapyPX();
            if (results != null)
            {
                var mapped = VCAutoMapper.AutoMapChemotherapyPX<IEnumerable<ChemotherapyPXModel>, IEnumerable<ChemotherapyPX_ReadDto>>(results);
                return Results.Ok(mapped);//200 SUCCESS

            }

            _log.Warning("API GetAllChemotherapyPX() 404, not found");
            return Results.NotFound(); //404


            //RETURN HTTP 200
            //return Results.Ok(_mapper.Map<IEnumerable<ChemotherapyPX_ReadDto>>(await repo.GetAllChemotherapyPX()));//200 SUCCESS
            //return Results.Ok(await repo.GetAllChemotherapyPX());
        }
        catch (Exception ex)
        {
            _log.Error(ex, "API GetAllChemotherapyPX threw an error");


            //RETURN ERROR
            return Results.Problem(ex.Message);

        }
    }


    private static async Task<IResult> GetChemotherapyPXTracking(IChemotherapyPX_Repo repo)
    {
        try
        {
            _log.Information("Requesting API GetChemotherapyPXTracking()...");

            var results = await repo.GetChemotherapyPXTrackingAsync();
            if (results != null)
            {
                return Results.Ok(results);//200 SUCCESS

            }

            _log.Warning("API GetChemotherapyPXTracking() 404, not found");
            return Results.NotFound(); //404


            //RETURN HTTP 200
            //return Results.Ok(_mapper.Map<IEnumerable<ChemotherapyPX_ReadDto>>(await repo.GetAllChemotherapyPX()));//200 SUCCESS
            //return Results.Ok(await repo.GetAllChemotherapyPX());
        }
        catch (Exception ex)
        {
            _log.Error(ex, "API GetChemotherapyPXTracking threw an error");


            //RETURN ERROR
            return Results.Problem(ex.Message);

        }
    }



    private static async Task<IResult> GetChemotherapyPX(int id, IChemotherapyPX_Repo repo)
    {
        try
        {
            _log.Information("Requesting API GetChemotherapyPX(id)...");
            var results = await repo.GetChemotherapyPX(id);
            if (results != null)
            {
                var mapped = VCAutoMapper.AutoMapChemotherapyPX<ChemotherapyPXModel, ChemotherapyPX_ReadDto>(results);
                return Results.Ok(mapped);//200 SUCCESS


                //return Results.Ok(_mapper.Map<ChemotherapyPX_ReadDto>(results));//RETURN HTTP 200SUCCESS
            }
            _log.Warning("API GetChemotherapyPX(id) 404, not found");
            return Results.NotFound(); //404
        }
        catch (Exception ex)
        {
            _log.Error(ex, "API GetChemotherapyPX(id) threw an error");
            //RETURN ERROR
            return Results.Problem(ex.Message);

        }
    }


    private static async Task<IResult> InsertChemotherapyPX(List<ChemotherapyPX_Tracking_CUD_Dto> chemPX, IChemotherapyPX_Repo repo)
    {
        try
        {
            _log.Information("Requesting API InsertChemotherapyPX()...");
            await repo.InsertChemotherapyPXTracking(chemPX);

            return Results.Ok();  ////RETURN HTTP 200

        }
        catch (Exception ex)
        {
            _log.Error(ex, "API InsertChemotherapyPX threw an error");
            //RETURN ERROR
            return Results.Problem(ex.Message);

        }
    }


    

    private static async Task<IResult> GetAllFilters(IChemotherapyPX_Repo repo)
    {
        try
        {
            _log.Information("Requesting API GetAllFilters()...");
            var results = await repo.GetAllFilters();
            return Results.Ok(results);//200 SUCCESS

        }
        catch (Exception ex)
        {
            _log.Error(ex, "API GetAllFilters threw an error");
            //RETURN ERROR
            return Results.Problem(ex.Message);

        }
    }


    private static async Task<IResult> GetAllProcCodes(IChemotherapyPX_Repo repo)
    {
        try
        {
            _log.Information("Requesting API GetAllProcCodes()...");
            var results = await repo.GetAllProcCodes();
            return Results.Ok(results);//200 SUCCESS

        }
        catch (Exception ex)
        {
            _log.Error(ex, "API GetAllProcCodes threw an error");
            //RETURN ERROR
            return Results.Problem(ex.Message);

        }
    }

    private static async Task<IResult> GetAllCodeCategory(IChemotherapyPX_Repo repo)
    {
        try
        {
            _log.Information("Requesting API GetAllCodeCategory()...");
            var results = await repo.GetAllCodeCategory();
            return Results.Ok(results);//200 SUCCESS

        }
        catch (Exception ex)
        {
            _log.Error(ex, "API GetAllCodeCategory threw an error");
            //RETURN ERROR
            return Results.Problem(ex.Message);

        }
    }

    private static async Task<IResult> GetAllASPCategory(IChemotherapyPX_Repo repo)
    {
        try
        {
            _log.Information("Requesting API GetAllASPCategory()...");
            var results = await repo.GetAllASPCategory();
            return Results.Ok(results);//200 SUCCESS

        }
        catch (Exception ex)
        {
            _log.Error(ex, "API GetAllASPCategory threw an error");
            //RETURN ERROR
            return Results.Problem(ex.Message);

        }
    }

    private static async Task<IResult> GetAllDrugAdmMode(IChemotherapyPX_Repo repo)
    {
        try
        {
            _log.Information("Requesting API GetAllDrugAdmMode()...");
            var results = await repo.GetAllDrugAdmMode();
            return Results.Ok(results);//200 SUCCESS

        }
        catch (Exception ex)
        {
            _log.Error(ex, "API GetAllDrugAdmMode threw an error");
            //RETURN ERROR
            return Results.Problem(ex.Message);

        }
    }

    private static async Task<IResult> GetAllPADrugs(IChemotherapyPX_Repo repo)
    {
        try
        {
            _log.Information("Requesting API GetAllPADrugs()...");
            var results = await repo.GetAllPADrugs();
            return Results.Ok(results);//200 SUCCESS

        }
        catch (Exception ex)
        {
            _log.Error(ex, "API GetAllPADrugs threw an error");
            //RETURN ERROR
            return Results.Problem(ex.Message);

        }
    }

    private static async Task<IResult> GetAllCEPPayCd(IChemotherapyPX_Repo repo)
    {
        try
        {
            _log.Information("Requesting API GetAllCEPPayCd()...");
            var results = await repo.GetAllCEPPayCd();
            return Results.Ok(results);//200 SUCCESS

        }
        catch (Exception ex)
        {
            _log.Error(ex, "API GetAllCEPPayCd threw an error");
            //RETURN ERROR
            return Results.Problem(ex.Message);

        }
    }

    private static async Task<IResult> GetAllCEPEnrollCd(IChemotherapyPX_Repo repo)
    {
        try
        {
            _log.Information("Requesting API GetAllCEPEnrollCd()...");
            var results = await repo.GetAllCEPEnrollCd();
            return Results.Ok(results);//200 SUCCESS

        }
        catch (Exception ex)
        {
            _log.Error(ex, "API GetAllCEPEnrollCd threw an error");
            //RETURN ERROR
            return Results.Problem(ex.Message);

        }
    }

    private static async Task<IResult> GetSource(IChemotherapyPX_Repo repo)
    {
        try
        {
            _log.Information("Requesting API GetSource()...");
            var results = await repo.GetSource();
            return Results.Ok(results);//200 SUCCESS

        }
        catch (Exception ex)
        {
            _log.Error(ex, "API GetSource threw an error");
            //RETURN ERROR
            return Results.Problem(ex.Message);

        }
    }


    private static async Task<IResult> GetCEP_Enroll_Excl_Desc(IChemotherapyPX_Repo repo)
    {
        try
        {
            _log.Information("Requesting API GetCEP_Enroll_Excl_Desc()...");
            var results = await repo.GetCEP_Enroll_Excl_Desc();
            return Results.Ok(results);//200 SUCCESS

        }
        catch (Exception ex)
        {
            _log.Error(ex, "API GetCEP_Enroll_Excl_Desc threw an error");
            //RETURN ERROR
            return Results.Problem(ex.Message);

        }
    }
}
