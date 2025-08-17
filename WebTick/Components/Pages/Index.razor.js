export function initIndex() {
   
   
   
    // tu código
// ===== Utilidades cortas
    const $ = s => document.querySelector(s);

    // ===== Tema (persistente)
    const themeToggle = $('#themeToggle');
    const themeIcon = $('#themeIcon');
    const body = document.body;
    (function initTheme() {
        const saved = localStorage.getItem('theme');
        if (saved) {
            body.classList.toggle('dark', saved === 'dark');
            body.classList.toggle('light', saved !== 'dark');
            themeIcon.classList.toggle('fa-sun', saved === 'dark');
            themeIcon.classList.toggle('fa-moon', saved !== 'dark');
        } else {
            body.classList.add('light');
        }
    })();

    themeToggle.addEventListener('click', () => {
        const toDark = !body.classList.contains('dark');
        body.classList.toggle('dark', toDark);
        body.classList.toggle('light', !toDark);
        themeIcon.classList.toggle('fa-sun', toDark);
        themeIcon.classList.toggle('fa-moon', !toDark);
        localStorage.setItem('theme', toDark ? 'dark' : 'light');
    });
    

    // ===== Modal de Match (demo)
    const matchModal = $('#matchModal');

    function openModal() {
        matchModal.classList.remove('hidden');
        matchModal.setAttribute('aria-hidden', 'false');
        trapFocus(matchModal);
    }

    function closeModal() {
        matchModal.classList.add('hidden');
        matchModal.setAttribute('aria-hidden', 'true');
        releaseTrap();
    }

    // Abre automáticamente a los 3s (demo)
    setTimeout(openModal, 3000);

    // Cerrar clic fuera
    matchModal.addEventListener('click', (e) => {
        if (e.target === matchModal) closeModal();
    });

    // Cerrar con ESC
    document.addEventListener('keydown', (e) => {
        if (e.key === 'Escape' && !matchModal.classList.contains('hidden')) closeModal();
    });

    // ===== Focus Trap accesible
    let trapCleanup = null;

    function trapFocus(container) {
        const selectors = ['a[href]', 'area[href]', 'input:not([disabled])', 'select:not([disabled])', 'textarea:not([disabled])', 'button:not([disabled])', '[tabindex]:not([tabindex="-1"])'];
        const list = Array.from(container.querySelectorAll(selectors.join(',')));
        if (list.length === 0) return;

        const first = list[0], last = list[list.length - 1];

        function onKeydown(e) {
            if (e.key !== 'Tab') return;
            if (e.shiftKey) {
                if (document.activeElement === first) {
                    last.focus();
                    e.preventDefault();
                }
            } else {
                if (document.activeElement === last) {
                    first.focus();
                    e.preventDefault();
                }
            }
        }

        container.addEventListener('keydown', onKeydown);
        // foco inicial
        setTimeout(() => first.focus(), 0);
        trapCleanup = () => container.removeEventListener('keydown', onKeydown);
    }

    function releaseTrap() {
        if (trapCleanup) {
            trapCleanup();
            trapCleanup = null;
        }
    }

    // Acción "Ir al chat" (demo)
    $('#btnIrChat').addEventListener('click', () => {
        closeModal();
        // Aquí podrías redirigir al chat:
        // window.location.href = 'chat.html';
    });

    // ===== Mostrar nav en md+ (simple, sin Tailwind)
    function updateNav() {
        const navMd = document.getElementById('navMd');
        if (window.innerWidth >= 768) navMd.style.display = 'flex';
        else navMd.style.display = 'none';
    }

    window.addEventListener('resize', updateNav);
    updateNav();
}
