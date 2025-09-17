using MyCv.Rating.Application.ViewModels;

namespace MyCv.Rating.Application.Queries
{
    /// <summary>
    /// Queries for assessments.
    /// </summary>
    public interface IAssessmentQueries
    {
        /// <summary>
        /// Return the assessment of a visitor.
        /// </summary>
        /// <param name="visitorId"></param>
        /// <returns></returns>
        Task<AssessmentViewModel?> GetVisitorAssessmentAsync(string visitorId);
    }
}
