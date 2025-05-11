/**
 * Управление выпадающим меню профиля
 */
function setupProfileDropdown() {
    const toggle = document.querySelector('.user-profile__toggle');
    const menu = document.querySelector('.user-profile__menu');

    if (!toggle || !menu) {
        console.warn('Элементы для выпадающего меню профиля не найдены');
        return;
    }

    // Переключение меню при клике на toggle
    toggle.addEventListener('click', (event) => {
        event.stopPropagation(); // Предотвращаем всплытие события
        menu.classList.toggle('active');
    });

    // Закрытие меню при клике вне области
    document.addEventListener('click', (event) => {
        if (!menu.contains(event.target) && !toggle.contains(event.target)) {
            menu.classList.remove('active');
        }
    });

    // Закрытие меню при нажатии клавиши Escape
    document.addEventListener('keydown', (event) => {
        if (event.key === 'Escape' && menu.classList.contains('active')) {
            menu.classList.remove('active');
        }
    });
}

// Вызов функции при загрузке страницы
document.addEventListener('DOMContentLoaded', setupProfileDropdown);