using System.Threading.Tasks;
using TutoriasDeClases.Modelos;

namespace TutoriasDeClases.Interfaces
{
    public interface IUsuarioRepository
    {
        Task<string> AddAsync(Usuario user);
        Task<Usuario?> FindByEmailAsync(string email);
        Task<Usuario?> ValidateCredentialsAsync(string email, string password);
        Task AddNotificationAsync(string userId, Notificacion notificacion);
        Task RemoveNotificationAsync(string userId, string notificationId);
        Task MarkNotificationAsReadAsync(string userId, string notificationId);
    }
}
