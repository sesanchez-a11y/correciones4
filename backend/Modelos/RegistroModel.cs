namespace TutoriasDeClases.Modelos
{
    // Este modelo se usa para mapear los datos que vienen del formulario del lado del cliente
    public class RegistroModel
    {
        public string? Rol { get; set; } // "Alumno" o "Tutor"
        public string? Email { get; set; }
        public string? Nombre { get; set; }
        public string? Apellido { get; set; }
        public int Edad { get; set; }
        public string? Especializacion { get; set; }
        public string? Contrasena { get; set; } // Nota: La contraseña debe ser hasheada en un entorno real.
    }
}