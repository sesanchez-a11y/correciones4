// URL base de tu API de ASP.NET Core
// Reemplaza esto con la URL real de tu servidor (ej: 'http://localhost:5000')
const API_BASE_URL = 'http://localhost:5000/api'; 

document.addEventListener('DOMContentLoaded', function() {

    const btnAlumno = document.getElementById('btn-alumno');
    const btnTutor = document.getElementById('btn-tutor');
    const registerForm = document.getElementById('registerForm');
    
    // Función auxiliar para mostrar mensajes (reemplaza alert)
    const showMessage = (message, isError = false) => {
        // Temporalmente usamos alert, ¡pero se debe cambiar por un modal!
        console.log(`[${isError ? 'ERROR' : 'INFO'}] ${message}`);
        alert(message); 
    };

    // ===================================================
    // LÓGICA DE CAMBIO DE ROL: ESTA ES LA PARTE CLAVE
    // ===================================================
    
    /**
     * Alterna las clases de Bootstrap entre el botón seleccionado y el deseleccionado
     * para dar la retroalimentación visual de qué rol está activo.
     */
    const handleRoleChange = (selectedBtn, deselectedBtn, roleName) => {
        // 1. Botón Seleccionado: Debe tener color (btn-success)
        selectedBtn.classList.remove('btn-outline-secondary');
        selectedBtn.classList.add('btn-success');
        
        // 2. Botón Deseleccionado: Debe tener contorno (btn-outline-secondary)
        deselectedBtn.classList.add('btn-outline-secondary');
        deselectedBtn.classList.remove('btn-success');
        
        console.log(`Rol seleccionado: ${roleName}`);
    };
    
    // Configuración de Event Listeners para el cambio de rol
    if (btnAlumno && btnTutor) {
        // ERROR CORREGIDO: Asegurar que el evento 'click' llame a la función
        // Al hacer clic en Alumno
        btnAlumno.addEventListener('click', function() {
            // Alumno es el seleccionado, Tutor es el deseleccionado
            handleRoleChange(btnAlumno, btnTutor, 'Alumno');
        });

        // ERROR CORREGIDO: Asegurar que el evento 'click' llame a la función
        // Al hacer clic en Tutor
        btnTutor.addEventListener('click', function() {
            // Tutor es el seleccionado, Alumno es el deseleccionado
            handleRoleChange(btnTutor, btnAlumno, 'Tutor');
        });
    }
    
    // ===================================================
    // LÓGICA DE ENVÍO DE DATOS A MONGODB (VÍA ASP.NET CORE)
    // ===================================================
    if (registerForm) {
        registerForm.addEventListener('submit', function(e) {
            e.preventDefault();

            // 1. Recoger los datos del formulario
            const email = document.getElementById('email').value.trim();
            const password = document.getElementById('password').value; 
            const nombre = document.getElementById('nombre').value.trim();
            const apellido = document.getElementById('apellido').value.trim();
            const edad = document.getElementById('edad').value;
            const especializacion = document.getElementById('especializacion').value.trim();
            
            // Determinar el rol: verificamos qué botón tiene la clase de color sólido 'btn-success'
            const isTutorSelected = btnTutor.classList.contains('btn-success');
            const isAlumnoSelected = btnAlumno.classList.contains('btn-success');

            if (!isTutorSelected && !isAlumnoSelected) {
                showMessage('Por favor, seleccione si desea registrarse como Alumno o como Tutor.', true);
                return; // Detiene el envío si no hay rol seleccionado
            }
            
            const rol = isTutorSelected ? 'Tutor' : 'Alumno';


            // 2. Validaciones simples
            if (!email || !password || !nombre || !apellido || !edad || !especializacion) {
                showMessage('Por favor, complete todos los campos, incluyendo la contraseña.', true);
                return;
            }

            if (!validateEmail(email)) {
                showMessage('Por favor, ingrese un correo válido.', true);
                return;
            }

            if (edad < 18 || edad > 100) {
                showMessage('La edad debe estar entre 18 y 100 años.', true);
                return;
            }

            // 3. Crear el objeto de datos para enviar al backend
            const userData = {
                rol: rol,
                email: email,
                contrasena: password,
                nombre: nombre,
                apellido: apellido,
                edad: parseInt(edad),
                especializacion: especializacion
            };

            // 4. Llamar a la función de registro
            registerUser(userData);
        });
    }

    // Función para enviar los datos al endpoint de ASP.NET Core
    async function registerUser(userData) {
        try {
            // Se asume que el endpoint de registro es /api/Auth/register
            const response = await fetch(`${API_BASE_URL}/ControladorDeSesion/register`, {
                method: 'POST',
                headers: {
                    'Content-Type': 'application/json',
                },
                body: JSON.stringify(userData)
            });

            const result = await response.json();

            if (response.ok) {
                // Éxito: El usuario se guardó en MongoDB
                showMessage(`¡Registro exitoso como ${userData.rol}! Obteniendo token de sesión...`, false);
                console.log('Respuesta del servidor:', result);
                
                // Guardar datos públicos del usuario
                if (result && result.user) {
                    localStorage.setItem('currentUser', JSON.stringify(result.user));
                    
                    // Hacer login automático para obtener JWT token
                    await performAutoLogin(userData.email, userData.contrasena);
                    return;
                }
                
            } else {
                // Error del servidor (ej: 400 Bad Request, 409 Conflict)
                showMessage(`Error al registrar: ${result.message || response.statusText}`, true);
                console.error('Detalles del error:', result);
            }

        } catch (error) {
            // Error de red, CORS o conexión
            showMessage('Error de conexión con el servidor. Verifique la URL y el CORS.', true);
            console.error('Fetch error:', error);
        }
    }

    // Función auxiliar para login automático tras registro
    async function performAutoLogin(email, password) {
        try {
            console.log('🔐 Iniciando auto-login para:', email);
            const loginResponse = await fetch(`${API_BASE_URL}/ControladorDeSesion/login`, {
                method: 'POST',
                headers: {
                    'Content-Type': 'application/json',
                },
                body: JSON.stringify({ Email: email, Password: password })
            });

            const loginResult = await loginResponse.json();
            console.log('Auto-login response:', loginResponse.status, loginResult);

            if (loginResponse.ok && loginResult.token) {
                // Guardar token JWT y usuario
                localStorage.setItem('token', loginResult.token);
                if (loginResult.user) {
                    localStorage.setItem('currentUser', JSON.stringify(loginResult.user));
                }
                console.log('✓ Token JWT y usuario guardados');
                showMessage('Sesión iniciada correctamente. Redirigiendo a tu perfil...', false);
                setTimeout(() => { window.location.href = './perfil.html'; }, 700);
            } else {
                // Si login falla, redirigir de todas formas con currentUser (fallback)
                console.warn('⚠️ Auto-login falló:', loginResult.message || 'Sin detalles');
                showMessage('Redirigiendo con datos almacenados...', false);
                setTimeout(() => { window.location.href = './perfil.html'; }, 700);
            }
        } catch (error) {
            // Si login falla, redirigir de todas formas con currentUser (fallback)
            console.error('❌ Error en auto-login:', error);
            showMessage('Perfil cargado con datos almacenados localmente.', false);
            setTimeout(() => { window.location.href = './perfil.html'; }, 700);
        }
    }


    // Lógica para botones de inicio de sesión social 
    document.getElementById('google-login')?.addEventListener('click', (e) => {
        e.preventDefault();
        showMessage('Iniciando flujo de OAuth para Google. Requiere configuración en el backend.');
    });
    
    document.getElementById('linkedin-login')?.addEventListener('click', (e) => {
        e.preventDefault();
        showMessage('Iniciando flujo de OAuth para LinkedIn. Requiere configuración en el backend.');
    });
    
    document.getElementById('apple-login')?.addEventListener('click', (e) => {
        e.preventDefault();
        showMessage('Iniciando flujo de OAuth para Apple. Requiere configuración en el backend.');
    });

});

// Función auxiliar para validar email
function validateEmail(email) {
    const re = /^[^\s@]+@[^\s@]+\.[^\s@]+$/;
    return re.test(String(email).toLowerCase());
}