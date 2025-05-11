/**
 * Утилиты для приложения
 * @module utils
 */

/**
 * Загружает содержимое файла
 * @async
 * @param {string} path - Путь к файлу
 * @returns {Promise<string>} Содержимое файла
 */
export async function fetchContent(path) {
    try {
        const response = await fetch(path);
        if (!response.ok) {
            throw new Error(`Не удалось загрузить ${path}`);
        }
        return await response.text();
    } catch (error) {
        console.error(`Ошибка загрузки ${path}:`, error);
        throw error;
    }
}