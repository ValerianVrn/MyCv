using Microsoft.Extensions.Diagnostics.HealthChecks;
using Moq;
using MyCv.Common.HealthChecks.Healthchecks;

namespace MyCv.Common.HealthChecks.UnitTests.Healthchecks
{
    [TestClass]
    public class LiveHealthCheckTest
    {
        /// <summary>
        /// Health check service.
        /// </summary>
        private readonly Mock<HealthCheckService> _healthCheckService;

        public LiveHealthCheckTest()
        {
            _healthCheckService = new Mock<HealthCheckService>();
        }

        [TestMethod]
        public void CheckHealthAsync_AllLiveAreHealthy_ReturnHealthy()
        {
            // Arrange. 
            var liveHealthCheck = new LiveHealthCheck(_healthCheckService.Object);
            var healthReport = new HealthReport(default, HealthStatus.Healthy, default);
            _healthCheckService.Setup(h => h.CheckHealthAsync(It.IsAny<Func<HealthCheckRegistration, bool>>(), It.IsAny<CancellationToken>())).ReturnsAsync(healthReport);

            // Act.
            var result = liveHealthCheck.CheckHealthAsync(default);

            // Assert.
            Assert.AreEqual(HealthCheckResult.Healthy().Status, result.Result.Status);

        }

        [TestMethod]
        public void CheckHealthAsync_NotAllLiveAreHealthy_ReturnUnhealthy()
        {
            // Arrange. 
            var liveHealthCheck = new LiveHealthCheck(_healthCheckService.Object);
            var healthReport = new HealthReport(default, HealthStatus.Unhealthy, default);
            _healthCheckService.Setup(h => h.CheckHealthAsync(It.IsAny<Func<HealthCheckRegistration, bool>>(), It.IsAny<CancellationToken>())).ReturnsAsync(healthReport);

            // Act.
            var result = liveHealthCheck.CheckHealthAsync(default);

            // Assert.
            Assert.AreEqual(HealthCheckResult.Unhealthy().Status, result.Result.Status);
        }
    }
}
