using CAD_Worker_Service;
using DataAccessLibrary.DataAccess;
using Serilog;




//CONFIGURATION 
var configuration = new ConfigurationBuilder().AddJsonFile("appsettings.json", optional: true, reloadOnChange: true).AddEnvironmentVariables().Build();

//LOG SERILOG INTERNALS
Serilog.Debugging.SelfLog.Enable(msg => Console.WriteLine(msg));
//LOGGING SERILOG
Log.Logger = new LoggerConfiguration()
//    .WriteTo.Email(   //SET WITHIN appsettings.json
//    new EmailConnectionInfo
//    {
//        FromEmail = "chris_giordano@uhc.com",
//        ToEmail = "chris_giordano@uhc.com",
//        MailServer = "mailo2.uhc.com",
//        NetworkCredentials = new NetworkCredential
//        {
//            UserName = null,
//            Password = null
//        },
//        EnableSsl = true,
//        Port = 25,
//        EmailSubject = "Test"
//    },
//    outputTemplate: "{Timestamp:yyyy-MM-dd HH:mm:ss.fff zzz} [{Level}] {Message}{NewLine}{Exception}",
//    batchPostingLimit: 10
//    , restrictedToMinimumLevel: Serilog.Events.LogEventLevel.Error
//)
//.WriteTo.Console() //SET WITHIN appsettings.json
//.WriteTo.File("logs/log.txt", restrictedToMinimumLevel: Serilog.Events.LogEventLevel.Information, rollingInterval: RollingInterval.Month)//SET WITHIN appsettings.json
//.WriteTo.File("logs/errors.txt", restrictedToMinimumLevel: Serilog.Events.LogEventLevel.Warning, rollingInterval: RollingInterval.Year)//SET WITHIN appsettings.json
.ReadFrom.Configuration(configuration).CreateLogger(); //GET appsettings "Serilog":




try
{
    Log.Information("Starting Automation Worker Service...");

    IHost host = Host.CreateDefaultBuilder(args)
    .UseSerilog() //PLUGIN SERILOG TO CAPTURE .NET CORE ILogger calls!!
    .ConfigureServices(services =>
    {
        //GRAB CONFIGURATION OPIONS
        //List<AppSettings>? options = configuration.GetSection("Automation").Get<List<AppSettings>>(); //GET appsettings "Automation":
        //services.AddSingleton(options);
        services.AddSingleton<IConfiguration>(configuration);


        services.AddSingleton<IRelationalDataAccess, SqlDataAccess>();



        services.AddHostedService<Worker>();
    }).Build();


    await host.RunAsync().ConfigureAwait(false);

}
catch (Exception ex)
{
    Log.Fatal(ex, "Fatal Exception in Automation Worker Service");
}
finally
{
    Log.Information("Exiting Automation Worker Service");
    Log.CloseAndFlush();
}


