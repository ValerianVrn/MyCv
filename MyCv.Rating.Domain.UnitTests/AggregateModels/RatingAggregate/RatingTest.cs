namespace MyCv.Rating.Domain.UnitTests.AggregateModels.RatingAggregate
{
    [TestClass]
    public class RatingTest
    {
        [TestMethod]
        public void Constructor_ValidRating_RatingIsNotNull()
        {
            // Arrange.
            var entityGuid = Guid.NewGuid();
            var visitorId = "1";
            var ratingValue = 1;
            var recommend = true;

            // Act.
            var rating = new Domain.AggregateModels.RatingAggregate.Rating(entityGuid, visitorId, ratingValue, recommend);

            // Assert.
            Assert.IsNotNull(rating);
            Assert.AreEqual(entityGuid, rating.EntityGuid);
            Assert.AreEqual(visitorId, rating.VisitorId);
            Assert.AreEqual(ratingValue, rating.RatingValue);
            Assert.AreEqual(recommend, rating.Recommend);
            Assert.AreEqual(0, rating.DomainEvents.Count);
        }

        [TestMethod]
        public void Constructor_EmptyGuid_ThrowsException()
        {
            // Arrange.
            var entityGuid = Guid.Empty;
            var visitorId = "1";
            var ratingValue = 1;
            var recommend = true;

            // Act.
            Domain.AggregateModels.RatingAggregate.Rating act()
            {
                return new(entityGuid, visitorId, ratingValue, recommend);
            }

            // Assert.
            _ = Assert.ThrowsException<ArgumentNullException>((Func<Domain.AggregateModels.RatingAggregate.Rating>)act);
        }

        [TestMethod]
        public void Constructor_EmptyVisitorId_ThrowsException()
        {
            // Arrange.
            var entityGuid = Guid.NewGuid();
            var visitorId = string.Empty;
            var ratingValue = 1;
            var recommend = true;

            // Act.
            Domain.AggregateModels.RatingAggregate.Rating act()
            {
                return new(entityGuid, visitorId, ratingValue, recommend);
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
            var ratingValue = -1;
            var recommend = true;

            // Act.
            Domain.AggregateModels.RatingAggregate.Rating act()
            {
                return new(entityGuid, visitorId, ratingValue, recommend);
            }

            // Assert.
            _ = Assert.ThrowsException<ArgumentOutOfRangeException>(act);
        }
    }
}
