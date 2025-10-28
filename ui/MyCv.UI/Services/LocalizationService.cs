using Microsoft.AspNetCore.Components;
using System.Globalization;

namespace MyCv.UI.Services
{
    /// <inheritdoc/>
    internal class LocalizationService(NavigationManager navigationManager) : ILocalizationService
    {
        public static readonly CultureInfo English = new("en-US");
        public static readonly CultureInfo French = new("fr-FR");
        public static readonly CultureInfo Japanese = new("ja");

        /// <summary>
        /// Navigation manager.
        /// </summary>
        private readonly NavigationManager _navigationManager = navigationManager;

        /// <summary>
        /// Current culture.
        /// </summary>
        private CultureInfo? _currentCulture;

        /// <inheritdoc/>
        public void SetCulture(CultureInfo cultureInfo)
        {
            _currentCulture = cultureInfo;
        }

        /// <inheritdoc/>
        public void ApplySelectedCulture()
        {
            if (CultureInfo.CurrentCulture != _currentCulture)
            {
                var uri = new Uri(_navigationManager.Uri).GetComponents(UriComponents.PathAndQuery, UriFormat.Unescaped);
                var cultureEscaped = Uri.EscapeDataString(_currentCulture?.Name ?? English.Name);
                var uriEscaped = Uri.EscapeDataString(uri);

                _navigationManager.NavigateTo($"Culture/Set?culture={cultureEscaped}&redirectUri={uriEscaped}", forceLoad: true);
            }
        }
    }
}
