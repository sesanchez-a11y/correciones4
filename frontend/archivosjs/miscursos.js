document.addEventListener('DOMContentLoaded', function() {
    // Obtener cursos almacenados en localStorage
    const cursos = JSON.parse(localStorage.getItem('misCursos')) || [];

    // Renderizar cursos en la página con formato de list-group e ícono
    const renderCursos = () => {
        const container = document.getElementById('cursosContainer');
        if (!container) return;

        if (cursos.length === 0) {
            container.innerHTML = '<div class="list-group-item text-center text-muted">No tienes cursos registrados.</div>';
            return;
        }

        container.innerHTML = cursos.map(curso => `
            <a href="#" class="list-group-item list-group-item-action bg-yellow d-flex align-items-center fw-medium mb-2">
                <i class="fas fa-chalkboard-teacher me-3 text-dark"></i>
                <div>
                    <div class="fw-bold">${curso.nombre}</div>
                    <div class="small text-muted">Docente: ${curso.docente} &nbsp;|&nbsp; Precio: $${curso.precio}</div>
                </div>
            </a>
        `).join('');
    };

    // Agregar un nuevo curso (si llega desde la confirmación de pago)
    const params = new URLSearchParams(window.location.search);
    if (params.get('curso') && params.get('docente') && params.get('precio')) {
        const nuevoCurso = {
            nombre: params.get('curso'),
            docente: params.get('docente'),
            precio: params.get('precio')
        };
        cursos.push(nuevoCurso);
        localStorage.setItem('misCursos', JSON.stringify(cursos));
    }

    // Renderizar los cursos al cargar la página
    renderCursos();
});