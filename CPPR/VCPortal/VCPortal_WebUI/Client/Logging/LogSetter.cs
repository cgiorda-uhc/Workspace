using System.Net.Http.Headers;

namespace VCPortal_WebUI.Client.Logging;

public class LogSetter : ILogger
{
    private readonly IVCPortal_Services _IVCPortal_Services;
    public LogSetter( IVCPortal_Services IVCPortal_Services)
    {
        _IVCPortal_Services = IVCPortal_Services;

    }

    public IDisposable BeginScope<TState>(TState state) where TState : notnull
    {
        return null;
    }

    public bool IsEnabled(LogLevel logLevel)
    {
        return true;
    }

    public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception exception, Func<TState, Exception, string> formatter)
    {

        var log = new VCPortal_Models.Models.Shared.VCLog();
        log.exception_message = exception?.Message;
        log.event_name = eventId.Name;
        log.stack_trace = exception?.StackTrace;
        log.log_level = logLevel.ToString();
        log.source = "Client";
        log.state = state.ToString();


        _IVCPortal_Services.Insertlog(log);
    }
}
