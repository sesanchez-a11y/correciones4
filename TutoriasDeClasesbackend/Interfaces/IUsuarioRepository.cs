using System.Threading.Tasks;
using TutoriasDeClases.Modelos;

namespace TutoriasDeClases.Interfaces
{
    public interface IUsuarioRepository
    {
        Task<string> AddAsync(Usuario user);
        Task<Usuario?> FindByEmailAsync(string email);
        Task<Usuario?> ValidateCredentialsAsync(string email, string password);
    }
}
