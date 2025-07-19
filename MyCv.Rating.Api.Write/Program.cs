using HealthChecks.UI.Client;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using MyCv.Common.HealthChecks.Healthchecks;
using MyCv.Rating.Api.Write.Services;
using MyCv.Rating.Application.Extensions;
using MyCv.Rating.Infrastructure;
using MyCv.Rating.Infrastructure.Extensions;
using Prometheus;
using Serilog;
using System.Globalization;

var builder = WebApplication.CreateBuilder(new WebApplicationOptions
{
    Args = args,
    ApplicationName = typeof(Program).Assembly.FullName,
    ContentRootPath = Directory.GetCurrentDirectory()
});

// Logging.
Log.Logger = new LoggerConfiguration()
    .WriteTo.Console(formatProvider: CultureInfo.InvariantCulture)
    .WriteTo.File("log/info-.log",
        rollingInterval: RollingInterval.Day,
        outputTemplate: "{Timestamp:yyyy-MM-dd HH:mm:ss.fff} | {Level,-11} | {SourceContext,-70} | {Message}{NewLine}{Exception}",
        formatProvider: CultureInfo.InvariantCulture)
    .CreateLogger();
builder.Host.UseSerilog();

// Configuration.
builder.Configuration
    .SetBasePath(Directory.GetCurrentDirectory())
    .AddEnvironmentVariables();
var sqlServerConnectionString = builder.Configuration.GetConnectionString("SqlServerConnectionString");

// API.
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Healthchecks.
_ = builder.Services
    .AddSingleton<StartupHealthCheck>()
    .AddHealthChecks()
    .AddCheck("default_check", () => HealthCheckResult.Healthy(), tags: [LiveHealthCheck.Tag])
    .AddCheck<LiveHealthCheck>(LiveHealthCheck.Tag, tags: [LiveHealthCheck.Tag])
    .AddCheck<ReadyHealthCheck>(ReadyHealthCheck.Tag, tags: [ReadyHealthCheck.Tag])
    .AddCheck<StartupHealthCheck>("startup", tags: [ReadyHealthCheck.Tag])
    .AddDbContextCheck<RatingContext>(tags: [LiveHealthCheck.Tag])
    .ForwardToPrometheus();

// Persistence.
builder.Services
    .AddDbContext<RatingContext>(options =>
    {
        _ = options.UseSqlServer(sqlServerConnectionString, x =>
        {
            _ = x.EnableRetryOnFailure(maxRetryCount: 15, maxRetryDelay: TimeSpan.FromSeconds(30), errorNumbersToAdd: null);
        });
    });

// Core services.
builder.Services
    .AddMediator()
    .AddRepositories()
    .AddHostedService<StartupBackgroundService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    _ = app.UseSwagger();
    _ = app.UseSwaggerUI();
}

app.UseHttpsRedirection()
    .UseRouting()
    .UseHttpMetrics()
    .UseAuthentication()
    .UseAuthorization()
    .UseMetricServer();
app.MapMetrics();
app.MapControllers();

app.MapHealthChecks($"/healthz/{LiveHealthCheck.Tag}", new HealthCheckOptions
{
    Predicate = healthCheck => healthCheck.Tags.Contains(LiveHealthCheck.Tag),
    ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse
});
app.MapHealthChecks($"/healthz/{ReadyHealthCheck.Tag}", new HealthCheckOptions
{
    Predicate = healthCheck => healthCheck.Tags.Contains(ReadyHealthCheck.Tag),
    ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse
});

try
{
    Log.Information("Starting web host...");

    await app.RunAsync();

    return 0;
}
catch (Exception ex)
{
    Log.Fatal(ex, "Program terminated unexpectedly!");
    return 1;
}
finally
{
    Log.CloseAndFlush();
}

namespace MyCv.Rating.Api.Write
{
    internal partial class Program
    {

    }
}
