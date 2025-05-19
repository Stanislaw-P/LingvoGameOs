document.getElementById('registration-form').addEventListener('submit', function(e) {
    e.preventDefault();
    const email = document.getElementById('email');
    const password = document.getElementById('password');
    const emailError = document.getElementById('email-error');
    const passwordError = document.getElementById('password-error');
    let isValid = true;

    // Reset error states
    emailError.classList.remove('registration__error--visible');
    passwordError.classList.remove('registration__error--visible');
    email.classList.remove('registration__input--error');
    password.classList.remove('registration__input--error');

    // Email validation
    if (!email.validity.valid) {
        emailError.classList.add('registration__error--visible');
        email.classList.add('registration__input--error');
        isValid = false;
    }

    // Password validation
    if (!password.validity.valid) {
        passwordError.classList.add('registration__error--visible');
        password.classList.add('registration__input--error');
        isValid = false;
    }

    if (isValid) {
        // Simulate form submission (e.g., API call)
        alert('Регистрация успешна!');
        this.reset();
    }
});

// Real-time validation
['email', 'password'].forEach(id => {
    const input = document.getElementById(id);
    input.addEventListener('input', () => {
        const error = document.getElementById(`${id}-error`);
        if (input.validity.valid) {
            error.classList.remove('registration__error--visible');
            input.classList.remove('registration__input--error');
        }
    });
});