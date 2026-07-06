namespace MyCv.Tailor.Api.Models;

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
