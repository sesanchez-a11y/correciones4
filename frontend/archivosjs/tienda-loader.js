// Cargar servicios (cursos aprobados) en tienda e inicio
const API_BASE_URL = 'http://localhost:5000/api';

async function loadServicios() {
  try {
    const resp = await fetch(`${API_BASE_URL}/Reservas/servicios`);
    if (!resp.ok) throw new Error(`Error ${resp.status}`);
    const data = await resp.json();
    return data.servicios || [];
  } catch (e) {
    console.error('Error cargando servicios:', e);
    return [];
  }
}

function renderCursosEnTienda(servicios) {
  const container = document.getElementById('cursosContainer');
  if (!container) return;

  if (servicios.length === 0) {
    container.innerHTML = '<div class="alert alert-info">No hay cursos disponibles en este momento.</div>';
    return;
  }

  let html = '<div class="row">';
  servicios.forEach(servicio => {
    const titulo = servicio.titulo || servicio.Titulo || servicio.titulo || servicio.Titulo || 'Curso';
    const precioVal = (servicio.precioBase !== undefined && servicio.precioBase !== null) ? servicio.precioBase : (servicio.PrecioBase !== undefined ? servicio.PrecioBase : 0);
    const idVal = servicio.id || servicio.Id || servicio._id || '';
    html += `
      <div class="col-md-4 mb-4">
        <div class="card shadow-sm h-100">
          <div class="card-body">
            <h5 class="card-title text-primary-custom">${escapeHtml(titulo)}</h5>
            <p class="card-text text-muted small">Precio: $${(Number(precioVal)||0).toFixed(2)}</p>
            <button class="btn btn-warning w-100 inscribirse-btn" data-id="${idVal}">
              <i class="fas fa-check-circle me-1"></i>Inscribirse
            </button>
          </div>
        </div>
      </div>
    `;
  });
  html += '</div>';
  container.innerHTML = html;

  // Agregar eventos de inscripción
  document.querySelectorAll('.inscribirse-btn').forEach(btn => {
    btn.addEventListener('click', async (e) => {
      e.preventDefault();
      const servicioId = btn.getAttribute('data-id');
      await registrarseAlCurso(servicioId, btn);
    });
  });
}

async function registrarseAlCurso(servicioId, btnEl) {
  const token = localStorage.getItem('token');
  if (!token) { alert('Por favor inicia sesión primero'); window.location.href = './reseccion.html'; return; }

  try {
    btnEl.disabled = true;
    const resp = await fetch(`${API_BASE_URL}/Reservas/crear`, {
      method: 'POST',
      headers: {
        'Content-Type': 'application/json',
        'Authorization': `Bearer ${token}`
      },
      body: JSON.stringify({ ServicioId: servicioId })
    });

    const result = await resp.json();
    if (resp.ok) {
      // Buscar los datos del curso seleccionado para pasarlos a pagos.html
      const servicios = await loadServicios();
      const curso = servicios.find(s => (s.id || s.Id || s._id) == servicioId);
      // Usar los campos de la tienda (nombre, tutorNombre, precio, id)
      const nombre = curso ? (curso.nombre || curso.titulo || curso.Titulo || 'Curso') : 'Curso';
      const docente = curso ? (curso.tutorNombre || curso.docente || curso.Docente || curso.profesor || curso.Profesor || '-') : '-';
      const precioVal = curso ? (curso.precio || curso.precioBase || curso.PrecioBase || 0) : 0;
      const id = curso ? (curso.id || curso.Id || curso._id || '') : '';
      // Redirigir a pagos.html con los datos correctos de la tienda
      window.location.href = `archivoshtml/pagos.html?id=${encodeURIComponent(id)}&curso=${encodeURIComponent(nombre)}&docente=${encodeURIComponent(docente)}&precio=${encodeURIComponent(precioVal)}`;
    } else {
      alert('Error: ' + (result.message || 'No se pudo inscribir'));
      btnEl.disabled = false;
    }
  } catch (e) {
    console.error('Error al inscribirse:', e);
    alert('Error de conexión');
    btnEl.disabled = false;
  }
}

function escapeHtml(str) {
  if (!str) return '';
  return str.replace(/&/g, '&amp;').replace(/</g, '&lt;').replace(/>/g, '&gt;');
}

// Auto-cargar al iniciar página
document.addEventListener('DOMContentLoaded', async () => {
  const servicios = await loadServicios();
  renderCursosEnTienda(servicios);
});
