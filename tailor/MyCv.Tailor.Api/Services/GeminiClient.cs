using FluentValidation;
using Microsoft.Extensions.Logging;
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
        private const string Model = "gemini-2.5-flash";
        public const string GEMINIAPIKEY = "GEMINI_API_KEY";

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
                    maxOutputTokens = 1024,
                    temperature = 0.7,
                    responseMimeType = "application/json"
                }
            };

            var apiKey = Environment.GetEnvironmentVariable(GEMINIAPIKEY);

            if (string.IsNullOrWhiteSpace(apiKey))
            {
                throw new InvalidOperationException($"Missing {GEMINIAPIKEY}");
            }

            var httpClient = httpClientFactory.CreateClient();
            var geminiResponse = await httpClient.PostAsJsonAsync($"{BaseUrl}{Model}:generateContent?key={apiKey}", geminiRequest);
            var geminiJson = await geminiResponse.Content.ReadAsStringAsync();

            Log.StatusCode(logger, geminiResponse.StatusCode);
            Log.Response(logger, geminiJson);

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
        [LoggerMessage(Level = LogLevel.Information, Message = "Gemini status: {Status}")]
        public static partial void StatusCode(ILogger logger, HttpStatusCode status);

        [LoggerMessage(Level = LogLevel.Debug, Message = "Gemini raw response: {Response}")]
        public static partial void Response(ILogger logger, string? response);
    }
}
