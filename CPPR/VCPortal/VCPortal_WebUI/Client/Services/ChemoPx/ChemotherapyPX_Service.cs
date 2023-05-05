
using DocumentFormat.OpenXml.Office2010.Excel;
using System.Net;
using Telerik.Blazor.Components.Scheduler.Models;

namespace VCPortal_WebUI.Client.Services.ChemoPx;

public class ChemotherapyPX_Services : IChemotherapyPX_Services
{

    private readonly HttpClient _httpClient;

    //public ChemotherapyPX_Services(HttpClient httpClient)
    //{
    //    _httpClient = httpClient;
    //}

    public ChemotherapyPX_Services(IHttpClientFactory httpClientFactory)
    {
        _httpClient = httpClientFactory.CreateClient("ChemotherapyPX_Services");
    }

    public async Task<List<ChemotherapyPX_ReadDto>> GetChemoPXListAsync()
    {

        //var result = await _httpClient.GetFromJsonAsync<List<ChemotherapyPX_ReadDto>>("/chemotherapypx");
        //return result;
        var response = await _httpClient.GetAsync("/chemotherapypx");
        await response.EnsureSuccessStatusCodeAsync();
        var reponseStream = await response.Content.ReadAsStreamAsync();
        var result = await JsonSerializer.DeserializeAsync<List<ChemotherapyPX_ReadDto>>(reponseStream, new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        });
        return result;

        //var response = await _httpClient.GetAsync("/chemotherapypx/" + Id);
        //await response.EnsureSuccessStatusCodeAsync();
        //var reponseStream = await response.Content.ReadAsStreamAsync();
        //var result = await JsonSerializer.DeserializeAsync<List<ChemotherapyPX_ReadDto>>(reponseStream);
        //return result;

    }

    public async Task<List<ChemotherapyPX_ReadDto>> GetChemoPXSingleAsync(int? Id)
    {
        //var data = await _httpClient.GetFromJsonAsync<List<ChemotherapyPX_ReadDto>>("/chemotherapypx/" + Id);
        //await data.EnsureSuccessStatusCodeAsync();
        //return data;

        var response = await _httpClient.GetAsync("/chemotherapypx/" + Id);
        await response.EnsureSuccessStatusCodeAsync();
        var reponseStream = await response.Content.ReadAsStreamAsync();
        var result = await JsonSerializer.DeserializeAsync<List<ChemotherapyPX_ReadDto>>(reponseStream, new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        });
        return result;


    }

    public async Task UpdateChemoPXAsync(ChemotherapyPX_UpdateDto chemoPXToUpdate)
    {

        HttpResponseMessage response = await _httpClient.PutAsJsonAsync("/chemotherapypx", chemoPXToUpdate);
        await response.EnsureSuccessStatusCodeAsync();

    }

    public async Task DeleteChemoPXAsync(int? Id)
    {
        HttpResponseMessage response = await _httpClient.DeleteAsync("/chemotherapypx/" + Id);
        await response.EnsureSuccessStatusCodeAsync();
    }

    public async Task<int> InsertChemoPXAsync(ChemotherapyPX_CreateDto chemoPXToInsert)
    {
        var response = await _httpClient.PostAsJsonAsync("/chemotherapypx", chemoPXToInsert);
        await response.EnsureSuccessStatusCodeAsync();
        var data = await response.Content.ReadAsStringAsync();
        int result = JsonSerializer.Deserialize<int>(data);

        return (int)result;

        ////HttpResponseMessage response = await _httpClient.PostAsJsonAsync("/chemotherapypx", chemoPXToInsert);
        ////response.EnsureSuccessStatusCode();

    }

    public async Task<List<ChemotherapyPXFilters>> GetAllFilters()
    {

        var data = await _httpClient.GetFromJsonAsync<List<ChemotherapyPXFilters>>("/filters");
        return data;

    }

    public async Task<List<ProcCodesModel>> GetAllProcCodes()
    {
        var data = await _httpClient.GetFromJsonAsync<List<ProcCodesModel>>("/proc_codes");
        return data;
    }



    //private List<string> _proc_codes { get; set; }
    //public async  Task<DataEnvelope<string>> GetProcCodes(DataSourceRequest request)
    //{
    //    if(_proc_codes == null)
    //        _proc_codes = GlobalObjects.Proc_Codes.Select(s => s.Proc_Cd + " ~ " + s.Proc_Desc).ToList();


    //    var result = await _proc_codes.ToDataSourceResultAsync(request);
    //    DataEnvelope<string> dataToReturn = new DataEnvelope<string>
    //    {
    //        Data = result.Data.Cast<string>().ToList(),
    //        Total = result.Total
    //    };

    //    return await Task.FromResult(dataToReturn);
    //}



    public async Task<List<Code_Category_Model>> GetAllCodeCategory()
    {
        var data = await _httpClient.GetFromJsonAsync<List<Code_Category_Model>>("/codecategory");
        return data;
    }
    public async Task<List<ASP_Category_Model>> GetAllASPCategory()
    {
        var data = await _httpClient.GetFromJsonAsync<List<ASP_Category_Model>>("/aspcategory");
        return data;
    }
    public async Task<List<Drug_Adm_Mode_Model>> GetAllDrugAdmMode()
    {
        var data = await _httpClient.GetFromJsonAsync<List<Drug_Adm_Mode_Model>>("/drugadmmode");
        return data;
    }
    public async Task<List<PA_Drugs_Model>> GetAllPADrugs()
    {
        var data = await _httpClient.GetFromJsonAsync<List<PA_Drugs_Model>>("/padrugs");
        return data;
    }
    public async Task<List<CEP_Pay_Cd_Model>> GetAllCEPPayCd()
    {
        var data = await _httpClient.GetFromJsonAsync<List<CEP_Pay_Cd_Model>>("/ceppaycd");
        return data;
    }
    public async Task<List<CEP_Enroll_Cd_Model>> GetAllCEPEnrollCd()
    {
        var data = await _httpClient.GetFromJsonAsync<List<CEP_Enroll_Cd_Model>>("/cepenrolcd");
        return data;
    }
}




