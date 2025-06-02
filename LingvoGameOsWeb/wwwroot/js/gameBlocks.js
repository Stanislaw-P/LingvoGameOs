import { showNotification } from './notifications.js';

// Инициализация скролла для списков игр
export function initializeGameBlocks() {
    const blocks = [
        {
            wrapper: '.user-added-games__list-wrapper',
            list: '.user-added-games__list',
            leftButton: '.user-added-games__scroll-button--left',
            rightButton: '.user-added-games__scroll-button--right'
        },
        {
            wrapper: '.recent-games__list-wrapper',
            list: '.recent-games__list',
            leftButton: '.recent-games__scroll-button--left',
            rightButton: '.recent-games__scroll-button--right'
        }
    ];

    blocks.forEach(block => {
        const wrapper = document.querySelector(block.wrapper);
        const list = document.querySelector(block.list);
        const leftButton = document.querySelector(block.leftButton);
        const rightButton = document.querySelector(block.rightButton);

        if (!wrapper || !list || !leftButton || !rightButton) {
            console.warn(`Элементы для ${block.list} не найдены`);
            return;
        }

        // Проверка необходимости кнопок прокрутки
        const updateScrollButtons = () => {
            const scrollLeft = list.scrollLeft;
            const maxScroll = list.scrollWidth - list.clientWidth;
            leftButton.disabled = scrollLeft <= 0;
            rightButton.disabled = scrollLeft >= maxScroll - 1;
            leftButton.style.opacity = scrollLeft <= 0 ? '0.3' : '1';
            rightButton.style.opacity = scrollLeft >= maxScroll - 1 ? '0.3' : '1';
        };

        // Обработчики событий для кнопок прокрутки
        leftButton.addEventListener('click', () => {
            list.scrollBy({ left: -300, behavior: 'smooth' });
            setTimeout(updateScrollButtons, 300);
        });

        rightButton.addEventListener('click', () => {
            list.scrollBy({ left: 300, behavior: 'smooth' });
            setTimeout(updateScrollButtons, 300);
        });

        // Обновление состояния кнопок при прокрутке
        list.addEventListener('scroll', updateScrollButtons);
        updateScrollButtons();
    });

    // Обработчик для кнопки загрузки игры
    const uploadButtons = document.querySelectorAll('.user-added-games__upload-button');
    uploadButtons.forEach(button => {
        button.addEventListener('click', () => {
            showNotification('Функция загрузки игры в разработке!', 'info');
            // Здесь можно добавить логику для открытия формы загрузки игры
        });
    });
}