using DataAccessLibrary.Data.Concrete.ETGFactSymmetry;
using Microsoft.EntityFrameworkCore;
using Serilog;

var builder = WebApplication.CreateBuilder(args);



//CONFIGURATION 
var configuration = new ConfigurationBuilder().AddJsonFile("appsettings.json").AddEnvironmentVariables().Build();


Serilog.Debugging.SelfLog.Enable(msg => Console.WriteLine(msg));
//LOGGING SERILOG

var logger = new LoggerConfiguration().ReadFrom.Configuration(configuration).CreateLogger();
builder.Logging.ClearProviders();
//builder.Logging.AddSerilog(logger);
Log.Logger = logger;

//COMMENTING FIXED rollingInterval
//builder.Host.UseSerilog(logger);
builder.Host.UseSerilog(Log.Logger);
//DEPENDENCY INJECTION AUTOMAPPER FOR DTO SUPPORT
//builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());

//1 CONFIGURE SERVICES
//1 CONFIGURE SERVICES
//1 CONFIGURE SERVICES
//SWAGGER
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

//AUTHENTICATION
builder.Services.AddAuthentication();
builder.Services.AddAuthorization();


//INJECTABLES
builder.Services.AddTransient<IRelationalDataAccess, SqlDataAccess>();
builder.Services.AddTransient<ILog_Repo, Logs_Repo>();
builder.Services.AddTransient<IChemotherapyPX_Repo, ChemotherapyPX_Repo>();
builder.Services.AddTransient<IMHPUniverse_Repo, MHPUniverse_Repo>();
builder.Services.AddTransient<IMHPData_Repo, MHPData_Repo>();

builder.Services.AddTransient<IEDCAdhoc_Repo, EDCAdhoc_Repo>();

builder.Services.AddTransient<IETGFactSymmetry_Repo, ETGFactSymmetry_Repo>();
//builder.Services.AddSingleton<IChemotherapyPX_Repo, ChemotherapyPX_EF_Repo>();
//builder.Services.AddSingleton<IConfiguration>(configuration);

////ADD DBContext
builder.Services.AddDbContext<ChemotherapyPX_Context>(options =>
        options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));




//builder.Services.TryAddSingleton<IHttpContextAccessor, HttpContextAccessor>();



//https://stackoverflow.com/questions/68250161/error-on-cors-policy-using-asp-net-core-5-and-blazor
//ALLOWS US TO AVOID Access to fetch has been blocked by CORS policy: No 'Access-Control-Allow-Origin'  ON THE CLIENT
builder.Services.AddCors(options =>
{
    options.AddPolicy("CorsPolicy",
        builder => builder
            .AllowAnyMethod()
            .AllowCredentials()
            .SetIsOriginAllowed((host) => true)//Having the host set as true means that it will allow any browser access this one. To limit this, replace (host) => true with (host) => {return host == "my.domain.com";} to allow just your trusted domain.
            .AllowAnyHeader());
});


//BUILD

try
{
    Log.Information("Starting VC API Server...");

    var app = builder.Build();

    //2 CONFIGURE MIDDLEWARE
    //2 CONFIGURE MIDDLEWARE
    //2 CONFIGURE MIDDLEWARE

    // Configure the HTTP request pipeline.
    if (app.Environment.IsDevelopment())
    {
        app.UseSwagger();
        app.UseSwaggerUI();
    }

    app.UseAuthentication();
    app.UseAuthorization();

    //"CorsPolicy" SET ABOVE
    app.UseCors("CorsPolicy");
    app.UseHttpsRedirection();

    app.UseSerilogRequestLogging();
    

    //MINIMAL API
    //EXTENSION METHOD WITHIN Api.cs
    //HANDLE ALL ENDPOINTS
    app.ConfigureMHPApi();
    app.ConfigureEDCAdhocApi();
    app.ConfigureChemoPXApi();
    app.ConfigureGlobalApi();
    app.ConfigureETGFactSymmetryApi();
    app.ConfigureADCallsApi(configuration);

    app.Run();
}
catch(Exception ex)
{
    Log.Fatal(ex, "Fatal Exception in VC API Server");
}
finally
{
    Log.Information("Exiting VC API Server starup");
    Log.CloseAndFlush();
}



