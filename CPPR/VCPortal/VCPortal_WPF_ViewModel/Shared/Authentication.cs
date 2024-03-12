using ActiveDirectoryLibrary;
using IdentityModel.OidcClient;
using SharedFunctionsLibrary;
using System.Text.Json;
using VCPortal_Models.Models.ActiveDirectory;
using static Org.BouncyCastle.Math.EC.ECCurve;
using VCPortal_Models.Shared;
using Microsoft.Extensions.Configuration;
using NPOI.Util;
using Newtonsoft.Json;


namespace VCPortal_WPF_ViewModel.Shared;
public class Authentication
{
    public static Serilog.ILogger Log { get; set; }
    public static string UserName { get; set; }

    public static UserAccessModel CurrentUser { get; set; }


    public static async Task SetCurrentUserAsync(string baseURL, string endpointURL, IConfiguration config)
    {
        try
        {

            var result = await  GetUser(UserName, config);

            CurrentUser = new UserAccessModel();

            CurrentUser.FirstName = result.FirstName; 
            CurrentUser.LastName = result.LastName;
            CurrentUser.MiddleName = result.MiddleName;



            CurrentUser.FullName = result.FullName;

            CurrentUser.LoginName = result.LoginName;


            CurrentUser.EmailAddress = result.EmailAddress;

            CurrentUser.Groups = result.Groups;



    // = JsonConvert.DeserializeObject<UserAccessModel>(JsonConvert.SerializeObject(result));


            Log.Information("Running Authentication.SetCurrentUserAsync() for {CurrentUser}...", UserName);
            //WebAPIConsume.BaseURI = baseURL;
            //var response = WebAPIConsume.GetCall(endpointURL);
            //if (response.Result.StatusCode == System.Net.HttpStatusCode.OK)
            //{
            //    var reponseStream = await response.Result.Content.ReadAsStreamAsync();
            //    var result = await JsonSerializer.DeserializeAsync<UserAccessModel>(reponseStream, new JsonSerializerOptions
            //    {
            //        PropertyNameCaseInsensitive = true
            //    });

            //    CurrentUser = result;
            //    Log.Information("Authentication.SetCurrentUserAsync() succeeded for {CurrentUser}...", UserName);
            //}
            //else
            //{
            //    Log.Error("Authentication.SetCurrentUserAsync threw an error {CurrentUser}: " + response.Result.StatusCode.ToString(), UserName);
            //}
        }
        catch (Exception ex)
        {
            Log.Fatal(ex, "Authentication.SetCurrentUserAsync.WebAPIConsume.GetCall threw an error for {CurrentUser}", UserName);
        }
        


    }


    private static async Task<ADUserModel> GetUser(string username, IConfiguration config)
    {
        try
        {
            Log.Information("Requesting API GetUser()...");

            var section = "ADConnection";
            ///EXTRACT IConfiguration INTO PBIMembershipConfig 
            IADConfig adc = config.GetSection(section).Get<ADConfig>();

            //WINDOWS IDENTITY IMPERSONATE BLAZOR
            ActiveDirectory ad = new ActiveDirectory(adc.LDAPPath, adc.LDAPDomain, adc.LDAPUser, adc.LDAPPW);
            //string username = System.Security.Principal.WindowsIdentity.GetCurrent().Name.ToLower().TrimStart('m', 's', '\\');
            //string username = Environment.UserName.ToLower().TrimStart('m', 's', '\\');
            var results = ad.GetUserByUserName(username);

            //if (results != null)
            //{
            //    var mapped = VCAutoMapper.AutoMapUserAccess<ADUserModel, UserAccessModel>(results);
            //    return Results.Ok(mapped);//200 SUCCESS

            //}
            return results; //404


        }
        catch (Exception ex)
        {
            Log.Error(ex, "API GetUser threw an error");
            //RETURN ERROR
            return null;

        }

        
    }

}
