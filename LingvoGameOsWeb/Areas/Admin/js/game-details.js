// game-details.js — логика для страницы деталей игры в админ-панели
// Мок-данные для примера. В реальном проекте данные должны приходить с бэкенда:
// GET /api/games/:id (JWT admin required)
// PUT /api/games/:id (JWT admin required)
// POST /api/games/:id/confirm (JWT admin required)

const mockGames = [
  {
    id: 1,
    title: 'Словарный Бой',
    developer: 'Иван Иванов',
    description: 'Изучайте слова с удовольствием.',
    rules: 'Собирайте слова из букв, зарабатывайте баллы.',
    date: '2024-06-01',
    cover: '/img/game1.png',
    screenshots: ['/img/game1.png', '/img/game2.png'],
    link: '/game/1',
    published: true
  },
  {
    id: 3,
    title: 'Уши на Макушке',
    developer: 'Алексей Смирнов',
    description: 'Тренируйте аудирование.',
    rules: 'Слушайте и выбирайте правильные ответы.',
    date: '2024-06-20',
    cover: '/img/game3.png',
    screenshots: ['/img/game3.png'],
    link: '/game/3',
    published: false
  }
];

function getGameIdFromUrl() {
  const params = new URLSearchParams(window.location.search);
  return Number(params.get('id'));
}

function fakeApi(data, delay = 400) {
  return new Promise(resolve => setTimeout(() => resolve(data), delay));
}

async function loadGameDetails() {
  const gameId = getGameIdFromUrl();
  // В реальном проекте: fetch(`/api/games/${gameId}`, {headers: {Authorization: 'Bearer ...'}})
  const game = mockGames.find(g => g.id === gameId);
  if (!game) {
    document.body.innerHTML = '<p style="color:red">Игра не найдена</p>';
    return;
  }
  document.getElementById('game-cover').src = game.cover;
  document.getElementById('game-title').value = game.title;
  document.getElementById('game-developer').value = game.developer;
  document.getElementById('game-description').value = game.description;
  document.getElementById('game-rules').value = game.rules;
  document.getElementById('game-date').value = game.date;
  document.getElementById('game-link').href = game.link;
  document.getElementById('game-link').textContent = game.published ? 'Играть' : 'Скачать';
  // Галерея скриншотов
  const gallery = document.getElementById('game-screenshots');
  gallery.innerHTML = '';
  game.screenshots.forEach(src => {
    const img = document.createElement('img');
    img.src = src;
    img.alt = 'Скриншот игры';
    gallery.appendChild(img);
  });
  // Кнопка "Подтвердить игру" только для игр на модерации
  document.getElementById('confirm-game').style.display = game.published ? 'none' : 'inline-block';
}

// Сохранение изменений (PUT /api/games/:id)
document.getElementById('game-edit-form').onsubmit = function(e) {
  e.preventDefault();
  const status = document.getElementById('game-edit-status');
  status.textContent = 'Сохранение...';
  status.style.color = 'var(--secondary)';
  // В реальном проекте: fetch(`/api/games/${id}`, {method: 'PUT', body: ...})
  setTimeout(() => {
    status.textContent = 'Изменения сохранены!';
    status.style.color = 'var(--success)';
    setTimeout(() => status.textContent = '', 1200);
  }, 700);
};

// Открытие модального окна обратной связи
document.getElementById('open-feedback').onclick = function() {
  window.openFeedbackModal && window.openFeedbackModal(getGameIdFromUrl());
};

// Подтверждение игры (POST /api/games/:id/confirm)
document.getElementById('confirm-game').onclick = function() {
  const status = document.getElementById('game-edit-status');
  status.textContent = 'Подтверждение...';
  status.style.color = 'var(--secondary)';
  // В реальном проекте: fetch(`/api/games/${id}/confirm`, {method: 'POST'})
  setTimeout(() => {
    status.textContent = 'Игра подтверждена и опубликована!';
    status.style.color = 'var(--success)';
    document.getElementById('confirm-game').style.display = 'none';
    setTimeout(() => status.textContent = '', 1200);
  }, 700);
};

window.addEventListener('DOMContentLoaded', loadGameDetails); 