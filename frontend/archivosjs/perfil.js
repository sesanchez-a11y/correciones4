// API base URL
const API_BASE_URL = 'http://localhost:5000/api';

// Datos simulados del historial de compras (fallback/demo)
const historialData = [
  { curso: "Desarrollo Web desde Cero", categoria: "Programación / Web", fecha: "05/01/2025", metodoPago: "Tarjeta", instructor: "Carlos DevMaster" },
  { curso: "Excel para Principiantes 2025", categoria: "Productividad / Office", fecha: "08/01/2025", metodoPago: "PayPal", instructor: "Laura DataCoach" },
  { curso: "Fotografía Profesional con Smartphone", categoria: "Arte / Fotografía", fecha: "10/01/2025", metodoPago: "Apple Pay", instructor: "Miguel LensStudio" },
  { curso: "Marketing Digital Completo", categoria: "Negocios / Marketing", fecha: "12/01/2025", metodoPago: "Tarjeta", instructor: "Ana GrowthMentor" },
  { curso: "Edición de Video con Premiere Pro", categoria: "Multimedia / Video", fecha: "15/01/2025", metodoPago: "Google Pay", instructor: "Javier FilmPro" }
];

const cursosData = [
  { id: 1, nombre: "Desarrollo Web desde Cero", progreso: 85, instructor: "Carlos DevMaster", fechaInicio: "05/01/2025" },
  { id: 2, nombre: "Marketing Digital Completo", progreso: 40, instructor: "Ana GrowthMentor", fechaInicio: "12/01/2025" },
  { id: 3, nombre: "Liderazgo y Gestión de Equipos", progreso: 100, instructor: "Dr. Elena Torres", fechaInicio: "10/11/2024" }
];

const materialesData = [
  { nombre: "Guía de SEO Avanzado", curso: "Marketing Digital Completo", tipo: "PDF", tamano: "2.5 MB" },
  { nombre: "Ejercicios Prácticos de JavaScript", curso: "Desarrollo Web desde Cero", tipo: "ZIP", tamano: "10 MB" },
  { nombre: "Plantilla de Presupuestos", curso: "Excel para Principiantes 2025", tipo: "XLSX", tamano: "0.5 MB" }
];

// ============================================
// FUNCIONES DE AUTENTICACIÓN Y DATOS DE USUARIO
// ============================================

// Obtener datos del usuario desde el backend (/me endpoint)
async function fetchMeFromApi(token) {
  try {
    console.log('📡 Llamando a /me con token:', token.substring(0, 20) + '...');
    // Evitar enviar header 'Content-Type' en GET para compatibilidad con navegadores
    const response = await fetch(`${API_BASE_URL}/ControladorDeSesion/me`, {
      method: 'GET',
      headers: {
        'Authorization': `Bearer ${token}`
      }
    });

    console.log('📡 /me response status:', response.status);

    if (response.status === 401 || response.status === 403) {
      console.warn('⚠️ Token inválido o expirado (401/403)');
      localStorage.removeItem('token');
      return null;
    }

    if (response.ok) {
      const data = await response.json();
      console.log('✓ /me devolvió:', data);
      return data;
    } else {
      console.warn(`⚠️ Error al obtener /me: ${response.status}`);
      const errorData = await response.json().catch(() => ({}));
      console.warn('Error details:', errorData);
      return null;
    }
  } catch (error) {
    console.error('❌ Error de conexión en fetchMeFromApi:', error);
    return null;
  }
}

// Registrar manejadores globales para errores de promesas no manejadas
window.addEventListener('unhandledrejection', function (event) {
  console.warn('Unhandled promise rejection:', event.reason);
});

// Cargar datos del usuario en el card de perfil
async function loadUserData() {
  const token = localStorage.getItem('token');
  const currentUser = localStorage.getItem('currentUser');
  
  console.log('=== LOADUSERDATA INICIADO ===');
  console.log('Token en localStorage:', token ? '✓ Existe' : '✗ No existe');
  console.log('CurrentUser en localStorage:', currentUser ? '✓ Existe' : '✗ No existe');
  
  let user = null;

  // Intentar obtener datos desde /me si hay token
  if (token) {
    console.log('Intentando llamar /me con token...');
    user = await fetchMeFromApi(token);
    console.log('Respuesta de /me:', user ? '✓ Éxito' : '✗ Falló');
  }

  // Fallback a localStorage.currentUser si /me no devolvió nada
  if (!user && currentUser) {
    try {
      user = JSON.parse(currentUser);
      console.log('Usando datos de currentUser (localStorage)');
    } catch (e) {
      console.error('Error al parsear currentUser:', e);
    }
  }

  // Si no hay usuario en ningún lado, redirigir a login
  if (!user) {
    console.warn('✗ No hay usuario autenticado, redirigiendo a login en 3 segundos...');
    // CAMBIO: Aumentar tiempo para asegurar que se vea antes de desaparecer
    await new Promise(resolve => setTimeout(resolve, 3000));
    console.warn('Redirigiendo a reseccion.html...');
    window.location.href = './reseccion.html';
    return;
  }

  console.log('✓ Usuario encontrado:', user.nombre);

  // Esperar a que los elementos del DOM estén listos (con más intentos)
  let attempts = 0;
  const maxAttempts = 50; // 5 segundos máximo (50 * 100ms)
  await new Promise(resolve => {
    const checkElements = () => {
      attempts++;
      const userNameTable = document.getElementById('userNameTable');
      const userEmailTable = document.getElementById('userEmailTable');
      
      if (userNameTable && userEmailTable) {
        console.log(`✓ Elementos del DOM encontrados en intento ${attempts}`);
        resolve();
      } else if (attempts >= maxAttempts) {
        console.warn(`⚠️ No se encontraron elementos del DOM después de ${attempts} intentos`);
        resolve(); // Continuar de todas formas
      } else {
        setTimeout(checkElements, 100);
      }
    };
    checkElements();
  });

  // Actualizar elementos del Card de Perfil con datos reales
  const userAvatar = document.getElementById('userAvatarTable');
  const userName = document.getElementById('userNameTable');
  const userEmail = document.getElementById('userEmailTable');
  const userRol = document.getElementById('userRolTable');
  const userPhone = document.getElementById('userPhoneTable');

  console.log('Elementos encontrados:', {
    avatar: !!userAvatar,
    nombre: !!userName,
    email: !!userEmail,
    rol: !!userRol,
    phone: !!userPhone
  });

  // Construir nombre completo
  const nombreCompleto = `${user.nombre || ''} ${user.apellido || ''}`.trim() || 'Usuario Desconocido';

  if (userAvatar) {
    try {
      userAvatar.src = user.avatar || 'https://placehold.co/100x100/F0F8FF/0056b3?text=Avatar';
      console.log('✓ Avatar actualizado');
    } catch (e) {
      console.warn('No se pudo actualizar avatar:', e);
    }
  }
  
  if (userName) {
    try {
      userName.textContent = nombreCompleto;
      console.log('✓ Nombre actualizado:', nombreCompleto);
    } catch (e) {
      console.warn('No se pudo actualizar nombre:', e);
    }
  }
  
  if (userEmail) {
    try {
      const emailText = user.correo || user.email || 'correo@example.com';
      userEmail.textContent = `Email: ${emailText}`;
      console.log('✓ Email actualizado');
    } catch (e) {
      console.warn('No se pudo actualizar email:', e);
    }
  }
  
  if (userRol) {
    try {
      userRol.textContent = user.rol || 'Rol no definido';
      console.log('✓ Rol actualizado');
    } catch (e) {
      console.warn('No se pudo actualizar rol:', e);
    }
  }
  
  if (userPhone) {
    try {
      const phoneText = user.telefono || 'No disponible';
      userPhone.textContent = `Teléfono: ${phoneText}`;
      console.log('✓ Teléfono actualizado');
    } catch (e) {
      console.warn('No se pudo actualizar teléfono:', e);
    }
  }

  console.log('✓ Datos del usuario cargados correctamente');
}

// ============================================
// FUNCIONES DE CONTENIDO DINÁMICO
// ============================================

function getHistorialHTML() {
  return `
    <h2 class="mb-4 text-primary-custom border-bottom pb-2 fw-bold"><i class="fas fa-history me-2"></i>Historial de compra</h2>
    <div class="filter-section p-3 rounded-3 mb-4 border border-primary-custom">
      <div class="d-flex flex-wrap align-items-center gap-2">
        <label class="form-label mb-0 fw-medium text-dark">Filtrar por:</label>
        <select id="filterCurso" class="form-select form-select-sm shadow-sm" style="width: auto; min-width: 120px;"><option value="">Curso</option><option value="Desarrollo Web desde Cero">Desarrollo Web</option><option value="Excel para Principiantes 2025">Excel</option><option value="Fotografía Profesional con Smartphone">Fotografía</option><option value="Marketing Digital Completo">Marketing</option><option value="Edición de Video con Premiere Pro">Edición de Video</option></select>
        <select id="filterCategoria" class="form-select form-select-sm shadow-sm" style="width: auto; min-width: 120px;"><option value="">Categoría</option><option value="Programación / Web">Programación / Web</option><option value="Productividad / Office">Productividad</option><option value="Arte / Fotografía">Arte / Fotografía</option><option value="Negocios / Marketing">Marketing</option><option value="Multimedia / Video">Multimedia</option></select>
        <input type="date" id="filterFecha" class="form-control form-control-sm shadow-sm" style="width: auto; min-width: 140px;">
        <select id="filterMetodoPago" class="form-select form-select-sm shadow-sm" style="width: auto; min-width: 140px;"><option value="">Método de pago</option><option value="Tarjeta">Tarjeta</option><option value="PayPal">PayPal</option><option value="Apple Pay">Apple Pay</option><option value="Google Pay">Google Pay</option></select>
        <input type="text" id="filterDocente" class="form-control form-control-sm shadow-sm" placeholder="Docente" style="width: auto; min-width: 100px;">
        <input type="text" id="filterBuscar" class="form-control form-control-sm shadow-sm" placeholder="Buscar..." style="flex-grow: 1; max-width: 200px;">
        <button id="applyFilters" class="btn btn-sm btn-primary-custom shadow-sm"><i class="fas fa-search"></i> Aplicar</button>
        <button id="clearFilters" class="btn btn-sm btn-outline-secondary shadow-sm"><i class="fas fa-redo"></i> Limpiar</button>
      </div>
    </div>
    <div class="table-responsive mt-4">
      <table class="table table-striped table-hover"><thead><tr><th>Curso</th><th>Categoría</th><th>Fecha de inscripción</th><th>Método de pago</th><th>Instructor</th></tr></thead><tbody id="historialTableBody"></tbody></table>
      <div id="noDataMessage" class="alert alert-info text-center d-none">No se encontraron registros en el historial con los filtros aplicados.</div>
    </div>
  `;
}

function getCursosHTML() {
  let cardsHtml = cursosData.map(curso => {
    let progressClass = curso.progreso === 100 ? 'bg-success' : 'bg-primary';
    return `
      <div class="col-md-6 mb-4">
        <div class="card shadow-sm h-100">
          <div class="card-body">
            <h5 class="card-title text-primary-custom fw-bold">${curso.nombre}</h5>
            <p class="card-text small text-muted">Instructor: ${curso.instructor}</p>
            <p class="card-text small text-muted">Inscrito el: ${curso.fechaInicio}</p>
            <div class="progress mb-2" style="height: 10px;"><div class="progress-bar ${progressClass}" role="progressbar" style="width: ${curso.progreso}%"></div></div>
            <p class="card-text fw-medium small text-dark">Progreso: ${curso.progreso}%</p>
            <a href="#" class="btn btn-sm btn-warning mt-2 w-100 fw-bold">Continuar curso <i class="fas fa-arrow-right ms-1"></i></a>
          </div>
        </div>
      </div>
    `;
  }).join('');
  return `<h2 class="mb-4 text-primary-custom border-bottom pb-2 fw-bold"><i class="fas fa-chalkboard-teacher me-2"></i>Mis Cursos</h2><div class="row">${cardsHtml}</div>`;
}

function getMaterialesHTML() {
  let listItems = materialesData.map(material => {
    let icon = '', color = 'text-primary';
    if (material.tipo === 'PDF') { icon = 'fas fa-file-pdf'; color = 'text-danger'; }
    else if (material.tipo === 'ZIP') { icon = 'fas fa-file-archive'; color = 'text-warning'; }
    else if (material.tipo === 'XLSX') { icon = 'fas fa-file-excel'; color = 'text-success'; }
    return `<li class="list-group-item d-flex justify-content-between align-items-center shadow-sm mb-2 rounded-3"><div><i class="${icon} ${color} me-3 fa-lg"></i><span class="fw-bold">${material.nombre}</span><small class="text-muted ms-3">(${material.curso})</small></div><div><span class="badge bg-secondary me-3">${material.tamano}</span><button class="btn btn-sm btn-outline-primary"><i class="fas fa-download"></i></button></div></li>`;
  }).join('');
  return `<h2 class="mb-4 text-primary-custom border-bottom pb-2 fw-bold"><i class="fas fa-folder-open me-2"></i>Materiales de Apoyo</h2><p class="text-muted">Aquí puedes descargar los recursos y materiales de apoyo de tus cursos activos.</p><ul class="list-group mt-3">${listItems}</ul>`;
}

function getHorariosHTML() {
  return `<h2 class="mb-4 text-primary-custom border-bottom pb-2 fw-bold"><i class="far fa-clock me-2"></i>Horarios y Sesiones en Vivo</h2><div class="alert alert-warning d-flex align-items-center" role="alert"><i class="fas fa-calendar-alt me-3 fa-lg"></i><div><strong>Aviso Importante:</strong> Revisa la zona horaria. Todas las horas son mostradas en su hora local.</div></div><div class="table-responsive mt-4"><table class="table table-bordered table-striped text-center"><thead><tr class="bg-primary text-white"><th>Curso</th><th>Día</th><th>Hora</th><th>Enlace</th></tr></thead><tbody><tr><td>Desarrollo Web (Módulo CSS)</td><td>Miércoles</td><td>19:00 - 20:30 (GMT-5)</td><td><a href="#" class="btn btn-sm btn-success">Unirse <i class="fas fa-video ms-1"></i></a></td></tr><tr><td>Marketing Digital (Sesión Q&A)</td><td>Viernes</td><td>17:00 - 18:00 (GMT-5)</td><td><span class="badge bg-danger">Sesión Pasada</span></td></tr><tr><td>Excel para Principiantes</td><td>Jueves</td><td>20:00 - 21:00 (GMT-5)</td><td><a href="#" class="btn btn-sm btn-success">Unirse <i class="fas fa-video ms-1"></i></a></td></tr></tbody></table></div>`;
}

// ============================================
// FUNCIONES DE HISTORIAL Y FILTROS
// ============================================

function renderHistorialTable(data) {
  const tbody = document.getElementById('historialTableBody');
  const noDataMessage = document.getElementById('noDataMessage');
  if (!tbody || !noDataMessage) return;
  tbody.innerHTML = '';
  if (!data || data.length === 0) {
    noDataMessage.classList.remove('d-none');
    return;
  }
  noDataMessage.classList.add('d-none');
  data.forEach(item => {
    const row = document.createElement('tr');
    row.innerHTML = `<td>${item.curso}</td><td>${item.categoria}</td><td>${item.fecha}</td><td><span class="badge ${getPaymentBadgeClass(item.metodoPago)}">${item.metodoPago}</span></td><td>${item.instructor}</td>`;
    tbody.appendChild(row);
  });
}

function getPaymentBadgeClass(method) {
  switch (method) {
    case 'Tarjeta': return 'bg-success';
    case 'PayPal': return 'bg-primary';
    case 'Apple Pay': return 'bg-dark';
    case 'Google Pay': return 'bg-info text-dark';
    default: return 'bg-secondary';
  }
}

function initHistorialFilters() {
  const applyBtn = document.getElementById('applyFilters');
  const clearBtn = document.getElementById('clearFilters');
  if (applyBtn) applyBtn.removeEventListener('click', applyFilters);
  if (clearBtn) clearBtn.removeEventListener('click', clearFilters);
  if (applyBtn) applyBtn.addEventListener('click', applyFilters);
  if (clearBtn) clearBtn.addEventListener('click', clearFilters);
  renderHistorialTable(historialData);
}

function applyFilters() {
  const curso = (document.getElementById('filterCurso')?.value || '').toLowerCase();
  const categoria = (document.getElementById('filterCategoria')?.value || '').toLowerCase();
  const fecha = document.getElementById('filterFecha')?.value || '';
  const metodoPago = (document.getElementById('filterMetodoPago')?.value || '').toLowerCase();
  const docente = (document.getElementById('filterDocente')?.value || '').toLowerCase();
  const buscar = (document.getElementById('filterBuscar')?.value || '').toLowerCase();

  const filtered = historialData.filter(item => {
    const [day, month, year] = (item.fecha || '').split('/');
    const itemDate = year && month && day ? `${year}-${month}-${day}` : '';
    const matchesSearch = buscar === '' || (
      (item.curso || '').toLowerCase().includes(buscar) ||
      (item.categoria || '').toLowerCase().includes(buscar) ||
      (item.instructor || '').toLowerCase().includes(buscar)
    );
    return (curso === '' || (item.curso || '').toLowerCase().includes(curso)) &&
           (categoria === '' || (item.categoria || '').toLowerCase().includes(categoria)) &&
           (fecha === '' || itemDate === fecha) &&
           (metodoPago === '' || (item.metodoPago || '').toLowerCase().includes(metodoPago)) &&
           (docente === '' || (item.instructor || '').toLowerCase().includes(docente)) &&
           matchesSearch;
  });
  renderHistorialTable(filtered);
}

function clearFilters() {
  if (document.getElementById('filterCurso')) document.getElementById('filterCurso').value = '';
  if (document.getElementById('filterCategoria')) document.getElementById('filterCategoria').value = '';
  if (document.getElementById('filterFecha')) document.getElementById('filterFecha').value = '';
  if (document.getElementById('filterMetodoPago')) document.getElementById('filterMetodoPago').value = '';
  if (document.getElementById('filterDocente')) document.getElementById('filterDocente').value = '';
  if (document.getElementById('filterBuscar')) document.getElementById('filterBuscar').value = '';
  renderHistorialTable(historialData);
}

// ============================================
// FUNCIONES DE NAVEGACIÓN
// ============================================

function switchContent(viewName) {
  const container = document.getElementById('dynamicContentContainer');
  if (!container) return;
  const views = {
    historial: { html: getHistorialHTML, init: initHistorialFilters },
    cursos: { html: getCursosHTML, init: () => {} },
    materiales: { html: getMaterialesHTML, init: () => {} },
    horarios: { html: getHorariosHTML, init: () => {} }
  };
  const view = views[viewName];
  if (!view) return;
  container.innerHTML = view.html();
  setTimeout(view.init, 0);
  updateSidebarActiveLink(viewName);
}

function updateSidebarActiveLink(activeView) {
  const links = document.querySelectorAll('#sidebarNavLinks .list-group-item');
  links.forEach(link => {
    if (link.getAttribute('data-view') === activeView) link.classList.add('active-menu-link');
    else link.classList.remove('active-menu-link');
  });
}

// ============================================
// INICIALIZACIÓN
// ============================================

async function init() {
  console.log('🚀 Init de perfil.js iniciado');
  
  // Dar MUCHO tiempo a que el DOM cargue completamente (aumentado a 500ms)
  await new Promise(resolve => setTimeout(resolve, 500));
  
  console.log('⏳ DOM está listo, cargando datos del usuario...');
  
  try {
    await loadUserData();
    console.log('✓ loadUserData completada');
  } catch (e) {
    console.error('Error en loadUserData:', e);
  }
  
  // Esperar un poco más después de cargar usuario
  await new Promise(resolve => setTimeout(resolve, 300));
  
  console.log('📄 Cargando contenido de historial...');
  switchContent('historial');

  const sidebarLinks = document.querySelectorAll('#sidebarNavLinks .list-group-item');
  console.log('🔗 Sidebar links encontrados:', sidebarLinks.length);
  
  sidebarLinks.forEach(link => {
    link.addEventListener('click', (e) => {
      e.preventDefault();
      const view = link.getAttribute('data-view');
      if (view) {
        switchContent(view);
        const offcanvasEl = document.getElementById('sidebarOffcanvas');
        try {
          const bsOffcanvas = bootstrap.Offcanvas.getInstance(offcanvasEl);
          if (bsOffcanvas) bsOffcanvas.hide();
        } catch (e) {}
      }
    });
  });

  const logoutBtn = document.getElementById('logoutBtn');
  const logoutDropdownBtn = document.getElementById('logoutDropdownBtn');
  
  const doLogout = () => {
    console.log('🔓 Cerrando sesión del usuario...');
    // Limpiar localStorage
    localStorage.removeItem('token');
    localStorage.removeItem('currentUser');
    console.log('✓ Sesión cerrada - localStorage limpiado');
    // Redirigir a login
    alert('Sesión cerrada correctamente');
    window.location.href = 'reseccion.html';
  };
  
  if (logoutBtn) {
    logoutBtn.addEventListener('click', (e) => {
      e.preventDefault();
      const confirmLogout = confirm('¿Estás seguro de que deseas cerrar sesión?');
      if (confirmLogout) {
        doLogout();
      }
    });
  }
  
  if (logoutDropdownBtn) {
    logoutDropdownBtn.addEventListener('click', (e) => {
      e.preventDefault();
      const confirmLogout = confirm('¿Estás seguro de que deseas cerrar sesión?');
      if (confirmLogout) {
        doLogout();
      }
    });
  }
  
  console.log('✓ Init completado');
}

// Iniciar la aplicación al cargar el DOM
// Usar DOMContentLoaded para asegurar que el DOM esté completamente listo
document.addEventListener('DOMContentLoaded', function() {
  console.log('📄 DOMContentLoaded disparado');
  init();
});
