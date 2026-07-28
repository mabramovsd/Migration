// Dashboard Navigator - handles navigation between dashboard screens

// Handle index/dashboard click - loads initial company statistics
async function handleIndexClick() {
    const loadingDiv = document.getElementById('loading');
    const errorDiv = document.getElementById('error');
    const dashboardDiv = document.getElementById('dashboard');

    try {
        // Load companies data first (for image lookup)
        let companiesData = [];
        try {
            const companiesResponse = await fetch('/Company/All');
            if (companiesResponse.ok) {
                companiesData = await companiesResponse.json();
            }
        } catch (err) {
            console.warn('Не удалось загрузить список компаний:', err);
            companiesData = [];
        }

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
        dashboardDiv.style.display = 'block';
        
        // Render company cards in grid container
        // Create map for quick image lookup: name -> imageUrl
        const companyImageMap = {};
        companiesData.forEach(company => {
            if (company.alias) {
                companyImageMap[company.alias.toLowerCase()] = company.image ? 'img/' + company.image : null;
            }
        });

        const companyCards = data.map(item => {
            const imageUrl = companyImageMap[item.companyName] || 'img/' + item.companyName + '.png';
            return `<div class="company-card" onclick="handleCompanyClick('${escapeHtml(item.companyName)}', '${escapeHtml(imageUrl)}')">
                ${imageUrl ? `<div class="company-image" style="width: 30px; height: 30px; overflow: hidden; border-radius: 4px; flex-shrink: 0;"><img src="${imageUrl}" style="width: 100%; height: 100%; object-fit: cover;"></div>` : ''}
                <div class="company-name">${escapeHtml(item.companyName)}</div>
                <div class="company-count">${item.count}</div>
            </div>`;
        }).join('');
        
        dashboardDiv.innerHTML = `
            <div class="dashboard-grid" style="display: grid;">
                ${companyCards}
            </div>
        `;
        
        // Fetch resources list from all companies
        const responseResources = await fetch('/Company/Resources');
        
        if (!responseResources.ok) {
            throw new Error(`Ошибка при загрузке данных ресурсов: ${responseResources.status} ${responseResources.statusText}`);
        }
        const resourcesData = await responseResources.json();
        
        // Use the helper function to render resources table
        dashboardDiv.innerHTML += renderResourcesTable(resourcesData, `Ресурсы всех компаний`);

    } catch (error) {
        loadingDiv.style.display = 'none';
        errorDiv.style.display = 'block';
        errorDiv.textContent = `Ошибка: ${error.message}`;
        console.error('Dashboard error:', error);
    }
}

// Handle company card click - loads profession statistics for selected company
async function handleCompanyClick(companyName, imageUrl) {
    const loadingDiv = document.getElementById('loading');
    const errorDiv = document.getElementById('error');
    const dashboardDiv = document.getElementById('dashboard');

    // For "All" and "unknown" companies, do nothing
    if (!companyName || companyName.toLowerCase() === 'all' || companyName.toLowerCase() === 'unknown') {
        return;
    }

    try {
        loadingDiv.style.display = 'block';
        loadingDiv.textContent = `Загрузка данных для ${escapeHtml(companyName)}...`;
        errorDiv.style.display = 'none';

        // Fetch profession counts for the selected company
        const responseProfessions = await fetch(`/HR/Stats/ProfessionCounts/${encodeURIComponent(companyName)}`);
        
        if (!responseProfessions.ok) {
            throw new Error(`Ошибка при загрузке статистики: ${responseProfessions.status} ${responseProfessions.statusText}`);
        }
        
        const professionData = await responseProfessions.json();

        // Fetch resources list for the selected company
        const responseResources = await fetch(`/Company/Resources/${encodeURIComponent(companyName)}`);
        
        if (!responseResources.ok) {
            throw new Error(`Ошибка при загрузке данных ресурсов: ${responseResources.status} ${responseProfessions.statusText}`);
        }
        const resourcesData = await responseResources.json();

        // Check if we have any data
        const hasProfessions = professionData && professionData.length > 0;
        const hasResources = resourcesData && resourcesData.length > 0;

        // Hide loading
        loadingDiv.style.display = 'none';
        dashboardDiv.style.display = 'block';

        // Always show company header with image
        let htmlContent = `
            <div class="company-header" style="display: flex; align-items: center; gap: 1rem; padding: 1.5rem 0;">
                ${imageUrl ? `<div style="width: 60px; height: 60px; flex-shrink: 0;"><img src="${imageUrl}" style="width: 100%; height: 100%; object-fit: cover; border-radius: 8px;"></div>` : ''}
                <div>
                    <div style="color: #667eea; font-size: 1.5rem; font-weight: 700;">${escapeHtml(companyName)}</div>
                    ${companyName ? `<div style="color: #666; font-size: 0.9rem;">${escapeHtml(companyName)}</div>` : ''}
                </div>
            </div>
        `;

        if (!hasProfessions && !hasResources) {
            htmlContent += `
                <div style="margin-top: 2rem; padding: 2rem; text-align: center; color: #666; background: #f7f7f7; border-radius: 8px;">
                    <div style="font-size: 1.2rem;">Ресурсы и профессии для компании ${escapeHtml(companyName)} не найдены</div>
                </div>
            `;
        } else {
            // Render profession cards if available
            if (hasProfessions) {
                const professionCards = professionData.map(item => `
                    <div class="company-card" onclick="handleProfessionClick('${escapeHtml(companyName)}', '${escapeHtml(item.professionTitle)}')">
                        <div class="company-name">${escapeHtml(item.professionTitle)}</div>
                        <div class="company-count">${item.count}</div>
                    </div>
                `).join('');
                htmlContent += `
                    <div class="dashboard-grid" style="display: grid; margin-top: 2rem;">
                        <div style="color: #667eea; font-size: 1.2rem; font-weight: 600; grid-column: 1 / -1; margin-bottom: 0.5rem;">Профессии</div>
                        ${professionCards}
                    </div>
                `;
            }

            // Render resources table if available
            if (hasResources) {
                htmlContent += renderResourcesTable(resourcesData, `Ресурсы компании ${escapeHtml(companyName)}`);
            }
        }

        // Fetch employees list for the selected company
        const responseEmployees = await fetch(`/HR/Filter?Company=${encodeURIComponent(companyName)}`);
        
        if (!responseEmployees.ok) {
            throw new Error(`Ошибка при загрузке данных сотрудников: ${responseEmployees.status} ${responseEmployees.statusText}`);
        }
        const employeesData = await responseEmployees.json();
        
        htmlContent += renderEmployeesTable(employeesData, `Сотрудники компании ${escapeHtml(companyName)}`);
    
        dashboardDiv.innerHTML = htmlContent;

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

// Export functions for use in other modules
window.handleIndexClick = handleIndexClick;
window.handleCompanyClick = handleCompanyClick;
window.handleProfessionClick = handleProfessionClick;
