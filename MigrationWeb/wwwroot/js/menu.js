// Menu items for the application
const menuItems = [
    { icon: '➕', text: 'Добавить сотрудника', href: '#' },
    { icon: '📋', text: 'Список сотрудников', href: '#' },
    { icon: 'ℹ️', text: 'О системе', href: '#' }
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
                // Вызываем handleIndexClick для сброса дашборда
                if (typeof handleIndexClick === 'function') {
                    handleIndexClick();
                }
            };
            menuContainer.appendChild(button);
        });
    }
});
