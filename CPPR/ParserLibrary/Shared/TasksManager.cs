


using ProjectManagerLibrary.Projects;
using System.Diagnostics.Contracts;

namespace ProjectManagerLibrary.Shared
{
    public class TasksManager : Disposable
    {
        private readonly IDelimitedParser _delimitedParser;
        private readonly IDataSourceVerification _dataSourceVerification;
        private readonly IPBIMembership _pbiMembership;
        private readonly IPPACATAT _ppaTAT;
        private readonly ICSScorecard _csScore;
        private readonly INICEUHCWestEligibility _nice;
        private readonly IADDirectReportAlertsLR _addrjlr;
        private readonly IEviCoreMRMembershipDetails _eviMemDetails;
        private readonly IEviCoreAmerichoiceAllstatesAuth _eviAmeriAA;
        private readonly IEvicoreScorecard _eviScoreCard;
        private readonly ISnowflakeDashboardData _snowflakeDash;
        private readonly IMHPUniverse _mhpUniverse;
        private readonly IEviCoreYTDMetrics _eviYTDMetrics;
        private readonly ISiteOfCare _siteOfCare;
        private readonly ISiteOfCare _siteOfCareGastro;
        private readonly List<IProjectConfig>? _projConfig;

        public TasksManager(IConfiguration config, IRelationalDataAccess db)
        {
            //INSTANTIATE ALL LIBRARIES FOR DIRECT FUNCTION CALLS
            _delimitedParser = new DelimitedParser();
            _dataSourceVerification = new DataSourceVerification(config);
            _pbiMembership = new PBIMembership(config, db);
            _ppaTAT = new PPACATAT(config, db);
            _csScore = new CSScorecard(config, db);
            _nice = new NICEUHCWestEligibility(config, db);
            _addrjlr = new ADDirectReportAlertsLR(config, db);
            _eviMemDetails  = new EviCoreMRMembershipDetails(config, db);
            _eviAmeriAA =  new EviCoreAmerichoiceAllstatesAuth(config, db);
            _eviScoreCard = new EvicoreScorecard(config, db);
            _snowflakeDash =  new SnowflakeDashboardData(config, db);
            _mhpUniverse = new MHPUniverse(config, db);
            _eviYTDMetrics = new EviCoreYTDMetrics(config, db);
            _siteOfCare = new SiteOfCare(config, db);
            _siteOfCareGastro = new SiteOfCareGastro(config, db);
            //CONFIGURATON FOR ALL FUNCTIONS
            var cfg = config.GetSection("Automation").Get<IEnumerable<ProjectConfig>>();
            if (cfg != null)
            {
                _projConfig = cfg.ToList<IProjectConfig>();

            }
  
        }


        public async Task  CheckDataSourcesAsync(CancellationToken stoppingToken)
        {
            //SETUP CUSTOM CONFIGURATION
            if (_projConfig == null)
            {
                return;
            }
            var cfg = _projConfig.Find(p => p.Name == "DataSourceVerification");
            if (cfg == null)
            {
                return;
            }
            var delay = cfg.Delay;

            //SEND CUSTOM CONFIGURATION
            var result = _dataSourceVerification.CheckDataSources();
            while (!stoppingToken.IsCancellationRequested)
            {
                if (result.Status == TaskStatus.Running)
                {
                    await Task.Delay(delay, stoppingToken).ConfigureAwait(false);
                }
                else if (result.Status == TaskStatus.RanToCompletion)
                {

                    //ONCE DONE LOG AND NULL TASK
                    Log.Information($"Total execution time for task: CheckDataSources = " + result.Result);
                    result = null;
                    break;
                }
                else if (result.Status == TaskStatus.Faulted) //ADDED ADD THIS WANT ENDLESS FAULT LOOPING-- HOPE THIS FIXES!!!
                {
                    var ex = result.Exception;
                    Log.Error($"Task faulted and stopped running. ErrorType={ex.GetType()} ErrorMessage={ex.Message}");
                    result = null;
                    break;
                }

            }
        }

        public async Task PBIMembershipRefreshAsync(CancellationToken stoppingToken)
        {

            if (_projConfig == null)
            {
                return;
            }
            var cfg = _projConfig.Find(p => p.Name == "PBIMembership");
            if (cfg == null)
            {
                return;
            }
            var delay = cfg.Delay;



            //SEND CUSTOM CONFIGURATION
            var result = _pbiMembership.RefreshTable();

            while (!stoppingToken.IsCancellationRequested)
            {

                if (result.Status == TaskStatus.Running)
                {
                    await Task.Delay(delay, stoppingToken).ConfigureAwait(false);
                }
                else if (result.Status == TaskStatus.RanToCompletion)
                {

                    //ONCE DONE LOG AND NULL TASK
                    Log.Information($"Total execution time for task: PBIMembership = " + result.Result);
                    result = null;
                    break;
                }
                else if (result.Status == TaskStatus.Faulted) //ADDED ADD THIS WANT ENDLESS FAULT LOOPING-- HOPE THIS FIXES!!!
                {
                    var ex = result.Exception;
                    Log.Error($"Task faulted and stopped running. ErrorType={ex.GetType()} ErrorMessage={ex.Message}");
                    result = null;
                    break;
                }

            }


        }


        public async Task ADDirectReportAlertsLRAsync(CancellationToken stoppingToken)
        {

            if (_projConfig == null)
            {
                return;
            }
            var cfg = _projConfig.Find(p => p.Name == "ADDirectReportAlertsLR");
            if (cfg == null)
            {
                return;
            }
            var delay = cfg.Delay;



            //SEND CUSTOM CONFIGURATION
            var result = _addrjlr.RefreshTable();

            while (!stoppingToken.IsCancellationRequested)
            {

                if (result.Status == TaskStatus.Running)
                {
                    await Task.Delay(delay, stoppingToken).ConfigureAwait(false);
                }
                else if (result.Status == TaskStatus.RanToCompletion)
                {

                    //ONCE DONE LOG AND NULL TASK
                    Log.Information($"Total execution time for task: ADDirectReportAlertsLR = " + result.Result);
                    result = null;
                    break;
                }
                else if (result.Status == TaskStatus.Faulted) //ADDED ADD THIS WANT ENDLESS FAULT LOOPING-- HOPE THIS FIXES!!!
                {
                    var ex = result.Exception;
                    Log.Error($"Task faulted and stopped running. ErrorType={ex.GetType()} ErrorMessage={ex.Message}");
                    result = null;
                    break;
                }

            }


        }




        public async Task PPACATATAppendAsync(CancellationToken stoppingToken)
        {

            if (_projConfig == null)
            {
                return;
            }
            var cfg = _projConfig.Find(p => p.Name == "PPACA_TAT");
            if (cfg == null)
            {
                return;
            }
            var delay = cfg.Delay;



            //SEND CUSTOM CONFIGURATION
            var result = _ppaTAT.LoadTATData();

            while (!stoppingToken.IsCancellationRequested)
            {

                if (result.Status == TaskStatus.Running)
                {
                    await Task.Delay(delay, stoppingToken).ConfigureAwait(false);
                }
                else if (result.Status == TaskStatus.RanToCompletion)
                {

                    //ONCE DONE LOG AND NULL TASK
                    Log.Information($"Total execution time for task: PPACA_TAT  = " + result.Result);
                    result = null;
                    break;
                }
                else if (result.Status == TaskStatus.Faulted) //ADDED ADD THIS WANT ENDLESS FAULT LOOPING-- HOPE THIS FIXES!!!
                {
                    var ex = result.Exception;
                    Log.Error($"Task faulted and stopped running. ErrorType={ex.GetType()} ErrorMessage={ex.Message}");
                    result = null;
                    break;
                }

            }

        }

        public async Task CSScorecardAppendAsync(CancellationToken stoppingToken)
        {

            if (_projConfig == null)
            {
                return;
            }
            var cfg = _projConfig.Find(p => p.Name == "CS_Scorecard");
            if (cfg == null)
            {
                return;
            }
            var delay = cfg.Delay;


            //SEND CUSTOM CONFIGURATION
            var result = _csScore.LoadCSScorecardData();

            while (!stoppingToken.IsCancellationRequested)
            {

                if (result.Status == TaskStatus.Running)
                {
                    await Task.Delay(delay, stoppingToken).ConfigureAwait(false);
                }
                else if (result.Status == TaskStatus.RanToCompletion)
                {

                    //ONCE DONE LOG AND NULL TASK
                    Log.Information($"Total execution time for task: CS_Scorecard  = " + result.Result);
                    result = null;
                    break;
                }
                else if (result.Status == TaskStatus.Faulted) //ADDED ADD THIS WANT ENDLESS FAULT LOOPING-- HOPE THIS FIXES!!!
                {
                    var ex = result.Exception;
                    Log.Error($"Task faulted and stopped running. ErrorType={ex.GetType()} ErrorMessage={ex.Message}");
                    result = null;
                    break;
                }

            }


        }


        public async Task EviCoreAmerichoiceAllstatesAuthAppendAsync(CancellationToken stoppingToken)
        {

            if (_projConfig == null)
            {
                return;
            }
            var cfg = _projConfig.Find(p => p.Name == "EviCoreAmerichoiceAllstatesAuth");
            if (cfg == null)
            {
                return;
            }
            var delay = cfg.Delay;


            //SEND CUSTOM CONFIGURATION
            var result = _eviAmeriAA.LoadEviCoreAmerichoiceAllstatesAuthData();

            while (!stoppingToken.IsCancellationRequested)
            {

                if (result.Status == TaskStatus.Running)
                {
                    await Task.Delay(delay, stoppingToken).ConfigureAwait(false);
                }
                else if (result.Status == TaskStatus.RanToCompletion)
                {

                    //ONCE DONE LOG AND NULL TASK
                    Log.Information($"Total execution time for task: EviCoreAmerichoiceAllstatesAuth  = " + result.Result);
                    result = null;
                    break;
                }
                else if (result.Status == TaskStatus.Faulted) //ADDED ADD THIS WANT ENDLESS FAULT LOOPING-- HOPE THIS FIXES!!!
                {
                    var ex = result.Exception;
                    Log.Error($"Task faulted and stopped running. ErrorType={ex.GetType()} ErrorMessage={ex.Message}");
                    result = null;
                    break;
                }

            }


        }


        public async Task NICEUHCWestEligibilityAppendAsync(CancellationToken stoppingToken)
        {

            if (_projConfig == null)
            {
                return;
            }
            var cfg = _projConfig.Find(p => p.Name == "NICEUHCWestEligibility");
            if (cfg == null)
            {
                return;
            }
            var delay = cfg.Delay;


            //SEND CUSTOM CONFIGURATION
            var result = _nice.LoadNICEUHCWestEligibilityData();

            while (!stoppingToken.IsCancellationRequested)
            {

                if (result.Status == TaskStatus.Running)
                {
                    await Task.Delay(delay, stoppingToken).ConfigureAwait(false);
                }
                else if (result.Status == TaskStatus.RanToCompletion)
                {

                    //ONCE DONE LOG AND NULL TASK
                    Log.Information($"Total execution time for task: NICEUHCWestEligibility  = " + result.Result);
                    result = null;
                    break;
                }
                else if (result.Status == TaskStatus.Faulted) //ADDED ADD THIS WANT ENDLESS FAULT LOOPING-- HOPE THIS FIXES!!!
                {
                    var ex = result.Exception;
                    Log.Error($"Task faulted and stopped running. ErrorType={ex.GetType()} ErrorMessage={ex.Message}");
                    result = null;
                    break;
                }

            }


        }


        public async Task EviCoreMRMembershipDetailsAppendAsync(CancellationToken stoppingToken)
        {

            if (_projConfig == null)
            {
                return;
            }
            var cfg = _projConfig.Find(p => p.Name == "EviCoreMRMembershipDetails");
            if (cfg == null)
            {
                return;
            }
            var delay = cfg.Delay;


            //SEND CUSTOM CONFIGURATION
            var result = _eviMemDetails.LoadEviCoreMRMembershipDetails();

            while (!stoppingToken.IsCancellationRequested)
            {

                if (result.Status == TaskStatus.Running)
                {
                    await Task.Delay(delay, stoppingToken).ConfigureAwait(false);
                }
                else if (result.Status == TaskStatus.RanToCompletion)
                {

                    //ONCE DONE LOG AND NULL TASK
                    Log.Information($"Total execution time for task: EviCoreMRMembershipDetails  = " + result.Result);
                    result = null;
                    break;
                }
                else if (result.Status == TaskStatus.Faulted) //ADDED ADD THIS WANT ENDLESS FAULT LOOPING-- HOPE THIS FIXES!!!
                {
                    var ex = result.Exception;
                    Log.Error($"Task faulted and stopped running. ErrorType={ex.GetType()} ErrorMessage={ex.Message}");
                    result = null;
                    break;
                }

            }


        }


        public async Task EvicoreScorecardAppendAsync(CancellationToken stoppingToken)
        {

            if (_projConfig == null)
            {
                return;
            }
            var cfg = _projConfig.Find(p => p.Name == "EvicoreScorecard");
            if (cfg == null)
            {
                return;
            }
            var delay = cfg.Delay;


            //SEND CUSTOM CONFIGURATION
            var result = _eviScoreCard.LoadEvicoreScorecardData();

            while (!stoppingToken.IsCancellationRequested)
            {

                if (result.Status == TaskStatus.Running)
                {
                    await Task.Delay(delay, stoppingToken).ConfigureAwait(false);
                }
                else if (result.Status == TaskStatus.RanToCompletion)
                {

                    //ONCE DONE LOG AND NULL TASK
                    Log.Information($"Total execution time for task: EvicoreScorecard  = " + result.Result);
                    result = null;
                    break;
                }
                else if (result.Status == TaskStatus.Faulted) //ADDED ADD THIS WANT ENDLESS FAULT LOOPING-- HOPE THIS FIXES!!!
                {
                    var ex = result.Exception;
                    Log.Error($"Task faulted and stopped running. ErrorType={ex.GetType()} ErrorMessage={ex.Message}");
                    result = null;
                    break;
                }

            }


        }


        public async Task SnowflakeDashboardDataRefreshAsync(CancellationToken stoppingToken)
        {

            if (_projConfig == null)
            {
                return;
            }
            var cfg = _projConfig.Find(p => p.Name == "SnowflakeDashboardData");
            if (cfg == null)
            {
                return;
            }
            var delay = cfg.Delay;


            //SEND CUSTOM CONFIGURATION
            var result = _snowflakeDash.SnowflakeDashboardDataRefresh();

            while (!stoppingToken.IsCancellationRequested)
            {

                if (result.Status == TaskStatus.Running)
                {
                    await Task.Delay(delay, stoppingToken).ConfigureAwait(false);
                }
                else if (result.Status == TaskStatus.RanToCompletion)
                {

                    //ONCE DONE LOG AND NULL TASK
                    Log.Information($"Total execution time for task: SnowflakeDashboardData  = " + result.Result);
                    result = null;
                    break;
                }
                else if (result.Status == TaskStatus.Faulted) //ADDED ADD THIS WANT ENDLESS FAULT LOOPING-- HOPE THIS FIXES!!!
                {
                    var ex = result.Exception;
                    Log.Error($"Task faulted and stopped running. ErrorType={ex.GetType()} ErrorMessage={ex.Message}");
                    result = null;
                    break;
                }

            }


        }

        public async Task MHPUniverseDataRefreshAsync(CancellationToken stoppingToken)
        {

            if (_projConfig == null)
            {
                return;
            }
            var cfg = _projConfig.Find(p => p.Name == "MHPUniverse");
            if (cfg == null)
            {
                return;
            }
            var delay = cfg.Delay;


            //SEND CUSTOM CONFIGURATION
            var result = _mhpUniverse.LoadMHPUniverseData();

            while (!stoppingToken.IsCancellationRequested)
            {

                if (result.Status == TaskStatus.Running)
                {
                    await Task.Delay(delay, stoppingToken).ConfigureAwait(false);
                }
                else if (result.Status == TaskStatus.RanToCompletion)
                {

                    //ONCE DONE LOG AND NULL TASK
                    Log.Information($"Total execution time for task: MHPUniverse  = " + result.Result);
                    result = null;
                    break;
                }
                else if (result.Status == TaskStatus.Faulted) //ADDED ADD THIS WANT ENDLESS FAULT LOOPING-- HOPE THIS FIXES!!!
                {
                    var ex = result.Exception;
                    Log.Error($"Task faulted and stopped running. ErrorType={ex.GetType()} ErrorMessage={ex.Message}");
                    result = null;
                    break;
                }

            }


        }


        public async Task EviCoreYTDMetricsDataRefreshAsync(CancellationToken stoppingToken)
        {

            if (_projConfig == null)
            {
                return;
            }
            var cfg = _projConfig.Find(p => p.Name == "EviCoreYTDMetrics");
            if (cfg == null)
            {
                return;
            }
            var delay = cfg.Delay;


            //SEND CUSTOM CONFIGURATION
            var result = _eviYTDMetrics.LoadEviCoreYTDMetricsData();

            while (!stoppingToken.IsCancellationRequested)
            {

                if (result.Status == TaskStatus.Running)
                {
                    await Task.Delay(delay, stoppingToken).ConfigureAwait(false);
                }
                else if (result.Status == TaskStatus.RanToCompletion)
                {

                    //ONCE DONE LOG AND NULL TASK
                    Log.Information($"Total execution time for task: EviCoreYTDMetrics  = " + result.Result);
                    result = null;
                    break;
                }
                else if (result.Status == TaskStatus.Faulted) //ADDED ADD THIS WANT ENDLESS FAULT LOOPING-- HOPE THIS FIXES!!!
                {
                    var ex = result.Exception;
                    Log.Error($"Task faulted and stopped running. ErrorType={ex.GetType()} ErrorMessage={ex.Message}");
                    result = null;
                    break;
                }

            }


        }


        public async Task SiteOfCareDataRefreshAsync(CancellationToken stoppingToken)
        {

            if (_projConfig == null)
            {
                return;
            }
            var cfg = _projConfig.Find(p => p.Name == "SiteOfCare");
            if (cfg == null)
            {
                return;
            }
            var delay = cfg.Delay;


            //SEND CUSTOM CONFIGURATION
            var result = _siteOfCare.LoadSiteOfCareData();

            while (!stoppingToken.IsCancellationRequested)
            {

                if (result.Status == TaskStatus.Running)
                {
                    await Task.Delay(delay, stoppingToken).ConfigureAwait(false);
                }
                else if (result.Status == TaskStatus.RanToCompletion)
                {

                    //ONCE DONE LOG AND NULL TASK
                    Log.Information($"Total execution time for task: SiteOfCare  = " + result.Result);
                    result = null;
                    break;
                }
                else if (result.Status == TaskStatus.Faulted) //ADDED ADD THIS WANT ENDLESS FAULT LOOPING-- HOPE THIS FIXES!!!
                {
                    var ex = result.Exception;
                    Log.Error($"Task faulted and stopped running. ErrorType={ex.GetType()} ErrorMessage={ex.Message}");
                    result = null;
                    break;
                }

            }


        }



        public async Task SiteOfCareGastroDataRefreshAsync(CancellationToken stoppingToken)
        {

            if (_projConfig == null)
            {
                return;
            }
            var cfg = _projConfig.Find(p => p.Name == "SiteOfCareGastro");
            if (cfg == null)
            {
                return;
            }
            var delay = cfg.Delay;


            //SEND CUSTOM CONFIGURATION
            var result = _siteOfCareGastro.LoadSiteOfCareData();

            while (!stoppingToken.IsCancellationRequested)
            {

                if (result.Status == TaskStatus.Running)
                {
                    await Task.Delay(delay, stoppingToken).ConfigureAwait(false);
                }
                else if (result.Status == TaskStatus.RanToCompletion)
                {

                    //ONCE DONE LOG AND NULL TASK
                    Log.Information($"Total execution time for task: SiteOfCareGastro  = " + result.Result);
                    result = null;
                    break;
                }
                else if (result.Status == TaskStatus.Faulted) //ADDED ADD THIS WANT ENDLESS FAULT LOOPING-- HOPE THIS FIXES!!!
                {
                    var ex = result.Exception;
                    Log.Error($"Task faulted and stopped running. ErrorType={ex.GetType()} ErrorMessage={ex.Message}");
                    result = null;
                    break;
                }

            }


        }



        private Task<long>? _taskParseDelimitedFiles;
        //private long _taskParseDelimitedFiles;
        public async Task parseDelimitedFilesAsync(CancellationToken stoppingToken)
        {

            if (_projConfig == null)
            {
                return;
            }
            var cfg = _projConfig.Find(p => p.Name == "DelimitedParser");
            if (cfg == null)
            {
                return;
            }
            var delay = cfg.Delay;



            while (!stoppingToken.IsCancellationRequested)
            {
                if (_taskParseDelimitedFiles == null)
                    //_taskParseDelimitedFiles = Task.Run(() => _delimitedParser.parseDelimitedFiles(pc.FilePath, pc.Delimiter, pc.ConnectionString, pc.Schema, pc.BulkSize, pc.Delay));

                if (_taskParseDelimitedFiles.Status == TaskStatus.Running)
                {
                    await Task.Delay(delay, stoppingToken).ConfigureAwait(false);
                }
                else if(_taskParseDelimitedFiles.Status == TaskStatus.RanToCompletion)
                {

                    //ONCE DONE LOG AND NULL TASK
                    Log.Information($"Total execution time for task:parseDelimitedFiles = {_taskParseDelimitedFiles}");
                    _taskParseDelimitedFiles = null;
                }

            }
        }



        //UNUSED NOTES BELOW!!!!
        //SAMPLES OF await Task.Run(() => Parallel.ForEach POWER!!!!
        /*
        public static List<string> RunStringTestParallelSync()
        {

            List<int> input = new List<int>();
            for (int i = 0; i < 10; i++)
                input.Add(i);

            List<string> output = new List<string>();

            //PASS IN List<int>input = foreach i in input
            Parallel.ForEach<int>(input, (i) =>
            {
                //2 SECOND PAUSE
                //NOT IN SYNCHRONOUS!!!
                //Task.Run(async () =>
                //{
                //    await Task.Delay(2000).ConfigureAwait(false);

                //});
                Thread.Sleep(2000);
                output.Add((i * 5).ToString() + " testing!");
            });
            
           
            return output;
        }


        public static async Task<List<string>> RunStringTestParallelASync()
        {

            List<int> input = new List<int>();
            for (int i = 0; i < 10; i++)
                input.Add(i);

            List<string> output = new List<string>();


            await Task.Run(() =>
            {
                //PASS IN List<int>input = foreach i in input
                Parallel.ForEach<int>(input, (i) =>
                {
                    //2 SECOND PAUSE
                    //Task.Run(async () =>
                    //{
                    //    await Task.Delay(2000).ConfigureAwait(false);

                    //});
                    Task.Delay(2000).ConfigureAwait(false);
                    output.Add((i * 5).ToString() + " testing!");
                });

            });
           
            return output;
        }

        //Task t = Task.Run(async () =>
        //ASYNC MAGIC!!!
        //(async () => {
        //await...

        //    // all of the script.... 

        //})();
        //    // nothing else

        */

    }
}
