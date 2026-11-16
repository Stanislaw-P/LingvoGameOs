// Глобальные тултипы для элементов с атрибутом data-tooltip.
// Рендерятся в body с position: fixed, поэтому не обрезаются контейнерами
// с overflow: hidden (например, карточками игр).

function initTooltips() {
    // На тач-устройствах тултипы не нужны (как и в CSS)
    if (window.matchMedia && window.matchMedia('(hover: none)').matches) {
        return;
    }

    const VISIBLE = 'global-tooltip--visible';
    const ABOVE = 'global-tooltip--above';
    const GAP = 8;      // расстояние между элементом и тултипом
    const EDGE = 12;    // отступ от края окна

    let tooltip = null;
    let current = null;

    function ensureTooltip() {
        if (tooltip) return tooltip;
        tooltip = document.createElement('div');
        tooltip.className = 'global-tooltip';
        tooltip.setAttribute('role', 'tooltip');
        tooltip.setAttribute('aria-hidden', 'true');
        document.body.appendChild(tooltip);
        return tooltip;
    }

    function place(el) {
        const tip = ensureTooltip();
        const rect = el.getBoundingClientRect();
        const tipWidth = tip.offsetWidth;
        const tipHeight = tip.offsetHeight;
        const viewportWidth = document.documentElement.clientWidth || window.innerWidth;
        const viewportHeight = document.documentElement.clientHeight || window.innerHeight;

        // по центру элемента, но не вылезая за края окна
        let left = rect.left + rect.width / 2 - tipWidth / 2;
        left = Math.max(EDGE, Math.min(left, viewportWidth - tipWidth - EDGE));

        // по умолчанию снизу, если не помещается — сверху
        let above = false;
        let top = rect.bottom + GAP;
        if (top + tipHeight > viewportHeight - EDGE) {
            const altTop = rect.top - tipHeight - GAP;
            if (altTop >= EDGE) {
                above = true;
                top = altTop;
            } else {
                top = Math.max(EDGE, viewportHeight - tipHeight - EDGE);
            }
        }

        // позиция стрелки относительно тултипа
        const arrowX = Math.max(10, Math.min(rect.left + rect.width / 2 - left, tipWidth - 10));

        tip.style.left = Math.round(left) + 'px';
        tip.style.top = Math.round(top) + 'px';
        tip.style.setProperty('--arrow-left', Math.round(arrowX) + 'px');
        tip.classList.toggle(ABOVE, above);
    }

    function show(el) {
        const text = el.getAttribute('data-tooltip');
        if (!text) return;
        if (current === el) {
            place(el);
            return;
        }
        current = el;
        const tip = ensureTooltip();
        tip.textContent = text;
        tip.setAttribute('aria-hidden', 'false');
        tip.classList.add(VISIBLE);
        place(el);
    }

    function hide(el) {
        if (el && el !== current) return;
        current = null;
        if (!tooltip) return;
        tooltip.classList.remove(VISIBLE);
        tooltip.setAttribute('aria-hidden', 'true');
    }

    function tooltipTarget(node) {
        return node && node.closest ? node.closest('[data-tooltip]') : null;
    }

    // Делегирование: работает и для динамически добавленных элементов
    document.addEventListener('pointerover', function (e) {
        const el = tooltipTarget(e.target);
        if (!el) return;
        if (e.relatedTarget && (el === e.relatedTarget || el.contains(e.relatedTarget))) return;
        show(el);
    }, true);

    document.addEventListener('pointerout', function (e) {
        const el = tooltipTarget(e.target);
        if (!el) return;
        if (e.relatedTarget && (el === e.relatedTarget || el.contains(e.relatedTarget))) return;
        hide(el);
    }, true);

    // Поддержка клавиатуры
    document.addEventListener('focusin', function (e) {
        const el = tooltipTarget(e.target);
        if (el) show(el);
    });

    document.addEventListener('focusout', function (e) {
        const el = tooltipTarget(e.target);
        if (el) hide(el);
    });

    document.addEventListener('keydown', function (e) {
        if (e.key === 'Escape') hide();
    });

    // При прокрутке/изменении размера fixed-тултип отсоединился бы от элемента
    window.addEventListener('scroll', function () { hide(); }, true);
    window.addEventListener('resize', function () { hide(); });

    // Отключаем CSS-тултипы, чтобы не было дублей
    document.documentElement.classList.add('has-js-tooltip');
}

if (document.readyState === 'loading') {
    document.addEventListener('DOMContentLoaded', initTooltips);
} else {
    initTooltips();
}
