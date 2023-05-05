



namespace ProjectManagerLibrary.Projects;

public class CSScorecard : ICSScorecard
{

    private readonly IRelationalDataAccess _db;
    private readonly ICSScorecardConfig? _config;
    private string _stagingArea;

    public CSScorecard(IConfiguration config, IRelationalDataAccess db)
    {
        //INJECT DB SOURCES
        _db = db;
        //EXTRACT CUSTOM CONFIG INTO GLOBAL ICSScorecardConfig _config
        _config = prepareConfig(config);
    }


    public async Task<long> LoadCSScorecardData()
    {
        //CHECK FOR CONFIG
        if (_config == null)
        {
            Log.Error($"No Config found for CSScorecard data load");
            throw new OperationCanceledException();
        }

        var workingPath = _stagingArea + "\\" + _config.Name + "\\";

        //SADLY Serilog.sinks.email DOES NOT NOTICE "IsBodyHtml": true, in App.Settings
        //WILL USE A CUSTOM EMAILING SOLUTION FOR THESE
        //Log.Information("<b>{LogFilter} - Testing it!</b>".Replace("\"", ""), "dsv"); appsettings.json = "expression": "LogFilter = 'dsv'"
        var stopwatch = Stopwatch.StartNew();
        IEmailConfig? email = null;
        bool blUpdated = false;
        var results = "";
        try
        {
            Log.Information($"Retrieving latest data from CSScorecard...");
            //FIND LAST FILE DATE TO DETERMIN IF ANYTHING NEW WAS DROPPED
            var sql = _config.SQLLists.Where(x => x.Name == "LatestFileDate").FirstOrDefault();
            var lastestFileDate = (await _db.LoadData<FileDateModel>(connectionString: sql.ConnectionString, sql.SQL.FirstOrDefault())).FirstOrDefault();

            sql = _config.SQLLists.Where(x => x.Name == "ZipState").FirstOrDefault();
            var zip_state = (await _db.LoadData<ZipStateModel>(connectionString: sql.ConnectionString, sql.SQL.FirstOrDefault()));


            Log.Information($"Searching for new files...");
            //COMPARE LAST DATE WITH NEW FILESD
            var newFiles = getNewFiles(_config.FileLists, lastestFileDate);


            if(newFiles.Count == 0)
            {
                Log.Information($"No results found for CSScorecard. Will try again next time");
                stopwatch.Stop();
                return stopwatch.ElapsedMilliseconds;
            }


            //DOWNLOAD NEW FILES TO STAGING AREA FOR PROCESSING
            string directory = null;
            Log.Information($"Copying new files to " + workingPath + "...");
            foreach (var file in newFiles)
            {
                if (directory == null)
                    directory = Path.GetDirectoryName(file);

                var fileName = Path.GetFileName(file);
                var current = workingPath + fileName;
                if (!File.Exists(current))
                {
                    File.Copy(file, current);
                }
                if (current.ToLower().EndsWith(".zip"))
                {
                    Log.Information($"Extracting zipped contents to " + workingPath + "...");
                    extractFromZipFile(fileName, workingPath, _config.FileLists);
                }

            }


            Log.Information($"Getting column mappings...");
            //MAP DB TO EXCEL COLUMN NAMES
            var closed_xml = new ClosedXMLFunctions();

            List<AllStatesCSScorecardModel> detail_final = new List<AllStatesCSScorecardModel>();
            List<CSScorecardModel> summary_final = new List<CSScorecardModel>();
            bool isDetails = false;


            //_month = 1;
            //_year = 2023;
            //_destination = "stg.EviCore_CS_Scorecard";
            //directory = @"\\NASGWFTP03\Care_Core_FTP_Files\Ovations";



            //GET STAGING FILES FOR PROCESSING
            Log.Information($"Processing working files...");
            var workingFiles = Directory.GetFiles(workingPath, "*.xls*", SearchOption.TopDirectoryOnly);
            foreach (var file in workingFiles)
            {
                string cleanFileName = file;
                var filename = Path.GetFileName(file);
                var ext = Path.GetExtension(file);
                string colrange = "";
                int startingRow = 1;
                string strType = null;
                if (filename.ToLower().Contains("_details_"))
                {
                    closed_xml.Mappings = getAllStateMappings();
                    colrange = "B4:AM4";
                    startingRow = 5;
                    isDetails = true;
                }
                else
                {
                    closed_xml.Mappings = getColumnMappings();
                    colrange = "B5:AK5";
                    startingRow = 6;
                    isDetails = false;
                }

                if(filename.ToLower().Contains("_rad"))
                {
                    strType = "RAD";
                }
                else
                {
                    strType = "CARD";
                }
          

                if(ext.ToLower() == ".xls")
                {
                    XLSToXLSXConverter.Convert(cleanFileName);
                    cleanFileName = file + "x";
                }
                
                //GET ALL SHEETS
                var sheet_names = OpenXMLFunctions.GetSheetNames(cleanFileName);
                string strLastState = null;
                //LOOP EACH SHEET AND ADD TO CSScorecardModel
                foreach (var sheet in sheet_names)
                {
                    Log.Information($"Processing " + filename + " sheet:" + sheet);

                    if (sheet.ToLower().Equals("summary") || sheet.ToLower().Equals("medicaid") || sheet.ToLower().Equals("child health plus"))
                    {
                        continue;
                    }

                    //HANDLE DETAIL SPREADSHEET
                    if (isDetails)
                    {
                        var details = closed_xml.ImportExcel<AllStatesCSScorecardModel>(cleanFileName, sheet, colrange, startingRow, nullCheck: "EncounterID");
                        for (int i = details.Count - 1; i >= 0; i--)
                        {
                            var zs = zip_state.Where(x => x.zip == details[i].SiteZipCode).FirstOrDefault();
                            if (zs != null)
                            {
                                details[i].Site_State = zs.state;   // or set it to some other value
                                details[i].file_type = strType;

                                detail_final.Add(details[i]);
                            }
                            else
                            {
                                details.RemoveAt(i);
                            }
                        }
                    }
                    else //HANDLE SUMMARY SPREADSHEET
                    {
                        //var lob = closed_xml.GetValueFromExcel(file, sheet, "F1");
                        var sum = closed_xml.ImportExcel<CSScorecardModel>(cleanFileName, sheet, colrange, startingRow, nullCheck: "Modality");
                        foreach (var s in sum)
                        {

                            //STATES REPEAT ARE BLANK IN SPREADSHEET
                            if (!string.IsNullOrEmpty(s.State))
                            {
                                strLastState = s.State;
                            }
                            else
                            {
                                s.State = strLastState;
                            }

                            //NOT IN SHEET
                            s.report_type = "CS Scorecard";
                            s.file_month = _month;
                            s.file_year = _year;
                            s.file_date = new DateTime(_year, _month, 01);
                            s.sheet_name = strType;
                            s.file_name = filename;
                            s.file_path = directory;

                            summary_final.Add(s);

                        }
                    }

                   
                   

                }


            }


            //TAT HAS BLANK/SUM COLUMNS THIS WILL REMOVE THOSE ROWS
            summary_final.RemoveAll(o => string.IsNullOrEmpty(o.Modality));

            var states = summary_final.Select(x => x.State).Distinct().ToList();

            var rad_card = new List<string>();
            rad_card.Add("RAD");
            rad_card.Add("CARD");


            var final = new List<CSScorecardModel>();

            //MERGE SUMMARY AND DETAILS INTO FINAL PRODUCT  final = new List<CSScorecardModel>();
            foreach (var rc in rad_card)
            {
                foreach (var state in states)
                {
                    var modalities = summary_final.Where(x => x.sheet_name == rc).Select(x => x.Modality).Distinct().ToList();
                    modalities.Insert(0, "ALL");


                    foreach (var modality in modalities)
                    {
                        var d = new CSScorecardModel();

                        d.State = state;
                        d.Modality = modality;


                        if (modality == "ALL")//GET ALL MODALITY VALUES IN ONE
                        {

                            d.Phone = detail_final.Where(x => x.CaseInit == "Phone" && x.Site_State == state && x.file_type == rc).Count();
                            d.Web = detail_final.Where(x => x.CaseInit == "Web" && x.Site_State == state && x.file_type == rc).Count();
                            d.Fax = detail_final.Where(x => x.CaseInit == "Fax" && x.Site_State == state && x.file_type == rc).Count();
                            d.RequestsPer1000 = summary_final.Where(x => x.State == state && x.sheet_name == rc).Select(x => x.RequestsPer1000).Sum();
                            d.ApprovalsPer1000 = summary_final.Where(x => x.State == state && x.sheet_name == rc).Select(x => x.ApprovalsPer1000).Sum();

                            d.Approved = summary_final.Where(x => x.State == state && x.sheet_name == rc).Select(x => x.Approved).Sum();
                            d.Auto_Approved = summary_final.Where(x => x.State == state && x.sheet_name == rc).Select(x => x.Auto_Approved).Sum();

                            d.Denied = summary_final.Where(x => x.State == state && x.sheet_name == rc).Select(x => x.Denied).Sum();
                            d.Withdrawn = summary_final.Where(x => x.State == state && x.sheet_name == rc).Select(x => x.Withdrawn).Sum();
                            d.Expired = summary_final.Where(x => x.State == state && x.sheet_name == rc).Select(x => x.Expired).Sum();
                            d.Others = summary_final.Where(x => x.State == state && x.sheet_name == rc).Select(x => x.Others).Sum();

                            d.report_type = summary_final.Where(x => x.State == state && x.sheet_name == rc).Select(x => x.report_type).FirstOrDefault();
                            d.file_month = summary_final.Where(x => x.State == state && x.sheet_name == rc).Select(x => x.file_month).FirstOrDefault();
                            d.file_year = summary_final.Where(x => x.State == state && x.sheet_name == rc).Select(x => x.file_year).FirstOrDefault();
                            d.file_date = summary_final.Where(x => x.State == state && x.sheet_name == rc).Select(x => x.file_date).FirstOrDefault();
                            d.sheet_name = summary_final.Where(x => x.State == state && x.sheet_name == rc).Select(x => x.sheet_name).FirstOrDefault();
                            d.file_name = summary_final.Where(x => x.State == state && x.sheet_name == rc).Select(x => x.file_name).FirstOrDefault();
                            d.file_path = summary_final.Where(x => x.State == state && x.sheet_name == rc).Select(x => x.file_path).FirstOrDefault();

                        }
                        else //GET SINGLE MODALITY VALUE
                        {
                            var tmp = summary_final.Where(x => x.State == state && x.Modality == modality && x.sheet_name == rc).ToList();
                            if (tmp.Count() > 1)//DUPLICATE ROW
                            {
                                foreach (var t in tmp)
                                {
                                    d.Approved = t.Approved;
                                    d.Auto_Approved = t.Auto_Approved;
                                    d.Denied = t.Denied;
                                    d.Withdrawn = t.Withdrawn;
                                    d.Expired = t.Expired;
                                    d.Others = t.Others;
                                    d.is_ignored = true;
                                    d.ignore_reason = "Duplicate Row";

                                    d.report_type = t.report_type;
                                    d.file_month = t.file_month;
                                    d.file_year = t.file_year;
                                    d.file_date = t.file_date;
                                    d.sheet_name = t.sheet_name;
                                    d.file_name = t.file_name;
                                    d.file_path = t.file_path;

                                }
                            }
                            else if (tmp.Count == 1) //AS EXPECTED SINGLE ROW
                            {
                                d.Approved = tmp[0].Approved;
                                d.Auto_Approved = tmp[0].Auto_Approved;
                                d.Denied = tmp[0].Denied;
                                d.Withdrawn = tmp[0].Withdrawn;
                                d.Expired = tmp[0].Expired;
                                d.Others = tmp[0].Others;
                                d.is_ignored = false;
                                d.ignore_reason = null;


                                d.report_type = tmp[0].report_type;
                                d.file_month = tmp[0].file_month;
                                d.file_year = tmp[0].file_year;
                                d.file_date = tmp[0].file_date;
                                d.sheet_name = tmp[0].sheet_name;
                                d.file_name = tmp[0].file_name;
                                d.file_path = tmp[0].file_path;

                            }
                            else //NO MATCHES FOUND
                            {
                                d.Approved = 0;
                                d.Auto_Approved = 0;
                                d.Denied = 0;
                                d.Withdrawn = 0;
                                d.Expired = 0;
                                d.Others = 0;
                                d.is_ignored = false;
                                d.ignore_reason = null;

                                d.report_type = summary_final.Where(x => x.State == state && x.sheet_name == rc).Select(x => x.report_type).FirstOrDefault();
                                d.file_month = summary_final.Where(x => x.State == state && x.sheet_name == rc).Select(x => x.file_month).FirstOrDefault();
                                d.file_year = summary_final.Where(x => x.State == state && x.sheet_name == rc).Select(x => x.file_year).FirstOrDefault();
                                d.file_date = summary_final.Where(x => x.State == state && x.sheet_name == rc).Select(x => x.file_date).FirstOrDefault();
                                d.sheet_name = summary_final.Where(x => x.State == state && x.sheet_name == rc).Select(x => x.sheet_name).FirstOrDefault();
                                d.file_name = summary_final.Where(x => x.State == state && x.sheet_name == rc).Select(x => x.file_name).FirstOrDefault();
                                d.file_path = summary_final.Where(x => x.State == state && x.sheet_name == rc).Select(x => x.file_path).FirstOrDefault();

                            }
                        }


                        final.Add(d);
                    }

                }
            }

            //SAVE FINAL INTO DATABASE
            string[] columns = typeof(CSScorecardModel).GetProperties().Select(p => p.Name).ToArray();
            Log.Information($"Saving contents of CS Scorecard to database");
            await _db.BulkSave<CSScorecardModel>(sql.ConnectionString, _destination, final, columns) ;
            blUpdated = true ;


            //ARCHIVE FILE ONCE LOADED
            foreach (var file in workingFiles)
            {
                string cleanFileName = file;
                var ext = Path.GetExtension(cleanFileName);
                if (ext.ToLower() == ".xls")
                {
                    cleanFileName = file + "x";
                }

                var fileName = Path.GetFileName(cleanFileName);
                if (!File.Exists(workingPath + "Archive\\" + fileName))
                {
                    Log.Information($"Archiving " + fileName + "...");
                    File.Move(cleanFileName, workingPath + "Archive\\" + fileName);
                }
                else
                {
                    Log.Information($"Deleting " + cleanFileName + "...");
                    File.Delete(file);
                }
            }


        }
        catch (Exception ex)
        {
            //GETTING EMAIL ERROR CONFIG DETAILS
            Log.Error(ex.ToString());
            email = _config.EmailLists.Where(x => x.EmailStatus == Status.Failure).FirstOrDefault();
            try
            {
                //SEND EMAIL TO NOTIFY ABOUT ERROR
                if (email != null)
                {
                    await SharedFunctions.EmailAsync(email.EmailTo, email.EmailFrom, email.EmailSubject, "<p>" + email.EmailBody + "</p><p>" + results + "</p>", email.EmailCC, null, System.Net.Mail.MailPriority.Normal).ConfigureAwait(false);
                }
                else
                {
                    Log.Error($"No Email found for CSScorecard Refresh");
                    throw new OperationCanceledException();
                }

            }
            catch (Exception e)//HANDLE EMAIL ERRORS
            {
                Log.Error(e.ToString());
            }
            finally
            {
                throw new OperationCanceledException(); //CANCEL CURRENT TASK
            }

        }
        finally
        {
            if (blUpdated)
            {
                email = _config.EmailLists.Where(x => x.EmailStatus == Status.Success).FirstOrDefault();
                Log.Information($"CSScorecard process completed. Sending email to: " + email.EmailTo);
                //await SharedFunctions.EmailAsync("chris_giordano@uhc.com", "chris_giordano@uhc.com", "Data Source Verification", strEmailBody, null, null, System.Net.Mail.MailPriority.Normal).ConfigureAwait(false);
                try
                {
                    await SharedFunctions.EmailAsync(email.EmailTo, email.EmailFrom, email.EmailSubject, email.EmailBody, email.EmailCC, null, System.Net.Mail.MailPriority.Normal).ConfigureAwait(false);
                }
                catch (Exception ex)
                {
                    Log.Error(ex.ToString());
                    throw new OperationCanceledException();
                }

            }
            else
            {

                Log.Information($"CSScorecard no new data found.");
            }

        }

        stopwatch.Stop();
        return stopwatch.ElapsedMilliseconds;


    }


    private void extractFromZipFile(string fileName, string workingPath, List<FileConfig> fileConfigs)
    {

        var filepath = workingPath + "\\" + fileName;
        using (ZipArchive archive = ZipFile.OpenRead(filepath))
        {
            foreach (ZipArchiveEntry entry in archive.Entries)
            {
                foreach (var cfg in fileConfigs)
                {

                    if (string.IsNullOrEmpty(cfg.ZippedMatch))
                    {
                        continue;
                    }

                    if (entry.FullName.ToLower().StartsWith(cfg.ZippedMatch.ToLower()))
                    {
                        var f = Path.Combine(workingPath, entry.FullName);
                        if (!File.Exists(f))
                        {
                            entry.ExtractToFile(f);
                        }
                    }
                }
            }
        }

        File.Delete(filepath);
    }

    private string _destination;
    private int _month;
    private int _year;
    private List<string> getNewFiles(List<FileConfig> fileList, FileDateModel fdate)
    {
        List<string> filesFound = new List<string>();
        int month, year;
        foreach (var file in fileList)
        {

            Log.Information($"Searching for " + file.FilePath + "\\" + file.FileName + "...");
            var list = Directory.GetFiles(file.FilePath, file.FileName, SearchOption.TopDirectoryOnly);
            //var list = Directory.EnumerateFiles(file.FilePath, file.FileName, SearchOption.TopDirectoryOnly);
            foreach (var f in list)
            {
                //IF DATE THEN ADD
                var fileName = Path.GetFileName(f).Replace(".xlsx", "");
                var fileParsed = fileName.Split('_');
                month = int.TryParse(fileParsed[fileParsed.Length - 1], out month) ? month : 0;
                year = int.TryParse(fileParsed[fileParsed.Length - 2], out year) ? year : 0;

                if ((fdate.file_month < month && fdate.file_year == year) || fdate.file_year < year)
                {

                    if (!filesFound.Contains(f))
                    {
                        Log.Information($"Match found in " + fileName + "..");

                        filesFound.Add(f);
                        _month = month;
                        _year = year;
                        _destination = file.Destination;
                    }
                }

            }

            if (!string.IsNullOrEmpty(file.ZippedFile))
            {
                Log.Information($"Searching for " + file.FilePath + "\\" + file.ZippedFile + "...");
                list = Directory.GetFiles(file.FilePath, file.ZippedFile, SearchOption.TopDirectoryOnly);
                foreach (var f in list)
                {
                    var fileName = Path.GetFileName(f).Replace(".zip", "");
                    var fileParsed = fileName.Split('_');

                    //var format = (fileParsed[2].Length == 3 ? "MMM" : "MMMM"); //Jan vs January
                    month = int.TryParse(fileParsed[fileParsed.Length - 1], out month) ? month : 0;
                    year = int.TryParse(fileParsed[fileParsed.Length - 2], out year) ? year : 0;

                    if ((fdate.file_month < month && fdate.file_year == year) || fdate.file_year < year)
                    {
                        if(!filesFound.Contains(f))
                        {
                            Log.Information($"Match found in " + fileName + "..");

                            filesFound.Add(f);
                            _month = month;
                            _year = year;
                            _destination = file.Destination;
                        }

                    }
                }
            }

        }
        return filesFound;
    }

    private List<KeyValuePair<string, string>> getColumnMappings()
    {
        var list =  new List<KeyValuePair<string, string>>
            {
                new KeyValuePair<string, string>("State","State"),
                new KeyValuePair<string, string>("Modality","Modality"),
                new KeyValuePair<string, string>("Requests   / 1000","RequestsPer1000"),
                new KeyValuePair<string, string>("Approved       / 1000","ApprovalsPer1000"),
                new KeyValuePair<string, string>("Approved (A)","Approved"),
                new KeyValuePair<string, string>("Auto Approved","Auto_Approved"),
                new KeyValuePair<string, string>("Denied (D)","Denied"),
                new KeyValuePair<string, string>("Withdrawn (W)","Withdrawn"),
                new KeyValuePair<string, string>("Expired (Y)","Expired"),
                new KeyValuePair<string, string>("Pending","Others")
            };

        return list;

    }

    private List<KeyValuePair<string, string>> getAllStateMappings()
    {
        var list = new List<KeyValuePair<string, string>>
            {
                new KeyValuePair<string, string>("Encounter ID","EncounterID"),
                new KeyValuePair<string, string>("Case Init","CaseInit"),
                new KeyValuePair<string, string>("Modality","Modality"),
                new KeyValuePair<string, string>("Site Zip Code","SiteZipCode")
            };

        return list;

    }
    //1. EXTRACT IConfiguration INTO PPACATATConfig
    //2. POPULATE VARIOUS appsettings ARRAYS USING Microsoft.Extensions.Configuration.Binder
    private ICSScorecardConfig prepareConfig(IConfiguration config)
    {


        var project = "CS_Scorecard";
        var section = "Automation";

        //GET APPWIDE (GENERIC) FILE STAGING PATH
        var gen = config.GetSection(section).Get<List<Generic>>();
        if (gen == null)
        {
            Log.Error($"No Generic Config found");
            throw new OperationCanceledException();
        }
        var g = gen.Find(p => p.Name == "Generic");
        _stagingArea = g.FileStagingArea;

        ///EXTRACT IConfiguration INTO PPACATATConfig
        var cfg = config.GetSection(section).Get<List<CSScorecardConfig>>();
        ICSScorecardConfig cs = new CSScorecardConfig();
        if (cfg == null)
        {
            Log.Error($"No Config found for CS_Scorecard");
            throw new OperationCanceledException();
        }
        cs = cfg.Find(p => p.Name == project);
        if (cs != null)
        {
            //Microsoft.Extensions.Configuration.Binder
            var e = config.GetSection(section + ":" + project + ":EmailLists").Get<EmailConfig[]>();
            if (e != null)
            {
                cs.EmailLists = e.ToList<EmailConfig>();
            }
            //Microsoft.Extensions.Configuration.Binder
            var s = config.GetSection(section + ":" + project + ":SQLLists").Get<SQLConfig[]>();
            if (s != null)
            {
                cs.SQLLists = s.ToList<SQLConfig>();
            }

            //Microsoft.Extensions.Configuration.Binder
            var f = config.GetSection(section + ":" + project + ":FileLists").Get<FileConfig[]>();
            if (f != null)
            {
                cs.FileLists = f.ToList<FileConfig>();
            }

        }


        return cs;

    }
}
