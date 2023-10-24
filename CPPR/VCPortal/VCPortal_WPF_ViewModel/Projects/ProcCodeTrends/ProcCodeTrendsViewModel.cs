using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using FileParsingLibrary.MSExcel;
using FileParsingLibrary.MSExcel.Custom.MHP;
using MathNet.Numerics;
using MathNet.Numerics.Providers.SparseSolver;
using Microsoft.Extensions.Configuration;
using SharedFunctionsLibrary;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Net.NetworkInformation;
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
using VCPortal_Models.Models.ProcCodeTrends;
using VCPortal_Models.Parameters.MHP;
using VCPortal_WPF_ViewModel.Projects.ChemotherapyPX;
using VCPortal_WPF_ViewModel.Projects.ETGFactSymmetry;
using VCPortal_WPF_ViewModel.Shared;

namespace VCPortal_WPF_ViewModel.Projects.ProcCodeTrends;
public partial class ProcCodeTrendsViewModel : ObservableObject
{
    private readonly IExcelFunctions _excelFunctions;
    private readonly IMHPUniverseConfig ? _config;
    private readonly Serilog.ILogger _logger;
    private StringBuilder _sbStatus;
    private List<MM_FINAL_Model> _mM_Final_Filters { get; set; }


    private readonly BackgroundWorker worker = new BackgroundWorker();

    [ObservableProperty]
    private string currentTitle;


    [ObservableProperty]
    private bool isModalOpen;

    [ObservableProperty]
    private bool canRunReport;


    public MessageViewModel ProgressMessageViewModel { get; }
    public MessageViewModel UserMessageViewModel { get; }

    [ObservableProperty]
    public List<string> _lOB;
    [ObservableProperty]
    public List<string> _region;
    [ObservableProperty]
    public List<string> _state;


 
    [ObservableProperty]
    public List<string> _product; //COMMERCIAL, NULL
    [ObservableProperty]
    public List<string> _cSProduct; //OP, PHYS
    [ObservableProperty]
    public List<string> _fundingType; //ASO, INSURED


    [ObservableProperty]
    public List<string> _legalEntity;//HP OP HP JV, MAMSI, NEIGHBORHOOD
    [ObservableProperty]
    public List<string> _source;//CIRRUS, OXFORD, TOPS/UNET
    [ObservableProperty]
    public List<string> _cSDualIndicator;
    [ObservableProperty]
    public List<string> _mRDualIndicator;

    public ProcCodeTrendsViewModel(IConfiguration config, IExcelFunctions excelFunctions, Serilog.ILogger logger)
    {
        _logger = logger;
        _excelFunctions = excelFunctions;
        _config = prepareConfig(config);

        UserMessageViewModel = new MessageViewModel();
        ProgressMessageViewModel = new MessageViewModel();

        worker.DoWork += worker_DoWork;
        worker.RunWorkerCompleted += worker_RunWorkerCompleted;


        CurrentTitle = "ProcCode Trending";


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
            _logger.Error($"No Config found for ProcCode Trends Reporting");
        }


    }

    private void worker_DoWork(object sender, DoWorkEventArgs e)
    {
        var callingFunction = (string)e.Argument;

        _sbStatus.Clear();
        UserMessageViewModel.Message = "";
        ProgressMessageViewModel.Message = "";
        ProgressMessageViewModel.HasMessage = true;

         GenerateReport();
       

    }
    private void worker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
    {
        //update ui once worker complete his work
        ProgressMessageViewModel.HasMessage = false;

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


    private List<string> _selected_regions;
    [RelayCommand]
    private void RegionChanged(object item)
    {
        string strItem = item.ToString();


        if (_selected_regions == null)
            _selected_regions = new List<string>();

        if (strItem == "--All--")
        {

            _selected_regions.Clear();
        }
        else if (_selected_regions.Contains(strItem))
        {
            _selected_regions.Remove(strItem);
        }
        else
        {
            _selected_regions.Add(strItem);
        }

        cleanGroups();
      

    }

    private List<string> _selected_markets;
    [RelayCommand]
    private void StateChanged(object item)
    {
        string strItem = item.ToString();


        if (_selected_markets == null)
            _selected_markets = new List<string>();

        if (strItem == "--All--")
        {

            _selected_markets.Clear();
        }
        else if (_selected_markets.Contains(strItem))
        {
            _selected_markets.Remove(strItem);
        }
        else
        {
            _selected_markets.Add(strItem);
        }

        cleanGroups();


    }


    private object _params;

    [RelayCommand]
    private async Task GenerateReportCall(object item)
    {
        _params = item;

        UserMessageViewModel.Message = "";
        Mouse.OverrideCursor = Cursors.Wait;
        await Task.Run(() => worker.RunWorkerAsync("GenerateReport"));
        Mouse.OverrideCursor = null;

    }

    

    private async Task GenerateReport()
    {

        _logger.Information("Running MHP.GenerateEIReport for {CurrentUser}...", Authentication.UserName);

        _sbStatus.Append("--Processing selected filters for EI" + Environment.NewLine);
        ProgressMessageViewModel.Message = _sbStatus.ToString();

        object[] parameters = _params as object[];

        var s = "";
        //MHP_EI_Parameters ei_param = new MHP_EI_Parameters();
        //MHP_EI_Parameters_All ei_param_all = new MHP_EI_Parameters_All();

        //List<MHP_EI_Model> mhp_final;
        //List<MHP_EI_Model> mhp_final_all;
        //List<MHPEIDetails_Model> mhp_details_final;
        //List<MHPEIDetails_Model> mhp_details_final_all;
        //try
        //{

        //    ei_param.State = "'" + String.Join(",", parameters[0].ToString().Replace("--All--,", "")).Replace(",", "', '") + "'";
        //    ei_param.StartDate = DateTime.Parse(parameters[1].ToString()).ToShortDateString();
        //    ei_param.EndDate = DateTime.Parse(parameters[2].ToString()).ToShortDateString();


        //    ei_param_all.State = "'" + String.Join(",", parameters[0].ToString().Replace("--All--,", "")).Replace(",", "', '") + "'";
        //    ei_param_all.StartDate = DateTime.Parse(parameters[1].ToString()).ToShortDateString();
        //    ei_param_all.EndDate = DateTime.Parse(parameters[2].ToString()).ToShortDateString();


        //    StringBuilder sbLE = new StringBuilder();

        //    var le = parameters[3].ToString().Replace("--All--~", "").Split('~');
        //    foreach (var e in le)
        //    {
        //        if (ei_param.LegalEntities == null)
        //        {
        //            ei_param.LegalEntities = new List<string>();
        //        }


        //        var val = e.ToString().Replace(" ", "").Split('-')[0];
        //        ei_param.LegalEntities.Add(val);
        //        sbLE.Append("'" + val + "',");
        //    }


        //    ei_param.Finc_Arng_Desc = "'" + String.Join(",", parameters[4].ToString().Replace("--All--,", "")).Replace(",", "', '") + "'";
        //    ei_param.Mkt_Seg_Rllp_Desc = "'" + String.Join(",", parameters[5].ToString().Replace("--All--,", "")).Replace(",", "', '") + "'";


        //    ei_param_all.LegalEntities = sbLE.ToString().TrimEnd(',');
        //    ei_param_all.Finc_Arng_Desc = "'" + String.Join(",", parameters[4].ToString().Replace("--All--,", "")).Replace(",", "', '") + "'";
        //    ei_param_all.Mkt_Seg_Rllp_Desc = "'" + String.Join(",", parameters[5].ToString().Replace("--All--,", "")).Replace(",", "', '") + "'";

        //    if (!string.IsNullOrEmpty(parameters[6] + ""))
        //    {
        //        ei_param.Mkt_Typ_Desc = "'" + String.Join(",", parameters[6].ToString().Replace("--All--,", "")).Replace(",", "', '") + "'";
        //        ei_param_all.Mkt_Typ_Desc = "'" + String.Join(",", parameters[6].ToString().Replace("--All--,", "")).Replace(",", "', '") + "'";
        //    }

        //    System.Collections.IList items = (System.Collections.IList)parameters[7];
        //    StringBuilder sb = new StringBuilder();
        //    foreach (var i in items)
        //    {
        //        sb.Append("'" + i.ToString().Split('-')[0].Trim() + "',");
        //    }
        //    if (sb.Length > 0)
        //    {
        //        ei_param.Cust_Seg = sb.ToString().TrimEnd(',');
        //        ei_param_all.Cust_Seg = sb.ToString().TrimEnd(',');
        //    }



        //    _sbStatus.Append("--Retreiving EI summary data from Database" + Environment.NewLine);
        //    ProgressMessageViewModel.Message = _sbStatus.ToString();
 
        //    var api = _config.APIS.Where(x => x.Name == "MHP_EI").FirstOrDefault();
        //    WebAPIConsume.BaseURI = api.BaseUrl;
        //    var response = await WebAPIConsume.PostCall<MHP_EI_Parameters>(api.Url, ei_param);
        //    if (response.StatusCode != System.Net.HttpStatusCode.OK)
        //    {

        //        UserMessageViewModel.IsError = true;
        //        UserMessageViewModel.Message = "An error was thrown. Please contact the system admin.";
        //        _logger.Error("MHP EI Report threw an error for {CurrentUser}" + response.StatusCode.ToString(), Authentication.UserName);
        //        return;
        //    }
        //    else
        //    {

        //        var reponseStream = await response.Content.ReadAsStreamAsync();
        //        var result = await JsonSerializer.DeserializeAsync<List<MHP_EI_Model>>(reponseStream, new JsonSerializerOptions
        //        {
        //            PropertyNameCaseInsensitive = true
        //        });

        //        mhp_final = result;

        //    }


        //    //EI ALL SUMMARY
        //    _sbStatus.Append("--Retreiving EI summary all data from Database" + Environment.NewLine);
        //    ProgressMessageViewModel.Message = _sbStatus.ToString();

        //    api = _config.APIS.Where(x => x.Name == "MHP_EI_All").FirstOrDefault();
        //    WebAPIConsume.BaseURI = api.BaseUrl;
        //    response = await WebAPIConsume.PostCall<MHP_EI_Parameters_All>(api.Url, ei_param_all);
        //    if (response.StatusCode != System.Net.HttpStatusCode.OK)
        //    {

        //        UserMessageViewModel.IsError = true;
        //        UserMessageViewModel.Message = "An error was thrown. Please contact the system admin.";
        //        _logger.Error("MHP EI All Report details threw an error for {CurrentUser}" + response.StatusCode.ToString(), Authentication.UserName);
        //        return;
        //    }
        //    else
        //    {

        //        var reponseStream = await response.Content.ReadAsStreamAsync();
        //        var result = await JsonSerializer.DeserializeAsync<List<MHP_EI_Model>>(reponseStream, new JsonSerializerOptions
        //        {
        //            PropertyNameCaseInsensitive = true
        //        });

        //        mhp_final_all = result;


        //    }





        //    _sbStatus.Append("--Retreiving EI details data from Database" + Environment.NewLine);
        //    ProgressMessageViewModel.Message = _sbStatus.ToString();

        //    api = _config.APIS.Where(x => x.Name == "MHP_EI_Details").FirstOrDefault();
        //    WebAPIConsume.BaseURI = api.BaseUrl;
        //    response = await WebAPIConsume.PostCall<MHP_EI_Parameters>(api.Url, ei_param);
        //    if (response.StatusCode != System.Net.HttpStatusCode.OK)
        //    {

        //        UserMessageViewModel.IsError = true;
        //        UserMessageViewModel.Message = "An error was thrown. Please contact the system admin.";
        //        _logger.Error("MHP EI Report details threw an error for {CurrentUser}" + response.StatusCode.ToString(), Authentication.UserName);
        //        return;
        //    }
        //    else
        //    {

        //        var reponseStream = await response.Content.ReadAsStreamAsync();
        //        var result = await JsonSerializer.DeserializeAsync<List<MHPEIDetails_Model>>(reponseStream, new JsonSerializerOptions
        //        {
        //            PropertyNameCaseInsensitive = true
        //        });

        //        mhp_details_final = result;


        //    }



        //    //NOT NEEDED!!!
        //    //_sbStatus.Append("--Retreiving All EI details data from Database" + Environment.NewLine);
        //    //ProgressMessageViewModel.Message = _sbStatus.ToString();

        //    //api = _config.APIS.Where(x => x.Name == "MHP_EI_Details_All").FirstOrDefault();
        //    //WebAPIConsume.BaseURI = api.BaseUrl;
        //    //response = await WebAPIConsume.PostCall<MHP_EI_Parameters_All>(api.Url, ei_param_all);
        //    //if (response.StatusCode != System.Net.HttpStatusCode.OK)
        //    //{

        //    //    UserMessageViewModel.IsError = true;
        //    //    UserMessageViewModel.Message = "An error was thrown. Please contact the system admin.";
        //    //    _logger.Error("MHP EI All Report details threw an error for {CurrentUser}" + response.StatusCode.ToString(), Authentication.UserName);
        //    //    return;
        //    //}
        //    //else
        //    //{

        //    //    var reponseStream = await response.Content.ReadAsStreamAsync();
        //    //    var result = await JsonSerializer.DeserializeAsync<List<MHPEIDetails_Model>>(reponseStream, new JsonSerializerOptions
        //    //    {
        //    //        PropertyNameCaseInsensitive = true
        //    //    });

        //    //    mhp_details_final_all = result;


        //    //}







        //    CancellationTokenSource cancellationToken;
        //    cancellationToken = new CancellationTokenSource();
        //    var bytes = await MHPExcelExport.ExportEIToExcel(mhp_final, mhp_final_all, mhp_details_final, () => ProgressMessageViewModel.Message, x => ProgressMessageViewModel.Message = x, cancellationToken.Token);

        //    var file = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + "\\MHP_Report_" + DateTime.Now.ToString("yyyy-dd-M--HH-mm-ss") + ".xlsx";


        //    _sbStatus.Append("--Saving Excel here: " + file + Environment.NewLine);
        //    ProgressMessageViewModel.Message = _sbStatus.ToString();

        //    if (File.Exists(file))
        //        File.Delete(file);

        //    await File.WriteAllBytesAsync(file, bytes);


        //    _sbStatus.Append("--Opening Excel" + Environment.NewLine);
        //    ProgressMessageViewModel.Message = _sbStatus.ToString();

        //    var p = new Process();
        //    p.StartInfo = new ProcessStartInfo(file)
        //    {
        //        UseShellExecute = true
        //    };
        //    p.Start();


        //    _sbStatus.Append("--Process completed!" + Environment.NewLine + Environment.NewLine + Environment.NewLine);
        //    _sbStatus.Append("--Ready" + Environment.NewLine);
        //    ProgressMessageViewModel.Message = _sbStatus.ToString();

        //    UserMessageViewModel.IsError = false;
        //    UserMessageViewModel.Message = "MHP EI Report sucessfully generated";
        //    _logger.Information("MHP EI Report sucessfully generated for {CurrentUser}...", Authentication.UserName);

        //}
        //catch (Exception ex)
        //{
        //    UserMessageViewModel.IsError = true;
        //    UserMessageViewModel.Message = "An error was thrown. Please contact the system admin.";
        //    _logger.Fatal(ex, "MHP EI Report threw an error for {CurrentUser}", Authentication.UserName);
        //}
        


    }

    

    private async Task populateFilters()
    {
        try
        {

            var api = _config.APIS.Where(x => x.Name == "PCT_MM_Final").FirstOrDefault();
            WebAPIConsume.BaseURI = api.BaseUrl;
            _sbStatus.Append("--Getting Cached Filters..." + Environment.NewLine);
            ProgressMessageViewModel.Message = _sbStatus.ToString();
            await Task.Delay(TimeSpan.FromSeconds(1));
            var response = WebAPIConsume.GetCall(api.Url);
            if (response.Result.StatusCode == System.Net.HttpStatusCode.OK)
            {
                var reponseStream = await response.Result.Content.ReadAsStreamAsync();
                var result = await JsonSerializer.DeserializeAsync<List<MM_FINAL_Model>>(reponseStream, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });

                _mM_Final_Filters = result;
            }
            else
            {
                UserMessageViewModel.IsError = true;
                UserMessageViewModel.Message = "An error was thrown. Please contact the system admin.";
                _logger.Error("populateFilters.MM_Final_Filters threw an error for {CurrentUser}" + response.Result.StatusCode.ToString(), Authentication.UserName);
            }



            //States = new List<string>(_mhpReportingFilters.Where(x=> x.Filter_Type == "State_of_Issue").GroupBy(s => s.Filter_Value).Select(g => g.First()).OrderBy(s => s.Filter_Value).Select(g => g.Filter_Value).ToList() as List<string>);
            //States.Insert(0, "--All--");


            LOB  = _mM_Final_Filters.Select(x => x.LOB).Distinct().OrderBy(t => t).ToList();
            LOB.Insert(0, "--All--");

            Region = _mM_Final_Filters.Select(x => x.REGION).Distinct().OrderBy(t => t).ToList();
            Region.Insert(0, "--All--");

            State = _mM_Final_Filters.Select(x => x.mapping_state).Distinct().OrderBy(t => t).ToList();
            State.Insert(0, "--All--");

            Product = _mM_Final_Filters.Select(x => x.PRDCT_LVL_1_NM).Distinct().OrderBy(t => t).ToList();
            Product.Insert(0, "--All--");

            CSProduct = _mM_Final_Filters.Select(x => x.CS_TADM_PRDCT_MAP).Distinct().OrderBy(t => t).ToList();
            CSProduct.Insert(0, "--All--");

            FundingType = _mM_Final_Filters.Select(x => x.HLTH_PLN_FUND_DESC).Distinct().OrderBy(t => t).ToList();
            FundingType.Insert(0, "--All--");

            LegalEntity = _mM_Final_Filters.Select(x => x.HCE_LEG_ENTY_ROLLUP_DESC).Distinct().OrderBy(t => t).ToList();
            LegalEntity.Insert(0, "--All--");

            Source = _mM_Final_Filters.Select(x => x.SRC_SYS_GRP_DESC).Distinct().OrderBy(t => t).ToList();
            Source.Insert(0, "--All--");

            CSDualIndicator = _mM_Final_Filters.Select(x => x.CS_DUAL_IND).Distinct().OrderBy(t => t).ToList();
            CSDualIndicator.Insert(0, "--All--");

            MRDualIndicator = _mM_Final_Filters.Select(x => x.MR_DUAL_IND).Distinct().OrderBy(t => t).ToList();
            MRDualIndicator.Insert(0, "--All--");





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

        //List<string> tmp;
        //this.GroupNumbers.Clear();

        //if (_selected_states.Count() > 0)
        //    tmp = _mhpGroupState.Where(x => _selected_states.Contains(x.State_of_Issue)).GroupBy(s => s.Group_Number).Select(g => g.First()).OrderBy(s => s.Group_Number).Select(g => g.Group_Number).ToList();
        //else
        //    tmp = _mhpGroupState.GroupBy(s => s.Group_Number).Select(g => g.First()).OrderBy(s => s.Group_Number).Select(g => g.Group_Number).ToList();

        //foreach (string s in tmp)
        //{
        //    this.GroupNumbers.Add(s);
        //}
        //this.GroupNumbers.Insert(0, "--All--");
    }


    private IMHPUniverseConfig prepareConfig(IConfiguration config)
    {

        var project = "PCT";
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
