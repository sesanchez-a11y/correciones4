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

    // 6. Filtrar por búsqueda (simulación básica)
    const searchInput = document.getElementById('buscar');
    if (searchInput) {
        searchInput.addEventListener('keyup', function(e) {
            if (e.key === 'Enter') {
                alert(`Buscando: "${this.value}"...`);
                // Aquí podrías filtrar los profesores dinámicamente
            }
        });
    }

});