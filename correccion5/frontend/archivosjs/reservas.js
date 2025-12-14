// reservas.js

document.addEventListener('DOMContentLoaded', function() {

    // 1. Simular clic en el botón de reproducción del video
    const playButton = document.querySelector('.play-button');
    if (playButton) {
        playButton.addEventListener('click', function() {
            alert("Reproduciendo video del profesor Luis.M...");
            // Aquí podrías insertar un video real con un iframe o modal
        });
    }

    // 2. Simular clic en "Reservar clase"
    const reserveButton = document.querySelector('.reserve-button');
    if (reserveButton) {
        reserveButton.addEventListener('click', function() {
            alert("✅ Clase reservada con éxito!\n\nTe enviaremos un correo con los detalles.\n¡Prepárate para aprender con Luis.M!");
            // window.location.href = 'confirmarpago.html'; // Redirigir a confirmación
        });
    }

    // 3. Simular clic en "Ver Horarios"
    const viewScheduleButton = document.querySelector('.view-schedule-button');
    if (viewScheduleButton) {
        viewScheduleButton.addEventListener('click', function() {
            alert("📅 Horarios disponibles:\nLunes 18:00 - 20:00\nMiércoles 19:00 - 21:00\nViernes 17:00 - 19:00");
        });
    }

    // 4. Simular clic en "Mandar mensaje"
    const sendMessageButton = document.querySelector('.send-message-button');
    if (sendMessageButton) {
        sendMessageButton.addEventListener('click', function() {
            alert("✉️ Mensaje enviado a Luis.M.\nÉl te responderá en las próximas 24 horas.");
        });
    }

    // 5. Efecto hover en WhatsApp
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

    // 6. Filtrar profesores dinámicamente
    const searchInput = document.getElementById('buscar');
    const selectClase = document.getElementById('clase');
    const selectPrecio = document.getElementById('precio');
    const selectPais = document.getElementById('pais');
    const selectDispon = document.getElementById('disponibilidad');

    const teacherItems = Array.from(document.querySelectorAll('.teacher-item'));

    function parsePriceRange(value) {
        // Intentar extraer dos números de la cadena "10 a 20 $" (puede variar)
        const nums = value.match(/\d+/g);
        if (!nums || nums.length < 2) return null;
        return { min: parseInt(nums[0], 10), max: parseInt(nums[1], 10) };
    }

    function matchesPrice(item, priceRange) {
        if (!priceRange) return true;
        const price = parseFloat(item.getAttribute('data-price') || '0');
        return price >= priceRange.min && price <= priceRange.max;
    }

    function matchesText(item, text) {
        if (!text) return true;
        const t = text.trim().toLowerCase();
        const name = (item.getAttribute('data-name') || '').toLowerCase();
        const subject = (item.getAttribute('data-subject') || '').toLowerCase();
        return name.includes(t) || subject.includes(t);
    }

    function matchesPais(item, paisValue) {
        if (!paisValue || paisValue.toLowerCase().includes('cualquier')) return true;
        const p = (item.getAttribute('data-country') || '').toLowerCase();
        return p === paisValue.toLowerCase();
    }

    function filterTeachers() {
        const q = searchInput ? searchInput.value : '';
        const clase = selectClase ? selectClase.value : '';
        const precioVal = selectPrecio ? selectPrecio.value : '';
        const paisVal = selectPais ? selectPais.value : '';

        const priceRange = parsePriceRange(precioVal);

        teacherItems.forEach(item => {
            const byText = matchesText(item, q);
            const byPrice = matchesPrice(item, priceRange);
            const byPais = matchesPais(item, paisVal);

            if (byText && byPrice && byPais) {
                item.style.display = '';
            } else {
                item.style.display = 'none';
            }
        });
    }

    if (searchInput) {
        let debounce;
        searchInput.addEventListener('keyup', function(e) {
            clearTimeout(debounce);
            debounce = setTimeout(filterTeachers, 220);
        });
    }
    [selectClase, selectPrecio, selectPais, selectDispon].forEach(el => {
        if (el) el.addEventListener('change', filterTeachers);
    });

    // Ejecutar filtro inicial para ocultar nada
    filterTeachers();

});