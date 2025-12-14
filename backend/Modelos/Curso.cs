using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace TutoriasDeClases.Modelos
{
    public class Curso
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }

        [BsonElement("nombre")]
        public string Nombre { get; set; }

        [BsonElement("descripcion")]
        public string Descripcion { get; set; }

        [BsonElement("categoria")]
        public string Categoria { get; set; }

        [BsonElement("precio")]
        public decimal Precio { get; set; }

        [BsonElement("nivel")]
        public string Nivel { get; set; } // Principiante, Intermedio, Avanzado

        [BsonElement("duracion")]
        public int Duracion { get; set; } // En horas

        [BsonElement("capacidad")]
        public int Capacidad { get; set; } // Máximo de estudiantes

        [BsonElement("temario")]
        public List<string> Temario { get; set; } = new List<string>();

        [BsonElement("tutorId")]
        public string TutorId { get; set; } // ID del usuario Tutor que creó el curso

        [BsonElement("tutorNombre")]
        public string TutorNombre { get; set; }

        [BsonElement("estado")]
        public string Estado { get; set; } = "Pendiente"; // Pendiente, Aprobado, Rechazado

        [BsonElement("fechaCreacion")]
        public DateTime FechaCreacion { get; set; } = DateTime.UtcNow;

        [BsonElement("fechaAprobacion")]
        public DateTime? FechaAprobacion { get; set; }

        [BsonElement("estudiantes")]
        public List<string> Estudiantes { get; set; } = new List<string>(); // IDs de estudiantes inscritos

        [BsonElement("calificacion")]
        public double Calificacion { get; set; } = 0; // Promedio de calificaciones
    }
}
