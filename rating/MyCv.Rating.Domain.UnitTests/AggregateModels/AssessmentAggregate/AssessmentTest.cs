using MyCv.Rating.Domain.AggregateModels.AssessmentAggregate;

namespace MyCv.Rating.Domain.UnitTests.AggregateModels.AssessmentAggregate
{
    [TestClass]
    public class AssessmentTest
    {
        [TestMethod]
        public void Constructor_ValidRating_RatingIsNotNull()
        {
            // Arrange.
            var entityGuid = Guid.NewGuid();
            var visitorId = "1";
            var score = 1;
            var recommend = true;

            // Act.
            var assessment = new Assessment(entityGuid, visitorId, score, recommend);

            // Assert.
            Assert.IsNotNull(assessment);
            Assert.AreEqual(entityGuid, assessment.EntityGuid);
            Assert.AreEqual(visitorId, assessment.VisitorId);
            Assert.AreEqual(score, assessment.Score);
            Assert.AreEqual(recommend, assessment.Recommend);
            Assert.AreEqual(0, assessment.DomainEvents.Count);
        }

        [TestMethod]
        public void Constructor_EmptyGuid_ThrowsException()
        {
            // Arrange.
            var entityGuid = Guid.Empty;
            var visitorId = "1";
            var score = 1;
            var recommend = true;

            // Act.
            Assessment act()
            {
                return new(entityGuid, visitorId, score, recommend);
            }

            // Assert.
            _ = Assert.ThrowsException<ArgumentNullException>((Func<Assessment>)act);
        }

        [TestMethod]
        public void Constructor_EmptyVisitorId_ThrowsException()
        {
            // Arrange.
            var entityGuid = Guid.NewGuid();
            var visitorId = string.Empty;
            var score = 1;
            var recommend = true;

            // Act.
            Assessment act()
            {
                return new(entityGuid, visitorId, score, recommend);
            }

            // Assert.
            _ = Assert.ThrowsException<ArgumentNullException>(act);
        }

        [TestMethod]
        public void Constructor_NegativeRatingValue_ThrowsException()
        {
            // Arrange.
            var entityGuid = Guid.NewGuid();
            var visitorId = "1";
            var score = -1;
            var recommend = true;

            // Act.
            Assessment act()
            {
                return new(entityGuid, visitorId, score, recommend);
            }

            // Assert.
            _ = Assert.ThrowsException<ArgumentOutOfRangeException>(act);
        }
    }
}
