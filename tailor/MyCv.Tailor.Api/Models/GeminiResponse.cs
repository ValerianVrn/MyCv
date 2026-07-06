namespace MyCv.Tailor.Api.Models;

public record GeminiResponse(List<GeminiCandidate> Candidates);
public record GeminiCandidate(GeminiContent Content);
public record GeminiContent(List<GeminiPart> Parts);
public record GeminiPart(string Text);
