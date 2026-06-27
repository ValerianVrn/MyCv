using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using System.Globalization;

namespace MyCv.UI.Wasm.Services
{
    /// <inheritdoc/>
    internal class LocalizationService(NavigationManager navigationManager, IJSRuntime jS) : ILocalizationService
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
        public async Task ApplySelectedCultureAsync()
        {
            if (CultureInfo.CurrentCulture != _currentCulture)
            {
                await jS.InvokeVoidAsync("blazorCulture.set", _currentCulture!.Name);

                _navigationManager.NavigateTo(_navigationManager.Uri, forceLoad: true);
            }
        }
    }
}
