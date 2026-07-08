using System.Text.Json.Serialization;

namespace MyCv.UI.Wasm.Services
{
    /// <summary>
    /// Skill that don't match perfectly in the tailor result.
    /// </summary>
    public class SkillBridge
    {
        [JsonPropertyName("asked")]
        public string Asked { get; set; } = "";

        [JsonPropertyName("have")]
        public string Have { get; set; } = "";
    }
}
