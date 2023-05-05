

namespace ProjectManagerLibrary.Projects;

public class EviCoreAmerichoiceAllstatesAuth : IEviCoreAmerichoiceAllstatesAuth
{
    private readonly IRelationalDataAccess _db;
    private readonly IEviCoreAmerichoiceAllstatesAuthConfig? _config;
    private string _stagingArea;

    public EviCoreAmerichoiceAllstatesAuth(IConfiguration config, IRelationalDataAccess db)
    {
        //INJECT DB SOURCES
        _db = db;
        //EXTRACT CUSTOM CONFIG INTO GLOBAL ICSScorecardConfig _config
        _config = prepareConfig(config);
    }


    public async Task<long> LoadEviCoreAmerichoiceAllstatesAuthData()
    {
        //CHECK FOR CONFIG
        if (_config == null)
        {
            Log.Error($"No Config found for EviCoreAmerichoiceAllstatesAuth data load");
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
            Log.Information($"Retrieving latest data from EviCoreAmerichoiceAllstatesAuth...");
            //FIND LAST FILE DATE TO DETERMIN IF ANYTHING NEW WAS DROPPED
            var sql = _config.SQLLists.Where(x => x.Name == "LatestFileDate").FirstOrDefault();
            var lastestFileDate = (await _db.LoadData<FileDateModel>(connectionString: sql.ConnectionString, sql.SQL.FirstOrDefault())).FirstOrDefault();


            Log.Information($"Searching for new files...");
            //COMPARE LAST DATE WITH NEW FILESD
            var newFiles = getNewFiles(_config.FileLists, lastestFileDate);


            if (newFiles.Count == 0)
            {
                Log.Information($"No results found for EviCoreAmerichoiceAllstatesAuth. Will try again next time");
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
                    CommonFunctions.ExtractFromZipFile(fileName, workingPath, _config.FileLists);
                }

            }

            Log.Information($"Getting column mappings...");
            //MAP DB TO EXCEL COLUMN NAMES
            var closed_xml = new ClosedXMLFunctions();

            //GET STAGING FILES FOR PROCESSING
            Log.Information($"Processing working files...");
            var workingFiles = Directory.GetFiles(workingPath, "*.xls*", SearchOption.TopDirectoryOnly);
            foreach (var file in workingFiles)
            {
                var filename = Path.GetFileName(file);
                var config_sheet = _config.FileLists.Find(f => filename.ToLower().StartsWith(f.ZippedMatch.ToLower())).ExcelConfigs[0];

                var ext = Path.GetExtension(file);
                string colrange = "";
                int startingRow = 1;
                string strType = null;

                closed_xml.Mappings = getColumnMappings();
                colrange = config_sheet.ColumnRange;
                startingRow = config_sheet.StartingDataRow;

                if (filename.ToLower().Contains("_rad"))
                {
                    strType = "Rad";
                }
                else
                {
                    strType = "Card";
                }

                string cleanFileName = file;
                if (ext.ToLower() == ".xls")
                {
                    XLSToXLSXConverter.Convert(cleanFileName);
                    cleanFileName = file + "x";
                }

                var sheet = config_sheet.SheetName;
                Log.Information($"Processing " + filename + " sheet:" + sheet);

                string strLastState = null;
                var evi = closed_xml.ImportExcel<EviCoreAmerichoiceAllstatesAuthModel>(cleanFileName, sheet, colrange, startingRow, config_sheet.ColumnToValidate);
                foreach (var e in evi)
                {

                    //STATES REPEAT ARE BLANK IN SPREADSHEET
                    if (!string.IsNullOrEmpty(e.State))
                    {
                        strLastState = e.State;
                    }
                    else
                    {
                        e.State = strLastState;
                    }

                    //NOT IN SHEET
                    e.report_type = strType;
                    e.file_month = _month;
                    e.file_year = _year;
                    e.file_date = new DateTime(_year, _month, 01);
                    e.sheet_name = sheet;
                    e.file_name = filename;
                    e.file_path = directory;
                }

                //SAVE FINAL INTO DATABASE
                string[] columns = typeof(EviCoreAmerichoiceAllstatesAuthModel).GetProperties().Select(p => p.Name).ToArray();
                Log.Information($"Saving contents of EviCoreAmerichoiceAllstatesAuth to database");
                await _db.BulkSave<EviCoreAmerichoiceAllstatesAuthModel>(connectionString: sql.ConnectionString, _destination, evi, columns);
                blUpdated = true;

            }

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
                    Log.Error($"No Email found for EviCoreAmerichoiceAllstatesAuth Refresh");
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
                Log.Information($"EviCoreAmerichoiceAllstatesAuth process completed. Sending email to: " + email.EmailTo);
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

                Log.Information($"EviCoreAmerichoiceAllstatesAuth no new data found.");
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

            //Log.Information($"Searching for " + file.FilePath + "\\" + file.FileName + "...");
            //var list = Directory.GetFiles(file.FilePath, file.FileName, SearchOption.TopDirectoryOnly);
       
            Log.Information($"Searching for " + file.FilePath + "\\" + file.ZippedFile + "...");
            var list = Directory.GetFiles(file.FilePath, file.ZippedFile, SearchOption.TopDirectoryOnly);
            foreach (var f in list)
            {
                var fileName = Path.GetFileName(f).Replace(".zip", "");
                var fileParsed = fileName.Split('_');

                //var format = (fileParsed[2].Length == 3 ? "MMM" : "MMMM"); //Jan vs January
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
        }

        return filesFound;
    }



    private List<KeyValuePair<string, string>> getColumnMappings()
    {
        var list = new List<KeyValuePair<string, string>>
            {
                new KeyValuePair<string, string>("State","State"),
                new KeyValuePair<string, string>("Modality","Modality"),
                new KeyValuePair<string, string>("Month","Month"),
                new KeyValuePair<string, string>("Member Lives","Member_Lives"),
                new KeyValuePair<string, string>("Total Requests","Total_Requests"),
                new KeyValuePair<string, string>("Approved (A)","Approved"),
                new KeyValuePair<string, string>("Denied (D)","Denied"),
                new KeyValuePair<string, string>("Withdrawn (W)","Withdrawn"),
                new KeyValuePair<string, string>("Expired (Y)","Expired"),
                new KeyValuePair<string, string>("Pending","Non_Cert"),
                new KeyValuePair<string, string>("Auto Approved","Pending"),
            };

        return list;

    }


    //1. EXTRACT IConfiguration INTO PPACATATConfig
    //2. POPULATE VARIOUS appsettings ARRAYS USING Microsoft.Extensions.Configuration.Binder
    private IEviCoreAmerichoiceAllstatesAuthConfig prepareConfig(IConfiguration config)
    {


        var project = "EviCoreAmerichoiceAllstatesAuth";
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
        var cfg = config.GetSection(section).Get<List<EviCoreAmerichoiceAllstatesAuthConfig>>();
        IEviCoreAmerichoiceAllstatesAuthConfig cs = new EviCoreAmerichoiceAllstatesAuthConfig();
        if (cfg == null)
        {
            Log.Error($"No Config found for EviCoreAmerichoiceAllstatesAuth");
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
            var f = config.GetSection(section + ":" + project + ":FileLists").Get<FileExcelConfig[]>();
            if (f != null)
            {
                cs.FileLists = f.ToList <FileExcelConfig>();
            }

        }


        return cs;

    }
}
