/**
 * Модуль для загрузки компонентов (header, footer)
 * @module components
 */
import { fetchContent } from './utils.js';

/**
 * Загружает компоненты в указанные placeholder
 * @async
 * @param {Object} components - Объект с конфигурацией компонентов
 * @param {string} components.placeholder - Селектор placeholder
 * @param {string} components.path - Путь к файлу компонента
 */
export async function loadComponents(components) {
    try {
        for (const [key, { placeholder, path }] of Object.entries(components)) {
            const element = document.querySelector(placeholder);
            if (!element) {
                console.warn(`Placeholder ${placeholder} не найден`);
                continue;
            }
            const content = await fetchContent(path);
            element.innerHTML = content;
        }
    } catch (error) {
        console.error('Ошибка загрузки компонентов:', error);
        throw error;
    }
}