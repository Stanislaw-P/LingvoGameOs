import { loadComponent } from './loadComponents.js';
import { showReviewModal, initializeReviews } from './reviews.js';
import { toggleFavorite, updateFavoritesPage, getFavorites } from './favorites.js';
import { initializeLeaderboard } from './leaderboard.js';
import { initializeActivityChart } from './activityChart.js';
import { showNotification } from './notifications.js';
import { initializeSlider } from './slider.js';
import { initializeProfileModal } from './profileModal.js';
import { initializeGameBlocks } from './gameBlocks.js';
import { initializeCarousel } from './carousel.js';

// Основная инициализация страницы
document.addEventListener('DOMContentLoaded', () => {
    console.log('DOM полностью загружен, инициализация начинается');

    // Загрузка компонентов header и footer
    loadComponent('components/header.html', 'header-placeholder');
    loadComponent('components/footer.html', 'footer-placeholder');

    // Обработка формы подписки на email
    const emailForm = document.querySelector('.email-form-container');
    if (emailForm) {
        emailForm.addEventListener('submit', async (e) => {
            e.preventDefault();
            const button = emailForm.querySelector('.email-button');
            const input = emailForm.querySelector('.email-prompt-text-style');
            if (!button || !input) {
                console.warn('Кнопка или поле ввода email не найдены');
                return;
            }
            if (!input.value.match(/^[^\s@]+@[^\s@]+\.[^\s@]+$/)) {
                showNotification('Пожалуйста, введите корректный email', 'error');
                return;
            }
            button.textContent = 'Отправка...';
            try {
                await new Promise(resolve => setTimeout(resolve, 1000));
                button.textContent = 'Подписаться';
                showNotification('Вы успешно подписались!');
                input.value = '';
            } catch (error) {
                button.textContent = 'Подписаться';
                showNotification('Ошибка при подписке. Попробуйте снова.', 'error');
            }
        });
    }

    //const playButton = document.querySelector('.game-hero__button');
    //if (playButton) playButton.addEventListener('click', () => alert('Игра запускается! (Функционал в разработке)'));

    const reviewButton = document.querySelector('.game-reviews__button');
    if (reviewButton) reviewButton.addEventListener('click', function () {
        // Получаем gameId из data атрибута
        const gameId = this.getAttribute('data-game-id');
        showReviewModal(gameId);
    });

    if (document.querySelector('.game-reviews__container')) initializeReviews();

    const gameLikeButton = document.querySelector('.header__like');
    if (gameLikeButton && window.location.pathname.includes('game.html')) {
        const gameData = { id: 'game1', name: 'Сокровища Осетии', image: 'img/game-banner.png' };
        gameLikeButton.addEventListener('click', () => toggleFavorite(gameData.id, gameData));
        if (getFavorites().some(game => game.id === gameData.id)) gameLikeButton.querySelector('img').src = 'icon/like-filled.svg';
    }

    // Обновление страницы избранного
    if (window.location.pathname.includes('favorites.html')) {
        updateFavoritesPage();
    }

    // Инициализация таблицы лидеров
    if (window.location.pathname.includes('leaderboard_game-id.html')) {
        initializeLeaderboard();
    }

    // Инициализация графика активности
    if (document.querySelector('.activity__chart')) {
        initializeActivityChart();
    }

    // Инициализация слайдера на странице игры
    if (document.querySelector('.game-hero__carousel')) {
        initializeSlider();
    }

    // Инициализация модального окна профиля
    const profileModal = document.getElementById('edit-profile-modal');
    if (profileModal) {
        console.log('Инициализация модального окна профиля');
        try {
            initializeProfileModal('/img/avatar100.png');
        } catch (error) {
            console.error('Ошибка при инициализации модального окна:', error);
        }
    } else {
        console.warn('Модальное окно профиля (#edit-profile-modal) не найдено');
    }

    // Инициализация блоков с играми
    if (document.querySelector('.user-added-games') || document.querySelector('.recent-games')) {
        console.log('Инициализация блоков с играми');
        try {
            initializeGameBlocks();
        } catch (error) {
            console.error('Ошибка при инициализации блоков с играми:', error);
        }
    }

    // Инициализация карусели для блоков с играми
    if (document.querySelector('.user-added-games__carousel') || document.querySelector('.recent-games__carousel')) {
        console.log('Инициализация карусели для блоков с играми');
        try {
            initializeCarousel();
        } catch (error) {
            console.error('Ошибка при инициализации карусели:', error);
        }
    }
});