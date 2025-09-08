// admin-panel.js — логика для админ-панели LingvoGameOS
// Мок-данные и имитация API-вызовов для демонстрации функционала

// Мок-данные для игр на модерации
const pendingGamesData = [
    {
        id: 1,
        title: "Словарный Бой",
        developer: "Иван Иванов",
        developerEmail: "ivan@example.com",
        description: "Интерактивная игра для изучения новых слов. Игроки должны угадывать значения слов, используя контекст и подсказки.",
        rules: "1. Выберите уровень сложности\n2. Читайте предложения с новыми словами\n3. Выбирайте правильное значение слова\n4. Зарабатывайте очки за правильные ответы",
        date: "20.06.2024",
        cover: "/img/game1.png",
        screenshots: ["/img/games/puzzle-1.jpg", "/img/games/puzzle-2.png", "/img/games/puzzle-3.jpg"],
        gameFile: "/games/slovarny-boy.zip",
        platform: "Desktop"
    },
    {
        id: 2,
        title: "Грамматический Штурм",
        developer: "Мария Петрова",
        developerEmail: "maria@example.com",
        description: "Игра для практики грамматических правил. Игроки составляют предложения, исправляют ошибки и изучают структуру языка.",
        rules: "1. Выберите грамматическую тему\n2. Составьте предложения из слов\n3. Исправьте ошибки в тексте\n4. Пройдите тесты на понимание",
        date: "22.06.2024",
        cover: "/img/game2.png",
        screenshots: ["/img/games/art-object-1.png", "/img/games/art-object-2.jpeg"],
        gameFile: "/games/grammatichesky-shturm.zip",
        platform: "Desktop"
    },
    {
        id: 3,
        title: "Уши на Макушке",
        developer: "Алексей Смирнов",
        developerEmail: "alex@example.com",
        description: "Аудио-игра для тренировки восприятия речи на слух. Игроки слушают диалоги и выполняют задания.",
        rules: "1. Включите звук\n2. Слушайте диалоги и монологи\n3. Отвечайте на вопросы по содержанию\n4. Повторяйте фразы для тренировки произношения",
        date: "25.06.2024",
        cover: "/img/game3.png",
        screenshots: ["/img/games/gameplay-animal-1.jpg", "/img/games/gameplay-animal-2.jpg"],
        gameFile: "/games/ushi-na-makushke.zip",
        platform: "Desktop"
    }
];

// Мок-данные для опубликованных игр
const publishedGamesData = [
    {
        id: 4,
        title: "Пазл Мастер",
        developer: "Ольга Кузнецова",
        date: "15.06.2024",
        cover: "/img/games/puzzle-1.jpg",
        platform: "Web",
        gameUrl: "/game/4"
    },
    {
        id: 5,
        title: "Горный Лабиринт",
        developer: "Дмитрий Волков",
        date: "10.06.2024",
        cover: "/img/games/mountain-labyrinth-banner-1.png",
        platform: "Desktop",
        gameUrl: "/games/mountain-labyrinth.zip"
    }
];

// Имитация API-вызовов
function fakeApiCall(data, delay = 500) {
    return new Promise(resolve => {
        setTimeout(() => {
            console.log('API Call:', data);
            resolve(data);
        }, delay);
    });
}

// Функции для работы с модальными окнами
function openModal(modalId) {
    const modal = document.getElementById(modalId);
    modal.setAttribute('aria-hidden', 'false');
    document.body.style.overflow = 'hidden';
}

function closeModal(modalId) {
    const modal = document.getElementById(modalId);
    modal.setAttribute('aria-hidden', 'true');
    document.body.style.overflow = '';
}

// Закрытие модальных окон по клику вне их
document.addEventListener('click', function (e) {
    if (e.target.classList.contains('admin-modal')) {
        closeModal(e.target.id);
    }
});

// Закрытие по Escape
document.addEventListener('keydown', function (e) {
    if (e.key === 'Escape') {
        const openModal = document.querySelector('.admin-modal[aria-hidden="false"]');
        if (openModal) {
            closeModal(openModal.id);
        }
    }
});

// Функции для работы с играми
function openGameDetails(gameId) {
    const game = pendingGamesData.find(g => g.id === gameId) ||
        publishedGamesData.find(g => g.id === gameId);

    if (!game) {
        console.error('Игра не найдена:', gameId);
        return;
    }

    // Заполняем модальное окно данными
    document.getElementById('modal-game-cover').src = game.cover;
    document.getElementById('modal-game-developer').value = game.developer;
    document.getElementById('modal-game-title').value = game.title;
    document.getElementById('modal-game-description').value = game.description || '';
    document.getElementById('modal-game-rules').value = game.rules || '';
    document.getElementById('modal-game-date').value = game.date;

    // Скриншоты
    const screenshotsContainer = document.getElementById('modal-game-screenshots');
    screenshotsContainer.innerHTML = '';
    if (game.screenshots) {
        game.screenshots.forEach(screenshot => {
            const img = document.createElement('img');
            img.src = screenshot;
            img.alt = 'Скриншот игры';
            screenshotsContainer.appendChild(img);
        });
    }

    // Файл игры
    const gameFileLink = document.getElementById('modal-game-file');
    if (game.platform === 'Desktop') {
        gameFileLink.href = game.gameFile || game.gameUrl;
        gameFileLink.textContent = 'Скачать игру';
    } else {
        gameFileLink.href = game.gameUrl;
        gameFileLink.textContent = 'Играть онлайн';
    }

    // Показываем/скрываем кнопку подтверждения
    const approveBtn = document.getElementById('approve-game-btn');
    if (pendingGamesData.find(g => g.id === gameId)) {
        approveBtn.style.display = 'inline-flex';
    } else {
        approveBtn.style.display = 'none';
    }

    openModal('game-details-modal');
}

function closeGameDetails() {
    closeModal('game-details-modal');
}

function saveGameChanges() {
    // Имитация сохранения изменений
    console.log('Сохранение изменений игры...');

    // В реальном проекте: PUT /api/games/:id
    fakeApiCall({ success: true, message: 'Изменения сохранены' })
        .then(() => {
            alert('Изменения успешно сохранены!');
            closeGameDetails();
        })
        .catch(error => {
            console.error('Ошибка сохранения:', error);
            alert('Ошибка при сохранении изменений');
        });
}

function approveGameFromModal() {
    const gameId = getCurrentGameId(); // Нужно реализовать получение ID текущей игры
    approveGame(gameId);
    closeGameDetails();
}

async function approveGame(gameId) {
    const button = document.getElementById('approve-game-btn');

    // Показываем индикатор загрузки
    button.textContent = 'Загрузка...';
    button.disabled = true;

    try {
        const response = await fetch(`/Admin/PendingGames/Publish?gameId=${gameId}`, {
            method: 'POST',
            headers: {
                'Content-Type': 'application/json'
            },
            body: JSON.stringify({
                gameId: gameId
            })
        });

        if (!response.ok) {
            throw new Error('Ошибка публикации игры');
        }

        // Удаляем игру из списка на модерации
        const gameElement = document.getElementById(`pending-game-${gameId}`);
        if (gameElement) {
            gameElement.remove();
        }

        const result = await response.json();

        // Добавляем игру в список опубликованных
        addToPublishedList(result.gameData);
        alert('Игра успешно подтверждена и опубликована!');
    } catch (error) {
        console.error('Ошибка подтверждения:', error);
        alert('Ошибка при подтверждении игры');
        button.textContent = 'Опубликовать игру';
        button.disabled = false;
    }

    //console.log('Подтверждение игры:', gameId);

    //// В реальном проекте: POST /api/games/:id/approve
    //fakeApiCall({ success: true, message: 'Игра подтверждена' })
    //    .then(() => {
    //        alert('Игра успешно подтверждена и опубликована!');
    //        // Обновляем UI - убираем игру из списка на модерации
    //        updatePendingGamesList();
    //    })
    //    .catch(error => {
    //        console.error('Ошибка подтверждения:', error);
    //        alert('Ошибка при подтверждении игры');
    //    });
}

function addToPublishedList(gameData) {
    const publishedList = document.getElementById('published-games-list');

    const newGameElement = document.createElement('div');
    newGameElement.className = 'admin-game-card';
    newGameElement.innerHTML = `
        <div class="admin-game-card__image">
            <img src="${gameData.imagePath || '/img/default-game.png'}" alt="${gameData.title}">
        </div>
        <div class="admin-game-card__content">
            <h3 class="admin-game-card__title">${gameData.title}</h3>
            <p class="admin-game-card__developer">${gameData.authorName}</p>
            <p class="admin-game-card__date">${gameData.publicationDate}</p>
        </div>
        <div class="admin-game-card__actions">
            ${gameData.platform === 'Desktop' ?
            `<a href="${gameData.gameUrl}" download class="admin-btn admin-btn--primary">Скачать</a>` :
            `<a href="/Game/Start/${gameData.id}" class="admin-btn admin-btn--primary">Играть</a>`
        }
            <button asp-area="Admin" asp-controller="Games" asp-action="Edit" asp-route-gameId="${gameData.id}" class="admin-btn admin-btn--secondary">Редактировать</button>
            <button class="admin-btn admin-btn--outline" onclick="openAnalytics(${gameData.id})">Аналитика</button>
        </div>
    `;

    publishedList.appendChild(newGameElement);
}


let currentFeedbackGameId = null;

async function openFeedback(gameId) {
    currentFeedbackGameId = gameId;
    document.getElementById('feedback-game-id').value = gameId;

    try {
        // Показываем загрузку
        document.getElementById('feedback-game-infoId').textContent = 'Загрузка...';
        document.getElementById('feedback-game-title').textContent = 'Загрузка...';
        document.getElementById('feedback-game-author').textContent = 'Загрузка...';

        const response = await fetch(`/Admin/PendingGames/GetGameInfo?gameId=${gameId}`);
        if (response.ok) {
            const gameInfo = await response.json();
            document.getElementById('feedback-game-infoId').textContent = `id: ${gameId}`;
            document.getElementById('feedback-game-title').textContent = `Игра: ${gameInfo.title}`;
            document.getElementById('feedback-game-author').textContent = `Разработчик: ${gameInfo.author} - Email: ${gameInfo.authorEmail}`;

        } else {
            throw new Error('Не удалось загрузить информацию об игре');
        }
    }
    catch (error) {
        console.error('Ошибка:', error);
        document.getElementById('feedback-game-info').textContent = 'Ошибка загрузки';
    }

    //if (game) {
    //  document.getElementById('feedback-message').placeholder = 
    //    `Сообщение для разработчика "${game.developer}" (${game.developerEmail})`;
    //}

    openModal('feedback-modal');
}

function closeFeedback() {
    closeModal('feedback-modal');
    document.getElementById('feedback-message').value = '';
    currentFeedbackGameId = null;
}

async function sendFeedback() {
    const message = document.getElementById('feedback-message').value.trim();
    const gameId = currentFeedbackGameId;

    if (!message) {
        alert('Введите сообщение для разработчика');
        return;
    }

    if (!currentFeedbackGameId) {
        alert('Ошибка: не выбрана игра');
        return;
    }

    try {
        console.log('Отправка обратной связи:', { gameId: currentFeedbackGameId, message });

        // Показываем индикатор загрузки
        const sendButton = document.querySelector('.admin-btn--primary');
        const originalText = sendButton.textContent;
        sendButton.textContent = 'Отправка...';
        sendButton.disabled = true;

        // Отправка данных на сервер
        const response = await fetch('/Admin/PendingGames/SendFeedback', {
            method: 'POST',
            headers: {
                'Content-Type': 'application/json'
            },
            body: JSON.stringify({
                message: message,
                gameId: gameId
            })
        });

        if (response.ok) {
            //const result = await response.json()
            alert('Обратная связь успешно отправлена разработчику!');
            closeFeedback();
        } else {
            throw new Error('Ошибка при отправке сообщения');
        }
    } catch (error) {
        console.error('Ошибка отправки:', error);
        alert('Ошибка при отправке обратной связи');
    } finally {
        // Восстанавливаем кнопку
        const sendButton = document.querySelector('.admin-btn--primary');
        sendButton.textContent = 'Отправить';
        sendButton.disabled = false;
    }

    // В реальном проекте: POST /api/games/:id/feedback
    //fakeApiCall({ success: true, message: 'Обратная связь отправлена' })
    //  .then(() => {
    //    alert('Обратная связь успешно отправлена разработчику!');
    //    closeFeedback();
    //  })
    //  .catch(error => {
    //    console.error('Ошибка отправки:', error);
    //    alert('Ошибка при отправке обратной связи');
    //  });
}

function openAnalytics(gameId) {
    console.log('Открытие аналитики для игры:', gameId);
    openModal('analytics-modal');
}

function closeAnalytics() {
    closeModal('analytics-modal');
}

// Функции поиска и сортировки
function setupSearchAndSort() {
    // Поиск опубликованных игр
    const publishedSearch = document.getElementById('published-search');
    if (publishedSearch) {
        publishedSearch.addEventListener('input', function () {
            filterGames('published', this.value);
        });
    }

    // Сортировка опубликованных игр
    const publishedSort = document.getElementById('published-sort');
    if (publishedSort) {
        publishedSort.addEventListener('change', function () {
            sortGames('published', this.value);
        });
    }

    // Поиск игр на модерации
    const pendingSearch = document.getElementById('pending-search');
    if (pendingSearch) {
        pendingSearch.addEventListener('input', function () {
            filterGames('pending', this.value);
        });
    }

    // Сортировка игр на модерации
    const pendingSort = document.getElementById('pending-sort');
    if (pendingSort) {
        pendingSort.addEventListener('change', function () {
            sortGames('pending', this.value);
        });
    }
}

function filterGames(type, query) {
    console.log(`Фильтрация ${type} игр по запросу:`, query);
    // В реальном проекте здесь была бы логика фильтрации
}

function sortGames(type, sortBy) {
    console.log(`Сортировка ${type} игр по:`, sortBy);
    // В реальном проекте здесь была бы логика сортировки
}

function updatePendingGamesList() {
    console.log('Обновление списка игр на модерации...');
    // В реальном проекте здесь было бы обновление UI
}



// Функция для фильтрации отзывов
function setupReviewsFilter() {
    const filters = document.querySelectorAll('.btn--filter');
    const reviews = document.querySelectorAll('.admin-review');

    if (filters.length === 0 || reviews.length === 0) {
        return;
    }

    filters.forEach(filter => {
        filter.addEventListener('click', function () {
            filters.forEach(f => f.classList.remove('active'));
            this.classList.add('active');

            const filterType = this.dataset.filter;

            reviews.forEach(review => {
                if (filterType === 'all') {
                    review.style.display = 'flex';
                } else if (filterType === 'published') {
                    review.style.display = review.classList.contains('published') ? 'flex' : 'none';
                } else if (filterType === 'pending') {
                    review.style.display = review.classList.contains('pending') ? 'flex' : 'none';
                }
            });
        });
    });
}


// Инициализация при загрузке страницы
document.addEventListener('DOMContentLoaded', function () {
    console.log('Админ-панель загружена');

    // Настройка поиска и сортировки
    setupSearchAndSort();

    // Настройка фильтрации отзывов
    setupReviewsFilter();

    // Инициализация статистики
    updateStats();
});

function updateStats() {
    // В реальном проекте статистика загружалась бы с сервера
    console.log('Обновление статистики...');
}

// Глобальные функции для вызова из HTML
window.openGameDetails = openGameDetails;
window.closeGameDetails = closeGameDetails;
window.saveGameChanges = saveGameChanges;
window.approveGameFromModal = approveGameFromModal;
window.approveGame = approveGame;
window.openFeedback = openFeedback;
window.closeFeedback = closeFeedback;
window.sendFeedback = sendFeedback;
window.openAnalytics = openAnalytics;
window.closeAnalytics = closeAnalytics;