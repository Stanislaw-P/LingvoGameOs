// Генерация графика активности
export function initializeActivityChart() {
    const chartGrid = document.querySelector('.activity__chart-grid');
    if (!chartGrid) return;

    for (let i = 0; i < 365; i++) {
        const cell = document.createElement('div');
        cell.className = 'activity__chart-cell';
        if (i % 10 === 0) cell.classList.add('active');
        chartGrid.appendChild(cell);
    }
}