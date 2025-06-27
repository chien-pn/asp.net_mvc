// Common utility functions for the application

/**
 * Tạo loading state cho button
 * @param {HTMLButtonElement} button - Button element
 * @param {string} loadingText - Text hiển thị khi loading
 * @returns {Function} Function để restore button về trạng thái ban đầu
 */
function setButtonLoading(button, loadingText = 'Đang xử lý...') {
    const originalText = button.textContent;
    const originalDisabled = button.disabled;
    
    button.textContent = loadingText;
    button.disabled = true;
    
    // Return function để restore
    return function() {
        button.textContent = originalText;
        button.disabled = originalDisabled;
    };
}

/**
 * Hiển thị thông báo
 * @param {string} message - Nội dung thông báo
 * @param {string} type - Loại thông báo: 'success', 'error', 'warning', 'info'
 * @param {HTMLElement} container - Container để hiển thị thông báo
 * @param {number} duration - Thời gian hiển thị (ms), 0 = không tự ẩn
 */
function showMessage(message, type = 'info', container = null, duration = 0) {
    const messageDiv = document.createElement('div');
    messageDiv.className = `alert alert-${type === 'error' ? 'danger' : type} alert-dismissible fade show`;
    messageDiv.innerHTML = `
        ${message}
        <button type="button" class="btn-close" data-bs-dismiss="alert"></button>
    `;
    
    if (container) {
        container.appendChild(messageDiv);
    } else {
        // Tạo toast container nếu chưa có
        let toastContainer = document.getElementById('toast-container');
        if (!toastContainer) {
            toastContainer = document.createElement('div');
            toastContainer.id = 'toast-container';
            toastContainer.className = 'position-fixed top-0 end-0 p-3';
            toastContainer.style.zIndex = '9999';
            document.body.appendChild(toastContainer);
        }
        toastContainer.appendChild(messageDiv);
    }
    
    // Tự động ẩn sau thời gian duration
    if (duration > 0) {
        setTimeout(() => {
            messageDiv.remove();
        }, duration);
    }
}

/**
 * Xử lý API request với loading state
 * @param {string} url - API endpoint
 * @param {Object} options - Fetch options
 * @param {HTMLButtonElement} button - Button để set loading
 * @param {string} loadingText - Text loading
 * @returns {Promise} Promise với response data
 */
async function apiRequest(url, options = {}, button = null, loadingText = 'Đang xử lý...') {
    let restoreButton = null;
    
    if (button) {
        restoreButton = setButtonLoading(button, loadingText);
    }
    
    try {
        const response = await fetch(url, options);
        const data = await response.json();
        
        return { response, data };
    } catch (error) {
        throw error;
    } finally {
        if (restoreButton) {
            restoreButton();
        }
    }
}

/**
 * Lưu token và user info vào localStorage
 * @param {string} token - JWT token
 * @param {Object} user - User information
 */
function saveAuthData(token, user) {
    localStorage.setItem('token', token);
    localStorage.setItem('user', JSON.stringify(user));
}

/**
 * Lấy token từ localStorage
 * @returns {string|null} Token hoặc null
 */
function getToken() {
    return localStorage.getItem('token');
}

/**
 * Lấy user info từ localStorage
 * @returns {Object|null} User object hoặc null
 */
function getUser() {
    const userStr = localStorage.getItem('user');
    return userStr ? JSON.parse(userStr) : null;
}

/**
 * Xóa auth data khỏi localStorage
 */
function clearAuthData() {
    localStorage.removeItem('token');
    localStorage.removeItem('user');
}

/**
 * Kiểm tra user đã đăng nhập chưa
 * @returns {boolean}
 */
function isAuthenticated() {
    return getToken() !== null;
}

/**
 * Tạo headers với authorization
 * @param {Object} additionalHeaders - Headers bổ sung
 * @returns {Object} Headers object
 */
function getAuthHeaders(additionalHeaders = {}) {
    const token = getToken();
    return {
        'Content-Type': 'application/json',
        ...(token && { 'Authorization': `Bearer ${token}` }),
        ...additionalHeaders
    };
}

// Menu toggle functionality
document.addEventListener('DOMContentLoaded', function() {
    const menuToggle = document.getElementById('menu-toggle');
    const menuClose = document.getElementById('menu-close');
    const sidebar = document.getElementById('sidebar');
    const overlay = document.getElementById('overlay');

    // Toggle sidebar
    if (menuToggle) {
        menuToggle.addEventListener('click', function() {
            sidebar.classList.add('active');
            overlay.classList.add('active');
        });
    }

    // Close sidebar
    if (menuClose) {
        menuClose.addEventListener('click', function() {
            sidebar.classList.remove('active');
            overlay.classList.remove('active');
        });
    }

    // Close sidebar when clicking overlay
    if (overlay) {
        overlay.addEventListener('click', function() {
            sidebar.classList.remove('active');
            overlay.classList.remove('active');
        });
    }

    // Highlight current page in menu
    const currentPath = window.location.pathname;
    const menuLinks = document.querySelectorAll('.menu-link');
    
    menuLinks.forEach(link => {
        if (link.getAttribute('href') === currentPath) {
            link.classList.add('active');
        }
    });
}); 