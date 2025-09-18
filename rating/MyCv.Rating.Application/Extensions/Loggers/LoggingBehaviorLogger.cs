using Microsoft.Extensions.Logging;

namespace MyCv.Rating.Application.Extensions.Loggers
{
    internal static partial class LoggingBehaviorLogger
    {
        [LoggerMessage(EventId = 1, Level = LogLevel.Information, Message = "Handling command {CommandName} ({CommandGuid}): {Command}")]
        internal static partial void HandlingCommand(this ILogger logger, string? commandName, Guid commandGuid, object command);

        [LoggerMessage(EventId = 2, Level = LogLevel.Information, Message = "Command {CommandName} ({CommandGuid}) handled: {Response}")]
        internal static partial void ResponseCommand(this ILogger logger, string? commandName, Guid commandGuid, object? response);
    }
}
