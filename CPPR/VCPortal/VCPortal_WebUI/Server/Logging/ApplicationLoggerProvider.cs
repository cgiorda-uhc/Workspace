using Microsoft.Extensions.Logging;
using System.Net.Http.Headers;
using VCPortal_WebUI.Client.Services.Shared;

namespace VCPortal_WebUI.Server.Logging;

public class ApplicationLoggerProvider : ILoggerProvider
{
    private readonly IVCPortal_Services _IVCPortal_Services;

    public ApplicationLoggerProvider( IVCPortal_Services IVCPortal_Services)
    {
        _IVCPortal_Services = IVCPortal_Services;
    }

    public ILogger CreateLogger(string categoryName)
    {
        return new LogSetter(_IVCPortal_Services);
    }

    public void Dispose()
    {
        
    }
}
