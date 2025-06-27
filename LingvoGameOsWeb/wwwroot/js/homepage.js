// Homepage JavaScript - Interactive functionality
document.addEventListener('DOMContentLoaded', function() {
    // Initialize all homepage components
    initCarousels();
    initFAQ();
    initReviewsNavigation();
    initSmoothScrolling();
    initAnimations();
});

// Carousel functionality
function initCarousels() {
    const carousels = document.querySelectorAll('.homepage-categories__carousel, .homepage-games__carousel');
    
    carousels.forEach(carousel => {
        const list = carousel.querySelector('.homepage-categories__list, .homepage-games__list');
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

// Reviews navigation functionality
function initReviewsNavigation() {
    const reviewsList = document.querySelector('.homepage-reviews__list');
    const indicators = document.querySelectorAll('.homepage-reviews__indicator');
    const prevBtn = document.querySelector('.homepage-reviews__nav-button--prev');
    const nextBtn = document.querySelector('.homepage-reviews__nav-button--next');
    
    if (!reviewsList || indicators.length === 0) return;
    
    const reviews = reviewsList.children;
    let currentReview = 0;
    
    // Update active review
    function updateReview() {
        // Hide all reviews
        Array.from(reviews).forEach((review, index) => {
            review.style.display = index === currentReview ? 'block' : 'none';
        });
        
        // Update indicators
        indicators.forEach((indicator, index) => {
            indicator.classList.toggle('active', index === currentReview);
        });
        
        // Update button states
        if (prevBtn) prevBtn.disabled = currentReview === 0;
        if (nextBtn) nextBtn.disabled = currentReview === reviews.length - 1;
    }
    
    // Event listeners for indicators
    indicators.forEach((indicator, index) => {
        indicator.addEventListener('click', () => {
            currentReview = index;
            updateReview();
        });
    });
    
    // Event listeners for navigation buttons
    if (prevBtn) {
        prevBtn.addEventListener('click', () => {
            if (currentReview > 0) {
                currentReview--;
                updateReview();
            }
        });
    }
    
    if (nextBtn) {
        nextBtn.addEventListener('click', () => {
            if (currentReview < reviews.length - 1) {
                currentReview++;
                updateReview();
            }
        });
    }
    
    // Initialize
    updateReview();
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
    const animateElements = document.querySelectorAll(
        '.homepage-categories__item, .homepage-games__item, .homepage-new-games__item, .homepage-reviews__card, .homepage-faq__item'
    );
    
    animateElements.forEach(el => {
        observer.observe(el);
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

// Add loading states
function showLoading(element) {
    element.classList.add('loading');
    element.innerHTML = '<div class="homepage-loading"><div class="homepage-loading__spinner"></div>Загрузка...</div>';
}

function hideLoading(element) {
    element.classList.remove('loading');
}

// Error handling
function showError(message) {
    const errorDiv = document.createElement('div');
    errorDiv.className = 'homepage-error';
    errorDiv.textContent = message;
    errorDiv.style.cssText = `
        position: fixed;
        top: 20px;
        right: 20px;
        background: #d54646;
        color: white;
        padding: 1rem;
        border-radius: 8px;
        z-index: 1000;
        animation: slideInRight 0.3s ease;
    `;
    
    document.body.appendChild(errorDiv);
    
    setTimeout(() => {
        errorDiv.remove();
    }, 5000);
}

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
document.addEventListener('submit', function(e) {
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
document.addEventListener('click', function(e) {
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
    if (e.target.classList.contains('homepage-games__button')) {
        // Add loading state
        e.target.style.pointerEvents = 'none';
        e.target.textContent = 'Загрузка...';
        
        setTimeout(() => {
            e.target.style.pointerEvents = 'auto';
            e.target.textContent = e.target.textContent === 'Загрузка...' ? 'Играть' : e.target.textContent;
        }, 1000);
    }
});

// Add CSS animations
const style = document.createElement('style');
style.textContent = `
    .animate-in {
        animation: fadeInUp 0.6s ease-out forwards;
    }
    
    .homepage-error,
    .homepage-success {
        box-shadow: 0 4px 12px rgba(0, 0, 0, 0.15);
    }
    
    .loading {
        opacity: 0.6;
        pointer-events: none;
    }
    
    @keyframes slideInRight {
        from {
            transform: translateX(100%);
            opacity: 0;
        }
        to {
            transform: translateX(0);
            opacity: 1;
        }
    }
`;
document.head.appendChild(style); 