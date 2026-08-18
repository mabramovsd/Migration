// Dashboard Menu - handles menu actions and form submissions

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

            // Fetch companies and professions in parallel
            const [responseCompanies, responseProfessions] = await Promise.all([
                fetch('/Company/All'),
                fetch('/Company/Professions')
            ]);
            
            if (!responseCompanies.ok) {
                throw new Error(`Ошибка при загрузке компаний: ${responseCompanies.status} ${responseCompanies.statusText}`);
            }
            
            const companies = await responseCompanies.json();
            
            if (!responseProfessions.ok) {
                throw new Error(`Ошибка при загрузке профессий: ${responseProfessions.status} ${responseProfessions.statusText}`);
            }
            
            const professions = await responseProfessions.json();
            
            // Hide loading and show form
            loadingDiv.style.display = 'none';
            dashboardDiv.style.display = 'block';
            
            // Render form for adding employee
            dashboardDiv.innerHTML = renderAddEmployeeForm(companies, professions);
            
            // Attach form submit handler and company change handler after rendering
            setTimeout(function() {
                const form = document.getElementById('addEmployeeForm');
                if (form) {
                    form.addEventListener('submit', handleAddEmployeeFormSubmit);
                }
                
                // Attach change handler for company dropdown
                const companySelect = document.getElementById('employeeCompany');
                if (companySelect) {
                    companySelect.addEventListener('change', handleCompanyChange);
                }
            }, 0);
        } else if (action === 'listEmployees') {
            // TODO: Implement list employees functionality
            console.log('List employees - not implemented yet');
        } else if (action === 'aboutSystem') {
            loadingDiv.textContent = 'Загрузка статуса сервисов...';

            const response = await fetch('/api/health/status');
            
            if (!response.ok) {
                throw new Error(`Ошибка при загрузке статуса: ${response.status} ${response.statusText}`);
            }
            
            const services = await response.json();
            
            loadingDiv.style.display = 'none';
            dashboardDiv.style.display = 'block';
            
            dashboardDiv.innerHTML = renderAboutSystem(services);
        }
    } catch (error) {
        loadingDiv.style.display = 'none';
        errorDiv.style.display = 'block';
        errorDiv.textContent = `Ошибка: ${error.message}`;
        console.error('Menu action error:', error);
    }
}

// Simple GUID generator
function Guid() {}
Guid.newGuid = function() {
    return 'xxxxxxxx-xxxx-4xxx-yxxx-xxxxxxxxxxxx'.replace(/[xy]/g, function(c) {
        const r = Math.random() * 16 | 0;
        const v = c === 'x' ? r : (r & 0x3 | 0x8);
        return v.toString(16);
    }).toUpperCase();
};

// Handle company change - update profession checkboxes
function handleCompanyChange(event) {
    const selectedCompany = event.target.value;
    const professionCheckboxesDiv = document.getElementById('professionCheckboxes');
    
    if (!professionCheckboxesDiv) {
        return;
    }
    
    // Get all professions from data attribute
    const allProfessions = JSON.parse(professionCheckboxesDiv.dataset.professions || '[]');
    
    // Filter professions by selected company (exclude "All" option)
    const companyProfessions = allProfessions.filter(p => p.company === selectedCompany && p.title.toLowerCase() !== 'все');
    
    // Render checkboxes for this company's professions
    if (companyProfessions.length > 0) {
        professionCheckboxesDiv.innerHTML = `
            <div style="margin-top: 1rem;">
                <label style="display: block; margin-bottom: 0.5rem; font-weight: 600; color: #333;">Выберите профессии:</label>
                ${companyProfessions.map(profession => `
                    <div style="margin-bottom: 0.5rem;">
                        <label style="display: flex; align-items: center; cursor: pointer;">
                            <input type="checkbox" name="professions" value="${escapeHtml(profession.column)}" 
                                   style="margin-right: 0.5rem; width: 1.2rem; height: 1.2rem;">
                            ${escapeHtml(profession.title)}
                        </label>
                    </div>
                `).join('')}
            </div>
        `;
    } else {
        professionCheckboxesDiv.innerHTML = `
            <div style="margin-top: 1rem; color: #666;">
                <p>Для этой компании нет доступных профессий</p>
            </div>
        `;
    }
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
    
    // Get all professions from data attribute
    const professionCheckboxesDiv = document.getElementById('professionCheckboxes');
    const allProfessions = JSON.parse(professionCheckboxesDiv.dataset.professions || '[]');
    
    // Filter professions by selected company
    const companyProfessions = allProfessions.filter(p => p.company === companyId && p.title.toLowerCase() !== 'все');
    
    // Build Professions: all professions with true/false based on checkbox
    const professions = {};
    companyProfessions.forEach(profession => {
        const isChecked = document.querySelector(`input[name="professions"][value="${profession.column}"]:checked`);
        professions[profession.column] = isChecked !== null;
    });
    
    // Build CreateEmployeeRequest model
    const request = {
        Event: "AddEmployee",
        CoreData: {
            Id: Guid.newGuid(),
            BirthDate: birthDate,
            FullName: name,
            CurrentCompany: companyId,
            IsDeleted: false
        },
        Professions: professions
    };
    
    try {
        const response = await fetch('/HR/Create', {
            method: 'POST',
            headers: {
                'Content-Type': 'application/json'
            },
            body: JSON.stringify(request)
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
}

// Handle edit employee - loads employee data and shows edit form
async function handleEditEmployee(employeeId) {
    const dashboardDiv = document.getElementById('dashboard');
    const loadingDiv = document.getElementById('loading');
    const errorDiv = document.getElementById('error');

    try {
        loadingDiv.style.display = 'block';
        loadingDiv.textContent = 'Загрузка данных сотрудника...';
        errorDiv.style.display = 'none';

        // Fetch employee data
        const responseEmployee = await fetch(`/HR/GetById?employeeId=${employeeId}`);
        if (!responseEmployee.ok) {
            throw new Error(`Ошибка при загрузке сотрудника: ${responseEmployee.status}`);
        }
        const employeeData = await responseEmployee.json();

        // Fetch companies and professions
        const [responseCompanies, responseProfessions] = await Promise.all([
            fetch('/Company/All'),
            fetch('/Company/Professions')
        ]);

        if (!responseCompanies.ok) {
            throw new Error(`Ошибка при загрузке компаний: ${responseCompanies.status}`);
        }
        if (!responseProfessions.ok) {
            throw new Error(`Ошибка при загрузке профессий: ${responseProfessions.status}`);
        }

        const companies = await responseCompanies.json();
        const professions = await responseProfessions.json();

        // Store employee ID for the submit handler
        window._editingEmployeeId = employeeId;

        loadingDiv.style.display = 'none';
        dashboardDiv.style.display = 'block';

        // Render edit form
        dashboardDiv.innerHTML = renderEditEmployeeForm(companies, professions, employeeData);

        // Attach form submit handler
        setTimeout(function () {
            const form = document.getElementById('editEmployeeForm');
            if (form) {
                form.addEventListener('submit', handleEditEmployeeFormSubmit);
            }

            // Attach company change handler for profession checkboxes
            const companySelect = document.getElementById('employeeCompany');
            if (companySelect) {
                companySelect.addEventListener('change', handleCompanyChange);

                const eventForProfessionsLoad = new Event('change', { bubbles: true });
                companySelect.dispatchEvent(eventForProfessionsLoad);
            }
        }, 0);
    } catch (error) {
        loadingDiv.style.display = 'none';
        errorDiv.style.display = 'block';
        errorDiv.textContent = `Ошибка: ${error.message}`;
        console.error('Edit employee error:', error);
    }
}

// Function to handle edit form submission
async function handleEditEmployeeFormSubmit(event) {
    event.preventDefault();
    
    const name = document.getElementById('employeeName').value.trim();
    const birthDate = document.getElementById('employeeBirthDate').value;
    const companyId = document.getElementById('employeeCompany').value;
    
    if (!name || !birthDate || !companyId) {
        alert('Пожалуйста, заполните все поля');
        return;
    }
    
    // Get employee ID from the page (we need to store it somewhere)
    // For now, we'll use a global variable set by handleEditEmployee
    if (!window._editingEmployeeId) {
        alert('Ошибка: не указан ID редактируемого сотрудника');
        return;
    }
    
    // Get all professions from data attribute
    const professionCheckboxesDiv = document.getElementById('professionCheckboxes');
    const allProfessions = JSON.parse(professionCheckboxesDiv.dataset.professions || '[]');
    
    // Filter professions by selected company
    const companyProfessions = allProfessions.filter(p => p.company === companyId && p.title.toLowerCase() !== 'все');
    
    // Build Professions: all professions with true/false based on checkbox
    const professions = {};
    companyProfessions.forEach(profession => {
        const isChecked = document.querySelector(`input[name="professions"][value="${profession.column}"]:checked`);
        professions[profession.column] = isChecked !== null;
    });
    
    // Build UpdateEmployeeRequest model
    const request = {
        Event: "EditEmployee",
        CoreData: {
            Id: window._editingEmployeeId,
            BirthDate: birthDate,
            FullName: name,
            CurrentCompany: companyId,
            IsDeleted: false
        },
        Professions: professions
    };
    console.log(request);
    
    try {
        const response = await fetch('/HR/Update', {
            method: 'POST',
            headers: {
                'Content-Type': 'application/json'
            },
            body: JSON.stringify(request)
        });
        
        if (response.ok) {
            alert('Сотрудник успешно обновлён!');
            window._editingEmployeeId = null; // Clear the global variable
            handleIndexClick(); // Return to dashboard
        } else {
            const error = await response.json();
            alert(`Ошибка при обновлении сотрудника: ${error.message}`);
        }
    } catch (error) {
        alert(`Ошибка при обновлении сотрудника: ${error.message}`);
    }
}

// Handle delete employee - confirm and call API
async function handleDeleteEmployee(employeeId) {
    if (!confirm('Вы уверены, что хотите удалить сотрудника?')) {
        return;
    }

    try {
        const response = await fetch('/HR/Delete', {
            method: 'DELETE',
            headers: {
                'Content-Type': 'application/json'
            },
            body: JSON.stringify({
                id: employeeId,
                softDelete: true
            })
        });

        if (response.ok) {
            alert('Сотрудник успешно удалён');
            handleIndexClick(); // Refresh dashboard
        } else {
            const error = await response.json();
            alert('Ошибка при удалении сотрудника: ' + (error.message || 'Unknown error'));
        }
    } catch (error) {
        alert('Ошибка при удалении сотрудника: ' + error.message);
    }
}

// Export functions for use in other modules
window.handleMenuAction = handleMenuAction;
window.handleAddEmployeeFormSubmit = handleAddEmployeeFormSubmit;
window.handleCompanyChange = handleCompanyChange;
window.handleEditEmployee = handleEditEmployee;
window.handleEditEmployeeFormSubmit = handleEditEmployeeFormSubmit;
window.handleDeleteEmployee = handleDeleteEmployee;
window.Guid = Guid;
