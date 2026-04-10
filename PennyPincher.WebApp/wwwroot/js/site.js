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
