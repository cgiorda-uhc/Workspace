
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using FileParsingLibrary.Models;
using FileParsingLibrary.MSExcel;
using Microsoft.Extensions.Configuration;
using NPOI.SS.Formula.PTG;
using SharedFunctionsLibrary;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Diagnostics;
using System.Text;
using System.Text.Json;
using System.Windows;
using System.Windows.Input;
using VCPortal_Models.Configuration.HeaderInterfaces.Abstract;
using VCPortal_Models.Configuration.HeaderInterfaces.Concrete;
using VCPortal_Models.Dtos.ETGFactSymmetry;
using VCPortal_Models.Models.ChemoPx;
using VCPortal_Models.Models.ETGFactSymmetry;
using VCPortal_Models.Models.ETGFactSymmetry.Configs;
using VCPortal_WPF_ViewModel.Shared;


namespace VCPortal_WPF_ViewModel.Projects.ETGFactSymmetry;

public partial class ETGFactSymmetryListingViewModel : ObservableObject
{
    private readonly IExcelFunctions _excelFunctions;
    private readonly IETGFactSymmetryConfig? _config;
    private readonly Serilog.ILogger _logger;

    public MessageViewModel UserMessageViewModel { get; }
    public MessageViewModel ProgressMessageViewModel { get; }

    [ObservableProperty]
    private ObservableCollection<ETGFactSymmetryViewModel> oC_ETGFactSymmetryViewModel;


    [ObservableProperty]
    private List<ETGFactSymmetryViewModel> eTGFactSymmetryFilterItems;


    private readonly BackgroundWorker worker = new BackgroundWorker();

    [ObservableProperty]
    private bool canSave;

    [ObservableProperty]
    private string selectedRow;

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

    [ObservableProperty]
    private ObservableCollection<string> pdVersions;


    [ObservableProperty]
    private string currentVersion = "No History";





    [ObservableProperty]
    private string currentTitle;
    [ObservableProperty]
    private bool is_PEC_Visibile;
    [ObservableProperty]
    private bool is_PTC_Visibile;





    private StringBuilder _sbStatus;
    public ETGFactSymmetryListingViewModel(IConfiguration config, IExcelFunctions excelFunctions, Serilog.ILogger logger)
    {
        _logger = logger;
        _excelFunctions = excelFunctions;
        _config = prepareConfig(config);

        UserMessageViewModel = new MessageViewModel();
        ProgressMessageViewModel = new MessageViewModel();

        SharedETGSymmObjects.ETGFactSymmetry_Tracking_List = new ObservableCollection<ETGFactSymmetry_Tracking_UpdateDto>();
        SharedETGSymmObjects.ETGFactSymmetry_Tracking_List.CollectionChanged += listChanged;

        worker.DoWork += worker_DoWork;
        worker.RunWorkerCompleted += worker_RunWorkerCompleted;
        _sbStatus = new StringBuilder();

        OC_ETGFactSymmetryViewModel = new ObservableCollection<ETGFactSymmetryViewModel>();
        ETGFactSymmetryFilterItems = new List<ETGFactSymmetryViewModel>();


        CurrentTitle = "ETG PTC";
        Is_PEC_Visibile = false;
        Is_PTC_Visibile = true;



        if (_config != null)
        {


            //InitialLoadData();
            //getETGFactSymmetryData();
            //Task.Run(async () => await getETGFactSymmetryData());

            InitialLoadData();

            //worker.RunWorkerAsync("InitialLoadData");


            //loadGridLists();
            //Task.Run(async () => await getETGFactSymmetryData());
        }
        else
        {
            UserMessageViewModel.Message = "An error was thrown. Please contact the system admin.";
            _logger.Error($"No Config found for ETGFactSymmetry");
        }
    }

    private async void InitialLoadData()
    {
        _sbStatus.Clear();
        Mouse.OverrideCursor = Cursors.Wait;
        UserMessageViewModel.Message = "";
        ProgressMessageViewModel.Message = "";
        ProgressMessageViewModel.HasMessage = true;
        //worker.RunWorkerAsync("InitialLoadData");
        await loadGridLists();
        await getETGFactSymmetryData();

        Mouse.OverrideCursor = null;
        ProgressMessageViewModel.HasMessage = false;

        //StringBuilder stringBuilder = new StringBuilder();
        //for (int i = 0; i < 100; i++)
        //{
        //    stringBuilder.AppendLine(i + "testing....");
        //    ProgressMessageViewModel.Message = stringBuilder.ToString();
        //    await Task.Delay(500);
        //}
        //ProgressMessageViewModel.Message = stringBuilder.ToString();
        //ProgressMessageViewModel.HasMessage = true;
    }

    [RelayCommand]
    private async Task GetETGFactSymmetryDataCall()
    {
        _sbStatus.Clear();
        Mouse.OverrideCursor = Cursors.Wait;
        UserMessageViewModel.Message = "";
        ProgressMessageViewModel.Message = "";
        ProgressMessageViewModel.HasMessage = true;
        await getETGFactSymmetryData();
        Mouse.OverrideCursor = null;
        ProgressMessageViewModel.HasMessage = false;
    }

    [RelayCommand]
    private async Task ExportConfigsCall()
    {
        UserMessageViewModel.Message = "";
        Mouse.OverrideCursor = Cursors.Wait;
        await Task.Run(() => worker.RunWorkerAsync("ExportConfigs"));
        Mouse.OverrideCursor = null;

    }

    [RelayCommand]
    private async Task SaveCall()
    {
        //worker.RunWorkerAsync("SaveData");
        save();
    }


    private void worker_DoWork(object sender, DoWorkEventArgs e)
    {
        var callingFunction = (string)e.Argument;

        _sbStatus.Clear();
        UserMessageViewModel.Message = "";
        ProgressMessageViewModel.Message = "";
        ProgressMessageViewModel.HasMessage = true;
        if (callingFunction == "ExportConfigs")
        {
            ProgressMessageViewModel.HasMessage = true;
            exportConfigs();
        }
        //else if (callingFunction == "LoadData")
        //{
        //    ProgressMessageViewModel.HasMessage = true;
        //    getETGFactSymmetryData();

        //}
        else if (callingFunction == "InitialLoadData")
        {
            ProgressMessageViewModel.HasMessage = true;
            loadGridLists();
            //getETGFactSymmetryData();

        }
        //else if (callingFunction == "SaveData")
        //{
        //    save();
        //}

    }
    private void worker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
    {
        //update ui once worker complete his work
        ProgressMessageViewModel.HasMessage = false;

    }
    private void listChanged(object sender, NotifyCollectionChangedEventArgs args)
    {
        // list changed
        CanSave = true;
    }


    private void save()
    {
        try
        {

            if (SharedETGSymmObjects.ETGFactSymmetry_Tracking_List.Count == 0)
            {
                UserMessageViewModel.Message = "No changes to save";
                return;
            }


            _logger.Information("Running ETGFactSymmetryData.Save for {CurrentUser}...", Authentication.UserName);

            var tracked = SharedETGSymmObjects.ETGFactSymmetry_Tracking_List;
            var date = DateTime.Now;
            foreach (var t in tracked)
            {
                t.update_date = date;
                t.username = Authentication.UserName;
            }

            var api = _config.APIS.Where(x => x.Name == "ETGInsert").FirstOrDefault();
            WebAPIConsume.BaseURI = api.BaseUrl;
            var response = WebAPIConsume.PostCall<ObservableCollection<ETGFactSymmetry_Tracking_UpdateDto>>(api.Url, tracked);
            if (response.Result.StatusCode != System.Net.HttpStatusCode.OK)
            {
                UserMessageViewModel.IsError = true;
                UserMessageViewModel.Message = "An error was thrown. Please contact the system admin.";
                _logger.Error("ETGFactSymmetryData.Save threw an error for {CurrentUser}" + response.Result.StatusCode.ToString(), Authentication.UserName);
            }
            else
            {

                UserMessageViewModel.IsError = false;
                UserMessageViewModel.Message = "ETGFactSymmetryData.Save sucessfully completed";
                _logger.Information("ETGFactSymmetryData.Save sucessfully completed for {CurrentUser}...", Authentication.UserName);
            }

            SharedETGSymmObjects.ETGFactSymmetry_Tracking_List.Clear();

            _logger.Information("ETGFactSymmetryData.Save sucessfully completed for {CurrentUser}...", Authentication.UserName);


        }
        catch (Exception ex)
        {
            UserMessageViewModel.IsError = true;
            UserMessageViewModel.Message = "An error was thrown. Please contact the system admin.";
            _logger.Fatal(ex, "ETGFactSymmetryData.Save threw an error for {CurrentUser}", Authentication.UserName);
        }
        finally
        {
            CanSave = false;
        }
    }


    private async Task getETGFactSymmetryData()
    {
        try
        {
            OC_ETGFactSymmetryViewModel.Clear();

            _logger.Information("Running getETGFactSymmetryData() for {CurrentUser}...", Authentication.UserName);
            _sbStatus.Append("--Requesting data for ETGFactSymmetry, please wait..." + Environment.NewLine);
            ProgressMessageViewModel.Message = _sbStatus.ToString();
            await Task.Delay(TimeSpan.FromSeconds(1));
            var api = _config.APIS.Where(x => x.Name == "MainDataPTC").FirstOrDefault();
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
                _sbStatus.Append("--Rendering row {$cnt} out of " + total.ToString("N0") + Environment.NewLine);

       
                foreach (var r in result)
                {
                    ProgressMessageViewModel.Message = _sbStatus.ToString().Replace("{$cnt}", cnt.ToString("N0"));
                    OC_ETGFactSymmetryViewModel.Add(new ETGFactSymmetryViewModel(r));
                    //await Task.Delay(TimeSpan.FromSeconds(.001));
                    if(cnt % 100 == 0)
                    {
                        await Task.Delay(TimeSpan.FromSeconds(.001));
                    }
                    cnt++;
                }    


                //result.ForEach(x =>
                //{
                //    ProgressMessageViewModel.Message = _sbStatus.ToString().Replace("{$cnt}", cnt.ToString("N0"));
                //    OC_ETGFactSymmetryViewModel.Add(new ETGFactSymmetryViewModel(x));
                //    //Task.Delay(TimeSpan.FromSeconds(1.0));
                //    cnt++;
                //});

                _logger.Information("ETGFactSymmetryData.getETGFactSymmetryData sucessfully completed for {CurrentUser}...", Authentication.UserName);
                ProgressMessageViewModel.Message = _sbStatus.ToString();
            }
            else
            {
                UserMessageViewModel.IsError = true;
                UserMessageViewModel.Message = "An error was thrown. Please contact the system admin.";
                _logger.Error("getETGFactSymmetryData threw an error for {CurrentUser}..." + response.Result.StatusCode.ToString(), Authentication.UserName);
            }

            //FIND WAY TO IGNORE LOADING THESE WHENEVER REFRESHED
            //DONT NEED TO TRACK LOADING OF DATA!!!!!
            SharedETGSymmObjects.ETGFactSymmetry_Tracking_List.Clear();


        }
        catch (Exception ex)
        {
            UserMessageViewModel.IsError = true;
            UserMessageViewModel.Message = "An error was thrown. Please contact the system admin.";
            _logger.Fatal(ex, "getETGFactSymmetryData.WebAPIConsume.GetCall threw an error for {CurrentUser}", Authentication.UserName);
        }
        finally
        {
            CanSave = false;
        }

    }


    private async Task exportConfigs()
    {

        try
        {

            _logger.Information("Running ETGFactSymmetryData.exportConfigs for {CurrentUser}...", Authentication.UserName);


            APIConfig api;
            string suffix;

            if(CurrentVersion != null)
            {
                var s = CurrentVersion;
            }



            List<ExcelExport> export = new List<ExcelExport>();
            var excel = _config.excelExportConfig;
            //var file = excel.FilePath + "\\" + excel.FileName;
            var file = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + "\\" + "tmp.xlsx";

            var sheet = excel.Sheets.Where(x => x.Name == "ETGSummaryConfig").FirstOrDefault();
            if(Is_PTC_Visibile)
            {

                suffix = "PTC_";



                //var etgsum = ETGFactSymmetryConfigMapper.getETG_PTC_SummaryConfig(OC_ETGFactSymmetryViewModel);
                //export.Add(new ExcelExport() { ExportList = etgsum.ToList<object>(), SheetName = suffix  + sheet.SheetName  });




                api = _config.APIS.Where(x => x.Name == "MainDataPTC").FirstOrDefault();
                var etgsum = await VM_Functions.APIGetResultAsync<ETGPTCSummaryConfig>(api.BaseUrl, api.Url);
                if (etgsum.Count > 0)
                {
                    export.Add(new ExcelExport() { ExportList = etgsum.ToList<object>(), SheetName = suffix + sheet.SheetName });
                }




                sheet = excel.Sheets.Where(x => x.Name == "ETGPTCModelConfig").FirstOrDefault();
                api = _config.APIS.Where(x => x.Name == "ETGPTCModelConfig").FirstOrDefault();
                var etgptc = await VM_Functions.APIGetResultAsync<ETG_PTC_Modeling_Model>(api.BaseUrl, api.Url);
                if (etgptc.Count > 0)
                {
                    export.Add(new ExcelExport() { ExportList = etgptc.ToList<object>(), SheetName = sheet.SheetName });
                }

                sheet = excel.Sheets.Where(x => x.Name == "ETGSummaryFinal").FirstOrDefault();
                api = _config.APIS.Where(x => x.Name == "ETGSummaryFinalPTC").FirstOrDefault();
                var etgfinal = await VM_Functions.APIGetResultAsync<ETGSummaryFinal_PTC_Config>(api.BaseUrl, api.Url);
                if (etgfinal.Count > 0)
                {
                    export.Add(new ExcelExport() { ExportList = etgfinal.ToList<object>(), SheetName = suffix  + sheet.SheetName });
                }

                sheet = excel.Sheets.Where(x => x.Name == "ETGPTUGAPConfig").FirstOrDefault();
                api = _config.APIS.Where(x => x.Name == "ETGPTUGAPConfig").FirstOrDefault();
                var etgugap = await VM_Functions.APIGetResultAsync<ETG_UGAP_CFG_Model>(api.BaseUrl, api.Url);
                if (etgugap.Count > 0)
                {
                    export.Add(new ExcelExport() { ExportList = etgugap.ToList<object>(), SheetName =  sheet.SheetName });
                }

            
                sheet = excel.Sheets.Where(x => x.Name == "ETGPCNrxConfig").FirstOrDefault();
                api = _config.APIS.Where(x => x.Name == "ETGPCNrxConfig").FirstOrDefault();
                var etgpcnrx = await VM_Functions.APIGetResultAsync<ETG_CNFG_PC_ETG_NRX>(api.BaseUrl, api.Url);
                if (etgpcnrx.Count > 0)
                {
                    export.Add(new ExcelExport() { ExportList = etgpcnrx.ToList<object>(), SheetName = sheet.SheetName });
                }

                sheet = excel.Sheets.Where(x => x.Name == "ETGNrxCompareConfig").FirstOrDefault();
                api = _config.APIS.Where(x => x.Name == "ETGNrxCompareConfig").FirstOrDefault();
                var etgnxc = await VM_Functions.APIGetResultAsync<ETG_CNFG_ETG_NRX_COMPARE>(api.BaseUrl, api.Url);
                if (etgnxc.Count > 0)
                {
                    export.Add(new ExcelExport() { ExportList = etgnxc.ToList<object>(), SheetName = sheet.SheetName });
                }

            }
            else
            {
                suffix = "PEC_";


                //var etgsum = ETGFactSymmetryConfigMapper.getETG_PEC_SummaryConfig(OC_ETGFactSymmetryViewModel);
                //export.Add(new ExcelExport() { ExportList = etgsum.ToList<object>(), SheetName = suffix + sheet.SheetName });
                api = _config.APIS.Where(x => x.Name == "MainData").FirstOrDefault();
                var etgsum = await VM_Functions.APIGetResultAsync<ETGPECSummaryConfig>(api.BaseUrl, api.Url);
                if (etgsum.Count > 0)
                {
                    export.Add(new ExcelExport() { ExportList = etgsum.ToList<object>(), SheetName = suffix + sheet.SheetName });
                }




                sheet = excel.Sheets.Where(x => x.Name == "ETGSummaryFinal").FirstOrDefault();
                api = _config.APIS.Where(x => x.Name == "ETGSummaryFinal").FirstOrDefault();
                var etgfinal = await VM_Functions.APIGetResultAsync<ETGSummaryFinal_PEC_Config>(api.BaseUrl, api.Url);
                if (etgfinal.Count > 0)
                {
                    export.Add(new ExcelExport() { ExportList = etgfinal.ToList<object>(), SheetName = suffix + sheet.SheetName });
                }



                sheet = excel.Sheets.Where(x => x.Name == "ETGNrxExclConfig").FirstOrDefault();
                api = _config.APIS.Where(x => x.Name == "ETGNrxExclConfig").FirstOrDefault();
                var etgnxe = await VM_Functions.APIGetResultAsync<ETG_CNFG_ETG_NRX_EXCLD>(api.BaseUrl, api.Url);
                if (etgnxe.Count > 0)
                {
                    export.Add(new ExcelExport() { ExportList = etgnxe.ToList<object>(), SheetName = sheet.SheetName });
                }


                sheet = excel.Sheets.Where(x => x.Name == "ETGSpclConfig").FirstOrDefault();
                api = _config.APIS.Where(x => x.Name == "ETGSpclConfig").FirstOrDefault();
                var etgspc = await VM_Functions.APIGetResultAsync<ETG_CNFG_ETG_SPCL>(api.BaseUrl, api.Url);
                if (etgspc.Count > 0)
                {
                    export.Add(new ExcelExport() { ExportList = etgspc.ToList<object>(), SheetName = sheet.SheetName });
                }

            }



            api = _config.APIS.Where(x => x.Name == "Tracking").FirstOrDefault();
            var tracking = await VM_Functions.APIGetResultAsync<ETGFactSymmetry_Tracking_ReadDto>(api.BaseUrl, api.Url);
            if (tracking.Count > 0)
            {
                export.Add(new ExcelExport() { ExportList = tracking.ToList<object>(), SheetName = "Tracking" });

            }



            api = _config.APIS.Where(x => x.Name == "ETGLatest").FirstOrDefault();
            var latest = await VM_Functions.APIGetResultAsync<ETG_Lastest_Model>(api.BaseUrl, api.Url);
            export.Add(new ExcelExport() { ExportList = latest.ToList<object>(), SheetName = "ETGLatest" });
            
        

            if(CurrentVersion.IsNumeric())
            {
                sheet = excel.Sheets.Where(x => x.Name == "ETGAdhoc").FirstOrDefault();
                api = _config.APIS.Where(x => x.Name == "ETGAdhoc").FirstOrDefault();
                var etgad = await VM_Functions.APIGetResultAsync<ETGSummaryAdhocConfig> (api.BaseUrl, api.Url + "/" + CurrentVersion);
                if (etgad.Count > 0)
                {
                    export.Add(new ExcelExport() { ExportList = etgad.ToList<object>(), SheetName = sheet.SheetName });
                }
            }


            if (ETGFactSymmetryFilterItems.Count>0)
            {
                sheet = excel.Sheets.Where(x => x.Name == "ETGFiltered").FirstOrDefault();
                var etgfil = ETGFactSymmetryConfigMapper.getETG_PTC_SummaryConfig(ETGFactSymmetryFilterItems);
                export.Add(new ExcelExport() { ExportList = etgfil.ToList<object>(), SheetName = sheet.SheetName });

            }



            //sheet = excel.Sheets.Where(x => x.Name == "ETGEpisodeCost").FirstOrDefault();
            //var etgec = ETGFactSymmetryConfigMapper.getETGEpisodeCostConfig(OC_ETGFactSymmetryViewModel);
            //export.Add(new ExcelExport() { ExportList = etgec.ToList<object>(), SheetName = sheet.SheetName });

            //sheet = excel.Sheets.Where(x => x.Name == "ETGPatientCentricConfig").FirstOrDefault();
            //var api = _config.APIS.Where(x => x.Name == "ETGPatientCentricConfig").FirstOrDefault();
            //var etgpc = await VM_Functions.APIGetResultAsync<ETGPatientCentricConfig>(api.BaseUrl, api.Url);
            //if (etgpc.Count > 0)
            //{
            //    export.Add(new ExcelExport() { ExportList = etgpc.ToList<object>(), SheetName = sheet.SheetName });
            //}

            //sheet = excel.Sheets.Where(x => x.Name == "ETGPopEpisodeConfig").FirstOrDefault();
            //var api = _config.APIS.Where(x => x.Name == "ETGPopEpisodeConfig").FirstOrDefault();
            //var etgpe = await VM_Functions.APIGetResultAsync<ETGPopEpisodeConfig>(api.BaseUrl, api.Url);
            //if (etgpe.Count > 0)
            //{
            //    export.Add(new ExcelExport() { ExportList = etgpe.ToList<object>(), SheetName = sheet.SheetName });
            //}



            var result = await _excelFunctions.ExportToExcelAsync(export, () => ProgressMessageViewModel.Message, x => ProgressMessageViewModel.Message = x);

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
            UserMessageViewModel.IsError = true;
            UserMessageViewModel.Message = "An error was thrown. Please contact the system admin.";
            _logger.Fatal(ex, "ETGFactSymmetryData.exportConfigs threw an error for {CurrentUser}", Authentication.UserName);
        }
    }

    private async Task loadGridLists()
    {

        var api = _config.APIS.Where(x => x.Name == "ETGPDVersion").FirstOrDefault();
        WebAPIConsume.BaseURI = api.BaseUrl;
        _sbStatus.Append("--Getting PDVersions list..." + Environment.NewLine);
        ProgressMessageViewModel.Message = _sbStatus.ToString();
        CurrentVersion = "No History";
        await Task.Delay(TimeSpan.FromSeconds(.5));
        var response = WebAPIConsume.GetCall(api.Url);

        PdVersions = new ObservableCollection<string>();


        if (response.Result.StatusCode == System.Net.HttpStatusCode.OK)
        {
            var reponseStream = await response.Result.Content.ReadAsStreamAsync();
            var result = await JsonSerializer.DeserializeAsync<List<ETGVersion_Model>>(reponseStream, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });

            var lst = result.Select(x => x.PD_Version.ToString()).ToList();
            PdVersions.Add(CurrentVersion);
            foreach (var l in lst)
            {
                PdVersions.Add(l);
            }

        }
        else
        {
            UserMessageViewModel.IsError = true;
            UserMessageViewModel.Message = "An error was thrown. Please contact the system admin.";
            _logger.Error("loadGridLists.PDVersions threw an error for {CurrentUser}" + response.Result.StatusCode.ToString(), Authentication.UserName);
        }

        _sbStatus.Append("--Getting RxNrxOption list..." + Environment.NewLine);
        ProgressMessageViewModel.Message = _sbStatus.ToString();
        RxNrxOptions = new List<string>();
        RxNrxOptions.Add("Not Mapped");
        RxNrxOptions.Add("Rx: N / NRx: Y");
        RxNrxOptions.Add("Rx: Y / NRx: Y");
        RxNrxOptions.Add("Rx: N / NRx: N");
        RxNrxOptions.Add("Rx: Y / NRx: N");


        _sbStatus.Append("--Getting LobOption list..." + Environment.NewLine);
        ProgressMessageViewModel.Message = _sbStatus.ToString();
        LobOptions = new List<string>();
        LobOptions.Add("Not Mapped");
        LobOptions.Add("All");
        LobOptions.Add("Commercial + Medicare");
        LobOptions.Add("Commercial + Medicaid");
        LobOptions.Add("Medicare + Medicaid");
        LobOptions.Add("Commercial Only");
        LobOptions.Add("Medicare Only");
        LobOptions.Add("Medicaid Only");


        _sbStatus.Append("--Getting TreatmentIndicatorOption list..." + Environment.NewLine);
        ProgressMessageViewModel.Message = _sbStatus.ToString();
        TreatmentIndicatorOptions = new List<string>();
        TreatmentIndicatorOptions.Add("Not Mapped");
        TreatmentIndicatorOptions.Add("All");
        TreatmentIndicatorOptions.Add("0");

        _sbStatus.Append("--Getting AttributionOption list..." + Environment.NewLine);
        ProgressMessageViewModel.Message = _sbStatus.ToString();
        AttributionOptions = new List<string>();
        AttributionOptions.Add("Not Mapped");
        AttributionOptions.Add("Always Attributed");
        AttributionOptions.Add("If Involved");

        _sbStatus.Append("--Getting TreatmentIndicatorECOption list..." + Environment.NewLine);
        ProgressMessageViewModel.Message = _sbStatus.ToString();
        TreatmentIndicatorECOptions = new List<string>();
        TreatmentIndicatorECOptions.Add("Not Mapped");
        TreatmentIndicatorECOptions.Add("0");
        TreatmentIndicatorECOptions.Add("0 & 1");

        _sbStatus.Append("--Getting MappingOption list..." + Environment.NewLine);
        ProgressMessageViewModel.Message = _sbStatus.ToString();
        MappingOptions = new List<string>();
        MappingOptions.Add("Mapped");
        MappingOptions.Add("Not Mapped");

        _sbStatus.Append("--Getting PatientCentricMapping list..." + Environment.NewLine);
        ProgressMessageViewModel.Message = _sbStatus.ToString();
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


    [RelayCommand]
    private async Task PTCSectionCall()
    {
        CurrentTitle = "ETG PTC";
        Is_PEC_Visibile = false;
        Is_PTC_Visibile = true;

    }


    [RelayCommand]
    private async Task PECSectionCall()
    {
        CurrentTitle = "ETG PEC";
        Is_PEC_Visibile = true;
        Is_PTC_Visibile = false;
    }





    //bool disposed;
    //protected virtual void Dispose(bool disposing)
    //{
    //    if (!disposed)
    //    {
    //        if (disposing)
    //        {
    //            //dispose managed resources
    //        }
    //    }
    //    //dispose unmanaged resources
    //    disposed = true;
    //}

    //public void Dispose()
    //{
    //    Dispose(true);
    //    GC.SuppressFinalize(this);
    //}

}
