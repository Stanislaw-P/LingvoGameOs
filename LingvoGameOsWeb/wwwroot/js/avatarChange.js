document.addEventListener('DOMContentLoaded', function () {
    const avatarWrapper = document.querySelector('.profile__avatar-wrapper__settings');
    const uploadButton = document.querySelector('.profile__avatar-upload__settings');
    const fileInput = document.querySelector('input[type="file"]');
    const avatarImage = document.querySelector('.profile__image__settings');

    // Открываем выбор файла при клике на кнопку "Сменить"
    uploadButton.addEventListener('click', function (e) {
        e.stopPropagation();
        fileInput.click();
    });

    // Показываем превью при выборе файла
    fileInput.addEventListener('change', function () {
        if (this.files && this.files[0]) {
            const reader = new FileReader();

            reader.onload = function (e) {
                avatarImage.src = e.target.result;
                avatarWrapper.classList.add('preview-active__settings');

                // Показываем уведомление об успехе (опционально)
                const success = document.createElement('div');
                success.className = 'profile__avatar-success__settings';
                success.innerHTML = '✓';
                avatarWrapper.appendChild(success);

                // Убираем уведомление через 2 секунды
                setTimeout(() => {
                    success.remove();
                }, 2000);
            }

            reader.readAsDataURL(this.files[0]);
        }
    });

    // Также открываем выбор файла при клике на саму аватарку
    avatarWrapper.addEventListener('click', function (e) {
        if (e.target === avatarWrapper || e.target === avatarImage) {
            fileInput.click();
        }
    });
});