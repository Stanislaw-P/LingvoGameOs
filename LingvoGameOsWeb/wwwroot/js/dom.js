/**
 * Модуль для работы с DOM
 * @module dom
 */
import { COLORS } from './config.js';

/**
 * Показывает сообщение об ошибке
 * @param {string} message - Текст ошибки
 */
export function showError(message) {
    const errorMessage = document.querySelector('#error-message');
    if (!errorMessage) return;
    errorMessage.textContent = message;
    errorMessage.classList.add('active');
    setTimeout(() => errorMessage.classList.remove('active'), 5000);
}

/**
 * Обновляет прогресс-бар
 * @param {number} current - Текущий прогресс
 * @param {number} total - Общее количество
 */
export function updateProgress(current, total) {
    const progressFill = document.querySelector('.game-play__progress-fill');
    const progressText = document.querySelector('.game-play__progress-text');
    const percentage = (current / total) * 100;
    if (progressFill) {
        progressFill.style.width = `${percentage}%`;
    }
    if (progressText) {
        progressText.textContent = `Слово ${current} из ${total}`;
    }
    document.querySelector('.game-play__progress')?.setAttribute('aria-valuenow', percentage);
    document.querySelector('#totalWords').textContent = total.toString();
}

/**
 * Настраивает обработчики модальных окон
 */
export function setupModalHandlers() {
    const modals = [
        { id: 'gameInfoModal', closeBtn: '.game-info-modal__close' },
        { id: 'pauseModal', closeBtn: '.game-pause-modal__close' }
    ];

    modals.forEach(({ id, closeBtn }) => {
        const modal = document.querySelector(`#${id}`);
        const closeButton = document.querySelector(closeBtn);
        if (modal && closeButton) {
            closeButton.addEventListener('click', () => {
                modal.style.display = 'none';
            });
        }
    });
}

/**
 * Дебаунс для функций
 * @param {Function} func - Функция для дебаунса
 * @param {number} wait - Время ожидания (мс)
 * @returns {Function} Дебаунсированная функция
 */
export function debounce(func, wait) {
    let timeout;
    return function executedFunction(...args) {
        const later = () => {
            clearTimeout(timeout);
            func(...args);
        };
        clearTimeout(timeout);
        timeout = setTimeout(later, wait);
    };
}