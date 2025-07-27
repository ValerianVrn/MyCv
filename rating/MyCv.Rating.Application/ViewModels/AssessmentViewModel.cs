using System.Diagnostics.CodeAnalysis;

namespace MyCv.Rating.Application.ViewModels
{
    /// <summary>
    /// View model of the assessment.
    /// </summary>
    [ExcludeFromCodeCoverage]
    public record AssessmentViewModel(Guid AssessmentGuid, string VisitorId, int Score, bool Recommend);
}
