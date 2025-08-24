// Мобильное меню для шапки
document.addEventListener('DOMContentLoaded', function() {
    const mobileMenuToggle = document.getElementById('mobileMenuToggle');
    const mobileMenu = document.getElementById('mobileMenu');
    const body = document.body;

    if (mobileMenuToggle && mobileMenu) {
        // Переключение мобильного меню
        mobileMenuToggle.addEventListener('click', function() {
            mobileMenuToggle.classList.toggle('active');
            mobileMenu.classList.toggle('active');
            body.style.overflow = mobileMenu.classList.contains('active') ? 'hidden' : '';
        });

        // Закрытие меню при клике на ссылку
        const mobileMenuLinks = mobileMenu.querySelectorAll('.mobile-menu__link');
        mobileMenuLinks.forEach(link => {
            link.addEventListener('click', function() {
                mobileMenuToggle.classList.remove('active');
                mobileMenu.classList.remove('active');
                body.style.overflow = '';
            });
        });

        // Закрытие меню при клике вне его
        document.addEventListener('click', function(event) {
            if (!mobileMenuToggle.contains(event.target) && !mobileMenu.contains(event.target)) {
                mobileMenuToggle.classList.remove('active');
                mobileMenu.classList.remove('active');
                body.style.overflow = '';
            }
        });

        // Закрытие меню при изменении размера экрана
        window.addEventListener('resize', function() {
            if (window.innerWidth > 768) {
                mobileMenuToggle.classList.remove('active');
                mobileMenu.classList.remove('active');
                body.style.overflow = '';
            }
        });

        // Закрытие меню при скролле (опционально)
        let lastScrollTop = 0;
        window.addEventListener('scroll', function() {
            const scrollTop = window.pageYOffset || document.documentElement.scrollTop;
            
            if (scrollTop > lastScrollTop && mobileMenu.classList.contains('active')) {
                // Скролл вниз - закрываем меню
                mobileMenuToggle.classList.remove('active');
                mobileMenu.classList.remove('active');
                body.style.overflow = '';
            }
            
            lastScrollTop = scrollTop;
        });
    }

    // Добавляем отступ для body, чтобы контент не скрывался под фиксированной шапкой
    function adjustBodyPadding() {
        const header = document.querySelector('.header');
        if (header) {
            const headerHeight = header.offsetHeight;
            body.style.paddingTop = headerHeight + 'px';
        }
    }

    // Вызываем функцию при загрузке и изменении размера экрана
    adjustBodyPadding();
    window.addEventListener('resize', adjustBodyPadding);

    // Обработка изменения ориентации устройства
    window.addEventListener('orientationchange', function() {
        setTimeout(adjustBodyPadding, 100);
    });
});
