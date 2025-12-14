using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using MongoDB.Bson;
using MongoDB.Driver;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using System.Linq;

namespace TutoriasDeClases.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class DebugController : ControllerBase
    {
        private readonly IConfiguration _config;

        public DebugController(IConfiguration config)
        {
            _config = config;
        }

        [HttpGet("userByEmail")]
        public async Task<IActionResult> GetUserByEmail([FromQuery] string email)
        {
            if (string.IsNullOrEmpty(email)) return BadRequest(new { message = "email es requerido" });

            var conn = _config.GetConnectionString("MongoDbConnection") ?? "mongodb://localhost:27017";
            var dbName = _config.GetValue<string>("Mongo:Database") ?? "EduMentor";

            try
            {
                var client = new MongoClient(conn);
                var db = client.GetDatabase(dbName);
                var collection = db.GetCollection<BsonDocument>("Usuarios");
                var filter = Builders<BsonDocument>.Filter.Eq("correo", email);
                var doc = await collection.Find(filter).FirstOrDefaultAsync();
                if (doc == null) return NotFound(new { message = "Usuario no encontrado en MongoDB" });
                return Ok(doc.ToJson());
            }
            catch (System.Exception ex)
            {
                return StatusCode(500, new { message = "Error al consultar MongoDB", detail = ex.Message });
            }
        }

        [Authorize(Roles = "Admin")]
        [HttpGet("admin/allUsers")]
        public async Task<IActionResult> GetAllUsers()
        {
            var conn = _config.GetConnectionString("MongoDbConnection") ?? "mongodb://localhost:27017";
            var dbName = _config.GetValue<string>("Mongo:Database") ?? "EduMentor";

            try
            {
                var client = new MongoClient(conn);
                var db = client.GetDatabase(dbName);
                var collection = db.GetCollection<BsonDocument>("Usuarios");
                var docs = await collection.Find(Builders<BsonDocument>.Filter.Empty).ToListAsync();

                var users = docs.Select(d => new {
                    id = d.Contains("_id") ? d["_id"].ToString() : string.Empty,
                    nombre = d.Contains("nombre") ? d["nombre"].AsString : string.Empty,
                    apellido = d.Contains("apellido") ? d["apellido"].AsString : string.Empty,
                    correo = d.Contains("correo") ? d["correo"].AsString : string.Empty,
                    rol = d.Contains("rol") ? d["rol"].AsString : string.Empty,
                    fechaCreacion = d.Contains("fechaCreacion") ? d["fechaCreacion"].ToString() : string.Empty
                }).ToList();

                return Ok(new { users = users });
            }
            catch (System.Exception ex)
            {
                return StatusCode(500, new { message = "Error al consultar MongoDB", detail = ex.Message });
            }
        }
    }
}
