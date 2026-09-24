window.initMapTooltips = function () {
  const tooltip = document.createElement('div');
  tooltip.className = 'map-tooltip';
  document.body.appendChild(tooltip);

  const hideTooltip = () => {
    tooltip.style.display = 'none';
  };

  document.querySelectorAll('.world-map path.visited').forEach(path => {
    path.addEventListener('mouseenter', e => {
      tooltip.textContent = path.getAttribute('title');
      tooltip.style.display = 'block';
    });
    path.addEventListener('mousemove', e => {
      tooltip.style.left = (e.clientX + 12) + 'px';
      tooltip.style.top = (e.clientY + 12) + 'px';
    });
    path.addEventListener('mouseleave', () => {
      hideTooltip();
    });
    path.addEventListener('touchstart', hideTooltip, { passive: true });
    path.addEventListener('touchmove', hideTooltip, { passive: true });
  });

  window.addEventListener('scroll', hideTooltip, { passive: true });
};
