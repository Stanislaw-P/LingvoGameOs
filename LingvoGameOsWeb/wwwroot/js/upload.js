import { API_URL } from './config.js';
import { showError } from './dom.js';

/**
 * Initializes the upload form and modal functionality
 */
function initializeUploadForm() {
    // Form and dropdown elements
    const form = document.querySelector('#upload-game-form');
    const skillsLearningDropdown = document.querySelector('#skillsLearning-dropdown'); //m
    const skillsLearningSelected = skillsLearningDropdown.querySelector('.custom-dropdown__selected');
    const skillsLearningMenu = skillsLearningDropdown.querySelector('.custom-dropdown__menu');
    const skillsLearningSearch = skillsLearningDropdown.querySelector('.custom-dropdown__search');
    const skillsLearningOptions = skillsLearningDropdown.querySelectorAll('.custom-dropdown__option');
    const selectedskillsLearningList = document.querySelector('#selected-skillsLearning-list'); //m
    const skillsLearningInput = document.querySelector('#game-skillsLearning');//m
    const skillsLearningError = document.querySelector('#skillsLearning-error');//m
    const keywordsInput = document.querySelector('#game-keywords');
    const keywordsList = document.querySelector('#keywords-list');
    const coverDropzone = document.querySelector('#cover-dropzone');
    const fileDropzone = document.querySelector('#file-dropzone');
    const coverFileInput = document.querySelector('#game-cover-file');
    const gameFileInput = document.querySelector('#game-file-file');
    const notification = document.querySelector('#notification');
    const modalOverlay = document.querySelector('#qualityModalOverlay');
    const modalCloseButton = document.querySelector('#qualityModalClose');
    const openModalLink = document.querySelector('#open-quality-modal');
    const platformDropdown = document.querySelector('#platform-dropdown');
    const platformSelected = platformDropdown.querySelector('.custom-dropdown__selected');
    const platformMenu = platformDropdown.querySelector('.custom-dropdown__menu');
    const platformSearch = platformDropdown.querySelector('.custom-dropdown__search') || null;
    const platformOptions = platformDropdown.querySelectorAll('.custom-dropdown__option');
    const selectedPlatformsList = document.querySelector('#selected-platforms-list');
    const platformInput = document.querySelector('#game-platform');
    const platformError = document.querySelector('#platform-error');
    const fileUrlGroup = document.querySelector('#game-file-url-group');
    const fileUploadGroup = document.querySelector('#game-file-upload-group');
    let lastFocusedElement = null;
    let selectedSkillsLearning = [];
    let selectedPlatforms = [];

    // Allowed file types and sizes
    const ALLOWED_GAME_TYPES = [
        'application/x-msi',
        'image/jpeg'
    ];
    const ALLOWED_GAME_EXTENSIONS = ['.msi', '.jpg'];
    const MAX_GAME_SIZE = 2 * 1024 * 1024 * 1024; // 2GB
    const ALLOWED_COVER_TYPES = ['image/jpeg', 'image/png', 'image/webp'];
    const MAX_COVER_SIZE = 30 * 1024 * 1024; // 30MB

    // Toggle dropdown (generic for both skillsLearning and platforms)   
    function toggleDropdown(selected, menu) {
        if (!selected || !menu) return; // Prevent null errors
        const isActive = selected.classList.contains('active');
        selected.classList.toggle('active', !isActive);
        menu.classList.toggle('active', !isActive);
        selected.setAttribute('aria-expanded', !isActive);
        menu.setAttribute('aria-hidden', isActive);
        if (!isActive && platformSearch) {
            platformSearch.focus();
        }
    }

    // Update selected items (generic for skillsLearning and platforms)  //m
    function updateSelectedItems(list, input, selectedItems, options, placeholderElement) {
        list.innerHTML = '';
        input.value = selectedItems.join(',');
        placeholderElement.textContent = selectedItems.length ? `${selectedItems.length} выбрано` : `Выберите ${input.id.includes('platform') ? 'платформу' : 'навыки'}`;

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
            fileUrlGroup.style.display = 'none';
            fileUploadGroup.style.display = 'none';
            if (selectedItems.includes('Web-Mobile') || selectedItems.includes('Web-Desktop')) {
                fileUrlGroup.style.display = 'block';
            }
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
                toggleDropdown(selected, menu); // Close dropdown for skillsLearning on "select-all" 
            } 
        } else {
            if (isPlatform) {
                selectedItems.length = 0; // Clear previous platform selection
                selectedItems.push(value);
            } else if (!selectedItems.includes(value)) {
                selectedItems.push(value);
            }
        }
        updateSelectedItems(list, input, selectedItems, options, placeholderElement);
        if (isPlatform && selected && menu) {
            toggleDropdown(selected, menu); // Close platform dropdown
        }
    }

    // Remove item (generic for skillsLearning and platforms) 
    function removeItem(value, selectedItems, options, list, input, placeholderElement) {
        selectedItems.splice(selectedItems.indexOf(value), 1);
        updateSelectedItems(list, input, selectedItems, options, placeholderElement);
    }

    // Filter items (generic for skillsLearning and platforms)  
    function filterItems(searchInput, options) {
        if (!searchInput) return; // Skip if no search input
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
                toggleDropdown(skillsLearningSelected, skillsLearningMenu); // Close dropdown on Enter
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
            addItem(value, text, selectedPlatforms, platformOptions, selectedPlatformsList, platformInput, platformSelected.querySelector('.custom-dropdown__placeholder'), true, platformSelected, platformMenu);
        });
        option.addEventListener('keydown', (e) => {
            if (e.key === 'Enter' || e.key === ' ') {
                e.preventDefault();
                const value = option.dataset.value;
                const text = option.textContent.trim();
                addItem(value, text, selectedPlatforms, platformOptions, selectedPlatformsList, platformInput, platformSelected.querySelector('.custom-dropdown__placeholder'), true, platformSelected, platformMenu);
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

    // Handle keywords input
    keywordsInput.addEventListener('keydown', (e) => {
        if (e.key === 'Enter' && e.target.value.trim()) {
            e.preventDefault();
            addTag(keywordsList, e.target.value.trim(), 'keyword');
            e.target.value = '';
        }
    });

    // Handle tag removal from keywordsList
    keywordsList.addEventListener('click', (e) => {
        if (e.target.classList.contains('skillsLearning-item__remove')) {
            e.target.parentElement.remove();
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
        ALLOWED_GAME_TYPES,
        ALLOWED_GAME_EXTENSIONS
    );

    // Modal functionality
    function openModal() {
        lastFocusedElement = document.activeElement;
        modalOverlay.classList.add('quality-modal__overlay--visible');
        modalCloseButton.focus();
        trapFocus(modalOverlay);
        document.body.style.overflow = 'hidden';
    }

    function closeModal() {
        modalOverlay.classList.remove('quality-modal__overlay--visible');
        if (lastFocusedElement) lastFocusedElement.focus();
        document.body.style.overflow = '';
    }

    function trapFocus(modal) {
        const focusableElements = modal.querySelectorAll('button, [href], input, select, textarea, [tabindex]:not([tabindex="-1"])');
        const firstElement = focusableElements[0];
        const lastElement = focusableElements[focusableElements.length - 1];

        modal.addEventListener('keydown', (e) => {
            if (e.key === 'Tab') {
                if (e.shiftKey && document.activeElement === firstElement) {
                    e.preventDefault();
                    lastElement.focus();
                } else if (!e.shiftKey && document.activeElement === lastElement) {
                    e.preventDefault();
                    firstElement.focus();
                }
            }
        });
    }

    openModalLink.addEventListener('click', (e) => {
        e.preventDefault();
        openModal();
    });

    modalCloseButton.addEventListener('click', closeModal);

    modalOverlay.addEventListener('click', (e) => {
        if (e.target === modalOverlay) closeModal();
    });

    document.addEventListener('keydown', (e) => {
        if (e.key === 'Escape' && modalOverlay.classList.contains('quality-modal__overlay--visible')) {
            closeModal();
        }
    });

    // Form submission
    form.addEventListener('submit', async (e) => {
        e.preventDefault();
        const submitButton = form.querySelector('.upload-form__submit');
        submitButton.setAttribute('aria-busy', 'true');
        clearErrors();

        const formData = new FormData(form);
        const keywords = Array.from(keywordsList.querySelectorAll('.skillsLearning-item')).map(item => item.dataset.value);
        const platforms = formData.get('GamePlatform').split(',').filter(p => p);

        // Validate required fields
        if (!formData.get('title').trim()) {
            showFormError('title-error', 'Название игры обязательно');
            submitButton.setAttribute('aria-busy', 'false');
            return;
        }
        if (!formData.get('short-description').trim()) {
            showFormError('short-description-error', 'Краткое описание обязательно');
            submitButton.setAttribute('aria-busy', 'false');
            return;
        }
        if (formData.get('short-description').length > 200) {
            showFormError('short-description-error', 'Краткое описание должно быть не длиннее 200 символов');
            submitButton.setAttribute('aria-busy', 'false');
            return;
        }
        if (!formData.get('description').trim()) {
            showFormError('description-error', 'Описание игры обязательно');
            submitButton.setAttribute('aria-busy', 'false');
            return;
        }
        if (!formData.get('rules').trim()) {
            showFormError('rules-error', 'Правила игры обязательны');
            submitButton.setAttribute('aria-busy', 'false');
            return;
        }
        if (selectedSkillsLearning.length === 0) {
            showFormError('skillsLearning-error', 'Необходимо выбрать хотя б один навык');
            submitButton.setAttribute('aria-busy', 'false');
            return;
        }
        if (selectedPlatforms.length === 0) {
            showFormError('platform-error', 'Необходимо выбрать хотя бы одну платформу');
            submitButton.setAttribute('aria-busy', 'false');
            return;
        }
        if (platforms.includes('Web-Mobile') || platforms.includes('Web-Desktop')) {
            if (!formData.get('GameURL').trim()) {
                showFormError('file-url-error', 'URL игры обязателен для веб-платформ');
                submitButton.setAttribute('aria-busy', 'false');
                return;
            }
        }
        if (platforms.includes('Desktop')) {
            if (!formData.get('UploadedGame')) {
                showFormError('file-error', 'Файл игры обязателен для десктопной платформы');
                submitButton.setAttribute('aria-busy', 'false');
                return;
            }
            const gameFile = gameFileInput.files[0];
            if (gameFile) {
                const fileExtension = gameFile.name.slice(gameFile.name.lastIndexOf('.')).toLowerCase();
                if (!ALLOWED_GAME_EXTENSIONS.includes(fileExtension)) {
                    showFormError('file-error', `Недопустимое расширение файла. Разрешено: ${ALLOWED_GAME_EXTENSIONS.join(', ')}`);
                    submitButton.setAttribute('aria-busy', 'false');
                    return;
                }
            }
        }

        formData.set('skillsLearning', JSON.stringify(selectedSkillsLearning)); 
        formData.set('keywords', JSON.stringify(keywords));
        formData.set('platform', JSON.stringify(platforms));

        try {
            //const response = await fetch(`${API_URL}/upload/index`, {
            const response = await fetch(`/upload/index`, {
                method: 'POST',
                body: formData
            });

            if (!response.ok) {
                const errorData = await response.json();
                throw new Error(errorData.message || 'Ошибка загрузки игры');
            }

            // Срабатывает, когда игры не загружена по причине валидации серверной
            showNotification('Игра успешно загружена!', 'success');
            form.reset();
            selectedSkillsLearning = [];
            selectedPlatforms = [];
            updateSelectedItems(selectedskillsLearningList, skillsLearningInput, selectedSkillsLearning, skillsLearningOptions, skillsLearningSelected.querySelector('.custom-dropdown__placeholder'));
            updateSelectedItems(selectedPlatformsList, platformInput, selectedPlatforms, platformOptions, platformSelected.querySelector('.custom-dropdown__placeholder'));
            keywordsList.innerHTML = '';
            document.querySelector('#cover-preview').innerHTML = '';
            document.querySelector('#file-preview').innerHTML = '';
            fileUrlGroup.style.display = 'none';
            fileUploadGroup.style.display = 'none';
        } catch (error) {
            showNotification(error.message || 'Не удалось загрузить игру. Попробуйте снова.', 'error');
            console.error('Upload error:', error);
        } finally {
            submitButton.setAttribute('aria-busy', 'false');
        }
    });

    // Initialize dropdowns
    updateSelectedItems(selectedskillsLearningList, skillsLearningInput, selectedSkillsLearning, skillsLearningOptions, skillsLearningSelected.querySelector('.custom-dropdown__placeholder'));
    updateSelectedItems(selectedPlatformsList, platformInput, selectedPlatforms, platformOptions, platformSelected.querySelector('.custom-dropdown__placeholder'));
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

    // Check MIME type
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

    // Handle file removal
    fileItem.querySelector('.file-upload__file-trash').addEventListener('click', () => {
        fileItem.remove();
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

/**
 * Shows a form error message
 * @param {string} errorId - The ID of the error element
 * @param {string} message - The error message
 */
function showFormError(errorId, message) {
    const errorElement = document.querySelector(`#${errorId}`);
    errorElement.textContent = message;
    errorElement.classList.add('error-message--visible');
}

/**
 * Clears all form errors
 */
function clearErrors() {
    document.querySelectorAll('.error-message').forEach((el) => {
        el.textContent = '';
        el.classList.remove('error-message--visible');
    });
}

// Initialize on DOM content loaded
document.addEventListener('DOMContentLoaded', initializeUploadForm);