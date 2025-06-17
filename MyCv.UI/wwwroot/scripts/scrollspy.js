// Minimal scroll spy - add to your _Layout.cshtml or index.html in a <script> tag
window.initScrollSpy = (scrollContainer, sectionIds, dotNetRef, baseUrl) => {
  let currentSection = '';
  let isNavigating = false;

  const updateCurrentSection = (sectionId) => {
    if (sectionId !== currentSection) {
      currentSection = sectionId;
      const newUrl = baseUrl ? `${baseUrl}#${sectionId}` : `${window.location.pathname}#${sectionId}`;
      history.replaceState(null, null, newUrl);
      dotNetRef.invokeMethodAsync('UpdateCurrentSection', sectionId);
    }
  };

  const observer = new IntersectionObserver((entries) => {
    if (isNavigating) return; // Don't update during navigation

    // Sort entries by their position in the viewport
    const visibleSections = entries
      .filter(entry => entry.isIntersecting)
      .sort((a, b) => a.boundingClientRect.top - b.boundingClientRect.top);

    if (visibleSections.length > 0) {
      // Use the topmost visible section
      const sectionId = visibleSections[0].target.id;
      updateCurrentSection(sectionId);
    } else {
      // If no sections are visible, check if we're at the top
      const scrollTop = scrollContainer.scrollTop;
      if (scrollTop < 100 && sectionIds.length > 0) {
        // We're near the top, activate first section
        const firstSectionId = sectionIds[0];
        updateCurrentSection(firstSectionId);
      }
    }
  }, {
    root: scrollContainer,
    rootMargin: '-20% 0px -60% 0px', // Adjusted margins for better detection
    threshold: [0, 0.1, 0.5] // Multiple thresholds for better detection
  });

  // Observe all sections
  sectionIds.forEach(id => {
    const element = document.getElementById(id);
    if (element) observer.observe(element);
  });

  // Also listen for scroll events to handle edge cases
  scrollContainer.addEventListener('scroll', () => {
    if (isNavigating) return; // Don't update during navigation

    const scrollTop = scrollContainer.scrollTop;
    // If we're at the very top, ensure first section is active
    if (scrollTop < 50 && sectionIds.length > 0) {
      const firstSectionId = sectionIds[0];
      updateCurrentSection(firstSectionId);
    }
  });

  // Handle navigation clicks
  document.addEventListener('click', (e) => {
    const link = e.target.closest('a[href*="#"]');
    if (link) {
      const href = link.getAttribute('href');
      const hashIndex = href.indexOf('#');
      if (hashIndex !== -1) {
        const sectionId = href.substring(hashIndex + 1);
        if (sectionIds.includes(sectionId)) {
          e.preventDefault();
          isNavigating = true;

          // Immediately update the active section
          updateCurrentSection(sectionId);

          // Scroll to the section
          const targetElement = document.getElementById(sectionId);
          if (targetElement) {
            targetElement.scrollIntoView({
              behavior: 'smooth',
              block: 'start'
            });
          }

          // Reset navigation flag after scroll completes
          setTimeout(() => {
            isNavigating = false;
          }, 1000);
        }
      }
    }
  });
};
