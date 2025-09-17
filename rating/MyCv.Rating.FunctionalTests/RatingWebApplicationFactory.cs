using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MyCv.Common.HealthChecks.Healthchecks;
using Polly;
using Testcontainers.MsSql;

namespace MyCv.Rating.FunctionalTests
{
    internal class RatingWebApplicationFactory<T> : WebApplicationFactory<T> where T : class
    {
        /// <summary>
        /// SQL Server container.
        /// </summary>
        private static MsSqlContainer? s_msSqlContainer;

        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            // Generate configuration.
            var configuration = new ConfigurationBuilder()
                .AddInMemoryCollection(
                    new Dictionary<string, string?>
                    {
                        ["ConnectionStrings:SqlServerConnectionString"] = s_msSqlContainer?.GetConnectionString(),
                    })
                .Build();

            _ = builder.UseConfiguration(configuration);

            _ = builder.UseEnvironment("Development");
        }

        public async Task InitializeAsync(string testName)
        {
            // SQL Server container.
            s_msSqlContainer = new MsSqlBuilder()
                .WithName($"sqlserver-{testName}")
                .Build();

            await s_msSqlContainer.StartAsync();

            // And wait it is ready.
            var startupHelthcheck = Services.GetRequiredService<StartupHealthCheck>();
            _ = await Policy
                .HandleResult<bool>(r => !r)
                .WaitAndRetryAsync(10, _ => TimeSpan.FromMilliseconds(500))
                .ExecuteAsync(() =>
                {
                    return Task.FromResult(startupHelthcheck.StartupCompleted);
                });
        }

        public new async Task DisposeAsync()
        {
            await base.DisposeAsync();

            if (s_msSqlContainer is not null)
            {
                await s_msSqlContainer.DisposeAsync();
            }
        }
    }
}
