import { API_URL } from './config.js';
import { showError } from './dom.js';

/**
 * Initializes the upload form and modal functionality
 */
function initializeUploadForm() {
    // Form and dropdown elements
    const form = document.querySelector('#upload-game-form');
    const skillsLearningDropdown = document.querySelector('#skillsLearning-dropdown');
    const skillsLearningSelected = skillsLearningDropdown.querySelector('.custom-dropdown__selected');
    const skillsLearningMenu = skillsLearningDropdown.querySelector('.custom-dropdown__menu');
    const skillsLearningSearch = skillsLearningDropdown.querySelector('.custom-dropdown__search');
    const skillsLearningOptions = skillsLearningDropdown.querySelectorAll('.custom-dropdown__option');
    const selectedskillsLearningList = document.querySelector('#selected-skillsLearning-list');
    const skillsLearningInput = document.querySelector('#game-skillsLearning');
    const coverDropzone = document.querySelector('#cover-dropzone');
    const fileDropzone = document.querySelector('#file-dropzone');
    const coverFileInput = document.querySelector('#game-cover-file');
    const gameFileInput = document.querySelector('#game-file-file');
    const platformDropdown = document.querySelector('#platform-dropdown');
    const platformSelected = platformDropdown.querySelector('.custom-dropdown__selected');
    const platformMenu = platformDropdown.querySelector('.custom-dropdown__menu');
    const platformSearch = platformDropdown.querySelector('.custom-dropdown__search') || null;
    const platformOptions = platformDropdown.querySelectorAll('.custom-dropdown__option');
    const selectedPlatformsList = document.querySelector('#selected-platforms-list');
    const platformInput = document.querySelector('#game-platform');
    const fileUrlGroup = document.querySelector('#game-file-url-group');
    const fileUploadGroup = document.querySelector('#game-file-upload-group');
    let lastFocusedElement = null;
    let selectedSkillsLearning = [];
    // Инициализируем из скрытого поля
    const initialSkills = document.getElementById('game-skillsLearning').value;
    if (initialSkills) {
        selectedSkillsLearning = initialSkills.split(',').filter(item => item.trim() !== '');
    }
    updatePlaceholderText(skillsLearningSelected.querySelector('.custom-dropdown__placeholder'), selectedSkillsLearning.length);
    let selectedPlatforms = [];

    // Allowed file types and sizes
    const ALLOWED_GAME_EXTENSIONS = ['.msi'];
    const MAX_GAME_SIZE = 2 * 1024 * 1024 * 1024; // 2GB
    const ALLOWED_COVER_TYPES = ['image/jpeg', 'image/png', 'image/webp'];
    const MAX_COVER_SIZE = 30 * 1024 * 1024; // 30MB

    // Toggle dropdown (generic for both skillsLearning and platforms)
    function toggleDropdown(selected, menu) {
        if (!selected || !menu) return;
        const isActive = selected.classList.contains('active');
        selected.classList.toggle('active', !isActive);
        menu.classList.toggle('active', !isActive);
        selected.setAttribute('aria-expanded', !isActive);
        menu.setAttribute('aria-hidden', isActive);
        if (!isActive && platformSearch) {
            platformSearch.focus();
        }
    }

    // Update selected items (generic for skillsLearning and platforms)
    function updateSelectedItems(list, input, selectedItems, options, placeholderElement) {
        list.innerHTML = '';
        input.value = selectedItems.join(',');
        updatePlaceholderText(placeholderElement, selectedItems.length);
        selectedItems.forEach(value => {
            const option = Array.from(options).find(opt => opt.dataset.value === value);
            if (option) {
                const item = document.createElement('span');
                item.className = 'selected-item';
                item.dataset.value = value;
                item.innerHTML = `
                ${option.textContent.trim()}
                <span class="selected-item__remove" role="button" aria-label="Удалить ${option.textContent.trim()}">×</span>
            `;
                list.appendChild(item);
            }
        });

        options.forEach(option => {
            option.classList.toggle('selected', selectedItems.includes(option.dataset.value));
        });

        // Update visibility of file input fields for platforms
        if (input.id === 'game-platform') {
            fileUploadGroup.style.display = 'none';

            // Показываем загрузку файла ТОЛЬКО для Desktop
            if (selectedItems.includes('Desktop')) {
                fileUploadGroup.style.display = 'block';
            }
        }
    }

    // Add item (generic for skillsLearning and platforms)
    function addItem(value, text, selectedItems, options, list, input, placeholderElement, isPlatform = false, selected = null, menu = null) {
        if (value === 'select-all' && !isPlatform) {
            selectedItems.length = 0;
            Array.from(options)
                .filter(opt => opt.dataset.value !== 'select-all')
                .forEach(opt => selectedItems.push(opt.dataset.value));
            if (selected && menu) {
                toggleDropdown(selected, menu);
            }
        } else {
            if (isPlatform) {
                selectedItems.length = 0;
                selectedItems.push(value);
            } else if (!selectedItems.includes(value)) {
                selectedItems.push(value);
            }
        }
        updateSelectedItems(list, input, selectedItems, options, placeholderElement);
        if (isPlatform && selected && menu) {
            toggleDropdown(selected, menu);
        }
    }

    // Remove item (generic for skillsLearning and platforms)
    function removeItem(value, selectedItems, options, list, input, placeholderElement) {
        const index = selectedItems.indexOf(value);
        if (index !== -1) {
            selectedItems.splice(index, 1);
            updateSelectedItems(list, input, selectedItems, options, placeholderElement);
        }
    }

    // Filter items (generic for skillsLearning and platforms)
    function filterItems(searchInput, options) {
        if (!searchInput) return;
        const query = searchInput.value.toLowerCase();
        options.forEach(option => {
            const text = option.textContent.toLowerCase();
            option.style.display = text.includes(query) || option.dataset.value === 'select-all' ? '' : 'none';
        });
    }

    // SkillsLearning event listeners
    skillsLearningSelected.addEventListener('click', () => toggleDropdown(skillsLearningSelected, skillsLearningMenu));
    skillsLearningSelected.addEventListener('keydown', (e) => {
        if (e.key === 'Enter' || e.key === ' ') {
            e.preventDefault();
            toggleDropdown(skillsLearningSelected, skillsLearningMenu);
        }
    });

    skillsLearningSearch.addEventListener('input', () => filterItems(skillsLearningSearch, skillsLearningOptions));
    skillsLearningSearch.addEventListener('keydown', (e) => {
        if (e.key === 'Escape') {
            skillsLearningSelected.classList.remove('active');
            skillsLearningMenu.classList.remove('active');
            skillsLearningSelected.setAttribute('aria-expanded', 'false');
            skillsLearningMenu.setAttribute('aria-hidden', 'true');
            skillsLearningSelected.focus();
        }
    });

    skillsLearningOptions.forEach(option => {
        option.addEventListener('click', () => {
            const value = option.dataset.value;
            const text = option.textContent.trim();
            addItem(value, text, selectedSkillsLearning, skillsLearningOptions, selectedskillsLearningList, skillsLearningInput, skillsLearningSelected.querySelector('.custom-dropdown__placeholder'), false, skillsLearningSelected, skillsLearningMenu);
        });
        option.addEventListener('keydown', (e) => {
            if (e.key === 'Enter' || e.key === ' ') {
                e.preventDefault();
                const value = option.dataset.value;
                const text = option.textContent.trim();
                addItem(value, text, selectedSkillsLearning, skillsLearningOptions, selectedskillsLearningList, skillsLearningInput, skillsLearningSelected.querySelector('.custom-dropdown__placeholder'), false, skillsLearningSelected, skillsLearningMenu);
                toggleDropdown(skillsLearningSelected, skillsLearningMenu);
            } else if (e.key === 'ArrowDown') {
                e.preventDefault();
                const items = Array.from(skillsLearningOptions);
                const index = items.indexOf(option);
                const next = index < items.length - 1 ? items[index + 1] : items[0];
                next.focus();
            } else if (e.key === 'ArrowUp') {
                e.preventDefault();
                const items = Array.from(skillsLearningOptions);
                const index = items.indexOf(option);
                const prev = index > 0 ? items[index - 1] : items[items.length - 1];
                prev.focus();
            }
        });
    });

    selectedskillsLearningList.addEventListener('click', (e) => {
        const removeButton = e.target.closest('.selected-item__remove');
        if (removeButton) {
            const item = removeButton.parentElement;
            const value = item.dataset.value;
            removeItem(value, selectedSkillsLearning, skillsLearningOptions, selectedskillsLearningList, skillsLearningInput, skillsLearningSelected.querySelector('.custom-dropdown__placeholder'));
        }
    });

    // Platforms event listeners
    platformSelected.addEventListener('click', () => toggleDropdown(platformSelected, platformMenu));
    platformSelected.addEventListener('keydown', (e) => {
        if (e.key === 'Enter' || e.key === ' ') {
            e.preventDefault();
            toggleDropdown(platformSelected, platformMenu);
        }
    });

    if (platformSearch) {
        platformSearch.addEventListener('input', () => filterItems(platformSearch, platformOptions));
        platformSearch.addEventListener('keydown', (e) => {
            if (e.key === 'Escape') {
                platformSelected.classList.remove('active');
                platformMenu.classList.remove('active');
                platformSelected.setAttribute('aria-expanded', 'false');
                platformMenu.setAttribute('aria-hidden', 'true');
                platformSelected.focus();
            }
        });
    }

    platformOptions.forEach(option => {
        option.addEventListener('click', () => {
            const value = option.dataset.value;
            const text = option.textContent.trim();
            addItem(value, text, selectedPlatforms, platformOptions, selectedPlatformsList, platformInput, platformSelected.querySelector('.custom-dropdown__placeholder'), true);
        });
        option.addEventListener('keydown', (e) => {
            if (e.key === 'Enter' || e.key === ' ') {
                e.preventDefault();
                const value = option.dataset.value;
                const text = option.textContent.trim();
                addItem(value, text, selectedPlatforms, platformOptions, selectedPlatformsList, platformInput, platformSelected.querySelector('.custom-dropdown__placeholder'), true);
            } else if (e.key === 'ArrowDown') {
                e.preventDefault();
                const items = Array.from(platformOptions);
                const index = items.indexOf(option);
                const next = index < items.length - 1 ? items[index + 1] : items[0];
                next.focus();
            } else if (e.key === 'ArrowUp') {
                e.preventDefault();
                const items = Array.from(platformOptions);
                const index = items.indexOf(option);
                const prev = index > 0 ? items[index - 1] : items[items.length - 1];
                prev.focus();
            }
        });
    });

    selectedPlatformsList.addEventListener('click', (e) => {
        const removeButton = e.target.closest('.selected-item__remove');
        if (removeButton) {
            const item = removeButton.parentElement;
            const value = item.dataset.value;
            removeItem(value, selectedPlatforms, platformOptions, selectedPlatformsList, platformInput, platformSelected.querySelector('.custom-dropdown__placeholder'));
        }
    });

    document.addEventListener('click', (e) => {
        if (!skillsLearningDropdown.contains(e.target)) {
            skillsLearningSelected.classList.remove('active');
            skillsLearningMenu.classList.remove('active');
            skillsLearningSelected.setAttribute('aria-expanded', 'false');
            skillsLearningMenu.setAttribute('aria-hidden', 'true');
        }
        if (!platformDropdown.contains(e.target)) {
            platformSelected.classList.remove('active');
            platformMenu.classList.remove('active');
            platformSelected.setAttribute('aria-expanded', 'false');
            platformMenu.setAttribute('aria-hidden', 'true');
        }
    });

    document.addEventListener('keydown', (e) => {
        if (e.key === 'Escape') {
            skillsLearningSelected.classList.remove('active');
            skillsLearningMenu.classList.remove('active');
            skillsLearningSelected.setAttribute('aria-expanded', 'false');
            skillsLearningMenu.setAttribute('aria-hidden', 'true');
            platformSelected.classList.remove('active');
            platformMenu.classList.remove('active');
            platformSelected.setAttribute('aria-expanded', 'false');
            platformMenu.setAttribute('aria-hidden', 'true');
        }
    });

    // Handle file uploads
    setupFileUpload(
        coverDropzone,
        coverFileInput,
        '#cover-preview',
        MAX_COVER_SIZE,
        ALLOWED_COVER_TYPES,
        ['.jpg', '.jpeg', '.png', '.webp']
    );

    setupFileUpload(
        fileDropzone,
        gameFileInput,
        '#file-preview',
        MAX_GAME_SIZE,
        null, // No MIME type check for .msi
        ALLOWED_GAME_EXTENSIONS
    );

    function resetForm() {
        form.reset();
        selectedSkillsLearning = [];
        selectedPlatforms = [];
        updateSelectedItems(selectedskillsLearningList, skillsLearningInput, selectedSkillsLearning, skillsLearningOptions, skillsLearningSelected.querySelector('.custom-dropdown__placeholder'));
        updateSelectedItems(selectedPlatformsList, platformInput, selectedPlatforms, platformOptions, platformSelected.querySelector('.custom-dropdown__placeholder'));
        document.querySelector('#cover-preview').innerHTML = '';
        document.querySelector('#file-preview').innerHTML = '';
        fileUploadGroup.style.display = 'none';
    }

    // Form submission
    form.addEventListener('submit', async (e) => {
        e.preventDefault();
        const submitButton = form.querySelector('.upload-form__submit');
        submitButton.setAttribute('aria-busy', 'true');
        clearErrors();
        clearValidationSummary();

        const formData = new FormData(form);
        const platforms = formData.get('GamePlatform').split(',').filter(p => p);
        const actionUrl = form.getAttribute('data-action');

        formData.set('skillsLearning', JSON.stringify(selectedSkillsLearning));
        formData.set('platform', JSON.stringify(platforms));

        try {
            const response = await fetch(actionUrl, {
                method: 'POST',
                body: formData,
                headers: {
                    'X-Requested-With': 'XMLHttpRequest'
                }
            });

            if (!response.ok) {
                const result = await response.json();
                if (response.status === 422 && result.errors && result.errors.length > 0) {
                    showValidationSummary(result.errors);
                    throw new Error(result.message || 'Ошибки валидации');
                }
                throw new Error(result.message || 'Ошибка загрузки игры');
            }

            const successData = await response.json(); // Парсим успешный ответ
            if (successData.success && successData.redirectUrl) {
                window.location.href = successData.redirectUrl; // Перенаправляем
                return;
            }

            showNotification('Игра успешно загружена!', 'success');
            resetForm();
        } catch (error) {
            // Показываем уведомление только если это не ошибки валидации (они уже показаны в summary)
            if (!error.message.includes('Ошибки валидации')) {
                showNotification(error.message || 'Не удалось загрузить игру. Попробуйте снова.', 'error');
            }

            console.error('Upload error:', error);
        } finally {
            submitButton.setAttribute('aria-busy', 'false');
        }
    });



    // Initialize dropdowns
    updateSelectedItems(selectedskillsLearningList, skillsLearningInput, selectedSkillsLearning, skillsLearningOptions, skillsLearningSelected.querySelector('.custom-dropdown__placeholder'));
    updateSelectedItems(selectedPlatformsList, platformInput, selectedPlatforms, platformOptions, platformSelected.querySelector('.custom-dropdown__placeholder'));
}



function updatePlaceholderText(placeholderElement, count) {
    placeholderElement.textContent = count ? `${count} выбрано` : 'Выберите развиваемые навыки';
}

/**
 * Adds a tag to the specified list
 * @param {HTMLElement} list - The list to add the tag to
 * @param {string} value - The tag value
 * @param {string} type - The type of tag (skillsLearning/keyword)
 */
function addTag(list, value, type) {
    const tag = document.createElement('span');
    tag.className = 'skillsLearning-item';
    tag.dataset.value = value;
    tag.innerHTML = `
        ${value}
        <span class="skillsLearning-item__remove" role="button" aria-label="Удалить ${value}">×</span>
    `;
    list.appendChild(tag);
}


/**
 * Sets up drag-and-drop file upload with improved validation
 * @param {HTMLElement} dropzone - The dropzone element
 * @param {HTMLInputElement} fileInput - The file input element
 * @param {string} previewSelector - The selector for the preview container
 * @param {number} maxSize - Maximum file size in bytes
 * @param {string[]} allowedTypes - Allowed MIME types
 * @param {string[]} allowedExtensions - Allowed file extensions
 */
function setupFileUpload(dropzone, fileInput, previewSelector, maxSize, allowedTypes, allowedExtensions) {
    const preview = document.querySelector(previewSelector);

    dropzone.addEventListener('click', () => fileInput.click());
    dropzone.addEventListener('dragover', (e) => {
        e.preventDefault();
        dropzone.classList.add('file-upload--active');
    });
    dropzone.addEventListener('dragleave', () => {
        dropzone.classList.remove('file-upload--active');
    });
    dropzone.addEventListener('drop', (e) => {
        e.preventDefault();
        dropzone.classList.remove('file-upload--active');
        handleFiles(e.dataTransfer.files, fileInput, preview, maxSize, allowedTypes, allowedExtensions);
    });

    fileInput.addEventListener('change', () => {
        handleFiles(fileInput.files, fileInput, preview, maxSize, allowedTypes, allowedExtensions);
    });
}

/**
 * Handles file uploads and validation with improved checks
 * @param {FileList} files - The uploaded files
 * @param {HTMLInputElement} fileInput - The file input element
 * @param {HTMLElement} preview - The preview container
 * @param {number} maxSize - Maximum file size in bytes
 * @param {string[]} allowedTypes - Allowed MIME types
 * @param {string[]} allowedExtensions - Allowed file extensions
 */
function handleFiles(files, fileInput, preview, maxSize, allowedTypes, allowedExtensions) {
    const file = files[0];
    if (!file) return;

    const fileExtension = file.name.slice(file.name.lastIndexOf('.')).toLowerCase();

    // Check file extension
    if (allowedExtensions && !allowedExtensions.includes(fileExtension)) {
        showNotification(`Недопустимое расширение файла. Разрешено: ${allowedExtensions.join(', ')}`, 'error');
        fileInput.value = '';
        return;
    }

    // Check MIME type only for cover images
    if (allowedTypes && !allowedTypes.includes(file.type)) {
        showNotification(`Недопустимый тип файла. Разрешено: ${allowedTypes.join(', ')}`, 'error');
        fileInput.value = '';
        return;
    }

    // Check file size
    if (file.size > maxSize) {
        const maxSizeMB = maxSize / (1024 * 1024);
        const fileSizeMB = (file.size / (1024 * 1024)).toFixed(2);
        showNotification(`Файл слишком большой (${fileSizeMB}MB). Максимальный размер: ${maxSizeMB}MB`, 'error');
        fileInput.value = '';
        return;
    }

    // Display file preview
    preview.innerHTML = '';
    const fileItem = document.createElement('div');
    fileItem.className = 'file-upload__file';
    fileItem.innerHTML = `
        <span class="file-upload__file-name">${file.name}</span>
        <span class="file-upload__file-size">${formatFileSize(file.size)}</span>
        <img src="/icon/trash.svg" alt="Удалить файл" class="file-upload__file-trash" role="button" aria-label="Удалить ${file.name}" />
    `;
    preview.appendChild(fileItem);

    // Special handling for image previews
    if (fileInput.id === 'game-cover-file' && file.type.startsWith('image/')) {
        const reader = new FileReader();
        reader.onload = (e) => {
            const img = document.createElement('img');
            img.src = e.target.result;
            img.className = 'file-upload__preview-image';
            img.alt = 'Превью обложки игры';
            preview.appendChild(img);
        };
        reader.readAsDataURL(file);
    }

    // Handle file removal - ИСПРАВЛЕННАЯ ЧАСТЬ
    fileItem.querySelector('.file-upload__file-trash').addEventListener('click', () => {
        // Полностью очищаем preview контейнер
        preview.innerHTML = '';
        // Сбрасываем значение file input
        fileInput.value = '';
    });
}

/**
 * Formats file size in human-readable format
 * @param {number} bytes - File size in bytes
 * @returns {string} Formatted file size
 */
function formatFileSize(bytes) {
    if (bytes < 1024) return `${bytes} B`;
    if (bytes < 1024 * 1024) return `${(bytes / 1024).toFixed(2)} KB`;
    if (bytes < 1024 * 1024 * 1024) return `${(bytes / (1024 * 1024)).toFixed(2)} MB`;
    return `${(bytes / (1024 * 1024 * 1024)).toFixed(2)} GB`;
}

/**
 * Shows a notification message
 * @param {string} message - The message to display
 * @param {string} type - The type of notification (success/error)
 */
function showNotification(message, type) {
    const notification = document.querySelector('#notification');
    notification.textContent = message;
    notification.className = `notification notification--${type}`;
    notification.classList.add('notification--visible');
    setTimeout(() => {
        notification.classList.remove('notification--visible');
    }, 5000);
}



function showValidationSummary(errors) {
    const validationSummary = document.getElementById('validation-summary');
    const errorsList = document.getElementById('validation-errors-list');

    if (!validationSummary) return;

    if (errorsList) {
        // Для спискового отображения
        errorsList.innerHTML = '';
        errors.forEach(error => {
            const li = document.createElement('li');
            li.textContent = error;
            li.className = 'validation-summary__error';
            errorsList.appendChild(li);
        });
    } else {
        // Для простого отображения
        validationSummary.innerHTML = errors.map(error =>
            `<div class="error-message">${error}</div>`
        ).join('');
    }

    validationSummary.style.display = 'block';

    // Прокручиваем к ошибкам
    validationSummary.scrollIntoView({
        behavior: 'smooth',
        block: 'nearest'
    });
}

function clearValidationSummary() {
    const validationSummary = document.getElementById('validation-summary');
    if (validationSummary) {
        validationSummary.style.display = 'none';
        const errorsList = document.getElementById('validation-errors-list');
        if (errorsList) {
            errorsList.innerHTML = '';
        } else {
            validationSummary.innerHTML = '';
        }
    }
}

/**
 * Clears all form errors
 */
function clearErrors() {
    // Очищаем field-specific errors
    document.querySelectorAll('.error-message').forEach((el) => {
        el.textContent = '';
        el.classList.remove('error-message--visible');
    });

    // Очищаем validation summary
    clearValidationSummary();
}

/**
 * Initializes multiple file upload functionality for screenshots
 */
function initializeScreenshotsUpload() {
    const MAX_IMAGES = 3;
    const fileInput = document.getElementById('game-screenshots-file');
    const previewContainer = document.getElementById('screenshots-preview');
    const errorElement = document.getElementById('screenshots-error');

    // Если элементы не найдены на странице, выходим
    if (!fileInput || !previewContainer) {
        return;
    }

    let currentFiles = [];

    fileInput.addEventListener('change', function (e) {
        errorElement.textContent = '';
        const newFiles = Array.from(e.target.files);

        if (currentFiles.length + newFiles.length > MAX_IMAGES) {
            errorElement.textContent = `Максимум можно загрузить ${MAX_IMAGES} изображения`;
            return;
        }

        newFiles.forEach(file => {
            if (!file.type.match('image.*')) {
                errorElement.textContent = 'Пожалуйста, загружайте только изображения';
                return;
            }

            // Проверяем на дубликаты
            if (!currentFiles.some(f => f.name === file.name && f.size === file.size)) {
                currentFiles.push(file);
            }
        });

        renderPreviews();
        updateFileInput();
    });

    function renderPreviews() {
        previewContainer.innerHTML = '';

        currentFiles.forEach((file, index) => {
            const reader = new FileReader();
            reader.onload = function (event) {
                const previewElement = document.createElement('div');
                previewElement.className = 'preview-item';
                previewElement.innerHTML = `
                    <div class="file-upload__file">
                        <span class="file-upload__file-name">${file.name}</span>
                        <span class="file-upload__file-size">${formatFileSize(file.size)}</span>
                        <img src="/icon/trash.svg" class="file-upload__file-trash"
                             alt="Удалить" title="Удалить">
                    </div>
                    <img src="${event.target.result}" class="file-upload__preview-image">
                `;

                previewElement.querySelector('.file-upload__file-trash').addEventListener('click', () => {
                    currentFiles.splice(index, 1);
                    renderPreviews();
                    updateFileInput();
                });

                previewContainer.appendChild(previewElement);
            };
            reader.readAsDataURL(file);
        });
    }

    function updateFileInput() {
        const dataTransfer = new DataTransfer();
        currentFiles.forEach(file => dataTransfer.items.add(file));
        fileInput.files = dataTransfer.files;
    }
}

// Initialize on DOM content loaded
document.addEventListener('DOMContentLoaded', function () {
    initializeUploadForm();
    initializeScreenshotsUpload();
});