using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using DataAccessLibrary.Data.Abstract;
using DataAccessLibrary.DataAccess;
using FileParsingLibrary.MSExcel;
using FileParsingLibrary.MSExcel.Custom.MHP;
using IdentityModel.OidcClient;
using Irony.Parsing;
using MathNet.Numerics;
using MathNet.Numerics.Providers.SparseSolver;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json.Linq;
using NPOI.SS.Formula.Functions;
using SharedFunctionsLibrary;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
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


    private readonly IMHPUniverse_Repo _mhp_sql;




    private StringBuilder _sbStatus;
    private List<MHP_Reporting_Filters> _mhpReportingFilters { get; set; }

    private List<MHP_Group_State_Model> _mhpGroupState { get; set; }

    private readonly BackgroundWorker worker = new BackgroundWorker();

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


    [ObservableProperty]
    public string _startDate;
    [ObservableProperty]
    public string _endDate;


    public MHPViewModel(IConfiguration config, IExcelFunctions excelFunctions, Serilog.ILogger logger, DBRepoModel dBRepo)
    {
        _logger = logger;
        _excelFunctions = excelFunctions;
        _config = prepareConfig(config);


        _mhp_sql = dBRepo.mhp_sql;





        UserMessageViewModel = new MessageViewModel();
        ProgressMessageViewModel = new MessageViewModel();

        worker.DoWork += worker_DoWork;
        worker.RunWorkerCompleted += worker_RunWorkerCompleted;


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

    private void worker_DoWork(object sender, DoWorkEventArgs e)
    {
        var callingFunction = (string)e.Argument;

        _sbStatus.Clear();
        UserMessageViewModel.Message = "";
        ProgressMessageViewModel.Message = "";
        ProgressMessageViewModel.HasMessage = true;

        if (callingFunction == "GenerateEIReport")
        {
            ProgressMessageViewModel.HasMessage = true;
            GenerateEIReport();
        }
        else if (callingFunction == "GenerateCSReport")
        {
            ProgressMessageViewModel.HasMessage = true;
            GenerateCSReport();
        }
        else if (callingFunction == "GenerateIFPReport")
        {
            ProgressMessageViewModel.HasMessage = true;
            GenerateIFPReport();
        }
        //else if (callingFunction == "LoadData")
        //{
        //    ProgressMessageViewModel.HasMessage = true;
        //    getETGFactSymmetryData();

        //}
        //else if (callingFunction == "InitialLoadData")
        //{
        //    ProgressMessageViewModel.HasMessage = true;
        //    loadGridLists();
        //    getETGFactSymmetryData();

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


    private object _params;

    [RelayCommand]
    private async Task GenerateEIReportCall(object item)
    {
        _params = item;

        UserMessageViewModel.Message = "";
        Mouse.OverrideCursor = Cursors.Wait;
        ProgressMessageViewModel.Message = "";
        ProgressMessageViewModel.HasMessage = true;
        await Task.Run(() => worker.RunWorkerAsync("GenerateEIReport"));
        Mouse.OverrideCursor = null;

    }

    [RelayCommand]
    private async Task GenerateIFPReportCall(object item)
    {
        _params = item;

        UserMessageViewModel.Message = "";
        Mouse.OverrideCursor = Cursors.Wait;
        ProgressMessageViewModel.Message = "";
        ProgressMessageViewModel.HasMessage = true;
        await Task.Run(() => worker.RunWorkerAsync("GenerateIFPReport"));
        Mouse.OverrideCursor = null;

    }

    [RelayCommand]
    private async Task GenerateCSReportCall(object item)
    {
        _params = item;

        UserMessageViewModel.Message = "";
        Mouse.OverrideCursor = Cursors.Wait;
        ProgressMessageViewModel.Message = "";
        ProgressMessageViewModel.HasMessage = true;
        await Task.Run(() => worker.RunWorkerAsync("GenerateCSReport"));
        Mouse.OverrideCursor = null;

    }


    private async Task GenerateEIReport()
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


        CancellationTokenSource cancellationToken;
        cancellationToken = new CancellationTokenSource();


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




            //var api = _config.APIS.Where(x => x.Name == "MHP_EI").FirstOrDefault();
            //WebAPIConsume.BaseURI = api.BaseUrl;
            //var response = await WebAPIConsume.PostCall<MHP_EI_Parameters>(api.Url, ei_param);
            //if (response.StatusCode != System.Net.HttpStatusCode.OK)
            //{

            //    UserMessageViewModel.IsError = true;
            //    UserMessageViewModel.Message = "An error was thrown. Please contact the system admin.";
            //    _logger.Error("MHP EI Report threw an error for {CurrentUser}" + response.StatusCode.ToString(), Authentication.UserName);
            //    return;
            //}
            //else
            //{

            //    var reponseStream = await response.Content.ReadAsStreamAsync();
            //    var result = await JsonSerializer.DeserializeAsync<List<MHP_EI_Model>>(reponseStream, new JsonSerializerOptions
            //    {
            //        PropertyNameCaseInsensitive = true
            //    });

            //    mhp_final = result;

            //}


            try
            {
                _sbStatus.Append("--Retreiving EI summary data from Database" + Environment.NewLine);
                ProgressMessageViewModel.Message = _sbStatus.ToString();


                ////RETURN HTTP 200
                var task =  _mhp_sql.GetMHP_EI_Async(ei_param.State, ei_param.StartDate, ei_param.EndDate, ei_param.Finc_Arng_Desc, ei_param.Mkt_Seg_Rllp_Desc, ei_param.LegalEntities, ei_param.Mkt_Typ_Desc, ei_param.Cust_Seg, cancellationToken.Token);//200 SUCCESS

                task.Wait(); // Blocks current thread until GetFooAsync task completes
                 // For pedagogical use only: in general, don't do this!
                var results = task.Result;

                if (results != null)
                {


                    mhp_final = results.ToList();

                }
                else
                {
                    _logger.Warning("API GetMHP_EI_Async 404, not found");
                    return;
                }
               

            }
            catch (Exception ex)
            {

                _logger.Error(ex, "API GetMHP_EI_Async threw an error");
                //RETURN ERROR
                // return Results.Problem(ex.Message);
                return;

            }


            ////EI ALL SUMMARY
            _sbStatus.Append("--Retreiving EI summary all data from Database" + Environment.NewLine);
            ProgressMessageViewModel.Message = _sbStatus.ToString();

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

            try
            {
                ////RETURN HTTP 200
                var task =  _mhp_sql.GetMHP_EI_ALL_Async(ei_param_all.State, ei_param_all.StartDate, ei_param_all.EndDate, ei_param_all.Finc_Arng_Desc, ei_param_all.Mkt_Seg_Rllp_Desc, ei_param_all.LegalEntities, ei_param_all.Mkt_Typ_Desc, ei_param_all.Cust_Seg, cancellationToken.Token);//200 SUCCESS

                task.Wait(); // Blocks current thread until GetFooAsync task completes
                             // For pedagogical use only: in general, don't do this!
                var results = task.Result;


                if (results != null)
                {
                    mhp_final_all = results.ToList();

                }
                else
                {
                    _logger.Warning("API GetMHP_EI_Async 404, not found");
                    return;
                }


            }
            catch (Exception ex)
            {

                _logger.Error(ex, "API GetMHP_EI_Async threw an error");
                //RETURN ERROR
                // return Results.Problem(ex.Message);
                return;
            }


            _sbStatus.Append("--Retreiving EI details data from Database" + Environment.NewLine);
            ProgressMessageViewModel.Message = _sbStatus.ToString();

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
            try
            {
                ////RETURN HTTP 200
                var task =  _mhp_sql.GetMHPEIDetailsAsync(ei_param.State, ei_param.StartDate, ei_param.EndDate, ei_param.Finc_Arng_Desc, ei_param.Mkt_Seg_Rllp_Desc, ei_param.LegalEntities, ei_param.Mkt_Typ_Desc, ei_param.Cust_Seg, cancellationToken.Token);//200 SUCCESS

                task.Wait(); // Blocks current thread until GetFooAsync task completes
                             // For pedagogical use only: in general, don't do this!
                var results = task.Result;


                if (results != null)
                {
                    mhp_details_final = results.ToList();

                }
                else
                {
                    
                    _logger.Warning("API GetMHP_EI_Async 404, not found");
                    return;
                }


            }
            catch (Exception ex)
            {

                _logger.Error(ex, "API GetMHP_EI_Async threw an error");
                //RETURN ERROR
                // return Results.Problem(ex.Message);
                return;
            }


            //NOT NEEDED!!!
            //_sbStatus.Append("--Retreiving All EI details data from Database" + Environment.NewLine);
            //ProgressMessageViewModel.Message = _sbStatus.ToString();

            //api = _config.APIS.Where(x => x.Name == "MHP_EI_Details_All").FirstOrDefault();
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
            //    var result = await JsonSerializer.DeserializeAsync<List<MHPEIDetails_Model>>(reponseStream, new JsonSerializerOptions
            //    {
            //        PropertyNameCaseInsensitive = true
            //    });

            //    mhp_details_final_all = result;


            //}







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
            UserMessageViewModel.Message = "MHP EI Report sucessfully generated";
            _logger.Information("MHP EI Report sucessfully generated for {CurrentUser}...", Authentication.UserName);

        }
        catch (Exception ex)
        {
            UserMessageViewModel.IsError = true;
            UserMessageViewModel.Message = "An error was thrown. Please contact the system admin.";
            _logger.Fatal(ex, "MHP EI Report threw an error for {CurrentUser}", Authentication.UserName);
        }
        finally
        {
            ProgressMessageViewModel.HasMessage = false;
        }


    }

    private async Task GenerateCSReport()
    {



        _logger.Information("Running MHP.GenerateCSReport for {CurrentUser}...", Authentication.UserName);

        _sbStatus.Append("--Processing selected filters for CS" + Environment.NewLine);
        ProgressMessageViewModel.Message = _sbStatus.ToString();

        object[] parameters = _params as object[];

        MHP_CS_Parameters cs_param = new MHP_CS_Parameters();


        List<MHP_CS_Model> mhp_final;
        List<MHPCSDetails_Model> mhp_details_final;

        CancellationTokenSource cancellationToken;
        cancellationToken = new CancellationTokenSource();


        try
        {
            cs_param.State = "'" + String.Join(",", parameters[0].ToString().Replace("--All--,", "")).Replace(",", "', '") + "'";
            cs_param.StartDate = DateTime.Parse(parameters[1].ToString()).ToShortDateString();
            cs_param.EndDate = DateTime.Parse(parameters[2].ToString()).ToShortDateString();
            cs_param.CS_Tadm_Prdct_Map = "'" + String.Join(",", parameters[3].ToString().Replace("--All--,", "")).Replace(",", "', '") + "'";

            if (parameters[4] != "")
            {
                cs_param.GroupNumbers = "'" + String.Join(",", parameters[4].ToString().Replace("--All--,", "")).Replace(",", "', '") + "'";
            }





            //var api = _config.APIS.Where(x => x.Name == "MHP_CS").FirstOrDefault();
            //WebAPIConsume.BaseURI = api.BaseUrl;
            //var response = await WebAPIConsume.PostCall<MHP_CS_Parameters>(api.Url, cs_param);
            //if (response.StatusCode != System.Net.HttpStatusCode.OK)
            //{

            //    UserMessageViewModel.IsError = true;
            //    UserMessageViewModel.Message = "An error was thrown. Please contact the system admin.";
            //    _logger.Error("MHP CS Report threw an error for {CurrentUser}" + response.StatusCode.ToString(), Authentication.UserName);
            //    return;
            //}
            //else
            //{

            //    var reponseStream = await response.Content.ReadAsStreamAsync();
            //    var result = await JsonSerializer.DeserializeAsync<List<MHP_CS_Model>>(reponseStream, new JsonSerializerOptions
            //    {
            //        PropertyNameCaseInsensitive = true
            //    });

            //    mhp_final = result;

            //}

            try
            {
                _sbStatus.Append("--Retreiving CS summary data from Database" + Environment.NewLine);
                ProgressMessageViewModel.Message = _sbStatus.ToString();


                ////RETURN HTTP 200
                var task = _mhp_sql.GetMHP_CS_Async(cs_param.State, cs_param.StartDate, cs_param.EndDate, cs_param.CS_Tadm_Prdct_Map, cs_param.GroupNumbers, cancellationToken.Token) ;//200 SUCCESS

                task.Wait(); // Blocks current thread until GetFooAsync task completes
                             // For pedagogical use only: in general, don't do this!
                var results = task.Result;

                if (results != null)
                {


                    mhp_final = results.ToList();

                }
                else
                {
                    _logger.Warning("API GetMHP_EI_Async 404, not found");
                    return;
                }


            }
            catch (Exception ex)
            {

                _logger.Error(ex, "API GetMHP_EI_Async threw an error");
                //RETURN ERROR
                // return Results.Problem(ex.Message);
                return;

            }




            //api = _config.APIS.Where(x => x.Name == "MHP_CS_Details").FirstOrDefault();
            //WebAPIConsume.BaseURI = api.BaseUrl;
            //response = await WebAPIConsume.PostCall<MHP_CS_Parameters>(api.Url, cs_param);
            //if (response.StatusCode != System.Net.HttpStatusCode.OK)
            //{

            //    UserMessageViewModel.IsError = true;
            //    UserMessageViewModel.Message = "An error was thrown. Please contact the system admin.";
            //    _logger.Error("MHP CS Report details threw an error for {CurrentUser}" + response.StatusCode.ToString(), Authentication.UserName);
            //    return;
            //}
            //else
            //{

            //    var reponseStream = await response.Content.ReadAsStreamAsync();
            //    var result = await JsonSerializer.DeserializeAsync<List<MHPCSDetails_Model>>(reponseStream, new JsonSerializerOptions
            //    {
            //        PropertyNameCaseInsensitive = true
            //    });

            //    mhp_details_final = result;


            //}

            try
            {

                _sbStatus.Append("--Retreiving CS details data from Database" + Environment.NewLine);
                ProgressMessageViewModel.Message = _sbStatus.ToString();


                ////RETURN HTTP 200
                var task = _mhp_sql.GetMHPCSDetailsAsync(cs_param.State, cs_param.StartDate, cs_param.EndDate, cs_param.CS_Tadm_Prdct_Map, cs_param.GroupNumbers, cancellationToken.Token);//200 SUCCESS

                task.Wait(); // Blocks current thread until GetFooAsync task completes
                             // For pedagogical use only: in general, don't do this!
                var results = task.Result;

                if (results != null)
                {


                    mhp_details_final = results.ToList();

                }
                else
                {
                    _logger.Warning("API GetMHP_EI_Async 404, not found");
                    return;
                }


            }
            catch (Exception ex)
            {

                _logger.Error(ex, "API GetMHP_EI_Async threw an error");
                //RETURN ERROR
                // return Results.Problem(ex.Message);
                return;

            }



            var bytes = await MHPExcelExport.ExportCSToExcel(mhp_final, mhp_details_final, cs_param.CS_Tadm_Prdct_Map, () => ProgressMessageViewModel.Message, x => ProgressMessageViewModel.Message = x, cancellationToken.Token);

            var file = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + "\\MHP_CS_Report_" + DateTime.Now.ToString("yyyy-dd-M--HH-mm-ss") + ".xlsx";


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
            UserMessageViewModel.Message = "MHP CS Report sucessfully generated";
            _logger.Information("MHP CS Report sucessfully generated for {CurrentUser}...", Authentication.UserName);

        }
        catch (Exception ex)
        {
            UserMessageViewModel.IsError = true;
            UserMessageViewModel.Message = "An error was thrown. Please contact the system admin.";
            _logger.Fatal(ex, "MHP CS Report threw an error for {CurrentUser}", Authentication.UserName);
        }


    }

    private async Task GenerateIFPReport()
    {
        _logger.Information("Running MHP.GenerateIFPReport for {CurrentUser}...", Authentication.UserName);

        _sbStatus.Append("--Processing selected filters for IFP" + Environment.NewLine);
        ProgressMessageViewModel.Message = _sbStatus.ToString();

        object[] parameters = _params as object[];

        MHP_IFP_Parameters ifp_param = new MHP_IFP_Parameters();

        List<MHP_IFP_Model> mhp_final;
        List<MHPIFPDetails_Model> mhp_details_final;

        CancellationTokenSource cancellationToken;
        cancellationToken = new CancellationTokenSource();


        try
        {

            ifp_param.State = "'" + String.Join(",", parameters[0].ToString().Replace("--All--,", "")).Replace(",", "', '") + "'"; ;
            ifp_param.StartDate = DateTime.Parse(parameters[1].ToString()).ToShortDateString();
            ifp_param.EndDate = DateTime.Parse(parameters[2].ToString()).ToShortDateString();
            ifp_param.ProductCodes = (string.IsNullOrEmpty(parameters[3].ToString()) ? null : new List<string>(parameters[3].ToString().Replace("--All--,", "").Split(',')));
            for (int i = 0; i < ifp_param.ProductCodes.Count; i++)
            {
                ifp_param.ProductCodes[i] = ifp_param.ProductCodes[i].Split("-")[0].Trim();
            }





            //var api = _config.APIS.Where(x => x.Name == "MHP_IFP").FirstOrDefault();
            //WebAPIConsume.BaseURI = api.BaseUrl;
            //var response = await WebAPIConsume.PostCall<MHP_IFP_Parameters>(api.Url, ifp_param);
            //if (response.StatusCode != System.Net.HttpStatusCode.OK)
            //{

            //    UserMessageViewModel.IsError = true;
            //    UserMessageViewModel.Message = "An error was thrown. Please contact the system admin.";
            //    _logger.Error("MHP IFP Report threw an error for {CurrentUser}" + response.StatusCode.ToString(), Authentication.UserName);
            //    return;
            //}
            //else
            //{

            //    var reponseStream = await response.Content.ReadAsStreamAsync();
            //    var result = await JsonSerializer.DeserializeAsync<List<MHP_IFP_Model>>(reponseStream, new JsonSerializerOptions
            //    {
            //        PropertyNameCaseInsensitive = true
            //    });

            //    mhp_final = result;

            //}
            try
            {

                _sbStatus.Append("--Retreiving IFP summary data from Database" + Environment.NewLine);
                ProgressMessageViewModel.Message = _sbStatus.ToString();


                ////RETURN HTTP 200
                var task = _mhp_sql.GetMHP_IFP_Async(ifp_param.State, ifp_param.StartDate, ifp_param.EndDate, ifp_param.ProductCodes, cancellationToken.Token);//200 SUCCESS

                task.Wait(); // Blocks current thread until GetFooAsync task completes
                             // For pedagogical use only: in general, don't do this!
                var results = task.Result;

                if (results != null)
                {


                    mhp_final = results.ToList();

                }
                else
                {
                    _logger.Warning("API GetMHP_EI_Async 404, not found");
                    return;
                }


            }
            catch (Exception ex)
            {

                _logger.Error(ex, "API GetMHP_EI_Async threw an error");
                //RETURN ERROR
                // return Results.Problem(ex.Message);
                return;

            }



            //api = _config.APIS.Where(x => x.Name == "MHP_IFP_Details").FirstOrDefault();
            //WebAPIConsume.BaseURI = api.BaseUrl;
            //response = await WebAPIConsume.PostCall<MHP_IFP_Parameters>(api.Url, ifp_param);
            //if (response.StatusCode != System.Net.HttpStatusCode.OK)
            //{

            //    UserMessageViewModel.IsError = true;
            //    UserMessageViewModel.Message = "An error was thrown. Please contact the system admin.";
            //    _logger.Error("MHP IFP Report details threw an error for {CurrentUser}" + response.StatusCode.ToString(), Authentication.UserName);
            //    return;
            //}
            //else
            //{

            //    var reponseStream = await response.Content.ReadAsStreamAsync();
            //    var result = await JsonSerializer.DeserializeAsync<List<MHPIFPDetails_Model>>(reponseStream, new JsonSerializerOptions
            //    {
            //        PropertyNameCaseInsensitive = true
            //    });

            //    mhp_details_final = result;


            //}
            try
            {

                _sbStatus.Append("--Retreiving IFP details data from Database" + Environment.NewLine);
                ProgressMessageViewModel.Message = _sbStatus.ToString();


                ////RETURN HTTP 200
                var task = _mhp_sql.GetMHPIFPDetailsAsync(ifp_param.State, ifp_param.StartDate, ifp_param.EndDate, ifp_param.ProductCodes, cancellationToken.Token);//200 SUCCESS

                task.Wait(); // Blocks current thread until GetFooAsync task completes
                             // For pedagogical use only: in general, don't do this!
                var results = task.Result;

                if (results != null)
                {


                    mhp_details_final = results.ToList();

                }
                else
                {
                    _logger.Warning("API GetMHP_EI_Async 404, not found");
                    return;
                }


            }
            catch (Exception ex)
            {

                _logger.Error(ex, "API GetMHP_EI_Async threw an error");
                //RETURN ERROR
                // return Results.Problem(ex.Message);
                return;

            }
            var bytes = await MHPExcelExport.ExportIFPToExcel(mhp_final, mhp_details_final, () => ProgressMessageViewModel.Message, x => ProgressMessageViewModel.Message = x, cancellationToken.Token);

            var file = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + "\\MHP_IFP_Report_" + DateTime.Now.ToString("yyyy-dd-M--HH-mm-ss") + ".xlsx";


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
            UserMessageViewModel.Message = "MHP IFP Report sucessfully generated";
            _logger.Information("MHP IFP Report sucessfully generated for {CurrentUser}...", Authentication.UserName);

        }
        catch (Exception ex)
        {
            UserMessageViewModel.IsError = true;
            UserMessageViewModel.Message = "An error was thrown. Please contact the system admin.";
            _logger.Fatal(ex, "MHP IFP Report threw an error for {CurrentUser}", Authentication.UserName);
        }

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
            _startDate = "01/01/" + (DateTime.Now.Year - 1) ;
            _endDate = "12/31/" + (DateTime.Now.Year - 1);


            //var api = _config.APIS.Where(x => x.Name == "MHP_Filters").FirstOrDefault();
            //WebAPIConsume.BaseURI = api.BaseUrl;
            _sbStatus.Append("--Getting Cached Filters..." + Environment.NewLine);
            ProgressMessageViewModel.Message = _sbStatus.ToString();
            //await Task.Delay(TimeSpan.FromSeconds(1));
            //var response = WebAPIConsume.GetCall(api.Url);
            //if (response.Result.StatusCode == System.Net.HttpStatusCode.OK)
            //{
            //    var reponseStream = await response.Result.Content.ReadAsStreamAsync();
            //    var result = await JsonSerializer.DeserializeAsync<List<MHP_Reporting_Filters>>(reponseStream, new JsonSerializerOptions
            //    {
            //        PropertyNameCaseInsensitive = true
            //    });

            //    _mhpReportingFilters = result;
            //}
            //else
            //{
            //    UserMessageViewModel.IsError = true;
            //    UserMessageViewModel.Message = "An error was thrown. Please contact the system admin.";
            //    _logger.Error("populateFilters.MHP_Filters threw an error for {CurrentUser}" + response.Result.StatusCode.ToString(), Authentication.UserName);
            //}



            try
            {
                ////RETURN HTTP 200
                var results = await _mhp_sql.GetMHP_Filters_Async( CancellationToken.None);//200 SUCCESS

                if (results != null)
                {
                    _mhpReportingFilters = results.ToList();

                }
                else
                {
                    _logger.Warning("API GetMHP_Filters_Async 404, not found");
                }
              


            }
            catch (Exception ex)
            {

                _logger.Error(ex, "API GetMHP_Filters_Async threw an error");


            }






            //api = _config.APIS.Where(x => x.Name == "MHP_GroupState").FirstOrDefault();
            //WebAPIConsume.BaseURI = api.BaseUrl;
            _sbStatus.Append("--Getting Group/State Mapping..." + Environment.NewLine);
            ProgressMessageViewModel.Message = _sbStatus.ToString();
            //await Task.Delay(TimeSpan.FromSeconds(1));
            //response = WebAPIConsume.GetCall(api.Url);
            //if (response.Result.StatusCode == System.Net.HttpStatusCode.OK)
            //{
            //    var reponseStream = await response.Result.Content.ReadAsStreamAsync();
            //    var result = await JsonSerializer.DeserializeAsync<List<MHP_Group_State_Model>>(reponseStream, new JsonSerializerOptions
            //    {
            //        PropertyNameCaseInsensitive = true
            //    });

            //    _mhpGroupState = result;
            //}
            //else
            //{
            //    UserMessageViewModel.IsError = true;
            //    UserMessageViewModel.Message = "An error was thrown. Please contact the system admin.";
            //    _logger.Error("populateFilters.MHP_GroupState threw an error for {CurrentUser}" + response.Result.StatusCode.ToString(), Authentication.UserName);
            //}



            try
            {
                ////RETURN HTTP 200
                var results = await _mhp_sql.GetMHP_Group_State_Async(CancellationToken.None);//200 SUCCESS

                if (results != null)
                {
                    _mhpGroupState = results.ToList();

                }
                else
                {
                    _logger.Warning("API GetMHP_Filters_Async 404, not found");
                }
               


            }
            catch (Exception ex)
            {

                _logger.Error(ex, "API GetMHP_Filters_Async threw an error");


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
