using System.Globalization;

namespace MyCv.UI.Wasm.Services
{
    /// <summary>
    /// Localization service.
    /// </summary>
    internal interface ILocalizationService
    {
        /// <summary>
        /// Change the culture.
        /// </summary>
        /// <param name="cultureInfo"></param>
        /// <returns></returns>
        void SetCulture(CultureInfo cultureInfo);

        /// <summary>
        /// Reload the page with the selected culture.
        /// </summary>
        /// <returns></returns>
        Task ApplySelectedCultureAsync();
    }
}
