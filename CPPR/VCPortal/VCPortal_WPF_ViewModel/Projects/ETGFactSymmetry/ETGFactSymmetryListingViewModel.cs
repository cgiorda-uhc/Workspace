
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using DocumentFormat.OpenXml.ExtendedProperties;
using FileParsingLibrary.Models;
using FileParsingLibrary.MSExcel;
using MathNet.Numerics.Providers.SparseSolver;
using Microsoft.Extensions.Configuration;
using SharedFunctionsLibrary;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Net.NetworkInformation;
using System.Runtime.Intrinsics.X86;
using System.Text;
using System.Text.Json;
using VCPortal_Models.Configuration.HeaderInterfaces.Abstract;
using VCPortal_Models.Configuration.HeaderInterfaces.Concrete;
using VCPortal_Models.Dtos.ChemoPx;
using VCPortal_Models.Dtos.ETGFactSymmetry;
using VCPortal_Models.Models.ETGFactSymmetry;
using VCPortal_WPF_ViewModel.Projects.ChemotherapyPX;
using VCPortal_WPF_ViewModel.Shared;


namespace VCPortal_WPF_ViewModel.Projects.ETGFactSymmetry;

public partial class ETGFactSymmetryListingViewModel : ObservableObject, ViewModelBase
{
    private readonly IExcelFunctions _excelFunctions;
    private readonly IETGFactSymmetryConfig? _config;
    private readonly Serilog.ILogger _logger;

    public MessageViewModel ErrorMessageViewModel { get; }
    public MessageViewModel StatusMessageViewModel { get; }

    [ObservableProperty]
    private ObservableCollection<ETGFactSymmetryViewModel> oC_ETGFactSymmetryViewModel;

    private readonly BackgroundWorker worker = new BackgroundWorker();

    //[ObservableProperty]
    //private bool isModalOpen;

    [ObservableProperty]
    private string selectedRow;


    //[ObservableProperty]
    //private string status;


    [ObservableProperty]
    private List<string> lobOptions;
    [ObservableProperty]
    private List<string> rxNrxOptions;
    [ObservableProperty]
    private List<string> treatmentIndicatorOptions;
    [ObservableProperty]
    private List<string> attributionOptions;
    [ObservableProperty]
    private List<string> treatmentIndicatorECOptions;
    [ObservableProperty]
    private List<string> mappingOptions;
    [ObservableProperty]
    private List<string> patientCentricMappingOptions;

    private StringBuilder _sbStatus;
    public ETGFactSymmetryListingViewModel(IConfiguration config, IExcelFunctions excelFunctions, Serilog.ILogger logger)
    {
        _logger = logger;
        _excelFunctions = excelFunctions;
        _config = prepareConfig(config);


        ErrorMessageViewModel = new MessageViewModel();
        StatusMessageViewModel = new MessageViewModel();

        SharedETGSymmObjects.ETGFactSymmetry_Tracking_List = new List<ETGFactSymmetry_Tracking_UpdateDto>();

        worker.DoWork += worker_DoWork;
        worker.RunWorkerCompleted += worker_RunWorkerCompleted;
 

        OC_ETGFactSymmetryViewModel = new ObservableCollection<ETGFactSymmetryViewModel>();

        if (_config != null)
        {
            worker.RunWorkerAsync("InitialLoadData");
            //loadGridLists();
            //Task.Run(async () => await getETGFactSymmetryData());
        }
        else
        {
            ErrorMessageViewModel.Message = "An error was thrown. Please contact the system admin.";
            _logger.Error($"No Config found for ETGFactSymmetry");
        }

    }
    private void worker_DoWork(object sender, DoWorkEventArgs e)
    {
        var callingFunction = (string)e.Argument;

        _sbStatus = new StringBuilder();
        ErrorMessageViewModel.Message = "";
        StatusMessageViewModel.Message = "";


        if (callingFunction == "ExportConfigs")
        {
            StatusMessageViewModel.HasMessage = true;
            exportConfigs();
        }
        else if (callingFunction == "LoadData")
        {
            StatusMessageViewModel.HasMessage = true;
            getETGFactSymmetryData();

        }
        else if (callingFunction == "InitialLoadData")
        {
            StatusMessageViewModel.HasMessage = true;
            loadGridLists();
            getETGFactSymmetryData();

        }

    }

    private void worker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
    {
        //update ui once worker complete his work
        StatusMessageViewModel.HasMessage = false;

    }



    [RelayCommand]
    private async Task GetETGFactSymmetryDataCall()
    {
        //StatusMessageViewModel.HasMessage = true;
        ////worker.RunWorkerAsync("GetETGFactSymmetryData");
        //getETGFactSymmetryData();
        //StatusMessageViewModel.HasMessage = false;
        StatusMessageViewModel.HasMessage = true;
        worker.RunWorkerAsync("LoadData");


    }

    [RelayCommand]
    private async Task ExportConfigsCall()
    {
        StatusMessageViewModel.HasMessage = true;
        worker.RunWorkerAsync("ExportConfigs");
    }
 
    [RelayCommand]
    private void save()
    {
        try
        {
            _logger.Information("Running ETGFactSymmetryData.Save for {CurrentUser}...", Authentication.UserName);

            var tracked = SharedETGSymmObjects.ETGFactSymmetry_Tracking_List;
            var date = DateTime.Now;
            foreach (var t in tracked)
            {
                t.update_date = date;
                t.username = "cgiorda";
            }

            var api = _config.APIS.Where(x => x.Name == "ETGInsert").FirstOrDefault();
            WebAPIConsume.BaseURI = api.BaseUrl;
            var response = WebAPIConsume.PostCall<List<ETGFactSymmetry_Tracking_UpdateDto>>(api.Url, tracked);
            if (response.Result.StatusCode != System.Net.HttpStatusCode.OK)
            {
                ErrorMessageViewModel.Message = "An error was thrown. Please contact the system admin.";
                _logger.Error("ETGFactSymmetryData.Save threw an error for {CurrentUser}" + response.Result.StatusCode.ToString(), Authentication.UserName);
            }

            SharedETGSymmObjects.ETGFactSymmetry_Tracking_List.Clear();

            _logger.Information("ETGFactSymmetryData.Save sucessfully completed for {CurrentUser}...", Authentication.UserName);


        }
        catch (Exception ex)
        {
            ErrorMessageViewModel.Message = "An error was thrown. Please contact the system admin.";
            _logger.Fatal(ex, "ETGFactSymmetryData.Save threw an error for {CurrentUser}", Authentication.UserName);
        }
    }


    private async Task getETGFactSymmetryData()
    {
        try
        {
            ErrorMessageViewModel.Message = "";
            OC_ETGFactSymmetryViewModel.Clear();

            _logger.Information("Running getETGFactSymmetryData() for {CurrentUser}...", Authentication.UserName);
            _sbStatus.Append("Requesting data for ETGFactSymmetry, please wait..." + Environment.NewLine);
            StatusMessageViewModel.Message = _sbStatus.ToString();
            var api = _config.APIS.Where(x => x.Name == "MainData").FirstOrDefault();
            WebAPIConsume.BaseURI = api.BaseUrl;
            var response = WebAPIConsume.GetCall(api.Url);
            if (response.Result.StatusCode == System.Net.HttpStatusCode.OK)
            {
                var reponseStream = await response.Result.Content.ReadAsStreamAsync();
                var result = await JsonSerializer.DeserializeAsync<List<ETGFactSymmetry_ReadDto>>(reponseStream, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });
                int cnt = 1;
                int total = result.Count();
                _sbStatus.Append("Rendering row {$cnt} out of " + total.ToString("N0") + Environment.NewLine);
                result.ForEach(x => 
                {
                    StatusMessageViewModel.Message = _sbStatus.ToString().Replace("{$cnt}", cnt.ToString("N0"));
                    OC_ETGFactSymmetryViewModel.Add(new ETGFactSymmetryViewModel(x));
                    cnt++;
                });

                _logger.Information("ETGFactSymmetryData.getETGFactSymmetryData sucessfully completed for {CurrentUser}...", Authentication.UserName);
                StatusMessageViewModel.Message = _sbStatus.ToString();
            }
            else
            {
                ErrorMessageViewModel.Message = "An error was thrown. Please contact the system admin.";
                _logger.Error("getETGFactSymmetryData threw an error for {CurrentUser}..." + response.Result.StatusCode.ToString(), Authentication.UserName);
            }

            //FIND WAY TO IGNORE LOADING THESE WHENEVER REFRESHED
            //DONT NEED TO TRACK LOADING OF DATA!!!!!
            SharedETGSymmObjects.ETGFactSymmetry_Tracking_List.Clear();


        }
        catch (Exception ex)
        {
            ErrorMessageViewModel.Message = "An error was thrown. Please contact the system admin.";
            _logger.Fatal(ex, "getETGFactSymmetryData.WebAPIConsume.GetCall threw an error for {CurrentUser}", Authentication.UserName);
        }

    }


    private async Task exportConfigs()
    {

        try
        {

            _logger.Information("Running ETGFactSymmetryData.exportConfigs for {CurrentUser}...", Authentication.UserName);


            List<ExcelExport> export = new List<ExcelExport>();
            var excel = _config.excelExportConfig;
            //var file = excel.FilePath + "\\" + excel.FileName;
            var file = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + "\\" + "tmp.xlsx";

            var sheet = excel.Sheets.Where(x => x.Name == "ETGSummaryConfig").FirstOrDefault();
            var etgsum = ETGFactSymmetryConfigMapper.getETGSummaryConfig(OC_ETGFactSymmetryViewModel);
            export.Add(new ExcelExport() { ExportList = etgsum.ToList<object>(), SheetName = sheet.SheetName });


            sheet = excel.Sheets.Where(x => x.Name == "ETGEpisodeCost").FirstOrDefault();
            var etgec = ETGFactSymmetryConfigMapper.getETGEpisodeCostConfig(OC_ETGFactSymmetryViewModel);
            export.Add(new ExcelExport() { ExportList = etgec.ToList<object>(), SheetName = sheet.SheetName });

            sheet = excel.Sheets.Where(x => x.Name == "ETGPatientCentricConfig").FirstOrDefault();
            var api = _config.APIS.Where(x => x.Name == "ETGPatientCentricConfig").FirstOrDefault();
            var etgpc = await VM_Functions.APIGetResultAsync<ETGPatientCentricConfig>(api.BaseUrl, api.Url);
            if (etgpc.Count > 0)
            {
                export.Add(new ExcelExport() { ExportList = etgpc.ToList<object>(), SheetName = sheet.SheetName });
            }

            sheet = excel.Sheets.Where(x => x.Name == "ETGPopEpisodeConfig").FirstOrDefault();
            api = _config.APIS.Where(x => x.Name == "ETGPopEpisodeConfig").FirstOrDefault();
            var etgpe = await VM_Functions.APIGetResultAsync<ETGPopEpisodeConfig>(api.BaseUrl, api.Url);
            if (etgpe.Count > 0)
            {
                export.Add(new ExcelExport() { ExportList = etgpe.ToList<object>(), SheetName = sheet.SheetName });
            }

            var result = await _excelFunctions.ExportToExcelAsync(export, () => StatusMessageViewModel.Message, x => StatusMessageViewModel.Message = x);

            if (File.Exists(file))
                File.Delete(file);

            await File.WriteAllBytesAsync(file, result);


            var p = new Process();
            p.StartInfo = new ProcessStartInfo(file)
            {
                UseShellExecute = true
            };
            p.Start();

            _logger.Information("ETGFactSymmetryData.exportConfigs sucessfully completed for {CurrentUser}...", Authentication.UserName);
        }
        catch (Exception ex)
        {
            ErrorMessageViewModel.Message = "An error was thrown. Please contact the system admin.";
            _logger.Fatal(ex, "ETGFactSymmetryData.exportConfigs threw an error for {CurrentUser}", Authentication.UserName);
        }
    }

    private void loadGridLists()
    {

        _sbStatus.Append("Getting RxNrxOption list..." + Environment.NewLine);
        StatusMessageViewModel.Message = _sbStatus.ToString();
        RxNrxOptions = new List<string>();
        RxNrxOptions.Add("Not Mapped");
        RxNrxOptions.Add("Rx: N / NRx: Y");
        RxNrxOptions.Add("Rx: Y / NRx: Y");
        RxNrxOptions.Add("Rx: N / NRx: N");
        RxNrxOptions.Add("Rx: Y / NRx: N");


        _sbStatus.Append("Getting LobOption list..." + Environment.NewLine);
        StatusMessageViewModel.Message = _sbStatus.ToString();
        LobOptions = new List<string>();
        LobOptions.Add("Not Mapped");
        LobOptions.Add("All");
        LobOptions.Add("Commercial + Medicare");
        LobOptions.Add("Commercial + Medicaid");
        LobOptions.Add("Medicare + Medicaid");
        LobOptions.Add("Commercial Only");
        LobOptions.Add("Medicare Only");
        LobOptions.Add("Medicaid Only");


        _sbStatus.Append("Getting TreatmentIndicatorOption list..." + Environment.NewLine);
        StatusMessageViewModel.Message = _sbStatus.ToString();
        TreatmentIndicatorOptions = new List<string>();
        TreatmentIndicatorOptions.Add("Not Mapped");
        TreatmentIndicatorOptions.Add("All");
        TreatmentIndicatorOptions.Add("0");

        _sbStatus.Append("Getting AttributionOption list..." + Environment.NewLine);
        StatusMessageViewModel.Message = _sbStatus.ToString();
        AttributionOptions = new List<string>();
        AttributionOptions.Add("Not Mapped");
        AttributionOptions.Add("Always Attributed");
        AttributionOptions.Add("If Involved");

        _sbStatus.Append("Getting TreatmentIndicatorECOption list..." + Environment.NewLine);
        StatusMessageViewModel.Message = _sbStatus.ToString();
        TreatmentIndicatorECOptions = new List<string>();
        TreatmentIndicatorECOptions.Add("Not Mapped");
        TreatmentIndicatorECOptions.Add("0");
        TreatmentIndicatorECOptions.Add("0 & 1");

        _sbStatus.Append("Getting MappingOption list..." + Environment.NewLine);
        StatusMessageViewModel.Message = _sbStatus.ToString();
        MappingOptions = new List<string>();
        MappingOptions.Add("Mapped");
        MappingOptions.Add("Not Mapped");

        _sbStatus.Append("Getting PatientCentricMapping list..." + Environment.NewLine);
        StatusMessageViewModel.Message = _sbStatus.ToString();
        PatientCentricMappingOptions = new List<string>();
        PatientCentricMappingOptions.Add("Not Mapped");
        PatientCentricMappingOptions.Add("Yes");
        PatientCentricMappingOptions.Add("No");

    }

    private IETGFactSymmetryConfig prepareConfig(IConfiguration config)
    {

        var project = "ETGSymmetry";
        var section = "Projects";

        ///EXTRACT IConfiguration INTO ETGFactSymmetryConfig 
        var cfg = config.GetSection(section).Get<List<ETGFactSymmetryConfig>>();
        IETGFactSymmetryConfig ecs = new ETGFactSymmetryConfig();
        if (cfg == null)
        {
            //Log.Error($"No Config found for ETGFactSymmetry");
            throw new OperationCanceledException();
        }
        ecs = cfg.Find(p => p.Name == project);
        if (ecs != null)
        {
            //Microsoft.Extensions.Configuration.Binder
            var e = config.GetSection(section + ":" + project + ":APIS").Get<APIConfig[]>();
            if (e != null)
            {
                ecs.APIS = e.ToList();
            }

            //Microsoft.Extensions.Configuration.Binder
            var f = config.GetSection(section + ":" + project + ":ExcelExport").Get<ExcelExportConfig>();
            if (f != null)
            {
                ecs.excelExportConfig = f;
            }
        }

        return ecs;

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
