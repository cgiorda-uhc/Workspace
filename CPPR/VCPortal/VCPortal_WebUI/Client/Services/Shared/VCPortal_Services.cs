

using ClosedXML.Graphics;
using FileParsingLibrary.MSExcel;
using System.Reflection;

namespace VCPortal_WebUI.Client.Services.Shared;

public class VCPortal_Services : IVCPortal_Services
{
    private readonly HttpClient _httpClient;
    private readonly IExcelFunctions _excelFunctions;
    private readonly IJSRuntime _JSRuntime;

    public VCPortal_Services(IHttpClientFactory httpClientFactory, IExcelFunctions excelFunctions, IJSRuntime jSRuntime)
    {
        _httpClient = httpClientFactory.CreateClient("VCPortal_Services");
        _excelFunctions = excelFunctions;
        _JSRuntime = jSRuntime; 
    }


    public async Task Insertlog(VCPortal_Models.Models.Shared.VCLog log)
    {
        var response = await _httpClient.PostAsJsonAsync("/log", log);
        response.EnsureSuccessStatusCode();



    }


    public void RunExcelExport<T>(List<T> list, string worksheetTitle, List<string[]> columns)
    {

        var result =  _excelFunctions.ExportToExcel(list, worksheetTitle, columns);
        var final = Convert.ToBase64String(result);

        _JSRuntime.InvokeAsync<VCPortal_Services>(
                "saveAsFile",
                "GeneratedExcel.xlsx",
                final
            );


    }
}
