// Инициализация таблицы лидеров с улучшенной разметкой
export function initializeLeaderboard() {
    const leaderboardList = document.querySelector('.leaderboard__list');
    if (!leaderboardList) return;

    const sampleLeaderboard = [
        { rank: 1, name: "Алан Д.", score: 2500, avatar: "img/avatar1.png" },
        { rank: 2, name: "Мария К.", score: 2300, avatar: "img/avatar2.png" },
        { rank: 3, name: "Тимур Б.", score: 2100, avatar: "img/avatar1.png" },
        { rank: 4, name: "Елена С.", score: 1900, avatar: "img/avatar2.png" },
        { rank: 5, name: "Заур Т.", score: 1800, avatar: "img/avatar1.png" },
        { rank: 6, name: "Игорь П.", score: 1700, avatar: "img/avatar2.png" },
        { rank: 7, name: "Ольга М.", score: 1600, avatar: "img/avatar1.png" },
        { rank: 8, name: "Дмитрий К.", score: 1500, avatar: "img/avatar2.png" },
        { rank: 9, name: "Анна В.", score: 1400, avatar: "img/avatar2.png" },
        { rank: 10, name: "Сергей Н.", score: 1300, avatar: "img/avatar1.png" }
    ];

    leaderboardList.innerHTML = ''; // Очищаем список перед заполнением

    sampleLeaderboard.forEach(player => {
        const leaderboardItem = document.createElement('div');
        leaderboardItem.className = 'leaderboard__item';
        leaderboardItem.innerHTML = `
            <span class="leaderboard__rank">${player.rank}</span>
            <div class="leaderboard__name">
                <img src="${player.avatar}" alt="${player.name}" class="leaderboard__avatar">
                <span>${player.name}</span>
            </div>
            <span class="leaderboard__score">${player.score}</span>
        `;
        leaderboardList.appendChild(leaderboardItem);
    });
}