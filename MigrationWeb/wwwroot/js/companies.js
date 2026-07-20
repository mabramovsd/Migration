// Companies JavaScript - отображение компаний на карте острова
document.addEventListener('DOMContentLoaded', function() {
    loadCompaniesOnMap();
});

async function loadCompaniesOnMap() {
    const sidebarLogo = document.querySelector('.sidebar-logo');
    
    if (!sidebarLogo) {
        console.error('Элемент sidebar-logo не найден');
        return;
    }

    try {
        // Загружаем список компаний с координатами
        const response = await fetch('/Company/All');
        
        if (!response.ok) {
            throw new Error(`Ошибка при загрузке компаний: ${response.status} ${response.statusText}`);
        }
        
        const companies = await response.json();
        
        // Фильтруем только компании с валидными координатами и изображениями
        const companiesWithCoords = companies.filter(c => 
            c.latitude !== null && c.longitude !== null &&
            typeof c.latitude === 'number' && typeof c.longitude === 'number' &&
            c.image // Проверяем, что есть изображение
        );
        
        // Получаем все компании (без ограничения)
        const allCompanies = companiesWithCoords;
        
        // Находим контейнер для кнопок компаний (уже есть в HTML)
        const companiesContainer = document.getElementById('companies-map-container');
        
        if (!companiesContainer) {
            console.error('Элемент companies-map-container не найден');
            return;
        }
        
        // Очищаем старые элементы (если есть)
        companiesContainer.innerHTML = '';
        
        // Создаем картинки для каждой компании
        allCompanies.forEach(company => {
            const companyImage = document.createElement('img');
            companyImage.src = "img/" + company.image;
            companyImage.className = 'company-map-image';
            companyImage.alt = company.name;
            companyImage.title = company.name;
            
            // Устанавливаем позицию на основе координат
            // Центрируем картинку: вычитаем 15 пикселей (половина размера 30px)
            companyImage.style.left = `${company.longitude - 5}%`;
            companyImage.style.top = `${company.latitude - 5}%`;
            
            // Добавляем обработчик клика на картинку компании
            companyImage.addEventListener('click', () => {
                handleCompanyClick(company.alias);
            });
            
            companiesContainer.appendChild(companyImage);
        });
        
        // Добавляем обработчик клика на картинку острова (сброс дашборда)
        const islandLogo = document.getElementById('island-logo');
        if (islandLogo) {
            islandLogo.addEventListener('click', () => {
                handleIndexClick();
            });
        }
        
        console.log(`Отображено компаний на карте: ${allCompanies.length}`);
        
    } catch (error) {
        console.error('Ошибка при загрузке компаний на карту:', error);
    }
}
