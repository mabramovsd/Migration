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

// Handle company change - update profession checkboxes
function handleCompanyChange(event) {
    const selectedCompany = event.target.value;
    const professionCheckboxesDiv = document.getElementById('professionCheckboxes');
    
    if (!professionCheckboxesDiv) {
        return;
    }
    
    // Get all professions from data attribute
    const allProfessions = JSON.parse(professionCheckboxesDiv.dataset.professions || '[]');
    
    // Filter professions by selected company
    const companyProfessions = allProfessions.filter(p => p.company === selectedCompany && p.title != 'Все');
    
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
    
    // Get selected professions
    const selectedProfessions = Array.from(document.querySelectorAll('input[name="professions"]:checked'))
        .map(cb => cb.value);
    
    if (!name || !birthDate || !companyId) {
        alert('Пожалуйста, заполните все поля');
        return;
    }
    
    if (selectedProfessions.length === 0) {
        alert('Пожалуйста, выберите хотя бы одну профессию');
        return;
    }
    
    // TODO: Implement actual employee creation logic
    console.log('Creating employee:', { name, birthDate, companyId, professions: selectedProfessions });
    
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
                currentCompanyId: companyId,
                professions: selectedProfessions
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
window.handleAddEmployeeFormSubmit = handleAddEmployeeFormSubmit;
window.handleCompanyChange = handleCompanyChange;
