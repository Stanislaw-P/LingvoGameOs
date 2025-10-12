document.addEventListener('DOMContentLoaded', function () {
    const carousel = document.querySelector('.game-reviews__container');
    const prevButton = document.querySelector('.game-reviews__prev');
    const nextButton = document.querySelector('.game-reviews__next');
    const indicators = document.querySelectorAll('.game-reviews__indicator');
    const cards = document.querySelectorAll('.game-reviews__card');

    if (!carousel || !prevButton || !nextButton || cards.length === 0) return;

    let currentIndex = 0;

    function getCardWidth() {
        const card = cards[0];
        const style = window.getComputedStyle(carousel);
        const gap = parseInt(style.gap) || 20;
        return card.offsetWidth + gap;
    }

    function getVisibleCardsCount() {
        const containerWidth = carousel.parentElement.offsetWidth; // Используем родительский контейнер
        const cardWidth = getCardWidth();
        return Math.floor(containerWidth / cardWidth);
    }

    function getMaxIndex() {
        const visibleCards = getVisibleCardsCount();
        // Максимальный индекс - когда последняя карточка видна
        return Math.max(0, cards.length - visibleCards);
    }

    function updateCarousel() {
        const cardWidth = getCardWidth();
        const translateX = -currentIndex * cardWidth;
        carousel.style.transform = `translateX(${translateX}px)`;

        // Обновляем индикаторы
        indicators.forEach((indicator, index) => {
            indicator.classList.toggle('active', index === currentIndex);
        });

        // Обновляем состояние кнопок
        const maxIndex = getMaxIndex();

        prevButton.disabled = currentIndex === 0;
        nextButton.disabled = currentIndex >= maxIndex;

        // Если после ресайза currentIndex стал больше максимального, корректируем
        if (currentIndex > maxIndex) {
            currentIndex = maxIndex;
            updateCarousel();
        }
    }

    function goToSlide(index) {
        const maxIndex = getMaxIndex();
        currentIndex = Math.max(0, Math.min(index, maxIndex));
        updateCarousel();
    }

    // Обработчики событий
    prevButton.addEventListener('click', () => {
        if (currentIndex > 0) {
            goToSlide(currentIndex - 1);
        }
    });

    nextButton.addEventListener('click', () => {
        const maxIndex = getMaxIndex();
        if (currentIndex < maxIndex) {
            goToSlide(currentIndex + 1);
        }
    });

    indicators.forEach((indicator, index) => {
        indicator.addEventListener('click', () => {
            goToSlide(index);
        });
    });

    // Обработчик ресайза с debounce
    let resizeTimeout;
    window.addEventListener('resize', () => {
        clearTimeout(resizeTimeout);
        resizeTimeout = setTimeout(() => {
            updateCarousel();
        }, 250);
    });

    // Touch/swipe для мобильных
    let startX = 0;
    let currentX = 0;
    let isDragging = false;

    carousel.addEventListener('touchstart', (e) => {
        startX = e.touches[0].clientX;
        isDragging = true;
        carousel.style.transition = 'none';
    });

    carousel.addEventListener('touchmove', (e) => {
        if (!isDragging) return;
        currentX = e.touches[0].clientX;
        const diff = startX - currentX;
        const cardWidth = getCardWidth();
        carousel.style.transform = `translateX(calc(-${currentIndex * cardWidth}px - ${diff}px))`;
    });

    carousel.addEventListener('touchend', () => {
        if (!isDragging) return;
        isDragging = false;
        carousel.style.transition = 'transform 0.5s ease-in-out';

        const diff = startX - currentX;
        const maxIndex = getMaxIndex();

        if (Math.abs(diff) > 50) {
            if (diff > 0 && currentIndex < maxIndex) {
                goToSlide(currentIndex + 1);
            } else if (diff < 0 && currentIndex > 0) {
                goToSlide(currentIndex - 1);
            } else {
                updateCarousel();
            }
        } else {
            updateCarousel();
        }
    });

    // Инициализация
    updateCarousel();
});