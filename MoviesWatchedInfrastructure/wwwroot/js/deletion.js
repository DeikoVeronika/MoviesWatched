document.getElementById('deleteForm').addEventListener('submit', function (event) {
    event.preventDefault();

    const form = this;
    const formData = new FormData(form);

    fetch(form.action, {
        method: 'POST',
        body: formData,
        headers: {
            'RequestVerificationToken': document.querySelector('input[name="__RequestVerificationToken"]').value
        }
    })
        .then(response => response.json())
        .then(data => {
            if (data.success) {
                Swal.fire({
                    icon: 'success',
                    title: 'Успіх!',
                    text: data.message, // Використання повідомлення з бекенду
                    confirmButtonText: 'ОК'
                }).then(() => {
                    window.location.href = form.getAttribute('data-return-url'); // Переходимо на сторінку списку
                });
            } else {
                Swal.fire({
                    icon: 'error',
                    title: 'Помилка!',
                    text: data.message, // Використання повідомлення з бекенду
                    confirmButtonText: 'ОК'
                });
            }
        })
        .catch(error => {
            Swal.fire({
                icon: 'error',
                title: 'Сталася помилка!',
                text: 'Неможливо видалити запис.',
                confirmButtonText: 'ОК'
            });
        });
});
