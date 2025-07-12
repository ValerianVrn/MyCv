using MediatR;
using Moq;
using MyCv.Common.Domain.EventSourcing;

namespace MyCv.Common.Domain.UnitTests.EventSourcing
{
    [TestClass]
    public class AggregateRootTest
    {
        private class DummyAggregateRootTest(Guid guid) : AggregateRoot(guid)
        {
            public List<INotification> AppliedEvents { get; private set; } = [];

            protected override void Apply(INotification @event)
            {
                AppliedEvents.Add(@event);
            }
        }

        private class DummyEntity(Guid guid) : Entity(guid)
        {

        }

        [TestMethod]
        public void Rehydrate_CustomEvents_AppliesEvents()
        {
            // Arrange.
            var domainEvent = new Mock<INotification>();
            var aggregateRoot = new DummyAggregateRootTest(Guid.NewGuid());

            // Act.
            aggregateRoot.Rehydrate([domainEvent.Object]);

            // Assert.
            Assert.AreEqual(1, aggregateRoot.AppliedEvents.Count());
            Assert.AreEqual(domainEvent.Object, aggregateRoot.AppliedEvents.First());
        }
    }
}
