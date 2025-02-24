﻿using DataAccessLibrary.Data.Abstract;
using DataAccessLibrary.Data.Concrete.ChemoPx;
using DataAccessLibrary.Data.Concrete.EDCAdhoc;
using DataAccessLibrary.Data.Concrete.ETGFactSymmetry;
using DataAccessLibrary.Data.Concrete.MHP;
using DataAccessLibrary.Data.Concrete.ProcCodeTrends;
using DataAccessLibrary.DataAccess;
using FileParsingLibrary.MSExcel;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using NPOI.OpenXmlFormats.Shared;
using Serilog;
using Serilog.Core;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.ExceptionServices;
using System.Security.Principal;
using System.Threading.Tasks;
using System.Windows;
using VCPortal_Models.Configuration.HeaderInterfaces.Abstract;
using VCPortal_Models.Configuration.HeaderInterfaces.Concrete;
using VCPortal_WPF.Shared;
using VCPortal_WPF_ViewModel.Shared;
using static Org.BouncyCastle.Math.EC.ECCurve;


namespace VCPortal_WPF;
/// <summary>
/// Interaction logic for App.xaml
/// </summary>
public partial class App : Application
{
    public static IHost? AppHost { get; private set; }

    Serilog.ILogger logger;
    public App()
    {
        //var appsettings = "appsettings.Development.json";
        var appsettings = "appsettings.json";

        var configuration = new ConfigurationBuilder().SetBasePath(Directory.GetCurrentDirectory()).AddJsonFile(appsettings).AddEnvironmentVariables().Build();
        //var configuration = new ConfigurationBuilder().AddJsonFile("appsettings.json", optional: true, reloadOnChange: true).AddEnvironmentVariables().Build();


        //Serilog.Debugging.SelfLog.Enable(msg => Console.WriteLine(msg));


         //Log.Logger = new LoggerConfiguration().ReadFrom.Configuration(configuration).CreateLogger();
        logger = new LoggerConfiguration().ReadFrom.Configuration(configuration).CreateLogger();
        Log.Logger = logger;


        //Log.Information("Starting up VC Portal...");
        logger.Information("Starting up VC Portal...");

        //Log.Information("Authicating user...");
        logger.Information("Authicating user...");
        getAuthentication(configuration);


        AppHost = Host.CreateDefaultBuilder()
            .UseSerilog()
                .ConfigureAppConfiguration((context, builder) =>
                {
                    // Add other configuration files...
                    builder.AddJsonFile(appsettings, optional: true);
                }).ConfigureServices((hostContext, services) =>
                {
                    services.AddSingleton<MainWindow>();
                    services.AddTransient<IExcelFunctions, ClosedXMLFunctions>();
                    services.AddTransient<IRelationalDataAccess, SqlDataAccess>();
                    services.AddTransient<IChemotherapyPX_Repo, ChemotherapyPX_Repo>();
                    services.AddTransient<IMHPUniverse_Repo, MHPUniverse_Repo>();
                    services.AddTransient<IProcCodeTrends_Repo, ProcCodeTrends_Repo>();
                    services.AddTransient<IEDCAdhoc_Repo, EDCAdhoc_Repo>();
                    services.AddTransient<IETGFactSymmetry_Repo, ETGFactSymmetry_Repo>();

                    services.AddSingleton(logger);


                }).Build();



    }

    protected override async void OnStartup(StartupEventArgs e)
    {

            await AppHost!.StartAsync();
            var config = AppHost.Services.GetService<IConfiguration>();
            var excel = AppHost.Services.GetService<IExcelFunctions>();


           DBRepoModel dBRepo = new DBRepoModel();

            dBRepo.db_sql = AppHost.Services.GetService<IRelationalDataAccess>();
            dBRepo.chemo_sql = AppHost.Services.GetService<IChemotherapyPX_Repo>();
            dBRepo.mhp_sql = AppHost.Services.GetService<IMHPUniverse_Repo>();
            dBRepo.pct_db = AppHost.Services.GetService<IProcCodeTrends_Repo>();
            dBRepo.edc_db = AppHost.Services.GetService<IEDCAdhoc_Repo>();
            dBRepo.etg_db = AppHost.Services.GetService<IETGFactSymmetry_Repo>();

        //var db_sql = AppHost.Services.GetService<IRelationalDataAccess>();
        //    var chemo_sql = AppHost.Services.GetService<IChemotherapyPX_Repo >();
        //    var mhp_sql = AppHost.Services.GetService<IMHPUniverse_Repo>();
        //    var pct_db = AppHost.Services.GetService<IProcCodeTrends_Repo>();
        //    var edc_db = AppHost.Services.GetService<IEDCAdhoc_Repo>();
        //    var etg_db = AppHost.Services.GetService<IETGFactSymmetry_Repo>();

        //AppDomain.CurrentDomain.FirstChanceException += new EventHandler<System.Runtime.ExceptionServices.FirstChanceExceptionEventArgs>(CurrentDomain_FirstChanceException);

        var startupForm = AppHost.Services.GetRequiredService<MainWindow>();
            //startupForm.DataContext = new MainWindowViewModel("", config, excel, logger, db_sql, chemo_sql, mhp_sql, pct_db, edc_db, etg_db);
        startupForm.DataContext = new MainWindowViewModel("", config, excel, logger, dBRepo);
        //startupForm.DataContext = new MainWindowViewModel("", config, excel, Log.Logger);
        startupForm.Show();

            base.OnStartup(e);
 
    }


    private async Task getAuthentication(IConfiguration config)
    {
        var section = "Authentication";
        var project = "Authenticate";

        var cfg = config.GetSection(section).Get<List<AuthenticationConfig>>();
        AuthenticationConfig ecs = new AuthenticationConfig();
        if (cfg == null)
        {
            //Log.Error($"No Config found for ETGFactSymmetry");
            throw new OperationCanceledException();
        }
        ecs = cfg.Find(p => p.Name == project);
        if (ecs != null)
        {
            //Microsoft.Extensions.Configuration.Binder
            var e = config.GetSection(section + ":API").Get<APIConfig>();
            if (e != null)
            {
                ecs.API = e;
            }
        }

        Authentication.Log = logger;
        //Authentication.Log = Log.Logger;
        Authentication.UserName = WindowsIdentity.GetCurrent().Name.Split('\\')[1];
        await Authentication.SetCurrentUserAsync(ecs.API.BaseUrl, ecs.API.Url + "/" + Authentication.UserName, config);


    }

    //private bool _blError = false;
    //private void CurrentDomain_FirstChanceException(object sender, FirstChanceExceptionEventArgs e)
    //{

    //    Log.Fatal(e.Exception, "VC Portal WPF failed");
    //    if(!_blError)
    //    {
    //        Dispatcher.BeginInvoke(new Action(() => MessageBox.Show("Error Occurred \n\r" + e.Exception.Message + "\n\r" + e.Exception.StackTrace, "ERROR", MessageBoxButton.OK, MessageBoxImage.Error)));
    //        _blError = true;
    //    }

    //}


    protected override async void OnExit(ExitEventArgs e)
    {
        await AppHost!.StopAsync();

        base.OnExit(e);
    }


}
