let isSoundOn = true;

const soundToggle = document.querySelector('.game-play__sound-toggle');
const successSound = document.getElementById('successSound');
const errorSound = document.getElementById('errorSound');
const titleButton = document.querySelector('.game-play__title-button');
const gameInfoModal = document.getElementById('gameInfoModal');
const pauseModal = document.getElementById('pauseModal');
const pauseButton = document.querySelector('.game-play__pause');
const fullscreenButton = document.querySelector('.game-play__fullscreen');
const closeButton = document.querySelector('.game-play__close');
const logoLink = document.querySelector('.game-play__logo');
const resumeButton = document.querySelector('.game-pause-modal__resume');
const restartButton = document.querySelector('.game-pause-modal__restart');
const exitButton = document.querySelector('.game-pause-modal__exit');
const gameSection = document.querySelector('.game-play');
const gameInfoStartButton = document.querySelector('.game-info-modal__button');

// Open game info modal
function openGameInfoModal() {
    if (gameInfoModal) {
        gameInfoModal.classList.add('is-visible');
        gameInfoModal.style.display = 'block'; // Fallback
        gameInfoModal.style.opacity = '1'; // Fallback to override opacity: 0
        gameInfoModal.style.visibility = 'visible'; // Fallback to override visibility: hidden
        gameInfoModal.focus();
        console.log('Game info modal opened');

        // Log computed styles for debugging
        const computedStyles = window.getComputedStyle(gameInfoModal);
        console.log('Modal computed styles:', {
            display: computedStyles.display,
            zIndex: computedStyles.zIndex,
            opacity: computedStyles.opacity,
            visibility: computedStyles.visibility,
            position: computedStyles.position,
            top: computedStyles.top,
            left: computedStyles.left
        });

        // Log parent element styles
        const parent = gameInfoModal.parentElement;
        if (parent) {
            const parentStyles = window.getComputedStyle(parent);
            console.log('Parent element styles:', {
                display: parentStyles.display,
                zIndex: parentStyles.zIndex,
                overflow: parentStyles.overflow
            });
        }
    } else {
        console.error('Game info modal (#gameInfoModal) not found in DOM');
    }
}

// Close game info modal
function closeGameInfoModal() {
    if (gameInfoModal) {
        gameInfoModal.classList.remove('is-visible');
        gameInfoModal.style.display = 'none';
        gameInfoModal.style.opacity = '0'; // Reset to default
        gameInfoModal.style.visibility = 'hidden'; // Reset to default
        console.log('Game info modal closed');
    }
}

// Toggle sound (placeholder for future game sounds)
function toggleSound() {
    isSoundOn = !isSoundOn;
    if (soundToggle) {
        soundToggle.setAttribute('aria-pressed', isSoundOn);
        soundToggle.querySelector('.game-play__sound-icon').src = isSoundOn ? 'icon/sound-on.svg' : 'icon/sound-off.svg';
        soundToggle.setAttribute('data-sound', isSoundOn ? 'on' : 'off');
    } else {
        console.warn('Sound toggle button not found');
    }
    if (successSound) successSound.muted = !isSoundOn;
    if (errorSound) errorSound.muted = !isSoundOn;
}

// Toggle fullscreen
function toggleFullscreen() {
    if (!document.fullscreenElement) {
        if (gameSection) {
            gameSection.requestFullscreen().catch(err => {
                console.error(`Ошибка при попытке включить полноэкранный режим: ${err.message}`);
            });
            if (fullscreenButton) {
                fullscreenButton.setAttribute('aria-pressed', 'true');
                fullscreenButton.querySelector('.game-play__fullscreen-icon').src = 'icon/fullscreen-exit.svg';
            }
        } else {
            console.warn('Game section element (.game-play) not found for fullscreen');
        }
    } else {
        document.exitFullscreen();
        if (fullscreenButton) {
            fullscreenButton.setAttribute('aria-pressed', 'false');
            fullscreenButton.querySelector('.game-play__fullscreen-icon').src = 'icon/fullscreen.svg';
        }
    }
}

// Close modals with Escape key
function handleModalKeyboard(event) {
    if (event.key === 'Escape') {
        if (gameInfoModal && gameInfoModal.classList.contains('is-visible')) {
            closeGameInfoModal();
        }
        if (pauseModal && pauseModal.style.display === 'block') {
            pauseModal.style.display = 'none';
            if (pauseButton) pauseButton.setAttribute('aria-pressed', 'false');
            console.log('Pause modal closed via Escape');
        }
    }
}

// Header functionality
function initializeHeader() {
    // Logo navigation
    if (logoLink) {
        logoLink.addEventListener('click', (e) => {
            e.preventDefault();
            window.location.href = 'index.html';
        });
    } else {
        console.warn('Logo link element (.game-play__logo) not found');
    }

    // Title button - Show game info modal
    if (titleButton) {
        titleButton.addEventListener('click', openGameInfoModal);
    } else {
        console.error('Title button (.game-play__title-button) not found in DOM');
    }

    // Pause button - Show pause modal
    if (pauseButton && pauseModal) {
        pauseButton.addEventListener('click', () => {
            pauseModal.style.display = 'block';
            pauseButton.setAttribute('aria-pressed', 'true');
            pauseModal.focus();
            console.log('Pause modal opened');
        });
    } else {
        console.warn('Pause button (.game-play__pause) or pause modal (#pauseModal) not found');
    }

    // Sound toggle
    if (soundToggle) {
        soundToggle.addEventListener('click', toggleSound);
    } else {
        console.warn('Sound toggle button (.game-play__sound-toggle) not found');
    }

    // Fullscreen toggle
    if (fullscreenButton) {
        fullscreenButton.addEventListener('click', toggleFullscreen);
    } else {
        console.warn('Fullscreen button (.game-play__fullscreen) not found');
    }

    // Close button - Exit game
    if (closeButton) {
        closeButton.addEventListener('click', () => {
            window.location.href = 'index.html';
        });
    } else {
        console.warn('Close button (.game-play__close) not found');
    }
}

// Modal event listeners
function initializeModals() {
    // Close game info modal
    const gameInfoCloseButton = document.querySelector('.game-info-modal__close');
    if (gameInfoCloseButton) {
        gameInfoCloseButton.addEventListener('click', closeGameInfoModal);
    } else {
        console.warn('Game info close button (.game-info-modal__close) not found');
    }

    // Start game button in game info modal
    if (gameInfoStartButton) {
        // Remove inline onclick to prevent conflicts
        gameInfoStartButton.removeAttribute('onclick');
        gameInfoStartButton.addEventListener('click', closeGameInfoModal);
    } else {
        console.warn('Game info start button (.game-info-modal__button) not found');
    }

    // Close pause modal
    const pauseCloseButton = document.querySelector('.game-pause-modal__close');
    if (pauseCloseButton) {
        pauseCloseButton.addEventListener('click', () => {
            if (pauseModal) {
                pauseModal.style.display = 'none';
                if (pauseButton) pauseButton.setAttribute('aria-pressed', 'false');
                console.log('Pause modal closed via close button');
            }
        });
    } else {
        console.warn('Pause modal close button (.game-pause-modal__close) not found');
    }

    // Resume game (placeholder for future game integration)
    if (resumeButton) {
        resumeButton.addEventListener('click', () => {
            if (pauseModal) {
                pauseModal.style.display = 'none';
                if (pauseButton) pauseButton.setAttribute('aria-pressed', 'false');
                console.log('Game resumed from pause modal');
            }
        });
    } else {
        console.warn('Resume button (.game-pause-modal__resume) not found');
    }

    // Restart game (placeholder for future game integration)
    if (restartButton) {
        restartButton.addEventListener('click', () => {
            if (pauseModal) {
                pauseModal.style.display = 'none';
                if (pauseButton) pauseButton.setAttribute('aria-pressed', 'false');
                console.log('Game restart triggered (placeholder)');
                // Future game restart logic here
            }
        });
    } else {
        console.warn('Restart button (.game-pause-modal__restart) not found');
    }

    // Exit game
    if (exitButton) {
        exitButton.addEventListener('click', () => {
            window.location.href = 'index.html';
            console.log('Exited to index.html');
        });
    } else {
        console.warn('Exit button (.game-pause-modal__exit) not found');
    }
}

// Event listeners
document.addEventListener('DOMContentLoaded', () => {
    console.log('DOM fully loaded, initializing header and modals');
    // Initialize header functionality
    initializeHeader();

    // Initialize modal event listeners
    initializeModals();

    // Add keyboard support for modals
    document.addEventListener('keydown', handleModalKeyboard);
});