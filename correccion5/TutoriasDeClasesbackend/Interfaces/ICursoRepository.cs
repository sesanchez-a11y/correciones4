using TutoriasDeClases.Modelos;

namespace TutoriasDeClases.Interfaces
{
    public interface ICursoRepository
    {
        Task<string> AddAsync(Curso curso);
        Task<Curso> GetByIdAsync(string id);
        Task<List<Curso>> GetAllAsync();
        Task<List<Curso>> GetByTutorIdAsync(string tutorId);
        Task<List<Curso>> GetAprobadosAsync();
        Task UpdateAsync(Curso curso);
        Task DeleteAsync(string id);
    }
}
