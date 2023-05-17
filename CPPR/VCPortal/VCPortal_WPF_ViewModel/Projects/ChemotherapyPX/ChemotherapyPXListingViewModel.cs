using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using DocumentFormat.OpenXml.Spreadsheet;
using FileParsingLibrary.Models;
using FileParsingLibrary.MSExcel;
using Microsoft.Extensions.Configuration;
using SharedFunctionsLibrary;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Windows.Input;
using System.Windows.Media;
using VCPortal_Models.Configuration.HeaderInterfaces.Abstract;
using VCPortal_Models.Configuration.HeaderInterfaces.Concrete;
using VCPortal_Models.Dtos.ChemoPx;
using VCPortal_Models.Models.ChemoPx;
using VCPortal_Models.Models.ETGFactSymmetry;
using VCPortal_Models.Models.Shared;
using VCPortal_WPF_ViewModel.Projects.ETGFactSymmetry;
using VCPortal_WPF_ViewModel.Shared;

namespace VCPortal_WPF_ViewModel.Projects.ChemotherapyPX;
public partial class ChemotherapyPXListingViewModel : ObservableObject
{
    private readonly IExcelFunctions _excelFunctions;
    private readonly IChemotherapyPXConfig? _config;
    private readonly Serilog.ILogger _logger;

    private readonly BackgroundWorker worker = new BackgroundWorker();


    public MessageViewModel UserMessageViewModel { get; }
    public MessageViewModel ProgressMessageViewModel{ get; }

    [ObservableProperty]
    private bool canSave;

    [ObservableProperty]
    private bool isValid;

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

    private StringBuilder _sbStatus;


    public ChemotherapyPXListingViewModel(IConfiguration config, IExcelFunctions excelFunctions, Serilog.ILogger logger)
    {

        _logger = logger;
        _excelFunctions = excelFunctions;
        _config = prepareConfig(config);

        IsValid = true;

        UserMessageViewModel = new MessageViewModel();
        ProgressMessageViewModel= new MessageViewModel();

        //ProgressMessageViewModel.HasMessage = true;

        SharedChemoObjects.ChemotherapyPX_Tracking_List = new ObservableCollection<ChemotherapyPX_Tracking_CUD_Dto>();
        SharedChemoObjects.ChemotherapyPX_Tracking_List.CollectionChanged += listChanged;

        worker.DoWork += worker_DoWork;
        worker.RunWorkerCompleted += worker_RunWorkerCompleted;
        _sbStatus = new StringBuilder();

        OC_ChemotherapyPXViewModel = new ObservableCollection<ChemotherapyPXViewModel>();

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
            _logger.Error($"No Config found for ChemotherapyPX");
        }

    }

    private async void InitialLoadData()
    {
        _sbStatus.Clear();
        Mouse.OverrideCursor = Cursors.Wait;
        UserMessageViewModel.Message = "";
        ProgressMessageViewModel.Message = "";
        ProgressMessageViewModel.HasMessage = true;
        await loadGridLists();
        await getChemotherapyPXData();
        Mouse.OverrideCursor = null;
        ProgressMessageViewModel.HasMessage = false;
    }

    [RelayCommand]
    private async Task getChemotherapyPXDataCall()
    {
        _sbStatus.Clear();
        Mouse.OverrideCursor = Cursors.Wait;
        UserMessageViewModel.Message = "";
        ProgressMessageViewModel.Message = "";
        ProgressMessageViewModel.HasMessage = true;
        await getChemotherapyPXData();
        Mouse.OverrideCursor = null;
        ProgressMessageViewModel.HasMessage = false;
    }

    [RelayCommand]
    private async Task ExportDataCall()
    {
        //ProgressMessageViewModel.HasMessage = true;
        UserMessageViewModel.Message = "";
        Mouse.OverrideCursor = Cursors.Wait;
        await Task.Run(() => worker.RunWorkerAsync("ExportData"));
        Mouse.OverrideCursor = null;
    }

    [RelayCommand]
    private async Task SaveCall()
    {
        //ProgressMessageViewModel.HasMessage = true;
        //worker.RunWorkerAsync("SaveData");
        save();
    }


    [RelayCommand]
    private async Task EditEndCall()
    {
        IsValid = isThisValid();
        CanSave = IsValid;

        //foreach (var t in SharedChemoObjects.ChemotherapyPX_Tracking_List)
        //{
        //    if (t.IsValid == false)
        //    {
        //        //UserMessageViewModel.IsError = true;
        //        //UserMessageViewModel.Message = "Data is invalid. Please update before saving.";
        //        CanSave = false;
        //        return;
        //    }
        //}
        //CanSave = true;
    }


    private void worker_DoWork(object sender, DoWorkEventArgs e)
    {
        _sbStatus.Clear();
        UserMessageViewModel.Message = "";
        ProgressMessageViewModel.Message = "";
        ProgressMessageViewModel.HasMessage = true;

        var callingFunction = (string)e.Argument;
        if (callingFunction == "ExportData")
        {
            ProgressMessageViewModel.HasMessage = true;
            exportData();

        }
        //else if (callingFunction == "LoadData")
        //{
        //    ProgressMessageViewModel.HasMessage = true;
        //    getChemotherapyPXData();

        //}
        //else if (callingFunction == "InitialLoadData") 
        //{
        //    ProgressMessageViewModel.HasMessage = true;
        //    loadGridLists();
        //    getChemotherapyPXData();

        //}
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
        //CanSave = true;
    }


    [RelayCommand]
    private void addNewRow()
    {
        IsValid = isThisValid();
        if (!IsValid)
        {
            UserMessageViewModel.IsError = true;
            UserMessageViewModel.Message = "Data is invalid. Please update before adding new row.";
            return;
        }

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

            //NEVER ADDED TO DB SO WERE DONE !
            if(row.CODE == null)
            {
                return;
            }

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
            UserMessageViewModel.IsError = true;
            UserMessageViewModel.Message = "An error was thrown. Please contact the system admin.";
            _logger.Fatal(ex, "ChemotherapyPXData.deleteRow threw an error for {CurrentUser}", Authentication.UserName);
        }

    }

   // [RelayCommand]
    private void save()
    {
        try
        {

            IsValid = isThisValid();
            if (!IsValid)
            { 
                UserMessageViewModel.IsError = true;
                UserMessageViewModel.Message = "Data is invalid. Please update before saving.";
                return;
            }
          

            //ValidationContext context = new ValidationContext(OC_ChemotherapyPXViewModel, null, null);
            //List<ValidationResult> validationResults = new List<ValidationResult>();
            //bool valid = Validator.TryValidateObject(OC_ChemotherapyPXViewModel, context, validationResults, true);
            //if (!valid)
            //{
            //    foreach (ValidationResult validationResult in validationResults)
            //    {
            //        Console.WriteLine("{0}", validationResult.ErrorMessage);
            //    }
            //}





            SharedChemoObjects.ChemotherapyPX_Tracking_List.Remove(x => x.CODE == null);


            if (SharedChemoObjects.ChemotherapyPX_Tracking_List.Count == 0)
            {
                UserMessageViewModel.Message = "No changes to save";
                return;
            }

            _logger.Information("Running ChemotherapyPXData.Save for {CurrentUser}...", Authentication.UserName);
            var tracked = SharedChemoObjects.ChemotherapyPX_Tracking_List;
            var date = DateTime.Now;
            foreach (var t in tracked)
            {
                t.UPDATE_DT = date;
                t.UPDATE_USER = "cgiorda";
            }


            //HORRIBLE HACK FIX THIS!!!!!!!
            //HORRIBLE HACK FIX THIS!!!!!!!
            foreach (var t in tracked)
            {
                if (t.FIRST_NOVEL_MNTH == 0)
                    t.FIRST_NOVEL_MNTH = null;
            }

            var api = _config.APIS.Where(x => x.Name == "MainData").FirstOrDefault();
            WebAPIConsume.BaseURI = api.BaseUrl;
            var response = WebAPIConsume.PostCall<ObservableCollection<ChemotherapyPX_Tracking_CUD_Dto>>(api.Url, tracked);
            if (response.Result.StatusCode != System.Net.HttpStatusCode.OK)
            {

                UserMessageViewModel.IsError = true;
                UserMessageViewModel.Message = "An error was thrown. Please contact the system admin.";
                _logger.Error("ChemotherapyPXData.Save threw an error for {CurrentUser}" + response.Result.StatusCode.ToString(), Authentication.UserName);
            }
            else
            {

                UserMessageViewModel.IsError = false;
                UserMessageViewModel.Message = "ChemotherapyPXData.Save sucessfully completed";
                _logger.Information("ChemotherapyPXData.Save sucessfully completed for {CurrentUser}...", Authentication.UserName);
                CanSave = false;
            }

            try
            {
                SharedChemoObjects.ChemotherapyPX_Tracking_List.Clear();
                ProcCodes = SharedObjects.ProcCodes.Select(x => x.Proc_Cd_Full).ToList();
            }
            catch (Exception ex)
            {
                UserMessageViewModel.IsError = true;
                UserMessageViewModel.Message = "An error was thrown. Please contact the system admin.";
                _logger.Error("ChemotherapyPXData.Save.ProcCodes threw an error for {CurrentUser}", Authentication.UserName);
            }
        }
        catch (Exception ex)
        {
            UserMessageViewModel.IsError = true;
            UserMessageViewModel.Message = "An error was thrown. Please contact the system admin.";
            _logger.Fatal(ex, "ChemotherapyPXData.save threw an error for {CurrentUser}", Authentication.UserName);
        }
        //finally
        //{
        //    CanSave = false;
        //}
    }


    private async Task getChemotherapyPXData()
    {
        try
        {

            OC_ChemotherapyPXViewModel.Clear();

            _logger.Information("Running getChemotherapyPXData() for {CurrentUser}...", Authentication.UserName);
            _sbStatus.Append("--Requesting data for ChemotherapyPX, please wait..." + Environment.NewLine);
            ProgressMessageViewModel.Message = _sbStatus.ToString();
            await Task.Delay(TimeSpan.FromSeconds(1));
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
                _sbStatus.Append("--Retrieving row {$cnt} out of " + total.ToString("N0") + Environment.NewLine);
                //result.ForEach(x => 
                //    {
                //        ProgressMessageViewModel.Message = _sbStatus.ToString().Replace("{$cnt}", cnt.ToString("N0"));
                //        OC_ChemotherapyPXViewModel.Add(new ChemotherapyPXViewModel(x));
                //        cnt++;
                //    });

                foreach (var r in result)
                {
                    ProgressMessageViewModel.Message = _sbStatus.ToString().Replace("{$cnt}", cnt.ToString("N0"));
                    OC_ChemotherapyPXViewModel.Add(new ChemotherapyPXViewModel(r));
                    //await Task.Delay(TimeSpan.FromSeconds(.001));
                    if (cnt % 10 == 0)
                    {
                        await Task.Delay(TimeSpan.FromSeconds(.001));
                    }
                    cnt++;
                }

                _logger.Information("getChemotherapyPXData sucessfully completed for {CurrentUser}...", Authentication.UserName);
                _sbStatus.Append("--Rendering grid..." + Environment.NewLine);
                ProgressMessageViewModel.Message = _sbStatus.ToString();

            }
            else
            {
                UserMessageViewModel.IsError = true;
                UserMessageViewModel.Message = "An error was thrown. Please contact the system admin.";
                await Task.Delay(TimeSpan.FromSeconds(1));
                _logger.Error("getChemotherapyPXData threw an error for {CurrentUser}..." + response.Result.StatusCode.ToString(), Authentication.UserName);
            }

            //FIND WAY TO IGNORE LOADING THESE WHENEVER REFRESHED
            //DONT NEED TO TRACK LOADING OF DATA!!!!!
            SharedChemoObjects.ChemotherapyPX_Tracking_List.Clear();

            //SelectedRow = OC_ChemotherapyPXViewModel[0];
        }
        catch (Exception ex)
        {
            UserMessageViewModel.IsError = true;
            UserMessageViewModel.Message = "An error was thrown. Please contact the system admin.";
            await Task.Delay(TimeSpan.FromSeconds(1));
            _logger.Fatal(ex, "getChemotherapyPXData.WebAPIConsume.GetCall threw an error for {CurrentUser}", Authentication.UserName);
        }
        finally
        {
            CanSave = false;
        }

    }

    private async Task exportData()
    {

        try
        {

            _logger.Information("Running ChemotherapyPXData.exportConfigs for {CurrentUser}...", Authentication.UserName);


            var file = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + "\\" + "tmp.xlsx";

            List<ExcelExport> export = new List<ExcelExport>();

            export.Add(new ExcelExport() { ExportList = OC_ChemotherapyPXViewModel.ToList<object>(), SheetName = "ChemotherapyPX_Data" });

            var api = _config.APIS.Where(x => x.Name == "Tracking").FirstOrDefault();
            var tracking = await VM_Functions.APIGetResultAsync<ChemotherapyPX_Tracking_ReadDto>(api.BaseUrl, api.Url);
            if (tracking.Count > 0)
            {
                export.Add(new ExcelExport() { ExportList = tracking.ToList<object>(), SheetName = "Tracking"});
            }


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

            _logger.Information("ChemotherapyPXData.exportConfigs sucessfully completed for {CurrentUser}...", Authentication.UserName);
        }
        catch (Exception ex)
        {
            UserMessageViewModel.IsError = true;
            UserMessageViewModel.Message = "An error was thrown. Please contact the system admin.";
            _logger.Fatal(ex, "ChemotherapyPXData.exportConfigs threw an error for {CurrentUser}", Authentication.UserName);
        }
    }

    private async Task loadGridLists()
    {
        try 
        {
            var api = _config.APIS.Where(x => x.Name == "CodeCategory").FirstOrDefault();
            WebAPIConsume.BaseURI = api.BaseUrl;
            _sbStatus.Append("--Getting CodeCategory list..." + Environment.NewLine);
            ProgressMessageViewModel.Message = _sbStatus.ToString();
            await Task.Delay(TimeSpan.FromSeconds(.5));
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
                UserMessageViewModel.IsError = true;
                UserMessageViewModel.Message = "An error was thrown. Please contact the system admin.";
                _logger.Error("loadGridLists.CodeCategory threw an error for {CurrentUser}" + response.Result.StatusCode.ToString(), Authentication.UserName);
            }


            api = _config.APIS.Where(x => x.Name == "AspCategory").FirstOrDefault();
            WebAPIConsume.BaseURI = api.BaseUrl;
            _sbStatus.Append("--Getting AspCategory list..." + Environment.NewLine);
            ProgressMessageViewModel.Message = _sbStatus.ToString();
            await Task.Delay(TimeSpan.FromSeconds(.5));
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
                UserMessageViewModel.IsError = true;
                UserMessageViewModel.Message = "An error was thrown. Please contact the system admin.";
                _logger.Error("loadGridLists.AspCategory threw an error for {CurrentUser}" + response.Result.StatusCode.ToString(), Authentication.UserName);
            }

            api = _config.APIS.Where(x => x.Name == "DrugAdmMode").FirstOrDefault();
            WebAPIConsume.BaseURI = api.BaseUrl;
            _sbStatus.Append("--Getting DrugAdmMode list..." + Environment.NewLine);
            ProgressMessageViewModel.Message = _sbStatus.ToString();
            await Task.Delay(TimeSpan.FromSeconds(.5));
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
                UserMessageViewModel.IsError = true;
                UserMessageViewModel.Message = "An error was thrown. Please contact the system admin.";
                _logger.Error("loadGridLists.DrugAdmMode threw an error for {CurrentUser}" + response.Result.StatusCode.ToString(), Authentication.UserName);
            }

            api = _config.APIS.Where(x => x.Name == "PADrugs").FirstOrDefault();
            WebAPIConsume.BaseURI = api.BaseUrl;
            _sbStatus.Append("--Getting PADrugs list..." + Environment.NewLine);
            ProgressMessageViewModel.Message = _sbStatus.ToString();
            await Task.Delay(TimeSpan.FromSeconds(.5));
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
                UserMessageViewModel.IsError = true;
                UserMessageViewModel.Message = "An error was thrown. Please contact the system admin.";
                _logger.Error("loadGridLists.PADrugs threw an error for {CurrentUser}" + response.Result.StatusCode.ToString(), Authentication.UserName);
            }

            api = _config.APIS.Where(x => x.Name == "CEPPayCd").FirstOrDefault();
            WebAPIConsume.BaseURI = api.BaseUrl;
            _sbStatus.Append("--Getting CEPPayCd list..." + Environment.NewLine);
            ProgressMessageViewModel.Message = _sbStatus.ToString();
            await Task.Delay(TimeSpan.FromSeconds(.5));
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
                UserMessageViewModel.Message = "An error was thrown. Please contact the system admin.";
                _logger.Error("loadGridLists.CEPPayCd threw an error for {CurrentUser}" + response.Result.StatusCode.ToString(), Authentication.UserName);
            }

            api = _config.APIS.Where(x => x.Name == "CEPEnrollCd").FirstOrDefault();
            WebAPIConsume.BaseURI = api.BaseUrl;
            _sbStatus.Append("--Getting CEPEnrollCd list..." + Environment.NewLine);
            ProgressMessageViewModel.Message = _sbStatus.ToString();
            await Task.Delay(TimeSpan.FromSeconds(.5));
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
                UserMessageViewModel.IsError = true;
                UserMessageViewModel.Message = "An error was thrown. Please contact the system admin.";
                _logger.Error("loadGridLists.CEPEnrollCd threw an error for {CurrentUser}" + response.Result.StatusCode.ToString(), Authentication.UserName);
            }

            api = _config.APIS.Where(x => x.Name == "Source").FirstOrDefault();
            WebAPIConsume.BaseURI = api.BaseUrl;
            _sbStatus.Append("--Getting Source list..." + Environment.NewLine);
            ProgressMessageViewModel.Message = _sbStatus.ToString();
            await Task.Delay(TimeSpan.FromSeconds(.5));
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
                UserMessageViewModel.IsError = true;
                UserMessageViewModel.Message = "An error was thrown. Please contact the system admin.";
                _logger.Error("loadGridLists.Source threw an error for {CurrentUser}" + response.Result.StatusCode.ToString(), Authentication.UserName);
            }

            api = _config.APIS.Where(x => x.Name == "CEPEnrExcl").FirstOrDefault();
            WebAPIConsume.BaseURI = api.BaseUrl;
            _sbStatus.Append("--Getting CEPEnrExcl list..." + Environment.NewLine);
            ProgressMessageViewModel.Message = _sbStatus.ToString();
            await Task.Delay(TimeSpan.FromSeconds(.5));
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
                UserMessageViewModel.IsError = true;
                UserMessageViewModel.Message = "An error was thrown. Please contact the system admin.";
                _logger.Error("loadGridLists.CEPEnrExcl threw an error for {CurrentUser}" + response.Result.StatusCode.ToString(), Authentication.UserName);
            }


            if (SharedObjects.ProcCodes == null)
            {
                api = _config.APIS.Where(x => x.Name == "ProcCodes").FirstOrDefault();
                WebAPIConsume.BaseURI = api.BaseUrl;
                _sbStatus.Append("--Getting ProcCodes list..." + Environment.NewLine);
                ProgressMessageViewModel.Message = _sbStatus.ToString();
                await Task.Delay(TimeSpan.FromSeconds(.5));
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
                    UserMessageViewModel.IsError = true;
                    UserMessageViewModel.Message = "An error was thrown. Please contact the system admin.";
                    _logger.Error("loadGridLists.ProcCodes threw an error for {CurrentUser}" + response.Result.StatusCode.ToString(), Authentication.UserName);
                }
            }


        }
        catch (Exception ex)
        {
            UserMessageViewModel.IsError = true;
            UserMessageViewModel.Message = "An error was thrown. Please contact the system admin.";
            _logger.Fatal(ex, "loadGridLists.WebAPIConsume.GetCall threw an error for {CurrentUser}", Authentication.UserName);
        }

    }


    private bool isThisValid()
    {
        foreach (var t in SharedChemoObjects.ChemotherapyPX_Tracking_List)
        {
            if (t.IsValid == false)
            {
                return false;
            }
        }
        return true;
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
