
using ClosedXML.Graphics;
using FileParsingLibrary.MSExcel;
using System.Reflection;
using VCPortal_Models.Models.ActiveDirectory;
using VCPortal_WebUI.Client.Logging;
using VCPortal_WebUI.Client.Services.ETGFactSymmetry;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
	builder.RootComponents.Add<App>("#app");
	builder.RootComponents.Add<HeadOutlet>("head::after");





//BLAZORE WASM SERILOG 1
//https://stackoverflow.com/questions/71220619/use-serilog-as-logging-provider-in-blazor-webassembly-client-app?rq=1
//var levelSwitch = new LoggingLevelSwitch();
//Log.Logger = new LoggerConfiguration()
//    .MinimumLevel.ControlledBy(levelSwitch)
//    .Enrich.WithProperty("InstanceId", Guid.NewGuid().ToString("n"))
//    .WriteTo.BrowserHttp(endpointUrl: $"{builder.HostEnvironment.BaseAddress}ingest", controlLevelSwitch: levelSwitch)
//    .WriteTo.BrowserConsole(restrictedToMinimumLevel: Serilog.Events.LogEventLevel.Debug)
//    .CreateLogger();

//BLAZORE WASM SERILOG 2
//https://nblumhardt.com/2019/11/serilog-blazor/
//var levelSwitch = new LoggingLevelSwitch();
//   Serilog.Log.Logger = new LoggerConfiguration()
//	.MinimumLevel.ControlledBy(levelSwitch)
//	.Enrich.WithProperty("InstanceId", Guid.NewGuid().ToString("n"))
//       .WriteTo.BrowserHttp(controlLevelSwitch: levelSwitch) //URL optional - will default to /ingest on the origin
//       //.WriteTo.BrowserHttp(endpointUrl: builder.HostEnvironment.BaseAddress + "ingest", controlLevelSwitch: levelSwitch)
//       .WriteTo.BrowserConsole()
//        //.WriteTo.File(@"C:\Users\cgiorda\Documents\VCLogs\log.txt", LogEventLevel.Debug)
//       .CreateLogger();



//builder.Logging.AddSerilog();

/* this is used instead of .UseSerilog to add Serilog to providers */
//builder.Services.AddLogging(loggingBuilder => loggingBuilder.AddSerilog(dispose: true));


//ILOGGER
//var logFactory = new LoggerFactory()
//		.AddConsole(LogLevel.Debug)
//		.AddDebug();
//var logger = logFactory.CreateLogger<Type>();
//logger.LogInformation("Starting VCPortal WebAssemblyHostBuilder...");

//builder.Logging.AddConfiguration(builder.Configuration.GetSection("Logging"));

//var sericeProvider = builder.Services.BuildServiceProvider.GetRequiredService<IVCPortal_Services>();
//var IVCPortal_Services = sericeProvider
//using var loggerFactory = LoggerFactory.Create(builder => 
//{
//    builder.SetMinimumLevel(LogLevel.Debug);
//    //builder.AddConsole();
//    builder.AddDebug();
//});

//builder.Services.AddLogging();

//var IVCPortal_Services = builder.Services.BuildServiceProvider().GetRequiredService<IVCPortal_Services>();
//loggerFactory.AddProvider(new ApplicationLoggerProvider(IVCPortal_Services));

////VCPortal_WebUI.Server.Logging.ApplicationLogging.LoggerFactory = loggerFactory;
//ApplicationLogging.LoggerFactory = loggerFactory;

////ILogger<Program> logger = loggerFactory.CreateLogger<Program>();
//ILogger logger = ApplicationLogging.CreateLogger<Program>();



//builder.Services.AddAuthorizationCore();
//builder.Services.AddScoped<AuthenticationStateProvider, ClientAuthorizationService>();
//builder.Services.AddScoped<AuthenticationStateProvider>();

//COMMENTED IN FAVOR OF POLLY
//builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri("https://localhost:7129") });
//builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });
//HTTPClient Factory POLLY
//https://www.c-sharpcorner.com/article/http-best-practices-using-asp-net-core-and-polly/
builder.Services
		.AddHttpClient<ChemotherapyPX_Services>("ChemotherapyPX_Services", client => client.BaseAddress = new Uri(builder.Configuration["MyApi:APIS"]));

    builder.Services
        .AddHttpClient<VCPortal_Services>("VCPortal_Services", client => client.BaseAddress = new Uri(builder.Configuration["MyApi:APIS"]));

//builder.Services
    //.AddHttpClient<ClientAuthorizationService>("AuthenticationServices", client => client.BaseAddress = new Uri(builder.Configuration["MyApi:APIS"]));

builder.Services
    .AddHttpClient<ClientAuthorizationService>("AuthenticationServices", client => client.BaseAddress = new Uri(builder.Configuration["MyApi:APIS"]));


builder.Services
        .AddHttpClient<ETGFactSymmetryServices>("ETGFactSymmetry_Services", client => client.BaseAddress = new Uri(builder.Configuration["MyApi:APIS"]));

//builder.Services
//    .AddHttpClient<ClientAuthorizationService>("ClientAuthorizationService", client => client.BaseAddress = new Uri(builder.Configuration["MyApi:APIS"]));

//.AddTransientHttpErrorPolicy(builder => builder.WaitAndRetryAsync(new[]
//{
//    TimeSpan.FromSeconds(1),
//    TimeSpan.FromSeconds(5)
//}));

//TESTING LIMITED USERS in appsettings.json FOR NOW

var uac = builder.Configuration.GetSection($"Authorization").Get<UserAccessConfig>();
builder.Services.AddSingleton<UserAccessConfig>(uac);



builder.Services.AddSingleton<IExcelFunctions, ClosedXMLFunctions>();



builder.Services.AddTransient<IChemotherapyPX_Services, ChemotherapyPX_Services>();
builder.Services.AddTransient<IETGFactSymmetryServices, ETGFactSymmetryServices>();
//builder.Services.AddTransient<MHPUniverse_Services>();
builder.Services.AddTransient<IVCPortal_Services, VCPortal_Services>();
	builder.Services.AddSingleton<IVCPortal_Globals, VCPortal_Globals>();

builder.Services.AddScoped<IClientAuthorizationService, ClientAuthorizationService>();

//builder.Services.AddSingleton<NavigationManager>();


//https://www.syncfusion.com/faq/blazor/general/how-do-i-implement-windows-authentication-and-authorization-in-blazor-webassembly
//builder.Services.AddAuthorizationCore();



//builder.Services.AddAuthorizationCore();
//builder.Services.AddScoped<IClientAuthorizationService, ClientAuthorizationService>(new ClientAuthorizationService(httpClient)
//{
//    ApiUriGetAuthorizedUser = "api/settings/user",

//    ApiUriSignIn = "AzureADB2C/Account/SignIn",
//    ApiUriSignOut = "AzureADB2C/Account/SignOut",
//});
//builder.Services.AddScoped<AuthenticationStateProvider>(sp => sp.GetRequiredService<ClientAuthorizationService>());
//builder.Services.AddOptions();





// Or you can also register as follows

//builder.Services.AddHttpContextAccessor();


using var loggerFactory = LoggerFactory.Create(builder =>
{
    builder.SetMinimumLevel(LogLevel.Debug);
    //builder.AddConsole();
    builder.AddDebug();
});

builder.Services.AddLogging();

var IVCPortal_Services = builder.Services.BuildServiceProvider().GetRequiredService<IVCPortal_Services>();
loggerFactory.AddProvider(new ApplicationLoggerProvider(IVCPortal_Services));

////VCPortal_WebUI.Server.Logging.ApplicationLogging.LoggerFactory = loggerFactory;
//ApplicationLogging.LoggerFactory = loggerFactory;

//VCPortal_WebUI.Server.Logging.ApplicationLogging.LoggerFactory = loggerFactory;
ApplicationLogging.LoggerFactory = loggerFactory;

//ILogger<Program> logger = loggerFactory.CreateLogger<Program>();
ILogger logger = ApplicationLogging.CreateLogger<Program>();


//var logger = builder.Services.AddLogging(logging =>
//    {

//        var IVCPortal_Services = builder.Services.BuildServiceProvider().GetRequiredService<IVCPortal_Services>();
//        logging.AddProvider(new ApplicationLoggerProvider(IVCPortal_Services));
//    });





builder.Services.AddBlazoredToast();
    builder.Services.AddTelerikBlazor();


    //GET INSTANCE OF APP
    //var app = builder.Build();
    //ILogger logger = ApplicationLogging.CreateLogger<Program>();
    try
    {

        logger.LogInformation("Client: VCPortal WebAssemblyHostBuilder Started...");
        await builder.Build().RunAsync();



    }
    catch (Exception ex)
    {
        logger.LogCritical(ex, "Client: Fatal Exception in VCPortal WebAssemblyHostBuilder");
        //throw;
    }
    finally
    {
        logger.LogInformation("Client: Exiting VCPortal WebAssemblyHostBuilder");
        //Log.CloseAndFlush();
    }





