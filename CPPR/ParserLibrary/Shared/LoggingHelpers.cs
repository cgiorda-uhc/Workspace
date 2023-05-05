using Serilog;
using ProjectManagerLibrary.Concrete;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Serilog.Events;
using System.Net;
using Serilog.Sinks.Email;

namespace ProjectManagerLibrary.Shared
{

    //public class LoggingFunctions
    //{

    //    //USED TO AVOID CALLING logger.IsEnabled EACH TIME. MS NEEDS TO FIX!!!
    //    public static void addToLog(ILogger logger, string strMessage, int intCode)
    //    {
    //        if(LoggingHelpers.DebugCode == intCode && logger.IsEnabled(LogLevel.Debug))
    //            logger.LogDebug(LoggingHelpers.DebugCode, strMessage);
    //        else if (LoggingHelpers.InformationCode == intCode && logger.IsEnabled(LogLevel.Information))
    //            logger.LogInformation(LoggingHelpers.InformationCode, strMessage);
    //        else if (LoggingHelpers.WarningCode == intCode && logger.IsEnabled(LogLevel.Warning))
    //            logger.LogWarning(LoggingHelpers.WarningCode, strMessage);
    //        else if (LoggingHelpers.ErrorCode == intCode && logger.IsEnabled(LogLevel.Error))
    //            logger.LogError(LoggingHelpers.ErrorCode, strMessage);
    //        else if (LoggingHelpers.CriticalCode == intCode && logger.IsEnabled(LogLevel.Critical))
    //            logger.LogCritical(LoggingHelpers.CriticalCode, strMessage);

    //    }
    //}
    
    ////TRACK MESSAGE TYPES
    //public class LoggingHelpers
    //{
    //    public const int DebugCode = 1000;
    //    public const int InformationCode = 2000;
    //    public const int WarningCode = 3000;
    //    public const int ErrorCode = 8000;
    //    public const int CriticalCode = 9000;
    //}

    public static class LoggingExtensions
    {
        public static void DataSourceVerification(this ILogger logger, string messageTemplate, params object[] args)
        {
            logger.ForContext("IsDSV", true).Information(messageTemplate, args);
        }

        public static void Important(this ILogger logger, string messageTemplate, params object[] args)
        {
            logger.ForContext("IsImportant", true).Information(messageTemplate, args);
        }

    }

}
