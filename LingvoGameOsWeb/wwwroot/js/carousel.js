export function initializeCarousel() {
    // Helper function to initialize a carousel
    function setupCarousel(carouselSelector, listSelector, prevSelector, nextSelector, counterSelector, itemsPerViewFn) {
        const carousel = document.querySelector(carouselSelector);
        if (!carousel) {
            console.warn(`Carousel not found: ${carouselSelector}`);
            return;
        }

        const list = carousel.querySelector(listSelector);
        const items = list.querySelectorAll(`${listSelector} > *`);
        const prevButton = carousel.querySelector(prevSelector);
        const nextButton = carousel.querySelector(nextSelector);
        const pageCounter = carousel.querySelector(counterSelector);
        if (!items.length || !prevButton || !nextButton || !pageCounter) {
            console.warn(`Carousel setup failed for ${carouselSelector}: missing elements (items=${items.length}, prev=${!!prevButton}, next=${!!nextButton}, counter=${!!pageCounter})`);
            return;
        }

        let currentPage = 0;

        function updateCarousel() {
            const itemsPerView = itemsPerViewFn();
            const totalPages = Math.ceil(items.length / itemsPerView);
            currentPage = Math.max(0, Math.min(currentPage, totalPages - 1));

            const itemWidth = items[0].offsetWidth + 24; // Include gap
            list.style.transform = `translateX(-${currentPage * itemWidth * itemsPerView}px)`;

            // Update page counter
            pageCounter.textContent = `${currentPage + 1}/${totalPages}`;

            // Update button states
            prevButton.disabled = currentPage === 0;
            nextButton.disabled = currentPage >= totalPages - 1;
        }

        prevButton.addEventListener('click', () => {
            if (currentPage > 0) {
                currentPage--;
                updateCarousel();
            }
        });

        nextButton.addEventListener('click', () => {
            const itemsPerView = itemsPerViewFn();
            if (currentPage < Math.ceil(items.length / itemsPerView) - 1) {
                currentPage++;
                updateCarousel();
            }
        });

        window.addEventListener('resize', () => {
            currentPage = 0;
            updateCarousel();
        });

        // Initial update
        updateCarousel();

        // Return reinitialize function for dynamic content
        return () => {
            currentPage = 0;
            updateCarousel();
        };
    }

    // User Added Games Carousel
    const reinitializeUserGames = setupCarousel(
        '.user-added-games__carousel',
        '.user-added-games__list',
        '.user-added-games__prev',
        '.user-added-games__next',
        '.user-added-games__page-counter',
        () => window.innerWidth <= 480 ? 1 : 2
    );

    // Recent Games Carousel
    const reinitializeRecentGames = setupCarousel(
        '.recent-games__carousel',
        '.recent-games__list',
        '.recent-games__prev',
        '.recent-games__next',
        '.recent-games__page-counter',
        () => window.innerWidth <= 480 ? 2 : 3
    );

    // MutationObserver for dynamic content
    const observer = new MutationObserver(() => {
        if (reinitializeUserGames) reinitializeUserGames();
        if (reinitializeRecentGames) reinitializeRecentGames();
    });

    const userGamesCarousel = document.querySelector('.user-added-games__carousel');
    const recentGamesCarousel = document.querySelector('.recent-games__carousel');
    if (userGamesCarousel) {
        observer.observe(userGamesCarousel, { childList: true, subtree: true });
    }
    if (recentGamesCarousel) {
        observer.observe(recentGamesCarousel, { childList: true, subtree: true });
    }
}