using MyCv.Common.Domain;

namespace MyCv.Rating.Infrastructure.Idempotency
{
    /// <inheritdoc />
    /// <summary>
    /// Constructor.
    /// </summary>
    /// <param name="context"></param>
    /// <exception cref="ArgumentNullException"></exception>
    public class RequestManager(RatingContext context) : IRequestManager
    {
        /// <summary>
        /// Context.
        /// </summary>
        private readonly RatingContext _ratingContext = context ?? throw new ArgumentNullException(nameof(context));

        /// <inheritdoc />
        public async Task<bool> ExistAsync(Guid entityGuid)
        {
            var request = await _ratingContext.FindAsync<ClientRequest>(entityGuid);

            return request != null;
        }

        /// <inheritdoc />
        public async Task CreateRequestForCommandAsync<T>(Guid id)
        {
            var exists = await ExistAsync(id);

            var request = exists ? throw new DomainException($"Request with {id} already exists") : new ClientRequest()
            {
                Id = id,
                Name = typeof(T).Name,
                Time = DateTime.UtcNow
            };

            _ratingContext.Add(request);

            await _ratingContext.SaveChangesAsync();
        }
    }
}
