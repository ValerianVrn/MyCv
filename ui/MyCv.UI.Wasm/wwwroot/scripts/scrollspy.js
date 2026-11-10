// Activates the content menu items while scrolling. Idempotent: safe to call after every render.
window.refreshScrollSpy = function () {
  var element = document.querySelector('[data-bs-spy="scroll"]');
  if (!element || !window.bootstrap) return;
  window.bootstrap.ScrollSpy.getOrCreateInstance(element).refresh();
};
