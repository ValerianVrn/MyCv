namespace MyCv.UI.Wasm.Services
{
    /// <summary>
    /// Tailor service.
    /// </summary>
    internal interface ITailorService
    {
        /// <summary>
        /// Call the AI model to return what fits in the profile.
        /// </summary>
        /// <returns></returns>
        Task<TailorResult?> TailorAsync(string input, CancellationToken ct = default);

    }
}
