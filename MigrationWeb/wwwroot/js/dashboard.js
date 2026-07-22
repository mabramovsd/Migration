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

// Handle menu actions (from menu.js)
async function handleMenuAction(action) {
    const dashboardDiv = document.getElementById('dashboard');
    const loadingDiv = document.getElementById('loading');
    const errorDiv = document.getElementById('error');

    try {
        // Сначала сбрасываем дашборд в начальное состояние
        loadingDiv.style.display = 'block';
        loadingDiv.textContent = 'Загрузка...';
        errorDiv.style.display = 'none';
        dashboardDiv.style.display = 'none';
        
        // Ждем небольшую задержку для перерисовки
        await new Promise(resolve => setTimeout(resolve, 100));
        
        if (action === 'addEmployee') {
            // Show loading
            loadingDiv.textContent = 'Загрузка формы добавления сотрудника...';

            // Fetch companies for dropdown
            const responseCompanies = await fetch('/Company/All');
            
            if (!responseCompanies.ok) {
                throw new Error(`Ошибка при загрузке компаний: ${responseCompanies.status} ${responseCompanies.statusText}`);
            }
            
            const companies = await responseCompanies.json();
            
            // Hide loading and show form
            loadingDiv.style.display = 'none';
            dashboardDiv.style.display = 'block';
            
            // Render form for adding employee
            dashboardDiv.innerHTML = renderAddEmployeeForm(companies);
            
            // Attach form submit handler after rendering
            setTimeout(function() {
                const form = document.getElementById('addEmployeeForm');
                if (form) {
                    form.addEventListener('submit', handleAddEmployeeFormSubmit);
                }
            }, 0);
        } else if (action === 'listEmployees') {
            // TODO: Implement list employees functionality
            console.log('List employees - not implemented yet');
        } else if (action === 'aboutSystem') {
            // TODO: Implement about system functionality
            console.log('About system - not implemented yet');
        }
    } catch (error) {
        loadingDiv.style.display = 'none';
        errorDiv.style.display = 'block';
        errorDiv.textContent = `Ошибка: ${error.message}`;
        console.error('Menu action error:', error);
    }
}

// Render form for adding new employee
function renderAddEmployeeForm(companies) {
    // Generate company options for dropdown
    const companyOptions = companies.map(company => 
        `<option value="${escapeHtml(company.id)}">${escapeHtml(company.name)}</option>`
    ).join('');
    
    return `
        <div style="margin-top: 2rem;">
            <div class="card-header" style="margin-bottom: 1rem;">
                <h2>➕ Добавить сотрудника</h2>
            </div>
            <div style="max-width: 600px; margin: 0 auto;">
                <form id="addEmployeeForm" style="display: flex; flex-direction: column; gap: 1rem;">
                    <div>
                        <label for="employeeName" style="display: block; margin-bottom: 0.5rem; font-weight: 600; color: #333;">Имя сотрудника:</label>
                        <input type="text" id="employeeName" name="employeeName" required 
                               style="width: 100%; padding: 0.75rem; border: 1px solid #ddd; border-radius: 4px; font-size: 1rem; box-sizing: border-box;">
                    </div>
                    
                    <div>
                        <label for="employeeBirthDate" style="display: block; margin-bottom: 0.5rem; font-weight: 600; color: #333;">Дата рождения:</label>
                        <input type="datetime-local" id="employeeBirthDate" name="employeeBirthDate" required 
                               style="width: 100%; padding: 0.75rem; border: 1px solid #ddd; border-radius: 4px; font-size: 1rem; box-sizing: border-box;">
                    </div>
                    
                    <div>
                        <label for="employeeCompany" style="display: block; margin-bottom: 0.5rem; font-weight: 600; color: #333;">Компания:</label>
                        <select id="employeeCompany" name="employeeCompany" required 
                                style="width: 100%; padding: 0.75rem; border: 1px solid #ddd; border-radius: 4px; font-size: 1rem; box-sizing: border-box;">
                            <option value="">Выберите компанию</option>
                            ${companyOptions}
                        </select>
                    </div>
                    
                    <button type="submit" style="padding: 0.75rem 1.5rem; background-color: #667eea; color: white; border: none; border-radius: 4px; font-size: 1rem; cursor: pointer; transition: background-color 0.2s;">
                        ➕ Создать сотрудника
                    </button>
                </form>
            </div>
        </div>
    `;
}

// Function to handle form submission
async function handleAddEmployeeFormSubmit(event) {
    event.preventDefault();
    
    const name = document.getElementById('employeeName').value.trim();
    const birthDate = document.getElementById('employeeBirthDate').value;
    const companyId = document.getElementById('employeeCompany').value;
    
    if (!name || !birthDate || !companyId) {
        alert('Пожалуйста, заполните все поля');
        return;
    }
    
    // TODO: Implement actual employee creation logic
    console.log('Creating employee:', { name, birthDate, companyId });
    
    // Example API call (to be implemented later):
    /*
    try {
        const response = await fetch('/HR/Create', {
            method: 'POST',
            headers: {
                'Content-Type': 'application/json'
            },
            body: JSON.stringify({
                fullName: name,
                birthDate: birthDate,
                currentCompanyId: companyId
            })
        });
        
        if (response.ok) {
            alert('Сотрудник успешно создан!');
            handleIndexClick(); // Return to dashboard
        } else {
            const error = await response.json();
            alert(`Ошибка при создании сотрудника: ${error.message}`);
        }
    } catch (error) {
        alert(`Ошибка при создании сотрудника: ${error.message}`);
    }
    */
}

// Export functions for use in other modules
window.handleMenuAction = handleMenuAction;
window.renderAddEmployeeForm = renderAddEmployeeForm;
window.handleAddEmployeeFormSubmit = handleAddEmployeeFormSubmit;
