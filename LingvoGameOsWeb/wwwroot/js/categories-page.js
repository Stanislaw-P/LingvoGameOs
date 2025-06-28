// Categories Page JavaScript

document.addEventListener('DOMContentLoaded', function() {
    // Initialize animations
    initializeAnimations();
    
    // Initialize category cards interactions
    initializeCategoryCards();
    
    // Initialize featured games interactions
    initializeFeaturedGames();
});

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
    document.querySelectorAll('.category-card').forEach(card => {
        card.style.opacity = '0';
        card.style.transform = 'translateY(30px)';
        card.style.transition = 'opacity 0.6s ease, transform 0.6s ease';
        observer.observe(card);
    });
    
    // Observe featured games
    document.querySelectorAll('.featured-games__item').forEach(item => {
        item.style.opacity = '0';
        item.style.transform = 'translateY(30px)';
        item.style.transition = 'opacity 0.6s ease, transform 0.6s ease';
        observer.observe(item);
    });
}

function initializeCategoryCards() {
    // Add hover effects and interactions
    document.querySelectorAll('.category-card').forEach(card => {
        const link = card.querySelector('.category-card__link');
        const arrow = card.querySelector('.category-card__arrow');
        const images = card.querySelectorAll('.category-card__image');
        
        // Enhanced hover effects
        card.addEventListener('mouseenter', function() {
            // Add subtle scale effect to images
            images.forEach(img => {
                img.style.transform = 'scale(1.05)';
            });
            
            // Animate arrow
            if (arrow) {
                arrow.style.transform = 'translateX(6px)';
            }
            
            // Add pulse effect to icon
            const iconWrapper = card.querySelector('.category-card__icon-wrapper');
            if (iconWrapper) {
                iconWrapper.style.animation = 'pulse 0.6s ease-in-out';
            }
        });
        
        card.addEventListener('mouseleave', function() {
            // Reset image transforms
            images.forEach(img => {
                img.style.transform = 'scale(1)';
            });
            
            // Reset arrow
            if (arrow) {
                arrow.style.transform = 'translateX(0)';
            }
            
            // Remove pulse animation
            const iconWrapper = card.querySelector('.category-card__icon-wrapper');
            if (iconWrapper) {
                iconWrapper.style.animation = 'none';
            }
        });
        
        // Add click feedback
        link.addEventListener('click', function(e) {
            // Add ripple effect
            createRippleEffect(e, card);
        });
    });
}

function initializeFeaturedGames() {
    // Add hover effects for featured games
    document.querySelectorAll('.featured-games__item').forEach(item => {
        const link = item.querySelector('.featured-games__link');
        const image = item.querySelector('.featured-games__image');
        
        item.addEventListener('mouseenter', function() {
            // Enhanced image scale
            if (image) {
                image.style.transform = 'scale(1.08)';
            }
            
            // Add glow effect to category tags
            const tags = item.querySelectorAll('.featured-games__category-tag');
            tags.forEach(tag => {
                tag.style.boxShadow = '0 0 10px rgba(44, 81, 174, 0.3)';
            });
        });
        
        item.addEventListener('mouseleave', function() {
            // Reset image scale
            if (image) {
                image.style.transform = 'scale(1)';
            }
            
            // Remove glow effect
            const tags = item.querySelectorAll('.featured-games__category-tag');
            tags.forEach(tag => {
                tag.style.boxShadow = 'none';
            });
        });
        
        // Add click feedback
        link.addEventListener('click', function(e) {
            createRippleEffect(e, item);
        });
    });
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

// Add CSS for ripple effect
const rippleStyles = `
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
    
    @keyframes pulse {
        0% { transform: scale(1); }
        50% { transform: scale(1.1); }
        100% { transform: scale(1); }
    }
`;

// Inject styles
const styleSheet = document.createElement('style');
styleSheet.textContent = rippleStyles;
document.head.appendChild(styleSheet);

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

// Add loading states for better UX
document.querySelectorAll('.category-card__link, .featured-games__link').forEach(link => {
    link.addEventListener('click', function() {
        // Add loading state
        const originalText = this.textContent;
        this.style.pointerEvents = 'none';
        
        // You can add a loading spinner here if needed
        // this.innerHTML = '<span class="loading-spinner"></span> Загрузка...';
        
        // Reset after navigation (this will be handled by the new page load)
        setTimeout(() => {
            this.style.pointerEvents = 'auto';
            this.textContent = originalText;
        }, 1000);
    });
});

// Add keyboard navigation support
document.addEventListener('keydown', function(e) {
    const focusableElements = document.querySelectorAll('.category-card__link, .featured-games__link, .featured-games__view-all');
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

// Add performance optimizations
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

// Optimize scroll events
const optimizedScrollHandler = debounce(() => {
    // Add any scroll-based animations here
}, 16);

window.addEventListener('scroll', optimizedScrollHandler);

// Add accessibility improvements
document.querySelectorAll('.category-card, .featured-games__item').forEach(element => {
    element.setAttribute('tabindex', '0');
    element.setAttribute('role', 'button');
    
    element.addEventListener('keydown', function(e) {
        if (e.key === 'Enter' || e.key === ' ') {
            e.preventDefault();
            const link = this.querySelector('a');
            if (link) {
                link.click();
            }
        }
    });
});

console.log('Categories page JavaScript loaded successfully!'); 