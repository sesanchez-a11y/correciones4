// script-pagos.js

document.addEventListener('DOMContentLoaded', function() {
    // Validar formulario de datos de pago
    const formDatosPago = document.getElementById('formDatosPago');
    const nombrePago = document.getElementById('nombrePago');
    const correoPago = document.getElementById('correoPago');
    const identificacionPago = document.getElementById('identificacionPago');

    // Leer parámetros de la URL
    function getQueryParams() {
        const params = {};
        window.location.search.replace(/[?&]+([^=&]+)=([^&]*)/gi, function(m, key, value) {
            params[key] = decodeURIComponent(value);
        });
        return params;
    }
    const params = getQueryParams();

    // Mostrar datos en las cards y resumen
    const cursoEl = document.getElementById('infoCurso');
    const docenteEl = document.getElementById('infoDocente');
    const precioEl = document.getElementById('infoPrecio');
    const resumenCurso = document.getElementById('resumenCurso');
    const resumenDocente = document.getElementById('resumenDocente');
    const resumenPrecio = document.getElementById('resumenPrecio');
    const resumenTotal = document.getElementById('resumenTotal');

    // Depuración: mostrar los parámetros recibidos en consola
    console.log('Parámetros recibidos en pagos.html:', params);

    let precio = params.precio ? parseFloat(params.precio) : 0;
    let total = precio;

    // Mostrar datos del curso de la tienda o advertir si faltan
    if (params.curso && params.curso !== '' && cursoEl) {
        cursoEl.textContent = params.curso;
        if (resumenCurso) resumenCurso.textContent = params.curso;
    } else if (cursoEl) {
        cursoEl.textContent = 'No se recibió el nombre del curso.';
    }
    if (params.docente && params.docente !== '' && docenteEl) {
        docenteEl.textContent = params.docente;
        if (resumenDocente) resumenDocente.textContent = params.docente;
    } else if (docenteEl) {
        docenteEl.textContent = '- (no recibido)';
    }
    if (params.precio && params.precio !== '' && precioEl) {
        precioEl.textContent = `$${Number(precio).toFixed(2)}`;
        if (resumenPrecio) resumenPrecio.textContent = `$${Number(precio).toFixed(2)}`;
    } else if (precioEl) {
        precioEl.textContent = '- (no recibido)';
    }
    if (resumenTotal) resumenTotal.textContent = `$${Number(total).toFixed(2)}`;

    // Botón de pago funcional
    const btnPagar = document.getElementById('btnPagar');
    if (btnPagar) {
        btnPagar.addEventListener('click', async function(e) {
            e.preventDefault();
            // Siempre intentar registrar la reserva y redirigir
            const token = localStorage.getItem('token');
            if (!token) {
                alert('Debes iniciar sesión para reservar.');
                window.location.href = './reseccion.html';
                return;
            }
            try {
                await fetch('http://localhost:5000/api/Reservas/crear', {
                    method: 'POST',
                    headers: {
                        'Content-Type': 'application/json',
                        'Authorization': `Bearer ${token}`
                    },
                    body: JSON.stringify({
                        ServicioId: params.id || '',
                        Curso: params.curso || '',
                        Docente: params.docente || '',
                        Precio: precio,
                        MetodoPago: 'paypal',
                        Nombre: nombrePago.value,
                        Correo: correoPago.value,
                        Identificacion: identificacionPago.value
                    })
                });
            } catch (err) {
                // Ignorar error de conexión para no bloquear la redirección
            }
            // Redirigir siempre a confirmación de pago con los datos
            window.location.href = `confirpago.html?curso=${encodeURIComponent(params.curso||'')}&docente=${encodeURIComponent(params.docente||'')}&precio=${precio}`;
        });
    }
    // 1. Validar selección de método de pago
    const paymentMethods = document.querySelectorAll('input[name="paymentMethod"]');
    paymentMethods.forEach(radio => {
        radio.addEventListener('change', function() {
            if (this.value === 'tarjetas') {
                alert("El método de pago 'Tarjetas' aún no está disponible. Por favor, elige PayPal.");
                this.checked = false;
                document.getElementById('paypal1').checked = true;
            }
        });
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

    // 4. Cambiar texto del botón si se selecciona otro método (opcional)
    document.getElementById('paypal2').addEventListener('change', function() {
        payButton.textContent = "Pagar 10.99 US$ por Paypal (Danilo)";
    });

    document.getElementById('paypal1').addEventListener('change', function() {
        payButton.textContent = "Pagar 10.99 US$ por Paypal";
    });
});