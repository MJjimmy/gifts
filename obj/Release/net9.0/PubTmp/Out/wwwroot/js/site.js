// Please see documentation at https://learn.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.

function showTaxCertificate() {

    const certificate =
        document.getElementById("taxCertificatePreview");

    if (certificate.style.display === "none") {

        certificate.style.display = "block";

        certificate.scrollIntoView({
            behavior: "smooth"
        });

    } else {

        certificate.style.display = "none";

    }
}

// --- Reveal-on-scroll using IntersectionObserver ---

(function () {
    if (window.matchMedia && window.matchMedia('(prefers-reduced-motion: reduce)').matches) {
        // don't run animations when user requests reduced motion
        return;
    }

    const selectors = [
        '.project-card',
        '.hero-card',
        '.intro-image',
        '.impact-item',
        '.project-content',
        '.donation-cta',
        '.projects-section',
        '.volunteer-hero'
    ];

    const elements = Array.from(document.querySelectorAll(selectors.join(',')))
        // exclude elements already visible (e.g., inside viewport on load) will be handled by observer
        .filter(Boolean);

    if (!elements.length || !('IntersectionObserver' in window)) return;

    const observer = new IntersectionObserver((entries, obs) => {
        entries.forEach(entry => {
            if (entry.isIntersecting) {
                entry.target.classList.add('is-visible');
                // add unified class so CSS uses .reveal-on-scroll.is-visible
                entry.target.classList.add('reveal-on-scroll');
                // if you want to animate children with .stagger, add small delays:
                const children = entry.target.querySelectorAll('.stagger');
                children.forEach((el, i) => el.style.transitionDelay = (i * 80) + 'ms');
                obs.unobserve(entry.target);
            } else {
                // keep once visible (no toggling off)
            }
        });
    }, {
        root: null,
        rootMargin: '0px 0px -8% 0px', // reveal slightly earlier
        threshold: 0.08
    });

    elements.forEach(el => {
        // set initial class for consistency
        el.classList.add('reveal-on-scroll');
        observer.observe(el);
    });
})();

// Project card tilt (lightweight, respects prefers-reduced-motion)
(function () {
    if (window.matchMedia && window.matchMedia('(prefers-reduced-motion: reduce)').matches) return;

    const maxTilt = 10; // degrees
    const activeWidthMin = 768; // don't run on small screens

    function clamp(v, a, b) { return Math.max(a, Math.min(b, v)); }

    function bindTilt(card) {
        // ensure an inner surface exists to transform
        let surface = card.querySelector('.card-surface');
        if (!surface) {
            surface = document.createElement('div');
            surface.className = 'card-surface';
            // move existing children into surface
            while (card.firstChild) surface.appendChild(card.firstChild);
            card.appendChild(surface);
        }

        let frameId = null;

        function onPointerMove(e) {
            if (window.innerWidth < activeWidthMin) return;
            const rect = card.getBoundingClientRect();
            const px = ((e.clientX - rect.left) / rect.width) * 2 - 1; // -1 .. 1
            const py = ((e.clientY - rect.top) / rect.height) * 2 - 1; // -1 .. 1
            const rotY = clamp(px * maxTilt, -maxTilt, maxTilt);
            const rotX = clamp(-py * maxTilt, -maxTilt, maxTilt);

            if (frameId) cancelAnimationFrame(frameId);
            frameId = requestAnimationFrame(() => {
                surface.style.transform = `rotateX(${rotX}deg) rotateY(${rotY}deg)`;
            });
        }

        function onPointerLeave() {
            if (frameId) cancelAnimationFrame(frameId);
            surface.style.transform = '';
        }

        card.addEventListener('pointermove', onPointerMove, { passive: true });
        card.addEventListener('pointerleave', onPointerLeave);
        card.addEventListener('pointercancel', onPointerLeave);
    }

    // apply to existing project cards
    const cards = document.querySelectorAll('.project-card');
    cards.forEach(bindTilt);
})();