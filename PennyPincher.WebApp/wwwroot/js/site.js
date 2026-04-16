// === APP-WIDE THEME SYSTEM ===
(function(){
    var themes=[
        {name:'amber',accent:'#e8b44c',rgb:'232,180,76',dim:'#3a2e18',text:'#0c0c10'},
        {name:'violet',accent:'#a78bfa',rgb:'167,139,250',dim:'#2a2240',text:'#e8e8ed'},
        {name:'rose',accent:'#e07a8e',rgb:'224,122,142',dim:'#3a1e28',text:'#e8e8ed'},
        {name:'silver',accent:'#b0b0c0',rgb:'176,176,192',dim:'#22222e',text:'#0c0c10'},
        {name:'mint',accent:'#4ade80',rgb:'74,222,128',dim:'#1a2e20',text:'#0c0c10'},
    ];

    function applyTheme(t){
        var s=document.documentElement.style;
        s.setProperty('--accent',t.accent);
        s.setProperty('--accent-rgb',t.rgb);
        s.setProperty('--accent-dim',t.dim);
        s.setProperty('--accent-text',t.text);
        localStorage.setItem('pp-theme',t.name);
        window.dispatchEvent(new Event('themeChanged'));
    }

    // Apply saved or random theme (random is saved for consistency)
    var saved=localStorage.getItem('pp-theme');
    var theme=themes.find(function(t){return t.name===saved})||themes[Math.floor(Math.random()*themes.length)];
    applyTheme(theme);

    // Expose for settings page
    window.ppThemes=themes;
    window.applyTheme=applyTheme;

    // Global helper for charts
    window.getAccentColor=function(){
        return getComputedStyle(document.documentElement).getPropertyValue('--accent').trim()||'#a78bfa';
    };
})();

// Convert UTC timestamps to local time
function convertUtcDates(root) {
    root.querySelectorAll('[data-utc]').forEach(function (el) {
        var d = new Date(el.dataset.utc);
        el.textContent = d.getFullYear() + '-' +
            String(d.getMonth() + 1).padStart(2, '0') + '-' +
            String(d.getDate()).padStart(2, '0') + ' ' +
            String(d.getHours()).padStart(2, '0') + ':' +
            String(d.getMinutes()).padStart(2, '0');
    });
}
convertUtcDates(document);
document.body.addEventListener('htmx:afterSettle', function (evt) {
    convertUtcDates(evt.detail.target);
});

// HTMX configuration
document.body.addEventListener('htmx:configRequest', function (evt) {
    evt.detail.headers['RequestVerificationToken'] =
        document.querySelector('input[name="__RequestVerificationToken"]')?.value;
});

// Searchable dropdown checkboxes — filter items as you type
document.addEventListener('input', function (e) {
    if (!e.target.classList.contains('dropdown-search')) return;
    var query = e.target.value.toLowerCase();
    var items = e.target.closest('.dropdown-menu').querySelectorAll('.dropdown-item-check');
    items.forEach(function (item) {
        var label = item.querySelector('.form-check-label').textContent.toLowerCase();
        item.style.display = label.includes(query) ? '' : 'none';
    });
});

// Clear search text button
document.addEventListener('mousedown', function (e) {
    var btn = e.target.closest('.dropdown-clear');
    if (!btn) return;
    e.preventDefault();
    e.stopPropagation();
    var menu = btn.closest('.dropdown-menu');
    var search = menu.querySelector('.dropdown-search');
    search.value = '';
    // Show all items again
    menu.querySelectorAll('.dropdown-item-check').forEach(function (item) {
        item.style.display = '';
    });
});

// Deselect all checkboxes in a dropdown
document.addEventListener('mousedown', function (e) {
    var btn = e.target.closest('.dropdown-deselect-all');
    if (!btn) return;
    e.preventDefault();
    e.stopPropagation();
    var menu = btn.closest('.dropdown-menu');
    menu.querySelectorAll('.form-check-input[type="checkbox"]').forEach(function (cb) {
        cb.checked = false;
    });
    // Trigger change on one checkbox to update button text + reorder
    var first = menu.querySelector('.form-check-input[type="checkbox"]');
    if (first) first.dispatchEvent(new Event('change', { bubbles: true }));
});

// When a checkbox changes: update button text + move checked to top
document.addEventListener('change', function (e) {
    if (!e.target.classList.contains('form-check-input')) return;
    var dropdown = e.target.closest('.dropdown');
    if (!dropdown) return;

    var menu = dropdown.querySelector('.dropdown-menu');
    var button = dropdown.querySelector('.dropdown-toggle');
    var checkboxes = menu.querySelectorAll('.form-check-input[type="checkbox"]');
    var items = menu.querySelectorAll('.dropdown-item-check');

    // Update button text
    var checked = Array.from(checkboxes).filter(function (cb) { return cb.checked; });
    var label = button.dataset.label || '';
    if (!label) {
        // Detect label from the current text ("All accounts" -> "accounts", "All categories" -> "categories")
        var current = button.textContent.trim();
        var match = current.match(/(?:All |^\d+ )(.+)$/);
        label = match ? match[1] : 'selected';
        button.dataset.label = label;
    }
    button.textContent = checked.length > 0 ? checked.length + ' ' + label : 'All ' + label;

    // Trigger HTMX table refresh for filter checkboxes
    var form = document.getElementById('filter-form');
    if (form && form.contains(dropdown)) {
        htmx.trigger(form, 'filterChanged');
    }

    // Stamp original order on first interaction
    items.forEach(function (item, i) {
        if (!item.dataset.order) item.dataset.order = i;
    });

    // Move checked items to top, unchecked back to original order
    var sorted = Array.from(items).sort(function (a, b) {
        var aChecked = a.querySelector('input').checked ? 0 : 1;
        var bChecked = b.querySelector('input').checked ? 0 : 1;
        if (aChecked !== bChecked) return aChecked - bChecked;
        return parseInt(a.dataset.order) - parseInt(b.dataset.order);
    });
    sorted.forEach(function (item) {
        menu.appendChild(item);
    });
});
