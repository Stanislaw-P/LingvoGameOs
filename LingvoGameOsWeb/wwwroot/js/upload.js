import { API_URL } from './config.js';
import { showError } from './dom.js';

/**
 * Initializes the upload form and modal functionality
 */
function initializeUploadForm() {
    const form = document.querySelector('#upload-game-form');
    const categoriesInput = document.querySelector('#game-categories');
    const keywordsInput = document.querySelector('#game-keywords');
    const categoriesList = document.querySelector('#categories-list');
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
    const categoryLabels = document.querySelectorAll('#categories-options-list label');
    let lastFocusedElement = null;

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

    // Custom dropdown functionality
    function toggleDropdown() {
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

    platformSelected.addEventListener('click', toggleDropdown);

    platformItems.forEach(item => {
        item.addEventListener('click', () => {
            selectPlatform(item.dataset.value, item.textContent);
        });
    });

    platformSelected.addEventListener('keydown', (e) => {
        if (e.key === 'Enter' || e.key === ' ') {
            e.preventDefault();
            toggleDropdown();
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

    // Handle categories input
    categoriesInput.addEventListener('keydown', (e) => {
        if (e.key === 'Enter' && e.target.value.trim()) {
            e.preventDefault();
            addTag(categoriesList, e.target.value.trim(), 'category');
            e.target.value = '';
        }
    });

    // Handle category labels click with toggle functionality
    categoryLabels.forEach(label => {
        label.classList.add('category-label'); // Add class for styling
        label.setAttribute('role', 'button'); // Improve accessibility
        label.setAttribute('tabindex', '0'); // Make focusable
        label.addEventListener('click', (e) => {
            e.preventDefault();
            const categoryText = label.textContent.trim();
            const isSelected = label.classList.contains('selected');

            if (isSelected) {
                // Deselect: remove from categoriesList and unhighlight
                const tag = Array.from(categoriesList.querySelectorAll('.category-item'))
                    .find(item => item.dataset.value === categoryText);
                if (tag) tag.remove();
                label.classList.remove('selected');
                label.setAttribute('aria-pressed', 'false');
            } else {
                // Select: add to categoriesList and highlight
                const existingCategories = Array.from(categoriesList.querySelectorAll('.category-item'))
                    .map(item => item.dataset.value);
                if (!existingCategories.includes(categoryText)) {
                    addTag(categoriesList, categoryText, 'category');
                    label.classList.add('selected');
                    label.setAttribute('aria-pressed', 'true');
                }
            }
        });

        // Handle keyboard interaction
        label.addEventListener('keydown', (e) => {
            if (e.key === 'Enter' || e.key === ' ') {
                e.preventDefault();
                label.click(); // Trigger click event
            }
        });
    });

    // Handle keywords input
    keywordsInput.addEventListener('keydown', (e) => {
        if (e.key === 'Enter' && e.target.value.trim()) {
            e.preventDefault();
            addTag(keywordsList, e.target.value.trim(), 'keyword');
            e.target.value = '';
        }
    });

    // Handle tag removal from categoriesList
    categoriesList.addEventListener('click', (e) => {
        if (e.target.classList.contains('category-item__remove')) {
            const tag = e.target.parentElement;
            const categoryText = tag.dataset.value;
            tag.remove();
            // Unhighlight corresponding label
            const label = Array.from(categoryLabels).find(l => l.textContent.trim() === categoryText);
            if (label) {
                label.classList.remove('selected');
                label.setAttribute('aria-pressed', 'false');
            }
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
        const categories = Array.from(categoriesList.querySelectorAll('.category-item')).map(item => item.dataset.value);
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
        if (categories.length === 0) {
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

        formData.set('categories', JSON.stringify(categories));
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
            categoriesList.innerHTML = '';
            keywordsList.innerHTML = '';
            document.querySelector('#cover-preview').innerHTML = '';
            document.querySelector('#file-preview').innerHTML = '';
            fileUrlGroup.style.display = 'none';
            fileUploadGroup.style.display = 'none';
            platformSelected.querySelector('span').textContent = 'Выберите платформу';
            platformInput.value = '';
            // Reset category labels
            categoryLabels.forEach(label => {
                label.classList.remove('selected');
                label.setAttribute('aria-pressed', 'false');
            });
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