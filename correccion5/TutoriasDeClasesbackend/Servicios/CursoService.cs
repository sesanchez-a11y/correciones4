using TutoriasDeClases.Interfaces;
using TutoriasDeClases.Modelos;

namespace TutoriasDeClases.Servicios
{
    /// <summary>
    /// Servicio que encapsula la lógica de negocio de cursos.
    /// Cumple con SRP (Single Responsibility Principle): solo maneja lógica de cursos.
    /// </summary>
    public class CursoService : ICursoService
    {
        private readonly ICursoRepository _cursoRepo;
        private readonly IUsuarioRepository _usuarioRepo;

        public CursoService(ICursoRepository cursoRepo, IUsuarioRepository usuarioRepo)
        {
            _cursoRepo = cursoRepo;
            _usuarioRepo = usuarioRepo;
        }

        /// <summary>
        /// Valida y crea un nuevo curso
        /// </summary>
        public async Task<(bool Success, string Message, string CursoId)> CrearCursoAsync(
            string nombre, string descripcion, string categoria, decimal precio,
            string nivel, int duracion, int capacidad, List<string> temario,
            string tutorId, string tutorNombre)
        {
            // Validaciones de negocio
            var validacion = ValidarDatosCurso(nombre, descripcion, precio, duracion, capacidad);
            if (!validacion.IsValid)
            {
                return (false, validacion.ErrorMessage, string.Empty);
            }

            var curso = new Curso
            {
                Nombre = nombre,
                Descripcion = descripcion,
                Categoria = categoria,
                Precio = precio,
                Nivel = nivel,
                Duracion = duracion,
                Capacidad = capacidad,
                Temario = temario ?? new List<string>(),
                TutorId = tutorId,
                TutorNombre = tutorNombre,
                Estado = "Pendiente",
                FechaCreacion = DateTime.UtcNow
            };

            var cursoId = await _cursoRepo.AddAsync(curso);
            return (true, "Curso creado exitosamente", cursoId);
        }

        /// <summary>
        /// Valida los datos básicos de un curso
        /// </summary>
        public (bool IsValid, string ErrorMessage) ValidarDatosCurso(
            string nombre, string descripcion, decimal precio, int duracion, int capacidad)
        {
            if (string.IsNullOrWhiteSpace(nombre))
                return (false, "El nombre del curso es requerido");

            if (nombre.Length < 3)
                return (false, "El nombre del curso debe tener al menos 3 caracteres");

            if (string.IsNullOrWhiteSpace(descripcion))
                return (false, "La descripción del curso es requerida");

            if (precio < 0)
                return (false, "El precio no puede ser negativo");

            if (duracion <= 0)
                return (false, "La duración debe ser mayor a 0 horas");

            if (capacidad <= 0)
                return (false, "La capacidad debe ser al menos 1 estudiante");

            if (capacidad > 100)
                return (false, "La capacidad no puede exceder 100 estudiantes");

            return (true, string.Empty);
        }

        /// <summary>
        /// Inscribe un estudiante a un curso
        /// </summary>
        public async Task<(bool Success, string Message)> InscribirEstudianteAsync(string cursoId, string estudianteId)
        {
            var curso = await _cursoRepo.GetByIdAsync(cursoId);
            if (curso == null)
                return (false, "Curso no encontrado");

            // Validar que el curso esté aprobado
            if (curso.Estado != "Aprobado")
                return (false, "Solo se puede inscribir a cursos aprobados");

            // Validar capacidad
            if (curso.Estudiantes.Count >= curso.Capacidad)
                return (false, "El curso está lleno. No hay cupos disponibles");

            // Validar que el estudiante no esté ya inscrito
            if (curso.Estudiantes.Contains(estudianteId))
                return (false, "El estudiante ya está inscrito en este curso");

            // Inscribir estudiante
            curso.Estudiantes.Add(estudianteId);
            await _cursoRepo.UpdateAsync(curso);

            return (true, "Inscripción exitosa");
        }

        /// <summary>
        /// Verifica si un curso tiene cupos disponibles
        /// </summary>
        public async Task<bool> TieneCuposDisponiblesAsync(string cursoId)
        {
            var curso = await _cursoRepo.GetByIdAsync(cursoId);
            if (curso == null)
                return false;

            return curso.Estudiantes.Count < curso.Capacidad;
        }

        /// <summary>
        /// Obtiene el número de cupos disponibles en un curso
        /// </summary>
        public async Task<int> ObtenerCuposDisponiblesAsync(string cursoId)
        {
            var curso = await _cursoRepo.GetByIdAsync(cursoId);
            if (curso == null)
                return 0;

            return Math.Max(0, curso.Capacidad - curso.Estudiantes.Count);
        }

        /// <summary>
        /// Aprueba un curso y realiza las acciones necesarias
        /// </summary>
        public async Task<(bool Success, string Message)> AprobarCursoAsync(string cursoId)
        {
            var curso = await _cursoRepo.GetByIdAsync(cursoId);
            if (curso == null)
                return (false, "Curso no encontrado");

            if (curso.Estado == "Aprobado")
                return (false, "El curso ya está aprobado");

            curso.Estado = "Aprobado";
            curso.FechaAprobacion = DateTime.UtcNow;
            await _cursoRepo.UpdateAsync(curso);

            return (true, "Curso aprobado exitosamente");
        }

        /// <summary>
        /// Rechaza un curso
        /// </summary>
        public async Task<(bool Success, string Message)> RechazarCursoAsync(string cursoId)
        {
            var curso = await _cursoRepo.GetByIdAsync(cursoId);
            if (curso == null)
                return (false, "Curso no encontrado");

            if (curso.Estado == "Rechazado")
                return (false, "El curso ya está rechazado");

            curso.Estado = "Rechazado";
            await _cursoRepo.UpdateAsync(curso);

            return (true, "Curso rechazado");
        }

        /// <summary>
        /// Obtiene todos los cursos aprobados
        /// </summary>
        public async Task<List<Curso>> ObtenerCursosAprobadosAsync()
        {
            return await _cursoRepo.GetAprobadosAsync();
        }

        /// <summary>
        /// Obtiene un curso por su ID
        /// </summary>
        public async Task<Curso> ObtenerCursoPorIdAsync(string cursoId)
        {
            return await _cursoRepo.GetByIdAsync(cursoId);
        }

        /// <summary>
        /// Obtiene todos los cursos de un tutor
        /// </summary>
        public async Task<List<Curso>> ObtenerCursosPorTutorAsync(string tutorId)
        {
            return await _cursoRepo.GetByTutorIdAsync(tutorId);
        }

        /// <summary>
        /// Obtiene todos los cursos (para admin)
        /// </summary>
        public async Task<List<Curso>> ObtenerTodosCursosAsync()
        {
            return await _cursoRepo.GetAllAsync();
        }
    }
}
