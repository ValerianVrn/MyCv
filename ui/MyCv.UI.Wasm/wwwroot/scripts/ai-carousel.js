(function () {
  function initializeVideo(video) {
    if (video.dataset.visibilityInitialized === 'true') return;
    video.dataset.visibilityInitialized = 'true';

    var observer = new IntersectionObserver(function (entries) {
      entries.forEach(function (entry) {
        if (entry.isIntersecting) {
          video.play().catch(function () { });
        } else {
          video.pause();
        }
      });
    }, { threshold: 0.1 });

    observer.observe(video);
  }

  function initializeVideos() {
    document.querySelectorAll('.ai-carousel-video').forEach(initializeVideo);
  }

  initializeVideos();
  new MutationObserver(initializeVideos).observe(document.body, {
    childList: true,
    subtree: true
  });
})();