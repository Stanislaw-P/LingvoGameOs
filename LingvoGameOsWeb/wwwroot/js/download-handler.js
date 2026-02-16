// Функция показа модального окна
function showDownloadModal(modal) {
    modal.style.display = 'flex';
    setTimeout(() => {
        modal.classList.add('active');
    }, 10);
    document.body.style.overflow = 'hidden';
}

// Функция скрытия модального окна
function hideDownloadModal(modal) {
    modal.classList.remove('active');
    setTimeout(() => {
        modal.style.display = 'none';
    }, 300);
    document.body.style.overflow = '';
}

// Глобальный обработчик кликов
document.addEventListener('click', function (event) {
    // Ищем, был ли клик по кнопке скачивания (или элементам внутри неё)
    const downloadBtn = event.target.closest('a.download-btn[download]');

    if (!downloadBtn) return;

    // Останавливаем стандартное скачивание
    event.preventDefault();
    event.stopPropagation();

    const downloadUrl = downloadBtn.getAttribute('href');
    const modal = document.getElementById('downloadModal');

    if (!modal) return;

    const confirmBtn = modal.querySelector('.download-modal__confirm');
    const cancelBtn = modal.querySelector('.download-modal__cancel');
    const closeBtn = modal.querySelector('.download-modal__close');

    // Привязываем URL к кнопке подтверждения
    confirmBtn.onclick = function () {
        window.location.href = downloadUrl;
        hideDownloadModal(modal);
    };

    // Настраиваем закрытие
    cancelBtn.onclick = () => hideDownloadModal(modal);
    closeBtn.onclick = () => hideDownloadModal(modal);

    modal.onclick = function (e) {
        if (e.target === modal) hideDownloadModal(modal);
    };

    // Показываем окно
    showDownloadModal(modal);
});

// Закрытие по ESC (один раз на весь документ)
document.addEventListener('keydown', function (e) {
    const modal = document.getElementById('downloadModal');
    if (e.key === 'Escape' && modal && modal.classList.contains('active')) {
        hideDownloadModal(modal);
    }
});