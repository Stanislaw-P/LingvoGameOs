// News page functionality
document.addEventListener('DOMContentLoaded', function() {
    // Initialize news page components
    initNewsPage();
});

function initNewsPage() {
    // Initialize category filtering
    initCategoryFilter();
    
    // Initialize subscription form
    initSubscriptionForm();
    
    // Initialize pagination
    initPagination();
    
    // Initialize news card interactions
    initNewsCards();
}

// Category filtering functionality
function initCategoryFilter() {
    const categories = document.querySelectorAll('.news-category');
    const newsCards = document.querySelectorAll('.news-card');
    
    categories.forEach(category => {
        category.addEventListener('click', function() {
            // Remove active class from all categories
            categories.forEach(c => c.classList.remove('active'));
            
            // Add active class to clicked category
            this.classList.add('active');
            
            const selectedCategory = this.textContent.trim();
            
            // Filter news cards
            newsCards.forEach(card => {
                const cardCategory = card.querySelector('.news-card__category').textContent.trim();
                
                if (selectedCategory === 'Все' || cardCategory === selectedCategory) {
                    card.style.display = 'block';
                    card.style.animation = 'fadeInUp 0.6s ease forwards';
                } else {
                    card.style.display = 'none';
                }
            });
        });
    });
}

// Subscription form functionality
function initSubscriptionForm() {
    const form = document.querySelector('.news-subscription__form');
    const input = document.querySelector('.news-subscription__input');
    const button = document.querySelector('.news-subscription__button');
    
    if (form) {
        form.addEventListener('submit', function(e) {
            e.preventDefault();
            
            const email = input.value.trim();
            
            if (validateEmail(email)) {
                // Show success message
                showNotification('Спасибо! Вы успешно подписались на новости.', 'success');
                input.value = '';
                
                // Simulate API call
                setTimeout(() => {
                    console.log('Subscription email sent:', email);
                }, 1000);
            } else {
                showNotification('Пожалуйста, введите корректный email адрес.', 'error');
            }
        });
    }
    
    // Real-time email validation
    if (input) {
        input.addEventListener('input', function() {
            const email = this.value.trim();
            
            if (email && !validateEmail(email)) {
                this.style.borderColor = 'var(--error-color)';
            } else {
                this.style.borderColor = 'var(--border-outline)';
            }
        });
    }
}

// Pagination functionality
function initPagination() {
    const paginationButtons = document.querySelectorAll('.news-pagination__button');
    
    paginationButtons.forEach(button => {
        button.addEventListener('click', function() {
            if (this.disabled) return;
            
            // Remove active class from all buttons
            paginationButtons.forEach(btn => btn.classList.remove('active'));
            
            // Add active class to clicked button
            this.classList.add('active');
            
            // Simulate page change
            simulatePageChange();
        });
    });
}

// News card interactions
function initNewsCards() {
    const newsCards = document.querySelectorAll('.news-card');
    
    newsCards.forEach(card => {
        card.addEventListener('click', function() {
            // Add click animation
            this.style.transform = 'scale(0.98)';
            
            setTimeout(() => {
                this.style.transform = '';
            }, 150);
            
            // Simulate navigation to news detail page
            const title = this.querySelector('.news-card__title').textContent;
            console.log('Navigating to news detail:', title);
            
            // Here you would typically navigate to the news detail page
            // window.location.href = `/news/detail/${newsId}`;
        });
        
        // Add hover effects
        card.addEventListener('mouseenter', function() {
            this.style.transform = 'translateY(-4px) scale(1.02)';
        });
        
        card.addEventListener('mouseleave', function() {
            this.style.transform = '';
        });
    });
}

// Sidebar item interactions
function initSidebarItems() {
    const sidebarItems = document.querySelectorAll('.news-sidebar__item');
    
    sidebarItems.forEach(item => {
        item.addEventListener('click', function() {
            const title = this.querySelector('.news-sidebar__item-title').textContent;
            console.log('Navigating to news:', title);
            
            // Here you would typically navigate to the news detail page
            // window.location.href = `/news/detail/${newsId}`;
        });
    });
}

// Utility functions
function validateEmail(email) {
    const emailRegex = /^[^\s@]+@[^\s@]+\.[^\s@]+$/;
    return emailRegex.test(email);
}

function showNotification(message, type = 'info') {
    // Create notification element
    const notification = document.createElement('div');
    notification.className = `news-notification news-notification--${type}`;
    notification.textContent = message;
    
    // Add styles
    notification.style.cssText = `
        position: fixed;
        top: 20px;
        right: 20px;
        padding: 16px 24px;
        border-radius: 8px;
        color: white;
        font-family: 'Axiforma', sans-serif;
        font-size: 14px;
        z-index: 1000;
        transform: translateX(100%);
        transition: transform 0.3s ease;
        max-width: 300px;
    `;
    
    // Set background color based on type
    if (type === 'success') {
        notification.style.background = 'var(--success-color)';
    } else if (type === 'error') {
        notification.style.background = 'var(--error-color)';
    } else {
        notification.style.background = 'var(--accent-2)';
    }
    
    // Add to page
    document.body.appendChild(notification);
    
    // Animate in
    setTimeout(() => {
        notification.style.transform = 'translateX(0)';
    }, 100);
    
    // Remove after 3 seconds
    setTimeout(() => {
        notification.style.transform = 'translateX(100%)';
        setTimeout(() => {
            document.body.removeChild(notification);
        }, 300);
    }, 3000);
}

function simulatePageChange() {
    // Add loading animation
    const newsContainer = document.querySelector('.news-featured');
    if (newsContainer) {
        newsContainer.style.opacity = '0.5';
        newsContainer.style.transition = 'opacity 0.3s ease';
        
        setTimeout(() => {
            newsContainer.style.opacity = '1';
        }, 500);
    }
}

// Search functionality (if needed)
function initSearch() {
    const searchInput = document.querySelector('.news-search__input');
    
    if (searchInput) {
        searchInput.addEventListener('input', function() {
            const query = this.value.toLowerCase().trim();
            const newsCards = document.querySelectorAll('.news-card');
            
            newsCards.forEach(card => {
                const title = card.querySelector('.news-card__title').textContent.toLowerCase();
                const excerpt = card.querySelector('.news-card__excerpt').textContent.toLowerCase();
                
                if (title.includes(query) || excerpt.includes(query)) {
                    card.style.display = 'block';
                } else {
                    card.style.display = 'none';
                }
            });
        });
    }
}

// Initialize all components when DOM is loaded
document.addEventListener('DOMContentLoaded', function() {
    initNewsPage();
    initSidebarItems();
    initSearch();
});

// Export functions for potential use in other modules
window.NewsPage = {
    init: initNewsPage,
    showNotification: showNotification,
    validateEmail: validateEmail
}; 