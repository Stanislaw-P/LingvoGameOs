// dashboard.js — логика для админ-панели LingvoGameOS
// Мок-данные для опубликованных игр и игр на модерации
// В реальном проекте эти данные должны приходить с бэкенда через API:
// GET /api/games/published (JWT admin required)
// GET /api/games/pending (JWT admin required)

const publishedGames = [
  {
    id: 1,
    title: 'Словарный Бой',
    developer: 'Иван Иванов',
    publishedAt: '2024-06-01',
    url: '/game/1',
    analytics: true
  },
  {
    id: 2,
    title: 'Грамматический Штурм',
    developer: 'Мария Петрова',
    publishedAt: '2024-06-10',
    url: '/game/2',
    analytics: false
  }
];

const pendingGames = [
  {
    id: 3,
    title: 'Уши на Макушке',
    developer: 'Алексей Смирнов',
    submittedAt: '2024-06-20',
    url: '/game/3',
  },
  {
    id: 4,
    title: 'Пазл Мастер',
    developer: 'Ольга Кузнецова',
    submittedAt: '2024-06-22',
    url: '/game/4',
  }
];

// Имитация задержки API (setTimeout)
function fakeApi(data, delay = 400) {
  return new Promise(resolve => setTimeout(() => resolve(data), delay));
}

// Заполнение статистики
function renderStats() {
  document.getElementById('total-games-value').textContent = publishedGames.length;
  document.getElementById('pending-games-value').textContent = pendingGames.length;
}

// Заполнение таблицы опубликованных игр
function renderPublishedGames(filter = '', sort = 'date') {
  let games = [...publishedGames];
  if (filter) {
    games = games.filter(g => g.title.toLowerCase().includes(filter) || g.developer.toLowerCase().includes(filter));
  }
  if (sort === 'title') games.sort((a, b) => a.title.localeCompare(b.title));
  else if (sort === 'developer') games.sort((a, b) => a.developer.localeCompare(b.developer));
  else games.sort((a, b) => new Date(b.publishedAt) - new Date(a.publishedAt));

  const tbody = document.querySelector('#published-games-table tbody');
  tbody.innerHTML = '';
  games.forEach(game => {
    const tr = document.createElement('tr');
    tr.innerHTML = `
      <td>${game.title}</td>
      <td>${game.developer}</td>
      <td>${game.publishedAt}</td>
      <td>
        <a href="${game.url}" class="btn secondary" target="_blank" aria-label="Играть в ${game.title}">Играть</a>
        <a href="game-details.html?id=${game.id}" class="btn outline" aria-label="Редактировать ${game.title}">Редактировать</a>
        <button class="btn primary" onclick="alert('Аналитика будет реализована позже')" aria-label="Аналитика по ${game.title}">Аналитика</button>
      </td>
    `;
    tbody.appendChild(tr);
  });
}

// Заполнение таблицы игр на модерации
function renderPendingGames(filter = '', sort = 'date') {
  let games = [...pendingGames];
  if (filter) {
    games = games.filter(g => g.title.toLowerCase().includes(filter) || g.developer.toLowerCase().includes(filter));
  }
  if (sort === 'title') games.sort((a, b) => a.title.localeCompare(b.title));
  else if (sort === 'developer') games.sort((a, b) => a.developer.localeCompare(b.developer));
  else games.sort((a, b) => new Date(b.submittedAt) - new Date(a.submittedAt));

  const tbody = document.querySelector('#pending-games-table tbody');
  tbody.innerHTML = '';
  games.forEach(game => {
    const tr = document.createElement('tr');
    tr.innerHTML = `
      <td>${game.title}</td>
      <td>${game.developer}</td>
      <td>${game.submittedAt}</td>
      <td>
        <a href="game-details.html?id=${game.id}" class="btn outline" aria-label="Посмотреть детали ${game.title}">Детали</a>
        <button class="btn outline" data-game-id="${game.id}" data-action="feedback" aria-label="Написать отзыв по ${game.title}">Написать отзыв</button>
        <button class="btn primary" data-game-id="${game.id}" data-action="confirm" aria-label="Подтвердить игру ${game.title}">Подтвердить</button>
      </td>
    `;
    tbody.appendChild(tr);
  });
}

// Фильтрация и сортировка
function setupFilters() {
  document.getElementById('published-search').addEventListener('input', e => {
    renderPublishedGames(e.target.value.toLowerCase(), document.getElementById('published-sort').value);
  });
  document.getElementById('published-sort').addEventListener('change', e => {
    renderPublishedGames(document.getElementById('published-search').value.toLowerCase(), e.target.value);
  });
  document.getElementById('pending-search').addEventListener('input', e => {
    renderPendingGames(e.target.value.toLowerCase(), document.getElementById('pending-sort').value);
  });
  document.getElementById('pending-sort').addEventListener('change', e => {
    renderPendingGames(document.getElementById('pending-search').value.toLowerCase(), e.target.value);
  });
}

// Обработка кнопок "Написать отзыв" и "Подтвердить"
document.addEventListener('click', function(e) {
  if (e.target.matches('[data-action="feedback"]')) {
    // Открыть модальное окно обратной связи
    window.openFeedbackModal && window.openFeedbackModal(e.target.dataset.gameId);
  }
  if (e.target.matches('[data-action="confirm"]')) {
    // Имитация подтверждения игры (POST /api/games/confirm)
    // Требуется: { gameId }, JWT admin
    const gameId = Number(e.target.dataset.gameId);
    // В реальном проекте: fetch('/api/games/confirm', {method: 'POST', body: JSON.stringify({gameId}), headers: {Authorization: 'Bearer ...'}})
    setTimeout(() => {
      const idx = pendingGames.findIndex(g => g.id === gameId);
      if (idx !== -1) {
        const [game] = pendingGames.splice(idx, 1);
        publishedGames.push({
          id: game.id,
          title: game.title,
          developer: game.developer,
          publishedAt: new Date().toISOString().slice(0,10),
          url: `/game/${game.id}`,
          analytics: false
        });
        renderStats();
        renderPublishedGames();
        renderPendingGames();
        alert('Игра подтверждена и опубликована!');
      }
    }, 500);
  }
});

// Инициализация
window.addEventListener('DOMContentLoaded', () => {
  renderStats();
  renderPublishedGames();
  renderPendingGames();
  setupFilters();
}); 