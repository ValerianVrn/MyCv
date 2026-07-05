using System.Text.Json.Serialization;

namespace MyCv.UI.Wasm.Services
{
    /// <summary>
    /// Resullt of the Tailor service.
    /// </summary>
    public class TailorResult
    {
        [JsonPropertyName("case")]
        public int Case { get; set; }

        [JsonPropertyName("stars")]
        public int Stars { get; set; }

        [JsonPropertyName("matchLabel")]
        public string MatchLabel { get; set; } = "";

        [JsonPropertyName("humor")]
        public string Humor { get; set; } = "";

        [JsonPropertyName("whyMatch")]
        public List<string> WhyMatch { get; set; } = [];

        [JsonPropertyName("skillBridges")]
        public List<SkillBridge> SkillBridges { get; set; } = [];

        [JsonPropertyName("bonusSkills")]
        public List<string> BonusSkills { get; set; } = [];

        [JsonPropertyName("pitch")]
        public string Pitch { get; set; } = "";

        [JsonPropertyName("contactCopy")]
        public string ContactCopy { get; set; } = "";
    }
}
