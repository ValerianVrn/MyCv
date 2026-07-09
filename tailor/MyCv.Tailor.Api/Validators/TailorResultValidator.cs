using FluentValidation;
using MyCv.Tailor.Api.Models;

namespace MyCv.Tailor.Api.Validators
{
    public class TailorResultValidator : AbstractValidator<TailorResult>
    {
        public TailorResultValidator()
        {
            // Required and strictly typed
            _ = RuleFor(x => x.Case).InclusiveBetween(1, 3);
            _ = RuleFor(x => x.Stars).InclusiveBetween(0, 5);
            _ = RuleFor(x => x.Pitch).NotEmpty();

            // Optional but not null
            _ = RuleFor(x => x.WhyMatch).NotNull();
            _ = RuleFor(x => x.BonusSkills).NotNull();
            _ = RuleFor(x => x.SkillBridges).NotNull();
            _ = RuleForEach(x => x.SkillBridges).ChildRules(b =>
            {
                _ = b.RuleFor(x => x.Asked).NotEmpty();
                _ = b.RuleFor(x => x.Have).NotEmpty();
            });

            // Business rules
            _ = RuleFor(x => x.Stars)
                .Must((r, stars) =>
                    (r.Case == 1 && stars == 0) ||
                    (r.Case == 2 && stars is >= 1 and <= 3) ||
                    (r.Case == 3 && stars is 4 or 5))
                .WithMessage("Stars inconsistent with Case");

            _ = RuleFor(x => x.WhyMatch)
                .Must(x => x.Count > 0)
                .When(x => x.Case is 2 or 3)
                .WithMessage("WhyMatch must not be empty for case 2 or 3");
        }
    }
}
