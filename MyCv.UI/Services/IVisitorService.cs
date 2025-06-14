namespace MyCv.UI.Services
{
    /// <summary>
    /// Service for managing visitors.
    /// </summary>
    internal interface IVisitorService
    {
        /// <summary>
        /// Get the ID of the visitor.
        /// </summary
        /// <returns>IDs of the 3 first intents</returns>
        Task<string> GetVisitorId();
    }
}
