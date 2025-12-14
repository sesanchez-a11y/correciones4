using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using TutoriasDeClases.Interfaces;
using TutoriasDeClases.Modelos;
using Microsoft.AspNetCore.SignalR;
using TutoriasDeClases.Hubs;

namespace TutoriasDeClases.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CursosController : ControllerBase
    {
        private readonly ICursoRepository _cursoRepo;
        private readonly IUsuarioRepository _usuarioRepo;
        private readonly MongoDB.Driver.IMongoCollection<TutoriasDeClases.Modelos.Servicio> _servicios;
        private readonly IHubContext<NotificationHub> _hubContext;

        public CursosController(ICursoRepository cursoRepo, IUsuarioRepository usuarioRepo, MongoDB.Driver.IMongoCollection<TutoriasDeClases.Modelos.Servicio> servicios, IHubContext<NotificationHub> hubContext)
        {
            _cursoRepo = cursoRepo;
            _usuarioRepo = usuarioRepo;
            _servicios = servicios;
            _hubContext = hubContext;
        }

        /// <summary>
        /// Crear un nuevo curso (Admin y Tutores autenticados)
        /// </summary>
        [Authorize(Roles = "Admin,Tutor")]
        [HttpPost("crear")]
        public async Task<IActionResult> CrearCurso([FromBody] CrearCursoDto dto)
        {
            try
            {
                // Obtener el ID del usuario del JWT
                var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                var email = User.FindFirst(ClaimTypes.Email)?.Value;
                var roleClaim = User.FindFirst(ClaimTypes.Role)?.Value;

                Console.WriteLine($"🔐 Intento de crear curso - Email: {email}, Rol claim: {roleClaim}");

                if (string.IsNullOrEmpty(userId) || string.IsNullOrEmpty(email))
                    return Unauthorized(new { message = "No autorizado: usuario no identificado." });

                // Validar que el rol sea Tutor o Admin
                if (roleClaim != "Tutor" && roleClaim != "Admin")
                {
                    Console.WriteLine($"❌ Acceso denegado: rol {roleClaim} no es Tutor ni Admin");
                    return Forbid();
                }

                // Obtener datos del usuario (Tutor o Admin)
                var usuario = await _usuarioRepo.FindByEmailAsync(email);
                if (usuario == null)
                    return Unauthorized(new { message = "Usuario no encontrado." });

                // Validar que sea Tutor o Admin
                if (usuario.Rol != "Tutor" && usuario.Rol != "Admin")
                    return Forbid(); // No es tutor ni admin

                // Crear el curso
                var curso = new Curso
                {
                    Nombre = dto.Nombre,
                    Descripcion = dto.Descripcion,
                    Categoria = dto.Categoria,
                    Precio = dto.Precio,
                    Nivel = dto.Nivel,
                    Duracion = dto.Duracion,
                    Capacidad = dto.Capacidad,
                    Temario = dto.Temario,
                    TutorId = usuario.Id,
                    TutorNombre = $"{usuario.Nombre} {usuario.Apellido}",
                    Estado = "Pendiente", // Requiere aprobación del admin
                    FechaCreacion = DateTime.UtcNow
                };

                var cursoId = await _cursoRepo.AddAsync(curso);

                return Ok(new
                {
                    message = "Curso creado exitosamente y enviado para aprobación.",
                    cursoId = cursoId,
                    estado = "Pendiente"
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error al crear el curso.", details = ex.Message });
            }
        }

        /// <summary>
        /// Obtener cursos del tutor actual (autenticado)
        /// </summary>
        [Authorize(Roles = "Tutor")]
        [HttpGet("mis-cursos")]
        public async Task<IActionResult> MisCursos()
        {
            try
            {
                var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (string.IsNullOrEmpty(userId))
                    return Unauthorized();

                var cursos = await _cursoRepo.GetByTutorIdAsync(userId);
                return Ok(new { cursos = cursos });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error al obtener los cursos.", details = ex.Message });
            }
        }

        /// <summary>
        /// Obtener todos los cursos aprobados (público)
        /// </summary>
        [HttpGet("aprobados")]
        public async Task<IActionResult> GetAprobados()
        {
            try
            {
                var cursos = await _cursoRepo.GetAprobadosAsync();
                return Ok(new { cursos = cursos });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error al obtener los cursos.", details = ex.Message });
            }
        }

        /// <summary>
        /// Obtener detalles de un curso (público)
        /// </summary>
        [HttpGet("{id}")]
        public async Task<IActionResult> GetCursoById(string id)
        {
            try
            {
                var curso = await _cursoRepo.GetByIdAsync(id);
                if (curso == null)
                    return NotFound(new { message = "Curso no encontrado." });

                return Ok(curso);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error al obtener el curso.", details = ex.Message });
            }
        }

        /// <summary>
        /// Admin: Obtener todos los cursos (pendientes, aprobados, etc.)
        /// </summary>
        [Authorize(Roles = "Admin")]
        [HttpGet("admin/todos")]
        public async Task<IActionResult> GetTodosAdmin()
        {
            try
            {
                var cursos = await _cursoRepo.GetAllAsync();
                return Ok(new { cursos = cursos });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error al obtener los cursos.", details = ex.Message });
            }
        }

        /// <summary>
        /// Admin: Aprobar un curso
        /// </summary>
        [Authorize(Roles = "Admin")]
        [HttpPut("admin/{id}/aprobar")]
        public async Task<IActionResult> AprobarCurso(string id)
        {
            try
            {
                var curso = await _cursoRepo.GetByIdAsync(id);
                if (curso == null)
                    return NotFound(new { message = "Curso no encontrado." });

                curso.Estado = "Aprobado";
                curso.FechaAprobacion = DateTime.UtcNow;
                await _cursoRepo.UpdateAsync(curso);

                // 1) Notificar al tutor
                try
                {
                    var noti = new TutoriasDeClases.Modelos.Notificacion { Mensaje = $"Tu curso '{curso.Nombre}' ha sido aprobado y está disponible en la tienda." };
                    try { noti.Id = MongoDB.Bson.ObjectId.GenerateNewId().ToString(); } catch {}
                    if (!string.IsNullOrEmpty(curso.TutorId))
                    {
                        await _usuarioRepo.AddNotificationAsync(curso.TutorId, noti);
                        // Enviar notificación en tiempo real
                        try
                        {
                            await _hubContext.Clients.User(curso.TutorId).SendAsync("ReceiveNotification", new { id = noti.Id, mensaje = noti.Mensaje });
                        }
                        catch (Exception sendEx)
                        {
                            Console.WriteLine("Error al enviar notificación en tiempo real: " + sendEx.Message);
                        }
                    }
                }
                catch (Exception nEx)
                {
                    Console.WriteLine("Error al agregar notificación: " + nEx.Message);
                }

                // 2) Agregar a la "tienda" como Servicio
                try
                {
                    var servicio = new TutoriasDeClases.Modelos.Servicio
                    {
                        Titulo = curso.Nombre,
                        PrecioBase = Convert.ToDouble(curso.Precio)
                    };
                    await _servicios.InsertOneAsync(servicio);
                    try { Console.WriteLine($"Servicio insertado en tienda: Id={servicio.Id}, Titulo={servicio.Titulo}, Precio={servicio.PrecioBase}"); } catch {}
                }
                catch (Exception sEx)
                {
                    Console.WriteLine("Error al insertar servicio en tienda: " + sEx.Message);
                }

                return Ok(new { message = "Curso aprobado exitosamente." });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error al aprobar el curso.", details = ex.Message });
            }
        }

        /// <summary>
        /// Admin: Rechazar un curso
        /// </summary>
        [Authorize(Roles = "Admin")]
        [HttpPut("admin/{id}/rechazar")]
        public async Task<IActionResult> RechazarCurso(string id)
        {
            try
            {
                var curso = await _cursoRepo.GetByIdAsync(id);
                if (curso == null)
                    return NotFound(new { message = "Curso no encontrado." });

                curso.Estado = "Rechazado";
                await _cursoRepo.UpdateAsync(curso);

                return Ok(new { message = "Curso rechazado." });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error al rechazar el curso.", details = ex.Message });
            }
        }
    }

    /// <summary>
    /// DTO para crear un curso
    /// </summary>
    public class CrearCursoDto
    {
        public string Nombre { get; set; }
        public string Descripcion { get; set; }
        public string Categoria { get; set; }
        public decimal Precio { get; set; }
        public string Nivel { get; set; }
        public int Duracion { get; set; }
        public int Capacidad { get; set; }
        public List<string> Temario { get; set; } = new List<string>();
    }
}
