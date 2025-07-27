using Microsoft.Extensions.Diagnostics.HealthChecks;
using MyCv.Common.HealthChecks.Healthchecks;

namespace MyCv.Common.HealthChecks.UnitTests.Healthchecks
{
    [TestClass]
    public class StartupHealthCheckTest
    {
        [TestMethod]
        public void CheckHealthAsync_StartupCompleted_ReturnHealthy()
        {
            // Arrange. 
            var startupHealthCheck = new StartupHealthCheck
            {
                StartupCompleted = true
            };

            // Act.
            var result = startupHealthCheck.CheckHealthAsync(default);

            // Assert.
            Assert.AreEqual(HealthCheckResult.Healthy().Status, result.Result.Status);

        }

        [TestMethod]
        public void CheckHealthAsync_StartupNotCompleted_ReturnUnhealthy()
        {
            // Arrange. 
            var startupHealthCheck = new StartupHealthCheck();

            // Act.
            var result = startupHealthCheck.CheckHealthAsync(default);

            // Assert.
            Assert.AreEqual(HealthCheckResult.Unhealthy().Status, result.Result.Status);
        }
    }
}
