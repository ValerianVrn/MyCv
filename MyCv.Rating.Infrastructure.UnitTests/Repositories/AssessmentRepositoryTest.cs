using MyCv.Rating.Domain.AggregateModels.AssessmentAggregate;
using MyCv.Rating.Infrastructure.Repositories;

namespace MyCv.Rating.Infrastructure.UnitTests.Repositories
{
    [TestClass]
    public class AssessmentRepositoryTest
    {
        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void Constructor_ThrowException_ContextIsNull()
        {
            _ = new AssessmentRepository(null!);
        }

        [TestMethod]
        public async Task GetAllAsync_TwoSavedAssessments_ReturnsAssessments()
        {
            // Arrange.
            var context = Helper.CreateRatingContext();
            var assessmentRepository = new AssessmentRepository(context);
            var savedAssessments = new List<Assessment>
            {
                new(Guid.NewGuid(), "1", 2, true),
                new(Guid.NewGuid(), "2", 3, true)
            };
            await context.Assessments.AddRangeAsync(savedAssessments);
            _ = await context.SaveChangesAsync();

            // Act.
            var assessments = await assessmentRepository.GetAllAsync(CancellationToken.None);

            // Assert
            Assert.IsNotNull(assessments);
            Assert.AreEqual(2, assessments.Count());
        }

        [TestMethod]
        public async Task Create_Assessment_AssessmentIsSavedAndReturnEntity()
        {
            // Arrange.
            var context = Helper.CreateRatingContext();
            var assessmentRepository = new AssessmentRepository(context);
            var assessment = new Assessment(Guid.NewGuid(), "1", 2, true);

            // Act.
            var savedAssessment = assessmentRepository.Create(assessment);
            _ = await context.SaveChangesAsync();

            // Assert.
            Assert.AreEqual(1, context.Assessments.Count());
            Assert.AreEqual(assessment, savedAssessment);
        }
    }
}
