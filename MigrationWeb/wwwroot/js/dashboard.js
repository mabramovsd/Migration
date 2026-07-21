// Dashboard JavaScript
document.addEventListener('DOMContentLoaded', function() {
    handleIndexClick();
});

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
        
        // Render profession cards
        const professionCards = professionData.map(item => `
            <div class="company-card">
                <div class="company-name">${escapeHtml(item.professionTitle)}</div>
                <div class="company-count">${item.count}</div>
            </div>
        `).join('');
        
        dashboardDiv.innerHTML = professionCards;

        // Fetch employees list for the selected company
        const responseEmployees = await fetch(`/HR/Filter?Company=${encodeURIComponent(companyName)}`);
        
        if (!responseEmployees.ok) {
            throw new Error(`Ошибка при загрузке данных сотрудников: ${responseEmployees.status} ${responseEmployees.statusText}`);
        }
        const employeesData = await responseEmployees.json();
        
        if (employeesData && employeesData.length > 0) {
            // Render employees as a table
            const employeesTable = `
                <h3 style="margin-top: 2rem; color: #667eea; font-size: 1.5rem;">Сотрудники компании ${escapeHtml(companyName)}</h3>
                <table class="employees-table">
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
            `;
            
            dashboardDiv.innerHTML += '<br/>' + employeesTable;
        } else {
            dashboardDiv.innerHTML += '<br/>' + '<p style="margin-top: 2rem; color: #666;">Нет данных о сотрудниках для этой компании</p>';
        }

    } catch (error) {
        loadingDiv.style.display = 'none';
        errorDiv.style.display = 'block';
        errorDiv.textContent = `Ошибка: ${error.message}`;
        console.error('Profession statistics error:', error);
    }
}
