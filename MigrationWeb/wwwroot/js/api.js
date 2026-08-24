// Functions for API calls
/**
 * Get list of companies
 * @returns - Companies list or empty array
 */
async function getCompanies() {
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

    return companiesData;
}

async function getProfessions() {
    const res = await fetch('/Company/Professions');
    if (!res.ok) throw new Error(`Ошибка загрузки профессий: ${res.status}`);
    return res.json();
}

// Export for use in other modules
window.getCompanies = getCompanies;
window.getProfessions = getProfessions;