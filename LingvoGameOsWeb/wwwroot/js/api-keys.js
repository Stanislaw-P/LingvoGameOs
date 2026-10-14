(() => {
    const button = document.querySelector('[data-copy-api-key]');
    const key = document.getElementById('created-api-key');
    const status = document.querySelector('[data-copy-status]');

    if (!button || !key || !status) {
        return;
    }

    button.addEventListener('click', async () => {
        try {
            await navigator.clipboard.writeText(key.textContent.trim());
            status.textContent = 'Ключ скопирован.';
            button.textContent = 'Скопировано';
        } catch {
            status.textContent = 'Не удалось скопировать автоматически. Выдели ключ вручную.';
        }
    });
})();
