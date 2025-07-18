// Please see documentation at https://learn.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.

/* Hur de olika knapparna på webbsidan ska bete sig och göra */
document.addEventListener('DOMContentLoaded', function () {
    const deleteButton = document.getElementById('deleteButton');
    const confirmationModal = document.getElementById('confirmationModal');
    const confirmDeleteButton = document.getElementById('confirmDelete');
    const cancelDeleteButton = document.getElementById('cancelDelete');
    const deleteForm = document.getElementById('deleteForm');

    deleteButton.addEventListener('click', function () {
        confirmationModal.classList.remove('hidden'); // Visar modalen
    });

    cancelDeleteButton.addEventListener('click', function () {
        confirmationModal.classList.add('hidden'); // Gömmer modalen
    });

    confirmDeleteButton.addEventListener('click', function () {
        confirmationModal.classList.add('hidden'); // Gömmer modalen
        deleteForm.submit(); // Skickar formuläret
    });

    // Optional: Close modal if clicking outside of it
    confirmationModal.addEventListener('click', function (event) {
        if (event.target === confirmationModal) {
            confirmationModal.classList.add('hidden');
        }
    });
 
});
