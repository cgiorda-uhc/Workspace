using FileParsingLibrary.MSExcel;
using VCPortal_Models.Dtos.ETGFactSymmetry;

namespace VCPortal_WebUI.Client.Services.ETGFactSymmetry;

public class ETGFactSymmetryServices : IETGFactSymmetryServices
{
    private readonly HttpClient _httpClient;


    //public ChemotherapyPX_Services(HttpClient httpClient)
    //{
    //    _httpClient = httpClient;
    //}

    public ETGFactSymmetryServices(IHttpClientFactory httpClientFactory)
    {
        _httpClient = httpClientFactory.CreateClient("ETGFactSymmetry_Services");

    }


    public async Task<List<ETGFactSymmetry_ReadDto>> GetETGFactSymmetryDisplayAsync()
    {

        var response = await _httpClient.GetAsync("/etgsymmetry");
        await response.EnsureSuccessStatusCodeAsync();
        var reponseStream = await response.Content.ReadAsStreamAsync();
        var result = await JsonSerializer.DeserializeAsync<List<ETGFactSymmetry_ReadDto>>(reponseStream, new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        });
        return result;


    }







}
