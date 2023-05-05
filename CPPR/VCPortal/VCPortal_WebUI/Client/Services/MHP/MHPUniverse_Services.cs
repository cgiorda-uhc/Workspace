
namespace VCPortal_WebUI.Client.Services.MHP;

public class MHPUniverse_Services
{

    private readonly HttpClient _httpClient;

    public MHPUniverse_Services(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }


    public async Task<List<MHP_EI_Model>> GetMHP_EI_Async()
    {
        var data = await _httpClient.GetFromJsonAsync<List<MHP_EI_Model>>("/mhp_ei");
        return data;
    }

    public async Task<List<MHP_CS_Model>> GetMHP_CS_Async()
    {
        var data = await _httpClient.GetFromJsonAsync<List<MHP_CS_Model>>("/mhp_cs");
        return data;
    }

    public async Task<List<MHP_IFP_Model>> GetMHP_IFP_Async()
    {
        var data = await _httpClient.GetFromJsonAsync<List<MHP_IFP_Model>>("/mhp_ifp");
        return data;
    }

    public async Task<List<MPHUniverseDetails_Model>> GetMHPEIDetailsAsync()
    {
        var data = await _httpClient.GetFromJsonAsync<List<MPHUniverseDetails_Model>>("/mhp_details");
        return data;
    }


    public async Task<List<MHP_Reporting_Filters>> GetMHP_Filters_Async()
    {
        var data = await _httpClient.GetFromJsonAsync<List<MHP_Reporting_Filters>>("/mhp_filters");
        return data;
    }









}
