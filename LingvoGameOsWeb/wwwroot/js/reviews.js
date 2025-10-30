// Отображение модального окна для отзыва
export async function showReviewModal(gameId) {
    const modal = document.createElement('div');
    modal.className = 'review-modal';
    modal.innerHTML = `
        <div class="review-modal__content">
            <h2 class="review-modal__title">Ваш отзыв об игре</h2>
            <p class="review-modal__label">Внимание! Если вы уже писали отзыв к этой игре, он будет перезаписан.</p>
            <br>
            <form class="review-modal__form">
                <div class="review-modal__field">
                    <label class="review-modal__label">Оценка игры</label>
                    <div class="review-modal__stars" id="star-rating">
                        ${Array(5).fill().map((_, i) => `
                            <svg class="star" data-value="${i + 1}" width="24" height="24" viewBox="0 0 24 24" fill="none" xmlns="http://www.w3.org/2000/svg">
                                <path d="M12 2L15.09 8.26L22 9.27L17 14.14L18.18 21.02L12 17.77L5.82 21.02L7 14.14L2 9.27L8.91 8.26L12 2Z" fill="#D2D2E5"/>
                            </svg>
                        `).join('')}
                    </div>
                </div>
                <div class="review-modal__field">
                    <label class="review-modal__label">Ваш комментарий</label>
                    <textarea class="review-modal__textarea" placeholder="Напишите ваш отзыв здесь..."></textarea>
                </div>
                <div class="review-modal__actions">
                    <button type="submit" class="profile__button">Отправить</button>
                    <button type="button" class="profile__button review-modal__close">Отмена</button>
                </div>
            </form>
        </div>
    `;
    document.body.appendChild(modal);

    const stars = modal.querySelectorAll('.star');
    let rating = 0;

    stars.forEach(star => {
        star.addEventListener('click', () => {
            rating = star.getAttribute('data-value');
            stars.forEach(s => {
                s.querySelector('path').setAttribute('fill', s.getAttribute('data-value') <= rating ? '#51D546' : '#D2D2E5');
            });
        });
    });

    modal.querySelector('.review-modal__close').addEventListener('click', () => modal.remove());
    modal.querySelector('.review-modal__form').addEventListener('submit', async (e) => {
        e.preventDefault();
        const comment = modal.querySelector('.review-modal__textarea').value;
        if (rating && comment) {
            try {
                const reviewData = {
                    text: comment,
                    rating: rating,
                    gameId: gameId
                }
                const response = await fetch('/Review/Send', {
                    method: 'POST',
                    headers: {
                        'Content-Type': 'application/json'
                    },
                    body: JSON.stringify(reviewData)
                });

                const result = await response.json();
                if (response.ok && result.success) {
                    modal.remove();
                    alert('Отзыв отправлен, скоро он появится в списке.');
                }
                else {
                    alert(result.message || 'Ошибка при отправке отзыва');
                }
            }
            catch (error) {
                console.error('Ошибка:', error);
                alert('Произошла ошибка при отправке отзыва.');
            }
        } else {
            alert('Пожалуйста, выберите рейтинг и напишите комментарий.');
        }
    });
}

export function initReviewsCarousel() {
    const container = document.querySelector('.game-reviews__container');
    const reviews = document.querySelectorAll('.game-reviews__card');
    const prevButton = document.querySelector('.game-reviews__prev');
    const nextButton = document.querySelector('.game-reviews__next');
    const indicatorsContainer = document.querySelector('.game-reviews__indicators');

    if (!container || reviews.length === 0) return;

    let currentSlide = 0;
    let slidesToShow = getSlidesToShow();

    function getSlidesToShow() {
        if (window.innerWidth <= 768) return 1;
        return 3;
    }

    function getMaxSlide() {
        return Math.ceil(reviews.length / slidesToShow) - 1;
    }

    function updateCarousel() {
        const slideWidth = 100 / slidesToShow;
        const translateX = -currentSlide * slideWidth * slidesToShow;

        container.style.transform = `translateX(${translateX}%)`;
        container.style.transition = 'transform 0.5s ease-in-out';

        updateIndicators();
        updateButtons();
    }

    function updateIndicators() {
        if (!indicatorsContainer) return;

        const totalSlides = getMaxSlide() + 1;
        indicatorsContainer.innerHTML = '';

        for (let i = 0; i < totalSlides; i++) {
            const indicator = document.createElement('span');
            indicator.className = `game-reviews__indicator ${i === currentSlide ? 'active' : ''}`;
            indicator.addEventListener('click', () => goToSlide(i));
            indicatorsContainer.appendChild(indicator);
        }
    }

    function updateButtons() {
        const maxSlide = getMaxSlide();

        if (prevButton) {
            prevButton.disabled = currentSlide === 0;
            prevButton.style.opacity = currentSlide === 0 ? '0.5' : '1';
        }

        if (nextButton) {
            nextButton.disabled = currentSlide >= maxSlide;
            nextButton.style.opacity = currentSlide >= maxSlide ? '0.5' : '1';
        }
    }

    function goToSlide(slideIndex) {
        const maxSlide = getMaxSlide();
        currentSlide = Math.max(0, Math.min(slideIndex, maxSlide));
        updateCarousel();
    }

    function nextSlide() {
        const maxSlide = getMaxSlide();
        if (currentSlide < maxSlide) {
            currentSlide++;
            updateCarousel();
        }
    }

    function prevSlide() {
        if (currentSlide > 0) {
            currentSlide--;
            updateCarousel();
        }
    }

    // Функция для расчета ширины карточек с учетом gap
    function updateCardWidths() {
        const containerWidth = container.offsetWidth;
        let cardWidth;

        if (window.innerWidth <= 768) {
            // Для мобильных: 100% ширины
            cardWidth = `100%`;
        } else {
            // Для десктопов: 33.333% ширины минус две трети gap
            cardWidth = `calc(33.333% - 13.333px)`;
        }

        reviews.forEach(review => {
            review.style.flex = `0 0 ${cardWidth}`;
            review.style.maxWidth = cardWidth;
        });
    }

    // Инициализация
    function init() {
        slidesToShow = getSlidesToShow();
        const maxSlide = getMaxSlide();

        // Ограничиваем текущий слайд если он выходит за пределы
        currentSlide = Math.min(currentSlide, maxSlide);

        // Устанавливаем ширину карточек
        updateCardWidths();

        // Добавляем обработчики событий
        if (prevButton) prevButton.addEventListener('click', prevSlide);
        if (nextButton) nextButton.addEventListener('click', nextSlide);

        // Обработчик изменения размера окна
        window.addEventListener('resize', () => {
            const newSlidesToShow = getSlidesToShow();
            const newMaxSlide = Math.ceil(reviews.length / newSlidesToShow) - 1;

            if (newSlidesToShow !== slidesToShow) {
                slidesToShow = newSlidesToShow;
                // Ограничиваем текущий слайд новым максимумом
                currentSlide = Math.min(currentSlide, newMaxSlide);

                // Обновляем ширину карточек
                updateCardWidths();

                updateCarousel();
            }
        });

        // Инициализируем карусель
        updateCarousel();
    }

    init();
}

// Добавление отзыва в список
//export function addReviewToList(review) {
//    const reviewsList = document.querySelector('.game-reviews__container');
//    if (!reviewsList) return;

//    const reviewCard = document.createElement('article');
//    reviewCard.className = 'game-reviews__card';

//    // Определяем аватарку: если есть avatar в review, используем её, иначе ищем по имени
//    const avatarSrc = review.authorAvatarPath;

//    reviewCard.innerHTML = `
//        <div class="game-reviews__reviewer">
//            <img src="${avatarSrc}" alt="Reviewer" class="game-reviews__avatar">
//            <div class="game-reviews__reviewer-details">
//                <span class="game-reviews__reviewer-name">${review.authorName}</span>
//                <span class="game-reviews__reviewer-location">${review.gameTitle}</span>
//            </div>
//            <div class="game-reviews__rating">
//                <span class="game-reviews__rating-score">${review.rating}</span>
//                <img src="/icon/star-filled.svg" alt="Star" class="game-reviews__star">
//            </div>
//        </div>
//        <p class="game-reviews__text">${review.text}</p>
//    `;
//    reviewsList.insertBefore(reviewCard, reviewsList.firstChild);
//    updatePagination();
//}

