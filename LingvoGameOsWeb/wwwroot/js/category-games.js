// Category Games Page JavaScript

document.addEventListener('DOMContentLoaded', function() {
    // Initialize animations
    initializeAnimations();
    
    // Initialize game cards interactions
    initializeGameCards();
    
    // Initialize related categories
    initializeRelatedCategories();
    
    // Initialize breadcrumb navigation
    initializeBreadcrumb();
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
    
    // Observe game cards
    document.querySelectorAll('.category-games__item').forEach(item => {
        item.style.opacity = '0';
        item.style.transform = 'translateY(30px)';
        item.style.transition = 'opacity 0.6s ease, transform 0.6s ease';
        observer.observe(item);
    });
    
    // Observe related categories
    document.querySelectorAll('.related-categories__item').forEach(item => {
        item.style.opacity = '0';
        item.style.transform = 'translateY(20px)';
        item.style.transition = 'opacity 0.6s ease, transform 0.6s ease';
        observer.observe(item);
    });
}

function initializeGameCards() {
    // Add hover effects and interactions for game cards
    document.querySelectorAll('.category-games__item').forEach(item => {
        const link = item.querySelector('.category-games__link');
        const image = item.querySelector('.category-games__image');
        const overlay = item.querySelector('.category-games__overlay');
        const skills = item.querySelectorAll('.category-games__skill-tag');
        
        // Enhanced hover effects
        item.addEventListener('mouseenter', function() {
            // Scale image slightly
            if (image) {
                image.style.transform = 'scale(1.05)';
            }
            
            // Show overlay with animation
            if (overlay) {
                overlay.style.opacity = '1';
            }
            
            // Add glow effect to skill tags
            skills.forEach(skill => {
                skill.style.boxShadow = '0 0 8px rgba(44, 81, 174, 0.4)';
                skill.style.transform = 'scale(1.05)';
            });
            
            // Add subtle animation to rating stars
            const stars = item.querySelectorAll('.category-games__star.filled');
            stars.forEach((star, index) => {
                star.style.animationDelay = `${index * 0.1}s`;
                star.style.animation = 'starTwinkle 0.6s ease-in-out';
            });
        });
        
        item.addEventListener('mouseleave', function() {
            // Reset image scale
            if (image) {
                image.style.transform = 'scale(1)';
            }
            
            // Hide overlay
            if (overlay) {
                overlay.style.opacity = '0';
            }
            
            // Remove glow effect from skill tags
            skills.forEach(skill => {
                skill.style.boxShadow = 'none';
                skill.style.transform = 'scale(1)';
            });
            
            // Remove star animation
            const stars = item.querySelectorAll('.category-games__star.filled');
            stars.forEach(star => {
                star.style.animation = 'none';
            });
        });
        
        // Add click feedback
        link.addEventListener('click', function(e) {
            createRippleEffect(e, item);
        });
        
        // Add keyboard navigation
        item.addEventListener('keydown', function(e) {
            if (e.key === 'Enter' || e.key === ' ') {
                e.preventDefault();
                link.click();
            }
        });
    });
}

function initializeRelatedCategories() {
    // Add hover effects for related categories
    document.querySelectorAll('.related-categories__item').forEach(item => {
        const arrow = item.querySelector('svg');
        
        item.addEventListener('mouseenter', function() {
            // Animate arrow
            if (arrow) {
                arrow.style.transform = 'translateX(4px)';
                arrow.style.transition = 'transform 0.3s ease';
            }
            
            // Add subtle scale effect
            item.style.transform = 'translateY(-2px) scale(1.02)';
        });
        
        item.addEventListener('mouseleave', function() {
            // Reset arrow
            if (arrow) {
                arrow.style.transform = 'translateX(0)';
            }
            
            // Reset scale
            item.style.transform = 'translateY(0) scale(1)';
        });
        
        // Add click feedback
        item.addEventListener('click', function(e) {
            createRippleEffect(e, item);
        });
    });
}

function initializeBreadcrumb() {
    // Add hover effects for breadcrumb links
    const breadcrumbLink = document.querySelector('.category-hero__breadcrumb-link');
    if (breadcrumbLink) {
        breadcrumbLink.addEventListener('mouseenter', function() {
            this.style.textDecoration = 'underline';
        });
        
        breadcrumbLink.addEventListener('mouseleave', function() {
            this.style.textDecoration = 'none';
        });
    }
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

// Add CSS for animations
const animationStyles = `
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
    
    @keyframes starTwinkle {
        0% { opacity: 1; transform: scale(1); }
        50% { opacity: 0.7; transform: scale(1.2); }
        100% { opacity: 1; transform: scale(1); }
    }
    
    .category-games__skill-tag {
        transition: all 0.3s ease;
    }
    
    .related-categories__item {
        transition: all 0.3s ease;
    }
    
    .category-hero__breadcrumb-link {
        transition: all 0.3s ease;
    }
`;

// Inject styles
const styleSheet = document.createElement('style');
styleSheet.textContent = animationStyles;
document.head.appendChild(styleSheet);

// Add loading states
document.querySelectorAll('.category-games__link, .related-categories__item').forEach(link => {
    link.addEventListener('click', function() {
        // Add loading state
        this.style.pointerEvents = 'none';
        
        // Add loading indicator
        const originalContent = this.innerHTML;
        this.innerHTML = '<span class="loading-spinner"></span>';
        
        // Reset after navigation
        setTimeout(() => {
            this.style.pointerEvents = 'auto';
            this.innerHTML = originalContent;
        }, 1000);
    });
});

// Add loading spinner styles
const spinnerStyles = `
    .loading-spinner {
        display: inline-block;
        width: 16px;
        height: 16px;
        border: 2px solid rgba(255, 255, 255, 0.3);
        border-radius: 50%;
        border-top-color: #fff;
        animation: spin 1s ease-in-out infinite;
    }
    
    @keyframes spin {
        to { transform: rotate(360deg); }
    }
`;

const spinnerStyleSheet = document.createElement('style');
spinnerStyleSheet.textContent = spinnerStyles;
document.head.appendChild(spinnerStyleSheet);

// Add smooth scrolling
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
document.querySelectorAll('.category-games__item, .related-categories__item').forEach(element => {
    element.setAttribute('tabindex', '0');
    element.setAttribute('role', 'button');
});

// Add keyboard navigation support
document.addEventListener('keydown', function(e) {
    const focusableElements = document.querySelectorAll('.category-games__link, .related-categories__item');
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

// Add error handling for missing elements
function safeQuerySelector(selector) {
    try {
        return document.querySelector(selector);
    } catch (error) {
        console.warn(`Element not found: ${selector}`);
        return null;
    }
}

// Add analytics tracking (if needed)
function trackCategoryView(categoryName) {
    // You can add analytics tracking here
    console.log(`Category viewed: ${categoryName}`);
}

// Track category view on page load
const categoryTitle = safeQuerySelector('.category-hero__title');
if (categoryTitle) {
    trackCategoryView(categoryTitle.textContent);
}

console.log('Category games page JavaScript loaded successfully!'); 