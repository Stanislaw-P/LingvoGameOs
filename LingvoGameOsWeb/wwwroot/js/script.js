import { loadComponent } from './loadComponents.js';
import { showReviewModal } from './reviews.js';
import { initializeReviews } from './reviews.js';
import { toggleFavorite, updateFavoritesPage, getFavorites } from './favorites.js';
import { initializeLeaderboard } from './leaderboard.js';
import { initializeActivityChart } from './activityChart.js';
import { showNotification } from './notifications.js';
import { initializeSlider } from './slider.js';

// Основная инициализация страницы
document.addEventListener('DOMContentLoaded', () => {
    loadComponent('components/header.html', 'header-placeholder');
    loadComponent('components/footer.html', 'footer-placeholder');

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
        if (getFavorites().some(game => game.id === gameData.id)) gameLikeButton.querySelector('img').src = 'icon/like-filled.svg';
    }

    if (window.location.pathname.includes('favorites.html')) updateFavoritesPage();

    if (window.location.pathname.includes('leaderboard_game-id.html')) initializeLeaderboard();

    initializeActivityChart();

    // Инициализация слайдера на странице деталей игры
    if (document.querySelector('.game-hero__carousel')) {
        initializeSlider();
    }
});