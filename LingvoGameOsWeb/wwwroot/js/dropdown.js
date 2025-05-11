// Инициализация выпадающего меню профиля
export function initializeDropdown() {
    const toggle = document.querySelector('.user-profile__toggle');
    const menu = document.querySelector('.user-profile__menu');
    const menuItems = document.querySelectorAll('.user-profile__menu-item');

    if (!toggle || !menu) return;

    toggle.addEventListener('click', (e) => {
        e.preventDefault();
        menu.classList.toggle('active');
    });

    document.addEventListener('click', (e) => {
        if (!toggle.contains(e.target) && !menu.contains(e.target)) {
            menu.classList.remove('active');
        }
    });

    menuItems.forEach(item => {
        const link = item.querySelector('a');
        const targetClass = item.getAttribute('data-target');

        if (link && targetClass) {
            link.addEventListener('click', (e) => {
                e.preventDefault();
                const currentPage = window.location.pathname.split('/').pop() || 'index.html';
                const targetPage = link.getAttribute('href').split('/').pop();

                if (currentPage === targetPage) {
                    const targetElement = document.querySelector(`.${targetClass}`);
                    if (targetElement) {
                        targetElement.scrollIntoView({ behavior: 'smooth', block: 'start' });
                    }
                } else {
                    window.location.href = `${link.getAttribute('href')}#${targetClass}`;
                }
                menu.classList.remove('active');
            });
        }
    });
}