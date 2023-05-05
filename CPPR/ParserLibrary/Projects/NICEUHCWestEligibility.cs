

namespace ProjectManagerLibrary.Projects;

public class NICEUHCWestEligibility : INICEUHCWestEligibility
{
    private readonly IRelationalDataAccess _db;
    private readonly INICEUHCWestEligibilityConfig? _config;
    private string _stagingArea;

    public NICEUHCWestEligibility(IConfiguration config, IRelationalDataAccess db)
    {
        //INJECT DB SOURCES
        _db = db;
        //EXTRACT CUSTOM CONFIG INTO GLOBAL ICSScorecardConfig _config
        _config = prepareConfig(config);
    }

    public async Task<long> LoadNICEUHCWestEligibilityData()
    {
        //CHECK FOR CONFIG
        if (_config == null)
        {
            Log.Error($"No Config found for NICEUHCWestEligibility data load");
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
            Log.Information($"Retrieving latest data from NICEUHCWestEligibility...");
            //FIND LAST FILE DATE TO DETERMIN IF ANYTHING NEW WAS DROPPED
            var sql = _config.SQLLists.Where(x => x.Name == "LatestFileDate").FirstOrDefault();
            var lastestFileDate = (await _db.LoadData<FileDateModel>(connectionString: sql.ConnectionString, sql.SQL.FirstOrDefault())).FirstOrDefault();

            Log.Information($"Searching for new files...");
            //COMPARE LAST DATE WITH NEW FILESD
            var newFiles = getNewFiles(_config.FileLists, lastestFileDate);
            if (newFiles.Count == 0)
            {
                Log.Information($"No results found for NICEUHCWestEligibility. Will try again next time");
                stopwatch.Stop();
                return stopwatch.ElapsedMilliseconds;
            }

            //DOWNLOAD NEW FILES TO STAGING AREA FOR PROCESSING
            string directory = null;
            Log.Information($"Copying new files to " + workingPath + "...");
            foreach (var file in newFiles)
            {
                if (directory == null)
                    directory = Path.GetFullPath(file);

                var fileName = Path.GetFileName(file);
                var current = workingPath + fileName;
                if (!File.Exists(current))
                {
                    File.Copy(file, current);
                }
            }

            Log.Information($"Getting column mappings...");
            //MAP DB TO EXCEL COLUMN NAMES
            var closed_xml = new ClosedXMLFunctions();
            closed_xml.Mappings = getColumnMappings();
            //GET STAGING FILES FOR PROCESSING
            var workingFiles = Directory.GetFiles(workingPath, "*.xlsx", SearchOption.TopDirectoryOnly);

            var config_sheet = _config.FileLists[0].ExcelConfigs[0];
            foreach (var file in workingFiles)
            {
                var sheet = config_sheet.SheetName;
                var filename = Path.GetFileName(file);

                Log.Information($"Processing " + filename + " sheet:" + sheet);
                var nice = closed_xml.ImportExcel<NICEUHCWestEligibilityModel>(file, sheet, config_sheet.ColumnRange, config_sheet.StartingDataRow);
                foreach (var n in nice)
                {

                    //NOT IN SHEET
                    n.report_type = "NICE";
                    n.file_month = _month;
                    n.file_year = _year;
                    n.file_date = new DateTime(_year, _month, 01);
                    n.sheet_name = sheet;
                    n.file_name = filename;
                    n.file_path = directory;

                }


                //BULK LOAD CURRENT List<NICEUHCWestEligibilityModel> INTO DB
                string[] columns = typeof(NICEUHCWestEligibilityModel).GetProperties().Select(p => p.Name).ToArray();
                Log.Information($"Saving contents of " + filename + " sheet:" + sheet + " to database");
                await _db.BulkSave<NICEUHCWestEligibilityModel>(sql.ConnectionString, _destination, nice, columns);
                blUpdated = true;



                //ARCHIVE FILE ONCE LOADED
                var fileName = Path.GetFileName(file);
                if (!File.Exists(workingPath + "Archive\\" + fileName))
                {
                    Log.Information($"Archiving " + filename + "...");
                    File.Move(file, workingPath + "Archive\\" + fileName);
                }
                else
                {
                    Log.Information($"Deleting " + filename + "...");
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
                    Log.Error($"No Email found for NICEUHCWestEligibility Refresh");
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
                Log.Information($"NICEUHCWestEligibility process completed. Sending email to: " + email.EmailTo);
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

                Log.Information($"NICEUHCWestEligibility no new data found.");
            }

        }

        stopwatch.Stop();
        return stopwatch.ElapsedMilliseconds;
    }




    private string _destination;
    private int _month;
    private int _year;
    private List<string> getNewFiles(List<FileExcelConfig> fileList, FileDateModel fdate)
    {
        List<string> filesFound = new List<string>();
        int month, year;
        foreach (var file in fileList)
        {
            var subFolder = (fdate.file_month == 12 ? fdate.file_year + 1 : fdate.file_year );


            Log.Information($"Searching for " + file.FilePath + "\\" + subFolder + "\\" + file.FileName + "...");
            var list = Directory.GetFiles(file.FilePath + "\\" + subFolder, file.FileName, SearchOption.AllDirectories);
            //var list = Directory.EnumerateFiles(file.FilePath, file.FileName, SearchOption.TopDirectoryOnly);
            foreach (var f in list)
            {
                //IF DATE THEN ADD
                var fileName = Path.GetFileName(f).Replace(".xlsx", "");
                var fileParsed = fileName.Split('_');
                var fulldate = fileParsed[2];

                month = int.TryParse(fulldate.Substring(4,2), out month) ? month : 0;
                year = int.TryParse(fulldate.Substring(0, 4), out year) ? year : 0;

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

        }
        return filesFound;
    }

    private List<KeyValuePair<string, string>> getColumnMappings()
    {
        var list = new List<KeyValuePair<string, string>>
        {
            new KeyValuePair<string, string>("CONTRACT NUMBER","Contract_Number"),
            new KeyValuePair<string, string>("PBP","PBP"),
            new KeyValuePair<string, string>("COMPANY STATE","Company_State"),
            new KeyValuePair<string, string>("Total","Member_Count")
        };

        return list;

    }


    private INICEUHCWestEligibilityConfig prepareConfig(IConfiguration config)
    {


        var project = "NICEUHCWestEligibility";
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

        ///EXTRACT IConfiguration INTO NICEUHCWestEligibilityConfig
        var cfg = config.GetSection(section).Get<List<NICEUHCWestEligibilityConfig>>();
        INICEUHCWestEligibilityConfig nc = new NICEUHCWestEligibilityConfig();
        if (cfg == null)
        {
            Log.Error($"No Config found for NICEUHCWestEligibility");
            throw new OperationCanceledException();
        }
        nc = cfg.Find(p => p.Name == project);
        if (nc != null)
        {
            //Microsoft.Extensions.Configuration.Binder
            var e = config.GetSection(section + ":" + project + ":EmailLists").Get<EmailConfig[]>();
            if (e != null)
            {
                nc.EmailLists = e.ToList<EmailConfig>();
            }

            //Microsoft.Extensions.Configuration.Binder
            var f = config.GetSection(section + ":" + project + ":FileLists").Get<FileExcelConfig[]>();
            if (f != null)
            {
                nc.FileLists = f.ToList<FileExcelConfig>();
            }

        }


        return nc;

    }

}
