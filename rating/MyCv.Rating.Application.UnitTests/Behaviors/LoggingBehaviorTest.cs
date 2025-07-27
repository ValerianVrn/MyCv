using MediatR;
using Microsoft.Extensions.Logging;
using Moq;
using MyCv.Rating.Application.Behaviors;

namespace MyCv.Rating.Application.UnitTests.Behaviors
{
    [TestClass]
    public class LoggingBehaviorTest
    {
        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void Constructor_LoggerIsNull_ShouldThrowArgumentNullException()
        {
            // Arrange
            ILogger<LoggingBehavior<IRequest<It.IsAnyType>, It.IsAnyType>>? logger = null;

            // Act & Assert
            _ = new LoggingBehavior<IRequest<It.IsAnyType>, It.IsAnyType>(logger!);
        }

        [TestMethod]
        public async Task Handle_ValidArgument_LoggerIsCalled()
        {
            // Arrange.
            var logger = new Mock<ILogger<LoggingBehavior<IRequest<It.IsAnyType>, It.IsAnyType>>>();
            _ = logger.Setup(m => m.IsEnabled(It.IsAny<LogLevel>())).Returns(true);
            var loggingBehavior = new LoggingBehavior<IRequest<It.IsAnyType>, It.IsAnyType>(logger.Object);

            // Act.
            _ = await loggingBehavior.Handle(new Mock<IRequest<It.IsAnyType>>().Object, new Mock<RequestHandlerDelegate<It.IsAnyType>>().Object, CancellationToken.None);

            // Assert.
            logger.Verify(l => l.Log(It.IsAny<LogLevel>(), It.IsAny<EventId>(), It.IsAny<It.IsAnyType>(), It.IsAny<Exception?>(), It.IsAny<Func<It.IsAnyType, Exception?, string>>()), Times.AtLeastOnce());
        }
    }
}
