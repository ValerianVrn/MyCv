window.spotlight = {
  init: function (selectors, rootMargin) {
    if (window.matchMedia('(hover: hover)').matches) return; // Only on phones
    if (window.matchMedia('(min-width: 960px)').matches) return; // Until md screen

    function spotlightOne(selector) {
      var cards = document.querySelectorAll(selector);
      if (!cards.length) return;
      var current = null;
      var obs = new IntersectionObserver(function (entries) {
        var visible = entries
          .filter(function (e) { return e.isIntersecting; })
          .sort(function (a, b) { return b.intersectionRatio - a.intersectionRatio; });
        if (!visible.length || visible[0].target === current) return;

        if (current) current.classList.remove('active');
        visible[0].target.classList.add('active');
        current = visible[0].target;
      }, { rootMargin: rootMargin, threshold: 0 });
      cards.forEach(function (c) { obs.observe(c); });
    }

    selectors.forEach(function (s) { spotlightOne(s); });
  },

  highlightAll: function (sectionSelector, chipSelector) {
    if (window.matchMedia('(hover: hover)').matches) return; // Only on phones
    if (window.matchMedia('(min-width: 960px)').matches) return; // Until md screen
    var section = document.querySelector(sectionSelector);
    if (!section) return;
    var obs = new IntersectionObserver(function (entries) {
      var chips = document.querySelectorAll(chipSelector);
      chips.forEach(function (c) {
        c.classList.toggle('active', entries[0].isIntersecting);
      });
    }, { rootMargin: '-10% 0px -90% 0px', threshold: 0 });
    obs.observe(section);
  }
};
