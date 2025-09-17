using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace MyCv.Common.HealthChecks.Healthchecks
{
    /// <summary>
    /// Health check used to know when the service is alive.
    /// </summary>
    public class LiveHealthCheck(HealthCheckService healthCheckService) : IHealthCheck
    {
        /// <summary>
        /// Liveness tag
        /// </summary>
        public const string Tag = "live";

        /// <summary>
        /// Liveness flag.
        /// </summary>
        private readonly HealthCheckService _healthCheckService = healthCheckService;

        /// <inheritdoc/>
        public Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = default)
        {
            var report = _healthCheckService.CheckHealthAsync(hc => hc.Tags.Contains(Tag) && hc.Name != Tag, cancellationToken).Result;

            if (report.Status == HealthStatus.Healthy)
            {
                return Task.FromResult(HealthCheckResult.Healthy("The microservice is alive."));
            }

            return Task.FromResult(HealthCheckResult.Unhealthy("The microservice is dead."));
        }
    }
}
