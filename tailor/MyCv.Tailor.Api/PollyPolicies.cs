using Polly;
using Polly.Extensions.Http;

namespace MyCv.Tailor.Api
{
    internal static class PollyPolicies
    {
        public static IAsyncPolicy<HttpResponseMessage> RetryPolicy()
        {
            return HttpPolicyExtensions
                .HandleTransientHttpError()                          // 5xx + network errors
                .OrResult(r => (int)r.StatusCode == 503)
                .OrResult(r => (int)r.StatusCode == 429)
                .WaitAndRetryAsync(
                    retryCount: 3,
                    sleepDurationProvider: attempt => TimeSpan.FromSeconds(Math.Pow(2, attempt)), // 2s, 4s, 8s
                    onRetry: (outcome, delay, attempt, _) =>
                        Console.WriteLine($"Gemini retry {attempt} after {delay.TotalSeconds}s — {outcome.Result?.StatusCode}"));
        }
    }
}
