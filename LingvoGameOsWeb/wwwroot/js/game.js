import { showNotification } from './notifications.js';

// Инициализация данных (заглушка, заменить реальными данными)
let games = [
    { id: 1, title: "Сокровища Осетии", category: "vocabulary", keywords: ["осетия", "слова"], rating: 4 },
    { id: 2, title: "Грамматика Нартов", category: "grammar", keywords: ["названия", "правила"], rating: 3 },
    // Добавьте больше игр, когда предоставите блок
];

// Инициализация состояния фильтров
let filters = {
    search: '',
    category: '',
    keyword: '',
    rating: ''
};

document.addEventListener('DOMContentLoaded', () => {
    // Инициализация выпадающих меню
    const dropdowns = document.querySelectorAll('.dropdown');

    dropdowns.forEach(dropdown => {
        const toggle = dropdown.querySelector('.dropdown__toggle');
        const menu = dropdown.querySelector('.dropdown__menu');
        const searchInput = dropdown.querySelector('.dropdown__search-input');
        const clearSearch = dropdown.querySelector('.dropdown__clear-search');
        const items = dropdown.querySelectorAll('.dropdown__list li');

        // Открытие/закрытие меню
        toggle.addEventListener('click', () => {
            const isExpanded = toggle.getAttribute('aria-expanded') === 'true';
            toggle.setAttribute('aria-expanded', !isExpanded);
            menu.classList.toggle('active');
        });

        // Закрытие меню при клике вне
        document.addEventListener('click', (e) => {
            if (!dropdown.contains(e.target)) {
                toggle.setAttribute('aria-expanded', 'false');
                menu.classList.remove('active');
            }
        });

        // Поиск внутри выпадающего списка
        searchInput.addEventListener('input', () => {
            const searchTerm = searchInput.value.toLowerCase();
            items.forEach(item => {
                const text = item.textContent.toLowerCase();
                item.style.display = text.includes(searchTerm) ? 'flex' : 'none';
            });
        });

        // Очистка поиска
        clearSearch.addEventListener('click', () => {
            searchInput.value = '';
            items.forEach(item => {
                item.style.display = 'flex';
            });
        });

        // Выбор элемента из списка
        items.forEach(item => {
            item.addEventListener('click', () => {
                const value = item.getAttribute('data-value');
                const type = dropdown.closest('.games-filters__component').classList[1]; // categories, keywords, rating
                toggle.querySelector('span').textContent = item.textContent.trim();
                filters[type] = value;
                applyFilters();
                toggle.setAttribute('aria-expanded', 'false');
                menu.classList.remove('active');
            });
        });
    });

    // Обработка поиска
    //const searchForm = document.querySelector('.games-search__form');
    //searchForm.addEventListener('submit', (e) => {
    //    e.preventDefault();
    //    const searchTerm = searchForm.querySelector('#game-search-input').value.trim().toLowerCase();
    //    filters.search = searchTerm;
    //    applyFilters();
    //    if (searchTerm) {
    //        showNotification(`Поиск игры: "${searchTerm}"...`);
    //    } else {
    //        showNotification('Введите название игры для поиска');
    //    }
    //});

    // Применение фильтров
    function applyFilters() {
        const filteredGames = games.filter(game => {
            const matchesSearch = !filters.search || game.title.toLowerCase().includes(filters.search);
            const matchesCategory = !filters.category || game.category === filters.category;
            const matchesKeyword = !filters.keyword || game.keywords.includes(filters.keyword);
            const matchesRating = !filters.rating || game.rating === parseInt(filters.rating);
            return matchesSearch && matchesCategory && matchesKeyword && matchesRating;
        });
        updateGamesList(filteredGames);
    }

    // Обновление списка игр (заглушка)
    function updateGamesList(gamesList) {
        const gamesListElement = document.querySelector('.games-list'); // Предполагается, что список будет добавлен
        if (gamesListElement) {
            gamesListElement.innerHTML = '';
            if (gamesList.length === 0) {
                gamesListElement.innerHTML = '<p>Игры не найдены</p>';
            } else {
                gamesList.forEach(game => {
                    const gameItem = document.createElement('div');
                    gameItem.className = 'game-item';
                    gameItem.innerHTML = `<h3>${game.title}</h3><p>Категория: ${game.category}, Рейтинг: ${game.rating}</p>`;
                    gamesListElement.appendChild(gameItem);
                });
            }
        }
    }

    // Добавление кнопки сброса фильтров
    const resetFiltersButton = document.createElement('button');
    resetFiltersButton.textContent = 'Сбросить фильтры';
    resetFiltersButton.className = 'profile__button reset-filters';
    //document.querySelector('.games-filters').appendChild(resetFiltersButton);

    resetFiltersButton.addEventListener('click', () => {
        filters = { search: '', category: '', keyword: '', rating: '' };
        document.querySelectorAll('.dropdown__toggle span').forEach(toggle => {
            toggle.textContent = toggle.closest('.dropdown').parentElement.querySelector('.games-filters__title').textContent;
        });
        document.querySelector('#game-search-input').value = '';
        applyFilters();
        showNotification('Фильтры сброшены');
    });

    // Инициализация слайдера Рекомендуемых игр
    const slider = document.querySelector('.games-featured__slider');
    if (slider) {
        const slides = slider.querySelectorAll('.games-featured__slide');
        const indicators = slider.querySelectorAll('.game-hero__indicator');
        const prevButton = slider.querySelector('.games-featured__nav-button--prev');
        const nextButton = slider.querySelector('.games-featured__nav-button--next');
        let currentSlide = 0;

        function showSlide(index) {
            slides.forEach(slide => slide.classList.remove('active'));
            indicators.forEach(indicator => indicator.classList.remove('active'));
            slides[index].classList.add('active');
            indicators[index].classList.add('active');
            currentSlide = index;
        }

        prevButton.addEventListener('click', () => {
            const newIndex = (currentSlide - 1 + slides.length) % slides.length;
            showSlide(newIndex);
        });

        nextButton.addEventListener('click', () => {
            const newIndex = (currentSlide + 1) % slides.length;
            showSlide(newIndex);
        });

        indicators.forEach(indicator => {
            indicator.addEventListener('click', () => {
                const index = parseInt(indicator.getAttribute('data-index'));
                showSlide(index);
            });
        });

        // Автопрокрутка каждые 5 секунд
        setInterval(() => {
            const newIndex = (currentSlide + 1) % slides.length;
            showSlide(newIndex);
        }, 5000);

        // Обработка кнопок "Играть" и "Дополнительная информация"
        const playButtons = slider.querySelectorAll('.games-featured__button--play');
        const infoButtons = slider.querySelectorAll('.games-featured__button--info');

        playButtons.forEach(button => {
            button.addEventListener('click', () => {
                const gameTitle = button.closest('.games-featured__slide').querySelector('.games-featured__subtitle').textContent;
                showNotification(`Запуск игры: ${gameTitle}`);
                // Здесь можно добавить реальную логику запуска игры
            });
        });

        infoButtons.forEach(button => {
            button.addEventListener('click', () => {
                const gameTitle = button.closest('.games-featured__slide').querySelector('.games-featured__subtitle').textContent;
                showNotification(`Открытие информации об игре: ${gameTitle}`);
                // Здесь можно добавить переход на страницу с деталями игры
            });
        });
    }
});