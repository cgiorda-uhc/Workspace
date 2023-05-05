using FileParsingLibrary.MSExcel;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Serilog;
using Serilog.Core;
using System;
using System.Collections.Generic;
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

        var configuration = new ConfigurationBuilder().AddJsonFile(appsettings).AddEnvironmentVariables().Build();
        logger = new LoggerConfiguration().ReadFrom.Configuration(configuration).CreateLogger();
        logger.Information("Starting up VC Portal...");
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
                    services.AddSingleton(logger);


                }).Build();

        logger.Information("Authicating user...");
        getAuthentication(configuration);


    }

    protected override async void OnStartup(StartupEventArgs e)
    {

            await AppHost!.StartAsync();
            var config = AppHost.Services.GetService<IConfiguration>();
            var excel = AppHost.Services.GetService<IExcelFunctions>();

            //AppDomain.CurrentDomain.FirstChanceException += new EventHandler<System.Runtime.ExceptionServices.FirstChanceExceptionEventArgs>(CurrentDomain_FirstChanceException);

            var startupForm = AppHost.Services.GetRequiredService<MainWindow>();
            startupForm.DataContext = new MainViewModel("", config, excel, logger);
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
        Authentication.UserName = WindowsIdentity.GetCurrent().Name.Split('\\')[1];
        await Authentication.SetCurrentUserAsync(ecs.API.BaseUrl, ecs.API.Url + "/" + Authentication.UserName);


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
