
using System.Data;
using System.Globalization;
using DBConnectionLibrary;
using System.Collections.Concurrent;
using ProjectManagerLibrary.Configuration.HeaderInterfaces.Concrete;
using ProjectManagerLibrary.Configuration.HeaderInterfaces.Abstract;

namespace ProjectManagerLibrary.Concrete
{
    public class DataSourceVerification : IDataSourceVerification
    {
        private readonly IDataSourceVerificationConfig? _config;
        public DataSourceVerification(IConfiguration config)
        {
            //EXTRACT CUSTOM CONFIG INTO GLOBAL IDataSourceVerificationConfig _config
            _config = prepareConfig(config);
        }

        public async Task<long> CheckDataSources()
        {
            //SADLY Serilog.sinks.email DOES NOT NOTICE "IsBodyHtml": true, in App.Settings
            //WILL USE A CUSTOM EMAILING SOLUTION FOR THESE

            //CHECK FOR CONFIG
            if (_config == null)
            {
                Log.Error($"No Config found for DataSourceVerification");
                throw new OperationCanceledException();
            }

            var stopwatch = Stopwatch.StartNew();
            IEmailConfig? email = null;
            var emailBody = "";
            try
            {

                Log.Information($"Retrieving driving data for the DataSourceVerification...");
                //GET DRIVING DATA FROM DB INTO A LIST
                var sql = _config.SQLLists.Where(x => x.Name == "Default").FirstOrDefault();
                if (sql == null)
                {
                    Log.Error($"No SQL found for DataSourceVerification");
                    throw new OperationCanceledException();
                }
                //GET DRIVING LIST
                List<DataSource> lstDS= await getDataSourcesAsync(sql.ConnectionString, sql.SQL.FirstOrDefault()).ConfigureAwait(false);
                //CHECK FOR NEW FILES
                emailBody = await checkFileSources(lstDS.Where(x => x.SourceType == "File").ToList()).ConfigureAwait(false);
                //CHECK FOR NEW DB Data DATABASES
                email = _config.EmailLists.Where(x => x.EmailStatus == Status.Success).FirstOrDefault();

            }
            catch (Exception ex)
            {
                Log.Error(ex.ToString());
                email = _config.EmailLists.Where(x => x.EmailStatus == Status.Failure).FirstOrDefault();
                throw new OperationCanceledException();
            }
            finally
            {
                if(email != null)
                {
                    Log.Information($"DataSourceVerification process completed. Sending email to: " + email.EmailTo);
                    //await SharedFunctions.EmailAsync("chris_giordano@uhc.com", "chris_giordano@uhc.com", "Data Source Verification", strEmailBody, null, null, System.Net.Mail.MailPriority.Normal).ConfigureAwait(false);
                    try
                    {
                        await SharedFunctions.EmailAsync(email.EmailTo, email.EmailFrom, email.EmailSubject, emailBody, email.EmailCC, null, System.Net.Mail.MailPriority.Normal).ConfigureAwait(false);
                    }
                    catch (Exception ex)
                    {
                        Log.Error(ex.ToString());
                        throw new OperationCanceledException();
                    }
                }
                else
                {
                    Log.Warning($"DataSourceVerification process completed. But no emails were found!");
                }

            }
            stopwatch.Stop();
            return stopwatch.ElapsedMilliseconds;
        }


        private static readonly ConcurrentBag<Task> tasks = new ConcurrentBag<Task>();

        private static Task<string> checkFileSources(List<DataSource> ds)
        {
            return TaskHelper.FromResultOf(() =>                          
            {
                //COLLECT INFO FORM EMAIL
                StringBuilder sbDetails = new StringBuilder();
                StringBuilder sbErrors = new StringBuilder();

                Log.Information($"Searching for {ds.Count} new files");
                string[] strPartArr;
                int? intMonth = null;
                int? intYear = null;
                string? strCheck = null;
                int? intMonthCheck = null;
                int? intYearCheck = null;
                List<string>? lstNewFile = null;
                int intCheckCnt = 1;

                foreach (var s in ds)
                {
                    Log.Information($"Searching #{intCheckCnt} " + s.SourceName);
                    var subsearch = s.HasDateFolders;
                    var tableName = s.DestinationName;
                    var fileNameFull = s.SourceName;
                    var fileName = s.SourceName.Replace(".xlsb", "").Replace(".zip", "").Replace(".xlsx", "").Replace(".xls", "").Replace(".txt", "");
                    var filePath = s.SourceRoute;
                    var fileSearch = s.SearchString;
                    strPartArr = fileName.Split('_');


                    if (fileSearch.StartsWith("Oxford"))
                    {
                        string ss = "";
                    }

                    //IF FILE NAME IS SEPARATED WITH '_' THEN LOOP THROUGH THEM SEACHING FOR DATE AND MONTH
                    if (strPartArr.Length > 1)
                    {
                        foreach (string p in strPartArr)
                        {
                            if (p.All(char.IsNumber))
                            {
                                if (p.Length == 2)
                                {
                                    intMonth = int.Parse(p);
                                }
                                else if (p.Length == 4)
                                {
                                    intYear = int.Parse(p);
                                }
                                else if (p.Length == 6 || p.Length == 8)
                                {
                                    intYear = int.Parse(p.Substring(0, 4));
                                    intMonth = int.Parse(p.Substring(4, 2));
                                }


                            }
                        }
                        // MONTH NOT FOUND? THEN CHECK FOR FULL NAME EX: January = 1;
                        if (intMonth == null)
                        {
                            DateTime dt;
                            foreach (string p in strPartArr)
                            {
                                var isInt = DateTime.TryParseExact(p.Trim(), "MMMM", CultureInfo.CurrentCulture, DateTimeStyles.None, out dt);
                                if (isInt)
                                {
                                    intMonth = dt.Month;
                                    break;
                                }
                            }
                        }

                    }
                    else//NOT SEPARATED WITH '_' SO TRY TO AT LEAST EXTRACT THE YEAR
                    {

                        strCheck = Regex.Match(fileName, @"\d+").Value;
                        if (strCheck.Length == 4)
                        {
                            intYear = int.Parse(strCheck);
                        }
                    }

                    //YTD - Cisco - UHC Metrics 2022_09.xlsx
                    if (intYear == null && intMonth != null)
                    {
                        var arr = strPartArr[0].Split(' ');
                        int intValue = int.TryParse(arr[arr.Length - 1], out intValue) ? intValue : 0;
                        intYear = (intValue == 0 ? null : intValue);
                    }

                    //Oxford January -Radiology Cardiology Universe 2023.xlsx
                    if (intYear != null && intMonth == null)
                    {
                        DateTime dt;
                        strPartArr  = fileName.Replace("-"," ").Replace("_", " ").Split(' ');
                        foreach (string p in strPartArr)
                        {
                            var isInt = DateTime.TryParseExact(p.Trim(), "MMMM", CultureInfo.CurrentCulture, DateTimeStyles.None, out dt);
                            if (isInt)
                            {
                                intMonth = dt.Month;
                                break;
                            }
                        }
                    }

                    //IS FOUND BOTH WE CAN PROCEED WITH THE CHECK
                    if (intYear != null && intMonth != null)
                    {
                        //'|' IS USED TO SEPARATE MULTIPLE FILE SEARCH PATTERNS IF NEED BE
                        string[] strFileSearchArr = fileSearch.Split('|');

                        //SOME PATHS REQUIRE US TO DYNAMICALLY SEARCH SUB FOLDERS LIKE 2022/09
                        string strLastYearPath = "";
                        if (subsearch == true)
                        {
                            strLastYearPath = @"\" + (DateTime.Now.Year - (intMonth == 11 ? 1 : 0)); //LOOK BACK TO PREVIOUS YEAR IF WERE ON FINAL MONTH
                        }

                        //LOOP THROUGH SEARCH PATTERNS
                        foreach (string fs in strFileSearchArr)
                        {
                            //FIND ALL POTENTIAL MATCHES
                            //SYNCHRONOUS
                            // List<string> files = Directory.EnumerateFiles(filePath + strLastYearPath, fs.Trim(), (subsearch == true ? SearchOption.AllDirectories : SearchOption.TopDirectoryOnly)).ToList();
                            //ASYNCHRONOUS
                            //https://stackoverflow.com/questions/34579606/asynchronously-enumerate-folders
                            List<string> files = new List<string>();
                            
                            try
                            {
                                tasks.Add(Task.Run(() =>
                                {
                                    if(Directory.Exists(filePath + strLastYearPath))
                                    {

                                        files = Directory.EnumerateFiles(filePath + strLastYearPath, fs.Trim(), (subsearch == true ? SearchOption.AllDirectories : SearchOption.TopDirectoryOnly)).ToList<string>();
                                    }
                   
                                }));
                                Task? taskToWaitFor;
                                while (tasks.TryTake(out taskToWaitFor))
                                    taskToWaitFor.Wait();
                            }
                            catch(Exception ex)
                            {
                                sbErrors.Append(ex.Message + "<br/>");

                            }

                            foreach (string f in files)
                            {

                                //IGNORE MANUALLY TWEAKED FILES!!!!!
                                if (f.ToLower().Contains("summary") || f.ToLower().Contains("preview") || f.ToLower().Contains("edited") || f.ToLower().Contains("orig") || f.ToLower().Contains("variance"))
                                    continue;


                                //CHECK RESULTS AGAINST LATEST FILES LOADED TO SEE IF WE HAVE A NEW FILE 
                                fileName = Path.GetFileName(f).Replace(".xlsb", "").Replace(".zip", "").Replace(".xlsx", "").Replace(".xls", "").Replace(".txt", "");
                                strPartArr = fileName.Split('_');

                                if(strPartArr.Length == 1)
                                {
                                    strPartArr = fileName.Replace("-", " ").Replace("_", " ").Split(' ');
                                }

                                if (strPartArr.Length > 1)
                                {
                                    foreach (string p in strPartArr)
                                    {
                                        if (p.All(char.IsNumber))
                                        {
                                            if (p.Length == 2)
                                            {
                                                intMonthCheck = int.Parse(p);
                                            }
                                            else if (p.Length == 4)
                                            {
                                                intYearCheck = int.Parse(p);
                                            }
                                            else if (p.Length == 6 || p.Length == 8)
                                            {
                                                intYearCheck = int.Parse(p.Substring(0, 4));
                                                intMonthCheck = int.Parse(p.Substring(4, 2));
                                            }

                                        }
                                        else
                                        {
                                            DateTime dt;
                                            bool blIsInt;
                                            blIsInt = DateTime.TryParseExact(p.Trim(), "MMM", CultureInfo.CurrentCulture, DateTimeStyles.None, out dt);
                                            if (blIsInt)
                                            {
                                                intMonthCheck = dt.Month;
                                            }
                                            else
                                            {
                                                blIsInt = DateTime.TryParseExact(p.Trim(), "MMMM", CultureInfo.CurrentCulture, DateTimeStyles.None, out dt);
                                                if (blIsInt)
                                                {
                                                    intMonthCheck = dt.Month;
                                                }
                                            }
                                        }
                                    }


                                    //IF THE FILE FOUND IS LATER THAN THE LAST ADD IT TO A LIST TO SHARE
                                    if ((intYearCheck == intYear && intMonthCheck > intMonth) || (intYearCheck > intYear))
                                    {
                                        if (lstNewFile == null)
                                            lstNewFile = new List<string>();

                                        lstNewFile.Add(Path.GetFileName(f));

                                    }

                                }
                                

                            }
                        }

                        //ADDING VEBIRAGE FOR EMAIL
                        sbDetails.Append("<b>Check #" + intCheckCnt + " for <font color=\"green\">" + tableName.TrimEnd('_', 'c').TrimEnd('_', 'r').TrimEnd('_', 'c', 's').TrimEnd('_', 'p', 'c', 'p').TrimEnd('_', 'o', 'x') + "</font></b><br/>");
                        sbDetails.Append("<b>Path searched:</b> " + filePath.Replace(" ", "%20").Replace("&", "&amp;") + "<br/>");
                        sbDetails.Append("<b>Search String(s):</b> " + fileSearch + "<br/>");
                        sbDetails.Append("<b>Last file loaded:</b>  " + fileNameFull + "<br/>");

                        //IF THERE WERE ERRORS SKIP AND LOG THOSE!!!
                        if (sbErrors.Length == 0)
                        {
                            //IF WE FOUND YEAR AND MONTH 
                            if (intYearCheck != null && intMonthCheck != null)
                            {
                                //AND FOUND NEW
                                if (lstNewFile != null)
                                {
                                    sbDetails.Append("<div style=\"background-color:#90EE90\"><b>New file(s) found: " + string.Join(",", lstNewFile) + "</b></div><br/>");
                                }
                                else
                                    sbDetails.Append("No new file found. Will try again tomorrow.<br/>");

                            }
                            else
                                sbDetails.Append("No new file found. Will try again tomorrow.<br/>");
                        }
                        else
                        {
                            sbDetails.Append("<div style=\"background-color:#FF2400\"><b>Error:" + sbErrors.ToString() +"</b></div>");
                        }
                    

                        sbDetails.Append("--------------------------------------------------------------------<br/>&nbsp;<br/>");

                        intCheckCnt++;

                        //CLEAR OUT OLD DATA AND TRY AGAIN
                        intYear = null;
                        intMonth = null;
                        intYearCheck = null;
                        intMonthCheck = null;
                        lstNewFile = null;
                        sbErrors.Remove(0, sbErrors.Length);

                    }
                }
                return sbDetails.ToString();          
            });
        }


        private async Task<List<DataSource>> getDataSourcesAsync(string strConnectionString, string strSQL)
        {
            List<DataSource> lstFS = new List<DataSource>();

            using (SqlDataReader dr = await DBConnection.GetMSSQLRecordSetAsync(strConnectionString, strSQL).ConfigureAwait(false))
            {
                while (await dr.ReadAsync().ConfigureAwait(false))
                {
            
                    lstFS.Add(new DataSource()
                    {
                        LastUpdateDate = await dr.GetFieldValueAsync<DateTime>(dr.GetOrdinal("LastUpdateDate")).ConfigureAwait(false),
                        SourceName = await dr.GetFieldValueAsync<string>(dr.GetOrdinal("SourceName")).ConfigureAwait(false),
                        SearchString = await dr.GetFieldValueAsync<string>(dr.GetOrdinal("SearchString")).ConfigureAwait(false),
                        SourceRoute = await dr.GetFieldValueAsync<string>(dr.GetOrdinal("SourceRoute")).ConfigureAwait(false),
                        DestinationName = await dr.GetFieldValueAsync<string>(dr.GetOrdinal("DestinationName")).ConfigureAwait(false),
                        HasDateFolders = (await dr.GetFieldValueAsync<int>(dr.GetOrdinal("HasDateFolders")).ConfigureAwait(false) != 0),
                        SourceType = await dr.GetFieldValueAsync<string>(dr.GetOrdinal("SourceType")).ConfigureAwait(false)
                    });

                }
            }


            return lstFS.ToList();
        }

        //1. EXTRACT IConfiguration INTO DataSourceVerificationConfig 
        //2. POPULATE VARIOUS appsettings ARRAYS USING Microsoft.Extensions.Configuration.Binder
        private IDataSourceVerificationConfig prepareConfig(IConfiguration config)
        {
            var project = "DataSourceVerification";
            var section = "Automation";
            ///EXTRACT IConfiguration INTO PBIMembershipConfig 
            var cfg = config.GetSection(section).Get<List<DataSourceVerificationConfig>>();
            IDataSourceVerificationConfig dsv = new DataSourceVerificationConfig();
            if (cfg != null)
            {
                dsv = cfg.Find(p => p.Name == project);
                if (dsv != null)
                {
                    //Microsoft.Extensions.Configuration.Binder
                    var e = config.GetSection(section + ":" + project + ":EmailLists").Get<EmailConfig[]>();
                    if (e != null)
                    {
                        dsv.EmailLists = e.ToList<EmailConfig>();
                    }
                    //Microsoft.Extensions.Configuration.Binder
                    var s = config.GetSection(section + ":" + project + ":SQLLists").Get<SQLConfig[]>();
                    if (s != null)
                    {
                        dsv.SQLLists = s.ToList<SQLConfig>();
                    }


                }

            }

            return dsv;

        }
        
    }
}
