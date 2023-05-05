using DocumentFormat.OpenXml.Drawing.Diagrams;
using DocumentFormat.OpenXml.Spreadsheet;
using FileParsingLibrary.MSExcel;
using Google.Api.Gax.ResourceNames;
using Irony.Parsing;
using ProjectManagerLibrary.Configuration.HeaderInterfaces.Abstract;
using ProjectManagerLibrary.Configuration.HeaderInterfaces.Concrete;
using System;
using System.Collections.Generic;
using System.Formats.Tar;
using System.Globalization;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VCPortal_Models.Models.ChemoPx;
using VCPortal_Models.Parameters.MHP;
using static Org.BouncyCastle.Math.EC.ECCurve;

namespace ProjectManagerLibrary.Projects
{
    public class PPACATAT : IPPACATAT
    {
        private readonly IRelationalDataAccess _db;
        private readonly IPPACATATConfig? _config;
        private string _stagingArea;

        public PPACATAT(IConfiguration config, IRelationalDataAccess db)
        {
            //INJECT DB SOURCES
            _db = db;
            //EXTRACT CUSTOM CONFIG INTO GLOBAL IPPACATATConfig _config
            _config = prepareConfig(config);
        }


        
        public async Task<long> LoadTATData()
        {
            //CHECK FOR CONFIG
            if (_config == null)
            {
                Log.Error($"No Config found for PPACATAT data load");
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
                Log.Information($"Retrieving latest data from PPACATAT...");
               //FIND LAST FILE DATE TO DETERMIN IF ANYTHING NEW WAS DROPPED
                var sql = _config.SQLLists.Where(x => x.Name == "LatestFileDate").FirstOrDefault();
                var lastestFileDate = (await _db.LoadData<FileDateModel>(connectionString: sql.ConnectionString, sql.SQL.FirstOrDefault())).FirstOrDefault();

                Log.Information($"Searching for new files...");
                //COMPARE LAST DATE WITH NEW FILESD
                var newFiles = getNewFiles(_config.FileLists, lastestFileDate);

                if (newFiles.Count == 0)
                {
                    Log.Information($"No results found for PPACATAT. Will try again next time");
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
                    if(!File.Exists(current))
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
                closed_xml.Mappings = getColumnMappings();
                //GET STAGING FILES FOR PROCESSING
                var workingFiles = Directory.GetFiles(workingPath, "*.xlsx", SearchOption.TopDirectoryOnly);
                foreach (var file in workingFiles)
                {
                    //GET ALL SHEETS
                    var sheet_names = OpenXMLFunctions.GetSheetNames(file);
                    var filename = Path.GetFileName(file);
                    //LOOP EACH SHEET AND ADD TO PPACATATModel
                    foreach (var sheet in sheet_names)
                    {
                        Log.Information($"Processing " + filename + " sheet:" + sheet);

                        if (sheet.ToLower().Equals("document map") || sheet.ToLower().Equals("sheet2"))
                        {
                            continue;
                        }
                        var lob = closed_xml.GetValueFromExcel(file, sheet, "F1");
                        var ppaca = closed_xml.ImportExcel<PPACATATModel>(file, sheet, "C3:O3", 4);
                        string strLastState = null;
                        foreach (var p in ppaca)
                        {

                            //VALUE REPEATS FOR EACH ROW
                            p.Summary_of_Lob = lob.ToString();

                            //STATES REPEAT ARE BLANK IN SPREADSHEET
                            if (!string.IsNullOrEmpty(p.Carrier_State))
                            {
                                strLastState = p.Carrier_State;
                            }
                            else
                            {
                                p.Carrier_State = strLastState;
                            }

                            //NOT IN SHEET
                            p.report_type = (filename.ToLower().Contains("routine") ? "Routine TAT" : "Urgent TAT");
                            p.file_month = _month;
                            p.file_year = _year;
                            p.file_date = new DateTime(_year, _month, 01);
                            p.sheet_name = sheet;
                            p.file_name = filename;
                            p.file_path = directory;

                        }
                        //TAT HAS BLANK/SUM COLUMNS THIS WILL REMOVE THOSE ROWS
                        ppaca.RemoveAll(o => string.IsNullOrEmpty(o.Modality));

                        //BULK LOAD CURRENT List<PPACATATModel> INTO DB
                        string[] columns = typeof(PPACATATModel).GetProperties().Select(p => p.Name).ToArray();
                        Log.Information($"Saving contents of " + filename + " sheet:" + sheet + " to database");
                        await _db.BulkSave<PPACATATModel>(sql.ConnectionString, _destination, ppaca, columns);
                        blUpdated = true;


                    }
                    //ARCHIVE FILE ONCE LOADED
                    var fileName = Path.GetFileName(file);
                    if(!File.Exists(workingPath + "Archive\\" + fileName))
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
                        Log.Error($"No Email found for PPACATAT Refresh");
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
                if(blUpdated)
                {
                    email = _config.EmailLists.Where(x => x.EmailStatus == Status.Success).FirstOrDefault();
                    Log.Information($"PPACATAT process completed. Sending email to: " + email.EmailTo);
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

                    Log.Information($"PPACATAT no new data found.");
                }

            }

            stopwatch.Stop();
            return stopwatch.ElapsedMilliseconds;


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
                foreach (var f in list)
                {
                    //IF DATE THEN ADD
                    var fileName = Path.GetFileName(f).Replace(".xlsx", "");
                    var fileParsed = fileName.Split('_');
                    month = int.TryParse(fileParsed[8], out month) ? month : 0;
                    year = int.TryParse(fileParsed[7], out year) ? year : 0;

                    if ((fdate.file_month < month && fdate.file_year == year) || fdate.file_year < year)
                    {

                        Log.Information($"Match found in " + fileName + "..");
                        filesFound.Add(f);
                        _month = month;
                        _year = year;
                        _destination = file.Destination;
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

                        var format = (fileParsed[2].Length == 3 ? "MMM" : "MMMM"); //Jan vs January
                        month = DateTime.ParseExact(fileParsed[2].Trim(), format, CultureInfo.CurrentCulture).Month;
                        year = int.TryParse(fileParsed[3], out year) ? year : 0;

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


        private List<KeyValuePair<string, string>> getColumnMappings()
        {
            var list = new List<KeyValuePair<string, string>>
            {
                new KeyValuePair<string, string>("Carrier State","Carrier_State"),
                new KeyValuePair<string, string>("Line of Business","Line_of_Business"),
                new KeyValuePair<string, string>("Modality","Modality"),
                new KeyValuePair<string, string>("Total Authorizations/Notifications","Total_Authorizations_Notifications"),
                new KeyValuePair<string, string>("<= 2 BUS Days","LessEqual_2_BUS_Days"),
                new KeyValuePair<string, string>("% <= 2 BUS Days","PerLessEqual_2_BUS_Days"),
                new KeyValuePair<string, string>("< State TAT Requirements","Less_State_TAT_Requirements"),
                new KeyValuePair<string, string>("% < State TAT Requirements","PerLess_State_TAT_Requirements"),
                new KeyValuePair<string, string>("Average Business Days","Average_Business_Days"),
                new KeyValuePair<string, string>("Average BUS Days Receipt Clinical","Average_BUS_Days_Receipt_Clinical"),
                new KeyValuePair<string, string>("Avg CAL Days Case Creation","Avg_CAL_Days_Case_Creation"),
                new KeyValuePair<string, string>("Average BUS Days Case Creation","Average_BUS_Days_Case_Creation"),
                new KeyValuePair<string, string>("Avg Business Days Denial Letter Sent","Avg_Business_Days_Denial_Letter_Sent")
            };

            return list;

        }


        //1. EXTRACT IConfiguration INTO PPACATATConfig
        //2. POPULATE VARIOUS appsettings ARRAYS USING Microsoft.Extensions.Configuration.Binder
        private IPPACATATConfig prepareConfig(IConfiguration config)
        {


            var project = "PPACA_TAT";
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
            var cfg = config.GetSection(section).Get<List<PPACATATConfig>>();
            IPPACATATConfig ppa = new PPACATATConfig();
            if (cfg == null)
            {
                Log.Error($"No Config found for PPACATAT");
                throw new OperationCanceledException();
            }
            ppa = cfg.Find(p => p.Name == project);
            if (ppa != null)
            {
                //Microsoft.Extensions.Configuration.Binder
                var e = config.GetSection(section + ":" + project + ":EmailLists").Get<EmailConfig[]>();
                if (e != null)
                {
                    ppa.EmailLists = e.ToList<EmailConfig>();
                }
                //Microsoft.Extensions.Configuration.Binder
                var s = config.GetSection(section + ":" + project + ":SQLLists").Get<SQLConfig[]>();
                if (s != null)
                {
                    ppa.SQLLists = s.ToList<SQLConfig>();
                }

                //Microsoft.Extensions.Configuration.Binder
                var f = config.GetSection(section + ":" + project + ":FileLists").Get<FileConfig[]>();
                if (f != null)
                {
                    ppa.FileLists = f.ToList<FileConfig>();
                }

            }


            return ppa;

        }

    }




}
