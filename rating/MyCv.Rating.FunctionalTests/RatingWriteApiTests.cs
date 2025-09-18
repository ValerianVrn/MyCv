using Microsoft.Extensions.DependencyInjection;
using MyCv.Rating.Application.Commands;
using MyCv.Rating.Domain.AggregateModels.AssessmentAggregate;
using MyCv.Rating.Infrastructure;
using System.Net;
using System.Net.Http.Json;

namespace MyCv.Rating.FunctionalTests
{
    [TestClass]
    [TestCategory("Functional")]
    public class RatingWriteApiTests(TestContext testContext)
    {
        /// <summary>
        /// System under test (SUT).
        /// </summary>
        private static RatingWebApplicationFactory<Api.Write.Program> s_webApplicationFactory;

        /// <summary>
        /// Context of the test.
        /// </summary>
        private readonly TestContext _testContext = testContext;

        [TestInitialize]
        public async Task InitializeAsync()
        {
            s_webApplicationFactory = new RatingWebApplicationFactory<Api.Write.Program>();
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
        public async Task CreateAssessmentAsync_NewAssessment_ReturnsSuccess()
        {
            // Arrange.
            var createAssessmentCommand = new CreateAssessmentCommand("123456789", 3, true);
            var httpClient = s_webApplicationFactory.CreateClient();

            // Act.
            var httpResponseMessage = await httpClient.PostAsJsonAsync($"/api/assessments", createAssessmentCommand);

            // Assert.
            _ = httpResponseMessage.EnsureSuccessStatusCode();
        }

        [TestMethod]
        [Timeout(60000)]
        public async Task CreateAssessmentAsync_ExistingAssessment_ReturnsConflict()
        {
            // Arrange.
            var visitorId = "123456789";
            await AddAssessmentInDatabaseAsync(visitorId);
            var createAssessmentCommand = new CreateAssessmentCommand(visitorId, 3, true);
            var httpClient = s_webApplicationFactory.CreateClient();

            // Act.
            var httpResponseMessage = await httpClient.PostAsJsonAsync($"/api/assessments", createAssessmentCommand);

            // Assert.
            Assert.AreEqual(HttpStatusCode.Conflict, httpResponseMessage.StatusCode);
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
