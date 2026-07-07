using System.Text.Json.Serialization;

namespace MyCv.Tailor.Api.Models
{
    public record GeminiResponse([property: JsonPropertyName("candidates")] List<GeminiCandidate> Candidates);
    public record GeminiCandidate([property: JsonPropertyName("content")] GeminiContent Content);
    public record GeminiContent([property: JsonPropertyName("parts")] List<GeminiPart> Parts);
    public record GeminiPart([property: JsonPropertyName("text")] string Text);
}
