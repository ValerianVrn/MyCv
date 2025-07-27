using FluentValidation;
using MyCv.Rating.Application.Commands;
using MyCv.Rating.Application.Extensions;

namespace MyCv.Rating.Application.Validators.Commands
{
    /// <summary>
    /// Validator of CreateAssessmentCommand
    /// </summary>
    internal class CreateAssessmentCommandValidator : AbstractValidator<CreateAssessmentCommand>
    {
        /// <summary>
        /// Constructor
        /// </summary>
        public CreateAssessmentCommandValidator()
        {
            _ = RuleFor(command => command.VisitorId).MustNotBeNullNorEmpty();
            _ = RuleFor(command => command.Score).MustBeBetween(1, 5);
        }
    }
}
