

using Microsoft.AspNetCore.Http;
using VCPortal_Models.Models.ActiveDirectory;

namespace VCPortal_WebUI.Client.Services.Shared;

public class ClientAuthorizationService : IClientAuthorizationService
{
    //private const string AuthenticationType = "BackEnd";
    private readonly HttpClient _httpClient;


    public ClientAuthorizationService(IHttpClientFactory httpClientFactory)
    {

        var httpClient = httpClientFactory.CreateClient("AuthenticationServices");
        if (httpClient == null) throw new ArgumentNullException(nameof(httpClient));
        _httpClient = httpClient;
    }

    public string UserName { get; set; }
    public List<string> AuthenticatedUsers { get; set; }
    public UserAccessModel CurrentUser { get; set; }
    public async Task<UserAccessModel> GetCurrentUserAsync(string username)
    {

        var response = await _httpClient.GetAsync(string.Format("/user/{0}", username));
        await response.EnsureSuccessStatusCodeAsync();
        var reponseStream = await response.Content.ReadAsStreamAsync();
        var result = await JsonSerializer.DeserializeAsync<UserAccessModel>(reponseStream, new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        });
        return await Task.Run(() => result);

    }




}