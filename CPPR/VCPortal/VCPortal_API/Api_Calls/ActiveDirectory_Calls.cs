using ActiveDirectoryLibrary;
using VCPortal_Models.Models.ActiveDirectory;
using VCPortal_Models.Models.Shared;
using VCPortal_Models.Shared;

namespace VCPortal_API.Api_Calls;

public static class ActiveDirectory_Calls
{

    private static readonly Serilog.ILogger _log = Serilog.Log.ForContext(typeof(ActiveDirectory_Calls));

    private static IConfiguration _config;

    public static void ConfigureADCallsApi(this WebApplication app, IConfiguration config)
    {
        _config = config;

        //ALL OF MY API ENDPOINT MAPPING
        app.MapGet(pattern: "/pbimembership", getPBIMembership).Produces<IEnumerable<PBIMembershipModel>>(StatusCodes.Status200OK, "application/json").Produces(StatusCodes.Status404NotFound);
        app.MapGet(pattern: "/user/{username}", GetUser).Produces<UserAccessModel>(StatusCodes.Status200OK, "application/json").Produces(StatusCodes.Status404NotFound);
    }



    //https://stackoverflow.com/questions/30701006/how-to-get-the-current-logged-in-user-id-in-asp-net-core
    private static async Task<IResult> GetUser(string username)
    {
        try
        {
            _log.Information("Requesting API GetUser()...");

            var section = "ADConnection";
            ///EXTRACT IConfiguration INTO PBIMembershipConfig 
            IADConfig adc = _config.GetSection(section).Get<ADConfig>();

            //WINDOWS IDENTITY IMPERSONATE BLAZOR
            ActiveDirectory ad = new ActiveDirectory(adc.LDAPPath, adc.LDAPDomain, adc.LDAPUser, adc.LDAPPW);
            //string username = System.Security.Principal.WindowsIdentity.GetCurrent().Name.ToLower().TrimStart('m', 's', '\\');
            //string username = Environment.UserName.ToLower().TrimStart('m', 's', '\\');
            var results =ad.GetUserByUserName(username);

            if (results != null)
            {
                var mapped = VCAutoMapper.AutoMapUserAccess<ADUserModel, UserAccessModel>(results);
                return Results.Ok(mapped);//200 SUCCESS

            }

            return Results.NotFound(); //404

        }
        catch (Exception ex)
        {
            _log.Error(ex, "API GetUser threw an error");
            //RETURN ERROR
            return Results.Problem(ex.Message);

        }
    }


    private static List<PBIMembershipModel> getPBIMembership()
    {
        _log.Information("Requesting private function getPBIMembership()...");

        try
        {

            List<PBIMembershipModel> lstFS = new List<PBIMembershipModel>();

            var section = "ADConnection";
            ///EXTRACT IConfiguration INTO PBIMembershipConfig 
            IADConfig adc = _config.GetSection(section).Get<ADConfig>();

            ActiveDirectory ad = new ActiveDirectory(adc.LDAPPath, adc.LDAPDomain, adc.LDAPUser, adc.LDAPPW);

            var groups = ad.GetGroupByName(adc.SearchString);
            foreach (var g in groups)
            {
                var grp = g.Replace("CN=", "");
                var users = ad.GetUserFromGroup(grp);
                foreach (var u in users)
                {
                    lstFS.Add(new PBIMembershipModel { userid = u.LoginName, email = u.EmailAddress, department = u.Department, global_group = grp });
                }
            }

            return lstFS.ToList();
        }
        catch(Exception ex)
        {
            _log.Error(ex, "getPBIMembership() threw an error");
        }

        return null;
    }



}
