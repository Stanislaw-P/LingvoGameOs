export function initializeSlider() {
    const slides = document.querySelectorAll('.game-hero__slide');
    const prevButton = document.querySelector('.game-hero__carousel-prev');
    const nextButton = document.querySelector('.game-hero__carousel-next');
    const indicators = document.querySelectorAll('.game-hero__indicator');
    let currentSlide = 0;
    const slideCount = slides.length;
    // Определяем количество активных слайдов (не с классом game-hero__slide--disabled)
    const activeSlideCount = Array.from(slides).filter(slide => !slide.classList.contains('game-hero__slide--disabled')).length;
    let autoSlide;

    function isVideoSlide(index) {
        return slides[index]?.classList.contains('game-hero__video-slide');
    }

    function pauseVideo(slide) {
        const iframe = slide.querySelector('iframe');
        if (iframe) {
            iframe.contentWindow.postMessage('{"event":"command","func":"pauseVideo","args":""}', '*');
        }
    }

    function showSlide(index) {
        // Ограничиваем индекс активными слайдами
        if (index >= activeSlideCount) {
            currentSlide = 0;
        } else if (index < 0) {
            currentSlide = activeSlideCount - 1;
        } else {
            currentSlide = index;
        }

        // Остановить все видео при переключении слайда
        slides.forEach(slide => pauseVideo(slide));

        slides.forEach((slide) => {
            slide.style.transform = `translateX(-${currentSlide * 100}%)`;
        });

        indicators.forEach((indicator, i) => {
            indicator.classList.toggle('active', i === currentSlide);
            // Отключаем индикаторы для неактивных слайдов
            if (i >= activeSlideCount) {
                indicator.classList.add('disabled');
                indicator.style.pointerEvents = 'none';
                indicator.style.opacity = '0.5';
            }
        });

        // Управление доступностью кнопок
        prevButton.disabled = currentSlide === 0;
        nextButton.disabled = currentSlide === activeSlideCount - 1;

        // Управление автопрокруткой
        clearInterval(autoSlide);
        if (!isVideoSlide(currentSlide)) {
            autoSlide = setInterval(() => {
                showSlide(currentSlide + 1);
            }, 5000);
        }
    }

    // Обработчик для кнопки "назад"
    prevButton.addEventListener('click', () => {
        showSlide(currentSlide - 1);
    });

    // Обработчик для кнопки "вперед"
    nextButton.addEventListener('click', () => {
        showSlide(currentSlide + 1);
    });

    // Обработчики для индикаторов
    indicators.forEach((indicator, index) => {
        indicator.addEventListener('click', () => {
            if (index < activeSlideCount) { // Игнорируем клики по неактивным индикаторам
                showSlide(index);
            }
        });
    });

    // Инициализация
    showSlide(currentSlide);
}