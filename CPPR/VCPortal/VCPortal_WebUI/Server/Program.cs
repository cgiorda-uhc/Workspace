

using DocumentFormat.OpenXml.Office2016.Drawing.ChartDrawing;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection.Extensions;
using VCPortal_WebUI.Server.Services;

var builder = WebApplication.CreateBuilder(args);



builder.Services.AddControllersWithViews();
builder.Services.AddRazorPages();


builder.Services.AddScoped<IVCPortal_Services, VCPortal_Services>();
builder.Services
    .AddHttpClient<VCPortal_Services>("VCPortal_Services", client => client.BaseAddress = new Uri(builder.Configuration["MyApi:VCPortal_Services"]));


using var loggerFactory = LoggerFactory.Create(loggingBuilder => loggingBuilder
    .SetMinimumLevel(LogLevel.Debug)
    .AddConsole()
.AddDebug());


builder.Services.AddLogging();

//builder.Services.AddHttpContextAccessor();
//builder.Services.AddScoped<IHttpContextAccessor, HttpContextAccessor>();
//builder.Services.AddScoped<IClientAuthorizationService, ClientAuthorizationService>();

var app = builder.Build();


//CUSTOM LOGGER ADDED TO ILOGGER
var sericeProvider = app.Services.CreateScope().ServiceProvider;
var IVCPortal_Services = sericeProvider.GetRequiredService<IVCPortal_Services>();
loggerFactory.AddProvider(new ApplicationLoggerProvider(IVCPortal_Services));


//VCPortal_WebUI.Server.Logging.ApplicationLogging.LoggerFactory = loggerFactory;
ApplicationLogging.LoggerFactory = loggerFactory;

//ILogger<Program> logger = loggerFactory.CreateLogger<Program>();
ILogger logger = ApplicationLogging.CreateLogger<Program>();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
	app.UseWebAssemblyDebugging();
    //app.UseSwagger();
    //app.UseSwaggerUI();
}
else
{
	app.UseExceptionHandler("/Error");
	// The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
	app.UseHsts();
}

app.UseHttpsRedirection();

app.UseBlazorFrameworkFiles();
app.UseStaticFiles();



app.UseRouting();


app.MapRazorPages();
app.MapControllers();
app.MapFallbackToFile("index.html");



logger.LogInformation("Server: Starting up");
    try
    {
        //var s = System.Security.Principal.WindowsIdentity.GetCurrent().Name;
        app.Run();
    //var username = HttpContext.Current.User.Identity.Name;
}
    catch (Exception ex)
    {
        logger.LogCritical(ex, "Server: An exception occurred while creating the web host");
    }
    finally
    {
       // Log.CloseAndFlush();
    }

