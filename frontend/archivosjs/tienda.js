document.addEventListener('DOMContentLoaded', () => {
    console.log('La página está cargada y lista.');
    
    // Ejemplo de funcionalidad JS: manejar el clic en la navegación
    const navLinks = document.querySelectorAll('.main-nav a');
    
    navLinks.forEach(link => {
        link.addEventListener('click', (e) => {
            // Elimina la clase 'active' de todos los enlaces
            navLinks.forEach(l => l.classList.remove('active'));
            
            // Añade la clase 'active' al enlace clickeado
            e.currentTarget.classList.add('active');
            
            // Si quieres evitar que navegue a #, descomenta la línea de abajo
            // e.preventDefault(); 
            
            console.log(`Navegación activa: ${e.currentTarget.textContent}`);
        });
    });

    // NOTA: Para hacer los filtros desplegables interactivos
    // (mostrando/ocultando contenido), se necesitaría más JS.
    // Este código es solo un punto de partida para que la página cobre vida.
});