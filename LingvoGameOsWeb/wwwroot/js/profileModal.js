export function initializeProfileModal(defaultAvatarUrl = '/img/avatar100.png') {
    const modal = document.getElementById('edit-profile-modal');
    const openModalBtn = document.getElementById('open-edit-modal');
    const closeModalBtn = document.getElementById('close-edit-modal');
    const cancelBtn = document.getElementById('cancel-edit');
    const form = document.getElementById('edit-profile-form');
    const avatarInput = document.getElementById('modal-avatar');
    const avatarPreview = document.getElementById('modal-avatar-preview');
    const errorDiv = document.getElementById('modal-error');

    if (!modal || !openModalBtn || !closeModalBtn || !cancelBtn || !form || !avatarInput || !avatarPreview || !errorDiv) {
        console.warn('Один или несколько элементов модального окна не найдены');
        return;
    }

    // Открытие модального окна
    openModalBtn.addEventListener('click', () => {
        modal.classList.add('modal--open');
        document.body.style.overflow = 'hidden'; // Блокировка прокрутки фона
    });

    // Закрытие модального окна
    const closeModal = () => {
        modal.classList.remove('modal--open');
        document.body.style.overflow = ''; // Восстановление прокрутки
        form.reset(); // Сброс формы
        errorDiv.style.display = 'none';
        avatarPreview.src = defaultAvatarUrl; // Сброс аватара
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
            };
            reader.readAsDataURL(file);
        }
    });

    // Обработка отправки формы
    form.addEventListener('submit', async (e) => {
        e.preventDefault();
        const saveBtn = document.getElementById('save-profile');
        saveBtn.setAttribute('aria-busy', 'true');

        const formData = new FormData(form);
        try {
            // Симуляция API-запроса (замените на реальный endpoint)
            // const response = await fetch('/api/update-profile', {
            //     method: 'POST',
            //     body: formData
            // });
            // if (!response.ok) throw new Error('Не удалось сохранить профиль');

            // Симуляция успешного сохранения
            setTimeout(() => {
                saveBtn.removeAttribute('aria-busy');
                saveBtn.setAttribute('data-state', 'success');
                saveBtn.textContent = 'Сохранено';
                setTimeout(closeModal, 1000);
            }, 1000);
        } catch (error) {
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
