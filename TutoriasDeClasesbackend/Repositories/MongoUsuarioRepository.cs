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

        public async Task<string> AddAsync(Usuario user)
        {
            // Hashear la contraseña antes de guardar
            user.ContrasenaHash = BCrypt.Net.BCrypt.HashPassword(user.ContrasenaHash);
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
            if (BCrypt.Net.BCrypt.Verify(password, user.ContrasenaHash))
                return user;
            return null;
        }
    }
}
