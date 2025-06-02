document.addEventListener('DOMContentLoaded', () => {
    // User Added Games Carousel
    const userGamesCarousel = document.querySelector('.user-added-games__carousel');
    if (userGamesCarousel) {
        const userGamesList = userGamesCarousel.querySelector('.user-added-games__list');
        const userGamesItems = userGamesList.querySelectorAll('.user-added-games__item');
        const prevButtonUser = userGamesCarousel.querySelector('.user-added-games__prev');
        const nextButtonUser = userGamesCarousel.querySelector('.user-added-games__next');
        let currentIndexUser = 0;
        const itemsPerViewUser = window.innerWidth <= 480 ? 1 : 2;

        const updateUserCarousel = () => {
            const itemWidth = userGamesItems[0].offsetWidth + 24; // Include gap
            userGamesList.style.transform = `translateX(-${currentIndexUser * itemWidth}px)`;
            prevButtonUser.disabled = currentIndexUser === 0;
            nextButtonUser.disabled = currentIndexUser >= userGamesItems.length - itemsPerViewUser;
        };

        prevButtonUser.addEventListener('click', () => {
            if (currentIndexUser > 0) {
                currentIndexUser--;
                updateUserCarousel();
            }
        });

        nextButtonUser.addEventListener('click', () => {
            if (currentIndexUser < userGamesItems.length - itemsPerViewUser) {
                currentIndexUser++;
                updateUserCarousel();
            }
        });

        window.addEventListener('resize', () => {
            currentIndexUser = 0;
            updateUserCarousel();
        });

        updateUserCarousel();
    }

    // Recent Games Carousel
    const recentGamesCarousel = document.querySelector('.recent-games__carousel');
    if (recentGamesCarousel) {
        const recentGamesList = recentGamesCarousel.querySelector('.recent-games__list');
        const recentGamesItems = recentGamesList.querySelectorAll('.recent-games__item');
        const prevButtonRecent = recentGamesCarousel.querySelector('.recent-games__prev');
        const nextButtonRecent = recentGamesCarousel.querySelector('.recent-games__next');
        let currentIndexRecent = 0;
        const itemsPerViewRecent = window.innerWidth <= 480 ? 2 : 3;

        const updateRecentCarousel = () => {
            const itemWidth = recentGamesItems[0].offsetWidth + 24; // Include gap
            recentGamesList.style.transform = `translateX(-${currentIndexPrevious * itemWidth}px)`;
            prevButtonRecent.disabled = currentIndexRecent === 0;
            nextButtonRecent.disabled = currentIndexRecent >= recentGamesItems.length - itemsPerViewRecent;
        };

        prevButtonRecent.addEventListener('click', () => {
            if (currentIndexRecent > 0) {
                currentIndexRecent--;
                updateRecentCarousel();
            }
        });

        nextButtonRecent.addEventListener('click', () => {
            if (currentIndexRecent < recentGamesItems.length - itemsPerViewRecent) {
                currentIndexRecent++;
                updateRecentCarousel();
            }
        });

        window.addEventListener('resize', () => {
            currentIndexRecent = 0;
            updateRecentCarousel();
        });

        updateRecentCarousel();
    }
});