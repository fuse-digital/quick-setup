using Microsoft.Extensions.Logging;
using Volo.Abp.ExceptionHandling;

namespace FuseDigital.QuickSetup.Logging;

public static class LoggerExtensions
{
    public static void LogBusinessException(this ILogger logger, Exception exception)
    {
        var logLevel = exception.GetLogLevel(LogLevel.Warning);

        logger.LogWithLevel(logLevel, exception.Message);

        if (exception is IHasErrorCode errorCode && !string.IsNullOrEmpty(errorCode.Code))
        {
            logger.LogWithLevel(logLevel, "Code:" + errorCode.Code);
        }

        if (exception is IHasErrorDetails errorDetails && !string.IsNullOrEmpty(errorDetails.Details))
        {
            logger.LogWithLevel(logLevel, "Details:" + errorDetails.Details);
        }
    }
}