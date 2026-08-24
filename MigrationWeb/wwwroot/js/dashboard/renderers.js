// Dashboard Renderers - HTML generation functions

/**
 * Renders employees table from data
 * @param {Array} employeesData - Array of employee objects
 * @param {string} title - Table title
 * @returns {string} - HTML string for the table
 */
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
                        <th>Действия</th>
                    </tr>
                </thead>
                <tbody>
                    ${employeesData.map(item => `
                        <tr>
                            <td>${escapeHtml(item.fullName)}</td>
                            <td>${item.birthDate ? new Date(item.birthDate).toLocaleDateString('ru-RU') : 'Не указано'}</td>
                            <td>${escapeHtml(item.currentCompany)}</td>
                            <td>
                                <button onclick="handleEditEmployee('${item.id}')" style="padding: 0.35rem 0.75rem; margin-right: 0.5rem; background-color: #667eea; color: white; border: none; border-radius: 4px; font-size: 0.85rem; cursor: pointer;">✏️ Редактировать</button>
                                <button onclick="handleDeleteEmployee('${item.id}')" style="padding: 0.35rem 0.75rem; background-color: #dc3545; color: white; border: none; border-radius: 4px; font-size: 0.85rem; cursor: pointer;">🗑️ Удалить</button>
                            </td>
                        </tr>
                    `).join('')}
                </tbody>
            </table>
        </div>
    `;
}

/**
 * Renders form for adding new employee
 * @param {Array} companies - Array of company objects
 * @param {Array} professions - Array of profession objects
 * @returns {string} - HTML string for the form
 */
function renderAddEmployeeForm(companies, professions) {
    // Generate company options for dropdown
    const companyOptions = companies.map(company => 
        `<option value="${escapeHtml(company.alias)}">${escapeHtml(company.name)}</option>`
    ).join('');
    
    // Store all professions in data attribute for JS to use
    const professionsJson = JSON.stringify(professions || []);
    
    // Generate profession checkboxes container (initially empty, will be populated by JS)
    const professionCheckboxesHtml = `
        <div id="professionCheckboxes" data-professions="${escapeHtml(professionsJson)}" style="margin-top: 1rem;">
            <label style="display: block; margin-bottom: 0.5rem; font-weight: 600; color: #333;">Выберите профессии:</label>
            <p style="color: #666; font-size: 0.9rem;">Сначала выберите компанию выше</p>
        </div>
    `;
    
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
                    
                    ${professionCheckboxesHtml}
                    
                    <button type="submit" style="padding: 0.75rem 1.5rem; background-color: #667eea; color: white; border: none; border-radius: 4px; font-size: 1rem; cursor: pointer; transition: background-color 0.2s;">
                        ➕ Создать сотрудника
                    </button>
                </form>
            </div>
        </div>
    `;
}

/**
 * Renders form for editing existing employee
 * @param {Array} companies - Array of company objects
 * @param {Array} professions - Array of profession objects
 * @param {Object} employeeData - Employee data to pre-fill the form
 * @returns {string} - HTML string for the form
 */
function renderEditEmployeeForm(companies, professions, employeeData) {
    // Generate company options for dropdown with current company selected
    const companyOptions = companies.map(company => 
        `<option value="${escapeHtml(company.alias)}" ${company.alias === employeeData.currentCompany ? 'selected' : ''}>${escapeHtml(company.name)}</option>`
    ).join('');
    
    // Store all professions in data attribute for JS to use
    const professionsJson = JSON.stringify(professions || []);
    
    // Generate profession checkboxes container (initially empty, will be populated by JS)
    const professionCheckboxesHtml = `
        <div id="professionCheckboxes" data-professions="${escapeHtml(professionsJson)}" style="margin-top: 1rem;">
            <label style="display: block; margin-bottom: 0.5rem; font-weight: 600; color: #333;">Выберите профессии:</label>
            <p style="color: #666; font-size: 0.9rem;">Сначала выберите компанию выше</p>
        </div>
    `;
    
    // Format birthDate for datetime-local input (expected format: yyyy-MM-ddTHH:mm)
    let birthDateValue = '';
    if (employeeData.birthDate) {
        const date = new Date(employeeData.birthDate);
        const year = date.getFullYear();
        const month = String(date.getMonth() + 1).padStart(2, '0');
        const day = String(date.getDate()).padStart(2, '0');
        const hours = String(date.getHours()).padStart(2, '0');
        const minutes = String(date.getMinutes()).padStart(2, '0');
        birthDateValue = `${year}-${month}-${day}T${hours}:${minutes}`;
    }
    
    return `
        <div style="margin-top: 2rem;">
            <div class="card-header" style="margin-bottom: 1rem;">
                <h2>✏️ Редактировать сотрудника</h2>
            </div>
            <div style="max-width: 600px; margin: 0 auto;">
                <form id="editEmployeeForm" style="display: flex; flex-direction: column; gap: 1rem;">
                    <div>
                        <label for="employeeName" style="display: block; margin-bottom: 0.5rem; font-weight: 600; color: #333;">Имя сотрудника:</label>
                        <input type="text" id="employeeName" name="employeeName" value="${escapeHtml(employeeData.fullName)}" required 
                               style="width: 100%; padding: 0.75rem; border: 1px solid #ddd; border-radius: 4px; font-size: 1rem; box-sizing: border-box;">
                    </div>
                    
                    <div>
                        <label for="employeeBirthDate" style="display: block; margin-bottom: 0.5rem; font-weight: 600; color: #333;">Дата рождения:</label>
                        <input type="datetime-local" id="employeeBirthDate" name="employeeBirthDate" value="${birthDateValue}" required 
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
                    
                    ${professionCheckboxesHtml}
                    
                    <button type="submit" style="padding: 0.75rem 1.5rem; background-color: #667eea; color: white; border: none; border-radius: 4px; font-size: 1rem; cursor: pointer; transition: background-color 0.2s;">
                        ✏️ Сохранить изменения
                    </button>
                </form>
            </div>
        </div>
    `;
}

/**
 * Renders resources table from data
 * @param {Array} resourcesData - Array of resource objects
 * @param {string} title - Table title
 * @returns {string} - HTML string for the table
 */
function renderResourcesTable(resourcesData, title) {
    if (!resourcesData || resourcesData.length === 0) {
        return '<p style="margin-top: 2rem; color: #666;">Нет данных о ресурсах</p>';
    }
    
    return `
        <div style="margin-top: 2rem;">
            <div style="color: #667eea; font-size: 1.2rem; font-weight: 600;">${title}</div>
            <table class="resources-table" style="margin-top: 0.5rem;">
                <thead>
                    <tr>
                        <th>Название</th>
                        <th>Количество</th>
                        <th>Единица измерения</th>
                    </tr>
                </thead>
                <tbody>
                    ${resourcesData.map(item => `
                        <tr>
                            <td>${escapeHtml(item.title)}</td>
                            <td>${item.count !== null && item.count !== undefined ? item.count : '0'}</td>
                            <td>${escapeHtml(item.unit)}</td>
                        </tr>
                    `).join('')}
                </tbody>
            </table>
        </div>
    `;
}

/**
 * Renders about system page with service health statuses
 * @param {Array} services - Array of ServiceHealthStatus
 * @returns {string} - HTML string for the about page
 */
function renderAboutSystem(services) {
    if (!services || services.length === 0) {
        return '<p style="margin-top: 2rem; color: #666;">Не удалось загрузить данные о сервисах</p>';
    }

    const statusBadge = (isAvailable) => 
        isAvailable 
            ? '<span style="display: inline-block; padding: 0.25rem 0.75rem; background-color: #d4edda; color: #155724; border-radius: 12px; font-size: 0.85rem; font-weight: 600;">● Доступен</span>'
            : '<span style="display: inline-block; padding: 0.25rem 0.75rem; background-color: #f8d7da; color: #721c24; border-radius: 12px; font-size: 0.85rem; font-weight: 600;">● Недоступен</span>';

    const versionInfo = (version) => {
        if (!version) return '';
        try {
            const data = JSON.parse(version);
            return `<span style="color: #666; font-size: 0.85rem;">v${data.apiVersion || data.assemblyVersion || 'unknown'}</span>`;
        } catch {
            return `<span style="color: #666; font-size: 0.85rem;">${version.substring(0, 50)}...</span>`;
        }
    };

    const errorInfo = (error) => error ? `<div style="color: #dc3545; font-size: 0.85rem; margin-top: 0.25rem;">${escapeHtml(error)}</div>` : '';

    return `
        <div style="margin-top: 2rem;">
            <div class="card-header" style="margin-bottom: 1rem;">
                <h2>ℹ️ О системе</h2>
            </div>
            
            <div style="max-width: 800px;">
                <p style="color: #555; margin-bottom: 1.5rem;">HR-платформа для управления персоналом. Версия 2.0.0</p>
                
                <h3 style="color: #667eea; font-size: 1.1rem; margin-bottom: 0.75rem;">Статус сервисов</h3>
                <table class="employees-table" style="margin-top: 0.5rem;">
                    <thead>
                        <tr>
                            <th style="width: 200px;">Сервис</th>
                            <th style="width: 150px;">Статус</th>
                            <th>Версия / Примечание</th>
                        </tr>
                    </thead>
                    <tbody>
                        ${services.map(s => `
                            <tr>
                                <td><strong>${escapeHtml(s.serviceName)}</strong></td>
                                <td>${statusBadge(s.isAvailable)}</td>
                                <td>
                                    ${versionInfo(s.version)}
                                    ${errorInfo(s.error)}
                                </td>
                            </tr>
                        `).join('')}
                    </tbody>
                </table>
            </div>
        </div>
    `;
}

function renderResourceForecast(forecastData, title) {
    if (!forecastData || forecastData.length === 0) {
        return '<p style="margin-top:1rem;color:#666;">Нет данных для прогноза</p>';
    }

    let html = `<div style="margin-top:1.5rem;">
        <div style="color:#667eea;font-size:1.2rem;font-weight:600;">${title}</div>
        <table class="resources-table">
            <thead><tr>
                <th>Ресурс</th>
                <th>Текущий запас</th>
                <th>Прогноз через ${forecastData[0]?.days || 30} дней</th>
                <th>Прирост</th>
            </tr></thead>
            <tbody>
                ${forecastData.map(item => `
                    <tr>
                        <td>${escapeHtml(item.resource)}</td>
                        <td>${item.currentAmount} ${escapeHtml(item.unit)}</td>
                        <td>${item.totalAmount.toFixed(1)} ${escapeHtml(item.unit)}</td>
                        <td style="color: ${item.producedAmount >= 0 ? '#28a745' : '#dc3545'};">
                            ${item.producedAmount >= 0 ? '+' : ''}${item.producedAmount.toFixed(1)} ${escapeHtml(item.unit)}
                        </td>
                    </tr>
                `).join('')}
            </tbody>
        </table>
    </div>`;

    return html;
}

function renderResourceForecastChart(forecastData, canvasId) {
    if (!forecastData || forecastData.length === 0) return;

    const ctx = document.getElementById(canvasId);
    if (!ctx) return;

    // Для простоты показываем bar chart или line chart
    // Здесь можно показать текущий запас и прогнозный
    const labels = forecastData.map(f => f.resource);
    const current = forecastData.map(f => f.currentAmount);
    const forecast = forecastData.map(f => f.totalAmount);

    new Chart(ctx, {
        type: 'bar',
        data: {
            labels: labels,
            datasets: [
                {
                    label: 'Текущий запас',
                    data: current,
                    backgroundColor: 'rgba(54, 162, 235, 0.6)',
                    borderColor: 'rgba(54, 162, 235, 1)',
                    borderWidth: 1
                },
                {
                    label: 'Прогноз через 30 дней',
                    data: forecast,
                    backgroundColor: 'rgba(75, 192, 192, 0.6)',
                    borderColor: 'rgba(75, 192, 192, 1)',
                    borderWidth: 1
                }
            ]
        },
        options: {
            responsive: true,
            plugins: {
                legend: { position: 'top' },
                title: { display: true, text: 'Прогноз ресурсов' }
            },
            scales: {
                y: { beginAtZero: true }
            }
        }
    });
}

// Export functions for use in other modules
window.renderEmployeesTable = renderEmployeesTable;
window.renderAddEmployeeForm = renderAddEmployeeForm;
window.renderEditEmployeeForm = renderEditEmployeeForm;
window.renderResourcesTable = renderResourcesTable;
window.renderAboutSystem = renderAboutSystem;
window.renderResourceForecast = renderResourceForecast;
window.renderResourceForecastChart = renderResourceForecastChart;
