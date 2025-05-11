import { initializeDropdown } from './dropdown.js';
import { initializeFavoritesLink } from './favorites.js';

// Функция для загрузки HTML-компонентов
export function loadComponent(url, placeholderId) {
    fetch(url)
        .then(response => response.text())
        .then(data => {
            document.getElementById(placeholderId).innerHTML = data;
            if (placeholderId === 'header-placeholder') {
                initializeDropdown();
                initializeFavoritesLink();
            }
        })
        .catch(error => console.error('Ошибка при загрузке компонента:', error));
}