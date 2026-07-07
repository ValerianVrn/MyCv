using System.Text.Json.Serialization;

namespace MyCv.Tailor.Api.Models
{
    public record TailorResult(
        [property: JsonPropertyName("case")] int Case,
        [property: JsonPropertyName("stars")] int Stars,
        [property: JsonPropertyName("matchLabel")] string MatchLabel,
        [property: JsonPropertyName("humor")] string Humor,
        [property: JsonPropertyName("whyMatch")] List<string> WhyMatch,
        [property: JsonPropertyName("skillBridges")] List<SkillBridge> SkillBridges,
        [property: JsonPropertyName("bonusSkills")] List<string> BonusSkills,
        [property: JsonPropertyName("pitch")] string Pitch,
        [property: JsonPropertyName("contactCopy")] string ContactCopy
    );

    public record SkillBridge(
        [property: JsonPropertyName("asked")] string Asked,
        [property: JsonPropertyName("have")] string Have
    );
}
