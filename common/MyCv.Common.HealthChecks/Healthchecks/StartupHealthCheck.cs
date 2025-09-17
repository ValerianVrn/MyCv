using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace MyCv.Common.HealthChecks.Healthchecks
{
    /// <summary>
    /// Health check used for startup operations.
    /// </summary>
    public class StartupHealthCheck : IHealthCheck
    {
        /// <summary>
        /// Readiness flag.
        /// </summary>
        private volatile bool _isReady;

        /// <summary>
        /// Startup completed flag.
        /// </summary>
        public bool StartupCompleted
        {
            get
            {
                return _isReady;
            }

            set
            {
                _isReady = value;
            }
        }

        /// <inheritdoc/>
        public Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = default)
        {
            if (StartupCompleted)
            {
                return Task.FromResult(HealthCheckResult.Healthy("The startup task has completed."));
            }

            return Task.FromResult(HealthCheckResult.Unhealthy("That startup task is still running."));
        }
    }
}
