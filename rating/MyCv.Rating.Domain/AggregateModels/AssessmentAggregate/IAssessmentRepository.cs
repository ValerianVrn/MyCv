using MyCv.Common.Domain.Cqrs;

namespace MyCv.Rating.Domain.AggregateModels.AssessmentAggregate
{
    /// <summary>
    /// Assessment repository.
    /// </summary>
    public interface IAssessmentRepository : IRepository<Assessment>
    {
        /// <summary>
        /// Get all assessments asynchronously.
        /// </summary>
        /// <returns></returns>
        Task<IEnumerable<Assessment>> GetAllAsync(CancellationToken cancellationToken);

        /// <summary>
        /// Get the assessment of a visitor asynchronously.
        /// </summary>
        /// <returns></returns>
        Task<Assessment?> GetAsync(string visitorId, CancellationToken cancellationToken);

        /// <summary>
        /// Create an assessment.
        /// </summary>
        /// <param name="assessment"></param>
        /// <returns></returns>
        Assessment Create(Assessment assessment);
    }
}
