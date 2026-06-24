window.spotlight = {
  init: function (selectors, rootMargin) {
    if (window.matchMedia('(hover: hover)').matches) return;

    function spotlightOne(selector) {
      var cards = document.querySelectorAll(selector);
      if (!cards.length) return;
      var current = null;
      var obs = new IntersectionObserver(function (entries) {
        entries.forEach(function (e) {
          if (e.isIntersecting) {
            if (current && current !== e.target)
              current.classList.remove('active');
            e.target.classList.add('active');
            current = e.target;
          } else {
            e.target.classList.remove('active');
            if (current === e.target) current = null;
          }
        });
      }, { rootMargin: rootMargin, threshold: 0 });
      cards.forEach(function (c) { obs.observe(c); });
    }

    selectors.forEach(function (s) { spotlightOne(s); });
  },

  highlightAll: function (sectionSelector, chipSelector) {
    if (window.matchMedia('(hover: hover)').matches) return;
    var section = document.querySelector(sectionSelector);
    if (!section) return;
    var obs = new IntersectionObserver(function (entries) {
      var chips = document.querySelectorAll(chipSelector);
      chips.forEach(function (c) {
        c.classList.toggle('active', entries[0].isIntersecting);
      });
    }, { rootMargin: '-50% 0px -50% 0px', threshold: 0 });
    obs.observe(section);
  }
};
