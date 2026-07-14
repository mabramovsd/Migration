// Dashboard JavaScript
document.addEventListener('DOMContentLoaded', function() {
    loadDashboardData();
});

async function loadDashboardData() {
    const loadingDiv = document.getElementById('loading');
    const errorDiv = document.getElementById('error');
    const dashboardDiv = document.getElementById('dashboard');

    try {
        // Make API call to get category counts
        const response = await fetch('/HR/Stats/CategoryCounts');
        
        if (!response.ok) {
            throw new Error(`Ошибка при загрузке данных: ${response.status} ${response.statusText}`);
        }
        
        const data = await response.json();
        
        if (!data || data.length === 0) {
            throw new Error('Нет данных для отображения');
        }

        // Hide loading and show dashboard
        loadingDiv.style.display = 'none';
        dashboardDiv.style.display = 'grid';
        
        // Render category cards
        const categoryCards = data.map(item => `
            <div class="category-card">
                <div class="category-name">${escapeHtml(item.categoryName)}</div>
                <div class="category-count">${item.count}</div>
            </div>
        `).join('');
        
        dashboardDiv.innerHTML = categoryCards;
        
    } catch (error) {
        loadingDiv.style.display = 'none';
        errorDiv.style.display = 'block';
        errorDiv.textContent = `Ошибка: ${error.message}`;
        console.error('Dashboard error:', error);
    }
}

// Helper function to escape HTML to prevent XSS
function escapeHtml(unsafe) {
    return unsafe
        .replace(/&/g, "&amp;")
        .replace(/</g, "&lt;")
        .replace(/>/g, "&gt;")
        .replace(/"/g, "&quot;")
        .replace(/'/g, "&#039;");
}
