using Microsoft.EntityFrameworkCore;
using MyCv.Rating.Application.ViewModels;
using MyCv.Rating.Infrastructure;

namespace MyCv.Rating.Application.Queries
{
    /// <inheritdoc/>
    /// <summary>
    /// Constructor.
    /// </summary>
    /// <param name="ratingContext"></param>
    public class AssessmentQueries(RatingContext ratingContext) : IAssessmentQueries
    {
        /// <inheritdoc />
        public async Task<AssessmentViewModel?> GetVisitorAssessmentAsync(string visitorId)
        {
            var assessment = await ratingContext.Assessments.FirstOrDefaultAsync(x => x.VisitorId == visitorId);

            return assessment is null ? null : new AssessmentViewModel(assessment.EntityGuid, assessment.VisitorId, assessment.Score, assessment.Recommend);
        }
    }
}
