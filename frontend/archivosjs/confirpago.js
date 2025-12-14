// script-confirmacion.js

document.addEventListener('DOMContentLoaded', function() {

    // Mostrar datos de la reserva usando los parámetros de la URL
    function getQueryParams() {
        const params = {};
        window.location.search.replace(/[?&]+([^=&]+)=([^&]*)/gi, function(m, key, value) {
            params[key] = decodeURIComponent(value);
        });
        return params;
    }
    const params = getQueryParams();
    if (params.curso) {
        const el = document.getElementById('confCurso');
        if (el) el.textContent = params.curso;
    }
    if (params.docente) {
        const el = document.getElementById('confDocente');
        if (el) el.textContent = params.docente;
    }
    if (params.precio) {
        const el = document.getElementById('confPrecio');
        if (el) el.textContent = `$${params.precio}`;
        const el2 = document.getElementById('confPrecio2');
        if (el2) el2.textContent = `$${params.precio}`;
    }
    if (params.descuento) {
        const el = document.getElementById('confDescuento');
        if (el) el.textContent = `$${Number(params.descuento).toFixed(2)}`;
        const el2 = document.getElementById('confDescuento2');
        if (el2) el2.textContent = `-$${Number(params.descuento).toFixed(2)}`;
    }
    if (params.total) {
        const el = document.getElementById('confTotal');
        if (el) el.textContent = `$${params.total}`;
    }

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