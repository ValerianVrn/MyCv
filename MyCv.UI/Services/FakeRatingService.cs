// Copyright (c) Fives Syleps. All rights reserved.
// See License.txt in the project root for license information.

using MyCv.UI.Dtos;

namespace MyCv.UI.Services
{
    /// <inheritdoc/>
    internal class FakeRatingService() : IRatingService
    {
        /// <inheritdoc/>
        public async Task<ResponseResult> PostRating(int rating, bool recommend)
        {
            if (rating is <= 0 or > 5)
            {
                return await Task.FromResult(ResponseResult.InvalidCommand());
            }
            if (rating >= 4 && !recommend)
            {
                return await Task.FromResult(ResponseResult.InvalidDomain());
            }

            return await Task.FromResult(ResponseResult.Success());
        }

        /// <inheritdoc/>
        public async Task<IEnumerable<Rating>> GetRatings()
        {
            return await Task.FromResult<IEnumerable<Rating>>([
                new(DateTime.Now, 4, true),
                new(DateTime.Now.AddHours(1), 2, false)
                ]);
        }
    }
}
