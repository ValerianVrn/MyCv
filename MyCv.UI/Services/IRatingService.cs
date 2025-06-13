using MyCv.UI.Dtos;

namespace MyCv.UI.Services
{
    /// <summary>
    /// Service for interactions with Rating service.
    /// </summary>
    internal interface IRatingService
    {
        /// <summary>
        /// Post a rating.
        /// </summary>
        /// <param name="rating"></param>
        /// <param name="recommend"></param>
        /// <returns></returns>
        Task<ResponseResult> PostRating(int rating, bool recommend);

        /// <summary>
        /// Get the ratings.
        /// </summary>
        /// <param name="rating"></param>
        /// <param name="recommend"></param>
        /// <returns></returns>
        Task<IEnumerable<Rating>> GetRatings();
    }
}
