using Microsoft.Extensions.Logging;
using Volo.Abp.ExceptionHandling;
using Volo.Abp.Validation;

namespace FuseDigital.QuickSetup.Logging;

public static class LoggerExtensions
{
    public static void LogBusinessException(this ILogger logger, Exception exception)
    {
        var logLevel = exception.GetLogLevel(LogLevel.Warning);

        if (!string.IsNullOrEmpty(exception.Message))
        {
            logger.LogWithLevel(logLevel, exception.Message);
        }

        if (exception is IHasErrorCode errorCode && !string.IsNullOrEmpty(errorCode.Code))
        {
            logger.LogWithLevel(logLevel, "Code:" + errorCode.Code);
        }

        if (exception is IHasErrorDetails errorDetails && !string.IsNullOrEmpty(errorDetails.Details))
        {
            logger.LogWithLevel(logLevel, "Details:" + errorDetails.Details);
        }

        if (exception is IHasValidationErrors {ValidationErrors: { }} validationException)
        {
            var message = validationException.ValidationErrors
                .SelectMany(x => x.MemberNames.Select(y => $"\t - {y} : {x.ErrorMessage}"))
                .JoinAsString(Environment.NewLine);

            logger.LogWithLevel(logLevel, $"Validation errors: {Environment.NewLine}{message}");
        }
    }
}