using MediatR;
using MyCv.Rating.Application.ResultHandling;

namespace MyCv.Rating.Application.Commands
{
    /// <summary>
    /// Command used for creating an assessment.
    /// </summary>
    /// <param name="VisitorId"></param>
    /// <param name="Score"></param>
    /// <param name="Recommend"></param>
    public record CreateAssessmentCommand(string VisitorId, int Score, bool Recommend) : IRequest<Result>;
}
