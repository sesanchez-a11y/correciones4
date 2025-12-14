using TutoriasDeClases.Modelos;

namespace TutoriasDeClases.Interfaces
{
    /// <summary>
    /// Interfaz para el servicio de cursos.
    /// Cumple con DIP (Dependency Inversion Principle): Las clases de alto nivel (controladores)
    /// dependen de esta abstracción en lugar de la implementación concreta.
    /// También cumple con ISP (Interface Segregation Principle): contiene solo métodos relacionados con cursos.
    /// </summary>
    public interface ICursoService
    {
        Task<(bool Success, string Message, string CursoId)> CrearCursoAsync(
            string nombre, string descripcion, string categoria, decimal precio,
            string nivel, int duracion, int capacidad, List<string> temario,
            string tutorId, string tutorNombre);

        (bool IsValid, string ErrorMessage) ValidarDatosCurso(
            string nombre, string descripcion, decimal precio, int duracion, int capacidad);

        Task<(bool Success, string Message)> InscribirEstudianteAsync(string cursoId, string estudianteId);

        Task<bool> TieneCuposDisponiblesAsync(string cursoId);

        Task<int> ObtenerCuposDisponiblesAsync(string cursoId);

        Task<(bool Success, string Message)> AprobarCursoAsync(string cursoId);

        Task<(bool Success, string Message)> RechazarCursoAsync(string cursoId);

        Task<List<Curso>> ObtenerCursosAprobadosAsync();

        Task<Curso> ObtenerCursoPorIdAsync(string cursoId);

        Task<List<Curso>> ObtenerCursosPorTutorAsync(string tutorId);

        Task<List<Curso>> ObtenerTodosCursosAsync();
    }
}
