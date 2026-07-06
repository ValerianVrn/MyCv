using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using MyCv.Tailor.Api.Services;

var builder = FunctionsApplication.CreateBuilder(args);

builder.Services
    .AddScoped<IGeminiClient, GeminiClient>()
    .AddScoped<ITailorService, TailorService>();

builder.ConfigureFunctionsWebApplication();
builder.Services
    .AddHttpClient()
    .AddApplicationInsightsTelemetryWorkerService()
    .ConfigureFunctionsApplicationInsights();

builder.Build().Run();
