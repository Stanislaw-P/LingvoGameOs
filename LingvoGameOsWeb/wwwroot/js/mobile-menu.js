document.addEventListener('DOMContentLoaded', function () {
    const burger = document.querySelector('.header__burger');
    const sidebar = document.querySelector('.sidebar-menu');
    const overlay = document.querySelector('.sidebar-overlay');
    const closeBtn = document.querySelector('.sidebar-menu__close');

    // Проверяем, что элементы существуют (для мобильной версии)
    if (burger && sidebar && overlay) {
        // Открытие меню
        burger.addEventListener('click', function () {
            sidebar.classList.add('active');
            overlay.classList.add('active');
            document.body.style.overflow = 'hidden';
            // Скрываем бургер при открытии меню
            burger.style.display = 'none';
        });

        // Закрытие меню
        function closeMenu() {
            sidebar.classList.remove('active');
            overlay.classList.remove('active');
            document.body.style.overflow = '';
            // Показываем бургер при закрытии меню (только на мобильных)
            if (window.innerWidth <= 480) {
                burger.style.display = 'flex';
            }
        }

        closeBtn.addEventListener('click', closeMenu);
        overlay.addEventListener('click', closeMenu);

        // Закрытие при клике на ссылку
        document.querySelectorAll('.sidebar-menu__link').forEach(link => {
            link.addEventListener('click', closeMenu);
        });

        // Закрытие при нажатии Escape
        document.addEventListener('keydown', function (e) {
            if (e.key === 'Escape' && sidebar.classList.contains('active')) {
                closeMenu();
            }
        });

        // Закрытие при изменении размера окна (если перешли на десктоп)
        window.addEventListener('resize', function () {
            if (window.innerWidth > 480 && sidebar.classList.contains('active')) {
                closeMenu();
            } else if (window.innerWidth <= 480 && !sidebar.classList.contains('active')) {
                // Восстанавливаем бургер при возврате на мобильный размер
                burger.style.display = 'flex';
            }
        });
    }
});