using Microsoft.EntityFrameworkCore;
using MyCv.Common.HealthChecks.Healthchecks;
using MyCv.Rating.Api.Write.Extensions.Loggers;
using MyCv.Rating.Infrastructure;

namespace MyCv.Rating.Api.Write.Services
{
    /// <summary>
    /// Service triggered for startup operations.
    /// </summary>
    /// <remarks>
    /// Constructor.
    /// </remarks>
    /// <param name="serviceProvider"></param>
    /// <param name="hostEnvironment"></param>
    /// <param name="healthCheck"></param>
    public class StartupBackgroundService(IServiceProvider serviceProvider, IHostEnvironment hostEnvironment, StartupHealthCheck healthCheck) : BackgroundService
    {
        /// <summary>
        /// Service provider.
        /// </summary>
        private readonly IServiceProvider _serviceProvider = serviceProvider;

        /// <summary>
        /// Host environment.
        /// </summary>
        private readonly IHostEnvironment _hostEnvironment = hostEnvironment;

        /// <summary>
        /// Startup health check.
        /// </summary>
        private readonly StartupHealthCheck _startupHealthCheck = healthCheck;

        /// <inheritdoc/>
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            // Get the services.
            using var scope = _serviceProvider.CreateScope();
            var logger = _serviceProvider.GetRequiredService<ILogger<StartupBackgroundService>>();

            logger.StartingStartupBackgroundService();

            // Migrate database only on development environment.
            if (_hostEnvironment.IsDevelopment())
            {
                // Apply migrations.
                logger.ApplyingMigrations();

                // Migrate context.
                var ratingContext = scope.ServiceProvider.GetRequiredService<RatingContext>();
                await ratingContext.Database.MigrateAsync(stoppingToken);
            }

            logger.StartupEnded();

            _startupHealthCheck.StartupCompleted = true;
        }
    }
}
