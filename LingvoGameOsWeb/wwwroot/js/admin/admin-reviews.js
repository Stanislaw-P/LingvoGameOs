// Функция для публикации отзыва
function publishReview(reviewId) {
    if (!confirm('Вы уверены, что хотите опубликовать этот отзыв?')) {
        return;
    }

    fetch('/Admin/Reviews/Publish', {
        method: 'POST',
        headers: {
            'Content-Type': 'application/json'
        },
        body: JSON.stringify(reviewId)
    })
        .then(response => {
            if (!response.ok) {
                throw new Error('Ошибка сети');
            }
            return response.json();
        })
        .then(data => {
            if (data.success) {
                // Находим элемент отзыва и обновляем его статус
                const reviewElement = document.querySelector(`.admin-review[data-review-id="${reviewId}"]`);
                if (reviewElement) {
                    reviewElement.classList.remove('pending');
                    reviewElement.classList.add('published');

                    // Обновляем статус
                    const statusElement = reviewElement.querySelector('.review-status');
                    statusElement.textContent = 'Опубликовано';
                    statusElement.classList.remove('status-pending');
                    statusElement.classList.add('status-published');

                    // Убираем кнопку "Опубликовать"
                    const publishButton = reviewElement.querySelector('.btn--success');
                    if (publishButton) {
                        publishButton.remove();
                    }

                }
            } else {
                alert('Ошибка при публикации отзыва: ' + data.message);
            }
        })
        .catch(error => {
            console.error('Ошибка:', error);
            alert('Произошла ошибка при публикации отзыва');
        });
}

// Функция для удаления отзыва
function deleteReview(reviewId) {
    if (!confirm('Вы уверены, что хотите удалить этот отзыв? Это действие нельзя отменить.')) {
        return;
    }

    fetch('/Admin/Reviews/Delete', {
        method: 'POST',
        headers: {
            'Content-Type': 'application/json'
        },
        body: JSON.stringify(reviewId)
    })
        .then(response => {
            if (!response.ok) {
                throw new Error('Ошибка сети');
            }
            return response.json();
        })
        .then(data => {
            if (data.success) {
                // Удаляем элемент отзыва из DOM
                const reviewElement = document.querySelector(`.admin-review[data-review-id="${reviewId}"]`);
                if (reviewElement) {
                    reviewElement.remove();
                }
            } else {
                alert('Ошибка при удалении отзыва: ' + data.message);
            }
        })
        .catch(error => {
            console.error('Ошибка:', error);
            alert('Произошла ошибка при удалении отзыва');
        });
}


// Инициализация обработчиков событий после загрузки DOM
document.addEventListener('DOMContentLoaded', function () {
    // Добавляем data-атрибут к каждому отзыву для идентификации
    document.querySelectorAll('.admin-review').forEach((reviewElement, index) => {
        // Предполагаем, что у вас есть способ получить ID отзыва
        // Например, через data-атрибут в разметке
        const reviewId = reviewElement.dataset.reviewId || index;
        reviewElement.setAttribute('data-review-id', reviewId);

        // Назначаем обработчики для кнопок
        const publishButton = reviewElement.querySelector('.btn--success');
        const deleteButton = reviewElement.querySelector('.btn--danger');

        if (publishButton) {
            publishButton.addEventListener('click', () => publishReview(reviewId));
        }

        if (deleteButton) {
            deleteButton.addEventListener('click', () => deleteReview(reviewId));
        }
    });
});