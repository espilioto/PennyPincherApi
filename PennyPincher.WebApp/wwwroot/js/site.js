// HTMX configuration
document.body.addEventListener('htmx:configRequest', function (evt) {
    evt.detail.headers['RequestVerificationToken'] =
        document.querySelector('input[name="__RequestVerificationToken"]')?.value;
});

// Searchable dropdown checkboxes
document.addEventListener('input', function (e) {
    if (!e.target.classList.contains('dropdown-search')) return;
    var query = e.target.value.toLowerCase();
    var items = e.target.closest('.dropdown-menu').querySelectorAll('.dropdown-item-check');
    items.forEach(function (item) {
        var label = item.querySelector('.form-check-label').textContent.toLowerCase();
        item.style.display = label.includes(query) ? '' : 'none';
    });
});
