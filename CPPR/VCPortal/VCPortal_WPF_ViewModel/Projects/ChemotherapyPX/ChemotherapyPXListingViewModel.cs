using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using FileParsingLibrary.Models;
using FileParsingLibrary.MSExcel;
using Microsoft.Extensions.Configuration;
using SharedFunctionsLibrary;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Text.Json;
using VCPortal_Models.Configuration.HeaderInterfaces.Abstract;
using VCPortal_Models.Configuration.HeaderInterfaces.Concrete;
using VCPortal_Models.Dtos.ChemoPx;
using VCPortal_Models.Dtos.ETGFactSymmetry;
using VCPortal_Models.Models.ChemoPx;
using VCPortal_Models.Models.ETGFactSymmetry;
using VCPortal_Models.Models.Shared;
using VCPortal_WPF_ViewModel.Projects.ETGFactSymmetry;
using VCPortal_WPF_ViewModel.Shared;

namespace VCPortal_WPF_ViewModel.Projects.ChemotherapyPX;
public partial class ChemotherapyPXListingViewModel : ObservableObject, ViewModelBase
{
    private readonly IExcelFunctions _excelFunctions;
    private readonly IChemotherapyPXConfig? _config;
    private readonly Serilog.ILogger _logger;

    private readonly BackgroundWorker worker = new BackgroundWorker();


    public MessageViewModel ErrorMessageViewModel { get; }
    public MessageViewModel StatusMessageViewModel { get; }

    [ObservableProperty]
    private ObservableCollection<ChemotherapyPXViewModel> oC_ChemotherapyPXViewModel;

    [ObservableProperty]
    private ChemotherapyPXViewModel selectedRow;

    [ObservableProperty]
    private List<string> procCodes;

    [ObservableProperty]
    private List<Code_Category_Model> codeCategories;

    [ObservableProperty]
    private List<ASP_Category_Model> aspCategories;

    [ObservableProperty]
    private List<Drug_Adm_Mode_Model> drugAdmModes;

    [ObservableProperty]
    private List<PA_Drugs_Model> pADrugs;

    [ObservableProperty]
    private List<CEP_Pay_Cd_Model> cEPPayCds;

    [ObservableProperty]
    private List<CEP_Enroll_Cd_Model> cEPEnrollCds;

    [ObservableProperty]
    private List<string> sources;

    [ObservableProperty]
    private List<string> cEPEnrExcl;

    public ChemotherapyPXListingViewModel(IConfiguration config, IExcelFunctions excelFunctions, Serilog.ILogger logger)
    {


        _logger = logger;
        _excelFunctions = excelFunctions;
        _config = prepareConfig(config);

        ErrorMessageViewModel = new MessageViewModel();
        StatusMessageViewModel = new MessageViewModel();
        //StatusMessageViewModel.HasMessage = true;

        SharedChemoObjects.ChemotherapyPX_Tracking_List = new List<ChemotherapyPX_Tracking_CUD_Dto>();

        worker.DoWork += worker_DoWork;
        worker.RunWorkerCompleted += worker_RunWorkerCompleted;


        OC_ChemotherapyPXViewModel = new ObservableCollection<ChemotherapyPXViewModel>();

        if (_config != null)
        {
            loadGridLists();
            //Task.Run(async () => await loadGridLists());
            worker.RunWorkerAsync("LoadData");


            //Task.Run(async () => await getChemotherapyPXData());
        }
        else
        {
            ErrorMessageViewModel.Message = "An error was thrown. Please contact the system admin.";
            _logger.Error($"No Config found for ChemotherapyPX");
        }

        //StatusMessageViewModel.HasMessage = false;
    }

    private void worker_DoWork(object sender, DoWorkEventArgs e)
    {
        var callingFunction = (string)e.Argument;
        if (callingFunction == "ExportData")
        {
            StatusMessageViewModel.HasMessage = true;
            exportData();

        }
        else if (callingFunction == "LoadData")
        {
            StatusMessageViewModel.HasMessage = true;
            getChemotherapyPXData();

        }
    }

    private void worker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
    {
        //update ui once worker complete his work
        StatusMessageViewModel.HasMessage = false;

    }


    [RelayCommand]
    private async Task getChemotherapyPXDataCall()
    {
        //StatusMessageViewModel.HasMessage = true;
        //getChemotherapyPXData();
        //StatusMessageViewModel.HasMessage = false;


        StatusMessageViewModel.HasMessage = true;
        worker.RunWorkerAsync("LoadData");
    }



    [RelayCommand]
    private async Task ExportDataCall()
    {
        StatusMessageViewModel.HasMessage = true;
        worker.RunWorkerAsync("ExportData");
    }

    [RelayCommand]
    private void addNewRow()
    {
        OC_ChemotherapyPXViewModel.Insert(0, new ChemotherapyPXViewModel(new ChemotherapyPX_ReadDto()));
    }

    [RelayCommand]
    private void deleteRow()
    {

        try
        {
            _logger.Information("Running ChemotherapyPX.deleteRow for {CurrentUser}...", Authentication.UserName);

            var row = SelectedRow;

            //UPDATE OBSERVED COLLECTION
            OC_ChemotherapyPXViewModel.Remove(row);

            //UPDATE TRACKING
            var chemo = SharedChemoObjects.ChemotherapyPX_Tracking_List.FirstOrDefault(x => x.CODE == row.CODE);
            if (chemo != null)
            {
                SharedChemoObjects.ChemotherapyPX_Tracking_List.Remove(chemo);
            }
            SharedChemoObjects.ChemotherapyPX_Tracking_List.Add(new ChemotherapyPX_Tracking_CUD_Dto() { ChemoPX_Id = row.Id, CODE = row.CODE, UPDATE_ACTION = "DELETE" });
            SharedObjects.ProcCodes.Add(new ProcCodesModel() { Proc_Cd = row.CODE, Proc_Desc = row.CODE_DESC_REF, Proc_Cd_Type = row.CODE_TYPE_REF, Proc_Cd_Date = row.CODE_END_DT_REF });

            _logger.Information("ChemotherapyPX.deleteRow sucessfully completed for {CurrentUser}...", Authentication.UserName);
        }
        catch (Exception ex)
        {
            ErrorMessageViewModel.Message = "An error was thrown. Please contact the system admin.";
            _logger.Fatal(ex, "ChemotherapyPXData.deleteRow threw an error for {CurrentUser}", Authentication.UserName);
        }

    }

    [RelayCommand]
    private void save()
    {
        try
        {
            _logger.Information("Running ChemotherapyPXData.Save for {CurrentUser}...", Authentication.UserName);

            var tracked = SharedChemoObjects.ChemotherapyPX_Tracking_List;
            var date = DateTime.Now;
            foreach (var t in tracked)
            {
                t.UPDATE_DT = date;
                t.UPDATE_USER = "cgiorda";
            }

            var api = _config.APIS.Where(x => x.Name == "MainData").FirstOrDefault();
            WebAPIConsume.BaseURI = api.BaseUrl;
            var response = WebAPIConsume.PostCall<List<ChemotherapyPX_Tracking_CUD_Dto>>(api.Url, tracked);
            if (response.Result.StatusCode != System.Net.HttpStatusCode.OK)
            {
                ErrorMessageViewModel.Message = "An error was thrown. Please contact the system admin.";
                _logger.Error("ChemotherapyPXData.Save threw an error for {CurrentUser}" + response.Result.StatusCode.ToString(), Authentication.UserName);
            }
            else
            {
                _logger.Information("ChemotherapyPXData.Save sucessfully completed for {CurrentUser}...", Authentication.UserName);
            }

            try
            {
                SharedChemoObjects.ChemotherapyPX_Tracking_List.Clear();
                ProcCodes = SharedObjects.ProcCodes.Select(x => x.Proc_Cd_Full).ToList();
            }
            catch (Exception ex)
            {
                _logger.Error("ChemotherapyPXData.Save.ProcCodes threw an error for {CurrentUser}", Authentication.UserName);
            }
        }
        catch (Exception ex)
        {
            ErrorMessageViewModel.Message = "An error was thrown. Please contact the system admin.";
            _logger.Fatal(ex, "ChemotherapyPXData.save threw an error for {CurrentUser}", Authentication.UserName);
        }
    }


    private async Task getChemotherapyPXData()
    {

        StringBuilder sbStatus = new StringBuilder();
        try
        {

            ErrorMessageViewModel.Message = "";
            OC_ChemotherapyPXViewModel.Clear();

            _logger.Information("Running getChemotherapyPXData() for {CurrentUser}...", Authentication.UserName);
            sbStatus.Append("Requesting data for ChemotherapyPX, please wait..." + Environment.NewLine);
            StatusMessageViewModel.Message = sbStatus.ToString();
            var api = _config.APIS.Where(x => x.Name == "MainData").FirstOrDefault();
            WebAPIConsume.BaseURI = api.BaseUrl;
            var response = WebAPIConsume.GetCall(api.Url);
            if (response.Result.StatusCode == System.Net.HttpStatusCode.OK)
            {
                var reponseStream = await response.Result.Content.ReadAsStreamAsync();
                var result = await JsonSerializer.DeserializeAsync<List<ChemotherapyPX_ReadDto>>(reponseStream, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });

                int cnt = 1;
                int total = result.Count();
                sbStatus.Append("Retrieving row {$cnt} out of " + total + Environment.NewLine);
                result.ForEach(x => 
                    {
                        StatusMessageViewModel.Message = sbStatus.ToString().Replace("{$cnt}", cnt.ToString());
                        OC_ChemotherapyPXViewModel.Add(new ChemotherapyPXViewModel(x));
                        cnt++;
                    });


                _logger.Information("getChemotherapyPXData sucessfully completed for {CurrentUser}...", Authentication.UserName);
                sbStatus.Append("Rendering grid..." + Environment.NewLine);
                StatusMessageViewModel.Message = sbStatus.ToString();

            }
            else
            {
                ErrorMessageViewModel.Message = "An error was thrown. Please contact the system admin.";
                _logger.Error("getChemotherapyPXData threw an error for {CurrentUser}..." + response.Result.StatusCode.ToString(), Authentication.UserName);
            }

            //SelectedRow = OC_ChemotherapyPXViewModel[0];
        }
        catch (Exception ex)
        {
            ErrorMessageViewModel.Message = "An error was thrown. Please contact the system admin.";
            _logger.Fatal(ex, "getChemotherapyPXData.WebAPIConsume.GetCall threw an error for {CurrentUser}", Authentication.UserName);
        }



    }

    private async Task exportData()
    {

        try
        {

            _logger.Information("Running ChemotherapyPXData.exportConfigs for {CurrentUser}...", Authentication.UserName);


            var file = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + "\\" + "tmp.xlsx";
            var result = await _excelFunctions.ExportToExcelAsync(OC_ChemotherapyPXViewModel.ToList(),"ChemotherapyPX_Data",  () => StatusMessageViewModel.Message, x => StatusMessageViewModel.Message = x);

            if (File.Exists(file))
                File.Delete(file);

            await File.WriteAllBytesAsync(file, result);


            var p = new Process();
            p.StartInfo = new ProcessStartInfo(file)
            {
                UseShellExecute = true
            };
            p.Start();

            _logger.Information("ChemotherapyPXData.exportConfigs sucessfully completed for {CurrentUser}...", Authentication.UserName);
        }
        catch (Exception ex)
        {
            ErrorMessageViewModel.Message = "An error was thrown. Please contact the system admin.";
            _logger.Fatal(ex, "ChemotherapyPXData.exportConfigs threw an error for {CurrentUser}", Authentication.UserName);
        }
    }



    private async Task loadGridLists()
    {
        try 
        { 
            var api = _config.APIS.Where(x => x.Name == "CodeCategory").FirstOrDefault();
            WebAPIConsume.BaseURI = api.BaseUrl;
            var response = WebAPIConsume.GetCall(api.Url);
            if (response.Result.StatusCode == System.Net.HttpStatusCode.OK)
            {
                var reponseStream = await response.Result.Content.ReadAsStreamAsync();
                var result = await JsonSerializer.DeserializeAsync<List<Code_Category_Model>>(reponseStream, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });

                CodeCategories = result;
            }
            else
            {
                ErrorMessageViewModel.Message = "An error was thrown. Please contact the system admin.";
                _logger.Error("loadGridLists.CodeCategory threw an error for {CurrentUser}" + response.Result.StatusCode.ToString(), Authentication.UserName);
            }


            api = _config.APIS.Where(x => x.Name == "AspCategory").FirstOrDefault();
            WebAPIConsume.BaseURI = api.BaseUrl;
            response = WebAPIConsume.GetCall(api.Url);
            if (response.Result.StatusCode == System.Net.HttpStatusCode.OK)
            {
                var reponseStream = await response.Result.Content.ReadAsStreamAsync();
                var result = await JsonSerializer.DeserializeAsync<List<ASP_Category_Model>>(reponseStream, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });

                AspCategories = result;
            }
            else
            {
                ErrorMessageViewModel.Message = "An error was thrown. Please contact the system admin.";
                _logger.Error("loadGridLists.AspCategory threw an error for {CurrentUser}" + response.Result.StatusCode.ToString(), Authentication.UserName);
            }

            api = _config.APIS.Where(x => x.Name == "DrugAdmMode").FirstOrDefault();
            WebAPIConsume.BaseURI = api.BaseUrl;
            response = WebAPIConsume.GetCall(api.Url);
            if (response.Result.StatusCode == System.Net.HttpStatusCode.OK)
            {
                var reponseStream = await response.Result.Content.ReadAsStreamAsync();
                var result = await JsonSerializer.DeserializeAsync<List<Drug_Adm_Mode_Model>>(reponseStream, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });

                DrugAdmModes = result;
            }
            else
            {
                ErrorMessageViewModel.Message = "An error was thrown. Please contact the system admin.";
                _logger.Error("loadGridLists.DrugAdmMode threw an error for {CurrentUser}" + response.Result.StatusCode.ToString(), Authentication.UserName);
            }

            api = _config.APIS.Where(x => x.Name == "PADrugs").FirstOrDefault();
            WebAPIConsume.BaseURI = api.BaseUrl;
            response = WebAPIConsume.GetCall(api.Url);
            if (response.Result.StatusCode == System.Net.HttpStatusCode.OK)
            {
                var reponseStream = await response.Result.Content.ReadAsStreamAsync();
                var result = await JsonSerializer.DeserializeAsync<List<PA_Drugs_Model>>(reponseStream, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });

                PADrugs = result;
            }
            else
            {
                ErrorMessageViewModel.Message = "An error was thrown. Please contact the system admin.";
                _logger.Error("loadGridLists.PADrugs threw an error for {CurrentUser}" + response.Result.StatusCode.ToString(), Authentication.UserName);
            }

            api = _config.APIS.Where(x => x.Name == "CEPPayCd").FirstOrDefault();
            WebAPIConsume.BaseURI = api.BaseUrl;
            response = WebAPIConsume.GetCall(api.Url);
            if (response.Result.StatusCode == System.Net.HttpStatusCode.OK)
            {
                var reponseStream = await response.Result.Content.ReadAsStreamAsync();
                var result = await JsonSerializer.DeserializeAsync<List<CEP_Pay_Cd_Model>>(reponseStream, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });

                CEPPayCds = result;
            }
            else
            {
                ErrorMessageViewModel.Message = "An error was thrown. Please contact the system admin.";
                _logger.Error("loadGridLists.CEPPayCd threw an error for {CurrentUser}" + response.Result.StatusCode.ToString(), Authentication.UserName);
            }

            api = _config.APIS.Where(x => x.Name == "CEPEnrollCd").FirstOrDefault();
            WebAPIConsume.BaseURI = api.BaseUrl;
            response = WebAPIConsume.GetCall(api.Url);
            if (response.Result.StatusCode == System.Net.HttpStatusCode.OK)
            {
                var reponseStream = await response.Result.Content.ReadAsStreamAsync();
                var result = await JsonSerializer.DeserializeAsync<List<CEP_Enroll_Cd_Model>>(reponseStream, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });

                CEPEnrollCds = result;
            }
            else
            {
                ErrorMessageViewModel.Message = "An error was thrown. Please contact the system admin.";
                _logger.Error("loadGridLists.CEPEnrollCd threw an error for {CurrentUser}" + response.Result.StatusCode.ToString(), Authentication.UserName);
            }

            api = _config.APIS.Where(x => x.Name == "Source").FirstOrDefault();
            WebAPIConsume.BaseURI = api.BaseUrl;
            response = WebAPIConsume.GetCall(api.Url);
            if (response.Result.StatusCode == System.Net.HttpStatusCode.OK)
            {
                var reponseStream = await response.Result.Content.ReadAsStreamAsync();
                var result = await JsonSerializer.DeserializeAsync<List<string>>(reponseStream, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });

                Sources = result;
            }
            else
            {
                ErrorMessageViewModel.Message = "An error was thrown. Please contact the system admin.";
                _logger.Error("loadGridLists.Source threw an error for {CurrentUser}" + response.Result.StatusCode.ToString(), Authentication.UserName);
            }

            api = _config.APIS.Where(x => x.Name == "CEPEnrExcl").FirstOrDefault();
            WebAPIConsume.BaseURI = api.BaseUrl;
            response = WebAPIConsume.GetCall(api.Url);
            if (response.Result.StatusCode == System.Net.HttpStatusCode.OK)
            {
                var reponseStream = await response.Result.Content.ReadAsStreamAsync();
                var result = await JsonSerializer.DeserializeAsync<List<string>>(reponseStream, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });

                CEPEnrExcl = result;
            }
            else
            {
                ErrorMessageViewModel.Message = "An error was thrown. Please contact the system admin.";
                _logger.Error("loadGridLists.CEPEnrExcl threw an error for {CurrentUser}" + response.Result.StatusCode.ToString(), Authentication.UserName);
            }


            if (SharedObjects.ProcCodes == null)
            {
                api = _config.APIS.Where(x => x.Name == "ProcCodes").FirstOrDefault();
                WebAPIConsume.BaseURI = api.BaseUrl;
                response = WebAPIConsume.GetCall(api.Url);
                if (response.Result.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    var reponseStream = await response.Result.Content.ReadAsStreamAsync();
                    var result = await JsonSerializer.DeserializeAsync<List<ProcCodesModel>>(reponseStream, new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true
                    });

                    SharedObjects.ProcCodes = result;
                    ProcCodes = SharedObjects.ProcCodes.Select(x => x.Proc_Cd_Full).ToList();
                }
                else
                {
                    ErrorMessageViewModel.Message = "An error was thrown. Please contact the system admin.";
                    _logger.Error("loadGridLists.ProcCodes threw an error for {CurrentUser}" + response.Result.StatusCode.ToString(), Authentication.UserName);
                }
            }


        }
        catch (Exception ex)
        {
            _logger.Fatal(ex, "loadGridLists.WebAPIConsume.GetCall threw an error for {CurrentUser}", Authentication.UserName);
        }

        

    }


    private IChemotherapyPXConfig prepareConfig(IConfiguration config)
    {

        var project = "ChemotherapyPX";
        var section = "Projects";

        ///EXTRACT IConfiguration INTO ETGFactSymmetryConfig 
        var cfg = config.GetSection(section).Get<List<ChemotherapyPXConfig>>();
        IChemotherapyPXConfig ecs = new ChemotherapyPXConfig();
        if (cfg == null)
        {
            return null;
            //throw new OperationCanceledException();
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
