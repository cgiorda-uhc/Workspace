using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using DocumentFormat.OpenXml.Spreadsheet;
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
    public ObservableCollection<string> _lOB;
    [ObservableProperty]
    public ObservableCollection<string> _region;
    [ObservableProperty]
    public ObservableCollection<string> _state;


 
    [ObservableProperty]
    public ObservableCollection<string> _product; //COMMERCIAL, NULL
    [ObservableProperty]
    public ObservableCollection<string> _cSProduct; //OP, PHYS
    [ObservableProperty]
    public ObservableCollection<string> _fundingType; //ASO, INSURED


    [ObservableProperty]
    public ObservableCollection<string> _legalEntity;//HP OP HP JV, MAMSI, NEIGHBORHOOD
    [ObservableProperty]
    public ObservableCollection<string> _source;//CIRRUS, OXFORD, TOPS/UNET
    [ObservableProperty]
    public ObservableCollection<string> _cSDualIndicator;
    [ObservableProperty]
    public ObservableCollection<string> _mRDualIndicator;

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


    private List<string> _selected_lobs;
    private List<string> _selected_states;
    private List<string> _selected_regions;
    private List<string> _selected_products;
    private List<string> _selected_cs_products;
    private List<string> _selected_funding_types;
    private List<string> _selected_legal_entitys;
    private List<string> _selected_sources;
    private List<string> _selected_cs_dual_indicators;
    private List<string> _selected_mr_dual_indicators;



    [RelayCommand]
    private void LOBChanged(object item)
    {
        cleanCurrentList(ref _selected_lobs, item);
        cleanCurrentFilters("LOB");
    }

    [RelayCommand]
    private void RegionChanged(object item)
    {
        cleanCurrentList(ref _selected_regions, item);
        cleanCurrentFilters("Region");
    }


    [RelayCommand]
    private void StateChanged(object item)
    {
        cleanCurrentList(ref _selected_states, item);
        cleanCurrentFilters("State");
    }


    [RelayCommand]
    private void ProductChanged(object item)
    {
        cleanCurrentList(ref _selected_products, item);
        cleanCurrentFilters("Product");
    }

    [RelayCommand]
    private void CSProductChanged(object item)
    {
        cleanCurrentList(ref _selected_cs_products, item);
        cleanCurrentFilters("CSProduct");
    }


    [RelayCommand]
    private void FundingTypeChanged(object item)
    {
        cleanCurrentList(ref _selected_funding_types, item);
        cleanCurrentFilters("FundingType");
    }

    [RelayCommand]
    private void LegalEntityChanged(object item)
    {
        cleanCurrentList(ref _selected_legal_entitys, item);
        cleanCurrentFilters("LegalEntity");
    }

    [RelayCommand]
    private void SourceChanged(object item)
    {
        cleanCurrentList(ref _selected_sources, item);
        cleanCurrentFilters("Source");
    }


    [RelayCommand]
    private void CSDualIndicatorChanged(object item)
    {
        cleanCurrentList(ref _selected_cs_dual_indicators, item);
        cleanCurrentFilters("CSDualIndicator");
    }

    [RelayCommand]
    private void MRDualIndicatorChanged(object item)
    {
        cleanCurrentList(ref _selected_mr_dual_indicators, item);
        cleanCurrentFilters("MRDualIndicator");
    }

    private void cleanCurrentList(ref List<string> lst, object item)
    {
        string strItem = item.ToString();
        if (lst == null)
            lst = new List<string>();

        if (strItem == "--All--")
        {
            lst.Clear();
        }
        else if (lst.Contains(strItem))
        {
            lst.Remove(strItem);
        }
        else
        {
            lst.Add(strItem);
        }


    }

    private void cleanCurrentFilters(string triggeredBy)
    {

        List<MM_FINAL_Model> tmp = _mM_Final_Filters;

        if (_selected_lobs != null)
            if (_selected_lobs.Count() > 0)
                tmp = tmp.Where(x => _selected_lobs.Contains(x.LOB)).ToList();

        if (_selected_states != null)
            if (_selected_states.Count() > 0)
                tmp = tmp.Where(x => _selected_states.Contains(x.mapping_state)).ToList();

        if (_selected_regions != null)
            if (_selected_regions.Count() > 0)
                tmp = tmp.Where(x => _selected_regions.Contains(x.REGION)).ToList();

        if (_selected_products != null)
            if (_selected_products.Count() > 0)
                tmp = tmp.Where(x => _selected_products.Contains(x.PRDCT_LVL_1_NM)).ToList();

        if (_selected_cs_products != null)
            if (_selected_cs_products.Count() > 0)
                tmp = tmp.Where(x => _selected_cs_products.Contains(x.CS_TADM_PRDCT_MAP)).ToList();

        if (_selected_funding_types != null)
            if (_selected_funding_types.Count() > 0)
                tmp = tmp.Where(x => _selected_funding_types.Contains(x.HLTH_PLN_FUND_DESC)).ToList();

        if (_selected_legal_entitys != null)
            if (_selected_legal_entitys.Count() > 0)
                tmp = tmp.Where(x => _selected_legal_entitys.Contains(x.HCE_LEG_ENTY_ROLLUP_DESC)).ToList();

        if (_selected_sources != null)
            if (_selected_sources.Count() > 0)
                tmp = tmp.Where(x => _selected_sources.Contains(x.SRC_SYS_GRP_DESC)).ToList();

        if (_selected_cs_dual_indicators != null)
            if (_selected_cs_dual_indicators.Count() > 0)
                tmp = tmp.Where(x => _selected_cs_dual_indicators.Contains(x.CS_DUAL_IND)).ToList();

        if (_selected_mr_dual_indicators != null)
            if (_selected_mr_dual_indicators.Count() > 0)
                tmp = tmp.Where(x => _selected_mr_dual_indicators.Contains(x.MR_DUAL_IND)).ToList();



        if(triggeredBy != "LOB")
        {
            LOB.Clear();
            foreach (string s in tmp.Select(x => x.LOB).Distinct().OrderBy(t => t).ToList())
            {
                this.LOB.Add(s);
            }
            this.LOB.Insert(0, "--All--");
        }
        

        if(triggeredBy != "Region")
        {
            Region.Clear();
            foreach (string s in tmp.Select(x => x.REGION).Distinct().OrderBy(t => t).ToList())
            {
                this.Region.Add(s);
            }
            this.Region.Insert(0, "--All--");
        }
        

        if(triggeredBy != "State")
        {
            State.Clear();
            foreach (string s in tmp.Select(x => x.mapping_state).Distinct().OrderBy(t => t).ToList())
            {
                this.State.Add(s);
            }
            this.State.Insert(0, "--All--");
        }
        

        if(triggeredBy != "Product")
        {
            Product.Clear();
            foreach (string s in tmp.Select(x => x.PRDCT_LVL_1_NM).Distinct().OrderBy(t => t).ToList())
            {
                this.Product.Add(s);
            }
            this.Product.Insert(0, "--All--");
        }
        

        if(triggeredBy != "CSProduct")
        {
            CSProduct.Clear();
            foreach (string s in tmp.Select(x => x.CS_TADM_PRDCT_MAP).Distinct().OrderBy(t => t).ToList())
            {
                this.CSProduct.Add(s);
            }
            this.CSProduct.Insert(0, "--All--");
        }
        

        if(triggeredBy != "FundingType")
        {
            FundingType.Clear();
            foreach (string s in tmp.Select(x => x.HLTH_PLN_FUND_DESC).Distinct().OrderBy(t => t).ToList())
            {
                this.FundingType.Add(s);
            }
            this.FundingType.Insert(0, "--All--");
        }
        

        if(triggeredBy != "LegalEntity")
        {
            LegalEntity.Clear();
            foreach (string s in tmp.Select(x => x.HCE_LEG_ENTY_ROLLUP_DESC).Distinct().OrderBy(t => t).ToList())
            {
                this.LegalEntity.Add(s);
            }
            this.LegalEntity.Insert(0, "--All--");
        }
        

        if(triggeredBy != "Source")
        {
            Source.Clear();
            foreach (string s in tmp.Select(x => x.SRC_SYS_GRP_DESC).Distinct().OrderBy(t => t).ToList())
            {
                this.Source.Add(s);
            }
            this.Source.Insert(0, "--All--");
        }
        

        if(triggeredBy != "CSDualIndicator")
        {
            CSDualIndicator.Clear();
            foreach (string s in tmp.Select(x => x.CS_DUAL_IND).Distinct().OrderBy(t => t).ToList())
            {
                this.CSDualIndicator.Add(s);
            }
            this.CSDualIndicator.Insert(0, "--All--");
        }
        

        if(triggeredBy != "MRDualIndicator")
        {
            MRDualIndicator.Clear();
            foreach (string s in tmp.Select(x => x.MR_DUAL_IND).Distinct().OrderBy(t => t).ToList())
            {
                this.MRDualIndicator.Add(s);
            }
            this.MRDualIndicator.Insert(0, "--All--");
        }
       



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



            LOB = new ObservableCollection<string>(_mM_Final_Filters.Select(x => x.LOB).Distinct().OrderBy(t => t).ToList() as List<string>);
            LOB.Insert(0, "--All--");


            Region = new ObservableCollection<string>(_mM_Final_Filters.Select(x => x.REGION).Distinct().OrderBy(t => t).ToList() as List<string>);
            Region.Insert(0, "--All--");

            State = new ObservableCollection<string>(_mM_Final_Filters.Select(x => x.mapping_state).Distinct().OrderBy(t => t).ToList() as List<string>);
            State.Insert(0, "--All--");

            Product = new ObservableCollection<string>(_mM_Final_Filters.Select(x => x.PRDCT_LVL_1_NM).Distinct().OrderBy(t => t).ToList() as List<string>);
            Product.Insert(0, "--All--");

            CSProduct = new ObservableCollection<string>(_mM_Final_Filters.Select(x => x.CS_TADM_PRDCT_MAP).Distinct().OrderBy(t => t).ToList() as List<string>);
            CSProduct.Insert(0, "--All--");

            FundingType = new ObservableCollection<string>(_mM_Final_Filters.Select(x => x.HLTH_PLN_FUND_DESC).Distinct().OrderBy(t => t).ToList() as List<string>);
            FundingType.Insert(0, "--All--");

            LegalEntity = new ObservableCollection<string>(_mM_Final_Filters.Select(x => x.HCE_LEG_ENTY_ROLLUP_DESC).Distinct().OrderBy(t => t).ToList() as List<string>);
            LegalEntity.Insert(0, "--All--");

            Source = new ObservableCollection<string>(_mM_Final_Filters.Select(x => x.SRC_SYS_GRP_DESC).Distinct().OrderBy(t => t).ToList() as List<string>);
            Source.Insert(0, "--All--");

            CSDualIndicator = new ObservableCollection<string>(_mM_Final_Filters.Select(x => x.CS_DUAL_IND).Distinct().OrderBy(t => t).ToList() as List<string>);
            CSDualIndicator.Insert(0, "--All--");

            MRDualIndicator = new ObservableCollection<string>(_mM_Final_Filters.Select(x => x.MR_DUAL_IND).Distinct().OrderBy(t => t).ToList() as List<string>);
            MRDualIndicator.Insert(0, "--All--");


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
