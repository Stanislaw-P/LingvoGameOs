document.addEventListener('DOMContentLoaded', function () {
    const carousel = document.querySelector('.game-reviews__container');
    const prevButton = document.querySelector('.game-reviews__prev');
    const nextButton = document.querySelector('.game-reviews__next');
    const indicators = document.querySelectorAll('.game-reviews__indicator');
    const cards = document.querySelectorAll('.game-reviews__card');
    const reviewsSection = document.querySelector('.game-reviews');

    if (!carousel || !prevButton || !nextButton || cards.length === 0) return;

    let currentIndex = 0;

    function getCardWidth() {
        const card = cards[0];
        const style = window.getComputedStyle(carousel);
        const gap = parseInt(style.gap) || 20;
        return card.offsetWidth + gap;
    }

    function getContainerWidth() {
        return reviewsSection ? reviewsSection.offsetWidth : carousel.parentElement.offsetWidth;
    }

    function getVisibleCardsCount() {
        const containerWidth = getContainerWidth();
        const cardWidth = getCardWidth();
        return Math.floor(containerWidth / cardWidth);
    }

    function getMaxIndex() {
        const containerWidth = getContainerWidth();

        if (containerWidth < 346) {
            // Для очень маленьких экранов - точный расчет с учетом полного отображения последнего элемента
            const cardRealWidth = cards[0].offsetWidth;
            const gap = parseInt(window.getComputedStyle(carousel).gap) || 20;
            const totalCardWidth = cardRealWidth + gap;
            const totalWidthNeeded = cards.length * totalCardWidth - gap; // Убираем последний gap

            // Если все карточки помещаются в контейнер, то максимальный индекс = 0
            if (totalWidthNeeded <= containerWidth) {
                return 0;
            }

            // Вычисляем максимальный индекс так, чтобы последняя карточка была полностью видна
            const availableWidth = containerWidth - cardRealWidth; // Доступная ширина для скролла
            const maxScrollDistance = totalWidthNeeded - containerWidth;
            const maxIndex = Math.floor(maxScrollDistance / totalCardWidth);

            return Math.max(0, Math.min(maxIndex, cards.length - 1));

        } else if (containerWidth < 480) {
            // Для мобильных можно скроллить до последней карточки
            return Math.max(0, cards.length - 1);
        }

        const visibleCards = getVisibleCardsCount();
        return Math.max(0, cards.length - visibleCards);
    }

    function updateCarousel() {
        const cardWidth = getCardWidth();
        const containerWidth = getContainerWidth();
        const maxIndex = getMaxIndex();

        // Корректируем currentIndex
        currentIndex = Math.min(currentIndex, maxIndex);

        let translateX;

        if (containerWidth < 346) {
            // Центрирование для очень маленьких экранов с гарантией полного отображения последнего элемента
            const cardRealWidth = cards[0].offsetWidth;
            const gap = parseInt(window.getComputedStyle(carousel).gap) || 20;
            const totalCardWidth = cardRealWidth + gap;

            // Центрируем текущую карточку
            translateX = -currentIndex * totalCardWidth + (containerWidth - cardRealWidth) / 2;

            // Для последней карточки корректируем позицию, чтобы она была полностью видна
            if (currentIndex === maxIndex && maxIndex > 0) {
                const totalCarouselWidth = cards.length * totalCardWidth - gap;
                const maxTranslate = containerWidth - totalCarouselWidth;
                translateX = Math.max(translateX, maxTranslate);
            }

        } else if (containerWidth < 480) {
            // Центрирование для мобильных
            const cardRealWidth = cards[0].offsetWidth;
            const gap = parseInt(window.getComputedStyle(carousel).gap) || 20;
            const totalCardWidth = cardRealWidth + gap;

            // Центрируем текущую карточку
            translateX = -currentIndex * totalCardWidth + (containerWidth - cardRealWidth) / 2;
        } else {
            // Стандартное поведение для десктопа
            translateX = -currentIndex * cardWidth;
        }

        carousel.style.transform = `translateX(${translateX}px)`;

        // Обновляем индикаторы
        indicators.forEach((indicator, index) => {
            indicator.classList.toggle('active', index === currentIndex);
        });

        // Обновляем состояние кнопок
        prevButton.disabled = currentIndex === 0;
        nextButton.disabled = currentIndex >= maxIndex;

        console.log(`Width: ${containerWidth}px, Current: ${currentIndex}, Max: ${maxIndex}, Total cards: ${cards.length}`);
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

    // Обработчик ресайза
    let resizeTimeout;
    window.addEventListener('resize', () => {
        clearTimeout(resizeTimeout);
        resizeTimeout = setTimeout(() => {
            // При ресайзе сбрасываем на первый слайд, чтобы избежать проблем
            currentIndex = 0;
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

        const containerWidth = getContainerWidth();
        const maxIndex = getMaxIndex();
        let baseTranslate;

        if (containerWidth < 346) {
            const cardRealWidth = cards[0].offsetWidth;
            const gap = parseInt(window.getComputedStyle(carousel).gap) || 20;
            const totalCardWidth = cardRealWidth + gap;
            baseTranslate = -currentIndex * totalCardWidth + (containerWidth - cardRealWidth) / 2;

            // Ограничиваем drag для последнего элемента
            if (currentIndex === maxIndex && maxIndex > 0) {
                const totalCarouselWidth = cards.length * totalCardWidth - gap;
                const maxTranslate = containerWidth - totalCarouselWidth;
                baseTranslate = Math.max(baseTranslate, maxTranslate);
            }

        } else if (containerWidth < 480) {
            const cardRealWidth = cards[0].offsetWidth;
            const gap = parseInt(window.getComputedStyle(carousel).gap) || 20;
            const totalCardWidth = cardRealWidth + gap;
            baseTranslate = -currentIndex * totalCardWidth + (containerWidth - cardRealWidth) / 2;
        } else {
            baseTranslate = -currentIndex * getCardWidth();
        }

        carousel.style.transform = `translateX(${baseTranslate - diff}px)`;
    });

    carousel.addEventListener('touchend', () => {
        if (!isDragging) return;
        isDragging = false;
        carousel.style.transition = 'transform 0.5s ease-in-out';

        const diff = startX - currentX;
        const maxIndex = getMaxIndex();

        if (Math.abs(diff) > 30) {
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