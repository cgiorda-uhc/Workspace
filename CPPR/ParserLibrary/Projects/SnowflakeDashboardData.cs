using IdentityModel.OidcClient;
using ProjectManagerLibrary.Configuration.HeaderInterfaces.Abstract;
using ProjectManagerLibrary.Configuration.HeaderInterfaces.Concrete;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectManagerLibrary.Projects;

public class SnowflakeDashboardData : ISnowflakeDashboardData
{

    private readonly IRelationalDataAccess _db;
    private readonly ISnowflakeDashboardDataConfig? _config;
    public SnowflakeDashboardData(IConfiguration config, IRelationalDataAccess db)
    {
        //INJECT DB SOURCES
        _db = db;
        //EXTRACT CUSTOM CONFIG INTO GLOBAL ICSScorecardConfig _config
        _config = prepareConfig(config);
    }

    public async Task<long> SnowflakeDashboardDataRefresh()
    {
        //CHECK FOR CONFIG
        if (_config == null)
        {
            Log.Error($"No Config found for SnowflakeDashboardData Refresh");
            throw new OperationCanceledException();
        }

        //SADLY Serilog.sinks.email DOES NOT NOTICE "IsBodyHtml": true, in App.Settings
        //WILL USE A CUSTOM EMAILING SOLUTION FOR THESE
        //Log.Information("<b>{LogFilter} - Testing it!</b>".Replace("\"", ""), "dsv"); appsettings.json = "expression": "LogFilter = 'dsv'"
        var stopwatch = Stopwatch.StartNew();
        IEmailConfig? email = null;

        try
        {

            IRelationalDataAccess db_src = new ODBCDataAccess();
            IRelationalDataAccess db_dest = new SqlDataAccess();


            var sql_src = _config.SQLLists.Where(x => x.Name == "Source").FirstOrDefault();
            if (sql_src == null)
            {
                Log.Error($"No Source SQL found for SnowflakeDashboardData Refresh");
                throw new OperationCanceledException();
            }

            var sql_dest = _config.SQLLists.Where(x => x.Name == "Destination").FirstOrDefault();
            if (sql_dest == null)
            {
                Log.Error($"No Destination SQL found for SnowflakeDashboardData Refresh");
                throw new OperationCanceledException();
            }


            foreach (var sql in sql_src.SQL)
            {

                //GET PROPER NAME FOR NEW TABLE
                var table =  CommonFunctions.getCleanTableName(sql);
                var tmp_table = table.Substring(0, Math.Min(28, table.Length)) + "_TMP";

                Log.Information($"Retrieving Snowflake Data from " + sql + "...");
                using (IDataReader dr = (await db_src.LoadData(connectionString: sql_src.ConnectionString, "select * from " + sql)))
                {
                    ////DYNAMIC TMP TABLE USES [varchar](MAX) FOR CATCH ALL
                    List<string> columns = new List<string>();
                    for (int col = 0; col < dr.FieldCount; col++)
                    {
                        columns.Add(dr.GetName(col).ToString().ToUpper());
                    }

                    //CREATE TMP TABLE
                    Log.Information($"Creating Temp Table...");
                    var script = CommonFunctions.getCreateTmpTableScript(sql_dest.Schema, tmp_table, columns);
                    await db_dest.Execute(connectionString: sql_dest.ConnectionString, script);

                    //LOAD DATA TO TMP
                    Log.Information($"Loading Snowflake Data to Temp Table...");
                    await db_dest.BulkSave(connectionString: sql_dest.ConnectionString, sql_dest.Schema + "." + tmp_table, dr);

                    //ANALYZE ALL DATA TO DETERMIN PROPER LENGTHS AND TYPES
                    Log.Information($"Analyzing data column types...");
                    script = CommonFunctions.getTableAnalysisScript(sql_dest.Schema, tmp_table, columns);
                    var dataTypes = (await db_dest.LoadData<DataTypeModel>(connectionString: sql_dest.ConnectionString, script));

                    //CREATE FINAL TABLE USING LENGTHS AND TYPES DETERMINED ABOVE
                    Log.Information($"Creating Final Table...");
                    script = CommonFunctions.getCreateFinalTableScript(sql_dest.Schema, table, dataTypes);
                    await db_dest.Execute(connectionString: sql_dest.ConnectionString, script);

                    //MOVE TMP TO FINAL TABLE
                    Log.Information($"Transfer from Temp to Final Table...");
                    script = CommonFunctions.getSelectInsertScript(sql_dest.Schema, tmp_table, table, columns);
                    await db_dest.Execute(connectionString: sql_dest.ConnectionString, script);

                    if (!dr.IsClosed)
                    {
                        dr.Close();
                    }

                }                                                                                                               
           
            }

            email = _config.EmailLists.Where(x => x.EmailStatus == Status.Success).FirstOrDefault();
            Log.Information($"SnowflakeDashboardData process completed. Sending email to: " + email.EmailTo);
            await SharedFunctions.EmailAsync(email.EmailTo, email.EmailFrom, email.EmailSubject, "<p>" + email.EmailBody + "</p>", email.EmailCC, null, System.Net.Mail.MailPriority.Normal).ConfigureAwait(false);

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
                    await SharedFunctions.EmailAsync(email.EmailTo, email.EmailFrom, email.EmailSubject, "<p>" + email.EmailBody + "</p>", email.EmailCC, null, System.Net.Mail.MailPriority.Normal).ConfigureAwait(false);
                }
                else
                {
                    Log.Error($"No Email found for SnowflakeDashboardData Refresh");
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
            Log.Information($"SnowflakeDashboardData process completed.");
        }

        stopwatch.Stop();
        return stopwatch.ElapsedMilliseconds;

    }


    //1. EXTRACT IConfiguration INTO PPACATATConfig
    //2. POPULATE VARIOUS appsettings ARRAYS USING Microsoft.Extensions.Configuration.Binder
    private ISnowflakeDashboardDataConfig prepareConfig(IConfiguration config)
    {


        var project = "SnowflakeDashboardData";
        var section = "Automation";

        //GET APPWIDE (GENERIC) FILE STAGING PATH
        var gen = config.GetSection(section).Get<List<Generic>>();
        if (gen == null)
        {
            Log.Error($"No Generic Config found");
            throw new OperationCanceledException();
        }

        ///EXTRACT IConfiguration INTO PPACATATConfig
        var cfg = config.GetSection(section).Get<List<SnowflakeDashboardDataConfig>>();
        ISnowflakeDashboardDataConfig cs = new SnowflakeDashboardDataConfig();
        if (cfg == null)
        {
            Log.Error($"No Config found for SnowflakeDashboardData");
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
        }


        return cs;

    }
}
