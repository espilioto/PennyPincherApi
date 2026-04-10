// HTMX configuration
document.body.addEventListener('htmx:configRequest', function (evt) {
    evt.detail.headers['RequestVerificationToken'] =
        document.querySelector('input[name="__RequestVerificationToken"]')?.value;
});
