using MyCv.Rating.Application.Queries;
using MyCv.Rating.Domain.AggregateModels.AssessmentAggregate;

namespace MyCv.Rating.Application.UnitTests.Queries
{
    [TestClass]
    public class AssessmentQueriesTest
    {
        [TestMethod]
        public async Task GetAssessmentAsync_KnownVisitorId_ReturnsAssessment()
        {
            // Arrange.
            var context = Helper.CreateRatingContext();
            var visitorId = "123456789";
            _ = await context.Assessments.AddAsync(new Assessment(Guid.NewGuid(), visitorId, 3, true));
            _ = await context.SaveChangesAsync();
            var assessmentQueries = new AssessmentQueries(context);

            // Act.
            var assessment = await assessmentQueries.GetVisitorAssessmentAsync(visitorId);

            // Assert.
            Assert.IsNotNull(assessment);
        }

        [TestMethod]
        public async Task GetAssessmentAsync_UnknownVisitorId_ReturnsNull()
        {
            // Arrange.
            var context = Helper.CreateRatingContext();
            var assessmentQueries = new AssessmentQueries(context);

            // Act.
            var assessment = await assessmentQueries.GetVisitorAssessmentAsync("123456789");

            // Assert.
            Assert.IsNull(assessment);
        }
    }
}
