(function () {
  function initializeMarquee(marquee) {
    if (marquee.dataset.loopInitialized === 'true') return;
    marquee.dataset.loopInitialized = 'true';

    var dragging = false;
    var startX = 0;
    var startScrollLeft = 0;

    function copyWidth() {
      return marquee.scrollWidth / 2;
    }

    function wrapScrollPosition() {
      var width = copyWidth();
      if (width <= 0) return;

      while (marquee.scrollLeft >= width) {
        marquee.scrollLeft -= width;
      }
      while (marquee.scrollLeft <= 0) {
        marquee.scrollLeft += width;
      }
    }

    function moveWhileDragging(clientX) {
      var width = copyWidth();
      if (width <= 0) return;

      var nextScrollLeft = startScrollLeft - (clientX - startX);
      while (nextScrollLeft <= 0) {
        startScrollLeft += width;
        nextScrollLeft += width;
      }
      while (nextScrollLeft >= width) {
        startScrollLeft -= width;
        nextScrollLeft -= width;
      }
      marquee.scrollLeft = nextScrollLeft;
    }

    marquee.addEventListener('scroll', function () {
      wrapScrollPosition();
    }, { passive: true });

    marquee.addEventListener('pointerdown', function (event) {
      if (event.pointerType !== 'mouse') return;
      dragging = true;
      startX = event.clientX;
      startScrollLeft = marquee.scrollLeft;
      marquee.classList.add('is-dragging');
      marquee.setPointerCapture(event.pointerId);
    });

    marquee.addEventListener('pointermove', function (event) {
      if (!dragging) return;
      event.preventDefault();
      moveWhileDragging(event.clientX);
    });

    function stopDragging(event) {
      if (!dragging) return;
      dragging = false;
      marquee.classList.remove('is-dragging');
      if (event && marquee.hasPointerCapture(event.pointerId)) {
        marquee.releasePointerCapture(event.pointerId);
      }
    }

    marquee.addEventListener('pointerup', stopDragging);
    marquee.addEventListener('pointercancel', stopDragging);

    marquee.addEventListener('mousedown', function (event) {
      dragging = true;
      startX = event.clientX;
      startScrollLeft = marquee.scrollLeft;
      marquee.classList.add('is-dragging');
      event.preventDefault();
    });

    document.addEventListener('mousemove', function (event) {
      if (!dragging) return;
      event.preventDefault();
      moveWhileDragging(event.clientX);
    });

    document.addEventListener('mouseup', function () {
      if (!dragging) return;
      dragging = false;
      marquee.classList.remove('is-dragging');
    });
  }

  function initializeAllMarquees() {
    document.querySelectorAll('.marquee').forEach(initializeMarquee);
  }

  initializeAllMarquees();
  new MutationObserver(initializeAllMarquees).observe(document.body, {
    childList: true,
    subtree: true
  });
})();