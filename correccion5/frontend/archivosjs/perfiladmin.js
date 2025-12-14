const API_BASE_URL = 'http://localhost:5000/api';

async function ensureAdmin() {
  const token = localStorage.getItem('token');
  if (!token) {
    // Si no hay token, mostramos un pequeño panel para pegar el token manualmente (útil para pruebas)
    const container = document.getElementById('adminTokenContainer');
    if (container) {
      container.innerHTML = `
        <div class="alert alert-warning">Acceso restringido. Pega un token válido o inicia sesión.</div>
        <div class="input-group mb-3">
          <input id="manualTokenInput" type="text" class="form-control" placeholder="Pega aquí el token JWT" />
          <button id="manualTokenBtn" class="btn btn-primary">Usar token</button>
        </div>
        <div><a href="./reseccion.html">Ir a iniciar sesión</a></div>
      `;
      document.getElementById('manualTokenBtn').addEventListener('click', () => {
        const v = document.getElementById('manualTokenInput').value.trim();
        if (v) { localStorage.setItem('token', v); window.location.reload(); }
      });
      return false;
    }

    alert('Acceso restringido. Inicia sesión como administrador.');
    window.location.href = './reseccion.html';
    return false;
  }
  try {
    const resp = await fetch(`${API_BASE_URL}/ControladorDeSesion/me`, { headers: { 'Authorization': `Bearer ${token}` } });
    if (!resp.ok) {
      const txt = await resp.text();
      console.error('Me() no autorizado:', resp.status, txt);
      throw new Error('No autorizado');
    }
    const user = await resp.json();
    if (!user || (user.rol || '').toLowerCase() !== 'admin') {
      alert('Acceso reservado a administradores.');
      window.location.href = './perfil.html';
      return false;
    }
    return true;
  } catch (e) {
    console.error('Error comprobando admin:', e);
    localStorage.removeItem('token');
    // Mostrar mensaje y ofrecer volver a iniciar sesión
    const container = document.getElementById('adminTokenContainer');
    if (container) container.innerHTML = '<div class="alert alert-danger">Token inválido o expirado. <a href="./reseccion.html">Inicia sesión</a> o pega un token válido.</div>';
    else window.location.href = './reseccion.html';
    return false;
  }
}

async function loadPendingCourses() {
  const token = localStorage.getItem('token');
  const container = document.getElementById('pendingCoursesContainer');
  container.innerHTML = 'Cargando...';
  try {
    const r = await fetch(`${API_BASE_URL}/Cursos/admin/todos`, { headers: { 'Authorization': `Bearer ${token}` } });
    if (!r.ok) throw new Error('No autorizado');
    const data = await r.json();
    const cursos = data.cursos || [];
    const pendientes = cursos.filter(c => (c.Estado || c.estado || '').toLowerCase() === 'pendiente');
    if (pendientes.length === 0) {
      container.innerHTML = '<div class="alert alert-info">No hay solicitudes pendientes.</div>';
      return;
    }
    const list = pendientes.map(c => {
      const id = c.Id || c.id || c._id || '';
      const nombre = c.Nombre || c.nombre || c.Nombre || 'Sin nombre';
      const tutor = c.TutorNombre || c.tutorNombre || c.tutor || 'Desconocido';
      return `
        <div class="card mb-2 p-2">
          <div class="d-flex justify-content-between align-items-start">
            <div>
              <h6 class="mb-1">${nombre}</h6>
              <small class="text-muted">Tutor: ${tutor}</small>
            </div>
            <div>
              <button class="btn btn-sm btn-success me-1" onclick="approveCourse('${id}')">Aprobar</button>
              <button class="btn btn-sm btn-danger" onclick="rejectCourse('${id}')">Rechazar</button>
            </div>
          </div>
        </div>`;
    }).join('');
    container.innerHTML = list;
  } catch (e) {
    console.error(e);
    container.innerHTML = '<div class="alert alert-danger">Error al cargar solicitudes.</div>';
  }
}

async function approveCourse(id) {
  if (!confirm('Confirmar aprobación del curso?')) return;
  const token = localStorage.getItem('token');
  try {
    const r = await fetch(`${API_BASE_URL}/Cursos/admin/${id}/aprobar`, { method: 'PUT', headers: { 'Authorization': `Bearer ${token}` } });
    if (!r.ok) { alert('Error al aprobar'); return; }
    alert('Curso aprobado');
    loadPendingCourses();
    loadUsers();
  } catch (e) { console.error(e); alert('Error'); }
}

async function rejectCourse(id) {
  if (!confirm('Confirmar rechazo del curso?')) return;
  const token = localStorage.getItem('token');
  try {
    const r = await fetch(`${API_BASE_URL}/Cursos/admin/${id}/rechazar`, { method: 'PUT', headers: { 'Authorization': `Bearer ${token}` } });
    if (!r.ok) { alert('Error al rechazar'); return; }
    alert('Curso rechazado');
    loadPendingCourses();
  } catch (e) { console.error(e); alert('Error'); }
}

async function loadUsers() {
  const token = localStorage.getItem('token');
  const container = document.getElementById('usersContainer');
  container.innerHTML = 'Cargando...';
  try {
    const r = await fetch(`${API_BASE_URL}/Debug/admin/allUsers`, { headers: { 'Authorization': `Bearer ${token}` } });
    if (!r.ok) throw new Error('No autorizado');
    const data = await r.json();
    const users = data.users || [];
    if (users.length === 0) { container.innerHTML = '<div class="alert alert-info">No hay usuarios registrados.</div>'; return; }
    const rows = users.map(u => `<tr><td>${u.nombre}</td><td>${u.apellido}</td><td>${u.correo}</td><td>${u.rol}</td></tr>`).join('');
    container.innerHTML = `<div class="table-responsive"><table class="table table-sm"><thead><tr><th>Nombre</th><th>Apellido</th><th>Correo</th><th>Rol</th></tr></thead><tbody>${rows}</tbody></table></div>`;
  } catch (e) {
    console.error(e);
    container.innerHTML = '<div class="alert alert-danger">Error al cargar usuarios.</div>';
  }
}

document.addEventListener('DOMContentLoaded', async () => {
  const ok = await ensureAdmin();
  if (!ok) return;
  document.getElementById('logoutBtn').addEventListener('click', () => { localStorage.removeItem('token'); localStorage.removeItem('currentUser'); window.location.href = './reseccion.html'; });
  await loadPendingCourses();
  await loadUsers();
});
