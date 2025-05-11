export function initializeSlider() {
    const slides = document.querySelectorAll('.game-hero__slide');
    const prevButton = document.querySelector('.game-hero__carousel-prev');
    const nextButton = document.querySelector('.game-hero__carousel-next');
    const indicators = document.querySelectorAll('.game-hero__indicator');
    let currentSlide = 0;
    const slideCount = slides.length;

    function showSlide(index) {
        if (index >= slideCount) currentSlide = 0;
        else if (index < 0) currentSlide = slideCount - 1;
        else currentSlide = index;

        // Остановить все видео при переключении слайда
        slides.forEach(slide => {
            const iframe = slide.querySelector('iframe');
            if (iframe) {
                iframe.contentWindow.postMessage('{"event":"command","func":"pauseVideo","args":""}', '*');
            }
        });

        slides.forEach((slide, i) => {
            slide.style.transform = `translateX(-${currentSlide * 100}%)`;
        });
        indicators.forEach((indicator, i) => {
            indicator.classList.toggle('active', i === currentSlide);
        });
    }

    let autoSlide = setInterval(() => {
        showSlide(currentSlide + 1);
    }, 5000);

    prevButton.addEventListener('click', () => {
        clearInterval(autoSlide);
        showSlide(currentSlide - 1);
        autoSlide = setInterval(() => {
            showSlide(currentSlide + 1);
        }, 5000);
    });

    nextButton.addEventListener('click', () => {
        clearInterval(autoSlide);
        showSlide(currentSlide + 1);
        autoSlide = setInterval(() => {
            showSlide(currentSlide + 1);
        }, 5000);
    });

    indicators.forEach((indicator, index) => {
        indicator.addEventListener('click', () => {
            clearInterval(autoSlide);
            showSlide(index);
            autoSlide = setInterval(() => {
                showSlide(currentSlide + 1);
            }, 5000);
        });
    });

    showSlide(currentSlide);
}