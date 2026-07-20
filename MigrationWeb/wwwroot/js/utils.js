// Utility functions for the application

/**
 * Escapes HTML special characters to prevent XSS attacks
 * @param {string} unsafe - Unsafe string that may contain HTML special characters
 * @returns {string} - Escaped string safe for HTML insertion
 */
function escapeHtml(unsafe) {
    return unsafe
        .replace(/&/g, "&amp;")
        .replace(/</g, "&lt;")
        .replace(/>/g, "&gt;")
        .replace(/"/g, "&quot;")
        .replace(/'/g, "&#039;");
}

// Export for use in other modules (if needed in the future)
// For now, these functions are available globally
window.escapeHtml = escapeHtml;
