using MediatR;
using Moq;
using MyCv.Common.Domain.Cqrs;

namespace MyCv.Common.Domain.UnitTests.Cqrs
{
    [TestClass]
    public class EntityTest
    {
        private class TestEntity : Entity
        {
            public TestEntity(Guid guid) : base(guid)
            {

            }
        }

        private class OtherTestEntity : Entity
        {
            public OtherTestEntity(Guid guid) : base(guid)
            {

            }
        }

        #region Constructor
        [TestMethod]
        public void Constructor_ValidGuid_EntityHasGuid()
        {
            // Arrange
            var guid = Guid.NewGuid();

            // Act
            var entity = new TestEntity(guid);

            // Assert
            Assert.AreEqual(guid, entity.EntityGuid);
        }

        [TestMethod]
        public void Constructor_EmptyGuid_ThrowsException()
        {
            // Arrange.
            var guid = Guid.Empty;

            // Act.
            var act = () => new TestEntity(guid);

            // Assert.
            Assert.ThrowsException<ArgumentNullException>(act);
        }
        #endregion

        #region DomainEvents
        [TestMethod]
        public void AddDomainEvent_DomainEvent_DomainEventIsAdded()
        {
            // Arrange.
            var domainEvent = new Mock<INotification>();
            var entity = new TestEntity(Guid.NewGuid());

            // Act.
            entity.AddDomainEvent(domainEvent.Object);

            // Assert.
            Assert.IsNotNull(entity.DomainEvents);
            Assert.AreEqual(1, entity.DomainEvents.Count);
            Assert.AreEqual(domainEvent.Object, entity.DomainEvents.First());
        }

        [TestMethod]
        public void RemoveDomainEvent_DomainEvent_DomainEventIsRemoved()
        {
            // Arrange.
            var domainEvent = new Mock<INotification>();
            var entity = new TestEntity(Guid.NewGuid());
            entity.AddDomainEvent(domainEvent.Object);

            // Act.
            entity.RemoveDomainEvent(domainEvent.Object);

            // Assert.
            Assert.AreEqual(0, entity.DomainEvents.Count);
        }

        [TestMethod]
        public void ClearDomainEvents_TwoDomainEvents_DomainEventsAreRemoved()
        {
            // Arrange.
            var domainEvent1 = new Mock<INotification>();
            var domainEvent2 = new Mock<INotification>();
            var entity = new TestEntity(Guid.NewGuid());
            entity.AddDomainEvent(domainEvent1.Object);
            entity.AddDomainEvent(domainEvent2.Object);

            // Act.
            entity.ClearDomainEvents();

            // Assert.
            Assert.AreEqual(0, entity.DomainEvents.Count);
        }
        #endregion

        #region Equality
        [TestMethod]
        public void Equals_EqualEntities_ReturnsTrue()
        {
            // Arrange.
            var entity1 = new TestEntity(Guid.NewGuid());

            // Act.
            var equals = entity1.Equals(entity1);

            // Assert.
            Assert.IsTrue(equals);
        }

        [TestMethod]
        [DynamicData(nameof(NonEqualEntities))]
        public void Equals_NonEqualEntities_ReturnsFalse(object entity2)
        {
            // Arrange.
            var entity1 = new TestEntity(Guid.NewGuid());

            // Act.
            var equals = entity1.Equals(entity2);

            // Assert.
            Assert.IsFalse(equals);
        }

        [TestMethod]
        public void EqualityOperator_SameGuid_ReturnsTrue()
        {
            // Arrange.
            var guid = Guid.NewGuid();
            var entity1 = new TestEntity(guid);
            var entity2 = new TestEntity(guid);

            // Act.
            var result = entity1 == entity2;

            // Assert.
            Assert.IsTrue(result);
        }

        [TestMethod]
        public void EqualityOperator_DifferentGuid_ReturnsFalse()
        {
            // Arrange.
            var entity1 = new TestEntity(Guid.NewGuid());
            var entity2 = new TestEntity(Guid.NewGuid());

            // Act.
            var result = entity1 == entity2;

            // Assert.
            Assert.IsFalse(result);
        }

        [TestMethod]
        public void EqualityOperator_NullEntity_ReturnsFalse()
        {
            // Arrange.
            var entity = new TestEntity(Guid.NewGuid());

            // Act.
            var result = null == entity;

            // Assert.
            Assert.IsFalse(result);
        }

        [TestMethod]
        public void InequalityOperator_SameGuid_ReturnsFalse()
        {
            // Arrange.
            var guid = Guid.NewGuid();
            var entity1 = new TestEntity(guid);
            var entity2 = new TestEntity(guid);

            // Act.
            var result = entity1 != entity2;

            // Assert.
            Assert.IsFalse(result);
        }

        [TestMethod]
        public void InequalityOperator_DifferentGuid_ReturnsTrue()
        {
            // Arrange.
            var entity1 = new TestEntity(Guid.NewGuid());
            var entity2 = new TestEntity(Guid.NewGuid());

            // Act.
            var result = entity1 != entity2;

            // Assert.
            Assert.IsTrue(result);
        }

        public static IEnumerable<object[]> NonEqualEntities
        {
            get
            {
                return new[]
                {
                    new object[]
                    {
                        default(TestEntity)
                    },
                    new object[]
                    {
                        string.Empty
                    },
                    new object[]
                    {
                        new OtherTestEntity(Guid.NewGuid())
                    },
                    new object[]
                    {
                        new TestEntity(Guid.NewGuid())
                    }
                };
            }
        }
        #endregion
    }
}
