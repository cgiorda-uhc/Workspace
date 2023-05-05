

using DataAccessLibrary.Models;
using DataAccessLibrary.Scripts;
using DataAccessLibrary.Shared;
using DocumentFormat.OpenXml.Wordprocessing;
using NPOI.HSSF.Record.Chart;
using System.Globalization;

namespace ProjectManagerLibrary.Projects;

public class MHPUniverse : IMHPUniverse
{

    private readonly IRelationalDataAccess _db;
    private readonly IMHPUniverseConfig? _config;
    private string _stagingArea;



    public MHPUniverse(IConfiguration config, IRelationalDataAccess db)
    {

        //INJECT DB SOURCES
        _db = db;
        //EXTRACT CUSTOM CONFIG INTO GLOBAL ICSScorecardConfig _config
        _config = prepareConfig(config);
    }

    public async Task<long> LoadMHPUniverseData()
    {
        //CHECK FOR CONFIG
        if (_config == null)
        {
            Log.Error($"No Config found for MHPUniverse data load");
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
        List<string> files_loaded = new List<string>();
        try
        {
            Log.Information($"Retrieving latest data from MHPUniverse...");
            //FIND LAST FILE DATE TO DETERMIN IF ANYTHING NEW WAS DROPPED
            var sql = _config.SQLLists.Where(x => x.Name == "LatestFileDate").FirstOrDefault();
            var lastestFileDates = (await _db.LoadData<FileDateModel>(connectionString: sql.ConnectionString, sql.SQL.FirstOrDefault())).ToList();


            Log.Information($"Searching for new files...");
            //COMPARE LAST DATE WITH NEW FILESD
            var newFiles = getNewFiles(_config.FileLists, lastestFileDates);


            if (newFiles.Count == 0)
            {
                Log.Information($"No results found for MHPUniverse. Will try again next time");
                stopwatch.Stop();
                return stopwatch.ElapsedMilliseconds;
            }


            //DOWNLOAD NEW FILES TO STAGING AREA FOR PROCESSING
            string directory = null;
            Log.Information($"Copying new files to " + workingPath + "...");
            foreach (var file in newFiles)
            {
                if (directory == null)
                    directory = Path.GetDirectoryName(file.FileName);

                var fileName = Path.GetFileName(file.FileName);
                var current = workingPath + fileName;
                if (!File.Exists(current))
                {
                    File.Copy(file.FileName, current);
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
                var ext = Path.GetExtension(file);
                string cleanFileName = file;
                if (ext.ToLower() == ".xls")
                {
                    XLSToXLSXConverter.Convert(cleanFileName);
                    cleanFileName = file + "x";
                }



                var files_found = newFiles.Where(x => Path.GetFileName(x.FileName) == filename).FirstOrDefault();
                var config_sheets = _config.FileLists.Find(f => f.FileName.ToLower().StartsWith(files_found.Name.ToLower())).ExcelConfigs;

                string colrange = "";
                int startingRow = 1;
                string strType = null;


                foreach (var cfg in config_sheets)
                {
                    closed_xml.Mappings = getColumnMappings();
                    colrange = cfg.ColumnRange;
                    startingRow = cfg.StartingDataRow;

                    if (filename.ToLower().Contains("_rad"))
                    {
                        strType = "RAD";
                    }
                    else
                    {
                        strType = "CARD";
                    }

                    var sheet = cfg.SheetName;
                    Log.Information($"Processing " + filename + " sheet:" + sheet);

                    string strLastState = null;
                    var mhp = closed_xml.ImportExcel<MHPUniverseModel>(cleanFileName, sheet, colrange, startingRow);
                    var mhp_final = mhp.Distinct().ToList();//MHP IS KNOWN FOR DUPLICATE ROWS
                    foreach (var m in mhp_final)
                    {
                        //NOT IN SHEET
                        m.file_month = files_found.Month;
                        m.file_year = files_found.Year;
                        m.file_date = new DateTime(files_found.Year, files_found.Month, 01);
                        m.sheet_name = sheet;//strType
                        m.file_name = filename;
                        m.file_path = directory;
                        m.classification = cfg.SheetIdentifier;

                    }

                    //SAVE FINAL INTO DATABASE
                    string[] columns = typeof(MHPUniverseModel).GetProperties().Select(p => p.Name).ToArray();
                    Log.Information($"Saving contents of MHPUniverse to database");
                    await _db.BulkSave<MHPUniverseModel>(connectionString: sql.ConnectionString, files_found.Destination, mhp_final, columns);
                    blUpdated = true;
                }
                files_loaded.Add(filename);
            }
            //RELOAD DUE TO CONVERSION XLS TO XLSX
            workingFiles = Directory.GetFiles(workingPath, "*.xls*", SearchOption.TopDirectoryOnly);
            //ARCHIVE FILE ONCE LOADED
            foreach (var file in workingFiles)
            {
                var fileName = Path.GetFileName(file);
                if (!File.Exists(workingPath + "Archive\\" + fileName))
                {
                    Log.Information($"Archiving " + fileName + "...");
                    File.Move(file, workingPath + "Archive\\" + fileName);
                }
                else
                {
                    Log.Information($"Deleting " + file + "...");
                    File.Delete(file);
                }
            }

            Log.Information($"Starting UGAP member cleanup.....");
            await cleanupMemberDataAsync(files_loaded);


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
                    Log.Error($"No Failure Email found for MHPUniverse Refresh");
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
                try
                {
                    email = _config.EmailLists.Where(x => x.EmailStatus == Status.Success).FirstOrDefault();
                    Log.Information($"MHPUniverse process completed. Sending email to: " + email.EmailTo);

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

                Log.Information($"MHPUniverse no new data found.");
            }

        }

        stopwatch.Stop();
        return stopwatch.ElapsedMilliseconds;

    }



    private List<FileFound> getNewFiles(List<FileExcelConfig> fileList, List<FileDateModel> fdate)
    {
        List<FileFound> filesFound = new List<FileFound>();
        int month, year;
        foreach (var file in fileList)
        {

            var date = fdate.Where(x => file.FileName.ToLower().StartsWith(x.name.ToLower())).FirstOrDefault();

            //var file_path = file.FilePath.Replace("*", (date.file_year + (date.file_month == 12 ? 1  : 0)).ToString());
            var file_path = file.FilePath;
            Log.Information($"Searching for " + file_path + "\\" + file.FileName + "...");
            var list = Directory.GetFiles(file_path, file.FileName, SearchOption.TopDirectoryOnly);
            foreach (var f in list)
            {
                var fileName = Path.GetFileName(f).Replace(".xlsx", "").Replace(".xls", "");
                //var fileParsed = fileName.Replace("_", " ").Replace("-", "").Split(' ');
                var fileParsed = fileName.Replace("-", " ").Replace("_", " ").Split(' ');

                var month_pos = 5;
                var year_pos = 1;
                if (date.name == "United")
                {
                    month_pos = 2;
                    year_pos = 1;
                }

                month = 0;
                DateTime dt;
                if (DateTime.TryParseExact(fileParsed[fileParsed.Length - month_pos].Trim(), "MMMM", CultureInfo.CurrentCulture, DateTimeStyles.None, out dt))
                {
                    month = dt.Month;
                }
                year = int.TryParse(fileParsed[fileParsed.Length - year_pos].Trim(), out year) ? year : 0;


                if (month == 0)
                {
                    fileParsed = fileName.Replace("-", " ").Replace("_", " ").Split(' ');
                    foreach (string p in fileParsed)
                    {
                        var isInt = DateTime.TryParseExact(p.Trim(), "MMMM", CultureInfo.CurrentCulture, DateTimeStyles.None, out dt);
                        if (isInt)
                        {
                            month = dt.Month;
                            break;
                        }
                    }
                }


                //if ((date.file_month < month && date.file_year + (date.file_month == 12 ? + 1 : 0) == year) || date.file_year < year)
                if ((date.file_month < month && date.file_year  == year) || date.file_year < year)
                {
                    var ff = new FileFound();
                    ff.Name = date.name;
                    ff.FileName = f;
                    ff.Month = month;
                    ff.Year = year;
                    ff.Destination = file.Destination;
                    if (!filesFound.Contains(ff))
                    {
                        Log.Information($"Match found in " + fileName + "..");
                        filesFound.Add(ff);
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

            new KeyValuePair<string, string>("State of Issue","State_of_Issue"),
            new KeyValuePair<string, string>("State of Residence","State_of_Residence"),
            new KeyValuePair<string, string>("Enrollee First Name","Enrollee_First_Name"),
            new KeyValuePair<string, string>("Enrollee Last Name","Enrollee_Last_Name"),
            new KeyValuePair<string, string>("Cardholder ID","Cardholder_ID"),
            new KeyValuePair<string, string>("Funding Arrangement","Funding_Arrangement"),
            new KeyValuePair<string, string>("Authorization","Authorization"),
            new KeyValuePair<string, string>("Authorization Type","Authorization_Type"),
            new KeyValuePair<string, string>("Date the request was received","Request_Date"),
            new KeyValuePair<string, string>("Time the request was received","Request_Time"),
            new KeyValuePair<string, string>("Request Decision","Request_Decision"),
            new KeyValuePair<string, string>("Date of Decision","Decision_Date"),
            new KeyValuePair<string, string>("Time of Decision","Decision_Time"),
            new KeyValuePair<string, string>("Decision Reason","Decision_Reason"),
            new KeyValuePair<string, string>("Was Extension Taken","Extension_Taken"),
            new KeyValuePair<string, string>("Was Extension Taken?","Extension_Taken"),
            new KeyValuePair<string, string>("Date of member notification of extension","Member_Notif_Extension_Date"),
            new KeyValuePair<string, string>("Date additional information received","Additional_Info_Date"),
            new KeyValuePair<string, string>("Date oral notification provided to enrollee","Oral_Notification_Enrollee_Date"),
            new KeyValuePair<string, string>("Time oral notification provided to enrollee","Oral_Notification_Enrollee_Time"),
            new KeyValuePair<string, string>("Date oral notification provided to provider","Oral_Notification_Provider_Date"),
            new KeyValuePair<string, string>("Time oral notification provided to provider","Oral_Notification_Provider_Time"),
            new KeyValuePair<string, string>("Date written notification sent to enrollee","Written_Notification_Enrollee_Date"),
            new KeyValuePair<string, string>("Time written notification sent to enrollee","Written_Notification_Enrollee_Time"),
            new KeyValuePair<string, string>("Date written notification sent to provider","Written_Notification_Provider_Date"),
            new KeyValuePair<string, string>("Time written notification sent to provider","Written_Notification_Provider_Time"),
            new KeyValuePair<string, string>("Primary Procedure Code(s) Requested","Primary_Procedure_Code_Req"),
            new KeyValuePair<string, string>("Primary Procedure Code Requested","Primary_Procedure_Code_Req"),
            new KeyValuePair<string, string>("Procedure Code Description","Procedure_Code_Description"),
            new KeyValuePair<string, string>("Primary Diagnosis Code","Primary_Diagnosis_Code"),
            new KeyValuePair<string, string>("Diagnosis Description","Diagnosis_Code_Description"),
            new KeyValuePair<string, string>("Diagnosis Code Description","Diagnosis_Code_Description"),
            new KeyValuePair<string, string>("Place of Service","Place_of_Service"),
            new KeyValuePair<string, string>("Member Date of Birth","Member_Date_of_Birth"),
            new KeyValuePair<string, string>("Was an urgent request made but processed as standard?","Urgent_Processed_Standard"),
            new KeyValuePair<string, string>("Date of request for additional information","Request_Additional_Info_Date"),
            new KeyValuePair<string, string>("Date additional information requested","Request_Additional_Info_Date"),
            new KeyValuePair<string, string>("First Tier, Downstream, and Related Entity","FirstTier_Downstream_RelatedEntity"),
            new KeyValuePair<string, string>("Par/Non-Par Site","Par_NonPar_Site"),
            new KeyValuePair<string, string>("PAR/NON PAR","Par_NonPar_Site"),
            new KeyValuePair<string, string>("Par Non Par Site","Par_NonPar_Site"),
            new KeyValuePair<string, string>("Par Non/ Par Site","Par_NonPar_Site"),
            new KeyValuePair<string, string>("Par-Non-Par Site","Par_NonPar_Site"),
            new KeyValuePair<string, string>("Par/Non-Par","Par_NonPar_Site"),
            new KeyValuePair<string, string>("Inpatient/Outpatient","Inpatient_Outpatient"),
            new KeyValuePair<string, string>("Inpatient Outpatient","Inpatient_Outpatient"),
            new KeyValuePair<string, string>("Inpatient /Outpatient","Inpatient_Outpatient"),
            new KeyValuePair<string, string>("Delegate Number","Delegate_Number"),
            new KeyValuePair<string, string>("ProgramType","ProgramType"),
            new KeyValuePair<string, string>("Program Type","ProgramType"),
            new KeyValuePair<string, string>("Insurance Carrier","Insurance_Carrier"),
            new KeyValuePair<string, string>("InsCarrier","Insurance_Carrier"),
            new KeyValuePair<string, string>("Insurance_Carrier","Insurance_Carrier"),
            new KeyValuePair<string, string>("Group Number","Group_Number"),
            new KeyValuePair<string, string>("Intake Method","Intake_Method"),
            new KeyValuePair<string, string>("MethodofContactDesc","Intake_Method")
        };

        return list;

    }

    private async Task cleanupMemberDataAsync(List<string> files_loaded)
    {
        //GET NEEDED CONFIGS
        var sql_cfg = _config.SQLLists.Where(x => x.Name == "MHP_DB").FirstOrDefault();
        var connectionString = sql_cfg.ConnectionString;
        var tableMHP = sql_cfg.SQL.FirstOrDefault();

        sql_cfg = _config.SQLLists.Where(x => x.Name == "UGAP_DB").FirstOrDefault();
        var tdConnectionString = sql_cfg.ConnectionString;
        var tableUGAP = sql_cfg.SQL.FirstOrDefault();
        var limit = sql_cfg.Limit;


        var files_csv = "'" + string.Join("','", files_loaded.Select(n => n.ToString()).ToArray()) + "'";

        //TWO DBS
        IRelationalDataAccess db_td = new TeraDataAccess();
        IRelationalDataAccess db_sql = new SqlDataAccess();

        //DRIVING LOOP
        var parameters = MHPCustomSQL.MHPParameters();

        string sql;
        StringBuilder sbSQL = new StringBuilder();

        int total;
        int total_counter;
        int limit_counter;
        var columns = typeof(MHPMemberDetailsModel).GetProperties().Select(p => p.Name).ToArray();
        foreach (var param in parameters)
        {
            Log.Information($"Seaching for " + param.SearchMethod + " within " + param.LOS  + "...");
            sql = MHPCustomSQL.MSSQLMHPMember(tableMHP, tableUGAP, files_csv, param.MHPSQL);
            //FIND CURRENT MEMBERS
            var mhp_search = (await db_sql.LoadData<MHPMemberSearchModel>(connectionString: connectionString, sql));
            total = mhp_search.Count();
            Log.Information(total + " records found");
            total_counter = 0;
            limit_counter = 0;

            foreach (var m in mhp_search)
            {
                sbSQL.Append(MHPCustomSQL.UGAPVolatileInsert(m, param));
                limit_counter++;
                total_counter++;
                if (limit_counter == limit)
                {
                    Log.Information("Searching UGAP for " + total_counter + " out of " + total);
                    if (param.LOS == LOS.EI || param.LOS == LOS.EI_OX)
                        sql = MHPCustomSQL.UGAPSQLLMemberDataEI(param.UGAPSQL, param.LOS == LOS.EI_OX).Replace("{$Inserts}", sbSQL.ToString());
                    else
                        sql = MHPCustomSQL.UGAPSQLMemberDataCS(param.UGAPSQL, param.LOS == LOS.CS).Replace("{$Inserts}", sbSQL.ToString());

                    var ugap = await db_td.LoadData<MHPMemberDetailsModel>(connectionString: tdConnectionString, sql);
                    foreach (var u in ugap)
                    {
                        u.SearchMethod = param.SearchMethod;
                    }

                    Log.Information("Loading " + ugap.Count() + " UGAP rows into MHP source." );
                    await db_sql.BulkSave<MHPMemberDetailsModel>(connectionString: connectionString, tableUGAP, ugap, columns);



                    sbSQL.Remove(0, sbSQL.Length);
                    limit_counter = 0;
                }
            }
            //FINISHED BEFORE LIMIT SO PROCESS REMAINDER
            if (sbSQL.Length > 0)
            {
                Log.Information("Searching UGAP for " + total_counter + " out of " + total);

                if (param.LOS == LOS.EI || param.LOS == LOS.EI_OX)
                    sql = MHPCustomSQL.UGAPSQLLMemberDataEI(param.UGAPSQL, param.LOS == LOS.EI_OX).Replace("{$Inserts}", sbSQL.ToString());
                else
                    sql = MHPCustomSQL.UGAPSQLMemberDataCS(param.UGAPSQL, param.LOS == LOS.CS).Replace("{$Inserts}", sbSQL.ToString());

                var ugap = await db_td.LoadData<MHPMemberDetailsModel>(connectionString: tdConnectionString, sql);
                foreach (var u in ugap)
                {
                    u.SearchMethod = param.SearchMethod;
                }

                Log.Information("Loading " + ugap.Count() + " UGAP rows into MHP source.");
                await db_sql.BulkSave<MHPMemberDetailsModel>(connectionString: connectionString, tableUGAP, ugap, columns);

                sbSQL.Remove(0, sbSQL.Length);

            }

        }
    }



    private IMHPUniverseConfig prepareConfig(IConfiguration config)
    {


        var project = "MHPUniverse";
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
        var cfg = config.GetSection(section).Get<List<MHPUniverseConfig>>();
        IMHPUniverseConfig cs = new MHPUniverseConfig();
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
                cs.FileLists = f.ToList<FileExcelConfig>();
            }

        }


        return cs;

    }

}
