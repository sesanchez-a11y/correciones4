using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace TutoriasDeClases.Modelos
{
    // Clase para almacenar usuarios en MongoDB (Estudiante y Tutor)
    public class Usuario
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; } = string.Empty;

        [BsonElement("nombre")]
        public string Nombre { get; set; } = string.Empty;

        [BsonElement("apellido")]
        public string Apellido { get; set; } = string.Empty;

        [BsonElement("correo")]
        public string Correo { get; set; } = string.Empty;
        
        // --- Campos Agregados del Formulario ---
        [BsonElement("edad")]
        public int Edad { get; set; }
        
        [BsonElement("especializacion")]
        public string Especializacion { get; set; } = string.Empty;

        [BsonElement("rol")]
        public string Rol { get; set; } = string.Empty; // "Alumno" o "Tutor"
        
        // Es crucial para el inicio de sesión
        [BsonElement("contrasenaHash")]
        public string ContrasenaHash { get; set; } = string.Empty; // ¡Siempre hashear la contraseña!
        // ---------------------------------------
    }
}