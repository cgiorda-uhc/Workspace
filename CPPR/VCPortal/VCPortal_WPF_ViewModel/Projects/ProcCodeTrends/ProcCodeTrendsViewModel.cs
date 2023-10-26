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
using VCPortal_Models.Parameters.ProcCodeTrends;
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
    private List<string> _selected_regions;

    [RelayCommand]
    private void LOBChanged(object item)
    {
        if (item.ToString().Equals("--All--") && _selected_lobs.FirstOrDefault(x => x.Contains("--All--")) == null)
        {
            _selected_lobs = _mM_Final_Filters.Select(x => x.LOB).Distinct().OrderBy(t => t).ToList();
            _selected_lobs.Insert(0, "--All--");
        }
        else
            cleanCurrentList(ref _selected_lobs, item);

        cleanCurrentFilters("LOB");
    }

    [RelayCommand]
    private void RegionChanged(object item)
    {
        if (item.ToString().Equals("--All--") && _selected_regions.FirstOrDefault(x => x.Contains("--All--")) == null)
        {
            _selected_regions = _mM_Final_Filters.Select(x => x.REGION).Distinct().OrderBy(t => t).ToList();
            _selected_regions.Insert(0, "--All--");
        }
        else
            cleanCurrentList(ref _selected_regions, item);

        cleanCurrentFilters("Region");
    }


    private void cleanCurrentList(ref List<string> lst, object item)
    {
        var strItem = item.ToString();

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

        if(triggeredBy == "Region")
        {
            if (_selected_regions != null)
                if (_selected_regions.Count() > 0)
                    tmp = tmp.Where(x => _selected_regions.Contains(x.REGION)).ToList();


            State.Clear();
            foreach (string s in tmp.Select(x => x.mapping_state).Distinct().OrderBy(t => t).ToList())
            {
                this.State.Add(s);
            }
            this.State.Insert(0, "--All--");
        }
        else if (triggeredBy == "LOB")
        {

            if (_selected_lobs != null)
                if (_selected_lobs.Count() > 0)
                    tmp = tmp.Where(x => _selected_lobs.Contains(x.LOB)).ToList();


            Product.Clear();
            foreach (string s in tmp.Select(x => x.PRDCT_LVL_1_NM).Distinct().OrderBy(t => t).ToList())
            {
                this.Product.Add(s);
            }
            this.Product.Insert(0, "--All--");


            CSProduct.Clear();
            foreach (string s in tmp.Select(x => x.CS_TADM_PRDCT_MAP).Distinct().OrderBy(t => t).ToList())
            {
                this.CSProduct.Add(s);
            }
            this.CSProduct.Insert(0, "--All--");

            FundingType.Clear();
            foreach (string s in tmp.Select(x => x.HLTH_PLN_FUND_DESC).Distinct().OrderBy(t => t).ToList())
            {
                this.FundingType.Add(s);
            }
            this.FundingType.Insert(0, "--All--");

            LegalEntity.Clear();
            foreach (string s in tmp.Select(x => x.HCE_LEG_ENTY_ROLLUP_DESC).Distinct().OrderBy(t => t).ToList())
            {
                this.LegalEntity.Add(s);
            }
            this.LegalEntity.Insert(0, "--All--");


            Source.Clear();
            foreach (string s in tmp.Select(x => x.SRC_SYS_GRP_DESC).Distinct().OrderBy(t => t).ToList())
            {
                this.Source.Add(s);
            }
            this.Source.Insert(0, "--All--");


            CSDualIndicator.Clear();
            foreach (string s in tmp.Select(x => x.CS_DUAL_IND).Distinct().OrderBy(t => t).ToList())
            {
                this.CSDualIndicator.Add(s);
            }
            this.CSDualIndicator.Insert(0, "--All--");

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

        _logger.Information("Running ProcCodeTrends.GenerateReport for {CurrentUser}...", Authentication.UserName);

        _sbStatus.Append("--Processing selected filters for ProcCodeTrends" + Environment.NewLine);
        ProgressMessageViewModel.Message = _sbStatus.ToString();

        object[] parameters = _params as object[];


        ProcCodeTrends_Parameters pc_param = new ProcCodeTrends_Parameters();

        try
        {
            if (!string.IsNullOrEmpty(parameters[0] + ""))
            {
                pc_param.LOB = "'" + String.Join(",", parameters[0].ToString().Replace("--All--,", "")).Replace(",", "', '") + "'";
            }

            if (!string.IsNullOrEmpty(parameters[1] + ""))
            {
                pc_param.Region = "'" + String.Join(",", parameters[1].ToString().Replace("--All--,", "")).Replace(",", "', '") + "'";
            }

            if (!string.IsNullOrEmpty(parameters[2] + ""))
            {
                pc_param.State = "'" + String.Join(",", parameters[2].ToString().Replace("--All--,", "")).Replace(",", "', '") + "'";
            }

            if (!string.IsNullOrEmpty(parameters[3] + ""))
            {
                pc_param.Product = "'" + String.Join(",", parameters[3].ToString().Replace("--All--,", "")).Replace(",", "', '") + "'";
            }

            if (!string.IsNullOrEmpty(parameters[4] + ""))
            {
                pc_param.CSProduct = "'" + String.Join(",", parameters[4].ToString().Replace("--All--,", "")).Replace(",", "', '") + "'";
            }

            if (!string.IsNullOrEmpty(parameters[5] + ""))
            {
                pc_param.FundingType = "'" + String.Join(",", parameters[5].ToString().Replace("--All--,", "")).Replace(",", "', '") + "'";
            }

            if (!string.IsNullOrEmpty(parameters[6] + ""))
            {
                pc_param.LegalEntity = "'" + String.Join(",", parameters[6].ToString().Replace("--All--,", "")).Replace(",", "', '") + "'";
            }


            if (!string.IsNullOrEmpty(parameters[7] + ""))
            {
                pc_param.Source = "'" + String.Join(",", parameters[7].ToString().Replace("--All--,", "")).Replace(",", "', '") + "'";
            }



            if (!string.IsNullOrEmpty(parameters[8] + ""))
            {
                pc_param.CSDualIndicator = "'" + String.Join(",", parameters[8].ToString().Replace("--All--,", "")).Replace(",", "', '") + "'";
            }



            if (!string.IsNullOrEmpty(parameters[9] + ""))
            {
                pc_param.MRDualIndicator = "'" + String.Join(",", parameters[9].ToString().Replace("--All--,", "")).Replace(",", "', '") + "'";
            }






            _sbStatus.Append("--Retreiving ProcCodeTrends claims phys data from Database" + Environment.NewLine);
            ProgressMessageViewModel.Message = _sbStatus.ToString();
            List<CLM_PHYS_Model> clm_phys_list;
            var api = _config.APIS.Where(x => x.Name == "PCT_Clm_Phys").FirstOrDefault();
            WebAPIConsume.BaseURI = api.BaseUrl;
            var response = await WebAPIConsume.PostCall<ProcCodeTrends_Parameters>(api.Url, pc_param);
            if (response.StatusCode != System.Net.HttpStatusCode.OK)
            {

                UserMessageViewModel.IsError = true;
                UserMessageViewModel.Message = "An error was thrown. Please contact the system admin.";
                _logger.Error("Clm_Phys_Phys Report threw an error for {CurrentUser}" + response.StatusCode.ToString(), Authentication.UserName);
                return;
            }
            else
            {

                var reponseStream = await response.Content.ReadAsStreamAsync();
                var result = await JsonSerializer.DeserializeAsync<List<CLM_PHYS_Model>>(reponseStream, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });

                clm_phys_list = result;

            }





            _sbStatus.Append("--Retreiving ProcCodeTrends claims op data from Database" + Environment.NewLine);
            ProgressMessageViewModel.Message = _sbStatus.ToString();
            List<CLM_OP_Model> clm_op_list;
            api = _config.APIS.Where(x => x.Name == "PCT_Clm_Op").FirstOrDefault();
            WebAPIConsume.BaseURI = api.BaseUrl;
            response = await WebAPIConsume.PostCall<ProcCodeTrends_Parameters>(api.Url, pc_param);
            if (response.StatusCode != System.Net.HttpStatusCode.OK)
            {

                UserMessageViewModel.IsError = true;
                UserMessageViewModel.Message = "An error was thrown. Please contact the system admin.";
                _logger.Error("Clm_Phys_Op Report threw an error for {CurrentUser}" + response.StatusCode.ToString(), Authentication.UserName);
                return;
            }
            else
            {

                var reponseStream = await response.Content.ReadAsStreamAsync();
                var result = await JsonSerializer.DeserializeAsync<List<CLM_OP_Model>>(reponseStream, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });

                clm_op_list = result;

            }



            var p = clm_phys_list;
            var o = clm_op_list;
            var s = "";

            ////EI ALL SUMMARY
            //_sbStatus.Append("--Retreiving EI summary all data from Database" + Environment.NewLine);
            //ProgressMessageViewModel.Message = _sbStatus.ToString();

            //api = _config.APIS.Where(x => x.Name == "MHP_EI_All").FirstOrDefault();
            //WebAPIConsume.BaseURI = api.BaseUrl;
            //response = await WebAPIConsume.PostCall<MHP_EI_Parameters_All>(api.Url, ei_param_all);
            //if (response.StatusCode != System.Net.HttpStatusCode.OK)
            //{

            //    UserMessageViewModel.IsError = true;
            //    UserMessageViewModel.Message = "An error was thrown. Please contact the system admin.";
            //    _logger.Error("MHP EI All Report details threw an error for {CurrentUser}" + response.StatusCode.ToString(), Authentication.UserName);
            //    return;
            //}
            //else
            //{

            //    var reponseStream = await response.Content.ReadAsStreamAsync();
            //    var result = await JsonSerializer.DeserializeAsync<List<MHP_EI_Model>>(reponseStream, new JsonSerializerOptions
            //    {
            //        PropertyNameCaseInsensitive = true
            //    });

            //    mhp_final_all = result;


            //}





            //_sbStatus.Append("--Retreiving EI details data from Database" + Environment.NewLine);
            //ProgressMessageViewModel.Message = _sbStatus.ToString();

            //api = _config.APIS.Where(x => x.Name == "MHP_EI_Details").FirstOrDefault();
            //WebAPIConsume.BaseURI = api.BaseUrl;
            //response = await WebAPIConsume.PostCall<MHP_EI_Parameters>(api.Url, ei_param);
            //if (response.StatusCode != System.Net.HttpStatusCode.OK)
            //{

            //    UserMessageViewModel.IsError = true;
            //    UserMessageViewModel.Message = "An error was thrown. Please contact the system admin.";
            //    _logger.Error("MHP EI Report details threw an error for {CurrentUser}" + response.StatusCode.ToString(), Authentication.UserName);
            //    return;
            //}
            //else
            //{

            //    var reponseStream = await response.Content.ReadAsStreamAsync();
            //    var result = await JsonSerializer.DeserializeAsync<List<MHPEIDetails_Model>>(reponseStream, new JsonSerializerOptions
            //    {
            //        PropertyNameCaseInsensitive = true
            //    });

            //    mhp_details_final = result;


            //}

            //CancellationTokenSource cancellationToken;
            //cancellationToken = new CancellationTokenSource();
            //var bytes = await MHPExcelExport.ExportEIToExcel(mhp_final, mhp_final_all, mhp_details_final, () => ProgressMessageViewModel.Message, x => ProgressMessageViewModel.Message = x, cancellationToken.Token);

            //var file = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + "\\MHP_Report_" + DateTime.Now.ToString("yyyy-dd-M--HH-mm-ss") + ".xlsx";


            //_sbStatus.Append("--Saving Excel here: " + file + Environment.NewLine);
            //ProgressMessageViewModel.Message = _sbStatus.ToString();

            //if (File.Exists(file))
            //    File.Delete(file);

            //await File.WriteAllBytesAsync(file, bytes);


            //_sbStatus.Append("--Opening Excel" + Environment.NewLine);
            //ProgressMessageViewModel.Message = _sbStatus.ToString();

            //var p = new Process();
            //p.StartInfo = new ProcessStartInfo(file)
            //{
            //    UseShellExecute = true
            //};
            //p.Start();


            //_sbStatus.Append("--Process completed!" + Environment.NewLine + Environment.NewLine + Environment.NewLine);
            //_sbStatus.Append("--Ready" + Environment.NewLine);
            //ProgressMessageViewModel.Message = _sbStatus.ToString();

            //UserMessageViewModel.IsError = false;
            //UserMessageViewModel.Message = "MHP EI Report sucessfully generated";
            //_logger.Information("MHP EI Report sucessfully generated for {CurrentUser}...", Authentication.UserName);

        }
        catch (Exception ex)
        {
            UserMessageViewModel.IsError = true;
            UserMessageViewModel.Message = "An error was thrown. Please contact the system admin.";
            _logger.Fatal(ex, "ProcCodeTrends Report threw an error for {CurrentUser}", Authentication.UserName);
        }



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



            LOB = new List<string>(_mM_Final_Filters.Select(x => x.LOB).Distinct().OrderBy(t => t).ToList() as List<string>);
            LOB.Insert(0, "--All--");


            Region = new List<string>(_mM_Final_Filters.Select(x => x.REGION).Distinct().OrderBy(t => t).ToList() as List<string>);
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


            _selected_lobs = new List<string>();
            _selected_regions = new List<string>();

            //_selected_lobs = LOB.Where(x=> x != "--All--").ToList();

            //_selected_regions = Region.Where(x => x != "--All--").ToList();


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
