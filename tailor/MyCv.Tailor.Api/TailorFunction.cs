using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.Logging;
using MyCv.Tailor.Api.Exceptions;
using MyCv.Tailor.Api.Models;
using MyCv.Tailor.Api.Services;
using System.Net;
using System.Text.Json;

namespace MyCv.Tailor.Api
{
    internal class TailorFunction(ITailorService tailorService, IRateLimiter rateLimiter, ILogger<TailorFunction> logger)
    {
        /// <summary>
        /// Maximum accepted length of the recruiter input (a full job description fits easily).
        /// </summary>
        public const int MaxInputLength = 4000;

        /// <summary>
        /// App setting listing the allowed CORS origins, comma-separated.
        /// </summary>
        public const string AllowedOriginsSetting = "ALLOWED_ORIGINS";

        private const string DefaultAllowedOrigins = "https://www.valerian-verona.fr";

        [Function("tailor")]
        public async Task<HttpResponseData> Run([HttpTrigger(AuthorizationLevel.Anonymous, "post", "options")] HttpRequestData req)
        {
            try
            {
                if (req.Method.Equals("OPTIONS", StringComparison.OrdinalIgnoreCase))
                {
                    return CreateResponse(req, HttpStatusCode.OK);
                }

                if (!rateLimiter.TryAcquire(GetClientKey(req)))
                {
                    Log.RateLimited(logger);
                    return await CreateErrorAsync(req, HttpStatusCode.TooManyRequests, "rate_limited");
                }

                TailorRequest? body;
                try
                {
                    body = await req.ReadFromJsonAsync<TailorRequest>();
                }
                catch (JsonException)
                {
                    return await CreateErrorAsync(req, HttpStatusCode.BadRequest, "invalid_json");
                }

                if (body is null || string.IsNullOrWhiteSpace(body.Input))
                {
                    return await CreateErrorAsync(req, HttpStatusCode.BadRequest, "empty_input");
                }

                if (body.Input.Length > MaxInputLength)
                {
                    return await CreateErrorAsync(req, HttpStatusCode.BadRequest, "input_too_long");
                }

                var result = await tailorService.TailorAsync(body.Input);

                var response = CreateResponse(req, HttpStatusCode.OK);
                await response.WriteAsJsonAsync(result);
                return response;
            }
            catch (GeminiUnavailableException ex)
            {
                Log.Unavailable(logger, ex.StatusCode);
                return await CreateErrorAsync(req, HttpStatusCode.ServiceUnavailable, "gemini_unavailable");
            }
            catch (Exception ex)
            {
                Log.Error(logger, ex);
                var response = CreateResponse(req, HttpStatusCode.InternalServerError);

#if DEBUG
                await response.WriteStringAsync(ex.ToString());
#else
                await response.WriteStringAsync("Unexpected server error");
#endif

                return response;
            }
        }

        private static HttpResponseData CreateResponse(HttpRequestData req, HttpStatusCode statusCode)
        {
            var response = req.CreateResponse(statusCode);
            AddCorsHeaders(req, response);
            return response;
        }

        private static async Task<HttpResponseData> CreateErrorAsync(HttpRequestData req, HttpStatusCode statusCode, string error)
        {
            // Not WriteAsJsonAsync: it resets the status code to 200.
            var response = CreateResponse(req, statusCode);
            response.Headers.Add("Content-Type", "application/json; charset=utf-8");
            await response.WriteStringAsync(JsonSerializer.Serialize(new { error }));
            return response;
        }

        /// <summary>
        /// Echo the request origin only if it is allowed.
        /// Note: if CORS is also configured on the Function App (platform CORS), the platform takes precedence.
        /// </summary>
        private static void AddCorsHeaders(HttpRequestData req, HttpResponseData response)
        {
            if (!req.Headers.TryGetValues("Origin", out var origins))
            {
                return;
            }

            var origin = origins.FirstOrDefault();
            if (origin is null || !GetAllowedOrigins().Contains(origin, StringComparer.OrdinalIgnoreCase))
            {
                return;
            }

            response.Headers.Add("Access-Control-Allow-Origin", origin);
            response.Headers.Add("Vary", "Origin");
            response.Headers.Add("Access-Control-Allow-Methods", "POST, OPTIONS");
            response.Headers.Add("Access-Control-Allow-Headers", "Content-Type");
        }

        private static string[] GetAllowedOrigins()
        {
            var setting = Environment.GetEnvironmentVariable(AllowedOriginsSetting);
            return (string.IsNullOrWhiteSpace(setting) ? DefaultAllowedOrigins : setting)
                .Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
        }

        /// <summary>
        /// Client IP as seen by the Azure front end (first X-Forwarded-For entry, port stripped).
        /// </summary>
        internal static string GetClientKey(HttpRequestData req)
        {
            if (req.Headers.TryGetValues("X-Forwarded-For", out var values))
            {
                var first = values.FirstOrDefault()?.Split(',')[0].Trim();
                if (!string.IsNullOrEmpty(first))
                {
                    // IPv4 "1.2.3.4:5678" → "1.2.3.4" ; IPv6 "[::1]:5678" → "::1"
                    if (first.StartsWith('['))
                    {
                        var end = first.IndexOf(']', StringComparison.Ordinal);
                        return end > 0 ? first[1..end] : first;
                    }

                    var colon = first.IndexOf(':', StringComparison.Ordinal);
                    return colon > 0 && first.IndexOf(':', colon + 1) < 0 ? first[..colon] : first;
                }
            }

            return "unknown";
        }
    }

    public static partial class Log
    {
        [LoggerMessage(EventId = 1, Level = LogLevel.Warning, Message = "Gemini unavailable ({Status})")]
        public static partial void Unavailable(ILogger logger, HttpStatusCode? status);

        [LoggerMessage(EventId = 2, Level = LogLevel.Error, Message = "Tailor function failed")]
        public static partial void Error(ILogger logger, Exception ex);

        [LoggerMessage(EventId = 3, Level = LogLevel.Warning, Message = "Tailor request rate limited")]
        public static partial void RateLimited(ILogger logger);
    }
}
