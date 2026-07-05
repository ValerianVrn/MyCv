using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using System.Net;
using System.Net.Http.Json;
using System.Text.Json;

namespace MyCv.Tailor.Api
{
    public class TailorFunction(IHttpClientFactory httpClientFactory)
    {
        private const string GeminiUrl = "https://generativelanguage.googleapis.com/v1beta/models/gemini-1.5-flash:generateContent?key=";

        private const string SystemPrompt = """
        You are an AI assistant embedded in Valérian Verona's CV website.
        A recruiter has typed a job title, description or tech stack.
        Your job is to analyze how well Valérian matches and return a structured JSON response.

        Here is Valérian's full profile:

        ## Identity
        Valérian Verona, Tech Lead C#/.NET, based in Vannes, Bretagne, France.

        ## Core skills
        C#/.NET, Tech Lead, CQRS/ES, Microservices, System Design, Azure, ASP.NET Core,
        Entity Framework, SQL Server, MSTest, Event Sourcing, Docker, Azure DevOps,
        Git Flow, Grafana, Loki, Prometheus, Agile/Scrum, Code Reviews, Mentoring,
        OpenAI, Claude, Gemini, HuggingFace, Together AI, Blazor, SignalR

        ## Experience
        - Fives Xcella: Tech Lead C#/.NET, microservices, Azure, CQRS, Event Sourcing
        - Previous roles: various .NET development and architecture positions
        - 10+ years experience in .NET ecosystem

        ## Education
        - Engineering degree: Télécom Physique Strasbourg (Institut Mines-Télécom), 2014, ranked 3rd/79
        - Master of Science: Université de Strasbourg, 2014, with high honours
        - Master of Business: EM Strasbourg Business School, 2015, with high honours
        - Azure Fundamentals AZ-900, 2026

        ## Personality
        Fast learner, has pivoted tech stacks before, strong architectural thinking,
        both technical and business mindset (dual engineering + MBA background).

        ## Rules
        - case 1 = not relevant at all (e.g. plumber, chef, unrelated field)
        - case 2 = partial match (some skills missing but transferable)
        - case 3 = strong match (most skills align)
        - stars: 0 for case 1, 1-3 for case 2, 4-5 for case 3
        - humor must be warm, slightly self-deprecating, never arrogant
        - bonusSkills: skills Valérian has that the recruiter didn't mention but could be valuable
        - skillBridges: skills asked that Valérian doesn't have but has a close equivalent
        - answer in the same language as the input (French or English)
        - pitch must be 1-2 sentences max, punchy and specific
        - contactCopy: short line inviting contact, adapted to the case tone

        Return ONLY valid JSON, no markdown, no explanation:
        {
          "case": 1 | 2 | 3,
          "stars": 0-5,
          "matchLabel": "string",
          "humor": "string",
          "whyMatch": ["skill1", "skill2"],
          "skillBridges": [{ "asked": "string", "have": "string" }],
          "bonusSkills": ["skill1", "skill2"],
          "pitch": "string",
          "contactCopy": "string"
        }
        """;

        [Function("tailor")]
        public async Task<HttpResponseData> Run([HttpTrigger(AuthorizationLevel.Anonymous, "post", "options")] HttpRequestData req)
        {
            // Handle CORS preflight
            if (req.Method.Equals("OPTIONS", StringComparison.OrdinalIgnoreCase))
            {
                var preflight = req.CreateResponse(HttpStatusCode.OK);
                AddCorsHeaders(preflight);
                return preflight;
            }

            var apiKey = Environment.GetEnvironmentVariable("GEMINI_API_KEY");
            if (string.IsNullOrEmpty(apiKey))
            {
                var error = req.CreateResponse(HttpStatusCode.InternalServerError);
                AddCorsHeaders(error);
                await error.WriteStringAsync("API key not configured");
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
                system_instruction = new { parts = new[] { new { text = SystemPrompt } } },
                contents = new[]
                {
                new { role = "user", parts = new[] { new { text = body.Input } } }
            },
                generationConfig = new
                {
                    maxOutputTokens = 1024,
                    temperature = 0.7,
                    responseMimeType = "application/json"
                }
            };

            var httpClient = httpClientFactory.CreateClient();
            var geminiResponse = await httpClient.PostAsJsonAsync(GeminiUrl + apiKey, geminiRequest);
            var geminiJson = await geminiResponse.Content.ReadAsStringAsync();

            using var doc = JsonDocument.Parse(geminiJson);
            var text = doc.RootElement
                .GetProperty("candidates")[0]
                .GetProperty("content")
                .GetProperty("parts")[0]
                .GetProperty("text")
                .GetString() ?? "{}";

            var response = req.CreateResponse(HttpStatusCode.OK);
            AddCorsHeaders(response);
            response.Headers.Add("Content-Type", "application/json");
            await response.WriteStringAsync(text);
            return response;
        }

        private static void AddCorsHeaders(HttpResponseData response)
        {
            response.Headers.Add("Access-Control-Allow-Origin", "*");
            response.Headers.Add("Access-Control-Allow-Methods", "POST, OPTIONS");
            response.Headers.Add("Access-Control-Allow-Headers", "Content-Type");
        }
    }

    public record TailorRequest(string Input);
}
