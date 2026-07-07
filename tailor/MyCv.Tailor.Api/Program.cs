using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using MyCv.Tailor.Api;
using MyCv.Tailor.Api.Services;

var builder = FunctionsApplication.CreateBuilder(args);

builder.Services
    .AddScoped<IGeminiClient, GeminiClient>()
    .AddScoped<ITailorService, TailorService>();
builder.ConfigureFunctionsWebApplication();
builder.Services
    .AddHttpClient(GeminiClient.HttpClientName)
    .AddPolicyHandler(PollyPolicies.RetryPolicy());
builder.Services.AddApplicationInsightsTelemetryWorkerService()
    .ConfigureFunctionsApplicationInsights();

builder.Build().Run();
