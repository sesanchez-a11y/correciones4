// ================
// FUNCIONALIDAD PARA LA PÁGINA DE LOGIN
// ================

document.addEventListener('DOMContentLoaded', function () {
  
  // 1. Detectar si estamos en la página de login (acepta `login-page` o `reseccion-page`)
  if (document.body.classList.contains('login-page') || document.body.classList.contains('reseccion-page')) {
    const loginButton = document.querySelector('.primary-login');
    const loginForm = document.getElementById('loginForm');
    const socialButtons = {
      linkedin: document.querySelector('.linkedin'),
      google: document.querySelector('.google'),
      apple: document.querySelector('.apple')
    };

    // Botón principal "Iniciar sesión"
    if (loginButton) {
      // Legacy: si existe un botón con clase primary-login, mantener comportamiento mínimo (redirige al formulario)
      loginButton.addEventListener('click', function () {
        const formEl = document.getElementById('loginForm');
        if (formEl) formEl.scrollIntoView({ behavior: 'smooth' });
      });
    }

    if (loginForm) {
      loginForm.addEventListener('submit', async function (e) {
        e.preventDefault();
        const email = document.getElementById('login-email')?.value?.trim();
        const password = document.getElementById('login-password')?.value;
        const msgEl = document.getElementById('loginMessage');

        if (!email || !password) {
          if (msgEl) { msgEl.style.display = 'block'; msgEl.textContent = 'Correo y contraseña son obligatorios.'; }
          return;
        }

        try {
          const resp = await fetch('http://localhost:5000/api/ControladorDeSesion/login', {
            method: 'POST',
            headers: { 'Content-Type': 'application/json' },
            body: JSON.stringify({ Email: email, Password: password })
          });

          const data = await resp.json();
          if (resp.ok) {
            if (msgEl) { msgEl.style.display = 'block'; msgEl.classList.remove('text-danger'); msgEl.classList.add('text-success'); msgEl.textContent = data.message || 'Inicio de sesión exitoso.'; }
            console.log('✓ Login success:', data);
            
            // Guardar token en localStorage (si se devuelve uno real)
            if (data.token) {
              try {
                localStorage.setItem('token', data.token);
                // Verificar que el token fue guardado (Firefox puede requerir pequeña espera)
                let verify = localStorage.getItem('token');
                let attempts = 0;
                while (verify !== data.token && attempts < 5) {
                  // pequeña espera
                  await new Promise(r => setTimeout(r, 100));
                  verify = localStorage.getItem('token');
                  attempts++;
                }
                if (verify === data.token) console.log('✓ Token guardado en localStorage');
                else console.warn('⚠️ token no verificado en localStorage después de intentos');
              } catch (e) {
                console.warn('⚠️ Error al guardar token en localStorage:', e);
              }
            }
            
            // Guardar datos públicos del usuario
            if (data.user) {
              try {
                localStorage.setItem('currentUser', JSON.stringify(data.user));
                // Verificar que currentUser quedó guardado
                let verifyUser = localStorage.getItem('currentUser');
                let verifyAttempts = 0;
                while (verifyUser === null && verifyAttempts < 5) {
                  await new Promise(r => setTimeout(r, 100));
                  verifyUser = localStorage.getItem('currentUser');
                  verifyAttempts++;
                }
                console.log('✓ Datos del usuario guardados en localStorage');
              } catch (e) {
                console.warn('⚠️ Error al guardar currentUser en localStorage:', e);
              }
                console.log('⏳ Redirigiendo según rol en 800ms...');
              // Dar más tiempo para asegurar que todo esté guardado
              setTimeout(() => { 
                console.log('🔄 Redirigiendo según rol...');
                try {
                  if (data.user && (data.user.rol || '').toLowerCase() === 'admin') {
                    window.location.href = 'perfiladmin.html';
                  } else {
                    window.location.href = 'perfil.html';
                  }
                } catch (e) {
                  window.location.href = 'perfil.html';
                }
              }, 800);
              return;
            }
            
            // Si no hay user pero hay token, redirigir de todas formas
            if (data.token) {
              console.log('⚠️ Token obtenido pero sin datos de usuario, redirigiendo de todas formas');
              setTimeout(() => { 
                window.location.href = 'perfil.html'; 
              }, 800);
              return;
            }
          } else {
            if (msgEl) { msgEl.style.display = 'block'; msgEl.classList.remove('text-success'); msgEl.classList.add('text-danger'); msgEl.textContent = data.message || 'Error en login.'; }
            console.error('✗ Login error:', data);
          }
        } catch (err) {
          if (msgEl) { msgEl.style.display = 'block'; msgEl.classList.remove('text-success'); msgEl.classList.add('text-danger'); msgEl.textContent = 'Error de conexión con el servidor.'; }
          console.error('Fetch error:', err);
        }
      });
    }

    // Botones sociales (opcionales)
    if (socialButtons.linkedin) {
      socialButtons.linkedin.addEventListener('click', function () {
        alert('Iniciando sesión con LinkedIn... (simulado)');
        // window.location.href = 'https://linkedin.com/oauth/...';
      });
    }

    if (socialButtons.google) {
      socialButtons.google.addEventListener('click', function () {
        alert('Iniciando sesión con Google... (simulado)');
      });
    }

    if (socialButtons.apple) {
      socialButtons.apple.addEventListener('click', function () {
        alert('Iniciando sesión con Apple... (simulado)');
      });
    }
  }

  // Mostrar Política de Privacidad en modal
  const privacyLink = document.getElementById('privacy-link');
  if (privacyLink) {
    privacyLink.addEventListener('click', function (e) {
      e.preventDefault();
      const body = document.getElementById('privacyModalBody');
      if (!body) return;
      body.innerHTML = `
        <p><strong>EduMentor – Protegemos tus datos</strong><br><small>Última actualización: 26 de noviembre de 2025</small></p>
        <h6>1. ¿Qué datos recopilamos?</h6>
        <p>Al registrarte o usar EduMentor, podemos recopilar:<br>
        <strong>Datos personales:</strong> Nombre completo, correo electrónico, número de teléfono, rol (ej. "estudiante universitario").<br>
        <strong>Datos de pago:</strong> Método de pago (sin almacenar números de tarjeta).<br>
        <strong>Datos técnicos:</strong> IP, navegador, dispositivo, historial de cursos comprados/inscritos.</p>

        <h6>2. ¿Para qué usamos tus datos?</h6>
        <p>Tus datos se utilizan para:<br>
        Crear y gestionar tu cuenta de usuario.<br>
        Procesar pagos y enviar certificados.<br>
        Enviar notificaciones sobre cursos, promociones o actualaciones.<br>
        Mejorar la Plataforma mediante análisis de uso.<br>
        Cumplir obligaciones legales.<br>
        <strong>No vendemos ni compartimos tus datos con terceros, salvo con:</strong><br>
        Proveedores de pago (PayPal, etc.).<br>
        Autoridades legales, si así lo exige la ley.</p>

        <h6>3. Base legal para el tratamiento</h6>
        <p>El tratamiento de tus datos se basa en:<br>
        Ejecución de un contrato (tu inscripción en cursos).<br>
        Consentimiento (para envío de promociones).<br>
        Obligaciones legales (facturación, prevención de fraudes).</p>

        <h6>4. Tus derechos</h6>
        <p>Tienes derecho a:<br>
        Acceder, rectificar o eliminar tus datos.<br>
        Solicitar la portabilidad de tus datos.<br>
        Oponerte al tratamiento de tus datos.<br>
        Retirar tu consentimiento en cualquier momento.<br>
        Para ejercer estos derechos, contáctanos en: <a href="mailto:capacitacion@tutorias.ec">capacitacion@tutorias.ec</a></p>

        <h6>5. Seguridad de tus datos</h6>
        <p>Implementamos medidas técnicas y organizativas (encriptación SSL, controles de acceso) para proteger tus datos contra pérdida, uso indebido o alteración.</p>

        <h6>6. Cookies</h6>
        <p>Usamos cookies para:<br>
        Recordar tu sesión de usuario.<br>
        Analizar el tráfico del sitio.<br>
        Personalizar contenido.<br>
        Puedes gestionar las cookies desde la configuración de tu navegador.</p>

        <h6>7. Menores de edad</h6>
        <p>No recopilamos intencionalmente datos de menores de 16 años. Si descubrimos que un menor nos ha proporcionado datos, los eliminaremos inmediatamente.</p>

        <h6>8. Cambios en esta política</h6>
        <p>Actualizaremos esta Política si cambian nuestras prácticas. La versión vigente estará siempre disponible en la Plataforma.</p>

        <h6>9. Contacto</h6>
        <p>¿Preguntas sobre privacidad? Escríbenos a:<br>
        <strong>📧 capacitacion@tutorias.ec</strong><br>
        <strong>📞 +593 98 541 2458</strong></p>
      `;

      // Mostrar modal usando Bootstrap
      const modalEl = document.getElementById('privacyModal');
      if (modalEl) {
        const modal = new bootstrap.Modal(modalEl);
        modal.show();
      }
    });
  }

  // ================
  // FUNCIONALIDAD PARA LA PÁGINA DE REGISTRO
  // ================

  // Si estamos en la página de registro
  if (document.body.classList.contains('register-page')) {
    const btnAlumno = document.getElementById('btn-alumno');
    const btnTutor = document.getElementById('btn-tutor');
    const registerForm = document.getElementById('registerForm');

    if (btnAlumno && btnTutor) {
      btnAlumno.classList.add('active');

      btnAlumno.addEventListener('click', function () {
        btnAlumno.classList.add('active');
        btnTutor.classList.remove('active');
      });

      btnTutor.addEventListener('click', function () {
        btnTutor.classList.add('active');
        btnAlumno.classList.remove('active');
      });
    }

    if (registerForm) {
      registerForm.addEventListener('submit', function (e) {
        e.preventDefault();

        const email = document.getElementById('email')?.value.trim();
        const nombre = document.getElementById('nombre')?.value.trim();
        const apellido = document.getElementById('apellido')?.value.trim();
        const edad = document.getElementById('edad')?.value;
        const especializacion = document.getElementById('especializacion')?.value.trim();

        if (!email || !nombre || !apellido || !edad || !especializacion) {
          alert('Por favor, complete todos los campos.');
          return;
        }

        if (!validateEmail(email)) {
          alert('Por favor, ingrese un correo válido.');
          return;
        }

        if (edad < 18 || edad > 100) {
          alert('La edad debe estar entre 18 y 100 años.');
          return;
        }

        const rol = btnTutor.classList.contains('active') ? 'Tutor' : 'Alumno';
        alert(`¡Registro exitoso!\nRol: ${rol}\nCorreo: ${email}`);
        // window.location.href = 'dashboard.html'; // Redirección futura
      });
    }
  }
});

// Validación de email
function validateEmail(email) {
  const re = /^[^\s@]+@[^\s@]+\.[^\s@]+$/;
  return re.test(String(email).toLowerCase());
}