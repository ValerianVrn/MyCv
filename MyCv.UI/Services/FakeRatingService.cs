using MyCv.UI.Dtos;

namespace MyCv.UI.Services
{
    /// <inheritdoc/>
    internal class FakeRatingService : IRatingService
    {
        /// <summary>
        /// Per scope (visitor's page) in-memory ratings.
        /// </summary>
        private readonly List<Rating> _ratings = [];

        /// <inheritdoc/>
        public async Task<ResponseResult> PostRating(string visitorId, int rating, bool recommend)
        {
            // Invalid command.
            if (rating is <= 0 or > 5)
            {
                return await Task.FromResult(ResponseResult.InvalidCommand());
            }

            // Invalid domain rule.
            if (rating >= 4 && !recommend)
            {
                return await Task.FromResult(ResponseResult.InvalidDomain());
            }

            // Store the rating.
            _ratings.Add(new(DateTime.Now, rating, recommend));

            return await Task.FromResult(ResponseResult.Success());
        }

        /// <inheritdoc/>
        public async Task<IEnumerable<Rating>> GetRatings()
        {
            return await Task.FromResult<IEnumerable<Rating>>(_ratings);
        }
    }
}
