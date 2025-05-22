import { API_URL } from './config.js';
import { showError } from './dom.js';

/**
 * Initializes the upload form and modal functionality
 */
function initializeUploadForm() {
    // ... (previous variable declarations remain unchanged)
    const form = document.querySelector('#upload-game-form');
    const categoriesDropdown = document.querySelector('#categories-dropdown');
    const categoriesSelected = categoriesDropdown.querySelector('.custom-dropdown__selected');
    const categoriesMenu = categoriesDropdown.querySelector('.custom-dropdown__menu');
    const categoriesSearch = categoriesDropdown.querySelector('.custom-dropdown__search');
    const categoriesOptions = categoriesDropdown.querySelectorAll('.custom-dropdown__option');
    const selectedCategoriesList = document.querySelector('#selected-categories-list');
    const categoriesInput = document.querySelector('#game-categories');
    const categoriesError = document.querySelector('#categories-error');
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
    const platformInput = document.querySelector('#game-platform');
    const fileUrlGroup = document.querySelector('#game-file-url-group');
    const fileUploadGroup = document.querySelector('#game-file-upload-group');
    const platformDropdown = document.querySelector('#platform-dropdown');
    const platformSelected = platformDropdown.querySelector('.custom-dropdown__selected');
    const platformList = platformDropdown.querySelector('.custom-dropdown__list');
    const platformItems = platformList.querySelectorAll('.custom-dropdown__item');
    let lastFocusedElement = null;
    let selectedCategories = [];

    // Allowed game file types and extensions
    const ALLOWED_GAME_TYPES = [
        'application/zip',
        'application/x-rar-compressed',
        'application/x-7z-compressed',
        'application/vnd.android.package-archive' // APK
    ];
    const ALLOWED_GAME_EXTENSIONS = ['.zip', '.rar', '.7z', '.apk', '.exe', '.app', '.dmg'];
    const MAX_GAME_SIZE = 2 * 1024 * 1024 * 1024; // 2GB

    // Allowed cover image types
    const ALLOWED_COVER_TYPES = ['image/jpeg', 'image/png', 'image/webp'];
    const MAX_COVER_SIZE = 30 * 1024 * 1024; // 30MB

    // Custom dropdown functionality for platforms
    function togglePlatformDropdown() {
        const isOpen = !platformList.hasAttribute('hidden');
        platformList.toggleAttribute('hidden', isOpen);
        platformSelected.setAttribute('aria-expanded', !isOpen);
    }

    function selectPlatform(value, text) {
        platformInput.value = value;
        platformSelected.querySelector('span').textContent = text;
        platformList.setAttribute('hidden', '');
        platformSelected.setAttribute('aria-expanded', 'false');
        const event = new Event('change', { bubbles: true });
        platformInput.dispatchEvent(event);
    }

    platformSelected.addEventListener('click', togglePlatformDropdown);

    platformItems.forEach(item => {
        item.addEventListener('click', () => {
            selectPlatform(item.dataset.value, item.textContent);
        });
    });

    platformSelected.addEventListener('keydown', (e) => {
        if (e.key === 'Enter' || e.key === ' ') {
            e.preventDefault();
            togglePlatformDropdown();
        }
    });

    platformList.addEventListener('keydown', (e) => {
        const items = Array.from(platformItems);
        const current = items.find(item => item === document.activeElement);
        const index = current ? items.indexOf(current) : -1;

        if (e.key === 'ArrowDown') {
            e.preventDefault();
            const next = index < items.length - 1 ? items[index + 1] : items[0];
            next.focus();
        } else if (e.key === 'ArrowUp') {
            e.preventDefault();
            const prev = index > 0 ? items[index - 1] : items[items.length - 1];
            prev.focus();
        } else if (e.key === 'Enter' && current) {
            e.preventDefault();
            selectPlatform(current.dataset.value, current.textContent);
        } else if (e.key === 'Escape') {
            platformList.setAttribute('hidden', '');
            platformSelected.setAttribute('aria-expanded', 'false');
            platformSelected.focus();
        }
    });

    document.addEventListener('click', (e) => {
        if (!platformDropdown.contains(e.target)) {
            platformList.setAttribute('hidden', '');
            platformSelected.setAttribute('aria-expanded', 'false');
        }
        if (!categoriesDropdown.contains(e.target)) {
            categoriesSelected.setAttribute('aria-expanded', 'false');
            categoriesMenu.setAttribute('aria-hidden', 'true');
        }
    });

    // Handle platform selection
    platformInput.addEventListener('change', () => {
        const platform = platformInput.value;
        fileUrlGroup.style.display = 'none';
        fileUploadGroup.style.display = 'none';

        if (platform === 'web-mobile' || platform === 'web-desktop') {
            fileUrlGroup.style.display = 'block';
        } else if (platform === 'desktop') {
            fileUploadGroup.style.display = 'block';
        }
    });

    // Handle categories selection
    function updateSelectedCategories() {
        selectedCategoriesList.innerHTML = '';
        categoriesInput.value = selectedCategories.join(',');
        categoriesSelected.querySelector('.custom-dropdown__placeholder').textContent =
            selectedCategories.length ? `${selectedCategories.length} выбрано` : 'Выберите категории';

        selectedCategories.forEach(value => {
            const option = Array.from(categoriesOptions).find(opt => opt.dataset.value === value);
            if (option) {
                const categoryItem = document.createElement('div');
                categoryItem.className = 'selected-category-item';
                categoryItem.dataset.value = value;
                categoryItem.innerHTML = `
                    ${option.textContent}
                    <span class="selected-category-item__remove" role="button" aria-label="Удалить ${option.textContent}" tabindex="0">×</span>
                `;
                selectedCategoriesList.appendChild(categoryItem);
            }
        });

        categoriesOptions.forEach(option => {
            option.setAttribute('aria-selected', selectedCategories.includes(option.dataset.value));
        });

        if (selectedCategories.length > 0) {
            categoriesError.classList.remove('error-message--visible');
            categoriesError.textContent = '';
        }
    }

    function toggleCategoriesDropdown() {
        const isExpanded = categoriesSelected.getAttribute('aria-expanded') === 'true';
        categoriesSelected.setAttribute('aria-expanded', !isExpanded);
        categoriesMenu.setAttribute('aria-hidden', isExpanded);
        categoriesMenu.style.display = isExpanded ? 'none' : 'block';
        if (!isExpanded) {
            categoriesSearch.focus();
            categoriesMenu.classList.add('custom-dropdown__menu--open');
        } else {
            categoriesMenu.classList.remove('custom-dropdown__menu--open');
        }
    }

    function addCategory(value, text) {
        if (!selectedCategories.includes(value)) {
            selectedCategories.push(value);
            updateSelectedCategories();
            const newTag = selectedCategoriesList.lastElementChild;
            if (newTag) {
                newTag.classList.add('selected-category-item--added');
                setTimeout(() => newTag.classList.remove('selected-category-item--added'), 300);
            }
        }
    }

    function removeCategory(value) {
        selectedCategories = selectedCategories.filter(cat => cat !== value);
        updateSelectedCategories();
    }

    function filterCategories() {
        const query = categoriesSearch.value.toLowerCase();
        categoriesOptions.forEach(option => {
            const text = option.textContent.toLowerCase();
            option.style.display = text.includes(query) ? 'block' : 'none';
        });
    }

    categoriesSelected.addEventListener('click', toggleCategoriesDropdown);
    categoriesSelected.addEventListener('keydown', (e) => {
        if (e.key === 'Enter' || e.key === ' ') {
            e.preventDefault();
            toggleCategoriesDropdown();
        }
    });

    categoriesSearch.addEventListener('input', filterCategories);
    categoriesSearch.addEventListener('keydown', (e) => {
        if (e.key === 'Escape') {
            categoriesSelected.setAttribute('aria-expanded', 'false');
            categoriesMenu.setAttribute('aria-hidden', 'true');
            categoriesMenu.style.display = 'none';
            categoriesSelected.focus();
        }
    });

    categoriesOptions.forEach(option => {
        option.addEventListener('click', () => {
            const value = option.dataset.value;
            const text = option.textContent;
            addCategory(value, text);
        });
        option.addEventListener('keydown', (e) => {
            if (e.key === 'Enter' || e.key === ' ') {
                e.preventDefault();
                const value = option.dataset.value;
                const text = option.textContent;
                addCategory(value, text);
            }
        });
    });

    selectedCategoriesList.addEventListener('click', (e) => {
        if (e.target.classList.contains('selected-category-item__remove')) {
            const tag = e.target.parentElement;
            const value = tag.dataset.value;
            removeCategory(value);
        }
    });

    selectedCategoriesList.addEventListener('keydown', (e) => {
        if (e.target.classList.contains('selected-category-item__remove') && (e.key === 'Enter' || e.key === ' ')) {
            e.preventDefault();
            const tag = e.target.parentElement;
            const value = tag.dataset.value;
            removeCategory(value);
        }
    });

    document.addEventListener('click', (e) => {
        if (!categoriesDropdown.contains(e.target) && categoriesSelected.getAttribute('aria-expanded') === 'true') {
            categoriesSelected.setAttribute('aria-expanded', 'false');
            categoriesMenu.setAttribute('aria-hidden', 'true');
            categoriesMenu.style.display = 'none';
        }
    });

    document.addEventListener('keydown', (e) => {
        if (e.key === 'Escape' && categoriesSelected.getAttribute('aria-expanded') === 'true') {
            categoriesSelected.setAttribute('aria-expanded', 'false');
            categoriesMenu.setAttribute('aria-hidden', 'true');
            categoriesMenu.style.display = 'none';
            categoriesSelected.focus();
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
        if (e.target.classList.contains('category-item__remove')) {
            e.target.parentElement.remove();
        }
    });

    // Handle file uploads with improved validation
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
        const keywords = Array.from(keywordsList.querySelectorAll('.category-item')).map(item => item.dataset.value);
        const platform = formData.get('platform');

        // Validate required fields
        if (!formData.get('title').trim()) {
            showFormError('title-error', 'Game title is required');
            submitButton.setAttribute('aria-busy', 'false');
            return;
        }
        if (!formData.get('short-description').trim()) {
            showFormError('short-description-error', 'Short description is required');
            submitButton.setAttribute('aria-busy', 'false');
            return;
        }
        if (formData.get('short-description').length > 200) {
            showFormError('short-description-error', 'Short description must be 200 characters or less');
            submitButton.setAttribute('aria-busy', 'false');
            return;
        }
        if (!formData.get('description').trim()) {
            showFormError('description-error', 'Game description is required');
            submitButton.setAttribute('aria-busy', 'false');
            return;
        }
        if (!formData.get('rules').trim()) {
            showFormError('rules-error', 'Game rules are required');
            submitButton.setAttribute('aria-busy', 'false');
            return;
        }
        if (selectedCategories.length === 0) {
            showFormError('categories-error', 'At least one category is required');
            submitButton.setAttribute('aria-busy', 'false');
            return;
        }
        if (!formData.get('level')) {
            showFormError('level-error', 'Please select a level');
            submitButton.setAttribute('aria-busy', 'false');
            return;
        }
        if (!platform) {
            showFormError('platform-error', 'Please select a platform');
            submitButton.setAttribute('aria-busy', 'false');
            return;
        }
        if (platform === 'web-mobile' || platform === 'web-desktop') {
            if (!formData.get('file-url').trim()) {
                showFormError('file-url-error', 'Game URL is required for web platforms');
                submitButton.setAttribute('aria-busy', 'false');
                return;
        }
        } else if (platform === 'desktop') {
            if (!formData.get('file-file')) {
                showFormError('file-error', 'Game file is required for desktop platform');
                submitButton.setAttribute('aria-busy', 'false');
                return;
            }
            const gameFile = gameFileInput.files[0];
            if (gameFile) {
                const fileExtension = gameFile.name.slice(gameFile.name.lastIndexOf('.')).toLowerCase();
                if (!ALLOWED_GAME_EXTENSIONS.includes(fileExtension)) {
                    showFormError('file-error', `Invalid file extension. Allowed: ${ALLOWED_GAME_EXTENSIONS.join(', ')}`);
                    submitButton.setAttribute('aria-busy', 'false');
                    return;
                }
            }
        }

        formData.set('categories', JSON.stringify(selectedCategories));
        formData.set('keywords', JSON.stringify(keywords));

        try {
            const response = await fetch(`${API_URL}/games/upload`, {
                method: 'POST',
                body: formData
            });

            if (!response.ok) {
                const errorData = await response.json();
                throw new Error(errorData.message || 'Failed to upload game');
            }

            showNotification('Game uploaded successfully!', 'success');
            form.reset();
            selectedCategories = [];
            updateSelectedCategories();
            keywordsList.innerHTML = '';
            document.querySelector('#cover-preview').innerHTML = '';
            document.querySelector('#file-preview').innerHTML = '';
            fileUrlGroup.style.display = 'none';
            fileUploadGroup.style.display = 'none';
            platformSelected.querySelector('span').textContent = 'Выберите платформу';
            platformInput.value = '';
            categoriesSelected.querySelector('.custom-dropdown__placeholder').textContent = 'Выберите категории';
        } catch (error) {
            showNotification(error.message || 'Failed to upload game. Please try again.', 'error');
            console.error('Upload error:', error);
        } finally {
            submitButton.setAttribute('aria-busy', 'false');
        }
    });
}

/**
 * Adds a tag to the specified list
 * @param {HTMLElement} list - The list to add the tag to
 * @param {string} value - The tag value
 * @param {string} type - The type of tag (category/keyword)
 */
function addTag(list, value, type) {
    const tag = document.createElement('span');
    tag.className = 'category-item';
    tag.dataset.value = value;
    tag.innerHTML = `
        ${value}
        <span class="category-item__remove" role="button" aria-label="Remove ${value}">×</span>
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
        showNotification(`Invalid file extension. Allowed: ${allowedExtensions.join(', ')}`, 'error');
        fileInput.value = '';
        return;
    }

    // Check MIME type
    if (allowedTypes && !allowedTypes.includes(file.type)) {
        showNotification(`Invalid file type. Allowed: ${allowedTypes.join(', ')}`, 'error');
        fileInput.value = '';
        return;
    }

    // Check file size
    if (file.size > maxSize) {
        const maxSizeMB = maxSize / (1024 * 1024);
        const fileSizeMB = (file.size / (1024 * 1024)).toFixed(2);
        showNotification(`File too large (${fileSizeMB}MB). Max size: ${maxSizeMB}MB`, 'error');
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
        <img src="icon/trash.svg" alt="Remove file" class="file-upload__file-trash" role="button" aria-label="Remove ${file.name}" />
    `;
    preview.appendChild(fileItem);

    // Special handling for image previews
    if (fileInput.id === 'game-cover-file' && file.type.startsWith('image/')) {
        const reader = new FileReader();
        reader.onload = (e) => {
            const img = document.createElement('img');
            img.src = e.target.result;
            img.className = 'file-upload__preview-image';
            img.alt = 'Game cover preview';
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