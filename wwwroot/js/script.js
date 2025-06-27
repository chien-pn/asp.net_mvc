const urlParams = new URLSearchParams(window.location.search);
const jwtToken = urlParams.get('token');
if (jwtToken) {
    localStorage.setItem("jwt_token", jwtToken);
    // Xóa token khỏi URL cho an toàn, đẹp URL
    window.history.replaceState({}, document.title, window.location.pathname);
}

const toggleButton = document.getElementById("menu-toggle");
const closeButton = document.getElementById("menu-close");
const sidebar = document.getElementById("sidebar");
const overlay = document.getElementById("overlay");

if (toggleButton) {
    toggleButton.addEventListener("click", () => {
        sidebar.classList.add("active");
        overlay.classList.add("active");
    });
}

if (closeButton) {
    closeButton.addEventListener("click", () => {
        sidebar.classList.remove("active");
        overlay.classList.remove("active");
    });
}

if (overlay) {
    overlay.addEventListener("click", () => {
        sidebar.classList.remove("active");
        overlay.classList.remove("active");
    });
}

function toggleReplies(btn) {
    // Tìm phần tử .replies gần nhất trong .comment-content
    var comment = btn.closest('.comment-content');
    if (!comment) return;
    var replies = comment.querySelector('.replies');
    if (replies) {
        replies.style.display = (replies.style.display === 'none' || replies.style.display === '') ? 'block' : 'none';
    }
}

// Hiển thị input để trả lời comment
function showReplyInput(replyBtn) {
    const commentContent = replyBtn.closest('.comment-content');
    const replyInput = commentContent.querySelector('.reply-input');

    if (replyInput) {
        replyInput.style.display = 'flex';
        replyInput.querySelector('input').focus();
    }
}

// Xử lý like comment
function toggleLike(likeBtn) {
    likeBtn.classList.toggle('liked');
    const likeText = likeBtn.textContent.trim();

    if (likeBtn.classList.contains('liked')) {
        likeBtn.textContent = 'Đã thích';
        likeBtn.style.color = '#1877f2';
    } else {
        likeBtn.textContent = 'Thích';
        likeBtn.style.color = '';
    }
}

// Xử lý gửi reply
function sendReply(sendBtn) {
    const replyInput = sendBtn.closest('.reply-input');
    const input = replyInput.querySelector('input');
    const replyText = input.value.trim();

    if (replyText) {
        // Tạo reply mới
        const repliesContainer = replyInput.closest('.comment-content').querySelector('.replies');
        const newReply = document.createElement('div');
        newReply.className = 'reply';
        newReply.innerHTML = `
            <div class="avatar reply-avatar"></div>
            <div class="comment-reply">
                <div class="comment-box">
                    <div class="author">Bạn</div>
                    <div class="text">${replyText}</div>
                </div>
                <div class="actions">Vừa xong · <span onclick="toggleLike(this)">Thích</span> · <span onclick="showReplyInput(this)">Trả lời</span></div>
            </div>
        `;

        repliesContainer.appendChild(newReply);
        repliesContainer.style.display = 'block';

        // Ẩn input và xóa nội dung
        replyInput.style.display = 'none';
        input.value = '';

        // Cập nhật text của replies-toggle
        const repliesToggle = replyInput.closest('.comment-content').querySelector('.replies-toggle');
        if (repliesToggle) {
            const currentText = repliesToggle.textContent;
            const match = currentText.match(/Xem tất cả (\d+) phản hồi/);
            if (match) {
                const currentCount = parseInt(match[1]);
                repliesToggle.textContent = `Xem tất cả ${currentCount + 1} phản hồi`;
            }
        }
    }
}

// Xử lý gửi comment chính
function sendComment(sendBtn) {
    const commentInputContainer = sendBtn.closest('.comment-input-container');
    const input = commentInputContainer.querySelector('input');
    const commentText = input.value.trim();

    if (commentText) {
        // Tạo comment mới
        const popupBody = commentInputContainer.closest('.popup-body');
        const newComment = document.createElement('div');
        newComment.className = 'comment';
        newComment.innerHTML = `
            <div class="avatar"></div>
            <div class="comment-content">
                <div class="comment-box">
                    <div class="author">Bạn</div>
                    <div class="text">${commentText}</div>
                </div>
                <div class="actions">Vừa xong · <span onclick="toggleLike(this)">Thích</span> · <span class="reply-btn" onclick="showReplyInput(this)">Trả lời</span></div>
                <div class="reply-input" style="display: none;">
                    <input type="text" placeholder="Viết phản hồi...">
                    <button onclick="sendReply(this)">Gửi</button>
                </div>
            </div>
        `;

        // Chèn comment mới trước comment input container
        popupBody.insertBefore(newComment, commentInputContainer);

        // Xóa nội dung input
        input.value = '';
    }
}

function formatTimeAgo(createdAtStr) {
    const createdAt = new Date(createdAtStr);
    const now = new Date();
    const diffMs = now - createdAt;
    const diffSec = Math.floor(diffMs / 1000);
    const diffMin = Math.floor(diffSec / 60);
    const diffHour = Math.floor(diffMin / 60);
    const diffDay = Math.floor(diffHour / 24);

    if (diffDay > 3) {
        // Hiển thị ngày/tháng/năm
        return createdAt.toLocaleDateString('vi-VN');
    } else if (diffDay >= 1) {
        return `${diffDay} ngày trước`;
    } else if (diffHour >= 1) {
        return `${diffHour} giờ trước`;
    } else if (diffMin >= 1) {
        return `${diffMin} phút trước`;
    } else {
        return `Vừa xong`;
    }
}

// Thêm event listeners khi DOM load xong
document.addEventListener('DOMContentLoaded', function () {
    // Thêm click handlers cho các nút like
    document.querySelectorAll('.like-btn').forEach(btn => {
        btn.addEventListener('click', function () {
            toggleLike(this);
        });
    });

    // Thêm click handlers cho nút gửi reply
    document.querySelectorAll('.reply-btn').forEach(btn => {
        btn.addEventListener('click', function () {
            showReplyInput(this);
        });
    });

    // Thêm click handlers cho nút gửi comment chính
    document.querySelectorAll('.comment-input-container').forEach(container => {
        const sendBtn = container.querySelector('button');
        if (sendBtn) {
            sendBtn.addEventListener('click', function () {
                sendComment(this);
            });
        }
    });

    // Thêm enter key handlers cho input fields
    document.querySelectorAll('.reply-input input').forEach(input => {
        input.addEventListener('keypress', function (e) {
            if (e.key === 'Enter') {
                const sendBtn = e.target.closest('.reply-input').querySelector('button');
                if (sendBtn) sendReply(sendBtn);
            }
        });
    });

    document.querySelectorAll('.actions[data-created-at]').forEach(el => {
        const createdAt = el.getAttribute('data-created-at');
        const timeText = el.querySelector('.time-text');
        if (createdAt && timeText) {
            timeText.textContent = formatTimeAgo(createdAt);
        }
    });

    document.querySelectorAll('input').forEach(input => {
        if (input) {
            input.addEventListener('blur', () => {
                if (!input.value.trim()) {
                    input.classList.add('is-invalid');
                } else {
                    input.classList.remove('is-invalid');
                }
            });
        }
    });
});

// Giả sử mỗi post có class .post-container
document.querySelectorAll('.post-container').forEach(postEl => {
    postEl.addEventListener('click', function () {
        // Lấy object model từ data-post
        const postData = JSON.parse(postEl.getAttribute('data-post'));

        // Bây giờ bạn có thể truy cập mọi trường: postData.Content, postData.Account.UserProfile.UserName, ...
        document.querySelector('#post-popup .popup-author').textContent = `Bài viết của ${postData.Account.UserProfile.UserName}`;
        document.querySelector('#post-popup .popup-post-content').textContent = postData.Content;
        document.querySelector('#post-popup .avatar-img').src = postData.ImageUrl || 'image/user.png';

        // Ẩn/hiện ảnh nếu có
        const popupImage = document.querySelector('#post-popup .popup-post-image');
        if (postData.ImageUrl) {
            popupImage.src = postData.ImageUrl;
            popupImage.style.display = 'block';
        } else {
            popupImage.style.display = 'none';
        }

        var apiUrl = "/api/Posts/GetComment?postId=" + encodeURIComponent(postData.Id);
        var token = localStorage.getItem("jwt_token");
        console.log("Token:", token);

        fetch(apiUrl, {
            method: "GET",
            headers: {
                "Content-Type": "application/json",
                "Authorization": token
            }
        })
            .then(response => response.json())
            .then(comments => {
                console.log("LOG CHECK:", comments);
                
            });

        // Hiện popup
        document.getElementById('post-popup').style.display = 'block';
    });
});

// Đóng popup
const closeBtn = document.getElementById('close-popup');
if (closeBtn) {
    closeBtn.addEventListener('click', function () {
        document.getElementById('post-popup').style.display = 'none';
    });
} else {
    console.warn("Không tìm thấy nút đóng popup với id 'close-popup'");
}

function renderComments(comments) {
    let html = '';
    comments.forEach(comment => {
        html += `
            <div class="comment">
                <img src="${comment.account.userProfile.avatarUrl || 'image/user.png'}" class="avatar-img" alt="Avatar">
                <div class="comment-content">
                    <div class="comment-box">
                        <div class="author">${comment.account.userProfile.userName}</div>
                        <div class="text">${comment.content}</div>
                    </div>
                    <div class="actions">${formatTimeAgo(comment.createdAt)}</div>
                    ${comment.replies && comment.replies.length > 0
                ? `<div class="replies">${renderComments(comment.replies)}</div>`
                : ''}
                </div>
            </div>
        `;
    });
    return html;
}

function getCookie(name) {
    const value = `; ${document.cookie}`;
    const parts = value.split(`; ${name}=`);
    if (parts.length === 2) return parts.pop().split(';').shift();
    return null;
}