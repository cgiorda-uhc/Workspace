using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
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
using System.Windows;
using System.Windows.Input;
using VCPortal_Models.Configuration.HeaderInterfaces.Abstract;
using VCPortal_Models.Dtos.ETGFactSymmetry;
using VCPortal_WPF_ViewModel.Projects.ETGFactSymmetry;
using VCPortal_WPF_ViewModel.Shared;

namespace VCPortal_WPF_ViewModel.Projects.MHP;
public partial class MHPViewModel : ObservableObject
{
    private readonly IExcelFunctions _excelFunctions;
    private readonly IConfiguration? _config;
    private readonly Serilog.ILogger _logger;



    [ObservableProperty]
    private ObservableCollection<string> states;
    // public IEnumerable<ETGFactSymmetryViewModel> OC_ETGFactSymmetryViewModel => _oc_ETGFactSymmetryViewModel;


    [ObservableProperty]
    private string currentTitle;


    [ObservableProperty]
    private Visibility eIFormVisibility;
    [ObservableProperty]
    private Visibility cSFormVisibility;
    [ObservableProperty]
    private Visibility iFPFormVisibility;





    [ObservableProperty]
    private bool isModalOpen;

    public MessageViewModel UserMessageViewModel { get; }

    public MHPViewModel(IConfiguration config, IExcelFunctions excelFunctions, Serilog.ILogger logger)
    {
        _logger = logger;
        _excelFunctions = excelFunctions;
        _config = config;

        UserMessageViewModel = new MessageViewModel();

        CurrentTitle = "MHP EI Reporting";
        EIFormVisibility = Visibility.Visible;
        CSFormVisibility = Visibility.Hidden;
        IFPFormVisibility = Visibility.Hidden;





        populateFilters();


    }


    [RelayCommand]
    private async Task EISectionCall()
    {
        CurrentTitle = "MHP EI Reporting";
        EIFormVisibility = Visibility.Visible;
        CSFormVisibility = Visibility.Hidden;
        IFPFormVisibility = Visibility.Hidden;
    }


    [RelayCommand]
    private async Task IFPSectionCall()
    {
        CurrentTitle = "MHP IFP Reporting";
        EIFormVisibility = Visibility.Hidden;
        CSFormVisibility = Visibility.Hidden;
        IFPFormVisibility = Visibility.Visible;
    }

    [RelayCommand]
    private async Task CSSectionCall()
    {
        CurrentTitle = "MHP CS Reporting";
        EIFormVisibility =  Visibility.Hidden;
        CSFormVisibility = Visibility.Visible;
        IFPFormVisibility = Visibility.Hidden;
    }


    private async Task populateFilters()
    {
        //WebAPIConsume.BaseURI = "https://localhost:7129";
        //var response = WebAPIConsume.GetCall("/mhpstates");
        //if (response.Result.StatusCode == System.Net.HttpStatusCode.OK)
        //{
        //    var reponseStream = await response.Result.Content.ReadAsStreamAsync();
        //    var result = await JsonSerializer.DeserializeAsync<List<string>>(reponseStream, new JsonSerializerOptions
        //    {
        //        PropertyNameCaseInsensitive = true
        //    });
        //    States = new ObservableCollection<string>();
        //    result.ForEach(x => { States.Add(x); });

        //    //States = result;
        //}
    }


   
}
