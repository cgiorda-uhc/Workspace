using CommunityToolkit.Mvvm.ComponentModel;
using FileParsingLibrary.MSExcel;
using Microsoft.Extensions.Configuration;
using SharedFunctionsLibrary;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using VCPortal_Models.Configuration.HeaderInterfaces.Abstract;
using VCPortal_Models.Dtos.ETGFactSymmetry;
using VCPortal_WPF_ViewModel.Projects.ETGFactSymmetry;
using VCPortal_WPF_ViewModel.Shared;

namespace VCPortal_WPF_ViewModel.Projects.MHP;
public partial class MHPViewModel : ObservableObject, ViewModelBase
{
    private readonly IExcelFunctions _excelFunctions;
    private readonly IETGFactSymmetryConfig? _config;



    [ObservableProperty]
    private ObservableCollection<string> states;
    // public IEnumerable<ETGFactSymmetryViewModel> OC_ETGFactSymmetryViewModel => _oc_ETGFactSymmetryViewModel;



    [ObservableProperty]
    private bool isModalOpen;



    public MHPViewModel(IConfiguration config, IExcelFunctions excelFunctions)
    {
        _excelFunctions = excelFunctions;
        //_config = prepareConfig(config);
        IsModalOpen = false;

        populateFilters();


    }

    private async Task populateFilters()
    {
        WebAPIConsume.BaseURI = "https://localhost:7129";
        var response = WebAPIConsume.GetCall("/mhpstates");
        if (response.Result.StatusCode == System.Net.HttpStatusCode.OK)
        {
            var reponseStream = await response.Result.Content.ReadAsStreamAsync();
            var result = await JsonSerializer.DeserializeAsync<List<string>>(reponseStream, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });
            States = new ObservableCollection<string>();
            result.ForEach(x => { States.Add(x); });

            //States = result;
        }
    }


    bool disposed;
    protected virtual void Dispose(bool disposing)
    {
        if (!disposed)
        {
            if (disposing)
            {
                //dispose managed resources
            }
        }
        //dispose unmanaged resources
        disposed = true;
    }

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }
}
