// Copyright (c) Fives Syleps. All rights reserved.
// See License.txt in the project root for license information.

using MyCv.UI.Dtos;

namespace MyCv.UI.Services
{
    /// <inheritdoc/>
    internal class RatingService(ILogger<RatingService> logger, IHttpClientFactory clientFactory) : IRatingService
    {
        /// <summary>
        /// Logger.
        /// </summary>
        private readonly ILogger<RatingService> _logger = logger ?? throw new ArgumentNullException(nameof(logger));

        /// <summary>
        /// Http client
        /// </summary>
        private readonly HttpClient _httpClient = clientFactory.CreateClient("APIGateway");

        /// <inheritdoc/>
        public async Task<ResponseResult> PostRating(int rating, bool recommend)
        {
            await _httpClient.PostAsync("api/stopsystem", null);

            return ResponseResult.Success();
        }

        /// <inheritdoc/>
        public async Task<IEnumerable<Rating>> GetRatings()
        {
            return null;
        }
    }
}
