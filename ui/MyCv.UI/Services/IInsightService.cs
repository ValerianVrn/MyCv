using MyCv.UI.Dtos;

namespace MyCv.UI.Services
{
    /// <summary>
    /// Service for interactions with Insight service.
    /// </summary>
    internal interface IInsightService
    {
        /// <summary>
        /// Add an intent.
        /// </summary>
        /// <param name="visitorId"></param>
        /// <param name="intentId"></param>
        /// <param name="priority"></param>
        /// <returns></returns>
        Task<ResponseResult> AddIntent(string visitorId, string intentId, int priority);

        /// <summary>
        /// Remove an intent.
        /// </summary>
        /// <param name="visitorId"></param>
        /// <param name="intentId"></param>
        /// <returns></returns>
        Task<ResponseResult> RemoveIntent(string visitorId, string intentId);

        /// <summary>
        /// Change the priority of an intent.
        /// </summary>
        /// <param name="visitorId"></param>
        /// <param name="intentId"></param>
        /// <param name="newPriority"></param>
        /// <returns></returns>
        Task<ResponseResult> ChangePriority(string visitorId, string intentId, int newPriority);

        /// <summary>
        /// Get the intents of a visitor.
        /// </summary>
        /// <param name="visitorId"></param>
        /// <returns></returns>
        Task<IEnumerable<DomainEvent>> GetDomainEvents(string visitorId);

        /// <summary>
        /// Get the top 3 intents of a visitor.
        /// </summary>
        /// <param name="visitorId"></param>
        /// <returns>IDs of the 3 first intents</returns>
        Task<IDictionary<int, string>> GetPodium(string visitorId);
    }
}
