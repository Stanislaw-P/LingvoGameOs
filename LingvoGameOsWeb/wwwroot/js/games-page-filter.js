// games-filter.js - ES6 модуль
export function initGamesFilter() {
    console.log('Инициализация фильтрации игр');
    
    // Dropdown логика
    initDropdowns();
    
    // Фильтрация
    initFilters();
}

function initDropdowns() {
    const toggles = document.querySelectorAll('.games__dropdown-toggle');
    console.log('Найдено dropdown кнопок:', toggles.length);
    
    toggles.forEach(function(toggle, index) {
        toggle.addEventListener('click', function(e) {
            e.preventDefault();
            e.stopPropagation();
            console.log('Клик по dropdown', index);
            
            const menu = this.nextElementSibling;
            console.log('Меню найдено:', menu);
            console.log('Меню классы:', menu.className);
            
            const isActive = menu.classList.contains('active');
            console.log('Меню активно:', isActive);
            
            // Закрываем все остальные
            document.querySelectorAll('.games__dropdown-menu').forEach(m => m.classList.remove('active'));
            document.querySelectorAll('.games__dropdown-toggle').forEach(t => t.classList.remove('active'));
            
            if (!isActive) {
                menu.classList.add('active');
                this.classList.add('active');
                console.log('Открыто меню', index);
                console.log('Меню классы после открытия:', menu.className);
                
                // Проверяем через 100мс, не удалился ли класс
                setTimeout(() => {
                    console.log('Меню классы через 100мс:', menu.className);
                    console.log('Меню активно через 100мс:', menu.classList.contains('active'));
                    console.log('Меню видимо через 100мс:', menu.style.visibility);
                    console.log('Меню opacity через 100мс:', menu.style.opacity);
                }, 100);
            }
        });
    });
    
    // Закрытие при клике вне
    document.addEventListener('click', function(e) {
        if (!e.target.closest('.games__dropdown')) {
            document.querySelectorAll('.games__dropdown-menu').forEach(m => m.classList.remove('active'));
            document.querySelectorAll('.games__dropdown-toggle').forEach(t => t.classList.remove('active'));
        }
    });
}

function initFilters() {
    const savedFilters = JSON.parse(localStorage.getItem('gameFilters')) || {
        category: [],
        keyword: [],
        rating: []
    };
    
    const filterItems = document.querySelectorAll('.games__dropdown-item');
    console.log('Найдено элементов фильтрации:', filterItems.length);
    
    filterItems.forEach(function(item) {
        const filterType = item.dataset.filter;
        const value = item.dataset.value;
        
        if (savedFilters[filterType] && savedFilters[filterType].includes(value)) {
            item.classList.add('selected');
        }
        
        item.addEventListener('click', function(e) {
            e.stopPropagation();
            
            const filterType = this.dataset.filter;
            const value = this.dataset.value;
            const isSelected = this.classList.contains('selected');
            
            if (isSelected) {
                this.classList.remove('selected');
                savedFilters[filterType] = savedFilters[filterType].filter(v => v !== value);
                showNotification(`Фильтр удален: ${this.textContent.trim()}`, 'warning');
            } else {
                this.classList.add('selected');
                if (!savedFilters[filterType]) {
                    savedFilters[filterType] = [];
                }
                savedFilters[filterType].push(value);
                showNotification(`Фильтр применен: ${this.textContent.trim()}`, 'success');
            }
            
            localStorage.setItem('gameFilters', JSON.stringify(savedFilters));
            applyFilters(savedFilters);
            updateActiveFiltersDisplay(savedFilters);
        });
    });
    
    // Поиск в dropdown
    const searchInputs = document.querySelectorAll('.games__dropdown-search-input');
    searchInputs.forEach(function(input) {
        input.addEventListener('input', function() {
            const menu = this.closest('.games__dropdown-menu');
            const items = menu.querySelectorAll('.games__dropdown-item');
            const query = this.value.toLowerCase();
            
            items.forEach(function(item) {
                const text = item.textContent.toLowerCase();
                item.style.display = text.includes(query) ? 'flex' : 'none';
            });
        });
    });
    
    // Применяем фильтры при загрузке
    applyFilters(savedFilters);
    updateActiveFiltersDisplay(savedFilters);
}

function applyFilters(filters) {
    const gameItems = document.querySelectorAll('.games__item');
    let visibleCount = 0;
    
    gameItems.forEach(function(item) {
        let isVisible = true;
        
        // Фильтр по категориям
        if (filters.category.length > 0) {
            const categories = item.dataset.categories ? item.dataset.categories.split(',').map(c => c.trim().toLowerCase()) : [];
            const hasMatchingCategory = filters.category.some(cat => 
                categories.some(itemCat => itemCat === cat.toLowerCase())
            );
            if (!hasMatchingCategory) {
                isVisible = false;
            }
        }
        
        // Фильтр по ключевым словам
        if (filters.keyword.length > 0 && isVisible) {
            const keywords = item.dataset.keywords ? item.dataset.keywords.split(',').map(k => k.trim().toLowerCase()) : [];
            const hasMatchingKeyword = filters.keyword.some(keyword => 
                keywords.some(itemKeyword => itemKeyword === keyword.toLowerCase())
            );
            if (!hasMatchingKeyword) {
                isVisible = false;
            }
        }
        
        // Фильтр по рейтингу
        if (filters.rating.length > 0 && isVisible) {
            const rating = parseInt(item.dataset.rating) || 0;
            const hasMatchingRating = filters.rating.some(r => rating >= parseInt(r));
            if (!hasMatchingRating) {
                isVisible = false;
            }
        }
        
        item.style.display = isVisible ? 'flex' : 'none';
        if (isVisible) visibleCount++;
    });
    
    if (visibleCount === 0) {
        showNotification('Игры не найдены. Попробуйте изменить фильтры.', 'warning');
    } else if (visibleCount < gameItems.length) {
        showNotification(`Найдено игр: ${visibleCount} из ${gameItems.length}`, 'success');
    }
}

function updateActiveFiltersDisplay(filters) {
    const activeFiltersContainer = document.getElementById('active-filters');
    const activeFiltersList = document.getElementById('active-filters-list');
    
    const allFilters = [...filters.category, ...filters.keyword, ...filters.rating];
    
    if (allFilters.length > 0) {
        activeFiltersList.innerHTML = '';
        allFilters.forEach(function(filter) {
            const filterTag = document.createElement('span');
            filterTag.className = 'games__filter-tag';
            filterTag.textContent = filter;
            filterTag.addEventListener('click', function() {
                removeFilter(filter);
            });
            activeFiltersList.appendChild(filterTag);
        });
        activeFiltersContainer.style.display = 'flex';
    } else {
        activeFiltersContainer.style.display = 'none';
    }
    
    updateDropdownIndicators(filters);
}

function updateDropdownIndicators(filters) {
    const dropdownToggles = document.querySelectorAll('.games__dropdown-toggle');
    
    dropdownToggles.forEach(function(toggle, index) {
        let hasFilters = false;
        
        switch(index) {
            case 0: // Категории
                hasFilters = filters.category.length > 0;
                break;
            case 1: // Ключевые слова
                hasFilters = filters.keyword.length > 0;
                break;
            case 2: // Рейтинг
                hasFilters = filters.rating.length > 0;
                break;
        }
        
        if (hasFilters) {
            toggle.classList.add('has-filters');
        } else {
            toggle.classList.remove('has-filters');
        }
    });
}

function removeFilter(filterValue) {
    const savedFilters = JSON.parse(localStorage.getItem('gameFilters')) || {
        category: [],
        keyword: [],
        rating: []
    };
    
    Object.keys(savedFilters).forEach(function(type) {
        savedFilters[type] = savedFilters[type].filter(v => v !== filterValue);
    });
    
    const filterItem = document.querySelector(`[data-value="${filterValue}"]`);
    if (filterItem) {
        filterItem.classList.remove('selected');
    }
    
    localStorage.setItem('gameFilters', JSON.stringify(savedFilters));
    applyFilters(savedFilters);
    updateActiveFiltersDisplay(savedFilters);
    showNotification(`Фильтр удален: ${filterValue}`, 'warning');
}

function showNotification(message, type) {
    let notification = document.querySelector('.notification');
    if (!notification) {
        notification = document.createElement('div');
        notification.className = 'notification';
        notification.style.cssText = `
            position: fixed;
            top: 20px;
            right: 20px;
            padding: 12px 20px;
            border-radius: 8px;
            color: white;
            font-weight: 500;
            z-index: 1000;
            opacity: 0;
            transition: opacity 0.3s ease;
            max-width: 300px;
        `;
        document.body.appendChild(notification);
    }
    
    notification.textContent = message;
    notification.className = `notification notification--${type}`;
    notification.style.display = 'block';
    notification.style.opacity = '1';
    
    setTimeout(function() {
        notification.style.opacity = '0';
        setTimeout(function() {
            notification.style.display = 'none';
        }, 300);
    }, 3000);
}

// Инициализация при загрузке модуля
if (document.readyState === 'loading') {
    document.addEventListener('DOMContentLoaded', initGamesFilter);
} else {
    initGamesFilter();
} 