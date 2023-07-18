using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ProjectManagerLibrary.Configuration.HeaderInterfaces.Abstract;
using ProjectManagerLibrary.Configuration.HeaderInterfaces.Concrete;

namespace ProjectManagerLibrary.Projects
{
    public class ADDirectReportAlertsLR : IADDirectReportAlertsLR
    {

        private readonly IRelationalDataAccess _db;
        private readonly IADDirectReportAlertsLRConfig? _config;

        public ADDirectReportAlertsLR(IConfiguration config, IRelationalDataAccess db)
        {
            //INJECT DB SOURCES
            _db = db;
            //EXTRACT CUSTOM CONFIG INTO GLOBAL IADDirectReportAlertsLRConfig _config
            _config = prepareConfig(config);

        }

        public async Task<long> RefreshTable()
        {
            //CHECK FOR CONFIG
            if (_config == null)
            {
                Log.Error($"No Config found for ADDirectReportAlertsLR Refresh");
                throw new OperationCanceledException();
            }

            //SADLY Serilog.sinks.email DOES NOT NOTICE "IsBodyHtml": true, in App.Settings
            //WILL USE A CUSTOM EMAILING SOLUTION FOR THESE
            //Log.Information("<b>{LogFilter} - Testing it!</b>".Replace("\"", ""), "dsv"); appsettings.json = "expression": "LogFilter = 'dsv'"
            var stopwatch = Stopwatch.StartNew();
            IEmailConfig? email = null;

            var results = "";
            try
            {
                //GET MEMEBERS FROM ACTIVE DIRECTORY
                Log.Information($"Retrieving AD data for the ADDirectReportAlertsLR...");
                var latest = await Task.Run(() => getAllADDirectReport(_config.LDAPPath, _config.LDAPDomain, _config.LDAPUser, _config.LDAPPW, _config.SearchString));
                //GET MEMEBERS IN DB FROM PREVIOUS ACTIVE DIRECTORY
                Log.Information($"Retrieving Previous data for the ADDirectReportAlertsLR...");
                var sql = _config.SQLLists.Where(x => x.Name == "Default").FirstOrDefault();
                if (sql == null)
                {
                    Log.Error($"No SQL found for ADDirectReportAlertsLR Refresh");
                    throw new OperationCanceledException();
                }
                var previous = (await _db.LoadData<ADDirectReportAlertsLRModel>(sql.ConnectionString, "SELECT * FROM " + sql.SQL.FirstOrDefault() + " ")).ToList();
                //COMPARE 2 LISTS FOR ADDITIONS OR REMOVAL
                Log.Information($"Comparing results...");
                if(previous.Count == 0)
                {

                    Log.Information($"No previous data found. Adding data and aborting...");

                    string[] columns = typeof(ADDirectReportAlertsLRModel).GetProperties().Select(p => p.Name).ToArray();
                    Log.Information($"Bulk Load Table...");
                    await _db.BulkSave<ADDirectReportAlertsLRModel>(sql.ConnectionString, sql.SQL.FirstOrDefault(), latest, columns);

                    stopwatch.Stop();
                    return stopwatch.ElapsedMilliseconds;
                }


                results = compareResults(latest, previous);
                //NO CHANGES DO NOTHING
                if (results == null)
                {
                    Log.Information($"No changes detected");
                    //email = _config.EmailLists.Where(x => x.EmailStatus == Status.Information).FirstOrDefault();
                }
                else//CHANGES REFRESH DB WITH LATEST
                {
                    Log.Information($"Changes detected");

                    Log.Information($"Truncate Table...");
                    await _db.Execute(sql.ConnectionString, "Truncate Table " + sql.SQL.FirstOrDefault());


                    string[] columns = typeof(ADDirectReportAlertsLRModel).GetProperties().Select(p => p.Name).ToArray();
                    Log.Information($"Bulk Load Table...");
                    await _db.BulkSave<ADDirectReportAlertsLRModel>(sql.ConnectionString, sql.SQL.FirstOrDefault(), latest, columns);



                    email = _config.EmailLists.Where(x => x.EmailStatus == Status.Success).FirstOrDefault();
                    Log.Information($"ADDirectReportAlertsLR process completed. Sending email to: " + email.EmailTo);
                    await SharedFunctions.EmailAsync(email.EmailTo, email.EmailFrom, email.EmailSubject, "<p>" + email.EmailBody + "</p><p>" + results + "</p>", email.EmailCC, null, System.Net.Mail.MailPriority.Normal).ConfigureAwait(false);
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
                        Log.Error($"No Email found for ADDirectReportAlertsLR Refresh");
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
                Log.Information($"ADDirectReportAlertsLR process completed.");
            }

            stopwatch.Stop();
            return stopwatch.ElapsedMilliseconds;

        }


        //RETURNS LIST OF ALL MEMBERS WITHIN GROUP_PREFIX_*
        private List<ADDirectReportAlertsLRModel> getAllADDirectReport(string path, string domain, string username, string password, List<string> managers)
        {


            ActiveDirectory ad = new ActiveDirectory(path, domain, username, password);
            List<ADDirectReportAlertsLRModel> _usersFinal = new List<ADDirectReportAlertsLRModel>();
            List<ADUserModel> _users;


            foreach (string mng in managers)
            {
                ad.UsersByManagerList = new List<ADUserModel>();
                ad.GetUsersByManager(mng);
                _users = ad.UsersByManagerList;
                foreach (var u in _users)
                {
                    if (string.IsNullOrEmpty(u.EmailAddress))//NOT THE MAIN ACCOUNT
                        continue;

                    var m = u.Manager;

                    _usersFinal.Add(new ADDirectReportAlertsLRModel {userid=u.LoginName, email = u.EmailAddress, employee_name = u.FullName, manager_name = ( m == null?  null : m.FullName)});
                }
            }
            return _usersFinal;
        }

        //COMPARES CURRENT AND PREVIOUS MEMBER LISTS AND RETURNS STRING OF NAMES ADDED OR REMOVED
        private string? compareResults(List<ADDirectReportAlertsLRModel> latest, List<ADDirectReportAlertsLRModel> previous)
        {

            bool blChanges = false;
            StringBuilder sb = new StringBuilder();
            sb.AppendLine("Added to DirectReports:<br/>");
            sb.AppendLine("<ul>");
            foreach (var item in latest)
            {
                if (!previous.Any(l => l.userid == item.userid && l.email == item.email && l.employee_name == item.employee_name && l.manager_name == item.manager_name))
                {
                    sb.AppendLine("<li>" + item.userid + ", " + item.email + ", " + item.employee_name + ", " + item.manager_name + "</li>");
                    blChanges = true;
                }
            }
            sb.AppendLine("</ul>");
            sb.AppendLine("<br/><br/>");

            sb.AppendLine("Removed from DirectReports:<br/>");
            sb.AppendLine("<ul>");
            foreach (var item in previous)
            {
                if (!latest.Any(l => l.userid == item.userid && l.email == item.email && l.employee_name == item.employee_name && l.manager_name == item.manager_name))
                {
                    sb.AppendLine("<li>" + item.userid + ", " + item.email + ", " + item.employee_name + ", " + item.manager_name + "</li>");
                    blChanges = true;
                }
            }
            sb.AppendLine("</ul>");

            if (blChanges)
                return sb.ToString();
            else
                return null;
        }


        //1. EXTRACT IConfiguration INTO PBIMembershipConfig 
        //2. POPULATE VARIOUS appsettings ARRAYS USING Microsoft.Extensions.Configuration.Binder
        private IADDirectReportAlertsLRConfig prepareConfig(IConfiguration config)
        {
            var project = "ADDirectReportAlertsLR";
            var section = "Automation";
            ///EXTRACT IConfiguration INTO PBIMembershipConfig 
            var cfg = config.GetSection(section).Get<List<ADDirectReportAlertsLRConfig>>();
            IADDirectReportAlertsLRConfig ad = new ADDirectReportAlertsLRConfig();
            if (cfg != null)
            {
                ad = cfg.Find(p => p.Name == project);
                if (ad != null)
                {
                    //Microsoft.Extensions.Configuration.Binder
                    var e = config.GetSection(section + ":" + project + ":EmailLists").Get<EmailConfig[]>();
                    if (e != null)
                    {
                        ad.EmailLists = e.ToList<EmailConfig>();
                    }
                    //Microsoft.Extensions.Configuration.Binder
                    var s = config.GetSection(section + ":" + project + ":SQLLists").Get<SQLConfig[]>();
                    if (s != null)
                    {
                        ad.SQLLists = s.ToList<SQLConfig>();
                    }


                }

            }

            return ad;

        }
    }
}
