(function() {
    function applyTheme(theme) {
        if (theme === 'system') {
            const systemTheme = window.matchMedia('(prefers-color-scheme: dark)').matches ? 'dark' : 'light';
            document.documentElement.setAttribute('data-theme', systemTheme);
            document.documentElement.setAttribute('data-bs-theme', systemTheme);
        } else {
            document.documentElement.setAttribute('data-theme', theme);
            document.documentElement.setAttribute('data-bs-theme', theme);
        }
    }
    
    const savedTheme = localStorage.getItem('theme') || 'system';
    applyTheme(savedTheme);

    window.addEventListener('DOMContentLoaded', () => {
        const themeSelect = document.getElementById('theme-select');
        if (themeSelect) {
            themeSelect.value = savedTheme;
            themeSelect.addEventListener('change', (e) => {
                const newTheme = e.target.value;
                localStorage.setItem('theme', newTheme);
                applyTheme(newTheme);
            });
        }

        // Listen for system changes if 'system' is selected
        window.matchMedia('(prefers-color-scheme: dark)').addEventListener('change', e => {
            if (localStorage.getItem('theme') === 'system') {
                applyTheme('system');
            }
        });
    });
})();
