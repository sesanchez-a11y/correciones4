using System.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;
using TutoriasDeClases.Interfaces;
using TutoriasDeClases.Modelos;
using MongoDB.Bson;

namespace TutoriasDeClases.Repositories
{
    public class InMemoryUsuarioRepository : IUsuarioRepository
    {
        private readonly List<Usuario> _users = new();
        private readonly object _lock = new();

        public Task<string> AddAsync(Usuario user)
        {
            // Hashear la contraseña
            user.ContrasenaHash = BCrypt.Net.BCrypt.HashPassword(user.ContrasenaHash);
            user.Id = ObjectId.GenerateNewId().ToString();
            lock (_lock)
            {
                _users.Add(user);
            }
            return Task.FromResult(user.Id);
        }

        public Task<Usuario?> FindByEmailAsync(string email)
        {
            lock (_lock)
            {
                return Task.FromResult(_users.FirstOrDefault(u => u.Correo == email));
            }
        }

        public Task<Usuario?> ValidateCredentialsAsync(string email, string password)
        {
            lock (_lock)
            {
                var user = _users.FirstOrDefault(u => u.Correo == email);
                if (user == null) return Task.FromResult<Usuario?>(null);
                
                // Intentar verificar con BCrypt
                try
                {
                    if (BCrypt.Net.BCrypt.Verify(password, user.ContrasenaHash))
                        return Task.FromResult<Usuario?>(user);
                }
                catch
                {
                    // Fallback: comparación de texto plano para usuarios antiguos
                    if (user.ContrasenaHash == password)
                        return Task.FromResult<Usuario?>(user);
                }
                
                return Task.FromResult<Usuario?>(null);
            }
        }

        public Task AddNotificationAsync(string userId, Notificacion notificacion)
        {
            lock (_lock)
            {
                var user = _users.FirstOrDefault(u => u.Id == userId);
                if (user != null)
                {
                    if (user.Notificaciones == null) user.Notificaciones = new List<Notificacion>();
                    user.Notificaciones.Add(notificacion);
                }
            }
            return Task.CompletedTask;
        }

        public Task RemoveNotificationAsync(string userId, string notificationId)
        {
            lock (_lock)
            {
                var user = _users.FirstOrDefault(u => u.Id == userId);
                if (user != null && user.Notificaciones != null)
                {
                    user.Notificaciones.RemoveAll(n => n.Id == notificationId);
                }
            }
            return Task.CompletedTask;
        }

        public Task MarkNotificationAsReadAsync(string userId, string notificationId)
        {
            lock (_lock)
            {
                var user = _users.FirstOrDefault(u => u.Id == userId);
                if (user != null && user.Notificaciones != null)
                {
                    var noti = user.Notificaciones.FirstOrDefault(n => n.Id == notificationId);
                    if (noti != null)
                    {
                        noti.Leida = true;
                        noti.FechaLeido = DateTime.UtcNow;
                    }
                }
            }
            return Task.CompletedTask;
        }
    }
}
