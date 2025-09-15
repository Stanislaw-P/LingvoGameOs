document.addEventListener('DOMContentLoaded', function () {
    const favoriteButtons = document.querySelectorAll('.games__favorite-btn, .game-hero__favorite');

    favoriteButtons.forEach(favoriteButton => {
        favoriteButton.addEventListener('click', async function () {
            const isFavoritesPage = window.location.pathname.includes('Favorites');
            const isHeroButton = this.classList.contains('game-hero__favorite');

            const gameId = this.dataset.gameId;
            const isFavorite = this.classList.contains('active');
            const icon = this.querySelector('.games__favorite-icon, .game-hero__favorite-icon');
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
                    // If delete from favorites page - delete game-cart
                    if (isFavorite) {
                        if (isFavoritesPage && !isHeroButton) {
                            // Fade animation only for cards on favorites page
                            gameCard.style.transition = 'all 0.3s ease';
                            gameCard.style.opacity = '0';
                            gameCard.style.transform = 'translateY(20px)';

                            // Delete after animation
                            setTimeout(() => {
                                gameCard.remove();
                                updateGamesLayout();
                            }, 300);
                        } else {
                            // For hero buttons or not on the favorites page - just change the style
                            this.classList.toggle('active');
                            if (icon) {
                                if (isHeroButton) {
                                    icon.src = this.classList.contains('active') ?
                                        '/icon/like3.svg' :
                                        '/icon/like.svg';
                                } else {
                                    icon.src = this.classList.contains('active') ?
                                        '/icon/like2.svg' :
                                        '/icon/like.svg';
                                }
                            }
                        }
                    } else {
                        // Если добавляем в избранное - меняем стиль
                        this.classList.toggle('active');
                        if (icon) {
                            // Для hero кнопки меняем на like3.svg при сохранении
                            if (isHeroButton) {
                                icon.src = this.classList.contains('active') ?
                                    '/icon/like3.svg' :
                                    '/icon/like.svg';
                            } else {
                                icon.src = this.classList.contains('active') ?
                                    '/icon/like2.svg' :
                                    '/icon/like.svg';
                            }
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
        if (!gamesContainer) return; // Если контейнера нет (например, на странице hero), выходим

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