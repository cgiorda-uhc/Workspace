using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace ProjectManagerLibrary.Projects;

public class SiteOfCareGastro : ISiteOfCare
{

    private readonly IRelationalDataAccess _db;
    private readonly ISiteOfCareConfig? _config;
    private string _stagingArea;

    public SiteOfCareGastro(IConfiguration config, IRelationalDataAccess db)
    {
        //INJECT DB SOURCES
        _db = db;
        //EXTRACT CUSTOM CONFIG INTO GLOBAL ICSScorecardConfig _config
        _config = prepareConfig(config);
    }

    public async Task<long> LoadSiteOfCareData()
    {
        //CHECK FOR CONFIG
        if (_config == null)
        {
            Log.Error($"No Config found for SiteOfCare Gastro data load");
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
            Log.Information($"Retrieving latest data from SiteOfCare...");
            //FIND LAST FILE DATE TO DETERMIN IF ANYTHING NEW WAS DROPPED
            var sql = _config.SQLLists.Where(x => x.Name == "LatestFileDate").FirstOrDefault();
            var lastestFileDate = (await _db.LoadData<FileDateModel>(connectionString: sql.ConnectionString, sql.SQL.FirstOrDefault())).FirstOrDefault();

            //var lastestFileDate = new FileDateModel() { file_month = 5, file_year = 2023, file_date = new DateTime(2023, 5, 1) };




            Log.Information($"Searching for new files...");
            //COMPARE LAST DATE WITH NEW FILESD
            var newFiles = getNewFiles(_config.FileLists, lastestFileDate);
            if (newFiles.Count == 0)
            {
                Log.Information($"No results found for SiteOfCare. Will try again next time");
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
            var workingFiles = Directory.GetFiles(workingPath, "*.xlsx", SearchOption.TopDirectoryOnly);

            var config_sheet = _config.FileLists[0].ExcelConfigs[0];
            foreach (var file in workingFiles)
            {
                var sheet = config_sheet.SheetName;
                var filename = Path.GetFileName(file);

                Log.Information($"Processing " + filename + " sheet:" + sheet);
                var soc = closed_xml.ImportExcel<SiteOfCareModel>(file, sheet, config_sheet.ColumnRange, config_sheet.StartingDataRow);
                foreach (var s in soc)
                {

                    //NOT IN SHEET
                    s.report_type = "Gastro";
                    s.file_month = _month;
                    s.file_year = _year;
                    s.file_date = new DateTime(_year, _month, 01);
                    s.sheet_name = sheet;
                    s.file_name = filename;
                    s.file_path = directory;

                }


                //BULK LOAD CURRENT List<SiteOfCareModel> INTO DB
                string[] columns = typeof(SiteOfCareModel).GetProperties().Select(p => p.Name).ToArray();
                Log.Information($"Saving contents of " + filename + " sheet:" + sheet + " to database");
                await _db.BulkSave<SiteOfCareModel>(sql.ConnectionString, _destination, soc, columns);
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
                    Log.Error($"No Email found for SiteOfCare Refresh");
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
                Log.Information($"SiteOfCare process completed. Sending email to: " + email.EmailTo);
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

                Log.Information($"SiteOfCare no new data found.");
            }

        }

        stopwatch.Stop();
        return stopwatch.ElapsedMilliseconds;

    }





    private string _destination;
    private int _month;
    private int _year;

    //private List<string> getNewFiles(List<FileExcelConfig> fileList, FileDateModel fdate)
    //{
    //    List<string> filesFound = new List<string>();
    //    int month, year;
    //    foreach (var file in fileList)
    //    {

    //        Log.Information($"Searching for " + file.FilePath + "\\" + file.FileName + "...");
    //        var list = Directory.GetFiles(file.FilePath, file.FileName, SearchOption.TopDirectoryOnly);
    //        foreach (var f in list)
    //        {
    //            //IF DATE THEN ADD
    //            var fileName = Path.GetFileName(f).Replace(".xlsx", "").Replace(".xls", "");
    //            var fileParsed = fileName.Split('_');
    //            month = int.TryParse(fileParsed[3], out month) ? month : 0;
    //            year = int.TryParse(fileParsed[2], out year) ? year : 0;

    //            if ((fdate.file_month < month && fdate.file_year == year) || fdate.file_year < year)
    //            {

    //                Log.Information($"Match found in " + fileName + "..");
    //                filesFound.Add(f);
    //                _month = month;
    //                _year = year;
    //                _destination = file.Destination;
    //            }

    //        }

    //        if (!string.IsNullOrEmpty(file.ZippedFile))
    //        {
    //            Log.Information($"Searching for " + file.FilePath + "\\" + file.ZippedFile + "...");
    //            list = Directory.GetFiles(file.FilePath, file.ZippedFile, SearchOption.TopDirectoryOnly);
    //            foreach (var f in list)
    //            {
    //                var fileName = Path.GetFileName(f).Replace(".zip", "");
    //                var fileParsed = fileName.Split('_');

    //                var format = (fileParsed[2].Length == 3 ? "MMM" : "MMMM"); //Jan vs January
    //                month = DateTime.ParseExact(fileParsed[2].Trim(), format, CultureInfo.CurrentCulture).Month;
    //                year = int.TryParse(fileParsed[3], out year) ? year : 0;

    //                if ((fdate.file_month < month && fdate.file_year == year) || fdate.file_year < year)
    //                {
    //                    Log.Information($"Match found in " + fileName + "..");
    //                    filesFound.Add(f);
    //                    _month = month;
    //                    _year = year;
    //                    _destination = file.Destination;
    //                }
    //            }
    //        }

    //    }
    //    return filesFound;
    //}




    private List<string> getNewFiles(List<FileExcelConfig> fileList, FileDateModel fdate)
    {
        List<string> filesFound = new List<string>();
        int month, year;
        foreach (var file in fileList)
        {

            Log.Information($"Searching for " + file.FilePath + "\\" + file.FileName + "...");
            var list = Directory.GetFiles(file.FilePath, file.FileName, SearchOption.TopDirectoryOnly);
            foreach (var f in list)
            {
                //IF DATE THEN ADD
                var fileName = Path.GetFileName(f).Replace(".xlsx", "").Replace(".xls", "");
                var fileParsed = fileName.Split('_');
                month = int.TryParse(fileParsed[2], out month) ? month : 0;
                year = int.TryParse(fileParsed[1], out year) ? year : 0;

                if ((fdate.file_month < month && fdate.file_year == year) || fdate.file_year < year)
                {

                    Log.Information($"Match found in " + fileName + "..");
                    filesFound.Add(f);
                    _month = month;
                    _year = year;
                    _destination = file.Destination;
                }

            }




            Log.Information($"Searching for " + file.FilePath + "\\" + file.FileName + "...");
             list = Directory.GetFiles(file.FilePath, file.FileName, SearchOption.TopDirectoryOnly);
            if (!string.IsNullOrEmpty(file.ZippedFile))
            {
                Log.Information($"Searching for " + file.FilePath + "\\" + file.ZippedFile + "...");
                list = Directory.GetFiles(file.FilePath, file.ZippedFile, SearchOption.TopDirectoryOnly);
                foreach (var f in list)
                {
                    var fileName = Path.GetFileName(f).Replace(".zip", "");
                    var fileParsed = fileName.Split('-')[1].Trim().Split(' ');

                    var format = (fileParsed[0].Length == 3 ? "MMM" : "MMMM"); //Jan vs January
                    month = DateTime.ParseExact(fileParsed[0].Trim(), format, CultureInfo.CurrentCulture).Month;
                    year = int.TryParse(fileParsed[1], out year) ? year : 0;

                    if ((fdate.file_month < month && fdate.file_year == year) || fdate.file_year < year)
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


    private ISiteOfCareConfig prepareConfig(IConfiguration config)
    {


        var project = "SiteOfCareGastro";
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
        var cfg = config.GetSection(section).Get<List<SiteOfCareConfig>>();
        ISiteOfCareConfig cs = new SiteOfCareConfig();
        if (cfg == null)
        {
            Log.Error($"No Config found for SiteOfCare");
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
                cs.FileLists = f.ToList<FileExcelConfig>();
            }

        }


        return cs;

    }
}
