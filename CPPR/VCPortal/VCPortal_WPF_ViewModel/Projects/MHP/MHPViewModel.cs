using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using FileParsingLibrary.MSExcel;
using MathNet.Numerics;
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
using VCPortal_Models.Dtos.ChemoPx;
using VCPortal_Models.Dtos.ETGFactSymmetry;
using VCPortal_Models.Models.ChemoPx;
using VCPortal_Models.Models.MHP;
using VCPortal_Models.Parameters.MHP;
using VCPortal_WPF_ViewModel.Projects.ChemotherapyPX;
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

    [ObservableProperty]
    private bool canRunReport;


    public MessageViewModel ProgressMessageViewModel { get; }
    public MessageViewModel UserMessageViewModel { get; }

    [ObservableProperty]
    public List<string> _states;
    [ObservableProperty]
    public List<string> _mKT_SEG_RLLP_DESC;
    [ObservableProperty]
    public List<string> _fINC_ARNG_DESC;
    [ObservableProperty]
    public List<string> _lEG_ENTY;
    [ObservableProperty]
    public List<string> _cS_TADM_PRDCT_MAP;
    [ObservableProperty]
    public List<string> _mKT_TYP_DESC;
    [ObservableProperty]
    public List<string> _cUST_SEG;

    [ObservableProperty]
    public ObservableCollection<string> _groupNumbers;
    [ObservableProperty]
    public List<string> _productCode;


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
        canRunReport = true;


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


    private List<string> _selected_states;
    [RelayCommand]
    private void StateChanged(object item)
    {
        string strItem = item.ToString();


        if (_selected_states == null)
            _selected_states = new List<string>();

        if (strItem == "--All--")
        {

            _selected_states.Clear();
        }
        else if (_selected_states.Contains(strItem))
        {
            _selected_states.Remove(strItem);
        }
        else
        {
            _selected_states.Add(strItem);
        }

        cleanGroups();
      

    }

    [RelayCommand]
    private async Task GenerateEIReport(object item)
    {
        object[] parameters = item as object[];

        MHP_EI_Parameters ei_param = new MHP_EI_Parameters();

        ei_param.State = parameters[0].ToString().Replace("--All--,", "");
        ei_param.StartDate = DateTime.Parse(parameters[1].ToString()).ToShortDateString();
        ei_param.EndDate = DateTime.Parse(parameters[2].ToString()).ToShortDateString();

        var le = parameters[3].ToString().Replace("--All--~", "").Split('~');
        foreach (var e in le)
        {
            if (ei_param.LegalEntities == null)
            {
                ei_param.LegalEntities = new List<string>();
            }
            ei_param.LegalEntities.Add(e.ToString().Replace(" ", "").Split('-')[0]);
        }

        ei_param.Finc_Arng_Desc = parameters[4].ToString().Replace("--All--,", "");
        ei_param.Mkt_Seg_Rllp_Desc = parameters[5].ToString().Replace("--All--,", "");
        ei_param.Mkt_Typ_Desc = parameters[6].ToString().Replace("--All--,", "");
        // ei_param.Cust_Seg = (string.IsNullOrEmpty(parameters[7].ToString()) ? null : new List<string>(parameters[7]));
        System.Collections.IList items = (System.Collections.IList)parameters[7];
        StringBuilder sb = new StringBuilder();
        foreach (var i in items)
        {
            sb.Append(i.ToString().Split('-')[0].Trim() + ",");
        }
        if(sb.Length > 0)
        {
            ei_param.Cust_Seg = sb.ToString().TrimEnd(',');
        }

        List<MHP_EI_Model> mhp_final;
        try
        {

            var api = _config.APIS.Where(x => x.Name == "MHP_EI").FirstOrDefault();
            WebAPIConsume.BaseURI = api.BaseUrl;


            //var url = api.Url + "?" + "State={0}&StartDate={1}&EndDate={2}&Finc_Arng_Desc={3}&Mkt_Seg_Rllp_Desc={4}&LegalEntities={5}&Mkt_Typ_Desc={6}&Cust_Seg={7}";
            //var response = WebAPIConsume.GetCall<MHP_EI_Parameters>(url, ei_param);

            var response = WebAPIConsume.PostCall<MHP_EI_Parameters>(api.Url, ei_param);


            if (response.Result.StatusCode != System.Net.HttpStatusCode.OK)
            {

                UserMessageViewModel.IsError = true;
                UserMessageViewModel.Message = "An error was thrown. Please contact the system admin.";
                _logger.Error("MHP.EI Report threw an error for {CurrentUser}" + response.Result.StatusCode.ToString(), Authentication.UserName);
            }
            else
            {

                var reponseStream = await response.Result.Content.ReadAsStreamAsync();
                var result = await JsonSerializer.DeserializeAsync<List<MHP_EI_Model>>(reponseStream, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });

                mhp_final = result;



                UserMessageViewModel.IsError = false;
                UserMessageViewModel.Message = "MHP.EI Report sucessfully generated";
                _logger.Information("MHP.EI Report sucessfully generated for {CurrentUser}...", Authentication.UserName);
            }

        }
        catch (Exception ex)
        {
            UserMessageViewModel.IsError = true;
            UserMessageViewModel.Message = "An error was thrown. Please contact the system admin.";
            _logger.Fatal(ex, "ChemotherapyPXData.save threw an error for {CurrentUser}", Authentication.UserName);
        }




    }
    [RelayCommand]
    private void GenerateCSReport(object item)
    {
        object[] parameters = item as object[];

        MHP_CS_Parameters cs_param = new MHP_CS_Parameters();

        cs_param.State = (string.IsNullOrEmpty(parameters[0].ToString()) ? null : new List<string>(parameters[0].ToString().Split(',')));
        cs_param.StartDate = DateTime.Parse(parameters[1].ToString()).ToShortDateString();
        cs_param.EndDate = DateTime.Parse(parameters[2].ToString()).ToShortDateString();
        cs_param.CS_Tadm_Prdct_Map = (string.IsNullOrEmpty(parameters[4].ToString()) ? null : new List<string>(parameters[4].ToString().Replace("--All--", "").Split(',')));
        cs_param.GroupNumbers = (string.IsNullOrEmpty(parameters[5].ToString()) ? null : new List<string>(parameters[5].ToString().Replace("--All--", "").Split(',')));
        

    }
    [RelayCommand]
    private void GenerateIFPReport(object item)
    {
        object[] parameters = item as object[];

        MHP_IFP_Parameters ifp_param = new MHP_IFP_Parameters();

        ifp_param.State = (string.IsNullOrEmpty(parameters[0].ToString()) ? null : new List<string>(parameters[0].ToString().Split(',')));
        ifp_param.StartDate = DateTime.Parse(parameters[1].ToString()).ToShortDateString();
        ifp_param.EndDate = DateTime.Parse(parameters[2].ToString()).ToShortDateString();
        ifp_param.ProductCodes = (string.IsNullOrEmpty(parameters[3].ToString()) ? null : new List<string>(parameters[3].ToString().Replace("--All--", "").Split(',')));

        
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

    private void cleanGroups()
    {

        List<string> tmp;
        this.GroupNumbers.Clear();

        if (_selected_states.Count() > 0)
            tmp = _mhpGroupState.Where(x => _selected_states.Contains(x.State_of_Issue)).GroupBy(s => s.Group_Number).Select(g => g.First()).OrderBy(s => s.Group_Number).Select(g => g.Group_Number).ToList();
        else
            tmp = _mhpGroupState.GroupBy(s => s.Group_Number).Select(g => g.First()).OrderBy(s => s.Group_Number).Select(g => g.Group_Number).ToList();

        foreach (string s in tmp)
        {
            this.GroupNumbers.Add(s);
        }
        this.GroupNumbers.Insert(0, "--All--");
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
