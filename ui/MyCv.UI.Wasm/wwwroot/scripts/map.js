// Loads the world map SVG (static file, ~1.2 MB) only when its container gets close to the viewport,
// then wires the tooltips on visited countries.
window.loadWorldMap = function (container, url) {
  if (!container || container.dataset.mapLoaded) return;
  container.dataset.mapLoaded = 'pending';

  function load() {
    fetch(url)
      .then(function (response) {
        if (!response.ok) throw new Error('World map: HTTP ' + response.status);
        return response.text();
      })
      .then(function (text) {
        var doc = new DOMParser().parseFromString(text, 'image/svg+xml');
        var svg = doc.documentElement;
        if (!svg || svg.nodeName.toLowerCase() !== 'svg') throw new Error('World map: invalid SVG');
        container.appendChild(document.importNode(svg, true));
        container.dataset.mapLoaded = 'true';
        window.initMapTooltips(container);
      })
      .catch(function (error) {
        delete container.dataset.mapLoaded;
        console.warn(error);
      });
  }

  if (!('IntersectionObserver' in window)) {
    load();
    return;
  }

  var observer = new IntersectionObserver(function (entries) {
    if (entries.some(function (e) { return e.isIntersecting; })) {
      observer.disconnect();
      load();
    }
  }, { rootMargin: '600px 0px' });
  observer.observe(container);
};

window.initMapTooltips = function (root) {
  var tooltip = document.querySelector('.map-tooltip');
  if (!tooltip) {
    tooltip = document.createElement('div');
    tooltip.className = 'map-tooltip';
    document.body.appendChild(tooltip);
    window.addEventListener('scroll', hideTooltip, { passive: true });
  }

  function hideTooltip() {
    tooltip.style.display = 'none';
  }

  (root || document).querySelectorAll('.world-map path.visited').forEach(function (path) {
    path.addEventListener('mouseenter', function () {
      tooltip.textContent = path.getAttribute('title');
      tooltip.style.display = 'block';
    });
    path.addEventListener('mousemove', function (e) {
      tooltip.style.left = (e.clientX + 12) + 'px';
      tooltip.style.top = (e.clientY + 12) + 'px';
    });
    path.addEventListener('mouseleave', hideTooltip);
    path.addEventListener('touchstart', hideTooltip, { passive: true });
    path.addEventListener('touchmove', hideTooltip, { passive: true });
  });
};
