using Microsoft.Extensions.Diagnostics.HealthChecks;
using Moq;

namespace MyCv.Common.HealthChecks.UnitTests.Healthchecks
{
    [TestClass]
    public class ReadyHealthCheckTest
    {
        /// <summary>
        /// Health check service.
        /// </summary>
        private readonly Mock<HealthCheckService> _healthCheckService;

        public ReadyHealthCheckTest()
        {
            _healthCheckService = new Mock<HealthCheckService>();
        }

        [TestMethod]
        public void CheckHealthAsync_AllReadyAreHealthy_ReturnHealthy()
        {
            // Arrange. 
            var readyHealthCheck = new ReadyHealthCheck(_healthCheckService.Object);
            var healthReport = new HealthReport(default, HealthStatus.Healthy, default);
            _healthCheckService.Setup(h => h.CheckHealthAsync(It.IsAny<Func<HealthCheckRegistration, bool>>(), It.IsAny<CancellationToken>())).ReturnsAsync(healthReport);

            // Act.
            var result = readyHealthCheck.CheckHealthAsync(default);

            // Assert.
            Assert.AreEqual(HealthCheckResult.Healthy().Status, result.Result.Status);

        }

        [TestMethod]
        public void CheckHealthAsync_NotAllReadyAreHealthy_ReturnUnhealthy()
        {
            // Arrange. 
            var readyHealthCheck = new ReadyHealthCheck(_healthCheckService.Object);
            var healthReport = new HealthReport(default, HealthStatus.Unhealthy, default);
            _healthCheckService.Setup(h => h.CheckHealthAsync(It.IsAny<Func<HealthCheckRegistration, bool>>(), It.IsAny<CancellationToken>())).ReturnsAsync(healthReport);

            // Act.
            var result = readyHealthCheck.CheckHealthAsync(default);

            // Assert.
            Assert.AreEqual(HealthCheckResult.Unhealthy().Status, result.Result.Status);
        }
    }
}
