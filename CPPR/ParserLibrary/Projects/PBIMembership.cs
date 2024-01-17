

using AutoMapper;
using ProjectManagerLibrary.Configuration.HeaderInterfaces.Abstract;
using ProjectManagerLibrary.Configuration.HeaderInterfaces.Concrete;


namespace ProjectManagerLibrary.Projects
{
    public class PBIMembership : IPBIMembership
    {
        private readonly IRelationalDataAccess _db;
        private readonly IPBIMembershipConfig? _config;

        public PBIMembership(IConfiguration config, IRelationalDataAccess db )
        {
            //INJECT DB SOURCES
            _db = db;
            //EXTRACT CUSTOM CONFIG INTO GLOBAL IPBIMembershipConfig _config
            _config = prepareConfig(config);

        }

        public async Task<long> RefreshTable()
        {
            //CHECK FOR CONFIG
            if (_config == null)
            {
                Log.Error($"No Config found for PBIMembership Refresh");
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
                Log.Information($"Retrieving AD data for the PBIMembership...");
                var latest = await Task.Run(() => getAllADMembers(_config.LDAPPath, _config.LDAPDomain, _config.LDAPUser, _config.LDAPPW, _config.SearchString));
                //GET MEMEBERS IN DB FROM PREVIOUS ACTIVE DIRECTORY
                Log.Information($"Retrieving Previous data for the PBIMembership...");
                var sql =  _config.SQLLists.Where(x => x.Name == "Default").FirstOrDefault();
                if ( sql == null ) 
                {
                    Log.Error($"No SQL found for PBIMembership Refresh");
                    throw new OperationCanceledException();
                }
                var previous = (await _db.LoadData<PBIMembershipModel>(sql.ConnectionString,"SELECT * FROM " + sql.SQL.FirstOrDefault() + " ")).ToList();
                //COMPARE 2 LISTS FOR ADDITIONS OR REMOVAL
                Log.Information($"Comparing results...");
                results = compareResults(latest, previous);

                //NO CHANGES DO NOTHING
                if(results == null)
                {
                    Log.Information($"No changes detected");
                    //email = _config.EmailLists.Where(x => x.EmailStatus == Status.Information).FirstOrDefault();
                }
                else//CHANGES REFRESH DB WITH LATEST
                {
                    Log.Information($"Changes detected");

                    Log.Information($"Truncate Table...");
                    await _db.Execute(sql.ConnectionString, "Truncate Table " + sql.SQL.FirstOrDefault());

                    string[] columns = typeof(PBIMembershipModel).GetProperties().Select(p => p.Name).ToArray();
                    Log.Information($"Bulk Load Table...");
                    await _db.BulkSave<PBIMembershipModel>(sql.ConnectionString, sql.SQL.FirstOrDefault(), latest, columns);
                    //email = _config.EmailLists.Where(x => x.EmailStatus == Status.Success).FirstOrDefault();
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
                        Log.Error($"No Email found for PBIMembership Refresh");
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
                Log.Information($"PBIMembership process completed.");
            }

            stopwatch.Stop();
            return stopwatch.ElapsedMilliseconds;
 
        }

        //RETURNS LIST OF ALL MEMBERS WITHIN GROUP_PREFIX_*
        private List<PBIMembershipModel> getAllADMembers(string path, string domain, string username, string password, string group)
        {
            List<PBIMembershipModel> lstFS = new List<PBIMembershipModel>();

            ActiveDirectory ad = new ActiveDirectory(path, domain, username, password);

            var groups = ad.GetGroupByName(group);
            foreach (var g in groups)
            {
                var grp = g.Replace("CN=", "");
                var users = ad.GetUserFromGroup(grp);
                foreach (var u in users)
                {
                    lstFS.Add(new PBIMembershipModel { userid = u.LoginName, email = u.EmailAddress, department = u.Department, global_group = grp });
                }
            }

            return lstFS.ToList();
        }

        //COMPARES CURRENT AND PREVIOUS MEMBER LISTS AND RETURNS STRING OF NAMES ADDED OR REMOVED
        private string? compareResults(List<PBIMembershipModel> latest, List<PBIMembershipModel> previous)
        {

            bool blChanges = false;
            StringBuilder sb = new StringBuilder();
            sb.AppendLine("Members Added:<br/>");
            sb.AppendLine("<ul>");
            foreach (var item in latest)
            {
                if (!previous.Any(l => l.userid == item.userid && l.email == item.email && l.department == item.department && l.global_group == item.global_group))
                {
                    sb.AppendLine("<li>" + item.userid + ", " + item.email + ", " + item.department + ", " + item.global_group + "</li>");
                    blChanges = true;
                }
            }
            sb.AppendLine("</ul>");
            sb.AppendLine("<br/><br/>");

            sb.AppendLine("Members Removed:<br/>");
            sb.AppendLine("<ul>");
            foreach (var item in previous)
            {
                if (!latest.Any(l => l.userid == item.userid && l.email == item.email && l.department == item.department && l.global_group == item.global_group))
                {
                    sb.AppendLine("<li>" + item.userid + ", " + item.email + ", " + item.department + ", " + item.global_group + "</li>");
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
        private IPBIMembershipConfig prepareConfig(IConfiguration config)
        {
            var project = "PBIMembership";
            var section = "Automation";
            ///EXTRACT IConfiguration INTO PBIMembershipConfig 
            var cfg = config.GetSection(section).Get<List<PBIMembershipConfig>>();
            IPBIMembershipConfig pbi = new PBIMembershipConfig() ;
            if (cfg != null)
            {
                pbi = cfg.Find(p => p.Name == project);
                if (pbi != null)
                {
                    //Microsoft.Extensions.Configuration.Binder
                    var e = config.GetSection(section + ":" + project + ":EmailLists").Get<EmailConfig[]>();
                    if (e != null)
                    {
                        pbi.EmailLists = e.ToList<EmailConfig>();
                    }
                    //Microsoft.Extensions.Configuration.Binder
                    var s = config.GetSection(section + ":" + project + ":SQLLists").Get<SQLConfig[]>();
                    if (s != null)
                    {
                        pbi.SQLLists = s.ToList<SQLConfig>();
                    }


                }

            }

            return pbi;

        }

        //public static U AutoMapChemotherapyPX<T, U>(IConfiguration config, string project, T input) where U : IAppsettings
        //{

        //    var section = "Automation";
        //    ///EXTRACT IConfiguration INTO PBIMembershipConfig 
        //    var cfg = config.GetSection(section).Get<List<T>>();
        //    U _config;
        //    if (cfg != null)
        //    {
        //        _config = cfg.Find(p => p.Name == project);
        //        if (_config != null)
        //        {
        //            //Microsoft.Extensions.Configuration.Binder
        //            var e = config.GetSection(section + ":" + project + ":EmailLists").Get<EmailConfig[]>();
        //            if (e != null)
        //            {
        //                _config.EmailLists = e.ToList<EmailConfig>();
        //            }
        //            //Microsoft.Extensions.Configuration.Binder
        //            var s = config.GetSection(section + ":" + project + ":SQLLists").Get<SQLConfig[]>();
        //            if (s != null)
        //            {
        //                _config.SQLLists = s.ToList<SQLConfig>();
        //            }


        //        }

        //    }

        //}



    }
}
