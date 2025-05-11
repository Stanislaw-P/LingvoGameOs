// Примерные данные для избранных игр
const sampleFavorites = [
    { id: 'game1', name: 'Сокровища Осетии', image: '/img/game1.png' },
    { id: 'game2', name: 'Словарный Бой', image: '/img/game1.png' },
    { id: 'game3', name: 'Грамматический Штурм', image: '/img/game2.png' }
];

// Управление избранными играми
let favorites = JSON.parse(localStorage.getItem('favorites')) || sampleFavorites;

// Инициализация кнопки "Избранное" в шапке
export function initializeFavoritesLink() {
    const favoritesButton = document.querySelector('.header__like');
    if (favoritesButton) {
        favoritesButton.addEventListener('click', () => {
            window.location.href = 'favorites.html';
        });
    }
}

export function toggleFavorite(gameId, gameData) {
    const index = favorites.findIndex(item => item.id === gameId);
    const likeButton = document.querySelector('.header__like img');

    if (index === -1) {
        favorites.push({ id: gameId, ...gameData });
        if (likeButton) likeButton.src = '/icon/like-filled.svg';
    } else {
        favorites.splice(index, 1);
        if (likeButton) likeButton.src = '/icon/like.svg';
    }

    localStorage.setItem('favorites', JSON.stringify(favorites));
    updateFavoritesPage();
}

// Обновление страницы избранного
export function updateFavoritesPage() {
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
                        <img src="/icon/shape2.svg" alt="star" class="rating-star filled">
                        <img src="/icon/shape2.svg" alt="star" class="rating-star filled">
                        <img src="/icon/shape2.svg" alt="star" class="rating-star filled">
                        <img src="/icon/shape2.svg" alt="star" class="rating-star filled">
                        <img src="/icon/shape.svg" alt="star" class="rating-star"> 4.5
                    </span>
                    <span class="recommended-games__likes">
                        <img src="icon/like2.svg" alt="like" class="stat-icon"> 1.5K
                    </span>
                    <span class="recommended-games__views">
                        <img src="/icon/eye.svg" alt="view" class="stat-icon"> 2.5K
                    </span>
                </div>
            </div>
        `;
        favoritesList.appendChild(favoriteItem);
    });
}

export function getFavorites() {
    return favorites;
}