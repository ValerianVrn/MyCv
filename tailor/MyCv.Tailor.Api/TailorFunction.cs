using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.Logging;
using MyCv.Tailor.Api.Models;
using MyCv.Tailor.Api.Services;
using System.Net;
using System.Net.Http.Json;

namespace MyCv.Tailor.Api
{
    internal class TailorFunction(ITailorService tailorService, ILogger<TailorFunction> logger)
    {
        [Function("tailor")]
        public async Task<HttpResponseData> Run([HttpTrigger(AuthorizationLevel.Anonymous, "post", "options")] HttpRequestData req)
        {
            try
            {
                if (req.Method.Equals("OPTIONS", StringComparison.OrdinalIgnoreCase))
                {
                    var preflight = req.CreateResponse(HttpStatusCode.OK);
                    AddCorsHeaders(preflight);
                    return preflight;
                }

                var body = await req.ReadFromJsonAsync<TailorRequest>();
                if (body is null || string.IsNullOrWhiteSpace(body.Input))
                {
                    var bad = req.CreateResponse(HttpStatusCode.BadRequest);
                    AddCorsHeaders(bad);
                    return bad;
                }

                var result = await tailorService.TailorAsync(body.Input);

                var response = req.CreateResponse(HttpStatusCode.OK);

                AddCorsHeaders(response);

                await response.WriteAsJsonAsync(result);

                return response;
            }
            catch (InvalidOperationException ex) when (ex.Message.Contains("503") || ex.Message.Contains("unavailable"))
            {
                Log.Unavailable(logger);
                var response = req.CreateResponse(HttpStatusCode.ServiceUnavailable);
                AddCorsHeaders(response);
                await response.WriteAsJsonAsync(new { error = "gemini_unavailable" });
                return response;
            }
            catch (Exception ex)
            {
                Log.Error(logger, ex);
                var response = req.CreateResponse(HttpStatusCode.InternalServerError);
                AddCorsHeaders(response);

#if DEBUG
                await response.WriteStringAsync(ex.ToString());
#else
                await response.WriteStringAsync("Unexpected server error");
#endif

                return response;
            }
        }

        private static void AddCorsHeaders(HttpResponseData response)
        {
            response.Headers.Add("Access-Control-Allow-Origin", "*");
            response.Headers.Add("Access-Control-Allow-Methods", "POST, OPTIONS");
            response.Headers.Add("Access-Control-Allow-Headers", "Content-Type");
        }
    }

    public static partial class Log
    {
        [LoggerMessage(EventId = 1, Level = LogLevel.Warning, Message = "Gemini unavailable")]
        public static partial void Unavailable(ILogger logger);

        [LoggerMessage(EventId = 2, Level = LogLevel.Error, Message = "Tailor function failed")]
        public static partial void Error(ILogger logger, Exception ex);
    }
}
