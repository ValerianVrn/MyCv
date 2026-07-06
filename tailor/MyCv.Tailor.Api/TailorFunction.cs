using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.Logging;
using System.Net;
using System.Net.Http.Json;
using System.Text.Json;

namespace MyCv.Tailor.Api;

public class TailorFunction(IHttpClientFactory httpClientFactory, ILogger<TailorFunction> logger)
{
    private const string BaseUrl = "https://generativelanguage.googleapis.com/v1beta/models/";
    private const string Model = "gemini-2.5-flash";

    private const string SystemPrompt = """
    You are an AI assistant embedded in Valérian Verona's CV website.
    A recruiter has typed a job title, description or tech stack.
    Your job is to analyze how well Valérian matches and return a structured JSON response.

    ## Rules
    - case 1 = not relevant at all
    - case 2 = partial match
    - case 3 = strong match
    - stars: 0-5
    - return ONLY valid JSON
    """;

    public record TailorRequest(string Input);

    public record TailorResult(
        int Case,
        int Stars,
        string MatchLabel,
        string Humor,
        List<string> WhyMatch,
        List<SkillBridge> SkillBridges,
        List<string> BonusSkills,
        string Pitch,
        string ContactCopy
    );

    public record SkillBridge(
        string Asked,
        string Have
    );

    public record GeminiResponse(List<GeminiCandidate> Candidates);
    public record GeminiCandidate(GeminiContent Content);
    public record GeminiContent(List<GeminiPart> Parts);
    public record GeminiPart(string Text);

    [Function("tailor")]
    public async Task<HttpResponseData> Run(
        [HttpTrigger(AuthorizationLevel.Anonymous, "post", "options")] HttpRequestData req)
    {
        try
        {
            if (req.Method.Equals("OPTIONS", StringComparison.OrdinalIgnoreCase))
            {
                var preflight = req.CreateResponse(HttpStatusCode.OK);
                AddCorsHeaders(preflight);
                return preflight;
            }

            var apiKey = Environment.GetEnvironmentVariable("GEMINI_API_KEY");
            if (string.IsNullOrWhiteSpace(apiKey))
            {
                var error = req.CreateResponse(HttpStatusCode.InternalServerError);
                AddCorsHeaders(error);
                await error.WriteStringAsync("Missing GEMINI_API_KEY");
                return error;
            }

            var body = await req.ReadFromJsonAsync<TailorRequest>();
            if (body is null || string.IsNullOrWhiteSpace(body.Input))
            {
                var bad = req.CreateResponse(HttpStatusCode.BadRequest);
                AddCorsHeaders(bad);
                return bad;
            }

            var geminiRequest = new
            {
                system_instruction = new
                {
                    parts = new[] { new { text = SystemPrompt } }
                },
                contents = new[]
                {
                    new
                    {
                        role = "user",
                        parts = new[] { new { text = body.Input } }
                    }
                },
                generationConfig = new
                {
                    maxOutputTokens = 1024,
                    temperature = 0.7,
                    responseMimeType = "application/json"
                }
            };

            var httpClient = httpClientFactory.CreateClient();

            var geminiResponse = await httpClient.PostAsJsonAsync(
                $"{BaseUrl}{Model}:generateContent?key={apiKey}",
                geminiRequest);

            var geminiJson = await geminiResponse.Content.ReadAsStringAsync();

            logger.LogInformation("Gemini status: {Status}", geminiResponse.StatusCode);
            logger.LogDebug("Gemini raw response: {Response}", geminiJson);

            if (!geminiResponse.IsSuccessStatusCode)
            {
                var errorResponse = req.CreateResponse(geminiResponse.StatusCode);
                AddCorsHeaders(errorResponse);
                await errorResponse.WriteStringAsync(geminiJson);
                return errorResponse;
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
            if (text.StartsWith("```"))
            {
                text = text
                    .Replace("```json", "")
                    .Replace("```", "")
                    .Trim();
            }

            // Deserialize strongly typed result
            var result = JsonSerializer.Deserialize<TailorResult>(text) ?? throw new InvalidOperationException($"Invalid Tailor JSON: {text}");

            var response = req.CreateResponse(HttpStatusCode.OK);
            AddCorsHeaders(response);
            await response.WriteAsJsonAsync(result);
            return response;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Tailor function failed");

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
