using MediatR;
using Moq;
using MyCv.Rating.Application.Behaviors;
using Prometheus;
using System.Text;

namespace MyCv.Rating.Application.UnitTests.Behaviors
{
    [TestClass]
    public class MetricsBehaviorTest
    {
        [TestMethod]
        public async Task Handle_ValidArgument_MetricsAreCollected()
        {
            // Arrange.
            var request = new Mock<IRequest<It.IsAnyType>>();
            var next = new Mock<RequestHandlerDelegate<It.IsAnyType>>();
            var cancellationToken = CancellationToken.None;
            var metricsBehavior = new MetricsBehavior<IRequest<It.IsAnyType>, It.IsAnyType>();

            // Act.
            _ = await metricsBehavior.Handle(request.Object, next.Object, cancellationToken);

            // Assert.
            var exportStream = new MemoryStream();
            await Metrics.DefaultRegistry.CollectAndExportAsTextAsync(exportStream, CancellationToken.None);
            exportStream.Position = 0;
            using var reader = new StreamReader(exportStream, Encoding.UTF8);
            var exportedText = await reader.ReadToEndAsync();
            Assert.IsTrue(exportedText.Contains("command_duration_seconds"));
            Assert.IsTrue(exportedText.Contains("command_total"));
            Assert.IsTrue(exportedText.Contains(request.Object.GetType().Name));
        }
    }
}
