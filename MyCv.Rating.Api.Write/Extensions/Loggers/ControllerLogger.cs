namespace MyCv.Rating.Api.Write.Extensions.Loggers
{
    public static partial class ControllerLogger
    {
        [LoggerMessage(EventId = 1, Level = LogLevel.Error, Message = "Failed to execute method.")]
        internal static partial void FailedToExecuteMethod(this ILogger logger, Exception exception);
    }
}
