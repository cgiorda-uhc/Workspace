using Microsoft.Extensions.Logging;
using Microsoft.Extensions.DependencyInjection;

namespace VCPortal_WebUI.Server.Logging;
//https://stackoverflow.com/questions/48676152/asp-net-core-web-api-logging-from-a-static-class
internal static class ApplicationLogging
{
    internal static ILoggerFactory LoggerFactory { get; set; }// = new LoggerFactory();
    internal static ILogger CreateLogger<T>() => LoggerFactory.CreateLogger<T>();
    internal static ILogger CreateLogger(string categoryName) => LoggerFactory.CreateLogger(categoryName);

}
