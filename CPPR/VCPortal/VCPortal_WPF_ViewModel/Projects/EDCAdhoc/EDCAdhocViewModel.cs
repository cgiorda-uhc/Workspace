using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using FileParsingLibrary.MSExcel;
using FileParsingLibrary.MSExcel.Custom.MHP;
using Microsoft.Extensions.Configuration;
using SharedFunctionsLibrary;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Text.Json;
using System.Windows;
using System.Windows.Input;
using VCPortal_Models.Configuration.HeaderInterfaces.Abstract;
using VCPortal_Models.Configuration.HeaderInterfaces.Concrete;
using VCPortal_Models.Models.MHP;
using VCPortal_Models.Parameters.MHP;
using VCPortal_WPF_ViewModel.Shared;

namespace VCPortal_WPF_ViewModel.Projects.EDCAdhoc;
public partial class EDCAdhocViewModel : ObservableObject
{
    private readonly IExcelFunctions _excelFunctions;
    private readonly IMHPUniverseConfig? _config;
    private readonly Serilog.ILogger _logger;
    private StringBuilder _sbStatus;
    private List<MHP_Reporting_Filters> _mhpReportingFilters { get; set; }

    private List<MHP_Group_State_Model> _mhpGroupState { get; set; }

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
    public List<string> _states;


    [ObservableProperty]
    public List<string> _proc_Codes;

    [ObservableProperty]
    public List<string> id_Filter;


    public EDCAdhocViewModel(IConfiguration config, IExcelFunctions excelFunctions, Serilog.ILogger logger)
    {
        _logger = logger;
        _excelFunctions = excelFunctions;
        _config = prepareConfig(config);

        UserMessageViewModel = new MessageViewModel();
        ProgressMessageViewModel = new MessageViewModel();

        worker.DoWork += worker_DoWork;
        worker.RunWorkerCompleted += worker_RunWorkerCompleted;


        CurrentTitle = "EDC EI Adhoc Reporting";


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
            _logger.Error($"No Config found for EDC Adhoc Reporting");
        }


    }

    private void worker_DoWork(object sender, DoWorkEventArgs e)
    {
        var callingFunction = (string)e.Argument;

        _sbStatus.Clear();
        UserMessageViewModel.Message = "";
        ProgressMessageViewModel.Message = "";
        ProgressMessageViewModel.HasMessage = true;

        if (callingFunction == "GenerateReport")
        {
            ProgressMessageViewModel.HasMessage = true;
            GenerateReport();
        }


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



    private object _params;

    [RelayCommand]
    private async Task GenerateEIReportCall(object item)
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
        MHP_EI_Parameters ei_param = new MHP_EI_Parameters();
        MHP_EI_Parameters_All ei_param_all = new MHP_EI_Parameters_All();

        List<MHP_EI_Model> mhp_final;
        List<MHP_EI_Model> mhp_final_all;
        List<MHPEIDetails_Model> mhp_details_final;
        List<MHPEIDetails_Model> mhp_details_final_all;
        try
        {

            ei_param.State = "'" + String.Join(",", parameters[0].ToString().Replace("--All--,", "")).Replace(",", "', '") + "'";
            ei_param.StartDate = DateTime.Parse(parameters[1].ToString()).ToShortDateString();
            ei_param.EndDate = DateTime.Parse(parameters[2].ToString()).ToShortDateString();


            ei_param_all.State = "'" + String.Join(",", parameters[0].ToString().Replace("--All--,", "")).Replace(",", "', '") + "'";
            ei_param_all.StartDate = DateTime.Parse(parameters[1].ToString()).ToShortDateString();
            ei_param_all.EndDate = DateTime.Parse(parameters[2].ToString()).ToShortDateString();


            StringBuilder sbLE = new StringBuilder();

            var le = parameters[3].ToString().Replace("--All--~", "").Split('~');
            foreach (var e in le)
            {
                if (ei_param.LegalEntities == null)
                {
                    ei_param.LegalEntities = new List<string>();
                }


                var val = e.ToString().Replace(" ", "").Split('-')[0];
                ei_param.LegalEntities.Add(val);
                sbLE.Append("'" + val + "',");
            }


            ei_param.Finc_Arng_Desc = "'" + String.Join(",", parameters[4].ToString().Replace("--All--,", "")).Replace(",", "', '") + "'";
            ei_param.Mkt_Seg_Rllp_Desc = "'" + String.Join(",", parameters[5].ToString().Replace("--All--,", "")).Replace(",", "', '") + "'";


            ei_param_all.LegalEntities = sbLE.ToString().TrimEnd(',');
            ei_param_all.Finc_Arng_Desc = "'" + String.Join(",", parameters[4].ToString().Replace("--All--,", "")).Replace(",", "', '") + "'";
            ei_param_all.Mkt_Seg_Rllp_Desc = "'" + String.Join(",", parameters[5].ToString().Replace("--All--,", "")).Replace(",", "', '") + "'";

            if (!string.IsNullOrEmpty(parameters[6] + ""))
            {
                ei_param.Mkt_Typ_Desc = "'" + String.Join(",", parameters[6].ToString().Replace("--All--,", "")).Replace(",", "', '") + "'";
                ei_param_all.Mkt_Typ_Desc = "'" + String.Join(",", parameters[6].ToString().Replace("--All--,", "")).Replace(",", "', '") + "'";
            }

            System.Collections.IList items = (System.Collections.IList)parameters[7];
            StringBuilder sb = new StringBuilder();
            foreach (var i in items)
            {
                sb.Append("'" + i.ToString().Split('-')[0].Trim() + "',");
            }
            if (sb.Length > 0)
            {
                ei_param.Cust_Seg = sb.ToString().TrimEnd(',');
                ei_param_all.Cust_Seg = sb.ToString().TrimEnd(',');
            }



            _sbStatus.Append("--Retreiving EI summary data from Database" + Environment.NewLine);
            ProgressMessageViewModel.Message = _sbStatus.ToString();

            var api = _config.APIS.Where(x => x.Name == "MHP_EI").FirstOrDefault();
            WebAPIConsume.BaseURI = api.BaseUrl;
            var response = await WebAPIConsume.PostCall<MHP_EI_Parameters>(api.Url, ei_param);
            if (response.StatusCode != System.Net.HttpStatusCode.OK)
            {

                UserMessageViewModel.IsError = true;
                UserMessageViewModel.Message = "An error was thrown. Please contact the system admin.";
                _logger.Error("EDC Adhoc Report threw an error for {CurrentUser}" + response.StatusCode.ToString(), Authentication.UserName);
                return;
            }
            else
            {

                var reponseStream = await response.Content.ReadAsStreamAsync();
                var result = await JsonSerializer.DeserializeAsync<List<MHP_EI_Model>>(reponseStream, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });

                mhp_final = result;

            }


            //EI ALL SUMMARY
            _sbStatus.Append("--Retreiving EI summary all data from Database" + Environment.NewLine);
            ProgressMessageViewModel.Message = _sbStatus.ToString();

            api = _config.APIS.Where(x => x.Name == "MHP_EI_All").FirstOrDefault();
            WebAPIConsume.BaseURI = api.BaseUrl;
            response = await WebAPIConsume.PostCall<MHP_EI_Parameters_All>(api.Url, ei_param_all);
            if (response.StatusCode != System.Net.HttpStatusCode.OK)
            {

                UserMessageViewModel.IsError = true;
                UserMessageViewModel.Message = "An error was thrown. Please contact the system admin.";
                _logger.Error("EDC Adhoc All Report details threw an error for {CurrentUser}" + response.StatusCode.ToString(), Authentication.UserName);
                return;
            }
            else
            {

                var reponseStream = await response.Content.ReadAsStreamAsync();
                var result = await JsonSerializer.DeserializeAsync<List<MHP_EI_Model>>(reponseStream, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });

                mhp_final_all = result;


            }


            _sbStatus.Append("--Retreiving EI details data from Database" + Environment.NewLine);
            ProgressMessageViewModel.Message = _sbStatus.ToString();

            api = _config.APIS.Where(x => x.Name == "MHP_EI_Details").FirstOrDefault();
            WebAPIConsume.BaseURI = api.BaseUrl;
            response = await WebAPIConsume.PostCall<MHP_EI_Parameters>(api.Url, ei_param);
            if (response.StatusCode != System.Net.HttpStatusCode.OK)
            {

                UserMessageViewModel.IsError = true;
                UserMessageViewModel.Message = "An error was thrown. Please contact the system admin.";
                _logger.Error("EDC Adhoc Report details threw an error for {CurrentUser}" + response.StatusCode.ToString(), Authentication.UserName);
                return;
            }
            else
            {

                var reponseStream = await response.Content.ReadAsStreamAsync();
                var result = await JsonSerializer.DeserializeAsync<List<MHPEIDetails_Model>>(reponseStream, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });

                mhp_details_final = result;


            }


            CancellationTokenSource cancellationToken;
            cancellationToken = new CancellationTokenSource();
            var bytes = await MHPExcelExport.ExportEIToExcel(mhp_final, mhp_final_all, mhp_details_final, () => ProgressMessageViewModel.Message, x => ProgressMessageViewModel.Message = x, cancellationToken.Token);

            var file = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + "\\MHP_Report_" + DateTime.Now.ToString("yyyy-dd-M--HH-mm-ss") + ".xlsx";


            _sbStatus.Append("--Saving Excel here: " + file + Environment.NewLine);
            ProgressMessageViewModel.Message = _sbStatus.ToString();

            if (File.Exists(file))
                File.Delete(file);

            await File.WriteAllBytesAsync(file, bytes);


            _sbStatus.Append("--Opening Excel" + Environment.NewLine);
            ProgressMessageViewModel.Message = _sbStatus.ToString();

            var p = new Process();
            p.StartInfo = new ProcessStartInfo(file)
            {
                UseShellExecute = true
            };
            p.Start();


            _sbStatus.Append("--Process completed!" + Environment.NewLine + Environment.NewLine + Environment.NewLine);
            _sbStatus.Append("--Ready" + Environment.NewLine);
            ProgressMessageViewModel.Message = _sbStatus.ToString();

            UserMessageViewModel.IsError = false;
            UserMessageViewModel.Message = "EDC Adhoc Report sucessfully generated";
            _logger.Information("EDC Adhoc Report sucessfully generated for {CurrentUser}...", Authentication.UserName);

        }
        catch (Exception ex)
        {
            UserMessageViewModel.IsError = true;
            UserMessageViewModel.Message = "An error was thrown. Please contact the system admin.";
            _logger.Fatal(ex, "EDC Adhoc Report threw an error for {CurrentUser}", Authentication.UserName);
        }



    }

    

    [RelayCommand]
    private async Task EISectionCall()
    {
        CurrentTitle = "EDC EI Adhoc Reporting";
    }


    [RelayCommand]
    private async Task MRSectionCall()
    {
        CurrentTitle = "EDC MR Adhoc Reporting";
    }

    [RelayCommand]
    private async Task CSSectionCall()
    {
        CurrentTitle = "EDC CS Adhoc Reporting";

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

            var abbr = StatesArray.Abbreviations();
            States = new List<string>(abbr);
            States.Insert(0, "--All--");


            Proc_Codes = new List<string>();
            Proc_Codes.Add("99281");
            Proc_Codes.Add("99282");
            Proc_Codes.Add("99283");
            Proc_Codes.Add("99284");
            Proc_Codes.Add("99285");
            Proc_Codes.Add("G0380");
            Proc_Codes.Add("G0381");
            Proc_Codes.Add("G0382");
            Proc_Codes.Add("G0383");
            Proc_Codes.Add("G0384");
            Proc_Codes.Insert(0, "--All--");


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
