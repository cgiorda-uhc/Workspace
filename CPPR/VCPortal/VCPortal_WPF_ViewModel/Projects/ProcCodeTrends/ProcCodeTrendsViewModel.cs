using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using DataAccessLibrary.Data.Abstract;
using DataAccessLibrary.DataAccess;
using FileParsingLibrary.MSExcel;
using FileParsingLibrary.MSExcel.Custom.ProcCodeTrends;
using MathNet.Numerics.Providers.SparseSolver;
using Microsoft.Extensions.Configuration;
using SharedFunctionsLibrary;

using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Text;
using System.Text.Json;

using System.Windows.Input;

using VCPortal_Models.Configuration.HeaderInterfaces.Abstract;
using VCPortal_Models.Configuration.HeaderInterfaces.Concrete;

using VCPortal_Models.Models.ProcCodeTrends;

using VCPortal_Models.Parameters.ProcCodeTrends;

using VCPortal_WPF_ViewModel.Shared;

namespace VCPortal_WPF_ViewModel.Projects.ProcCodeTrends;
public partial class ProcCodeTrendsViewModel : ObservableObject
{
    private readonly IExcelFunctions _excelFunctions;
    private readonly IProcCodeTrendConfig? _config;
    private readonly Serilog.ILogger _logger;

    private readonly IRelationalDataAccess _db_sql;
    private readonly IChemotherapyPX_Repo _chemo_sql;
    private readonly IMHPUniverse_Repo _mhp_sql;
    private readonly IProcCodeTrends_Repo _pct_db;
    private readonly IEDCAdhoc_Repo _edc_db;
    private readonly IETGFactSymmetry_Repo _etg_db;


    private StringBuilder _sbStatus;
    private List<MM_FINAL_Model> _mM_Final_Filters { get; set; }

    private List<DateSpan_Model> _date_span { get; set; }


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


    [ObservableProperty]
    public List<string> _proc_Cd;


    [ObservableProperty]
    public int _topRows = 100;


    public ProcCodeTrendsViewModel(IConfiguration config, IExcelFunctions excelFunctions, Serilog.ILogger logger, IRelationalDataAccess db_sql, IChemotherapyPX_Repo chemo_sql, IMHPUniverse_Repo mhp_sql, IProcCodeTrends_Repo pct_db, IEDCAdhoc_Repo edc_db, IETGFactSymmetry_Repo etg_db)
    {
        _logger = logger;
        _excelFunctions = excelFunctions;
        _config = prepareConfig(config);

        _db_sql = db_sql;
        _chemo_sql = chemo_sql;
        _mhp_sql = mhp_sql;
        _pct_db = pct_db;
        _edc_db = edc_db;
        _etg_db = etg_db;



        UserMessageViewModel = new MessageViewModel();
        ProgressMessageViewModel = new MessageViewModel();

        worker.DoWork += worker_DoWork;
        worker.RunWorkerCompleted += worker_RunWorkerCompleted;


        CurrentTitle = "ProcCode Trending";


        _sbStatus = new StringBuilder();
        canRunReport = true;


        if (_config != null)
        {

            InitialLoadData();

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

         var task = GenerateReport();
        task.Wait();


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

        if (_date_span == null)
        {
            UserMessageViewModel.IsError = true;
            UserMessageViewModel.Message = "An error was thrown. Please contact the system admin.";
            _logger.Error("ProcCodeTrends.GenerateReport threw an error for {CurrentUser}: DateSpan had no values", Authentication.UserName);
            return;
        }



        _logger.Information("Running ProcCodeTrends.GenerateReport for {CurrentUser}...", Authentication.UserName);

        _sbStatus.Append("--Processing selected filters for ProcCodeTrends" + Environment.NewLine);
        ProgressMessageViewModel.Message = _sbStatus.ToString();

        object[] parameters = _params as object[];


        ProcCodeTrends_Parameters pc_param = new ProcCodeTrends_Parameters();

        CancellationTokenSource cancellationToken;
        cancellationToken = new CancellationTokenSource();

        List<string> lob_list = new List<string>();
        List<string> file_list = new List<string>();


        bool blFirstPass = true;
        try
        {
            var sb_lob = new StringBuilder();

            if(!string.IsNullOrEmpty(parameters[0] + ""))
            {
 

                var lst = parameters[0].ToString().Replace("--All--,", "").Split(',');
                foreach(var l in lst)
                {
                    lob_list.Add(l);


                    sb_lob.Append(l.Replace("EI","E&I").Replace("MR", "M&R").Replace("CS", "C&S") + ", ");
                }

                
            }
            else
            {
                lob_list.Add("CS");
                lob_list.Add("EI");
                lob_list.Add("MR");

                sb_lob.Append("E&I, M&R, C&S");
            }



            foreach (var lob  in lob_list)
            {
                //if (!string.IsNullOrEmpty(parameters[0] + ""))
                //{
                //    pc_param.LOB = "'" + String.Join(",", parameters[0].ToString().Replace("--All--,", "")).Replace(",", "', '") + "'";
                //}

                pc_param.LOB = "'" + lob + "'";
                


                if (!string.IsNullOrEmpty(parameters[1] + ""))
                {
                    pc_param.Region = "'" + String.Join(",", parameters[1].ToString().Replace("--All--,", "")).Replace(",", "', '") + "'";
                }

                if (!string.IsNullOrEmpty(parameters[2] + ""))
                {
                    pc_param.mapping_state = "'" + String.Join(",", parameters[2].ToString().Replace("--All--,", "")).Replace(",", "', '") + "'";
                }

                if (!string.IsNullOrEmpty(parameters[3] + ""))
                {
                    pc_param.PRDCT_LVL_1_NM = "'" + String.Join(",", parameters[3].ToString().Replace("--All--,", "")).Replace(",", "', '") + "'";
                }

                if (!string.IsNullOrEmpty(parameters[4] + ""))
                {
                    pc_param.CS_TADM_PRDCT_MAP = "'" + String.Join(",", parameters[4].ToString().Replace("--All--,", "")).Replace(",", "', '") + "'";
                }

                if (!string.IsNullOrEmpty(parameters[5] + ""))
                {
                    pc_param.HLTH_PLN_FUND_DESC = "'" + String.Join(",", parameters[5].ToString().Replace("--All--,", "")).Replace(",", "', '") + "'";
                }

                if (!string.IsNullOrEmpty(parameters[6] + ""))
                {
                    pc_param.HCE_LEG_ENTY_ROLLUP_DESC = "'" + String.Join(",", parameters[6].ToString().Replace("--All--,", "")).Replace(",", "', '") + "'";
                }

                if (!string.IsNullOrEmpty(parameters[7] + ""))
                {
                    pc_param.SRC_SYS_GRP_DESC = "'" + String.Join(",", parameters[7].ToString().Replace("--All--,", "")).Replace(",", "', '") + "'";
                }

                if (!string.IsNullOrEmpty(parameters[8] + ""))
                {
                    pc_param.CS_DUAL_IND = "'" + String.Join(",", parameters[8].ToString().Replace("--All--,", "")).Replace(",", "', '") + "'";
                }

                if (!string.IsNullOrEmpty(parameters[9] + ""))
                {
                    pc_param.MR_DUAL_IND = "'" + String.Join(",", parameters[9].ToString().Replace("--All--,", "")).Replace(",", "', '") + "'";
                }

                System.Collections.IList items = (System.Collections.IList)parameters[10];
                StringBuilder sb = new StringBuilder();
                foreach (var i in items)
                {
                    sb.Append("'" + i.ToString().Split('-')[0].Trim() + "',");
                }
                if (sb.Length > 0)
                {
                    pc_param.px = sb.ToString().TrimEnd(',');
                }

                pc_param.RowCount = _topRows;


                pc_param.DateSpanList = _date_span;




                if(blFirstPass)
                {
                    var template = @"\\WP000003507\csg_share\VC_Portal\templates\PC_Trends\Read_Me.xlsx";

                    var b = await ProcCodeTrendsExport.ExportReadmeToExcel(pc_param, template, sb_lob.ToString().Trim().TrimEnd(','), () => ProgressMessageViewModel.Message, x => ProgressMessageViewModel.Message = x, cancellationToken.Token);

                    _sbStatus.Remove(0, _sbStatus.Length);
                    _sbStatus.Append(ProgressMessageViewModel.Message);
                    ProgressMessageViewModel.Message = _sbStatus.ToString();


                    var f = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + "\\Read_Me_" + DateTime.Now.ToString("yyyy-dd-M--HH-mm-ss") + ".xlsx";

                    file_list.Add(f);

                    await File.WriteAllBytesAsync(f, b);

                    blFirstPass = false;

                    ProgressMessageViewModel.HasMessage = true;
                }




                CLM_OP_Report_Model report_results;

                try
                {

                    _sbStatus.Append("--Retreiving ProcCodeTrends " + lob + " claims op/phys data from Database" + Environment.NewLine);
                    ProgressMessageViewModel.Message = _sbStatus.ToString();


                    ////RETURN HTTP 200
                    var task = _pct_db.GetMainPCTReport_Async(pc_param, cancellationToken.Token);//200 SUCCESS

                    task.Wait(); // Blocks current thread until GetFooAsync task completes
                                 // For pedagogical use only: in general, don't do this!
                    var results = task.Result;

                    if (results != null)
                    {

                        report_results = results;

                        report_results.unique_individual_op_comment = _config.Comments.FirstOrDefault(x => x.Header == "OP Unique Individual").Comment;
                        report_results.unique_individual_phys_comment = _config.Comments.FirstOrDefault(x => x.Header == "PHYS Unique Individual").Comment;
                        report_results.unique_individual_total_comment = _config.Comments.FirstOrDefault(x => x.Header == "TOTAL Unique Individual").Comment;
                        report_results.events_op_comment = _config.Comments.FirstOrDefault(x => x.Header == "OP Events").Comment;
                        report_results.events_phys_comment = _config.Comments.FirstOrDefault(x => x.Header == "PHYS Events").Comment;
                        report_results.events_total_comment = _config.Comments.FirstOrDefault(x => x.Header == "TOTAL Events").Comment;
                        report_results.claims_op_comment = _config.Comments.FirstOrDefault(x => x.Header == "OP Claims").Comment;
                        report_results.claims_phys_comment = _config.Comments.FirstOrDefault(x => x.Header == "PHYS Claims").Comment;
                        report_results.claims_total_comment = _config.Comments.FirstOrDefault(x => x.Header == "TOTAL Claims").Comment;
                        report_results.allowed_op_comment = _config.Comments.FirstOrDefault(x => x.Header == "OP Allowed Amount").Comment;
                        report_results.allowed_phys_comment = _config.Comments.FirstOrDefault(x => x.Header == "PHYS Allowed Amount").Comment;
                        report_results.allowed_total_comment = _config.Comments.FirstOrDefault(x => x.Header == "TOTAL Allowed Amount").Comment;
                        report_results.allowed_pmpm_op_comment = _config.Comments.FirstOrDefault(x => x.Header == "OP Allowed Amount PMPM").Comment;
                        report_results.allowed_pmpm_phys_comment = _config.Comments.FirstOrDefault(x => x.Header == "PHYS Allowed Amount PMPM").Comment;
                        report_results.allowed_pmpm_total_comment = _config.Comments.FirstOrDefault(x => x.Header == "TOTAL Allowed Amount PMPM").Comment;
                        report_results.utilization000_op_comment = _config.Comments.FirstOrDefault(x => x.Header == "OP Utilization/000").Comment;
                        report_results.utilization000_phys_comment = _config.Comments.FirstOrDefault(x => x.Header == "PHYS Utilization/000").Comment;
                        report_results.utilization000_total_comment = _config.Comments.FirstOrDefault(x => x.Header == "TOTAL Utilization/000").Comment;
                        report_results.events_op_cost_comment = _config.Comments.FirstOrDefault(x => x.Header == "OP Event Cost").Comment;
                        report_results.events_phys_cost_comment = _config.Comments.FirstOrDefault(x => x.Header == "PHYS Event Cost").Comment;
                        report_results.events_total_cost_comment = _config.Comments.FirstOrDefault(x => x.Header == "TOTAL Event Cost").Comment;
                        report_results.unit_cost_op_comment = _config.Comments.FirstOrDefault(x => x.Header == "OP Unit Cost").Comment;
                        report_results.unit_cost_phys_comment = _config.Comments.FirstOrDefault(x => x.Header == "PHYS Unit Cost").Comment;
                        report_results.unit_cost_total_comment = _config.Comments.FirstOrDefault(x => x.Header == "TOTAL Unit Cost").Comment;


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

                    UserMessageViewModel.IsError = true;
                    UserMessageViewModel.Message = "An error was thrown. Please contact the system admin.";
                    _logger.Fatal(ex, "ProcCodeTrends Report threw an error for {CurrentUser}", Authentication.UserName);


                    return;

                }









                //var api = _config.APIS.Where(x => x.Name == "PCT_MainReport").FirstOrDefault();
                //WebAPIConsume.BaseURI = api.BaseUrl;
                //var response = await WebAPIConsume.PostCall<ProcCodeTrends_Parameters>(api.Url, pc_param);
                //if (response.StatusCode != System.Net.HttpStatusCode.OK)
                //{

                //    UserMessageViewModel.IsError = true;
                //    UserMessageViewModel.Message = "An error was thrown. Please contact the system admin.";
                //    _logger.Error("ProcCodeTrends.GenerateReport threw an error for {CurrentUser}" + response.StatusCode.ToString(), Authentication.UserName);
                //    return;
                //}
                //else
                //{

                //    var reponseStream = await response.Content.ReadAsStreamAsync();
                //    var result = await JsonSerializer.DeserializeAsync<CLM_OP_Report_Model>(reponseStream, new JsonSerializerOptions
                //    {
                //        PropertyNameCaseInsensitive = true
                //    });

                //    report_results = result;



                //    report_results.unique_individual_op_comment = _config.Comments.FirstOrDefault(x => x.Header == "OP Unique Individual").Comment;
                //    report_results.unique_individual_phys_comment = _config.Comments.FirstOrDefault(x => x.Header == "PHYS Unique Individual").Comment;
                //    report_results.events_op_comment = _config.Comments.FirstOrDefault(x => x.Header == "OP Events").Comment;
                //    report_results.events_phys_comment = _config.Comments.FirstOrDefault(x => x.Header == "PHYS Events").Comment;
                //    report_results.claims_op_comment = _config.Comments.FirstOrDefault(x => x.Header == "OP Claims").Comment;
                //    report_results.claims_phys_comment = _config.Comments.FirstOrDefault(x => x.Header == "PHYS Claims").Comment;
                //    report_results.allowed_op_comment = _config.Comments.FirstOrDefault(x => x.Header == "OP Allowed Amount").Comment;
                //    report_results.allowed_phys_comment = _config.Comments.FirstOrDefault(x => x.Header == "PHYS Allowed Amount").Comment;
                //    report_results.allowed_pmpm_op_comment = _config.Comments.FirstOrDefault(x => x.Header == "OP Allowed Amount PMPM").Comment;
                //    report_results.allowed_pmpm_phys_comment = _config.Comments.FirstOrDefault(x => x.Header == "PHYS Allowed Amount PMPM").Comment;
                //    report_results.utilization000_op_comment = _config.Comments.FirstOrDefault(x => x.Header == "OP Utilization/000").Comment;
                //    report_results.utilization000_phys_comment = _config.Comments.FirstOrDefault(x => x.Header == "PHYS Utilization/000").Comment;
                //    report_results.events_op_comment = _config.Comments.FirstOrDefault(x => x.Header == "OP Event Cost").Comment;
                //    report_results.events_phys_comment = _config.Comments.FirstOrDefault(x => x.Header == "PHYS Event Cost").Comment;
                //    report_results.unit_cost_op_comment = _config.Comments.FirstOrDefault(x => x.Header == "OP Unit Cost").Comment;
                //    report_results.unit_cost_phys_comment = _config.Comments.FirstOrDefault(x => x.Header == "PHYS Unit Cost").Comment;


                //}



                var bytes = await ProcCodeTrendsExport.ExportProcDataToExcel(report_results, () => ProgressMessageViewModel.Message, x => ProgressMessageViewModel.Message = x, cancellationToken.Token);

                _sbStatus.Remove(0, _sbStatus.Length);
                _sbStatus.Append(ProgressMessageViewModel.Message);
                ProgressMessageViewModel.Message = _sbStatus.ToString();

                var file = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + "\\PC_Trend_"+lob+"_CLM_OP_PHYS_" + DateTime.Now.ToString("yyyy-dd-M--HH-mm-ss") + ".xlsx";

                file_list.Add(file);

                //_sbStatus.Append("--Saving Excel here: " + file + Environment.NewLine);
                //ProgressMessageViewModel.Message = _sbStatus.ToString();

                //_sbStatus.Append("--Finalizing "+lob+ " file " + Environment.NewLine);
                //ProgressMessageViewModel.Message = _sbStatus.ToString();

                if (File.Exists(file))
                    File.Delete(file);

                await File.WriteAllBytesAsync(file, bytes);


                //_sbStatus.Append("--Opening "+lob+" Excel" + Environment.NewLine);
                //ProgressMessageViewModel.Message = _sbStatus.ToString();

                //var p = new Process();
                //p.StartInfo = new ProcessStartInfo(file)
                //{
                //    UseShellExecute = true
                //};
                //p.Start();

                //_sbStatus.Append("--Process completed!" + Environment.NewLine + Environment.NewLine + Environment.NewLine);
                _sbStatus.Append("--Proc Code Trend " + lob + " Report sucessfully generated" + Environment.NewLine);
                ProgressMessageViewModel.Message = _sbStatus.ToString();

                //_sbStatus.Append("--Process completed!" + Environment.NewLine + Environment.NewLine + Environment.NewLine);
                //_sbStatus.Append("--Ready" + Environment.NewLine);
                //ProgressMessageViewModel.Message = _sbStatus.ToString();

                //serMessageViewModel.IsError = false;
                //UserMessageViewModel.Message = "Proc Code Trend "+lob+ " Report sucessfully generated";
                //_logger.Information("Proc Code Trend Report sucessfully generated for {CurrentUser}...", Authentication.UserName);
                ProgressMessageViewModel.HasMessage = true;
            }


            _sbStatus.Append("--Generating final file" + Environment.NewLine);
            ProgressMessageViewModel.Message = _sbStatus.ToString();
            var final_zip = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + "\\PC_Trend_CLM_OP_PHYS_" + DateTime.Now.ToString("yyyy-dd-M--HH-mm-ss") + ".zip";
            using (ZipArchive zip = ZipFile.Open(final_zip, ZipArchiveMode.Create))
            {
             
                foreach(var f in file_list)
                {
                    zip.CreateEntryFromFile(f, Path.GetFileName(f));
                }
                
            }

            _sbStatus.Append("--Cleaning up files" + Environment.NewLine);
            ProgressMessageViewModel.Message = _sbStatus.ToString();
            foreach (var f in file_list)
            {
                try
                {
                    File.Delete(f);
                }
                catch(IOException)
                {
                }
  
            }


            _sbStatus.Append("--Opening final file" + Environment.NewLine);
            ProgressMessageViewModel.Message = _sbStatus.ToString();
            System.Diagnostics.Process.Start("explorer.exe", final_zip);

            ProgressMessageViewModel.HasMessage = false; ;
            UserMessageViewModel.IsError = false;
            UserMessageViewModel.Message = "Proc Code Trend Report sucessfully generated";
            _logger.Information("Proc Code Trend Report sucessfully generated for {CurrentUser}...", Authentication.UserName);
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
            CancellationTokenSource cancellationToken;
            cancellationToken = new CancellationTokenSource();

            //var api = _config.APIS.Where(x => x.Name == "PCT_MM_Final").FirstOrDefault();
            //WebAPIConsume.BaseURI = api.BaseUrl;
            //_sbStatus.Append("--Getting Cached Filters..." + Environment.NewLine);
            //ProgressMessageViewModel.Message = _sbStatus.ToString();
            //await Task.Delay(TimeSpan.FromSeconds(1));
            //var response = WebAPIConsume.GetCall(api.Url);
            //if (response.Result.StatusCode == System.Net.HttpStatusCode.OK)
            //{
            //    var reponseStream = await response.Result.Content.ReadAsStreamAsync();
            //    var result = await JsonSerializer.DeserializeAsync<List<MM_FINAL_Model>>(reponseStream, new JsonSerializerOptions
            //    {
            //        PropertyNameCaseInsensitive = true
            //    });

            //    _mM_Final_Filters = result;
            //}
            //else
            //{
            //    UserMessageViewModel.IsError = true;
            //    UserMessageViewModel.Message = "An error was thrown. Please contact the system admin.";
            //    _logger.Error("populateFilters.MM_Final_Filters threw an error for {CurrentUser}" + response.Result.StatusCode.ToString(), Authentication.UserName);
            //}

            try
            {

                _sbStatus.Append("--Getting Cached Filters..." + Environment.NewLine);
                ProgressMessageViewModel.Message = _sbStatus.ToString();


                ////RETURN HTTP 200
                var results = await _pct_db.GetMM_FINAL_Async(cancellationToken.Token);//200 SUCCESS


                if (results != null)
                {

                    _mM_Final_Filters = results.ToList();

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



            try
            {

                _sbStatus.Append("--Getting Current Date Span..." + Environment.NewLine);
                ProgressMessageViewModel.Message = _sbStatus.ToString();


                ////RETURN HTTP 200
                var results = await _pct_db.GetDateSpan_Async(cancellationToken.Token);//200 SUCCESS

       

                if (results != null)
                {


                    _date_span = results.ToList();

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


            //api = _config.APIS.Where(x => x.Name == "PCT_DateSpan").FirstOrDefault();
            //WebAPIConsume.BaseURI = api.BaseUrl;
            //_sbStatus.Append("--Getting Current Date Span..." + Environment.NewLine);
            //ProgressMessageViewModel.Message = _sbStatus.ToString();
            //await Task.Delay(TimeSpan.FromSeconds(1));
            // response = WebAPIConsume.GetCall(api.Url);
            //if (response.Result.StatusCode == System.Net.HttpStatusCode.OK)
            //{
            //    var reponseStream = await response.Result.Content.ReadAsStreamAsync();
            //    var result = await JsonSerializer.DeserializeAsync<List<DateSpan_Model>>(reponseStream, new JsonSerializerOptions
            //    {
            //        PropertyNameCaseInsensitive = true
            //    });

            //    _date_span = result;
            //}
            //else
            //{
            //    UserMessageViewModel.IsError = true;
            //    UserMessageViewModel.Message = "An error was thrown. Please contact the system admin.";
            //    _logger.Error("populateFilters.DateSpan threw an error for {CurrentUser}" + response.Result.StatusCode.ToString(), Authentication.UserName);
            //}


            //api = _config.APIS.Where(x => x.Name == "PCT_Proc_Cd").FirstOrDefault();
            //WebAPIConsume.BaseURI = api.BaseUrl;
            //_sbStatus.Append("--Getting Proc Code Filters..." + Environment.NewLine);
            //ProgressMessageViewModel.Message = _sbStatus.ToString();
            //await Task.Delay(TimeSpan.FromSeconds(1));
            //response = WebAPIConsume.GetCall(api.Url);
            //if (response.Result.StatusCode == System.Net.HttpStatusCode.OK)
            //{
            //    var reponseStream = await response.Result.Content.ReadAsStreamAsync();
            //    var result = await JsonSerializer.DeserializeAsync<List<string>>(reponseStream, new JsonSerializerOptions
            //    {
            //        PropertyNameCaseInsensitive = true
            //    });

            //    Proc_Cd = result;
            //}
            //else
            //{
            //    UserMessageViewModel.IsError = true;
            //    UserMessageViewModel.Message = "An error was thrown. Please contact the system admin.";
            //    _logger.Error("populateFilters.ProcCode threw an error for {CurrentUser}" + response.Result.StatusCode.ToString(), Authentication.UserName);
            //}


            try
            {

                _sbStatus.Append("--Getting Proc Code Filters..." + Environment.NewLine);
                ProgressMessageViewModel.Message = _sbStatus.ToString();


                ////RETURN HTTP 200
                var results = await _pct_db.GetPROC_CD_Async(cancellationToken.Token);//200 SUCCESS


                if (results != null)
                {


                    Proc_Cd = results.ToList();

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
        finally
        {
            ProgressMessageViewModel.HasMessage = false;
        }
    }



    private IProcCodeTrendConfig prepareConfig(IConfiguration config)
    {

        var project = "PCT";
        var section = "Projects";

        ///EXTRACT IConfiguration INTO ETGFactSymmetryConfig 
        var cfg = config.GetSection(section).Get<List<ProcCodeTrendConfig>>();
        IProcCodeTrendConfig pct = new ProcCodeTrendConfig();
        if (cfg == null)
        {
            return null;
            //throw new OperationCanceledException();
        }
        pct = cfg.Find(p => p.Name == project);
        if (pct != null)
        {
            //Microsoft.Extensions.Configuration.Binder
            var e = config.GetSection(section + ":" + project + ":APIS").Get<APIConfig[]>();
            if (e != null)
            {
                pct.APIS = e.ToList();
            }


            var c = config.GetSection(section + ":" + project + ":Comments").Get<CommentsConfig[]>();
            if (c != null)
            {
                pct.Comments = c.ToList();
            }
        }
        return pct;
    }
}
