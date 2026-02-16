// Используем один глобальный обработчик клика
document.addEventListener('click', async function (event) {
    // Ищем, был ли клик по кнопке избранного (или по иконке внутри неё)
    const favoriteButton = event.target.closest('.games__favorite-btn, .game-hero__favorite');

    // Если клик не по кнопке ИЛИ это ссылка (для неавторизованных), ничего не делаем
    if (!favoriteButton || favoriteButton.tagName === 'A') return;

    // Предотвращаем стандартное поведение (если нужно)
    event.preventDefault();

    const isFavoritesPage = window.location.pathname.includes('Favorites');
    const isHeroButton = favoriteButton.classList.contains('game-hero__favorite');

    const gameId = favoriteButton.dataset.gameId;
    const isFavorite = favoriteButton.classList.contains('active');
    const icon = favoriteButton.querySelector('.games__favorite-icon, .game-hero__favorite-icon');
    const gameCard = favoriteButton.closest('.games__item');

    const fetchUrl = isFavorite ? `/Favorites/Remove?gameId=${gameId}` : `/Favorites/Add?gameId=${gameId}`;

    try {
        const response = await fetch(fetchUrl, {
            method: isFavorite ? 'DELETE' : 'POST',
            headers: { 'Content-Type': 'application/json' },
        });

        if (response.ok) {
            if (isFavorite && isFavoritesPage && !isHeroButton && gameCard) {
                // Анимация удаления на странице избранного
                gameCard.style.transition = 'all 0.3s ease';
                gameCard.style.opacity = '0';
                gameCard.style.transform = 'translateY(20px)';

                setTimeout(() => {
                    gameCard.remove();
                    updateGamesLayout();
                }, 300);
            } else {
                // Переключение состояния (активно/неактивно)
                favoriteButton.classList.toggle('active');
                const nowActive = favoriteButton.classList.contains('active');

                if (icon) {
                    if (isHeroButton) {
                        icon.src = nowActive ? '/icon/like3.svg' : '/icon/like.svg';
                    } else {
                        icon.src = nowActive ? '/icon/like2.svg' : '/icon/like.svg';
                    }
                }
            }
        } else {
            const error = await response.text();
            console.error('Ошибка сервера: ', error);
            alert(`Ошибка сохранения: ${error}`);
        }
    } catch (error) {
        console.error('Ошибка сети:', error);
    }
});

// Функцию выносим наружу, чтобы она была доступна
function updateGamesLayout() {
    const gamesContainer = document.getElementById('all-games');
    if (!gamesContainer) return;

    const remainingGames = gamesContainer.querySelectorAll('.games__item');
    if (remainingGames.length === 0) {
        gamesContainer.innerHTML = `
            <div id="no-favorites-message" class="no-favorites-message">
                <h4 class="games__title">У вас ещё нет избранных игр :(</h4>
                <a role="button" href="/Home/Games" class="games__search-button">К списку всех игр</a>
            </div>
        `;
    }
}