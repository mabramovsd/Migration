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
window.handleAddEmployeeFormSubmit = handleAddEmployeeFormSubmit;
