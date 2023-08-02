using DocumentFormat.OpenXml.InkML;
using DocumentFormat.OpenXml.Spreadsheet;
using MongoDB.Driver.Core.Configuration;
using NPOI.HPSF;
using SharpCompress.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace ProjectManagerLibrary.Projects;

public class EviCoreYTDMetrics : IEviCoreYTDMetrics
{
    private readonly IRelationalDataAccess _db;
    private readonly IEviCoreYTDMetricsConfig? _config;
    private string _stagingArea;

    public EviCoreYTDMetrics(IConfiguration config, IRelationalDataAccess db)
    {
        //INJECT DB SOURCES
        _db = db;
        //EXTRACT CUSTOM CONFIG INTO GLOBAL ICSScorecardConfig _config
        _config = prepareConfig(config);
    }

    public async Task<long> LoadEviCoreYTDMetricsData()
    {
        //CHECK FOR CONFIG
        if (_config == null)
        {
            Log.Error($"No Config found for EviCoreYTDMetrics data load");
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
            Log.Information($"Retrieving latest data from EviCoreYTDMetrics...");
            //FIND LAST FILE DATE TO DETERMIN IF ANYTHING NEW WAS DROPPED
            var sql = _config.SQLLists.Where(x => x.Name == "LatestFileDate").FirstOrDefault();
            var lastestFileDate = (await _db.LoadData<FileDateModel>(connectionString: sql.ConnectionString, sql.SQL.FirstOrDefault())).FirstOrDefault();


            Log.Information($"Searching for new files...");
            //COMPARE LAST DATE WITH NEW FILESD
            var newFiles = getNewFiles(_config.FileLists, lastestFileDate);


            if (newFiles.Count == 0)
            {
                Log.Information($"No results found for EviCoreYTDMetrics. Will try again next time");
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
            var reportType = "Cisco UHC Metrics";
            //FULL MONTH NAME IS NEEDED TO USE REFLECTION TO ACCESS IMPORT PROPERTIES
            var month_name = new DateTime(_year, _month, 1).ToString("MMMM");
            //USE HISTORY TO DETERMINE VALID SHEET NAMES
            sql = _config.SQLLists.Where(x => x.Name == "ValidSheets").FirstOrDefault();
            var sheet_ref = await _db.LoadData<string>(connectionString: sql.ConnectionString, sql.SQL.FirstOrDefault());
            //CAPTURE EACH ROW FOR EXPORT
            EviCoreYTDMetricsModel data_model = null;
            //WILL HOLD FINAL CLEAN LIST FOR EXPORT
            var data_models = new List<EviCoreYTDMetricsModel>();
            //GET CUSTOM CONFIGURATIONS FOR EXCEL
            var config_sheet = _config.FileLists.FirstOrDefault().ExcelConfigs[0];
            //GET MAPPINGS FOR EXPORT
            var mappings = getExportMappings();
            closed_xml.Mappings = getImportMappings();

            var workingFiles = Directory.GetFiles(workingPath, "*.xls*", SearchOption.TopDirectoryOnly);
            foreach (var file in workingFiles)
            {
                var filename = Path.GetFileName(file);
                var filepath = Path.GetDirectoryName(file);


                //GET ALL SHEETS
                var sheet_names = OpenXMLFunctions.GetSheetNames(file);
                //LOOP EACH SHEET AND ADD TO CSScorecardModel
                foreach (var sheet in sheet_names)
                {
                    Log.Information($"Processing " + filename + " sheet:" + sheet);

                    var chk = sheet_ref.Where(fi => fi.ToLower().Trim().Contains(sheet.Trim().ToLower()));
                    if (!chk.Any() && !sheet.ToLower().Contains("for_new_sheet_names"))
                    {
                        continue;
                    }


                    //if(sheet.ToLower().Contains("gastro"))
                    //{
                    //    var s = "";
                    //}


                    var lob = closed_xml.GetValueFromExcel(file, sheet, config_sheet.SheetIdentifier);
                    var export = closed_xml.ImportExcel<YTDCiscoExportModel>(file, sheet, config_sheet.ColumnRange, config_sheet.StartingDataRow, nullCheck: config_sheet.ColumnToValidate);

                    foreach (var e in export)
                    {

                        if (e.Call_Center_Statistics.ToLower().Trim().StartsWith("intake"))
                        {
                            if (data_model != null)
                            {
                                finalizeExportRow(data_model, lob.ToString(), filename, filepath, sheet, reportType, ref data_models);
                            }

                            data_model = new EviCoreYTDMetricsModel();
                            data_model.Call_Taker = "Intake";
                        }
                        else if (e.Call_Center_Statistics.ToLower().Trim().StartsWith("medical"))
                        {
                            if (data_model != null)
                            {
                                finalizeExportRow(data_model, lob.ToString(), filename, filepath, sheet, reportType, ref data_models);
                            }

                            data_model = new EviCoreYTDMetricsModel();
                            data_model.Call_Taker = "MD";
                        }
                        else if (e.Call_Center_Statistics.ToLower().Trim().StartsWith("nurse"))
                        {
                            break;
                        }

                        var mapping = mappings.Where(m => m.Key.ToLower().Trim() == e.Call_Center_Statistics.ToLower().Trim());
                        if (mapping.Count() > 0)
                        {

                            var val = e.GetType().GetProperty(month_name).GetValue(e, null);
                            PropertyInfo propertyInfo = data_model.GetType().GetProperty(mapping.FirstOrDefault().Value);
                            object value;
                            if (propertyInfo.PropertyType.IsGenericType && propertyInfo.PropertyType.GetGenericTypeDefinition() == typeof(Nullable<>))
                            {
                                if (string.IsNullOrEmpty(val + ""))
                                    value = null;
                                else
                                    value = Convert.ChangeType(val, propertyInfo.PropertyType.GetGenericArguments()[0]);
                            }
                            else
                            {
                                value = Convert.ChangeType(val, propertyInfo.PropertyType);
                            }
                            //propertyInfo.SetValue(f, Convert.ChangeType(value, propertyInfo.PropertyType), null);
                            propertyInfo.SetValue(data_model, value, null);
                        }


                    }
                    if (data_model != null)
                    {
                        finalizeExportRow(data_model, lob.ToString(), filename, filepath, sheet, reportType, ref data_models);
                    }
                    data_model = null;

                }

            }


            //BULK LOAD CURRENT List<EvicoreScorecardModel> INTO DB
            string[] columns = typeof(EviCoreYTDMetricsModel).GetProperties().Select(p => p.Name).ToArray();
            Log.Information($"Saving contents of EviCoreYTDMetrics to database");
            await _db.BulkSave<EviCoreYTDMetricsModel>(sql.ConnectionString, _destination, data_models, columns);
            blUpdated = true;



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
                    Log.Information($"Deleting " + fileName + "...");
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
                    Log.Error($"No Email found for EviCoreYTDMetrics Refresh");
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
                Log.Information($"EviCoreYTDMetrics process completed. Sending email to: " + email.EmailTo);
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

                Log.Information($"EviCoreYTDMetrics no new data found.");
            }

        }

        stopwatch.Stop();
        return stopwatch.ElapsedMilliseconds;

    }

    private void finalizeExportRow(EviCoreYTDMetricsModel data_model, string lob, string filename, string filepath, string sheet, string reportType, ref List<EviCoreYTDMetricsModel> data_models)
    {
        data_model.Summary_of_Lob = lob.ToString();
        data_model.file_month = _month;
        data_model.file_year = _year;
        data_model.file_date = new DateTime(_year, _month, 01);
        data_model.sheet_name = sheet;//strType
        data_model.file_name = filename;
        data_model.file_path = filepath;
        data_model.report_type = reportType;
        data_models.Add(data_model);
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



    private List<KeyValuePair<string, string>> getImportMappings()
    {
        var list = new List<KeyValuePair<string, string>>
            {
                new KeyValuePair<string, string>("Call Center Statistics","Call_Center_Statistics")
            };

        return list;

    }

    private List<KeyValuePair<string, string>> getExportMappings()
    {
        var list = new List<KeyValuePair<string, string>>
            {
                new KeyValuePair<string, string>("Total Calls","Total_Calls"),
                new KeyValuePair<string, string>("ACD Calls","Total_Calls"),
                new KeyValuePair<string, string>("Average Answer Speed","Avg_Speed_Answer"),
                new KeyValuePair<string, string>("Total Calls Abandoned","Abandoned_Calls"),
                new KeyValuePair<string, string>("Aban Calls","Abandoned_Calls"),
                new KeyValuePair<string, string>("% Of Calls Abandoned","Abandoned_Percent"),
                 new KeyValuePair<string, string>("% Abn Calls","Abandoned_Percent"),
                new KeyValuePair<string, string>("Average Answer Speed","Avg_Speed_Answer"),
                new KeyValuePair<string, string>("Avg ACD Time","Avg_Speed_Answer"),
                new KeyValuePair<string, string>("Average Talk Time","Average_Talk_Time"),
                new KeyValuePair<string, string>("% In Service Level","ASA_in_SL_Perent"),
                new KeyValuePair<string, string>("% Ans Calls","ASA_in_SL_Perent")
            };

        return list;

    }

    private IEviCoreYTDMetricsConfig prepareConfig(IConfiguration config)
    {


        var project = "EviCoreYTDMetrics";
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
        var cfg = config.GetSection(section).Get<List<EviCoreYTDMetricsConfig>>();
        IEviCoreYTDMetricsConfig cs = new EviCoreYTDMetricsConfig();
        if (cfg == null)
        {
            Log.Error($"No Config found for EviCoreYTDMetrics");
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
