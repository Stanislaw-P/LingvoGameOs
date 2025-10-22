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

async function approveGame(gameId) {
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
    }
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
            <a href="/Admin/Game/Edit?gameId=${gameData.id}" class="admin-btn admin-btn--secondary">Редактировать</a>
            <a href="/Admin/Game/Deactivate?gameId=${gameData.id}" class="admin-btn admin-btn--danger">Скрыть</a>
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



let currentDeleteGameId = null;
let currentDeleteGameTitle = null;

// Открытие модального окна удаления
function openDeleteModal(gameId, gameTitle) {
    currentDeleteGameId = gameId;
    currentDeleteGameTitle = gameTitle;

    // Заполняем информацию об игре
    document.getElementById('delete-game-title').textContent = gameTitle;

    // Сбрасываем состояние кнопки
    const deleteBtn = document.getElementById('confirm-delete-btn');
    deleteBtn.textContent = 'Удалить игру';
    deleteBtn.disabled = false;

    openModal('delete-modal');
}

// Закрытие модального окна удаления
function closeDeleteModal() {
    closeModal('delete-modal');
    currentDeleteGameId = null;
    currentDeleteGameTitle = null;
}

// Подтверждение удаления игры
async function confirmDeleteGame() {
    if (!currentDeleteGameId) {
        alert('Ошибка: игра не выбрана');
        return;
    }

    const deleteBtn = document.getElementById('confirm-delete-btn');

    // Показываем индикатор загрузки
    deleteBtn.textContent = 'Удаление...';
    deleteBtn.disabled = true;

    try {
        const response = await fetch(`/Admin/PendingGames/Delete?gameId=${currentDeleteGameId}`, {
            method: 'DELETE',
            headers: {
                'Content-Type': 'application/json'
            }
        });

        if (response.ok) {
            // Удаляем игру из DOM
            const gameElement = document.getElementById(`pending-game-${currentDeleteGameId}`);
            if (gameElement) {
                gameElement.style.opacity = '0';
                gameElement.style.transform = 'translateX(100%)';
                setTimeout(() => {
                    gameElement.remove();
                    updatePendingGamesStats();
                }, 300);
            }

            alert(`Игра "${currentDeleteGameTitle}" успешно удалена!`);
            closeDeleteModal();
        } else {
            throw new Error('Ошибка удаления игры');
        }
    } catch (error) {
        console.error('Ошибка удаления:', error);
        alert('Ошибка при удалении игры');

        // Восстанавливаем кнопку
        deleteBtn.textContent = 'Удалить игру';
        deleteBtn.disabled = false;
    }
}

function initializeDeleteButtons() {
    document.addEventListener('click', function (e) {
        const deleteBtn = e.target.closest('.delete-game-btn');
        if (deleteBtn) {
            const gameId = deleteBtn.getAttribute('data-game-id');
            const gameTitle = deleteBtn.getAttribute('data-game-title');
            openDeleteModal(gameId, gameTitle);
        }
    });
}



document.addEventListener('DOMContentLoaded', function () {
    initializeDeleteButtons();
});

// Глобальные функции для вызова из HTML
window.approveGame = approveGame;
window.openFeedback = openFeedback;
window.closeFeedback = closeFeedback;
window.sendFeedback = sendFeedback;
window.openAnalytics = openAnalytics;
window.closeAnalytics = closeAnalytics;
window.openDeleteModal = openDeleteModal;
window.closeDeleteModal = closeDeleteModal;
window.confirmDeleteGame = confirmDeleteGame;
window.initializeDeleteButtons = initializeDeleteButtons;