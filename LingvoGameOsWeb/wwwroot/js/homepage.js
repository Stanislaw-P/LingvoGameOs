document.addEventListener('DOMContentLoaded', function () {
    // Initialize all homepage components
    initCarousels();
    initFAQ();
    initSmoothScrolling();
    initAnimations();
    initCTA();

    // Initialize enhanced category cards
    initializeEnhancedCategoryCards();

    // Initialize existing homepage functionality
    initializeHomepageFeatures();
});

// Carousel functionality
function initCarousels() {
    const carousels = document.querySelectorAll('.homepage-categories__carousel');

    carousels.forEach(carousel => {
        const list = carousel.querySelector('.homepage-categories__list');
        const prevBtn = carousel.parentElement.querySelector('.homepage-categories__prev, .homepage-games__prev');
        const nextBtn = carousel.parentElement.querySelector('.homepage-categories__next, .homepage-games__next');
        const counter = carousel.parentElement.querySelector('.homepage-carousel-page-counter');

        if (!list || !prevBtn || !nextBtn) return;

        const items = list.children;
        const itemWidth = items[0].offsetWidth + 32; // 32px is gap
        const visibleItems = Math.floor(carousel.offsetWidth / itemWidth);
        const totalPages = Math.ceil(items.length / visibleItems);
        let currentPage = 0;

        // Update counter
        function updateCounter() {
            if (counter) {
                counter.textContent = `${currentPage + 1} из ${totalPages}`;
            }
        }

        // Update carousel position
        function updateCarousel() {
            const translateX = -currentPage * itemWidth * visibleItems;
            list.style.transform = `translateX(${translateX}px)`;
            updateCounter();

            // Update button states
            prevBtn.disabled = currentPage === 0;
            nextBtn.disabled = currentPage === totalPages - 1;
        }

        // Event listeners
        prevBtn.addEventListener('click', () => {
            if (currentPage > 0) {
                currentPage--;
                updateCarousel();
            }
        });

        nextBtn.addEventListener('click', () => {
            if (currentPage < totalPages - 1) {
                currentPage++;
                updateCarousel();
            }
        });

        // Initialize
        updateCarousel();

        // Handle window resize
        window.addEventListener('resize', () => {
            const newVisibleItems = Math.floor(carousel.offsetWidth / itemWidth);
            if (newVisibleItems !== visibleItems) {
                location.reload(); // Simple solution for responsive carousel
            }
        });
    });
}

// FAQ Accordion functionality
function initFAQ() {
    const faqItems = document.querySelectorAll('.homepage-faq__item');

    faqItems.forEach(item => {
        const question = item.querySelector('.homepage-faq__question');

        question.addEventListener('click', () => {
            const isActive = item.classList.contains('active');

            // Close all other items
            faqItems.forEach(otherItem => {
                if (otherItem !== item) {
                    otherItem.classList.remove('active');
                }
            });

            // Toggle current item
            if (isActive) {
                item.classList.remove('active');
            } else {
                item.classList.add('active');
            }
        });
    });
}

// Smooth scrolling for anchor links
function initSmoothScrolling() {
    const anchorLinks = document.querySelectorAll('a[href^="#"]');

    anchorLinks.forEach(link => {
        link.addEventListener('click', (e) => {
            e.preventDefault();

            const targetId = link.getAttribute('href').substring(1);
            const targetElement = document.getElementById(targetId);

            if (targetElement) {
                const headerHeight = document.querySelector('.header')?.offsetHeight || 0;
                const targetPosition = targetElement.offsetTop - headerHeight - 20;

                window.scrollTo({
                    top: targetPosition,
                    behavior: 'smooth'
                });
            }
        });
    });
}

// Animation on scroll
function initAnimations() {
    const observerOptions = {
        threshold: 0.1,
        rootMargin: '0px 0px -50px 0px'
    };

    const observer = new IntersectionObserver((entries) => {
        entries.forEach(entry => {
            if (entry.isIntersecting) {
                entry.target.classList.add('animate-in');
            }
        });
    }, observerOptions);

    // Observe elements for animation
    //const animateElements = document.querySelectorAll(
    //    '.homepage-categories__item, .homepage-games__item, .homepage-new-games__item, .homepage-reviews__card, .homepage-faq__item'
    //);

    //animateElements.forEach(el => {
    //    observer.observe(el);
    //});
}

// CTA Section Animations and Interactions
function initCTA() {
    const ctaButton = document.querySelector('.homepage-cta__button');
    const ctaIcon = document.querySelector('.homepage-cta__icon');
    const ctaStats = document.querySelectorAll('.homepage-cta__stat');

    // Button hover effects
    if (ctaButton) {
        ctaButton.addEventListener('mouseenter', function () {
            this.style.transform = 'translateY(-3px)';
        });

        ctaButton.addEventListener('mouseleave', function () {
            this.style.transform = 'translateY(0)';
        });
    }

    // Icon hover animation
    if (ctaIcon) {
        ctaIcon.addEventListener('mouseenter', function () {
            this.style.transform = 'scale(1.1)';
        });

        ctaIcon.addEventListener('mouseleave', function () {
            this.style.transform = 'scale(1)';
        });
    }

    // Stats hover effects
    ctaStats.forEach((stat) => {
        stat.addEventListener('mouseenter', function () {
            this.style.transform = 'translateY(-2px)';
        });

        stat.addEventListener('mouseleave', function () {
            this.style.transform = 'translateY(0)';
        });
    });
}

// Utility functions
function debounce(func, wait) {
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

// Handle window resize
window.addEventListener('resize', debounce(() => {
    // Reinitialize carousels on resize
    initCarousels();
}, 250));


// Success notification
function showSuccess(message) {
    const successDiv = document.createElement('div');
    successDiv.className = 'homepage-success';
    successDiv.textContent = message;
    successDiv.style.cssText = `
        position: fixed;
        top: 20px;
        right: 20px;
        background: #51d546;
        color: white;
        padding: 1rem;
        border-radius: 8px;
        z-index: 1000;
        animation: slideInRight 0.3s ease;
    `;

    document.body.appendChild(successDiv);

    setTimeout(() => {
        successDiv.remove();
    }, 3000);
}

// Handle form submissions
document.addEventListener('submit', function (e) {
    if (e.target.classList.contains('homepage-form')) {
        e.preventDefault();

        const form = e.target;
        const submitBtn = form.querySelector('button[type="submit"]');

        if (submitBtn) {
            submitBtn.disabled = true;
            submitBtn.textContent = 'Отправка...';
        }

        // Simulate form submission
        setTimeout(() => {
            if (submitBtn) {
                submitBtn.disabled = false;
                submitBtn.textContent = 'Отправить';
            }
            showSuccess('Форма успешно отправлена!');
        }, 2000);
    }   
});

// Handle button clicks
document.addEventListener('click', function (e) {
    // FAQ button
    if (e.target.classList.contains('homepage-faq__button')) {
        e.preventDefault();
        showSuccess('Вопрос отправлен! Мы ответим вам в ближайшее время.');
    }

    // Reviews button
    if (e.target.classList.contains('homepage-reviews__button')) {
        e.preventDefault();
        showSuccess('Форма отзыва открыта!');
    }

    // Game buttons
    //if (e.target.classList.contains('homepage-games__button')) {
    //    // Add loading state
    //    e.target.style.pointerEvents = 'none';
    //    e.target.textContent = 'Загрузка...';

    //    setTimeout(() => {
    //        e.target.style.pointerEvents = 'auto';
    //        e.target.textContent = e.target.textContent === 'Загрузка...' ? 'Играть' : e.target.textContent;
    //    }, 1000);
    //}
});

// Add CSS for ripple effect
const style = document.createElement('style');
style.textContent = `
    /* Удаляем лишние стили, оставляем только базовые */
`;
document.head.appendChild(style);

// Homepage JavaScript - Enhanced Category Cards

function initializeEnhancedCategoryCards() {
    // Add enhanced interactions for category cards
    document.querySelectorAll('.homepage-category-card').forEach(card => {
        const images = card.querySelectorAll('.homepage-category-card__image');
        const iconWrapper = card.querySelector('.homepage-category-card__icon-wrapper');
        const arrow = card.querySelector('.homepage-category-card__arrow');
        const overlay = card.querySelector('.homepage-category-card__overlay');

        // Enhanced hover effects
        card.addEventListener('mouseenter', function () {
            // Scale images with different timing
            images.forEach((img, index) => {
                img.style.transform = 'scale(1.05)';
                img.style.transitionDelay = `${index * 0.1}s`;
            });

            // Animate icon wrapper
            if (iconWrapper) {
                iconWrapper.style.transform = 'scale(1.1) rotate(5deg)';
            }

            // Animate arrow
            if (arrow) {
                arrow.style.transform = 'translateX(6px)';
            }

            // Show overlay with animation
            if (overlay) {
                overlay.style.opacity = '0.1';
            }

            // Add subtle glow effect
            card.style.boxShadow = '0 20px 40px rgba(0, 0, 0, 0.2)';
        });

        card.addEventListener('mouseleave', function () {
            // Reset image transforms
            images.forEach(img => {
                img.style.transform = 'scale(1)';
                img.style.transitionDelay = '0s';
            });

            // Reset icon wrapper
            if (iconWrapper) {
                iconWrapper.style.transform = 'scale(1) rotate(0deg)';
            }

            // Reset arrow
            if (arrow) {
                arrow.style.transform = 'translateX(0)';
            }

            // Hide overlay
            if (overlay) {
                overlay.style.opacity = '0';
            }

            // Reset shadow
            card.style.boxShadow = '0 4px 15px rgba(0, 0, 0, 0.1)';
        });

        // Add click feedback
        card.addEventListener('click', function (e) {
            createRippleEffect(e, card);
        });

        // Add keyboard navigation
        card.addEventListener('keydown', function (e) {
            if (e.key === 'Enter' || e.key === ' ') {
                e.preventDefault();
                card.click();
            }
        });
    });
}

function initializeHomepageFeatures() {
    // Initialize existing carousel functionality
    initializeCarousels();

    // Initialize smooth scrolling
    initializeSmoothScrolling();

    // Initialize animations
    initializeAnimations();
}

function initializeCarousels() {
    // Category carousel
    const categoryCarousel = document.querySelector('.homepage-categories__carousel');
    if (categoryCarousel) {
        // Add touch/swipe support for mobile
        let startX = 0;
        let currentX = 0;

        categoryCarousel.addEventListener('touchstart', function (e) {
            startX = e.touches[0].clientX;
        });

        categoryCarousel.addEventListener('touchmove', function (e) {
            currentX = e.touches[0].clientX;
        });

        categoryCarousel.addEventListener('touchend', function () {
            const diff = startX - currentX;
            if (Math.abs(diff) > 50) {
                // Swipe detected - you can add carousel navigation here
                console.log('Swipe detected:', diff > 0 ? 'left' : 'right');
            }
        });
    }
}

function initializeSmoothScrolling() {
    // Smooth scrolling for anchor links
    document.querySelectorAll('a[href^="#"]').forEach(anchor => {
        anchor.addEventListener('click', function (e) {
            e.preventDefault();
            const target = document.querySelector(this.getAttribute('href'));
            if (target) {
                target.scrollIntoView({
                    behavior: 'smooth',
                    block: 'start'
                });
            }
        });
    });
}

function initializeAnimations() {
    // Animate elements on scroll
    const observerOptions = {
        threshold: 0.1,
        rootMargin: '0px 0px -50px 0px'
    };

    const observer = new IntersectionObserver((entries) => {
        entries.forEach(entry => {
            if (entry.isIntersecting) {
                entry.target.style.opacity = '1';
                entry.target.style.transform = 'translateY(0)';
            }
        });
    }, observerOptions);

    // Observe category cards
    document.querySelectorAll('.homepage-category-card').forEach(card => {
        card.style.opacity = '0';
        card.style.transform = 'translateY(30px)';
        card.style.transition = 'opacity 0.6s ease, transform 0.6s ease';
        observer.observe(card);
    });

    // Observe game cards
    //document.querySelectorAll('.homepage-games__item').forEach(item => {
    //    item.style.opacity = '0';
    //    item.style.transform = 'translateY(30px)';
    //    item.style.transition = 'opacity 0.6s ease, transform 0.6s ease';
    //    observer.observe(item);
    //});
}

function createRippleEffect(event, element) {
    const ripple = document.createElement('span');
    const rect = element.getBoundingClientRect();
    const size = Math.max(rect.width, rect.height);
    const x = event.clientX - rect.left - size / 2;
    const y = event.clientY - rect.top - size / 2;

    ripple.style.width = ripple.style.height = size + 'px';
    ripple.style.left = x + 'px';
    ripple.style.top = y + 'px';
    ripple.classList.add('ripple');

    element.appendChild(ripple);

    setTimeout(() => {
        ripple.remove();
    }, 600);
}

// Add CSS for ripple effect and animations
const enhancedStyles = `
    .ripple {
        position: absolute;
        border-radius: 50%;
        background: rgba(255, 255, 255, 0.3);
        transform: scale(0);
        animation: ripple-animation 0.6s linear;
        pointer-events: none;
    }
    
    @keyframes ripple-animation {
        to {
            transform: scale(4);
            opacity: 0;
        }
    }
    
    .homepage-category-card {
        position: relative;
        overflow: hidden;
    }
    
    .homepage-category-card__image {
        transition: transform 0.3s ease;
    }
    
    .homepage-category-card__icon-wrapper {
        transition: transform 0.3s ease;
    }
    
    .homepage-category-card__arrow {
        transition: transform 0.3s ease;
    }
    
    .homepage-category-card__overlay {
        transition: opacity 0.3s ease;
    }
    
    /* Enhanced hover states */
    .homepage-category-card:hover {
        transform: translateY(-8px);
        box-shadow: 0 20px 40px rgba(0, 0, 0, 0.15);
    }
    
    /* Loading states */
    .homepage-category-card.loading {
        pointer-events: none;
    }
    
    .homepage-category-card.loading::after {
        content: '';
        position: absolute;
        top: 50%;
        left: 50%;
        width: 20px;
        height: 20px;
        margin: -10px 0 0 -10px;
        border: 2px solid rgba(255, 255, 255, 0.3);
        border-radius: 50%;
        border-top-color: #fff;
        animation: spin 1s ease-in-out infinite;
    }
    
    @keyframes spin {
        to { transform: rotate(360deg); }
    }
`;

// Inject styles
const styleSheet = document.createElement('style');
styleSheet.textContent = enhancedStyles;
document.head.appendChild(styleSheet);

// Add loading states for better UX
document.querySelectorAll('.homepage-category-card').forEach(card => {
    card.addEventListener('click', function () {
        // Add loading state
        this.classList.add('loading');

        // Remove loading state after navigation
        setTimeout(() => {
            this.classList.remove('loading');
        }, 1000);
    });
});


// Optimize scroll events
const optimizedScrollHandler = debounce(() => {
    // Add any scroll-based animations here
}, 16);

window.addEventListener('scroll', optimizedScrollHandler);

// Add accessibility improvements
document.querySelectorAll('.homepage-category-card').forEach(element => {
    element.setAttribute('tabindex', '0');
    element.setAttribute('role', 'button');
    element.setAttribute('aria-label', element.querySelector('.homepage-category-card__title')?.textContent || 'Category card');
});

// Add keyboard navigation support
document.addEventListener('keydown', function (e) {
    const focusableElements = document.querySelectorAll('.homepage-category-card');
    const currentIndex = Array.from(focusableElements).indexOf(document.activeElement);

    if (e.key === 'ArrowRight' || e.key === 'ArrowDown') {
        e.preventDefault();
        const nextIndex = (currentIndex + 1) % focusableElements.length;
        focusableElements[nextIndex].focus();
    } else if (e.key === 'ArrowLeft' || e.key === 'ArrowUp') {
        e.preventDefault();
        const prevIndex = currentIndex <= 0 ? focusableElements.length - 1 : currentIndex - 1;
        focusableElements[prevIndex].focus();
    }
});

console.log('Enhanced homepage JavaScript loaded successfully!'); 