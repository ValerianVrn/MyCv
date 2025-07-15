namespace MyCv.Rating.Infrastructure.Idempotency
{
    /// <summary>
    /// Manager for requests from commands.
    /// </summary>
    public interface IRequestManager
    {
        /// <summary>
        /// Check if the GUID of the request already exists.
        /// </summary>
        /// <param name="entityGuid"></param>
        /// <returns></returns>
        Task<bool> ExistAsync(Guid entityGuid);

        /// <summary>
        /// Create a request for a command asynchronously.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="id"></param>
        /// <returns></returns>
        Task CreateRequestForCommandAsync<T>(Guid id);
    }
}
