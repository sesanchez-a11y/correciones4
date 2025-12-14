using System.Threading.Tasks;
using TutoriasDeClases.Interfaces;
using TutoriasDeClases.Modelos;
using MongoDB.Driver;
using BCrypt.Net;

namespace TutoriasDeClases.Repositories
{
    public class MongoUsuarioRepository : IUsuarioRepository
    {
        private readonly IMongoCollection<Usuario> _collection;

        public MongoUsuarioRepository(IMongoCollection<Usuario> collection)
        {
            _collection = collection;
        }

        public async Task AddNotificationAsync(string userId, Notificacion notificacion)
        {
            var update = Builders<Usuario>.Update.Push(u => u.Notificaciones, notificacion);
            await _collection.UpdateOneAsync(u => u.Id == userId, update);
        }

        public async Task RemoveNotificationAsync(string userId, string notificationId)
        {
            var update = Builders<Usuario>.Update.PullFilter(u => u.Notificaciones, Builders<Notificacion>.Filter.Eq(n => n.Id, notificationId));
            await _collection.UpdateOneAsync(u => u.Id == userId, update);
        }

        public async Task MarkNotificationAsReadAsync(string userId, string notificationId)
        {
            var filter = Builders<Usuario>.Filter.And(
                Builders<Usuario>.Filter.Eq(u => u.Id, userId),
                Builders<Usuario>.Filter.ElemMatch(u => u.Notificaciones, Builders<Notificacion>.Filter.Eq(n => n.Id, notificationId))
            );

            var update = Builders<Usuario>.Update
                .Set("notificaciones.$.leida", true)
                .Set("notificaciones.$.fechaLeido", DateTime.UtcNow);

            await _collection.UpdateOneAsync(filter, update);
        }

        public async Task<string> AddAsync(Usuario user)
        {
            // Evitar re-hashear si la contraseña ya viene en formato BCrypt
            var pwd = user.ContrasenaHash ?? string.Empty;
            if (!pwd.StartsWith("$2a$") && !pwd.StartsWith("$2b$") && !pwd.StartsWith("$2y$"))
            {
                user.ContrasenaHash = BCrypt.Net.BCrypt.HashPassword(pwd);
            }

            await _collection.InsertOneAsync(user);
            return user.Id ?? string.Empty;
        }

        public async Task<Usuario?> FindByEmailAsync(string email)
        {
            return await _collection.Find(u => u.Correo == email).FirstOrDefaultAsync();
        }

        public async Task<Usuario?> ValidateCredentialsAsync(string email, string password)
        {
            var user = await FindByEmailAsync(email);
            if (user == null) return null;
            
            // Intentar verificar con BCrypt (contraseña hasheada)
            try
            {
                if (BCrypt.Net.BCrypt.Verify(password, user.ContrasenaHash))
                    return user;
            }
            catch
            {
                // BCrypt.Verify() falla si el hash no es válido (ej: contraseña en texto plano)
                // Fallback: comparación de texto plano para usuarios antiguos
                if (user.ContrasenaHash == password)
                    return user;
            }
            
            return null;
        }
    }
}
