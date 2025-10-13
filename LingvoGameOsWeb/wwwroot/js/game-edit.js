// game-edit.js - логика для страницы редактирования игры
export function initializeGameEdit() {
    console.log('Initializing game edit form...');

    const editForm = document.getElementById('upload-game-form');
    if (!editForm) {
        console.log('Game edit form not found');
        return;
    }

    setupDropdowns();
    setupCoverUpload();
    setupScreenshotsUpload();
    setupGameFileUpload();
    setupModal();
}

// Функционал выпадающих списков
function setupDropdowns() {
    setupSkillsLearningDropdown();
    setupPlatformDropdown();
}

function setupSkillsLearningDropdown() {
    const skillsLearningDropdown = document.querySelector('#skillsLearning-dropdown');
    if (!skillsLearningDropdown) {
        console.log('Skills learning dropdown not found');
        return;
    }

    const skillsLearningSelected = skillsLearningDropdown.querySelector('.custom-dropdown__selected');
    const skillsLearningMenu = skillsLearningDropdown.querySelector('.custom-dropdown__menu');
    const skillsLearningSearch = skillsLearningDropdown.querySelector('.custom-dropdown__search');
    const skillsLearningOptions = skillsLearningDropdown.querySelectorAll('.custom-dropdown__option');
    const selectedSkillsLearningList = document.querySelector('#selected-skillsLearning-list');
    const skillsLearningInput = document.querySelector('#game-skillsLearning');
    const skillsLearningError = document.querySelector('#skillsLearning-error');

    let selectedSkillsLearning = [];

    // Инициализируем из скрытого поля
    if (skillsLearningInput && skillsLearningInput.value) {
        selectedSkillsLearning = skillsLearningInput.value.split(',').filter(item => item.trim() !== '');
    }

    updateSkillsLearningDropdown();

    // Обработчики событий для выпадающего списка
    skillsLearningSelected.addEventListener('click', () => toggleDropdown(skillsLearningSelected, skillsLearningMenu));
    skillsLearningSelected.addEventListener('keydown', (e) => {
        if (e.key === 'Enter' || e.key === ' ') {
            e.preventDefault();
            toggleDropdown(skillsLearningSelected, skillsLearningMenu);
        }
    });

    skillsLearningSearch.addEventListener('input', () => filterDropdownItems(skillsLearningSearch, skillsLearningOptions));
    skillsLearningSearch.addEventListener('keydown', (e) => {
        if (e.key === 'Escape') {
            closeDropdown(skillsLearningSelected, skillsLearningMenu);
            skillsLearningSelected.focus();
        }
    });

    skillsLearningOptions.forEach(option => {
        option.addEventListener('click', () => {
            const value = option.dataset.value;
            handleSkillsLearningSelection(value, option);
        });

        option.addEventListener('keydown', (e) => {
            if (e.key === 'Enter' || e.key === ' ') {
                e.preventDefault();
                const value = option.dataset.value;
                handleSkillsLearningSelection(value, option);
            }
        });
    });

    // Обработчик удаления выбранных навыков
    selectedSkillsLearningList.addEventListener('click', (e) => {
        const removeButton = e.target.closest('.selected-item__remove');
        if (removeButton) {
            const item = removeButton.parentElement;
            const value = item.dataset.value;
            removeSkillsLearningItem(value);
        }
    });

    function handleSkillsLearningSelection(value, option) {
        if (value === 'select-all') {
            // Выбрать все
            selectedSkillsLearning = [];
            skillsLearningOptions.forEach(opt => {
                if (opt.dataset.value !== 'select-all') {
                    selectedSkillsLearning.push(opt.dataset.value);
                }
            });
        } else {
            // Переключить выбор отдельного элемента
            const index = selectedSkillsLearning.indexOf(value);
            if (index === -1) {
                selectedSkillsLearning.push(value);
            } else {
                selectedSkillsLearning.splice(index, 1);
            }
        }
        updateSkillsLearningDropdown();
    }

    function removeSkillsLearningItem(value) {
        const index = selectedSkillsLearning.indexOf(value);
        if (index !== -1) {
            selectedSkillsLearning.splice(index, 1);
            updateSkillsLearningDropdown();
        }
    }

    function updateSkillsLearningDropdown() {
        // Обновляем скрытое поле
        if (skillsLearningInput) {
            skillsLearningInput.value = selectedSkillsLearning.join(',');
        }

        // Обновляем отображение выбранных элементов
        selectedSkillsLearningList.innerHTML = '';
        selectedSkillsLearning.forEach(value => {
            const option = Array.from(skillsLearningOptions).find(opt => opt.dataset.value === value);
            if (option) {
                const item = document.createElement('span');
                item.className = 'selected-item';
                item.dataset.value = value;
                item.innerHTML = `
                    ${option.textContent.trim()}
                    <span class="selected-item__remove" role="button" aria-label="Удалить ${option.textContent.trim()}">×</span>
                `;
                selectedSkillsLearningList.appendChild(item);
            }
        });

        // Обновляем состояние опций в dropdown
        skillsLearningOptions.forEach(option => {
            const isSelected = selectedSkillsLearning.includes(option.dataset.value);
            option.classList.toggle('selected', isSelected);
            option.setAttribute('aria-selected', isSelected);
        });

        // Обновляем placeholder
        const placeholder = skillsLearningSelected.querySelector('.custom-dropdown__placeholder');
        if (placeholder) {
            placeholder.textContent = selectedSkillsLearning.length > 0
                ? `${selectedSkillsLearning.length} выбрано`
                : 'Выберите развиваемые навыки';
        }
    }

    // Закрытие dropdown при клике вне его
    document.addEventListener('click', (e) => {
        if (!skillsLearningDropdown.contains(e.target)) {
            closeDropdown(skillsLearningSelected, skillsLearningMenu);
        }
    });
}

function setupPlatformDropdown() {
    const platformDropdown = document.querySelector('#platform-dropdown');
    if (!platformDropdown) {
        console.log('Platform dropdown not found');
        return;
    }

    const platformSelected = platformDropdown.querySelector('.custom-dropdown__selected');
    const platformMenu = platformDropdown.querySelector('.custom-dropdown__menu');
    const platformOptions = platformDropdown.querySelectorAll('.custom-dropdown__option');
    const selectedPlatformsList = document.querySelector('#selected-platforms-list');
    const platformInput = document.querySelector('#game-platform');

    let selectedPlatforms = [];

    // Инициализируем из текущего значения
    if (platformInput && platformInput.value) {
        selectedPlatforms = [platformInput.value];
    }

    updatePlatformDropdown();

    // Обработчики событий
    platformSelected.addEventListener('click', () => toggleDropdown(platformSelected, platformMenu));
    platformSelected.addEventListener('keydown', (e) => {
        if (e.key === 'Enter' || e.key === ' ') {
            e.preventDefault();
            toggleDropdown(platformSelected, platformMenu);
        }
    });

    platformOptions.forEach(option => {
        option.addEventListener('click', () => {
            const value = option.dataset.value;
            selectedPlatforms = [value];
            updatePlatformDropdown();
            closeDropdown(platformSelected, platformMenu);
        });
    });

    // Обработчик удаления выбранной платформы
    selectedPlatformsList.addEventListener('click', (e) => {
        const removeButton = e.target.closest('.selected-item__remove');
        if (removeButton) {
            selectedPlatforms = [];
            updatePlatformDropdown();
        }
    });

    function updatePlatformDropdown() {
        // Обновляем скрытое поле
        if (platformInput) {
            platformInput.value = selectedPlatforms.length > 0 ? selectedPlatforms[0] : '';
        }

        // Обновляем отображение выбранных элементов
        selectedPlatformsList.innerHTML = '';
        selectedPlatforms.forEach(value => {
            const option = Array.from(platformOptions).find(opt => opt.dataset.value === value);
            if (option) {
                const item = document.createElement('span');
                item.className = 'selected-item';
                item.dataset.value = value;
                item.innerHTML = `
                    ${option.textContent.trim()}
                    <span class="selected-item__remove" role="button" aria-label="Удалить ${option.textContent.trim()}">×</span>
                `;
                selectedPlatformsList.appendChild(item);
            }
        });

        // Обновляем состояние опций в dropdown
        platformOptions.forEach(option => {
            const isSelected = selectedPlatforms.includes(option.dataset.value);
            option.classList.toggle('selected', isSelected);
            option.setAttribute('aria-selected', isSelected);
        });

        // Обновляем placeholder
        const placeholder = platformSelected.querySelector('.custom-dropdown__placeholder');
        if (placeholder) {
            placeholder.textContent = selectedPlatforms.length > 0
                ? selectedPlatforms[0]
                : 'Выберите платформу';
        }

        // Обновляем видимость полей ввода файла/URL
        updatePlatformSpecificFields();
    }

    function updatePlatformSpecificFields() {
        const fileUrlGroup = document.getElementById('game-file-url-group');
        const fileUploadGroup = document.getElementById('game-file-upload-group');

        if (fileUrlGroup && fileUploadGroup) {
            const platform = selectedPlatforms.length > 0 ? selectedPlatforms[0] : '';

            if (platform === 'Desktop') {
                fileUrlGroup.style.display = 'none';
                fileUploadGroup.style.display = 'block';
            } else if (platform === 'Web-Mobile' || platform === 'Web-Desktop') {
                fileUrlGroup.style.display = 'block';
                fileUploadGroup.style.display = 'none';
            } else {
                fileUrlGroup.style.display = 'none';
                fileUploadGroup.style.display = 'none';
            }
        }
    }
}

// Вспомогательные функции для dropdown
function toggleDropdown(selected, menu) {
    const isActive = selected.classList.contains('active');
    selected.classList.toggle('active', !isActive);
    menu.classList.toggle('active', !isActive);
    selected.setAttribute('aria-expanded', !isActive);
    menu.setAttribute('aria-hidden', isActive);
}

function closeDropdown(selected, menu) {
    selected.classList.remove('active');
    menu.classList.remove('active');
    selected.setAttribute('aria-expanded', 'false');
    menu.setAttribute('aria-hidden', 'true');
}

function filterDropdownItems(searchInput, options) {
    if (!searchInput) return;
    const query = searchInput.value.toLowerCase();
    options.forEach(option => {
        const text = option.textContent.toLowerCase();
        option.style.display = text.includes(query) || option.dataset.value === 'select-all' ? '' : 'none';
    });
}

// Для обложки игры
function setupCoverUpload() {
    const fileInput = document.getElementById('cover-image-file');
    const previewContainer = document.getElementById('cover-image-preview');
    const dropzone = document.querySelector('#cover-dropzone .file-upload__dropzone');

    if (!fileInput || !previewContainer) {
        console.log('Cover upload elements not found');
        return;
    }

    let currentFile = null;

    // Если у модели уже есть изображение, показываем его
    const modelImagePath = previewContainer.getAttribute('data-existing-image') || '';
    if (modelImagePath) {
        showPreview(modelImagePath, {
            name: previewContainer.getAttribute('data-image-name') || 'Existing image',
            size: previewContainer.getAttribute('data-image-size') || '0'
        });
    }

    // Обработка выбора файла через input
    fileInput.addEventListener('change', function (e) {
        handleFileSelect(e.target.files[0]);
    });

    // Обработка drag and drop
    if (dropzone) {
        dropzone.addEventListener('dragover', function (e) {
            e.preventDefault();
            dropzone.classList.add('file-upload__dropzone--dragover');
        });

        dropzone.addEventListener('dragleave', function () {
            dropzone.classList.remove('file-upload__dropzone--dragover');
        });

        dropzone.addEventListener('drop', function (e) {
            e.preventDefault();
            dropzone.classList.remove('file-upload__dropzone--dragover');
            if (e.dataTransfer.files.length > 0) {
                handleFileSelect(e.dataTransfer.files[0]);
            }
        });

        // Клик по dropzone открывает диалог выбора файла
        dropzone.addEventListener('click', function () {
            fileInput.click();
        });
    }

    function handleFileSelect(file) {
        if (!file || !file.type.match('image.*')) {
            return;
        }

        // Очищаем предыдущее превью
        clearPreview();

        const reader = new FileReader();
        reader.onload = function (event) {
            showPreview(event.target.result, {
                name: file.name,
                size: Math.round(file.size / 1024)
            });
            currentFile = file;
        };
        reader.readAsDataURL(file);
    }

    function showPreview(imageSrc, fileInfo) {
        previewContainer.innerHTML = `
            <div class="file-upload__file">
                <span class="file-upload__file-name">${fileInfo.name}</span>
                <span class="file-upload__file-size">${fileInfo.size} Кб</span>
                <img src="/icon/trash.svg" alt="Удалить файл" class="file-upload__file-trash" role="button">
            </div>
            <img src="${imageSrc}" class="file-upload__preview-image" alt="Превью обложки игры">
        `;

        // Добавляем обработчик для кнопки удаления
        const deleteButton = previewContainer.querySelector('.file-upload__file-trash');
        deleteButton.addEventListener('click', function (e) {
            e.stopPropagation();
            clearPreview();
            fileInput.value = '';
            currentFile = null;
        });
    }

    function clearPreview() {
        previewContainer.innerHTML = '';
    }
}

// Для скриншотов игры
function setupScreenshotsUpload() {
    const MAX_IMAGES = 3;
    const fileInput = document.getElementById('game-screenshots-file');
    const previewContainer = document.getElementById('screenshots-preview');
    const errorElement = document.getElementById('screenshots-error');
    const form = document.querySelector('form');

    if (!fileInput || !previewContainer) {
        console.log('Screenshots upload elements not found');
        return;
    }

    let currentImages = []; // Все текущие изображения (новые + существующие)
    let originalImages = []; // Исходные существующие изображения

    // Инициализация существующих изображений из data-атрибутов
    const existingImageElements = previewContainer.querySelectorAll('[data-existing-image]');
    existingImageElements.forEach(imgElement => {
        const imageData = {
            type: 'existing',
            id: imgElement.getAttribute('data-image-id'),
            name: imgElement.getAttribute('data-image-name'),
            path: imgElement.getAttribute('data-existing-image')
        };
        originalImages.push(imageData.id);
        currentImages.push(imageData);
    });

    renderPreviews();

    // Обработка новых файлов
    fileInput.addEventListener('change', function (e) {
        if (errorElement) errorElement.textContent = '';
        const files = Array.from(e.target.files);

        // Проверка лимита
        if (currentImages.length + files.length > MAX_IMAGES) {
            if (errorElement) errorElement.textContent = `Максимум можно загрузить ${MAX_IMAGES} изображения`;
            fileInput.value = '';
            return;
        }

        files.forEach(file => {
            if (!file.type.match('image.*')) {
                if (errorElement) errorElement.textContent = 'Пожалуйста, загружайте только изображения';
                return;
            }

            const reader = new FileReader();
            reader.onload = function (event) {
                currentImages.push({
                    type: 'new',
                    file: file,
                    name: file.name,
                    preview: event.target.result
                });
                renderPreviews();
                updateFileInput(); // ВАЖНО: обновляем input после добавления файлов
            };
            reader.readAsDataURL(file);
        });

        fileInput.value = '';
    });

    function renderPreviews() {
        previewContainer.innerHTML = '';

        // Обновляем статус кнопки загрузки
        fileInput.disabled = currentImages.length >= MAX_IMAGES;

        currentImages.forEach((img, index) => {
            const previewElement = document.createElement('div');
            previewElement.className = 'preview-item';
            previewElement.innerHTML = `
                <div class="file-upload__file">
                    <span class="file-upload__file-name">${img.name}</span>
                    <img src="/icon/trash.svg" class="file-upload__file-trash"
                         alt="Удалить" title="Удалить">
                </div>
                <img src="${img.type === 'existing' ? img.path : img.preview}"
                     class="file-upload__preview-image">
            `;

            // Обработчик удаления
            previewElement.querySelector('.file-upload__file-trash').addEventListener('click', () => {
                currentImages.splice(index, 1);
                renderPreviews();
                updateFileInput(); // ВАЖНО: обновляем input после удаления
            });

            previewContainer.appendChild(previewElement);
        });
    }

    function updateFileInput() {
        const newInput = document.createElement('input');
        newInput.type = 'file';
        newInput.name = 'UploadedImages';
        newInput.multiple = true;
        newInput.hidden = true;
        newInput.id = 'game-screenshots-file'; // ВАЖНО: сохраняем ID

        const dataTransfer = new DataTransfer();
        const newFiles = currentImages
            .filter(img => img.type === 'new')
            .map(img => img.file);

        newFiles.forEach(file => dataTransfer.items.add(file));
        newInput.files = dataTransfer.files;

        const oldInput = document.getElementById('game-screenshots-file');
        if (oldInput && oldInput.parentNode) {
            oldInput.parentNode.replaceChild(newInput, oldInput);

            // ВАЖНО: перепривязываем обработчики событий к новому input
            newInput.addEventListener('change', function (e) {
                if (errorElement) errorElement.textContent = '';
                const files = Array.from(e.target.files);

                if (currentImages.length + files.length > MAX_IMAGES) {
                    if (errorElement) errorElement.textContent = `Максимум можно загрузить ${MAX_IMAGES} изображения`;
                    newInput.value = '';
                    return;
                }

                files.forEach(file => {
                    if (!file.type.match('image.*')) {
                        if (errorElement) errorElement.textContent = 'Пожалуйста, загружайте только изображения';
                        return;
                    }

                    const reader = new FileReader();
                    reader.onload = function (event) {
                        currentImages.push({
                            type: 'new',
                            file: file,
                            name: file.name,
                            preview: event.target.result
                        });
                        renderPreviews();
                        updateFileInput();
                    };
                    reader.readAsDataURL(file);
                });

                newInput.value = '';
            });
        }
    }

    // Подготовка данных перед отправкой
    if (form) {
        form.addEventListener('submit', function (e) {
            const deletedImages = originalImages.filter(original =>
                !currentImages.some(img => img.type === 'existing' && img.id === original)
            );

            addHiddenInputs('DeletedImages', deletedImages);
            updateFileInput();
        });
    }

    function addHiddenInputs(name, values) {
        if (!form) return;

        // Удаляем старые inputs
        document.querySelectorAll(`input[name^="${name}"]`).forEach(el => el.remove());

        // Добавляем новые
        values.forEach((value, index) => {
            const input = document.createElement('input');
            input.type = 'hidden';
            input.name = `${name}[${index}]`;
            input.value = value;
            form.appendChild(input);
        });
    }
}


// Для файла игры - версия с data-атрибутами
// Для файла игры - ПРОСТАЯ И НАДЕЖНАЯ ВЕРСИЯ
function setupGameFileUpload() {
    const fileInput = document.getElementById('game-cover-file');
    const previewContainer = document.getElementById('file-preview');
    const gameUrlHiddenInput = document.querySelector('input[name="GameURL"]');

    console.log('Initializing game file upload...');

    if (!previewContainer) return;

    // Универсальная функция удаления
    const deleteGameFile = () => {
        console.log('Deleting game file...');

        // 1. Очищаем скрытое поле
        if (gameUrlHiddenInput) {
            gameUrlHiddenInput.value = '';
        }

        // 2. Очищаем file input
        if (fileInput) {
            fileInput.value = '';
        }

        // 3. Показываем сообщение об удалении
        previewContainer.innerHTML = `
            <div class="file-upload__file">
                <span class="file-upload__file-name">Файл игры удален</span>
                <span class="file-upload__file-size">Вы можете загрузить новый файл</span>
            </div>
        `;

        console.log('Game file deleted successfully');
    };

    // Обработчик для ВСЕХ кнопок удаления в контейнере
    previewContainer.addEventListener('click', (e) => {
        if (e.target.closest('.file-upload__file-trash')) {
            e.preventDefault();
            e.stopPropagation();
            deleteGameFile();
        }
    });

    // Обработка нового файла
    if (fileInput) {
        fileInput.addEventListener('change', (e) => {
            const file = e.target.files[0];
            if (!file) return;

            const fileInfo = {
                name: file.name,
                size: Math.round(file.size / (1024 * 1024))
            };

            previewContainer.innerHTML = `
                <div class="file-upload__file">
                    <span class="file-upload__file-name">${fileInfo.name}</span>
                    <span class="file-upload__file-size">${fileInfo.size} Мб</span>
                    <img src="/icon/trash.svg" alt="Удалить файл" class="file-upload__file-trash" role="button">
                </div>
            `;

            console.log('New game file selected:', fileInfo.name);
        });
    }

    // Проверяем, есть ли уже файл при загрузке
    const existingFileElement = previewContainer.querySelector('.file-upload__file');
    if (existingFileElement && existingFileElement.querySelector('.file-upload__file-trash')) {
        console.log('Existing game file found, delete functionality ready');
    }
}

// Модальное окно
function setupModal() {
    const openModalBtn = document.getElementById('open-quality-modal');
    const closeModalBtn = document.getElementById('qualityModalClose');
    const modalOverlay = document.getElementById('qualityModalOverlay');

    if (openModalBtn && modalOverlay) {
        openModalBtn.addEventListener('click', function (e) {
            e.preventDefault();
            modalOverlay.style.display = 'flex';
        });
    }

    if (closeModalBtn && modalOverlay) {
        closeModalBtn.addEventListener('click', function () {
            modalOverlay.style.display = 'none';
        });
    }

    if (modalOverlay) {
        modalOverlay.addEventListener('click', function (e) {
            if (e.target === modalOverlay) {
                modalOverlay.style.display = 'none';
            }
        });
    }
}

// Автоматическая инициализация
document.addEventListener('DOMContentLoaded', function () {
    initializeGameEdit();
});