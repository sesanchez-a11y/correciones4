using MongoDB.Driver;
using TutoriasDeClases.Interfaces;
using TutoriasDeClases.Modelos;

namespace TutoriasDeClases.Repositorios
{
    public class CursoRepository : ICursoRepository
    {
        private readonly IMongoCollection<Curso> _cursosCollection;

        public CursoRepository(IMongoClient mongoClient, IConfiguration config)
        {
            var dbName = config.GetValue<string>("MongoDB:DatabaseName") ?? "EduMentor";
            var database = mongoClient.GetDatabase(dbName);
            _cursosCollection = database.GetCollection<Curso>("cursos");
        }

        public async Task<string> AddAsync(Curso curso)
        {
            await _cursosCollection.InsertOneAsync(curso);
            return curso.Id;
        }

        public async Task<Curso> GetByIdAsync(string id)
        {
            var filter = Builders<Curso>.Filter.Eq(c => c.Id, id);
            return await _cursosCollection.Find(filter).FirstOrDefaultAsync();
        }

        public async Task<List<Curso>> GetAllAsync()
        {
            return await _cursosCollection.Find(Builders<Curso>.Filter.Empty).ToListAsync();
        }

        public async Task<List<Curso>> GetByTutorIdAsync(string tutorId)
        {
            var filter = Builders<Curso>.Filter.Eq(c => c.TutorId, tutorId);
            return await _cursosCollection.Find(filter).ToListAsync();
        }

        public async Task<List<Curso>> GetAprobadosAsync()
        {
            var filter = Builders<Curso>.Filter.Eq(c => c.Estado, "Aprobado");
            return await _cursosCollection.Find(filter).ToListAsync();
        }

        public async Task UpdateAsync(Curso curso)
        {
            var filter = Builders<Curso>.Filter.Eq(c => c.Id, curso.Id);
            await _cursosCollection.ReplaceOneAsync(filter, curso);
        }

        public async Task DeleteAsync(string id)
        {
            var filter = Builders<Curso>.Filter.Eq(c => c.Id, id);
            await _cursosCollection.DeleteOneAsync(filter);
        }
    }
}
