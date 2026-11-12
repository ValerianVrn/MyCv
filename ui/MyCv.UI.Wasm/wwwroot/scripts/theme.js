// Visitor's theme choice ("light" | "dark"), remembered across visits. null = never chosen (dark by default).
window.themePreference = {
  get: function () {
    try { return window.localStorage['ThemePreference'] || null; } catch (e) { return null; }
  },
  set: function (value) {
    try { window.localStorage['ThemePreference'] = value; } catch (e) { /* storage blocked */ }
  }
};
