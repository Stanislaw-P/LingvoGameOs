// feedback.js — логика для модального окна обратной связи в админ-панели
// В реальном проекте отправка обратной связи должна идти на API:
// POST /api/games/:id/feedback { message: string } (JWT admin required)

const feedbackModal = document.getElementById('feedback-modal');
const feedbackForm = document.getElementById('feedback-form');
const feedbackMessage = document.getElementById('feedback-message');
const feedbackStatus = document.getElementById('feedback-status');
let currentGameId = null;

window.openFeedbackModal = function(gameId) {
  currentGameId = gameId;
  feedbackModal.setAttribute('aria-hidden', 'false');
  feedbackMessage.value = '';
  feedbackStatus.textContent = '';
  feedbackMessage.focus();
};

document.getElementById('close-feedback-modal').onclick = closeModal;
document.getElementById('cancel-feedback').onclick = closeModal;

function closeModal() {
  feedbackModal.setAttribute('aria-hidden', 'true');
  feedbackStatus.textContent = '';
}

feedbackForm.onsubmit = function(e) {
  e.preventDefault();
  const message = feedbackMessage.value.trim();
  if (!message) {
    feedbackStatus.textContent = 'Введите сообщение';
    feedbackStatus.style.color = 'var(--danger)';
    return;
  }
  feedbackStatus.textContent = 'Отправка...';
  feedbackStatus.style.color = 'var(--secondary)';
  // Имитация отправки на сервер (POST /api/games/:id/feedback)
  // Требуется: { message }, JWT admin
  setTimeout(() => {
    // В реальном проекте: fetch(`/api/games/${currentGameId}/feedback`, {method: 'POST', body: JSON.stringify({message}), headers: {Authorization: 'Bearer ...'}})
    console.log('Отзыв отправлен:', {gameId: currentGameId, message});
    feedbackStatus.textContent = 'Отзыв успешно отправлен!';
    feedbackStatus.style.color = 'var(--success)';
    setTimeout(closeModal, 1200);
  }, 700);
}; 