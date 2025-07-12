using MyCv.Common.Domain.Cqrs;

namespace MyCv.Rating.Domain.AggregateModels.RatingAggregate
{
    /// <summary>
    /// Rating repository.
    /// </summary>
    public interface IRatingRepository : IRepository<Rating>
    {
        /// <summary>
        /// Get all ratings asynchronously.
        /// </summary>
        /// <returns></returns>
        Task<IEnumerable<Rating>> GetAllAsync();

        /// <summary>
        /// Create a rating.
        /// </summary>
        /// <param name="rating"></param>
        /// <returns></returns>
        Rating Create(Rating rating);
    }
}
