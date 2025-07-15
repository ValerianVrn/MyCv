using MediatR;
using Microsoft.EntityFrameworkCore;
using Moq;
using MyCv.Rating.Application.Behaviors;

namespace MyCv.Rating.Application.UnitTests.Behaviors
{
    [TestClass]
    public class RetryBehaviorTest
    {
        [TestMethod]
        public async Task Handle_ValidArgument_NoException()
        {
            // Arrange.
            var retryBehavior = new RetryBehavior<IRequest<It.IsAnyType>, It.IsAnyType>();

            // Act.
            _ = await retryBehavior.Handle(new Mock<IRequest<It.IsAnyType>>().Object, new Mock<RequestHandlerDelegate<It.IsAnyType>>().Object, CancellationToken.None);

            // Assert.
        }

        [TestMethod]
        public async Task Handle_DbUpdateConcurrencyException_Retry()
        {
            // Arrange.
            var retryBehavior = new RetryBehavior<IRequest<It.IsAnyType>, It.IsAnyType>();
            var requestHandlerDelegate = new Mock<RequestHandlerDelegate<It.IsAnyType>>();
            _ = requestHandlerDelegate.SetupSequence(r => r())
                                  .Throws(new DbUpdateConcurrencyException())
                                  .Throws(new DbUpdateConcurrencyException())
                                  .Throws(new DbUpdateConcurrencyException())
                                  .ReturnsAsync(await new Mock<RequestHandlerDelegate<It.IsAnyType>>().Object());

            // Act.
            _ = await retryBehavior.Handle(new Mock<IRequest<It.IsAnyType>>().Object, requestHandlerDelegate.Object, CancellationToken.None);

            // Assert.
            requestHandlerDelegate.Verify(r => r(), Times.Exactly(4));
        }
    }
}
