namespace MyCv.Rating.Api.Read.Extensions.Loggers
{
    public static partial class StartupBackgroundServiceLogger
    {
        [LoggerMessage(EventId = 1, Level = LogLevel.Information, Message = "Starting startup background service")]
        internal static partial void StartingStartupBackgroundService(this ILogger logger);

        [LoggerMessage(EventId = 2, Level = LogLevel.Information, Message = "Applying migrations")]
        internal static partial void ApplyingMigrations(this ILogger logger);
    }
}
