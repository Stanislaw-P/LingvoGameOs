// nav.js — логика для выпадающего меню навигации администратора
// Обеспечивает доступность (aria), управление с клавиатуры и мышью

document.addEventListener('DOMContentLoaded', function() {
  const dropdown = document.querySelector('.admin-navbar__dropdown');
  const toggle = dropdown.querySelector('.admin-navbar__dropdown-toggle');
  const list = dropdown.querySelector('.admin-navbar__dropdown-list');

  function openMenu() {
    list.setAttribute('aria-expanded', 'true');
    toggle.setAttribute('aria-expanded', 'true');
    list.style.display = 'block';
    toggle.focus();
  }
  function closeMenu() {
    list.setAttribute('aria-expanded', 'false');
    toggle.setAttribute('aria-expanded', 'false');
    list.style.display = 'none';
  }
  // Toggle on click
  toggle.addEventListener('click', function(e) {
    e.stopPropagation();
    const expanded = toggle.getAttribute('aria-expanded') === 'true';
    if (expanded) closeMenu();
    else openMenu();
  });
  // Keyboard navigation
  toggle.addEventListener('keydown', function(e) {
    if (e.key === 'ArrowDown' || e.key === 'Enter' || e.key === ' ') {
      e.preventDefault();
      openMenu();
      const firstLink = list.querySelector('a');
      if (firstLink) firstLink.focus();
    }
    if (e.key === 'Escape') {
      closeMenu();
      toggle.focus();
    }
  });
  list.addEventListener('keydown', function(e) {
    const links = Array.from(list.querySelectorAll('a'));
    const idx = links.indexOf(document.activeElement);
    if (e.key === 'ArrowDown') {
      e.preventDefault();
      if (idx < links.length - 1) links[idx + 1].focus();
      else links[0].focus();
    }
    if (e.key === 'ArrowUp') {
      e.preventDefault();
      if (idx > 0) links[idx - 1].focus();
      else links[links.length - 1].focus();
    }
    if (e.key === 'Escape') {
      closeMenu();
      toggle.focus();
    }
  });
  // Close on outside click
  document.addEventListener('click', function(e) {
    if (!dropdown.contains(e.target)) closeMenu();
  });
  // Close on blur (optional, for accessibility)
  list.addEventListener('focusout', function(e) {
    setTimeout(() => {
      if (!dropdown.contains(document.activeElement)) closeMenu();
    }, 10);
  });
  // Init
  closeMenu();
}); 