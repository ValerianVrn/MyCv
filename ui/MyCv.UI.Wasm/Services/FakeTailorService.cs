namespace MyCv.UI.Wasm.Services
{
    /// <inheritdoc/>
    internal class FakeTailorService : ITailorService
    {
        public async Task<TailorResult?> TailorAsync(string input, CancellationToken ct = default)
        {
            await Task.Delay(1500, ct); // simulate network latency

            if (input.Contains("plumb", StringComparison.OrdinalIgnoreCase))
                return new TailorResult
                {
                    Case = 1,
                    Stars = 0,
                    MatchLabel = "Not a match",
                    Humor = "Honestly? Pipes aren't really Valérian's thing — unless it's data pipelines.",
                    Pitch = "This one's outside his territory.",
                    ContactCopy = "Curiosity beats a perfect CV sometimes."
                };

            if (input.Contains("aws", StringComparison.OrdinalIgnoreCase))
                return new TailorResult
                {
                    Case = 2,
                    Stars = 3,
                    MatchLabel = "Partial match",
                    Humor = "Not a perfect match on paper — but Valérian has pivoted tech stacks before.",
                    WhyMatch = ["Cloud architecture", "Tech Lead", "Microservices"],
                    SkillBridges = [new() { Asked = "AWS Lambda", Have = "Azure Functions" }],
                    BonusSkills = ["Event Sourcing", "AI integration"],
                    Pitch = "Different cloud, same architecture principles.",
                    ContactCopy = "Worth a conversation?"
                };

            return new TailorResult
            {
                Case = 3,
                Stars = 5,
                MatchLabel = "Strong match",
                Humor = "Congratulations — Valérian is pretty much your guy. (I don't say that every time, I promise.)",
                WhyMatch = ["Azure", "C#/.NET", "Microservices", "Tech Lead", "CQRS"],
                BonusSkills = ["Event Sourcing", "Mentoring", "AI integration"],
                Pitch = "10+ years building distributed systems on Azure. Led teams up to 8 engineers.",
                ContactCopy = "Convinced? Let's talk."
            };
        }
    }

}
