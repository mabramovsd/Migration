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

/**
 * Renders form for adding new employee
 * @param {Array} companies - Array of company objects
 * @returns {string} - HTML string for the form
 */
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

// Export functions for use in other modules
window.renderEmployeesTable = renderEmployeesTable;
window.renderAddEmployeeForm = renderAddEmployeeForm;
