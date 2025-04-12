// Функция для загрузки HTML-компонентов
function loadComponent(url, placeholderId) {
    fetch(url)
        .then(response => response.text())
        .then(data => {
            document.getElementById(placeholderId).innerHTML = data;
            if (placeholderId === 'header-placeholder') {
                initializeDropdown();
                initializeFavoritesLink();
            }
        })
        .catch(error => console.error('Ошибка при загрузке компонента:', error));
}

// Инициализация выпадающего меню профиля
function initializeDropdown() {
    const toggle = document.querySelector('.user-profile__toggle');
    const menu = document.querySelector('.user-profile__menu');
    const menuItems = document.querySelectorAll('.user-profile__menu-item');

    if (!toggle || !menu) return;

    toggle.addEventListener('click', (e) => {
        e.preventDefault();
        menu.classList.toggle('active');
    });

    document.addEventListener('click', (e) => {
        if (!toggle.contains(e.target) && !menu.contains(e.target)) {
            menu.classList.remove('active');
        }
    });

    menuItems.forEach(item => {
        const link = item.querySelector('a');
        const targetClass = item.getAttribute('data-target');

        if (link && targetClass) {
            link.addEventListener('click', (e) => {
                e.preventDefault();
                const currentPage = window.location.pathname.split('/').pop() || 'index.html';
                const targetPage = link.getAttribute('href').split('/').pop();

                if (currentPage === targetPage) {
                    const targetElement = document.querySelector(`.${targetClass}`);
                    if (targetElement) {
                        targetElement.scrollIntoView({ behavior: 'smooth', block: 'start' });
                    }
                } else {
                    window.location.href = `${link.getAttribute('href')}#${targetClass}`;
                }
                menu.classList.remove('active');
            });
        }
    });
}

// Инициализация кнопки "Избранное" в шапке
function initializeFavoritesLink() {
    const favoritesButton = document.querySelector('.header__like');
    if (favoritesButton) {
        favoritesButton.addEventListener('click', () => {
            window.location.href = 'favorites.html';
        });
    }
}

// Примерные данные для избранных игр
const sampleFavorites = [
    { id: 'game1', name: 'Сокровища Осетии', image: 'img/game1.png' },
    { id: 'game2', name: 'Словарный Бой', image: 'img/game1.png' },
    { id: 'game3', name: 'Грамматический Штурм', image: 'img/game2.png' }
];

// Управление избранными играми
let favorites = JSON.parse(localStorage.getItem('favorites')) || sampleFavorites;

function toggleFavorite(gameId, gameData) {
    const index = favorites.findIndex(item => item.id === gameId);
    const likeButton = document.querySelector('.header__like img');

    if (index === -1) {
        favorites.push({ id: gameId, ...gameData });
        if (likeButton) likeButton.src = 'icon/like-filled.svg';
    } else {
        favorites.splice(index, 1);
        if (likeButton) likeButton.src = 'icon/like.svg';
    }

    localStorage.setItem('favorites', JSON.stringify(favorites));
    updateFavoritesPage();
}

// Обновление страницы избранного
function updateFavoritesPage() {
    const favoritesList = document.getElementById('favorites-list');
    const emptyMessage = document.getElementById('favorites-empty');
    if (!favoritesList || !emptyMessage) return;

    favoritesList.innerHTML = '';
    emptyMessage.classList.toggle('active', favorites.length === 0);

    favorites.forEach(game => {
        const favoriteItem = document.createElement('div');
        favoriteItem.className = 'recommended-games__item';
        favoriteItem.innerHTML = `
            <img src="${game.image}" alt="${game.name}" class="recommended-games__image">
            <div class="recommended-games__content">
                <h4 class="recommended-games__name">${game.name}</h4>
                <div class="recommended-games__stats">
                    <span class="recommended-games__rating">
                        <img src="icon/shape2.svg" alt="star" class="rating-star filled">
                        <img src="icon/shape2.svg" alt="star" class="rating-star filled">
                        <img src="icon/shape2.svg" alt="star" class="rating-star filled">
                        <img src="icon/shape2.svg" alt="star" class="rating-star filled">
                        <img src="icon/shape.svg" alt="star" class="rating-star"> 4.5
                    </span>
                    <span class="recommended-games__likes">
                        <img src="icon/like2.svg" alt="like" class="stat-icon"> 1.5K
                    </span>
                    <span class="recommended-games__views">
                        <img src="icon/eye.svg" alt="view" class="stat-icon"> 2.5K
                    </span>
                </div>
            </div>
        `;
        favoritesList.appendChild(favoriteItem);
    });
}

// Отображение модального окна для отзыва
function showReviewModal() {
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
            addReviewToList({ name: 'Вы', location: 'Ваше местоположение', rating, text: comment });
            modal.remove();
        } else {
            alert('Пожалуйста, выберите рейтинг и напишите комментарий.');
        }
    });
}

// Добавление отзыва в список
function addReviewToList(review) {
    const reviewsList = document.querySelector('.game-reviews__container');
    if (!reviewsList) return;

    const reviewCard = document.createElement('article');
    reviewCard.className = 'game-reviews__card';
    reviewCard.innerHTML = `
        <div class="game-reviews__reviewer">
            <img src="img/avatar.png" alt="Reviewer" class="game-reviews__avatar">
            <div class="game-reviews__reviewer-details">
                <span class="game-reviews__reviewer-name">${review.name}</span>
                <span class="game-reviews__reviewer-location">${review.location}</span>
            </div>
            <div class="game-reviews__rating">
                <span class="game-reviews__rating-score">${review.rating}</span>
                <img src="icon/star-filled.svg" alt="Star" class="game-reviews__star">
            </div>
        </div>
        <p class="game-reviews__text">${review.text}</p>
    `;
    reviewsList.insertBefore(reviewCard, reviewsList.firstChild);
    updatePagination();
}

// Примерные отзывы
const sampleReviews = [
    { name: "Алан Д.", location: "Владикавказ, Россия", rating: "4.5", text: "Игра просто потрясающая! Открывать сундуки и учить осетинский язык одновременно — это гениально. Иногда задания кажутся сложноватыми, но это только подогревает интерес!" },
    { name: "Мария К.", location: "Москва, Россия", rating: "5", text: "Очень увлекательная игра, особенно для тех, кто хочет погрузиться в культуру Осетии. Графика приятная, а головоломки заставляют думать. Рекомендую всем!" },
    { name: "Тимур Б.", location: "Тбилиси, Грузия", rating: "4", text: "Интересный способ учить язык через игру. Понравилось, как вплетены элементы истории Осетии. Хотелось бы больше уровней и разнообразия в артефактах." },
    { name: "Елена С.", location: "Санкт-Петербург, Россия", rating: "4.5", text: "Отличная идея — бегать по горам и решать лингвистические задачки. Иногда интерфейс немного тормозит, но в целом впечатления супер!" },
    { name: "Заур Т.", location: "Нальчик, Россия", rating: "5", text: "Игра затягивает с первых минут! Особенно круто узнавать про нартские сказания и осетинскую культуру. Спасибо разработчикам за такой проект!" }
];

// Инициализация отзывов
function initializeReviews() {
    const container = document.querySelector('.game-reviews__container');
    if (!container) return;

    container.innerHTML = '';
    sampleReviews.forEach(review => addReviewToList(review));
    updatePagination();
}

// Обновление пагинации отзывов
function updatePagination() {
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

// Инициализация таблицы лидеров с улучшенной разметкой
function initializeLeaderboard() {
    const leaderboardList = document.querySelector('.leaderboard__list');
    if (!leaderboardList) return;

    const sampleLeaderboard = [
        { rank: 1, name: "Алан Д.", score: 2500, avatar: "img/avatar1.png" },
        { rank: 2, name: "Мария К.", score: 2300, avatar: "img/avatar2.png" },
        { rank: 3, name: "Тимур Б.", score: 2100, avatar: "img/avatar1.png" },
        { rank: 4, name: "Елена С.", score: 1900, avatar: "img/avatar2.png" },
        { rank: 5, name: "Заур Т.", score: 1800, avatar: "img/avatar1.png" },
        { rank: 6, name: "Игорь П.", score: 1700, avatar: "img/avatar2.png" },
        { rank: 7, name: "Ольга М.", score: 1600, avatar: "img/avatar1.png" },
        { rank: 8, name: "Дмитрий К.", score: 1500, avatar: "img/avatar2.png" },
        { rank: 9, name: "Анна В.", score: 1400, avatar: "img/avatar2.png" },
        { rank: 10, name: "Сергей Н.", score: 1300, avatar: "img/avatar1.png" }
    ];

    leaderboardList.innerHTML = ''; // Очищаем список перед заполнением

    sampleLeaderboard.forEach(player => {
        const leaderboardItem = document.createElement('div');
        leaderboardItem.className = 'leaderboard__item';
        leaderboardItem.innerHTML = `
            <span class="leaderboard__rank">${player.rank}</span>
            <div class="leaderboard__name">
                <img src="${player.avatar}" alt="${player.name}" class="leaderboard__avatar">
                <span>${player.name}</span>
            </div>
            <span class="leaderboard__score">${player.score}</span>
        `;
        leaderboardList.appendChild(leaderboardItem);
    });
}

// Основная инициализация страницы
document.addEventListener('DOMContentLoaded', () => {
    loadComponent('header.html', 'header-placeholder');
    loadComponent('footer.html', 'footer-placeholder');

    const emailForm = document.querySelector('.email-form-container');
    if (emailForm) {
        emailForm.addEventListener('submit', (e) => {
            e.preventDefault();
            const button = emailForm.querySelector('.email-button');
            button.textContent = 'Отправка...';
            setTimeout(() => {
                button.textContent = 'Подписаться';
                showNotification('Вы успешно подписались!');
                emailForm.querySelector('.email-prompt-text-style').value = '';
            }, 1000);
        });
    }

    const playButton = document.querySelector('.game-hero__button');
    if (playButton) playButton.addEventListener('click', () => alert('Игра запускается! (Функционал в разработке)'));

    const reviewButton = document.querySelector('.game-reviews__button');
    if (reviewButton) reviewButton.addEventListener('click', showReviewModal);

    if (document.querySelector('.game-reviews__container')) initializeReviews();

    const gameLikeButton = document.querySelector('.header__like');
    if (gameLikeButton && window.location.pathname.includes('game.html')) {
        const gameData = { id: 'game1', name: 'Сокровища Осетии', image: 'img/game-banner.png' };
        gameLikeButton.addEventListener('click', () => toggleFavorite(gameData.id, gameData));
        if (favorites.some(game => game.id === gameData.id)) gameLikeButton.querySelector('img').src = 'icon/like-filled.svg';
    }

    if (window.location.pathname.includes('favorites.html')) updateFavoritesPage();

    if (window.location.pathname.includes('leaderboard_game-id.html')) initializeLeaderboard();

    initializeActivityChart();
});

// Генерация графика активности
function initializeActivityChart() {
    const chartGrid = document.querySelector('.activity__chart-grid');
    if (!chartGrid) return;

    for (let i = 0; i < 365; i++) {
        const cell = document.createElement('div');
        cell.className = 'activity__chart-cell';
        if (i % 10 === 0) cell.classList.add('active');
        chartGrid.appendChild(cell);
    }
}

// Показ уведомлений
function showNotification(message) {
    const notification = document.createElement('div');
    notification.className = 'notification';
    notification.textContent = message;
    document.body.appendChild(notification);
    setTimeout(() => notification.remove(), 3000);
}