namespace MyCv.Rating.Api.Write.Extensions.Loggers
{
    public static partial class StartupBackgroundServiceLogger
    {
        [LoggerMessage(EventId = 1, Level = LogLevel.Information, Message = "Starting startup background service")]
        internal static partial void StartingStartupBackgroundService(this ILogger logger);

        [LoggerMessage(EventId = 2, Level = LogLevel.Information, Message = "Applying migrations")]
        internal static partial void ApplyingMigrations(this ILogger logger);

        [LoggerMessage(EventId = 3, Level = LogLevel.Information, Message = "Startup ended")]
        internal static partial void StartupEnded(this ILogger logger);
    }
}
