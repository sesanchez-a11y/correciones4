// notificaciones.js - versión ligera para páginas públicas
const API_BASE_URL = 'http://localhost:5000/api';

async function fetchMeLight(token) {
  try {
    const resp = await fetch(`${API_BASE_URL}/ControladorDeSesion/me`, {
      method: 'GET',
      headers: { 'Authorization': `Bearer ${token}` }
    });
    if (!resp.ok) return null;
    return await resp.json();
  } catch (e) {
    console.warn('fetchMeLight error', e);
    return null;
  }
}

function renderNotificationsNavbarLight(notificaciones) {
  const dropdown = document.getElementById('notificacionesDropdownList');
  const badge = document.getElementById('notificationsNavBadge');
  if (!dropdown) return;
  const noLeidas = (notificaciones || []).filter(n => !n.leida);
  dropdown.innerHTML = '';
  if (noLeidas.length === 0) {
    dropdown.innerHTML = '<li><div class="dropdown-item-text text-muted small">Sin notificaciones nuevas</div></li>';
    if (badge) badge.classList.add('d-none');
  } else {
    if (badge) { badge.textContent = noLeidas.length; badge.classList.remove('d-none'); }
    noLeidas.forEach(n => {
      const li = document.createElement('li');
      li.innerHTML = `<a href="#" class="dropdown-item small" data-id="${n.id}"><strong>${escapeHtml(n.mensaje.substring(0,40))}...</strong><br/><small class="text-muted">Marcar leído</small></a>`;
      dropdown.appendChild(li);
      const a = li.querySelector('a');
      a.addEventListener('click', async (e) => { e.preventDefault(); await markNotificationAsRead(n.id, li); });
    });
  }
}

function escapeHtml(str) {
  if (!str) return '';
  return str.replace(/&/g,'&amp;').replace(/</g,'&lt;').replace(/>/g,'&gt;');
}

async function markNotificationAsRead(id, el) {
  if (!id) return;
  const token = localStorage.getItem('token');
  if (!token) return;
  try {
    const resp = await fetch(`${API_BASE_URL}/ControladorDeSesion/notifications/${id}/read`, {
      method: 'PUT',
      headers: { 'Authorization': `Bearer ${token}` }
    });
    if (resp.ok) {
      if (el) el.remove();
      const badge = document.getElementById('notificationsNavBadge');
      if (badge) {
        const cur = parseInt(badge.textContent||'0');
        const next = Math.max(0, cur-1);
        if (next === 0) badge.classList.add('d-none');
        badge.textContent = next;
      }
    }
  } catch (e) { console.warn('markNotificationAsRead', e); }
}

let __srConnLight = null;
async function initSignalRLight(token) {
  try {
    const base = API_BASE_URL.replace(/\/api\/?$/,'');
    __srConnLight = new signalR.HubConnectionBuilder()
      .withUrl(`${base}/hubs/notifications`, { accessTokenFactory: () => token })
      .withAutomaticReconnect()
      .build();

    __srConnLight.on('ReceiveNotification', (n) => {
      try {
        const dropdown = document.getElementById('notificacionesDropdownList');
        const badge = document.getElementById('notificationsNavBadge');
        if (!dropdown) return;
        const li = document.createElement('li');
        li.innerHTML = `<a href="#" class="dropdown-item small" data-id="${n.id}"><strong>${escapeHtml(n.mensaje.substring(0,40))}...</strong><br/><small class="text-muted">Marcar leído</small></a>`;
        dropdown.prepend(li);
        const a = li.querySelector('a');
        a.addEventListener('click', async (e) => { e.preventDefault(); await markNotificationAsRead(n.id, li); });
        if (badge) { badge.classList.remove('d-none'); badge.textContent = (parseInt(badge.textContent||'0') + 1); }
      } catch (e) { console.warn('ReceiveNotification light error', e); }
    });

    await __srConnLight.start();
    console.log('SignalR (light) conectado');
  } catch (e) {
    console.warn('No se pudo conectar SignalR (light):', e);
  }
}

// Inicializar sólo si hay token (no redirigir si no hay)
document.addEventListener('DOMContentLoaded', async () => {
  const token = localStorage.getItem('token');
  if (!token) return; // no autenticado -> no hacemos nada
  const me = await fetchMeLight(token);
  if (!me) return;
  // inyectar menú de usuario en la navbar y ocultar el enlace de login
  try { injectUserNav(me); } catch (e) { console.warn('injectUserNav error', e); }
  renderNotificationsNavbarLight(me.notificaciones || []);
  try { await initSignalRLight(token); } catch (e) { console.warn(e); }
});

function injectUserNav(me) {
  const container = document.getElementById('navUtilitiesContainer');
  const loginContainer = document.getElementById('navLoginContainer');
  if (!container) return;
  // ocultar login
  if (loginContainer) loginContainer.style.display = 'none';

  const html = `
    <ul class="navbar-nav utility-links d-flex flex-row align-items-center">
      <li class="nav-item me-3 dropdown">
        <a class="nav-link dropdown-toggle position-relative" href="#" id="notificacionesDropdown" role="button"
          data-bs-toggle="dropdown" aria-expanded="false" title="Notificaciones">
          NOTIFICACIONES
          <span id="notificationsNavBadge" class="position-absolute badge rounded-pill bg-danger d-none">0</span>
        </a>
        <ul class="dropdown-menu dropdown-menu-end shadow-lg" style="min-width: 300px;" aria-labelledby="notificacionesDropdown" id="notificacionesDropdownList">
          <li><div class="dropdown-item-text text-muted small">Cargando notificaciones...</div></li>
        </ul>
      </li>

      <li class="nav-item me-3"><a class="nav-link" href="cursos.html">Cursos</a></li>

      <li class="nav-item dropdown">
        <a class="nav-link dropdown-toggle" href="#" id="perfilDropdown" role="button" data-bs-toggle="dropdown" aria-expanded="false">
          <i class="fas fa-user-circle me-1"></i> ${escapeHtml((me.nombre||'Usuario').toString())}
        </a>
        <ul class="dropdown-menu dropdown-menu-end shadow-lg" aria-labelledby="perfilDropdown">
          <li><a class="dropdown-item" href="perfil.html">Mi perfil</a></li>
          <li><a class="dropdown-item" href="miscursos.html">Mis cursos</a></li>
          <li><hr class="dropdown-divider"></li>
          <li><a class="dropdown-item text-danger" href="#" id="logoutDropdownBtn">Cerrar sesión</a></li>
        </ul>
      </li>
    </ul>
  `;

  container.innerHTML = html;

  // agregar handler de logout
  const logoutBtn = document.getElementById('logoutDropdownBtn');
  if (logoutBtn) {
    logoutBtn.addEventListener('click', (e) => {
      e.preventDefault();
      localStorage.removeItem('token');
      localStorage.removeItem('currentUser');
      // volver a mostrar login
      if (loginContainer) loginContainer.style.display = '';
      // limpiar utilidades
      const container = document.getElementById('navUtilitiesContainer');
      if (container) container.innerHTML = '';
      // opcional: redirigir
      window.location.href = 'reseccion.html';
    });
  }
}
