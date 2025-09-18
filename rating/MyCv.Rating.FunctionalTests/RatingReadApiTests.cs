using Microsoft.Extensions.DependencyInjection;
using MyCv.Rating.Application.ViewModels;
using MyCv.Rating.Domain.AggregateModels.AssessmentAggregate;
using MyCv.Rating.Infrastructure;
using System.Net.Http.Json;

namespace MyCv.Rating.FunctionalTests
{
    [TestClass]
    [TestCategory("Functional")]
    public class RatingReadApiTests(TestContext testContext)
    {
        /// <summary>
        /// System under test (SUT).
        /// </summary>
        private static RatingWebApplicationFactory<Api.Read.Program> s_webApplicationFactory;

        /// <summary>
        /// Context of the test.
        /// </summary>
        private readonly TestContext _testContext = testContext;

        [TestInitialize]
        public async Task InitializeAsync()
        {
            s_webApplicationFactory = new RatingWebApplicationFactory<Api.Read.Program>();
            await s_webApplicationFactory.InitializeAsync(_testContext.TestName ?? "notestname");
        }

        [TestCleanup]
        public async Task DisposeAsync()
        {
            if (s_webApplicationFactory != null)
            {
                await s_webApplicationFactory.DisposeAsync();
            }
        }

        [TestMethod]
        [Timeout(60000)]
        public async Task GetAssessmentAsync_ExistingAssessment_AssessmentIsNotNull()
        {
            // Arrange.
            var visitorId = "123456789";
            await AddAssessmentInDatabaseAsync(visitorId);
            var httpClient = s_webApplicationFactory.CreateClient();

            // Act.
            var httpResponseMessage = await httpClient.GetAsync($"/api/assessments/visitor/{visitorId}");

            // Assert.
            _ = httpResponseMessage.EnsureSuccessStatusCode();
            var assessment = await httpResponseMessage.Content.ReadFromJsonAsync<AssessmentViewModel>();

            Assert.IsNotNull(assessment);
            Assert.AreEqual(visitorId, assessment.VisitorId);
        }

        /// <summary>
        /// Add an assessment in read-only database.
        /// </summary>
        /// <param name="visitorId"></param>
        /// <returns></returns>
        private static async Task AddAssessmentInDatabaseAsync(string visitorId)
        {
            using var scope = s_webApplicationFactory.Services.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<RatingContext>();
            _ = context.Assessments.Add(new Assessment(Guid.NewGuid(), visitorId, 3, true));
            _ = await context.SaveChangesAsync();
        }
    }
}
