using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using FileParsingLibrary.MSExcel;
using MathNet.Numerics.Providers.SparseSolver;
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
using System.Xml.Linq;
using VCPortal_Models.Configuration.HeaderInterfaces.Abstract;
using VCPortal_Models.Configuration.HeaderInterfaces.Concrete;
using VCPortal_Models.Dtos.ETGFactSymmetry;
using VCPortal_Models.Models.ChemoPx;
using VCPortal_Models.Parameters.MHP;
using VCPortal_WPF_ViewModel.Projects.ETGFactSymmetry;
using VCPortal_WPF_ViewModel.Shared;

namespace VCPortal_WPF_ViewModel.Projects.MHP;
public partial class MHPViewModel : ObservableObject
{
    private readonly IExcelFunctions _excelFunctions;
    private readonly IMHPUniverseConfig ? _config;
    private readonly Serilog.ILogger _logger;
    private StringBuilder _sbStatus;
    private List<MHP_Reporting_Filters> _mhpReportingFilters { get; set; }

    private List<MHP_Group_State_Model> _mhpGroupState { get; set; }


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

    public MessageViewModel ProgressMessageViewModel { get; }
    public MessageViewModel UserMessageViewModel { get; }


    public List<string> States { get; set; }
   
    public List<string> MKT_SEG_RLLP_DESC { get; set; }
  
    public List<string> FINC_ARNG_DESC { get; set; }
    
    public List<string> LEG_ENTY { get; set; }
  
    public List<string> CS_TADM_PRDCT_MAP { get; set; }
    public List<string> MKT_TYP_DESC { get; set; }
    public List<string> CUST_SEG { get; set; }

    public ObservableCollection<string> GroupNumbers { get; set; }

    public List<string> ProductCode { get; set; }


    public MHPViewModel(IConfiguration config, IExcelFunctions excelFunctions, Serilog.ILogger logger)
    {
        _logger = logger;
        _excelFunctions = excelFunctions;
        _config = prepareConfig(config);

        UserMessageViewModel = new MessageViewModel();
        ProgressMessageViewModel = new MessageViewModel();

        CurrentTitle = "MHP EI Reporting";
        EIFormVisibility = Visibility.Visible;
        CSFormVisibility = Visibility.Hidden;
        IFPFormVisibility = Visibility.Hidden;

        _sbStatus = new StringBuilder();



        if (_config != null)
        {
            //Task.Run(async () => await loadGridLists());
            //worker.RunWorkerAsync("InitialLoadData");
            InitialLoadData();

            //Task.Run(async () => await getChemotherapyPXData());
        }
        else
        {
            UserMessageViewModel.IsError = true;
            UserMessageViewModel.Message = "An error was thrown. Please contact the system admin.";
            _logger.Error($"No Config found for MHP Universe Reporting");
        }


    }


    private async void InitialLoadData()
    {
        _sbStatus.Clear();
        Mouse.OverrideCursor = Cursors.Wait;
        UserMessageViewModel.Message = "";
        ProgressMessageViewModel.Message = "";
        ProgressMessageViewModel.HasMessage = true;
        await populateFilters();
        Mouse.OverrideCursor = null;
        ProgressMessageViewModel.HasMessage = false;
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
        try
        {
            var api = _config.APIS.Where(x => x.Name == "MHP_Filters").FirstOrDefault();
            WebAPIConsume.BaseURI = api.BaseUrl;
            _sbStatus.Append("--Getting Cached Filters..." + Environment.NewLine);
            ProgressMessageViewModel.Message = _sbStatus.ToString();
            await Task.Delay(TimeSpan.FromSeconds(1));
            var response = WebAPIConsume.GetCall(api.Url);
            if (response.Result.StatusCode == System.Net.HttpStatusCode.OK)
            {
                var reponseStream = await response.Result.Content.ReadAsStreamAsync();
                var result = await JsonSerializer.DeserializeAsync<List<MHP_Reporting_Filters>>(reponseStream, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });

                _mhpReportingFilters = result;
            }
            else
            {
                UserMessageViewModel.IsError = true;
                UserMessageViewModel.Message = "An error was thrown. Please contact the system admin.";
                _logger.Error("populateFilters.MHP_Filters threw an error for {CurrentUser}" + response.Result.StatusCode.ToString(), Authentication.UserName);
            }



            api = _config.APIS.Where(x => x.Name == "MHP_GroupState").FirstOrDefault();
            WebAPIConsume.BaseURI = api.BaseUrl;
            _sbStatus.Append("--Getting Group/State Mapping..." + Environment.NewLine);
            ProgressMessageViewModel.Message = _sbStatus.ToString();
            await Task.Delay(TimeSpan.FromSeconds(1));
            response = WebAPIConsume.GetCall(api.Url);
            if (response.Result.StatusCode == System.Net.HttpStatusCode.OK)
            {
                var reponseStream = await response.Result.Content.ReadAsStreamAsync();
                var result = await JsonSerializer.DeserializeAsync<List<MHP_Group_State_Model>>(reponseStream, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });

                _mhpGroupState = result;
            }
            else
            {
                UserMessageViewModel.IsError = true;
                UserMessageViewModel.Message = "An error was thrown. Please contact the system admin.";
                _logger.Error("populateFilters.MHP_GroupState threw an error for {CurrentUser}" + response.Result.StatusCode.ToString(), Authentication.UserName);
            }


            States = new List<string>(_mhpReportingFilters.Where(x=> x.Filter_Type == "State_of_Issue").GroupBy(s => s.Filter_Value).Select(g => g.First()).OrderBy(s => s.Filter_Value).Select(g => g.Filter_Value).ToList() as List<string>);
            States.Insert(0, "--All--");

            MKT_SEG_RLLP_DESC = new List<string>(_mhpReportingFilters.Where(x=> x.Filter_Type == "MKT_SEG_RLLP_DESC").GroupBy(s => s.Filter_Value).Select(g => g.First()).OrderBy(s => s.Filter_Value).Select(g => g.Filter_Value).ToList() as List<string>);
            MKT_SEG_RLLP_DESC.Insert(0, "--All--");

            FINC_ARNG_DESC = new List<string>(_mhpReportingFilters.Where(x=> x.Filter_Type == "FINC_ARNG_DESC").GroupBy(s => s.Filter_Value).Select(g => g.First()).OrderBy(s => s.Filter_Value).Select(g => g.Filter_Value).ToList() as List<string>);
            FINC_ARNG_DESC.Insert(0, "--All--");

            LEG_ENTY = new List<string>(_mhpReportingFilters.Where(x=> x.Filter_Type == "LEG_ENTY").GroupBy(s => s.Filter_Value).Select(g => g.First()).OrderBy(s => s.Filter_Value).Select(g => g.Filter_Value).ToList() as List<string>);
            LEG_ENTY.Insert(0, "--All--");

            CS_TADM_PRDCT_MAP = new List<string>(_mhpReportingFilters.Where(x=> x.Filter_Type == "CS_TADM_PRDCT_MAP").GroupBy(s => s.Filter_Value).Select(g => g.First()).OrderBy(s => s.Filter_Value).Select(g => g.Filter_Value).ToList() as List<string>);
            CS_TADM_PRDCT_MAP.Insert(0, "--All--");

            MKT_TYP_DESC = new List<string>(_mhpReportingFilters.Where(x=> x.Filter_Type == "MKT_TYP_DESC").GroupBy(s => s.Filter_Value).Select(g => g.First()).OrderBy(s => s.Filter_Value).Select(g => g.Filter_Value).ToList() as List<string>);
            MKT_TYP_DESC.Insert(0, "--All--");

            CUST_SEG  = new List<string>(_mhpReportingFilters.Where(x=> x.Filter_Type == "CUST_SEG").GroupBy(s => s.Filter_Value).Select(g => g.First()).OrderBy(s => s.Filter_Value).Select(g => g.Filter_Value).ToList() as List<string>);
            CUST_SEG.Insert(0, "--All--");

            ProductCode = new List<string>(_mhpReportingFilters.Where(x=> x.Filter_Type == "PRDCT_CD").GroupBy(s => s.Filter_Value).Select(g => g.First()).OrderBy(s => s.Filter_Value).Select(g => g.Filter_Value).ToList() as List<string>);
            ProductCode.Insert(0, "--All--");

            GroupNumbers = new ObservableCollection<string>(_mhpGroupState.GroupBy(s => s.Group_Number).Select(g => g.First()).OrderBy(s => s.Group_Number).Select(g => g.Group_Number).ToList() as List<string>);
            GroupNumbers.Insert(0, "--All--");









        }
        catch (Exception ex)
        {
            UserMessageViewModel.IsError = true;
            UserMessageViewModel.Message = "An error was thrown. Please contact the system admin.";
            _logger.Fatal(ex, "populateFilters.WebAPIConsume.GetCall threw an error for {CurrentUser}", Authentication.UserName);
        }
    }

    private IMHPUniverseConfig prepareConfig(IConfiguration config)
    {

        var project = "MHP";
        var section = "Projects";

        ///EXTRACT IConfiguration INTO ETGFactSymmetryConfig 
        var cfg = config.GetSection(section).Get<List<MHPUniverseConfig>>();
        IMHPUniverseConfig mhp = new MHPUniverseConfig();
        if (cfg == null)
        {
            return null;
            //throw new OperationCanceledException();
        }
        mhp = cfg.Find(p => p.Name == project);
        if (mhp != null)
        {
            //Microsoft.Extensions.Configuration.Binder
            var e = config.GetSection(section + ":" + project + ":APIS").Get<APIConfig[]>();
            if (e != null)
            {
                mhp.APIS = e.ToList();
            }
        }
        return mhp;
    }
}
