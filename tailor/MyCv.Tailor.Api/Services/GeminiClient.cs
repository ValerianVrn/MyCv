using FluentValidation;
using Microsoft.Extensions.Logging;
using MyCv.Tailor.Api.Exceptions;
using MyCv.Tailor.Api.Models;
using MyCv.Tailor.Api.Prompt;
using MyCv.Tailor.Api.Validators;
using System.Net;
using System.Net.Http.Json;
using System.Text.Json;

namespace MyCv.Tailor.Api.Services
{
    public class GeminiClient(ILogger<GeminiClient> logger, IHttpClientFactory httpClientFactory) : IGeminiClient
    {
        private const string BaseUrl = "https://generativelanguage.googleapis.com/v1beta/models/";
        public const string GEMINIAPIKEY = "GEMINI_API_KEY";

        /// <summary>
        /// Optional app setting to change the Gemini model without redeploying.
        /// </summary>
        public const string GEMINIMODEL = "GEMINI_MODEL";

        /// <summary>
        /// Model used when <see cref="GEMINIMODEL"/> is not set.
        /// </summary>
        public const string DefaultModel = "gemini-2.5-flash";
        public const string HttpClientName = "gemini";

        public async Task<TailorResult> GenerateAsync(string input)
        {
            var geminiRequest = new
            {
                system_instruction = new
                {
                    parts = new[] { new { text = TailorPrompt.SystemPrompt } }
                },
                contents = new[]
                {
                        new
                        {
                            role = "user",
                            parts = new[] { new { text = input } }
                        }
                    },
                generationConfig = new
                {
                    maxOutputTokens = 2048,
                    temperature = 0.7,
                    responseMimeType = "application/json",
                    thinkingConfig = new
                    {
                        thinkingBudget = 512
                    }
                }
            };

            var apiKey = Environment.GetEnvironmentVariable(GEMINIAPIKEY);

            if (string.IsNullOrWhiteSpace(apiKey))
            {
                throw new InvalidOperationException($"Missing {GEMINIAPIKEY}");
            }

            var httpClient = httpClientFactory.CreateClient(HttpClientName);

            // API key sent as header (not query string) so it never appears in logged URLs.
            var model = Environment.GetEnvironmentVariable(GEMINIMODEL);
            if (string.IsNullOrWhiteSpace(model))
            {
                model = DefaultModel;
            }

            using var request = new HttpRequestMessage(HttpMethod.Post, $"{BaseUrl}{model.Trim()}:generateContent")
            {
                Content = JsonContent.Create(geminiRequest)
            };
            request.Headers.Add("x-goog-api-key", apiKey);

            var geminiResponse = await httpClient.SendAsync(request);
            Log.StatusCode(logger, geminiResponse.StatusCode);

            // Force buffer the entire response before reading
            await geminiResponse.Content.LoadIntoBufferAsync();

            var geminiJson = await geminiResponse.Content.ReadAsStringAsync();
            Log.Response(logger, geminiJson);
            Log.RawLength(logger, geminiJson.Length);

            if (geminiResponse.StatusCode is HttpStatusCode.ServiceUnavailable or HttpStatusCode.TooManyRequests)
            {
                throw new GeminiUnavailableException(geminiResponse.StatusCode);
            }

            if (!geminiResponse.IsSuccessStatusCode)
            {
                throw new InvalidOperationException($"Gemini response code : {geminiResponse.StatusCode}");
            }

            // Parse Gemini response safely
            var gemini = JsonSerializer.Deserialize<GeminiResponse>(geminiJson);

            if (gemini?.Candidates == null || gemini.Candidates.Count == 0)
            {
                throw new InvalidOperationException($"No candidates returned: {geminiJson}");
            }

            var text = gemini?.Candidates?
                .FirstOrDefault()?
                .Content?
                .Parts?
                .FirstOrDefault()?
                .Text;

            if (string.IsNullOrWhiteSpace(text))
            {
                throw new InvalidOperationException("Gemini returned empty content");
            }

            // Clean markdown fences if any
            text = text.Trim();
            if (text.StartsWith("```", StringComparison.InvariantCulture))
            {
                text = text
                    .Replace("```json", "")
                    .Replace("```", "")
                    .Trim();
            }

            // Deserialize strongly typed result
            var result = JsonSerializer.Deserialize<TailorResult>(text) ?? throw new InvalidOperationException($"Invalid Tailor JSON: {text}");

            new TailorResultValidator().ValidateAndThrow(result);

            return result;
        }
    }

    public static partial class Log
    {
        [LoggerMessage(EventId = 1, Level = LogLevel.Information, Message = "Gemini status: {Status}")]
        public static partial void StatusCode(ILogger logger, HttpStatusCode status);

        [LoggerMessage(EventId = 2, Level = LogLevel.Debug, Message = "Gemini raw response: {Response}")]
        public static partial void Response(ILogger logger, string? response);

        [LoggerMessage(EventId = 3, Level = LogLevel.Debug, Message = "Gemini raw response length: {Length}")]
        public static partial void RawLength(ILogger logger, int length);
    }
}
