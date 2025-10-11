// Функция для показа модального окна скачивания
function confirmDownload(event) {
    event.preventDefault();
    event.stopPropagation();

    const downloadUrl = event.currentTarget.getAttribute('href');
    const modal = document.getElementById('downloadModal');
    const confirmBtn = modal.querySelector('.download-modal__confirm');
    const cancelBtn = modal.querySelector('.download-modal__cancel');
    const closeBtn = modal.querySelector('.download-modal__close');

    // Устанавливаем URL для скачивания в кнопку подтверждения
    confirmBtn.onclick = function () {
        window.location.href = downloadUrl;
        hideDownloadModal();
    };

    // Показываем модальное окно
    showDownloadModal();

    // Функции показа/скрытия модального окна
    function showDownloadModal() {
        modal.style.display = 'flex';
        setTimeout(() => {
            modal.classList.add('active');
        }, 10);
        document.body.style.overflow = 'hidden';
    }

    function hideDownloadModal() {
        modal.classList.remove('active');
        setTimeout(() => {
            modal.style.display = 'none';
        }, 300);
        document.body.style.overflow = '';
    }

    // Обработчики событий для закрытия модального окна
    cancelBtn.onclick = hideDownloadModal;
    closeBtn.onclick = hideDownloadModal;

    // Закрытие по клику на фон
    modal.onclick = function (e) {
        if (e.target === modal) {
            hideDownloadModal();
        }
    };

    // Закрытие по ESC
    function handleEscape(e) {
        if (e.key === 'Escape' && modal.classList.contains('active')) {
            hideDownloadModal();
        }
    }

    document.addEventListener('keydown', handleEscape);

    // Убираем обработчик после закрытия
    modal._escapeHandler = handleEscape;

    return false;
}

// Инициализация после загрузки DOM
document.addEventListener('DOMContentLoaded', function () {
    // Находим все ссылки скачивания по классу
    const downloadLinks = document.querySelectorAll('a.download-btn[download]');
    downloadLinks.forEach((link, index) => {
        link.addEventListener('click', confirmDownload);
    });

    // Альтернативный селектор - если класс не работает
    //const alternativeLinks = document.querySelectorAll('a[download].games__button');

    //alternativeLinks.forEach((link, index) => {
    //    console.log(`Setting up alternative handler for link ${index}:`, link);
    //    link.addEventListener('click', confirmDownload);
    //});
});
