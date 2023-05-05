using SharedFunctionsLibrary;
using System.Text.Json;
using VCPortal_Models.Models.ActiveDirectory;


namespace VCPortal_WPF_ViewModel.Shared;
public class Authentication
{
    public static Serilog.ILogger Log { get; set; }
    public static string UserName { get; set; }

    public static UserAccessModel CurrentUser { get; set; }


    public static async Task SetCurrentUserAsync(string baseURL, string endpointURL)
    {
        try
        {

            Log.Information("Running Authentication.SetCurrentUserAsync() for {CurrentUser}...", UserName);
            WebAPIConsume.BaseURI = baseURL;
            var response = WebAPIConsume.GetCall(endpointURL);
            if (response.Result.StatusCode == System.Net.HttpStatusCode.OK)
            {
                var reponseStream = await response.Result.Content.ReadAsStreamAsync();
                var result = await JsonSerializer.DeserializeAsync<UserAccessModel>(reponseStream, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });

                CurrentUser = result;
                Log.Information("Authentication.SetCurrentUserAsync() succeeded for {CurrentUser}...", UserName);
            }
            else
            {
                Log.Error("Authentication.SetCurrentUserAsync threw an error {CurrentUser}: " + response.Result.StatusCode.ToString(), UserName);
            }
        }
        catch (Exception ex)
        {
            Log.Fatal(ex, "Authentication.SetCurrentUserAsync.WebAPIConsume.GetCall threw an error for {CurrentUser}", UserName);
        }
        


  

    }
}
