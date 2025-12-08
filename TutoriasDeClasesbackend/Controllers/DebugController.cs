using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using MongoDB.Bson;
using MongoDB.Driver;
using System.Threading.Tasks;

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
    }
}
