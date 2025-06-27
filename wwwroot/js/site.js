// Please see documentation at https://learn.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.
document.addEventListener('DOMContentLoaded', function () {
    const popup = document.getElementById('post-popup');
    if (!popup) return;

    const closeButton = document.getElementById('close-popup');
    const popupAuthor = popup.querySelector('.popup-author');
    const popupContent = popup.querySelector('.popup-post-content');
    const popupImage = popup.querySelector('.popup-post-image');

    document.body.addEventListener('click', function(event) {
        const commentButton = event.target.closest('.comment-btn');
        if (commentButton) {
            const postContainer = event.target.closest('.post-container');
            if (postContainer) {
                event.preventDefault();

                const author = postContainer.dataset.author || 'N/A';
                const content = postContainer.dataset.content || '';
                const imageUrl = postContainer.dataset.imageUrl;

                popupAuthor.textContent = `Bài viết của ${author}`;
                popupContent.textContent = content;

                if (imageUrl) {
                    popupImage.src = imageUrl;
                    popupImage.style.display = 'block';
                } else {
                    popupImage.style.display = 'none';
                }

                popup.style.display = 'flex';
            }
        }
    });

    if (closeButton) {
        closeButton.addEventListener('click', function () {
            popup.style.display = 'none';
        });
    }

    popup.addEventListener('click', function (event) {
        if (event.target === popup) {
            popup.style.display = 'none';
        }
    });
});
