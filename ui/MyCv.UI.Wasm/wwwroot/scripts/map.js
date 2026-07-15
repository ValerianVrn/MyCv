window.initMapTooltips = function () {
  const tooltip = document.createElement('div');
  tooltip.className = 'map-tooltip';
  document.body.appendChild(tooltip);

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
      tooltip.style.display = 'none';
    });
  });
};
