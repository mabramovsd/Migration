// Menu items for the application
const menuItems = [
    { icon: '➕', text: 'Добавить сотрудника', href: '#', action: 'addEmployee' },
    { icon: '📋', text: 'Список сотрудников', href: '#', action: 'listEmployees' },
    { icon: 'ℹ️', text: 'О системе', href: '#', action: 'aboutSystem' }
];

// Initialize menu items with click handlers
document.addEventListener('DOMContentLoaded', function() {
    const menuContainer = document.getElementById('sidebar-menu');
    if (menuContainer && menuItems.length > 0) {
        menuItems.forEach(item => {
            const button = document.createElement('button');
            button.className = 'menu-btn';
            button.innerHTML = `${item.icon} ${item.text}`;
            button.onclick = function() {
                if (typeof handleMenuAction === 'function') {
                    handleMenuAction(item.action);
                }
            };
            menuContainer.appendChild(button);
        });
    }
});
