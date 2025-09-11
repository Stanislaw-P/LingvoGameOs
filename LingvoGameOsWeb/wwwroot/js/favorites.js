document.addEventListener('DOMContentLoaded', function () {
    const favoriteButtons = document.querySelectorAll('.games__favorite-btn');

    favoriteButtons.forEach(favoriteButton => {
        favoriteButton.addEventListener('click', async function () {
            const isFavoritesPage = window.location.pathname.includes('Favorites');

            const gameId = this.dataset.gameId;
            const isFavorite = this.classList.contains('active');
            const icon = this.querySelector('.games__favorite-icon');
            const gameCard = this.closest('.games__item'); // Находим карточку игры
            const fetchUrl = isFavorite ? `/Favorites/Remove?gameId=${gameId}` : `/Favorites/Add?gameId=${gameId}`

            try {
                const response = await fetch(fetchUrl, {
                    method: isFavorite ? 'DELETE' : 'POST',
                    headers: {
                        'Content-Type': 'application/json'
                    },
                });

                if (response.ok) {
                    // Если удаляем из избранного - удаляем карточку
                    if (isFavorite) {
                        if (isFavoritesPage) {
                            // Анимация исчезновения
                            gameCard.style.transition = 'all 0.3s ease';
                            gameCard.style.opacity = '0';
                            gameCard.style.transform = 'translateY(20px)';

                            // Удаляем после анимации
                            setTimeout(() => {
                                gameCard.remove();

                                // Обновляем layout если нужно
                                updateGamesLayout();
                            }, 300);
                        } else {
                            // Если добавляем в избранное - просто меняем стиль
                            this.classList.toggle('active');
                            // Меняем иконку
                            if (icon) {
                                icon.src = this.classList.contains('active') ?
                                    '/icon/like2.svg' :
                                    '/icon/like.svg';
                            }
                        }
                    } else {

                        // Если добавляем в избранное - просто меняем стиль
                        this.classList.toggle('active');
                        // Меняем иконку
                        if (icon) {
                            icon.src = this.classList.contains('active') ?
                                '/icon/like2.svg' :
                                '/icon/like.svg';
                        }
                    }
                } else {
                    const error = await response.text();
                    console.log('Ошибка сервера: ', error);
                    alert(`Ошибка сохранения: ${error}`);
                }
            }
            catch (error) {
                console.error('Ошибка сети:', error);
            }
        });
    });

    function updateGamesLayout() {
        const gamesContainer = document.getElementById('all-games');
        const remainingGames = gamesContainer.querySelectorAll('.games__item');

        if (remainingGames.length === 0) {
            // Если карточек не осталось, показываем сообщение
            gamesContainer.innerHTML = `
                <div id="no-favorites-message" class="no-favorites-message">
                    <h4 class="games__title">У вас ещё нет избранных игр :(</h4>
                    <a role="button" href="/Home/Games" class="games__search-button">К списку всех игр</a>
                </div>
            `;
        }
    }
});