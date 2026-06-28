window.addEventListener('error', function () {
  document.getElementById('loader')?.remove();
  document.body.innerHTML = '<div style="height:100vh;display:flex;align-items:center;justify-content:center;background:#050810;color:white;font-family:system-ui;text-align:center"><div><p style="font-size:1.2rem">Something went wrong</p><a href="/" style="color:#5b8fff">Reload</a></div></div>';
});
