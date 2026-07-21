// Dashboard JavaScript
document.addEventListener('DOMContentLoaded', function() {
    handleIndexClick();
});

// Helper function to map HR filter results to table HTML
function renderEmployeesTable(employeesData, title) {
    if (!employeesData || employeesData.length === 0) {
        return '<p style="margin-top: 2rem; color: #666;">Нет данных о сотрудниках</p>';
    }
    
    return `
        <div style="margin-top: 2rem;">
            <div style="color: #667eea; font-size: 1.2rem; font-weight: 600;">${title}</div>
            <table class="employees-table" style="margin-top: 0.5rem;">
                <thead>
                    <tr>
                        <th>Имя</th>
                        <th>Дата рождения</th>
                        <th>Компания</th>
                    </tr>
                </thead>
                <tbody>
                    ${employeesData.map(item => `
                        <tr>
                            <td>${escapeHtml(item.fullName)}</td>
                            <td>${item.birthDate ? new Date(item.birthDate).toLocaleDateString('ru-RU') : 'Не указано'}</td>
                            <td>${escapeHtml(item.currentCompany)}</td>
                        </tr>
                    `).join('')}
                </tbody>
            </table>
        </div>
    `;
}

// Handle index/dashboard click - loads initial company statistics
async function handleIndexClick() {
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
            <div class="company-card" onclick="handleCompanyClick('${escapeHtml(item.companyName)}')">
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

// Handle company card click - loads profession statistics for selected company
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
        const responseProfessions = await fetch(`/HR/Stats/ProfessionCounts/${encodeURIComponent(companyName)}`);
        
        if (!responseProfessions.ok) {
            throw new Error(`Ошибка при загрузке статистики: ${responseProfessions.status} ${responseProfessions.statusText}`);
        }
        
        const professionData = await responseProfessions.json();
        
        if (!professionData || professionData.length === 0) {
            throw new Error('Нет данных по профессиям для этой компании');
        }

        // Hide loading and show profession statistics
        loadingDiv.style.display = 'none';
        dashboardDiv.style.display = 'block';
        
        // Render profession cards in grid container
        const professionCards = professionData.map(item => `
            <div class="company-card" onclick="handleProfessionClick('${escapeHtml(companyName)}', '${escapeHtml(item.professionTitle)}')">
                <div class="company-name">${escapeHtml(item.professionTitle)}</div>
                <div class="company-count">${item.count}</div>
            </div>
        `).join('');
        
        dashboardDiv.innerHTML =
            '<div class="dashboard-grid" display="grid">' +
                professionCards +
            '</div>';

        // Fetch employees list for the selected company
        const responseEmployees = await fetch(`/HR/Filter?Company=${encodeURIComponent(companyName)}`);
        
        if (!responseEmployees.ok) {
            throw new Error(`Ошибка при загрузке данных сотрудников: ${responseEmployees.status} ${responseEmployees.statusText}`);
        }
        const employeesData = await responseEmployees.json();
        
        // Use the helper function to render employees table
        dashboardDiv.innerHTML += renderEmployeesTable(employeesData, `Сотрудники компании ${escapeHtml(companyName)}`);

    } catch (error) {
        loadingDiv.style.display = 'none';
        errorDiv.style.display = 'block';
        errorDiv.textContent = `Ошибка: ${error.message}`;
        console.error('Profession statistics error:', error);
    }
}

// Handle profession click - loads employees for specific company and profession
async function handleProfessionClick(companyName, professionName) {
    const loadingDiv = document.getElementById('loading');
    const errorDiv = document.getElementById('error');
    const dashboardDiv = document.getElementById('dashboard');

    // For "All" and "unknown" companies, do nothing
    if (!companyName || companyName.toLowerCase() === 'all' || companyName.toLowerCase() === 'unknown') {
        return;
    }

    try {
        loadingDiv.style.display = 'block';
        loadingDiv.textContent = `Загрузка сотрудников профессии ${escapeHtml(professionName)} в компании ${escapeHtml(companyName)}...`;
        errorDiv.style.display = 'none';

        // Fetch employees list for the selected company and profession
        const responseEmployees = await fetch(`/HR/Filter?Company=${encodeURIComponent(companyName)}&Profession=${encodeURIComponent(professionName)}`);
        
        if (!responseEmployees.ok) {
            throw new Error(`Ошибка при загрузке данных сотрудников: ${responseEmployees.status} ${responseEmployees.statusText}`);
        }
        const employeesData = await responseEmployees.json();
        
        // Hide loading and show only the employees table
        loadingDiv.style.display = 'none';
        dashboardDiv.style.display = 'block';
        
        // Use the helper function to render employees table (only table, no buttons)
        dashboardDiv.innerHTML = renderEmployeesTable(employeesData, `Сотрудники профессии ${escapeHtml(professionName)} в компании ${escapeHtml(companyName)}`);

    } catch (error) {
        loadingDiv.style.display = 'none';
        errorDiv.style.display = 'block';
        errorDiv.textContent = `Ошибка: ${error.message}`;
        console.error('Profession click error:', error);
    }
}
