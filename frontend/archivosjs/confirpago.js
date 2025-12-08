// script-confirmacion.js

document.addEventListener('DOMContentLoaded', function() {

    // 1. Simular pago al hacer clic en "Desea continuar la compra"
    const continueButton = document.querySelector('.btn-warning');
    const cancelButton = document.querySelector('.btn-outline-secondary');

    continueButton.addEventListener('click', function(e) {
        e.preventDefault();
        alert("✅ Pago procesado con éxito!\n\nGracias por comprar con EduMentor.\nTu curso estará disponible en tu perfil.");
        // Redirigir a la página de perfil o historial de compra
        // window.location.href = 'perfil.html';
    });

    // 2. Cancelar compra
    cancelButton.addEventListener('click', function(e) {
        e.preventDefault();
        if (confirm("¿Estás seguro de que deseas cancelar la compra?")) {
            window.location.href = 'tienda.html'; // o donde quieras redirigir
        }
    });

    // 3. Efecto hover en WhatsApp
    const whatsappIcon = document.querySelector('.whatsapp-icon');
    if (whatsappIcon) {
        whatsappIcon.addEventListener('mouseenter', function() {
            this.style.transform = 'scale(1.1)';
            this.style.boxShadow = '0 8px 16px rgba(0,0,0,0.3)';
        });
        whatsappIcon.addEventListener('mouseleave', function() {
            this.style.transform = 'scale(1)';
            this.style.boxShadow = '0 4px 8px rgba(0,0,0,0.2)';
        });
    }

});