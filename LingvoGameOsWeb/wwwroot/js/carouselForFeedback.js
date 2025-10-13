document.addEventListener('DOMContentLoaded', function () {
    class ReviewsCarousel {
        constructor() {
            this.carousel = document.querySelector('.game-reviews__container');
            this.prevButton = document.querySelector('.game-reviews__prev');
            this.nextButton = document.querySelector('.game-reviews__next');
            this.indicatorsContainer = document.querySelector('.game-reviews__indicators');
            this.cards = document.querySelectorAll('.game-reviews__card');
            this.reviewsSection = document.querySelector('.game-reviews');

            this.currentIndex = 0;
            this.isAnimating = false;
            this.autoPlayInterval = null;
            this.touchStartX = 0;
            this.touchEndX = 0;
            this.isDragging = false;
            this.dragOffset = 0;

            this.init();
        }

        init() {
            if (!this.carousel || this.cards.length === 0) return;

            this.setupInfiniteScroll();
            this.createIndicators();
            this.setupEventListeners();
            this.updateCarousel();
            this.startAutoPlay();

            // Наблюдатель за изменениями DOM
            this.setupMutationObserver();
        }

        setupInfiniteScroll() {
            if (this.cards.length === 0) return;

            // Клонируем карточки для бесконечной прокрутки
            const firstCard = this.cards[0].cloneNode(true);
            const lastCard = this.cards[this.cards.length - 1].cloneNode(true);

            // Добавляем клоны в начало и конец
            this.carousel.insertBefore(lastCard, this.cards[0]);
            this.carousel.appendChild(firstCard);

            // Обновляем список карточек
            this.cards = document.querySelectorAll('.game-reviews__card');
            
            // Устанавливаем начальную позицию (первая оригинальная карточка)
            this.currentIndex = 1;
        }

        setupMutationObserver() {
            const observer = new MutationObserver((mutations) => {
                mutations.forEach((mutation) => {
                    if (mutation.type === 'childList') {
                        // Пересчитываем карточки при изменении DOM
                        this.cards = document.querySelectorAll('.game-reviews__card');
                        this.setupInfiniteScroll();
                        this.createIndicators();
                        this.updateCarousel();
                    }
                });
            });

            if (this.carousel) {
                observer.observe(this.carousel, {
                    childList: true,
                    subtree: true
                });
            }
        }

        getCardWidth() {
            if (this.cards.length === 0) return 0;
            const card = this.cards[0];
            const style = window.getComputedStyle(this.carousel);
            const gap = parseInt(style.gap) || 20;
            return card.offsetWidth + gap;
        }

        getContainerWidth() {
            return this.reviewsSection ? this.reviewsSection.offsetWidth : this.carousel.parentElement.offsetWidth;
        }

        getVisibleCardsCount() {
            const containerWidth = this.getContainerWidth();
            const cardWidth = this.getCardWidth();
            return cardWidth > 0 ? Math.floor(containerWidth / cardWidth) : 1;
        }

        getOriginalCardsCount() {
            return this.cards.length - 2; // Исключаем клоны
        }

        createIndicators() {
            if (!this.indicatorsContainer) return;

            this.indicatorsContainer.innerHTML = '';
            const originalCount = this.getOriginalCardsCount();

            for (let i = 0; i < originalCount; i++) {
                const indicator = document.createElement('span');
                indicator.className = 'game-reviews__indicator';
                indicator.addEventListener('click', () => this.goToSlide(i + 1)); // +1 из-за клона в начале
                this.indicatorsContainer.appendChild(indicator);
            }
        }

        updateCarousel() {
            if (this.cards.length === 0 || this.isAnimating) return;

            const cardWidth = this.getCardWidth();
            const translateX = -this.currentIndex * cardWidth + this.dragOffset;

            this.carousel.style.transform = `translateX(${translateX}px)`;

            // Обновляем индикаторы
            const indicators = this.indicatorsContainer.querySelectorAll('.game-reviews__indicator');
            const originalIndex = this.getOriginalIndex();
            indicators.forEach((indicator, index) => {
                indicator.classList.toggle('active', index === originalIndex);
            });

            // Обновляем кнопки
            this.updateButtons();
        }

        getOriginalIndex() {
            const originalCount = this.getOriginalCardsCount();
            if (this.currentIndex === 0) return originalCount - 1; // Последняя оригинальная карточка
            if (this.currentIndex === this.cards.length - 1) return 0; // Первая оригинальная карточка
            return this.currentIndex - 1; // Обычная карточка
        }

        updateButtons() {
            if (this.prevButton) {
                this.prevButton.disabled = false;
            }
            if (this.nextButton) {
                this.nextButton.disabled = false;
            }
        }

        goToSlide(index) {
            if (this.isAnimating || this.cards.length === 0) return;

            this.isAnimating = true;
            this.currentIndex = index;
            this.dragOffset = 0;

            this.updateCarousel();

            // Проверяем на необходимость перехода к клонам
            setTimeout(() => {
                this.checkInfiniteLoop();
                this.isAnimating = false;
            }, 500);
        }

        checkInfiniteLoop() {
            const originalCount = this.getOriginalCardsCount();
            
            if (this.currentIndex === 0) {
                // Переходим к последней оригинальной карточке
                this.currentIndex = originalCount;
                this.carousel.style.transition = 'none';
                this.updateCarousel();
                setTimeout(() => {
                    this.carousel.style.transition = 'transform 0.5s ease-in-out';
                }, 50);
            } else if (this.currentIndex === this.cards.length - 1) {
                // Переходим к первой оригинальной карточке
                this.currentIndex = 1;
                this.carousel.style.transition = 'none';
                this.updateCarousel();
                setTimeout(() => {
                    this.carousel.style.transition = 'transform 0.5s ease-in-out';
                }, 50);
            }
        }

        nextSlide() {
            if (this.cards.length === 0) return;
            this.currentIndex++;
            this.goToSlide(this.currentIndex);
        }

        prevSlide() {
            if (this.cards.length === 0) return;
            this.currentIndex--;
            this.goToSlide(this.currentIndex);
        }

        setupEventListeners() {
            // Кнопки навигации
            if (this.prevButton) {
                this.prevButton.addEventListener('click', () => {
                    this.stopAutoPlay();
                    this.prevSlide();
                    this.startAutoPlay();
                });
            }

            if (this.nextButton) {
                this.nextButton.addEventListener('click', () => {
                    this.stopAutoPlay();
                    this.nextSlide();
                    this.startAutoPlay();
                });
            }

            // Ресайз окна
            let resizeTimeout;
            window.addEventListener('resize', () => {
                clearTimeout(resizeTimeout);
                resizeTimeout = setTimeout(() => {
                    this.updateCarousel();
                }, 250);
            });

            // Touch события
            this.setupTouchEvents();

            // Mouse события для десктопа
            this.setupMouseEvents();

            // Пауза автопрокрутки при наведении
            if (this.carousel) {
                this.carousel.addEventListener('mouseenter', () => this.stopAutoPlay());
                this.carousel.addEventListener('mouseleave', () => this.startAutoPlay());
            }
        }

        setupTouchEvents() {
            if (!this.carousel) return;

            this.carousel.addEventListener('touchstart', (e) => {
                this.touchStartX = e.touches[0].clientX;
                this.isDragging = true;
                this.carousel.style.transition = 'none';
                this.stopAutoPlay();
            });

            this.carousel.addEventListener('touchmove', (e) => {
                if (!this.isDragging) return;
                e.preventDefault();
                
                this.touchEndX = e.touches[0].clientX;
                this.dragOffset = this.touchStartX - this.touchEndX;
                
                const cardWidth = this.getCardWidth();
                const translateX = -this.currentIndex * cardWidth + this.dragOffset;
                this.carousel.style.transform = `translateX(${translateX}px)`;
            });

            this.carousel.addEventListener('touchend', () => {
                if (!this.isDragging) return;
                this.isDragging = false;
                this.carousel.style.transition = 'transform 0.5s ease-in-out';

                const threshold = 50;
                if (Math.abs(this.dragOffset) > threshold) {
                    if (this.dragOffset > 0) {
                        this.nextSlide();
                    } else {
                        this.prevSlide();
                    }
                } else {
                    this.dragOffset = 0;
                    this.updateCarousel();
                }

                this.startAutoPlay();
            });
        }

        setupMouseEvents() {
            if (!this.carousel) return;

            let isMouseDown = false;
            let startX = 0;

            this.carousel.addEventListener('mousedown', (e) => {
                isMouseDown = true;
                startX = e.clientX;
                this.carousel.style.transition = 'none';
                this.stopAutoPlay();
                e.preventDefault();
            });

            this.carousel.addEventListener('mousemove', (e) => {
                if (!isMouseDown) return;
                
                const dragOffset = startX - e.clientX;
                const cardWidth = this.getCardWidth();
                const translateX = -this.currentIndex * cardWidth + dragOffset;
                this.carousel.style.transform = `translateX(${translateX}px)`;
            });

            this.carousel.addEventListener('mouseup', (e) => {
                if (!isMouseDown) return;
                isMouseDown = false;
                this.carousel.style.transition = 'transform 0.5s ease-in-out';

                const dragOffset = startX - e.clientX;
                const threshold = 50;
                
                if (Math.abs(dragOffset) > threshold) {
                    if (dragOffset > 0) {
                        this.nextSlide();
                    } else {
                        this.prevSlide();
                    }
                } else {
                    this.updateCarousel();
                }

                this.startAutoPlay();
            });

            this.carousel.addEventListener('mouseleave', () => {
                if (isMouseDown) {
                    isMouseDown = false;
                    this.carousel.style.transition = 'transform 0.5s ease-in-out';
                    this.updateCarousel();
                    this.startAutoPlay();
                }
            });
        }

        startAutoPlay() {
            this.stopAutoPlay();
            if (this.cards.length > 1) {
                this.autoPlayInterval = setInterval(() => {
                    this.nextSlide();
                }, 4000); // Автопрокрутка каждые 4 секунды
            }
        }

        stopAutoPlay() {
            if (this.autoPlayInterval) {
                clearInterval(this.autoPlayInterval);
                this.autoPlayInterval = null;
            }
        }

        // Публичный метод для добавления нового отзыва
        addReview(reviewData) {
            if (!this.carousel) return;

            const reviewCard = document.createElement('article');
            reviewCard.className = 'game-reviews__card';
            reviewCard.innerHTML = `
                <div class="game-reviews__reviewer">
                    <img src="${reviewData.authorAvatarPath}" alt="Reviewer" class="game-reviews__avatar">
                    <div class="game-reviews__reviewer-details">
                        <span class="game-reviews__reviewer-name">${reviewData.authorName}</span>
                        <span class="game-reviews__reviewer-location">${reviewData.gameTitle}</span>
                    </div>
                    <div class="game-reviews__rating">
                        <span class="game-reviews__rating-score">${reviewData.rating}</span>
                        <img src="/icon/star-filled.svg" alt="Star" class="game-reviews__star">
                    </div>
                </div>
                <p class="game-reviews__text">${reviewData.text}</p>
            `;

            // Добавляем перед последним клоном
            const lastClone = this.carousel.lastElementChild;
            this.carousel.insertBefore(reviewCard, lastClone);
        }
    }

    // Инициализация карусели
    const carousel = new ReviewsCarousel();

    // Глобальная функция для добавления новых отзывов
    window.addReviewToCarousel = function (reviewData) {
        if (carousel) {
            carousel.addReview(reviewData);
        }
    };

    // Экспорт для использования в других модулях
    window.ReviewsCarousel = carousel;
});