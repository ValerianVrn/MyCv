// Fallback screen when the app fails to START (e.g. WASM download/boot failure).
// Once Blazor has rendered (the #loader element is gone), runtime errors are left to
// Blazor's own #blazor-error-ui, and errors from other origins (extensions, CDNs) are ignored.
window.addEventListener('error', function (event) {
  if (!document.getElementById('loader')) return;
  if (event.filename && !event.filename.startsWith(window.location.origin)) return;

  var fr = (window.blazorCulture ? window.blazorCulture.current() : navigator.language || '').toLowerCase().startsWith('fr');
  document.getElementById('loader').remove();
  document.body.innerHTML = '<div style="height:100vh;display:flex;align-items:center;justify-content:center;background:#050810;color:white;font-family:system-ui;text-align:center"><div><p style="font-size:1.2rem">'
    + (fr ? 'Une erreur est survenue' : 'Something went wrong')
    + '</p><a href="/" style="color:#5b8fff">' + (fr ? 'Recharger' : 'Reload') + '</a></div></div>';
});
