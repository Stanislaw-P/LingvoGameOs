// Доступные аватарки с именами
const avatarMapping = {
    'Дэвид': '/Avatars/DavaAva.jpg',
    'Владислав': '/Avatars/VslavAva.jpg',
    'Марат': '/Avatars/MaraAva.jpg',

};

// Функция для получения аватарки по имени
function getAvatarByName(name) {
    return avatarMapping[name] || '/Avatars/AvaNone.jpg'; // fallback
}

// Функция для получения случайной аватарки (для новых отзывов) ВЫКИНУТЬ!!!
function getRandomAvatar() {
    const avatars = Object.values(avatarMapping);
    return avatars[Math.floor(Math.random() * avatars.length)];
}

// Примерные отзывы (без аватарок, они будут добавляться динамически)
const sampleReviewsData = [
    { name: "Дэвид", location: "Мир по кусочкам", rating: "4", text: "Игра прикольная! Как интересно собирается картинка, мне интересно как разработчику. Иногда задания кажутся сложноватыми, но это только подогревает интерес!" },
    { name: "Владислав", location: "Ребусы на осетинском", rating: "4", text: "Очень сложная игра, подсказки тоже не помогли. Хоть головоломки и заставляют думать, но мне в нее пока играть рано" },
    {
        name: "Марат", location: "Кроссворд", rating: "5", text: "мне понравилось, что игры разделены на категории и есть подробности об игре. При этом оформление очень красивое, я как пользователь и как разработчик кайфую. Самое моё любимое это прозвища в профиле, которые за баллы улучшаются." },
];

// Отображение модального окна для отзыва
export function showReviewModal() {
    alert("Функция добавления отзывов находится в стадии разработки. Ждите обновлений!");
    /*
    const modal = document.createElement('div');
    modal.className = 'review-modal';
    modal.innerHTML = `
        <div class="review-modal__content">
            <h2 class="review-modal__title">Ваш отзыв об игре</h2>
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
    modal.querySelector('.review-modal__form').addEventListener('submit', (e) => {
        e.preventDefault();
        const comment = modal.querySelector('.review-modal__textarea').value;
        if (rating && comment) {
            addReviewToList({ 
                name: 'Вы', 
                location: 'Ваше местоположение', 
                rating, 
                text: comment,
                avatar: getRandomAvatar() // Используем случайную аватарку для новых отзывов
            });
            modal.remove();
        } else {
            alert('Пожалуйста, выберите рейтинг и напишите комментарий.');
        }
    });
    */
}

// Добавление отзыва в список
export function addReviewToList(review) {
    const reviewsList = document.querySelector('.game-reviews__container');
    if (!reviewsList) return;

    const reviewCard = document.createElement('article');
    reviewCard.className = 'game-reviews__card';
    
    // Определяем аватарку: если есть avatar в review, используем её, иначе ищем по имени
    const avatarSrc = review.avatar || getAvatarByName(review.name);
    
    reviewCard.innerHTML = `
        <div class="game-reviews__reviewer">
            <img src="${avatarSrc}" alt="Reviewer" class="game-reviews__avatar">
            <div class="game-reviews__reviewer-details">
                <span class="game-reviews__reviewer-name">${review.name}</span>
                <span class="game-reviews__reviewer-location">${review.location}</span>
            </div>
            <div class="game-reviews__rating">
                <span class="game-reviews__rating-score">${review.rating}</span>
                <img src="/icon/star-filled.svg" alt="Star" class="game-reviews__star">
            </div>
        </div>
        <p class="game-reviews__text">${review.text}</p>
    `;
    reviewsList.insertBefore(reviewCard, reviewsList.firstChild);
    updatePagination();
}

// Инициализация отзывов
export function initializeReviews() {
    const container = document.querySelector('.game-reviews__container');
    if (!container) return;

    container.innerHTML = '';
    
    // Создаем отзывы с соответствующими аватарками
    const reviewsWithAvatars = sampleReviewsData.map(review => ({
        ...review,
        avatar: getAvatarByName(review.name)
    }));
    
    reviewsWithAvatars.forEach(review => addReviewToList(review));
    updatePagination();
}

// Обновление пагинации отзывов
export function updatePagination() {
    const reviews = document.querySelectorAll('.game-reviews__card');
    const itemsPerPage = 3;
    let currentPage = 1;
    const totalPages = Math.ceil(reviews.length / itemsPerPage);

    function showPage(page) {
        reviews.forEach((review, index) => {
            review.style.display = (index >= (page - 1) * itemsPerPage && index < page * itemsPerPage) ? 'block' : 'none';
        });

        const indicators = document.querySelector('.game-reviews__indicators');
        if (indicators) {
            indicators.innerHTML = '';
            for (let i = 1; i <= totalPages; i++) {
                const indicator = document.createElement('span');
                indicator.className = `game-reviews__indicator ${i === page ? 'active' : ''}`;
                indicators.appendChild(indicator);
            }
        }
    }

    const prevButton = document.querySelector('.game-reviews__prev');
    const nextButton = document.querySelector('.game-reviews__next');

    if (prevButton) prevButton.addEventListener('click', () => {
        if (currentPage > 1) showPage(--currentPage);
    });

    if (nextButton) nextButton.addEventListener('click', () => {
        if (currentPage < totalPages) showPage(++currentPage);
    });

    showPage(currentPage);
}