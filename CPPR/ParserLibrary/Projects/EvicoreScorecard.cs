




using AutoMapper;
using System.Reflection;

namespace ProjectManagerLibrary.Projects;

public class EvicoreScorecard : IEvicoreScorecard
{
    private readonly IRelationalDataAccess _db;
    private readonly IEvicoreScorecardConfig? _config;
    private string _stagingArea;

    public EvicoreScorecard(IConfiguration config, IRelationalDataAccess db)
    {
        //INJECT DB SOURCES
        _db = db;
        //EXTRACT CUSTOM CONFIG INTO GLOBAL ICSScorecardConfig _config
        _config = prepareConfig(config);
    }
    public async Task<long> LoadEvicoreScorecardData()
    {
        //CHECK FOR CONFIG
        if (_config == null)
        {
            Log.Error($"No Config found for EvicoreScorecard data load");
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
            Log.Information($"Retrieving latest data from EvicoreScorecard...");
            //FIND LAST FILE DATE TO DETERMIN IF ANYTHING NEW WAS DROPPED
            var sql = _config.SQLLists.Where(x => x.Name == "LatestFileDate").FirstOrDefault();
            var lastestFileDate = (await _db.LoadData<FileDateModel>(connectionString: sql.ConnectionString, sql.SQL.FirstOrDefault())).FirstOrDefault();

            Log.Information($"Searching for new files...");
            //COMPARE LAST DATE WITH NEW FILESD
            var newFiles = getNewFiles(_config.FileLists, lastestFileDate);


            //var newFiles = new List<string>();
            //newFiles.Add("C:\\Users\\cgiorda\\Desktop\\ManualProjects\\Over_All_September_2023.zip");
            //_month = 9;
            //_year = 2023;
            //_destination = "stg.EviCore_Scorecard";


            if (newFiles.Count == 0)
            {
                Log.Information($"No results found for EvicoreScorecard. Will try again next time");
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

                    //var t1 = SharedFunctions.ConvertToType(_config.FileLists, typeof(FileConfig));
                    //var t2 = _config.FileLists.CastToObject<List<FileConfig>>();

                    CommonFunctions.ExtractFromZipFile(fileName, workingPath, _config.FileLists);
                }

            }

            //REFLECTION USED TO DYNAMICALLY CALL PROPERTIES WITH STRING NAMES FOUND IN MAPPINGS
            System.Reflection.PropertyInfo propModel;
            System.Reflection.PropertyInfo propSheetModel;
            List<EvicoreScorecardModel> ecsFinalList;


            Log.Information($"Getting column mappings...");
            //MAP DB TO EXCEL COLUMN NAMES
            var closed_xml = new ClosedXMLFunctions();
            closed_xml.Mappings = getColumnLOSMappings();
            var ignore = getColumnsToIgnore();
            var headerMappings = getHeaderMappings();
            //GET STAGING FILES FOR PROCESSING
            var workingFiles = Directory.GetFiles(workingPath, "*.xls*", SearchOption.TopDirectoryOnly);


            //string[] workingFiles = { "C:\\Users\\cgiorda\\Desktop\\Projects\\EvicoreScorecard\\UHC_Scorecard_2023_09.xls" };

            foreach (var file in workingFiles)
            {
                //RESET FOR NEW FILE
                ecsFinalList = new List<EvicoreScorecardModel>();
                propModel = null;
                propSheetModel = null;


                string cleanFileName = file;
                var ext = Path.GetExtension(file);
                if (ext.ToLower() == ".xls")
                {
                    XLSToXLSXConverter.Convert(cleanFileName);
                    cleanFileName = file + "x";
                }
                var filename = Path.GetFileName(cleanFileName);
                var path = Path.GetDirectoryName(cleanFileName);
                var config_sheets = _config.FileLists[0].ExcelConfigs;
                //LOOP EACH SHEET AND ADD TO EvicoreScorecardModel
                foreach (var sheet in config_sheets)
                {
                    Log.Information($"Processing " + filename + " sheet:" + sheet.SheetName);
                    var lob = closed_xml.GetValueFromExcel(cleanFileName, sheet.SheetName, sheet.SheetIdentifier);

                    //LOS STYLE SHEETS 
                    if (sheet.SheetName == "RADIOLOGY" || sheet.SheetName == "CARDIOLOGY" || sheet.SheetName == "GASTROENTEROLOGY")
                    {
                        //USE LOS MAPPINGS TO PRELOAD  List<EvicoreScorecardModel>
                        foreach (var m in closed_xml.Mappings)
                        {
                            if (m.Value != "Header")
                            {
                                ecsFinalList.Add(new EvicoreScorecardModel() { Header = m.Key, Summary_of_Lob = lob.ToString(), report_type = "UHC Scorecard", file_month = _month, file_year = _year, file_date = new DateTime(_year, _month, 01), sheet_name = sheet.SheetName, file_name = filename, file_path = path });
                            }
                        }

                        var eslos = closed_xml.ImportExcel<EvicoreScorecardSheetLOSModel>(cleanFileName, sheet.SheetName, sheet.ColumnRange, sheet.StartingDataRow);


                        if (sheet.SheetName == "GASTROENTEROLOGY")
                        {
                            //closed_xml.Mappings = getColumnGastroMappings();

                            foreach (var e in eslos.Where(x => !string.IsNullOrEmpty(x.EIAN)))
                            {
                                if (e.Header.Trim().EqualsAnyOf(ignore))
                                {
                                    continue;
                                }


                                //ex Map '% Fax' = 'Per_Fax'
                                var mapping = headerMappings.SingleOrDefault(m => m.Key.ToLower().Trim() == e.Header.ToLower().Trim());
                                var colMapped = mapping.Value;

                                foreach (var f in ecsFinalList.Where(x => x.sheet_name == sheet.SheetName))
                                {
                                    //ex MAP 'E&I - Notif.' to 'EINotif'
                                    mapping = closed_xml.Mappings.SingleOrDefault(m => m.Key.ToLower().Trim() == f.Header.ToLower().Trim());
                                    var colSheetMapped = mapping.Value;



                                    try
                                    {
                                        //DYNAMICALLY SET VALUES TO PROPERTIES BASED ON MAPPED STRINGS
                                        propModel = typeof(EvicoreScorecardModel).GetProperty(colMapped); //ex colMapped = 'Per_Fax'
                                        propSheetModel = typeof(EvicoreScorecardSheetLOSModel).GetProperty(colSheetMapped); //ex f.Header = 'EINotif'
                                        var val = propSheetModel.GetValue(e);
                                        if (val == null)
                                        {
                                            continue;
                                        }
                                        var value = SharedFunctions.ConvertToType(val, propModel.PropertyType);
                                        propModel.SetValue(f, value);
                                    }
                                    catch (Exception ex)
                                    {
                                        string exc = ex.Message;
                                    }


                                }


                            }
                        }
                        else
                        {

                            //closed_xml.Mappings = getColumnLOSMappings();
                            //var cnt = -1;
                            var es = eslos.Where(x => !string.IsNullOrEmpty(x.EINotif));


                            if (es.Count() == 0)
                            {
                              es  = eslos.Where(x => !string.IsNullOrEmpty(x.EIPA));
                            }
                           //var test2 = eslos.Where(x => !string.IsNullOrEmpty(x.EINotif)).ToList();

                            foreach (var e in es)
                            {
 

                                if (e.Header == null)
                                {
                                    continue;
                                }


                                if (e.Header.Trim().EqualsAnyOf(ignore))
                                {
                                    continue;
                                }
             

                                //ex Map '% Fax' = 'Per_Fax'
                                var mapping = headerMappings.SingleOrDefault(m => m.Key.ToLower().Trim() == e.Header.ToLower().Trim());
                                var colMapped = mapping.Value;

                                foreach (var f in ecsFinalList.Where(x => x.sheet_name == sheet.SheetName))
                                {
                                    //ex MAP 'E&I - Notif.' to 'EINotif'
                                    mapping = closed_xml.Mappings.SingleOrDefault(m => m.Key.ToLower().Trim() == f.Header.ToLower().Trim());
                                    var colSheetMapped = mapping.Value;

                                    try
                                    {
                                        //DYNAMICALLY SET VALUES TO PROPERTIES BASED ON MAPPED STRINGS
                                        propModel = typeof(EvicoreScorecardModel).GetProperty(colMapped); //ex colMapped = 'Per_Fax'
                                        propSheetModel = typeof(EvicoreScorecardSheetLOSModel).GetProperty(colSheetMapped); //ex f.Header = 'EINotif'
                                        var value = SharedFunctions.ConvertToType(propSheetModel.GetValue(e), propModel.PropertyType);
                                        propModel.SetValue(f, value);
                                    }
                                    catch (Exception ex)
                                    {
                                        string exc = ex.Message;
                                    }


                                }


                            }
                        }



                        
                    }
                    else //STATE STYLE SHEETS
                    {


                        closed_xml.Mappings = new List<KeyValuePair<string, string>>
                        {
                            new KeyValuePair<string, string>("","Header")
                        };

                        var esstate = closed_xml.ImportExcel<EvicoreScorecardSheetStateModel>(cleanFileName, sheet.SheetName, sheet.ColumnRange, sheet.StartingDataRow);

                        foreach (var p in typeof(EvicoreScorecardSheetStateModel).GetProperties())
                        {
                            if (p.Name != "Header")
                            {
                                propSheetModel = typeof(EvicoreScorecardSheetStateModel).GetProperty(p.Name); //ex f.Header = 'EINotif'
                                var value = propSheetModel.GetValue(esstate[0]);
                                if (value != null)
                                {
                                    ecsFinalList.Add(new EvicoreScorecardModel() { Header = p.Name, Summary_of_Lob = lob.ToString(), report_type = "UHC Scorecard", file_month = _month, file_year = _year, file_date = new DateTime(_year, _month, 01), sheet_name = sheet.SheetName, file_name = filename, file_path = path });
                                }
                            }
                        }


                        //var test = esstate.Where(x => x.AZ == null);

                        foreach (var e in esstate.Where(x => !string.IsNullOrEmpty(x.AZ)))
                        {
                            if (e.Header.Trim().EqualsAnyOf(ignore))
                            {
                                continue;
                            }


                            var head = e.Header;

                            //ex Map '% Fax' = 'Per_Fax'
                            var mapping = headerMappings.SingleOrDefault(m => m.Key.ToLower().Trim() == e.Header.ToLower().Trim());
                            var colMapped = mapping.Value;



                            foreach (var f in ecsFinalList.Where(x => x.sheet_name == sheet.SheetName))
                            {

                                //DYNAMICALLY SET VALUES TO PROPERTIES BASED ON MAPPED STRINGS
                                propModel = typeof(EvicoreScorecardModel).GetProperty(colMapped); //ex colMapped = 'Per_Fax'
                                propSheetModel = typeof(EvicoreScorecardSheetStateModel).GetProperty(f.Header); //ex f.Header = 'EINotif'

                                var value = propSheetModel.GetValue(e);
                                if (value != null)
                                {
                                    propModel.SetValue(f, SharedFunctions.ConvertToType(value, propModel.PropertyType));
                                }
                                //NULLABLE NUMBERS NOT FULLY WORKING!!!!
                                //PropertyInfo propertyInfo = f.GetType().GetProperty(colMapped);
                                //object value;

                                //if (propertyInfo.PropertyType.IsGenericType && propertyInfo.PropertyType.GetGenericTypeDefinition() == typeof(Nullable<>))
                                //{
                                //    if (string.IsNullOrEmpty(val + ""))
                                //        value = null;
                                //    else
                                //        value = Convert.ChangeType(val, propertyInfo.PropertyType.GetGenericArguments()[0]);
                                //}
                                //else
                                //{
                                //    value = Convert.ChangeType(val, propertyInfo.PropertyType);
                                //}
                                //propertyInfo.SetValue(f, value);

                            }


                        }
                    }
                   
                }
                //BULK LOAD CURRENT List<EvicoreScorecardModel> INTO DB
                string[] columns = typeof(EvicoreScorecardModel).GetProperties().Select(p => p.Name).ToArray();
                Log.Information($"Saving contents of " + filename + " to database");
                await _db.BulkSave<EvicoreScorecardModel>(sql.ConnectionString, _destination, ecsFinalList, columns);
                blUpdated = true;

                //ARCHIVE FILE ONCE LOADED
                var fileName = Path.GetFileName(file);
                if (!File.Exists(workingPath + "Archive\\" + fileName))
                {
                    Log.Information($"Archiving " + filename + "...");
                    File.Move(cleanFileName, workingPath + "Archive\\" + fileName);
                }
                else
                {
                    Log.Information($"Deleting " + filename + "...");
                    File.Delete(cleanFileName);
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
                    Log.Error($"No Email found for EvicoreScorecard Refresh");
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
                Log.Information($"EvicoreScorecard process completed. Sending email to: " + email.EmailTo);
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

                Log.Information($"EvicoreScorecard no new data found.");
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

            Log.Information($"Searching for " + file.FilePath + "\\" + file.FileName + "...");
            var list = Directory.GetFiles(file.FilePath, file.FileName, SearchOption.TopDirectoryOnly);
            foreach (var f in list)
            {
                //IF DATE THEN ADD
                var fileName = Path.GetFileName(f).Replace(".xlsx", "").Replace(".xls", "");
                var fileParsed = fileName.Split('_');
                month = int.TryParse(fileParsed[3], out month) ? month : 0;
                year = int.TryParse(fileParsed[2], out year) ? year : 0;

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

    //private List<string> getNewFiles(List<FileExcelConfig> fileList, FileDateModel fdate)
    //{
    //    List<string> filesFound = new List<string>();
    //    int month, year;
    //    foreach (var file in fileList)
    //    {

    //        Log.Information($"Searching for " + file.FilePath + "\\" + file.FileName + "...");
    //        var list = Directory.GetFiles(file.FilePath, file.FileName, SearchOption.TopDirectoryOnly);
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

    private string[] getColumnsToIgnore()
    {
        string[] ignore = { "Expedited Authorizations/Notifications",
                    "%TAT < State Specific TAT",
                    "SLA for TAT",
                    "Standard Authorizations/Notifications",
                    "% TAT < 48 hours"
                    };

        return ignore;
    }


    private List<KeyValuePair<string, string>> getColumnLOSMappings()
    {
        var list = new List<KeyValuePair<string, string>>
            {
                new KeyValuePair<string, string>("","Header"),
                new KeyValuePair<string, string>("E&I - Notif.","EINotif"),
                new KeyValuePair<string, string>("E&I - PA","EIPA"),
                new KeyValuePair<string, string>("E&I - AN","EIAN"),
                new KeyValuePair<string, string>("M&R","MR"),
                new KeyValuePair<string, string>("C&S","CS"),
                new KeyValuePair<string, string>("Oxford","Oxford"),
                new KeyValuePair<string, string>("NHP","NHP"),
                new KeyValuePair<string, string>("River Valley","RiverValley")
            };

        return list;

    }

    private List<KeyValuePair<string, string>> getColumnGastroMappings()
    {
        var list = new List<KeyValuePair<string, string>>
            {
                new KeyValuePair<string, string>("","Header"),
                //new KeyValuePair<string, string>("E&I - PA","EIPA"),
                new KeyValuePair<string, string>("E&I - AN","EIAN"),
                new KeyValuePair<string, string>("Oxford","Oxford"),
                new KeyValuePair<string, string>("NHP","NHP"),
                new KeyValuePair<string, string>("River Valley","RiverValley")
            };

        return list;

    }


    //private List<KeyValuePair<string, string>> getColumnLOSGastroMappings()
    //{
    //    var list = new List<KeyValuePair<string, string>>
    //        {
    //            new KeyValuePair<string, string>("","Header"),
    //            new KeyValuePair<string, string>("E&I - PA","EIPA"),
    //            new KeyValuePair<string, string>("Oxford","Oxford"),
    //            new KeyValuePair<string, string>("NHP","NHP"),
    //            new KeyValuePair<string, string>("River Valley","RiverValley")
    //        };

    //    return list;

    //}



    private List<KeyValuePair<string, string>> getHeaderMappings()
    {
        var list = new List<KeyValuePair<string, string>>
            {
                new KeyValuePair<string, string>("Total Requests","Total_Requests"),
                new KeyValuePair<string, string>("% Call","Per_Call"),
                new KeyValuePair<string, string>("% Website","Per_Website"),
                new KeyValuePair<string, string>("% Fax","Per_Fax"),
                new KeyValuePair<string, string>("% Intellipath","Per_Intellipath"),
                new KeyValuePair<string, string>("Approved","Approved"),
                new KeyValuePair<string, string>("Denied","Denied"),
                new KeyValuePair<string, string>("Withdrawn","Withdrawn"),
                new KeyValuePair<string, string>("Admin Expired","Admin_Expired"),
                new KeyValuePair<string, string>("Expired","Expired"),
                new KeyValuePair<string, string>("Pending","Pending"),
                new KeyValuePair<string, string>("Non-Cert (D + W + E) exc. Admin ex)","Non_Cert"),
                new KeyValuePair<string, string>("Requests/1000","Requests_per_thou"),
                new KeyValuePair<string, string>("Approvals/1000","Approval_per_thou"),


                 new KeyValuePair<string, string>("Notified","Notified"),
                new KeyValuePair<string, string>("Notifications/1000","Notifications_per_thou"),


                new KeyValuePair<string, string>("3DI","MOD_3DI"),
                new KeyValuePair<string, string>("BONE DENSITY","MOD_BONE_DENSITY"),
                new KeyValuePair<string, string>("CT SCAN","MOD_CT_SCAN"),
                new KeyValuePair<string, string>("MRA","MOD_MRA"),
                new KeyValuePair<string, string>("MRI","MOD_MRI"),
                new KeyValuePair<string, string>("NOT COVERED PROCEDURE","MOD_NOT_COVERED_PROCEDURE"),
                new KeyValuePair<string, string>("NUCLEAR CARDIOLOGY","MOD_NUCLEAR_CARDIOLOGY"),
                new KeyValuePair<string, string>("NUCLEAR MEDICINE","MOD_NUCLEAR_MEDICINE"),
                new KeyValuePair<string, string>("PET SCAN","MOD_PET_SCAN"),
                new KeyValuePair<string, string>("ULTRASOUND","MOD_ULTRASOUND"),
                new KeyValuePair<string, string>("UNLISTED PROCEDURE","MOD_UNLISTED_PROCEDURE"),
                new KeyValuePair<string, string>("CARDIAC CATHETERIZATION","MOD_CARDIAC_CATHETERIZATION"),
                new KeyValuePair<string, string>("CARDIAC CT/CCTA","MOD_CARDIAC_CT_CCTA"),
                new KeyValuePair<string, string>("CARDIAC IMPLANTABLE DEVICES","MOD_CARDIAC_IMPLANTABLE_DEVICES"),
                new KeyValuePair<string, string>("CARDIAC MRI","MOD_CARDIAC_MRI"),
                new KeyValuePair<string, string>("CARDIAC PET","MOD_CARDIAC_PET"),
                new KeyValuePair<string, string>("ECHO STRESS","MOD_ECHO_STRESS"),
                new KeyValuePair<string, string>("ECHO STRESS-ADDON","MOD_ECHO_STRESS_ADDON"),
                new KeyValuePair<string, string>("ECHOCARDIOGRAPHY","MOD_ECHOCARDIOGRAPHY"),
                new KeyValuePair<string, string>("ECHOCARDIOGRAPHY-ADDON","MOD_ECHOCARDIOGRAPHY_ADDON"),
                new KeyValuePair<string, string>("NUCLEAR STRESS","MOD_NUCLEAR_STRESS"),
                new KeyValuePair<string, string>("CCCM Misc Cath Codes","MOD_CCCM_Misc_Cath_Codes"),
                new KeyValuePair<string, string>("CAPSULE ENDOSCOPY","CAPSULE_ENDOSCOPY"),
                new KeyValuePair<string, string>("COLONOSCOPY","COLONOSCOPY"),
                new KeyValuePair<string, string>("DIAG RAD","DIAG_RAD"),
                new KeyValuePair<string, string>("EGD","EGD")

            };

        return list;

    }

    private IEvicoreScorecardConfig prepareConfig(IConfiguration config)
    {


        var project = "EvicoreScorecard";
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

        ///EXTRACT IConfiguration INTO EvicoreScorecardConfig
        var cfg = config.GetSection(section).Get<List<EvicoreScorecardConfig>>();
        IEvicoreScorecardConfig ecs = new EvicoreScorecardConfig();
        if (cfg == null)
        {
            Log.Error($"No Config found for EvicoreScorecard");
            throw new OperationCanceledException();
        }
        ecs = cfg.Find(p => p.Name == project);
        if (ecs != null)
        {
            //Microsoft.Extensions.Configuration.Binder
            var e = config.GetSection(section + ":" + project + ":EmailLists").Get<EmailConfig[]>();
            if (e != null)
            {
                ecs.EmailLists = e.ToList<EmailConfig>();
            }
            //Microsoft.Extensions.Configuration.Binder
            var s = config.GetSection(section + ":" + project + ":SQLLists").Get<SQLConfig[]>();
            if (s != null)
            {
                ecs.SQLLists = s.ToList<SQLConfig>();
            }

            //Microsoft.Extensions.Configuration.Binder
            var f = config.GetSection(section + ":" + project + ":FileLists").Get<FileExcelConfig[]>();
            if (f != null)
            {
                ecs.FileLists = f.ToList<FileExcelConfig>();
            }

        }


        return ecs;

    }
}
