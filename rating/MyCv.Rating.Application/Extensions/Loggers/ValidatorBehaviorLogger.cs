using Microsoft.Extensions.Logging;

namespace MyCv.Rating.Application.Extensions.Loggers
{
    internal static partial class ValidatorBehaviorLogger
    {
        [LoggerMessage(EventId = 1, Level = LogLevel.Information, Message = "Validating command {CommandType}")]
        internal static partial void ValidatingCommand(this ILogger logger, string commandType);

        [LoggerMessage(EventId = 2, Level = LogLevel.Error, Message = "Validation errors - {CommandType} - Command: {Command} - Errors: {ValidationErrors}")]
        internal static partial void ErrorLogOnCommandValidation(this ILogger logger, string commandType, object command, string validationErrors);

        [LoggerMessage(EventId = 3, Level = LogLevel.Warning, Message = "Validation errors - {CommandType} - Command: {Command} - Errors: {ValidationErrors}")]
        internal static partial void WarningLogOnCommandValidation(this ILogger logger, string commandType, object command, string validationErrors);

        [LoggerMessage(EventId = 4, Level = LogLevel.Information, Message = "Validation errors - {CommandType} - Command: {Command} - Errors: {ValidationErrors}")]
        internal static partial void InformationLogOnCommandValidation(this ILogger logger, string commandType, object command, string validationErrors);
    }
}
