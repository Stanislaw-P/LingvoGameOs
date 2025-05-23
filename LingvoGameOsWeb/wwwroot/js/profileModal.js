export function initializeProfileModal(defaultAvatarUrl = '/img/avatar100.png') {
    const modal = document.getElementById('edit-profile-modal');
    const openModalBtn = document.getElementById('open-edit-modal');
    const closeModalBtn = document.getElementById('close-edit-modal');
    const cancelBtn = document.getElementById('cancel-edit');
    const form = document.getElementById('edit-profile-form');
    const avatarInput = document.getElementById('modal-avatar');
    const avatarPreview = document.getElementById('modal-avatar-preview');
    const errorDiv = document.getElementById('modal-error');

    // Проверка наличия всех элементов
    if (!modal || !openModalBtn || !closeModalBtn || !cancelBtn || !form || !avatarInput || !avatarPreview || !errorDiv) {
        console.error('Один или несколько элементов модального окна не найдены:', {
            modal, openModalBtn, closeModalBtn, cancelBtn, form, avatarInput, avatarPreview, errorDiv
        });
        return;
    }

    // Инициализация модального окна в закрытом состоянии
    modal.style.display = 'none'; // Явно скрываем модальное окно
    modal.classList.remove('modal--open');

    // Открытие модального окна
    openModalBtn.addEventListener('click', () => {
        console.log('Кнопка "Изменить профиль" нажата');
        modal.style.display = 'flex'; // Показываем модальное окно
        modal.classList.add('modal--open');
        document.body.style.overflow = 'hidden';
        console.log('Модальное окно открыто');
    });

    // Закрытие модального окна
    const closeModal = () => {
        modal.style.display = 'none';
        modal.classList.remove('modal--open');
        document.body.style.overflow = ''; // Восстанавливаем прокрутку
        form.reset();
        errorDiv.style.display = 'none';
        avatarPreview.src = defaultAvatarUrl;
        console.log('Модальное окно закрыто');
    };

    closeModalBtn.addEventListener('click', closeModal);
    cancelBtn.addEventListener('click', closeModal);

    // Закрытие при клике вне модального окна
    modal.addEventListener('click', (e) => {
        if (e.target === modal) {
            closeModal();
        }
    });

    // Предварительный просмотр аватара
    avatarInput.addEventListener('change', (e) => {
        const file = e.target.files[0];
        if (file && file.type.startsWith('image/')) {
            const reader = new FileReader();
            reader.onload = (event) => {
                avatarPreview.src = event.target.result;
                console.log('Аватар обновлен в предварительном просмотре');
            };
            reader.readAsDataURL(file);
        } else {
            console.warn('Выбранный файл не является изображением');
        }
    });

    // Обработка отправки формы
    form.addEventListener('submit', async (e) => {
        e.preventDefault();
        const saveBtn = document.getElementById('save-profile');
        saveBtn.setAttribute('aria-busy', 'true');
        errorDiv.style.display = 'none';

        const formData = new FormData(form);
        try {
            // Симуляция API-запроса (замените на реальный endpoint)
            await new Promise(resolve => setTimeout(resolve, 1000));
            saveBtn.removeAttribute('aria-busy');
            saveBtn.setAttribute('data-state', 'success');
            saveBtn.textContent = 'Сохранено';
            console.log('Профиль успешно сохранен');
            setTimeout(closeModal, 1000);
        } catch (error) {
            console.error('Ошибка при сохранении профиля:', error);
            saveBtn.removeAttribute('aria-busy');
            saveBtn.setAttribute('data-state', 'error');
            errorDiv.textContent = 'Ошибка при сохранении профиля. Попробуйте снова.';
            errorDiv.style.display = 'block';
            setTimeout(() => {
                saveBtn.removeAttribute('data-state');
                saveBtn.textContent = 'Сохранить';
            }, 2000);
        }
    });
}