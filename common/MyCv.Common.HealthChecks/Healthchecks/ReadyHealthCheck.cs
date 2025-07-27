using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace MyCv.Common.HealthChecks.Healthchecks
{
    /// <summary>
    /// Health check used to know when service is ready.
    /// </summary>
    public class ReadyHealthCheck(HealthCheckService healthCheckService) : IHealthCheck
    {
        /// <summary>
        /// Readiness tag
        /// </summary>
        public const string Tag = "ready";

        /// <summary>
        /// Readiness flag.
        /// </summary>
        private readonly HealthCheckService _healthCheckService = healthCheckService;

        /// <inheritdoc/>
        public Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = default)
        {
            var report = _healthCheckService.CheckHealthAsync(hc => hc.Tags.Contains(Tag) && hc.Name != Tag, cancellationToken).Result;

            if (report.Status == HealthStatus.Healthy)
            {
                return Task.FromResult(HealthCheckResult.Healthy("The microservice is ready."));
            }

            return Task.FromResult(HealthCheckResult.Unhealthy("The microservice is not ready."));
        }
    }
}
