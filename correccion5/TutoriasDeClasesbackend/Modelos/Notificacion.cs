using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace TutoriasDeClases.Modelos
{
    public class Notificacion
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; } = string.Empty;

        [BsonElement("mensaje")]
        public string Mensaje { get; set; } = string.Empty;
        
        [BsonElement("leida")]
        public bool Leida { get; set; } = false;

        [BsonElement("fechaLeido")]
        public DateTime? FechaLeido { get; set; }
    }
}