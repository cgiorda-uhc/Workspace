using Microsoft.Extensions.Logging;
using Microsoft.Extensions.DependencyInjection;

namespace VCPortal_WebUI.Client.Logging;
//https://stackoverflow.com/questions/48676152/asp-net-core-web-api-logging-from-a-static-class
public static class ApplicationLogging
{
    public static ILoggerFactory LoggerFactory { get; set; }// = new LoggerFactory();
    public static ILogger CreateLogger<T>() => LoggerFactory.CreateLogger<T>();
    public static ILogger CreateLogger(string categoryName) => LoggerFactory.CreateLogger(categoryName);

}
