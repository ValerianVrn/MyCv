window.blazorCulture = {
  get: function () {
    try { return window.localStorage['BlazorCulture'] || null; } catch (e) { return null; }
  },
  set: function (value) {
    try { window.localStorage['BlazorCulture'] = value; } catch (e) { /* storage blocked */ }
  },
  // First visit: follow the browser language (French for fr-*, English otherwise).
  browserDefault: function () {
    var lang = (navigator.language || navigator.userLanguage || 'en').toLowerCase();
    return lang.startsWith('fr') ? 'fr-FR' : 'en-US';
  },
  current: function () {
    return window.blazorCulture.get() || window.blazorCulture.browserDefault();
  }
};

document.documentElement.lang = window.blazorCulture.current().split('-')[0];
