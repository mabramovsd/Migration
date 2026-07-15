// Dashboard JavaScript
document.addEventListener('DOMContentLoaded', function() {
    loadDashboardData();
});

async function loadDashboardData() {
    const loadingDiv = document.getElementById('loading');
    const errorDiv = document.getElementById('error');
    const dashboardDiv = document.getElementById('dashboard');

    try {
        // Make API call to get company counts
        const response = await fetch('/HR/Stats/CompanyCounts');
        
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
        
        // Render company cards
        const companyCards = data.map(item => `
            <div class="company-card" onclick="handleCompanyClick('${item.companyName}')">
                <div class="company-name">${escapeHtml(item.companyName)}</div>
                <div class="company-count">${item.count}</div>
            </div>
        `).join('');
        
        dashboardDiv.innerHTML = companyCards;
        
    } catch (error) {
        loadingDiv.style.display = 'none';
        errorDiv.style.display = 'block';
        errorDiv.textContent = `Ошибка: ${error.message}`;
        console.error('Dashboard error:', error);
    }
}

// Handle company card click
async function handleCompanyClick(companyName) {
    const loadingDiv = document.getElementById('loading');
    const errorDiv = document.getElementById('error');
    const dashboardDiv = document.getElementById('dashboard');

    // For "All" and "unknown" companies, do nothing
    if (!companyName || companyName.toLowerCase() === 'all' || companyName.toLowerCase() === 'unknown') {
        return;
    }

    try {
        loadingDiv.style.display = 'block';
        loadingDiv.textContent = `Загрузка статистики по профессиям для ${escapeHtml(companyName)}...`;
        errorDiv.style.display = 'none';

        // Fetch profession counts for the selected company
        const response = await fetch(`/HR/Stats/ProfessionCounts/${encodeURIComponent(companyName)}`);
        
        if (!response.ok) {
            throw new Error(`Ошибка при загрузке статистики: ${response.status} ${response.statusText}`);
        }
        
        const professionData = await response.json();
        
        if (!professionData || professionData.length === 0) {
            throw new Error('Нет данных по профессиям для этой компании');
        }

        // Hide loading and show profession statistics
        loadingDiv.style.display = 'none';
        dashboardDiv.style.display = 'grid';
        
        // Render profession cards
        const professionCards = professionData.map(item => `
            <div class="company-card">
                <div class="company-name">${escapeHtml(item.professionTitle)}</div>
                <div class="company-count">${item.count}</div>
            </div>
        `).join('');
        
        dashboardDiv.innerHTML = professionCards;
        
    } catch (error) {
        loadingDiv.style.display = 'none';
        errorDiv.style.display = 'block';
        errorDiv.textContent = `Ошибка: ${error.message}`;
        console.error('Profession statistics error:', error);
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
