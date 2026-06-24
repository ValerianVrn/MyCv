namespace MyCv.UI.Services
{
    /// <summary>
    /// Service for holding current theme.
    /// </summary>
    internal interface IThemeService
    {
        /// <summary>
        /// Event to notify components of theme changes.
        /// </summary>
        event Action OnThemeChanged;

        /// <summary>
        /// Returns true is the theme is dark.
        /// </summary>
        bool IsDarkMode { get; }

        /// <summary>
        /// Set the dark theme.
        /// </summary>
        void SetDarkMode();

        /// <summary>
        /// Set the light theme.
        /// </summary>
        void SetLightMode();

        /// <summary>
        /// Switch between light and dark modes.
        /// </summary>
        void ToggleMode();
    }
}
