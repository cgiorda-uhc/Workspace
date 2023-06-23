using ProjectManagerLibrary.Shared;
using Serilog;
using System.Timers;

using CAD_Worker_Service.Shared;
using IdentityModel.OidcClient;
using ProjectManagerLibrary.Configuration.HeaderInterfaces;
using DataAccessLibrary.DataAccess;
using ProjectManagerLibrary.Configuration.RoleInterfaces;
using DocumentFormat.OpenXml.Math;
using DocumentFormat.OpenXml.Office2013.Drawing.ChartStyle;

namespace CAD_Worker_Service
{
    public class Worker : BackgroundService
    {

        private const int _intDelay = 5 * 1000; //60*1000 = 1 MINUTE IDEAL FOR LIVE!

        private int _intWorkerCnt;


        //ALL PARSING LIBRARIES HERE
        private TasksManager? _tasksManager;

        //READ appsettings.json
        private readonly IConfiguration _config;
        //private readonly List<AppSettings> _configuration;
        private readonly IRelationalDataAccess _db;

        //USED TO TRACK ALL PARSER CONFIGS

        //HOLD ALL TASK FOR EASY DISPOSAL
        private readonly List<CronosTimer> _scheduledTasks;

        //DEFAULT constructor
        public Worker(IConfiguration config, IRelationalDataAccess db)
        {
            //SERILOG OVERRIDES ILogging, NO NEED TO PASS IT IN
            _config = config;
            _db = db;
            _intWorkerCnt = 0;
            _scheduledTasks = new List<CronosTimer>();
        }

        public override Task StartAsync(CancellationToken cancellationToken)
        {
            Log.Information($"Worker starting....");

            //TasksManager WILL HANDLE CALLING ALL FUNCTIONS DIRECTLY AND PASSING IN PARAMETERS TO EACH (_options)
            _tasksManager = new TasksManager(_config, _db);

            //USED TO POPULATE GLOBAL _scheduledTasks.Add(timer);
            loadTasks();

            //CALL BASE FUNCTIONS
            return base.StartAsync(cancellationToken);
        }


        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            if (_tasksManager == null)
                _tasksManager = new TasksManager(_config, _db);

            while (!stoppingToken.IsCancellationRequested)
            {
                //PAUSE TO CHECK FOR CANCELLATION
                await Task.Delay(_intDelay, stoppingToken).ConfigureAwait(false); //1 * 1000 = 1 second

                _intWorkerCnt++;
                Log.Information($"Worker has run {_intWorkerCnt} times");
                
            }
        }


        public override Task StopAsync(CancellationToken cancellationToken)
        {
            Log.Information($"Worker stopping....");
            if (_tasksManager != null)
                _tasksManager.Dispose();


            foreach(CronosTimer t in _scheduledTasks)
            {
                Task.Run(async () =>
                {
                   await t.DisposeAsync();
                });
            }


            return base.StopAsync(cancellationToken);
        }

        //https://stackoverflow.com/questions/74391108/creating-a-function-that-returns-a-generic-function
        //EVENT HANDLER FOR CronosTimer
        public Func<CancellationToken, Task> HandleTimerElapsed(Func<CancellationToken, Task> func)
        {

            return async cancellationToken =>
            {
                try
                {
                    Task result = func(cancellationToken);
                    await result;
 
                    if (result.IsCompletedSuccessfully)
                    {
                        Log.Information($"VC Job Scheduler successfully ran. Status code {result.Status}"); //ONLY IN TASK MANAGER PERHAPS????
                    }
                    else
                    {
                        Log.Error($"VC Job Scheduler failed. Status code {result.Status}"); //ONLY IN TASK MANAGER PERHAPS????
                    }

                }
                catch (Exception ex)
                {
                    Log.Error(ex.ToString());
                }
            };
        }

        //HERE IS WHERE A LIST OF TASKS ARE LINKED TO THE CRONOSTIMER
        private void loadTasks()
        {
            //ADDING TASK #1
            string schedule;
            CronosTimer timer;

            //COLLECT OPTIONS TO GET JOB SCHEDULES
            var options = _config.GetSection("Automation").Get<List<ProjectConfig>>();
            if (options == null)
            {
                return;
            }


            var cfg = options.Find(p => p.Name == "ADDirectReportAlertsLR");
            if (cfg != null && _tasksManager != null)
            {
                schedule = cfg.Schedule;

                //CREATE NEW TIMER TASK
                timer = new CronosTimer("30 9 * * *");
                //USING TOKEN FROM EVENT
                timer.Elapsed += HandleTimerElapsed(_tasksManager.ADDirectReportAlertsLRAsync);
                // USING STOPPINGTOKEN
                //timer.Elapsed += HandleTimerElapsed(_ => _tasksManager.CheckDataSourcesAsync(cancellationToken));
                //ADD TO LIST FOR EASY DISPOSE ON STOP
                _scheduledTasks.Add(timer);
            }

            return;


            //cfg = options.Find(p => p.Name == "ADDirectReportAlertsLR");
            //if (cfg != null && _tasksManager != null)
            //{
            //    schedule = cfg.Schedule;

            //    //CREATE NEW TIMER TASK
            //    timer = new CronosTimer("09 10 * * *");
            //    //USING TOKEN FROM EVENT
            //    timer.Elapsed += HandleTimerElapsed(_tasksManager.ADDirectReportAlertsLRAsync);
            //    // USING STOPPINGTOKEN
            //    //timer.Elapsed += HandleTimerElapsed(_ => _tasksManager.CheckDataSourcesAsync(cancellationToken));
            //    //ADD TO LIST FOR EASY DISPOSE ON STOP
            //    _scheduledTasks.Add(timer);
            //}


            //cfg = options.Find(p => p.Name == "DataSourceVerification");
            //if (cfg != null && _tasksManager != null)
            //{
            //    schedule = cfg.Schedule;
            //    //CREATE NEW TIMER TASK
            //    timer = new CronosTimer("4 10 * * *");
            //    //USING TOKEN FROM EVENT
            //    timer.Elapsed += HandleTimerElapsed(_tasksManager.CheckDataSourcesAsync);
            //    // USING STOPPINGTOKEN
            //    //timer.Elapsed += HandleTimerElapsed(_ => _tasksManager.CheckDataSourcesAsync(cancellationToken));
            //    //ADD TO LIST FOR EASY DISPOSE ON STOP
            //    _scheduledTasks.Add(timer);
            //}

            //return;
            //////TASK 11. SnowflakeDashboardData





            //TASK 1. DATASOURCE VERIFICATION
            //FIND THE SCHEDULE IN OPTIONS appsettings.json
            cfg = options.Find(p => p.Name == "DataSourceVerification");
            if (cfg != null && _tasksManager != null)
            {
                schedule = cfg.Schedule;
                //CREATE NEW TIMER TASK
                timer = new CronosTimer(schedule);
                //USING TOKEN FROM EVENT
                timer.Elapsed += HandleTimerElapsed(_tasksManager.CheckDataSourcesAsync);
                // USING STOPPINGTOKEN
                //timer.Elapsed += HandleTimerElapsed(_ => _tasksManager.CheckDataSourcesAsync(cancellationToken));
                //ADD TO LIST FOR EASY DISPOSE ON STOP
                _scheduledTasks.Add(timer);
            }
         
            ////TASK 2. PBI Membership
            cfg = options.Find(p => p.Name == "PBIMembership");
            if (cfg != null && _tasksManager != null)
            {
                schedule = cfg.Schedule;

                //CREATE NEW TIMER TASK
                timer = new CronosTimer(schedule);
                //USING TOKEN FROM EVENT
                timer.Elapsed += HandleTimerElapsed(_tasksManager.PBIMembershipRefreshAsync);
                // USING STOPPINGTOKEN
                //timer.Elapsed += HandleTimerElapsed(_ => _tasksManager.CheckDataSourcesAsync(cancellationToken));
                //ADD TO LIST FOR EASY DISPOSE ON STOP
                _scheduledTasks.Add(timer);
            }

            ////TASK 3. PPACA_TAT
            cfg = options.Find(p => p.Name == "PPACA_TAT");
            if (cfg != null && _tasksManager != null)
            {
                schedule = cfg.Schedule;

                //CREATE NEW TIMER TASK
                timer = new CronosTimer(schedule);
                //USING TOKEN FROM EVENT
                timer.Elapsed += HandleTimerElapsed(_tasksManager.PPACATATAppendAsync);
                // USING STOPPINGTOKEN
                //timer.Elapsed += HandleTimerElapsed(_ => _tasksManager.CheckDataSourcesAsync(cancellationToken));
                //ADD TO LIST FOR EASY DISPOSE ON STOP
                _scheduledTasks.Add(timer);
            }

            ////TASK 4. CS_Scorecard
            cfg = options.Find(p => p.Name == "CS_Scorecard");
            if (cfg != null && _tasksManager != null)
            {
                schedule = cfg.Schedule;

                //CREATE NEW TIMER TASK
                timer = new CronosTimer(schedule);
                //USING TOKEN FROM EVENT
                timer.Elapsed += HandleTimerElapsed(_tasksManager.CSScorecardAppendAsync);
                // USING STOPPINGTOKEN
                //timer.Elapsed += HandleTimerElapsed(_ => _tasksManager.CheckDataSourcesAsync(cancellationToken));
                //ADD TO LIST FOR EASY DISPOSE ON STOP
                _scheduledTasks.Add(timer);
            }



            ////TASK 5. NICEUHCWestEligibility
            cfg = options.Find(p => p.Name == "NICEUHCWestEligibility");
            if (cfg != null && _tasksManager != null)
            {
                schedule = cfg.Schedule;

                //CREATE NEW TIMER TASK
                timer = new CronosTimer(schedule);
                //USING TOKEN FROM EVENT
                timer.Elapsed += HandleTimerElapsed(_tasksManager.NICEUHCWestEligibilityAppendAsync);
                // USING STOPPINGTOKEN
                //timer.Elapsed += HandleTimerElapsed(_ => _tasksManager.CheckDataSourcesAsync(cancellationToken));
                //ADD TO LIST FOR EASY DISPOSE ON STOP
                _scheduledTasks.Add(timer);
            }



            ////TASK 6. ADDirectReportAlertsLR
            cfg = options.Find(p => p.Name == "ADDirectReportAlertsLR");
            if (cfg != null && _tasksManager != null)
            {
                schedule = cfg.Schedule;

                //CREATE NEW TIMER TASK
                timer = new CronosTimer(schedule);
                //USING TOKEN FROM EVENT
                timer.Elapsed += HandleTimerElapsed(_tasksManager.ADDirectReportAlertsLRAsync);
                // USING STOPPINGTOKEN
                //timer.Elapsed += HandleTimerElapsed(_ => _tasksManager.CheckDataSourcesAsync(cancellationToken));
                //ADD TO LIST FOR EASY DISPOSE ON STOP
                _scheduledTasks.Add(timer);
            }


            ////TASK 7. NICEUHCWestEligibility
            cfg = options.Find(p => p.Name == "EviCoreMRMembershipDetails");
            if (cfg != null && _tasksManager != null)
            {
                schedule = cfg.Schedule;

                //CREATE NEW TIMER TASK
                timer = new CronosTimer(schedule);
                //USING TOKEN FROM EVENT
                timer.Elapsed += HandleTimerElapsed(_tasksManager.EviCoreMRMembershipDetailsAppendAsync);
                // USING STOPPINGTOKEN
                //timer.Elapsed += HandleTimerElapsed(_ => _tasksManager.CheckDataSourcesAsync(cancellationToken));
                //ADD TO LIST FOR EASY DISPOSE ON STOP
                _scheduledTasks.Add(timer);
            }

            ////TASK 8. EviCoreAmerichoiceAllstatesAuth
            cfg = options.Find(p => p.Name == "EviCoreAmerichoiceAllstatesAuth");
            if (cfg != null && _tasksManager != null)
            {
                schedule = cfg.Schedule;

                //CREATE NEW TIMER TASK
                timer = new CronosTimer(schedule);
                //USING TOKEN FROM EVENT
                timer.Elapsed += HandleTimerElapsed(_tasksManager.EviCoreAmerichoiceAllstatesAuthAppendAsync);
                // USING STOPPINGTOKEN
                //timer.Elapsed += HandleTimerElapsed(_ => _tasksManager.CheckDataSourcesAsync(cancellationToken));
                //ADD TO LIST FOR EASY DISPOSE ON STOP
                _scheduledTasks.Add(timer);
            }


            ////TASK 9. EvicoreScorecard
            cfg = options.Find(p => p.Name == "EvicoreScorecard");
            if (cfg != null && _tasksManager != null)
            {
                schedule = cfg.Schedule;

                //CREATE NEW TIMER TASK
                timer = new CronosTimer(schedule);
                //USING TOKEN FROM EVENT
                timer.Elapsed += HandleTimerElapsed(_tasksManager.EvicoreScorecardAppendAsync);
                // USING STOPPINGTOKEN
                //timer.Elapsed += HandleTimerElapsed(_ => _tasksManager.CheckDataSourcesAsync(cancellationToken));
                //ADD TO LIST FOR EASY DISPOSE ON STOP
                _scheduledTasks.Add(timer);
            }


            ////TASK 10. SnowflakeDashboardData
            cfg = options.Find(p => p.Name == "SnowflakeDashboardData");
            if (cfg != null && _tasksManager != null)
            {
                schedule = cfg.Schedule;

                //CREATE NEW TIMER TASK
                timer = new CronosTimer(schedule);
                //USING TOKEN FROM EVENT
                timer.Elapsed += HandleTimerElapsed(_tasksManager.SnowflakeDashboardDataRefreshAsync);
                // USING STOPPINGTOKEN
                //timer.Elapsed += HandleTimerElapsed(_ => _tasksManager.CheckDataSourcesAsync(cancellationToken));
                //ADD TO LIST FOR EASY DISPOSE ON STOP
                _scheduledTasks.Add(timer);
            }

            ////TASK 11. MHPUniverseData
            cfg = options.Find(p => p.Name == "MHPUniverse");
            if (cfg != null && _tasksManager != null)
            {
                schedule = cfg.Schedule;

                //CREATE NEW TIMER TASK
                timer = new CronosTimer(schedule);
                //USING TOKEN FROM EVENT
                timer.Elapsed += HandleTimerElapsed(_tasksManager.MHPUniverseDataRefreshAsync);
                // USING STOPPINGTOKEN
                //timer.Elapsed += HandleTimerElapsed(_ => _tasksManager.CheckDataSourcesAsync(cancellationToken));
                //ADD TO LIST FOR EASY DISPOSE ON STOP
                _scheduledTasks.Add(timer);
            }


            ////TASK 12. EviCoreYTDMetricsData
            cfg = options.Find(p => p.Name == "EviCoreYTDMetrics");
            if (cfg != null && _tasksManager != null)
            {
                schedule = cfg.Schedule;

                //CREATE NEW TIMER TASK
                timer = new CronosTimer(schedule);
                //USING TOKEN FROM EVENT
                timer.Elapsed += HandleTimerElapsed(_tasksManager.EviCoreYTDMetricsDataRefreshAsync);
                // USING STOPPINGTOKEN
                //timer.Elapsed += HandleTimerElapsed(_ => _tasksManager.CheckDataSourcesAsync(cancellationToken));
                //ADD TO LIST FOR EASY DISPOSE ON STOP
                _scheduledTasks.Add(timer);
            }


            ////TASK 13. EviCoreYTDMetricsData
            cfg = options.Find(p => p.Name == "SiteOfCare");
            if (cfg != null && _tasksManager != null)
            {
                schedule = cfg.Schedule;

                //CREATE NEW TIMER TASK
                timer = new CronosTimer(schedule);
                //USING TOKEN FROM EVENT
                timer.Elapsed += HandleTimerElapsed(_tasksManager.SiteOfCareDataRefreshAsync);
                // USING STOPPINGTOKEN
                //timer.Elapsed += HandleTimerElapsed(_ => _tasksManager.CheckDataSourcesAsync(cancellationToken));
                //ADD TO LIST FOR EASY DISPOSE ON STOP
                _scheduledTasks.Add(timer);
            }

            ////TASK 2. CHEMO CSV PARSER
            //schedule = _configuration.Find(p => p.Name == "CGPInjChemoParser").Schedule;
            //timer = new CronosTimer(schedule);
            //timer.Elapsed += HandleTimerElapsed(_tasksManager.CheckDataSourcesAsync);
            //_scheduledTasks.Add(timer);


        }



        //HANDLING FOR NOW WITHIN program.cs
        //COLLECT CONFIGURATION FORM appsettings..json
        //        public IConfiguration InitConfiguration()
        //        {
        //            IConfigurationBuilder builder = new ConfigurationBuilder()
        //                //.SetBasePath(Directory.GetCurrentDirectory) NEEDED FOR MANUAL SUPPORT OF JSON
        //                .AddJsonFile("appsettings.json");

        //#if DEBUG
        //            builder.AddJsonFile("appsettings.Development.json", optional: true, reloadOnChange: true);
        //#else
        //            builder.AddJsonFile("appsettings.Production.json", optional: true, reloadOnChange: true);
        //#endif

        //            builder.AddEnvironmentVariables();
        //            return builder.Build();

        //        }





    }
}