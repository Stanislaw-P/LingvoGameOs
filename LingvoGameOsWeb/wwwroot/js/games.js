
document.addEventListener('DOMContentLoaded', () => {
    // Карусель
    const prevButton = document.querySelector('.game-hero__carousel-prev');
    const nextButton = document.querySelector('.game-hero__carousel-next');
    const indicators = document.querySelectorAll('.game-hero__indicator');
    let currentSlide = 0;
    let autoSlideInterval;


    const startAutoSlide = () => {
        autoSlideInterval = setInterval(() => {
            goToSlide(currentSlide + 1);
        }, 5000);
    };

    const stopAutoSlide = () => {
        clearInterval(autoSlideInterval);
    };

    prevButton.addEventListener('click', () => {
        stopAutoSlide();
        goToSlide(currentSlide - 1);
        startAutoSlide();
    });

    nextButton.addEventListener('click', () => {
        stopAutoSlide();
        goToSlide(currentSlide + 1);
        startAutoSlide();
    });

    indicators.forEach((indicator, index) => {
        indicator.addEventListener('click', () => {
            stopAutoSlide();
            goToSlide(index);
            startAutoSlide();
        });
    });

    startAutoSlide();

    // Dropdowns
    const dropdownToggles = document.querySelectorAll('.games__dropdown-toggle');
    dropdownToggles.forEach(toggle => {
        toggle.addEventListener('click', () => {
            const menu = toggle.nextElementSibling;
            const isActive = menu.classList.contains('active');
            document.querySelectorAll('.games__dropdown-menu').forEach(m => m.classList.remove('active'));
            document.querySelectorAll('.games__dropdown-toggle').forEach(t => t.classList.remove('active'));
            if (!isActive) {
                menu.classList.add('active');
                toggle.classList.add('active');
            }
        });
    });

    // Закрытие dropdown при клике вне
    document.addEventListener('click', (e) => {
        if (!e.target.closest('.games__dropdown')) {
            document.querySelectorAll('.games__dropdown-menu').forEach(menu => menu.classList.remove('active'));
            document.querySelectorAll('.games__dropdown-toggle').forEach(toggle => toggle.classList.remove('active'));
        }
    });

    // Фильтрация
    const gameItems = document.querySelectorAll('.games__item');
    const dropdownItems = document.querySelectorAll('.games__dropdown-item');
    const searchInput = document.querySelector('.games__search-input');
    const notification = document.querySelector('.notification');
    const filters = JSON.parse(localStorage.getItem('gameFilters')) || {
        category: [],
        keyword: [],
        rating: []
    };

    const showNotification = (message, type = 'info') => {
        notification.textContent = message;
        notification.className = `notification notification--${type}`;
        notification.style.display = 'block';
        notification.style.opacity = '1';
        setTimeout(() => {
            notification.style.opacity = '0';
            setTimeout(() => {
                notification.style.display = 'none';
            }, 300);
        }, 3000);
    };

    const saveFilters = () => {
        localStorage.setItem('gameFilters', JSON.stringify(filters));
    };

    const applyFilters = () => {
        let visibleItems = 0;
        gameItems.forEach(item => {
            const categories = item.dataset.categories.split(',');
            const keywords = item.dataset.keywords.split(',');
            const rating = parseInt(item.dataset.rating);
            let isVisible = true;

            if (filters.category.length > 0 && !filters.category.some(cat => categories.includes(cat))) {
                isVisible = false;
            }
            if (filters.keyword.length > 0 && !filters.keyword.some(kw => keywords.includes(kw))) {
                isVisible = false;
            }
            if (filters.rating.length > 0 && !filters.rating.includes(rating.toString())) {
                isVisible = false;
            }
            if (searchInput.value && !item.querySelector('.games__name').textContent.toLowerCase().includes(searchInput.value.toLowerCase())) {
                isVisible = false;
            }

            item.style.display = isVisible ? 'flex' : 'none';
            if (isVisible) visibleItems++;
        });

        if (visibleItems === 0) {
            showNotification('Игры не найдены. Попробуйте изменить фильтры.', 'warning');
        }
    };

    dropdownItems.forEach(item => {
        if (filters[item.dataset.filter].includes(item.dataset.value)) {
            item.classList.add('selected');
        }
        item.addEventListener('click', () => {
            const filterType = item.dataset.filter;
            const value = item.dataset.value;
            const isSelected = item.classList.contains('selected');

            if (isSelected) {
                item.classList.remove('selected');
                filters[filterType] = filters[filterType].filter(v => v !== value);
                showNotification(`Фильтр ${filterType} удален: ${value}`, 'warning');
            } else {
                item.classList.add('selected');
                filters[filterType].push(value);
                showNotification(`Фильтр ${filterType} применен: ${value}`, 'success');
            }

            applyFilters();
            saveFilters();
        });
    });

    searchInput.addEventListener('input', applyFilters);
    applyFilters();

    // Поиск в dropdown
    const dropdownSearchInputs = document.querySelectorAll('.games__dropdown-search-input');
    dropdownSearchInputs.forEach(input => {
        input.addEventListener('input', () => {
            const list = input.closest('.games__dropdown-menu').querySelector('.games__dropdown-list');
            const items = list.querySelectorAll('.games__dropdown-item');
            const query = input.value.toLowerCase();

            items.forEach(item => {
                const text = item.textContent.toLowerCase();
                item.style.display = text.includes(query) ? 'flex' : 'none';
            });
        });
    });

    // Favorite button functionality
    const favoriteButtons = document.querySelectorAll('.game-hero__favorite');
    const favorites = JSON.parse(localStorage.getItem('favorites')) || [];
    const updateFavoriteIcons = () => {
        favoriteButtons.forEach(button => {
            const gameTitle = button.closest('.game-hero__slide').querySelector('.game-hero__slide-title').textContent;
            button.classList.toggle('filled', favorites.includes(gameTitle));
        });
        document.querySelectorAll('.favorite-toggle').forEach(icon => {
            const gameName = icon.closest('.games__item').querySelector('.games__name').textContent;
            icon.classList.toggle('filled', favorites.includes(gameName));
        });
    };

    favoriteButtons.forEach(button => {
        button.addEventListener('click', () => {
            const gameTitle = button.closest('.game-hero__slide').querySelector('.game-hero__slide-title').textContent;
            const index = favorites.indexOf(gameTitle);
            if (index === -1) {
                favorites.push(gameTitle);
                showNotification(`Добавлено в избранное: ${gameTitle}`, 'success');
            } else {
                favorites.splice(index, 1);
                showNotification(`Удалено из избранного: ${gameTitle}`, 'warning');
            }
            localStorage.setItem('favorites', JSON.stringify(favorites));
            updateFavoriteIcons();
        });
    });

    document.querySelectorAll('.favorite-toggle').forEach(icon => {
        icon.addEventListener('click', () => {
            const gameItem = icon.closest('.games__item');
            const gameName = gameItem.querySelector('.games__name').textContent;
            const index = favorites.indexOf(gameName);
            if (index === -1) {
                favorites.push(gameName);
                showNotification(`Добавлено в избранное: ${gameName}`, 'success');
            } else {
                favorites.splice(index, 1);
                showNotification(`Удалено из избранного: ${gameName}`, 'warning');
            }
            localStorage.setItem('favorites', JSON.stringify(favorites));
            updateFavoriteIcons();
        });
    });

    updateFavoriteIcons();

    // Модальное окно для предпросмотра
    const modal = document.querySelector('#game-preview-modal');
    const modalContent = modal.querySelector('.modal__content');
    document.addEventListener('click', (e) => {
        if (e.target.classList.contains('games__preview') && e.target.closest('.games__item')) {
            console.log('Обработка кнопки предпросмотра:', e.target, 'Родительский games__item:', e.target.closest('.games__item')); // Для отладки
            const gameItem = e.target.closest('.games__item');
            const gameNameElement = gameItem.querySelector('.games__name');
            if (!gameNameElement) {
                console.error('Не удалось найти .games__name для кнопки предпросмотра:', e.target);
                return;
            }
            const gameName = gameNameElement.textContent;
            const gameImage = gameItem.querySelector('.games__image').src;
            const gameRating = gameItem.dataset.rating;
            const gameLikes = gameItem.querySelector('.games__likes span').textContent;

            modalContent.querySelector('.modal__title').textContent = gameName;
            modalContent.querySelector('.modal__image').src = gameImage;
            modalContent.querySelector('.modal__description').textContent = `Изучайте ${gameName.toLowerCase()} через интерактивные задания и погружение в культуру.`;
            modalContent.querySelector('.modal__rating span').textContent = gameRating;
            modalContent.querySelector('.modal__likes span').textContent = gameLikes;

            modal.classList.add('active');
            document.body.style.overflow = 'hidden';
        }
    });

    modal.querySelector('.modal__close').addEventListener('click', () => {
        modal.classList.remove('active');
        document.body.style.overflow = '';
    });

    modal.addEventListener('click', (e) => {
        if (e.target === modal) {
            modal.classList.remove('active');
            document.body.style.overflow = '';
        }
    });

    // Пагинация
    const gamesList = document.querySelector('.games__list');
    const itemsPerPage = 6;
    let currentPage = 1;

    const paginateGames = () => {
        const visibleItems = Array.from(gameItems).filter(item => item.style.display !== 'none');
        const totalPages = Math.ceil(visibleItems.length / itemsPerPage);

        visibleItems.forEach((item, index) => {
            item.style.display = (index >= (currentPage - 1) * itemsPerPage && index < currentPage * itemsPerPage) ? 'flex' : 'none';
        });

        updatePaginationControls(totalPages);
    };

    const updatePaginationControls = (totalPages) => {
        let pagination = document.querySelector('.games__pagination');
        if (!pagination) {
            pagination = document.createElement('div');
            pagination.className = 'games__pagination';
            gamesList.insertAdjacentElement('afterend', pagination);
        }

        pagination.innerHTML = `
            <button class="pagination__prev" ${currentPage === 1 ? 'disabled' : ''}>Предыдущая</button>
            <span class="pagination__info">Страница ${currentPage} из ${totalPages}</span>
            <button class="pagination__next" ${currentPage === totalPages ? 'disabled' : ''}>Следующая</button>
        `;

        pagination.querySelector('.pagination__prev').addEventListener('click', () => {
            if (currentPage > 1) {
                currentPage--;
                paginateGames();
            }
        });

        pagination.querySelector('.pagination__next').addEventListener('click', () => {
            if (currentPage < totalPages) {
                currentPage++;
                paginateGames();
            }
        });
    };

    searchInput.addEventListener('input', () => {
        currentPage = 1;
        applyFilters();
        paginateGames();
    });

    dropdownItems.forEach(item => {
        item.addEventListener('click', () => {
            currentPage = 1;
            applyFilters();
            paginateGames();
        });
    });

    paginateGames();
});

