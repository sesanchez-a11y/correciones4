using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using MongoDB.Driver;
using System.Security.Claims;
using TutoriasDeClases.Modelos;
using TutoriasDeClases.Factories;
using TutoriasDeClases.Strategies;
using TutoriasDeClases.Observers;
using TutoriasDeClases.Interfaces;

namespace TutoriasDeClases.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ReservasController : ControllerBase
    {
        private readonly IMongoCollection<Servicio> _servicios;
        private readonly IMongoCollection<Reserva> _reservas;
        private readonly IUsuarioRepository _usuarioRepo;

        public ReservasController(IMongoCollection<Servicio> servicios, IMongoCollection<Reserva> reservas, IUsuarioRepository usuarioRepo)
        {
            _servicios = servicios;
            _reservas = reservas;
            _usuarioRepo = usuarioRepo;
        }

        [Authorize(Roles = "Alumno,Admin")]
        [HttpPost("crear")]
        public async Task<IActionResult> CrearReserva([FromBody] CrearReservaRequest request)
        {
            try
            {
                // Obtener datos del usuario autenticado
                var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                var email = User.FindFirst(ClaimTypes.Email)?.Value;

                if (string.IsNullOrEmpty(userId) || string.IsNullOrEmpty(email))
                    return Unauthorized(new { message = "Usuario no identificado." });

                var usuario = await _usuarioRepo.FindByEmailAsync(email);
                if (usuario == null)
                    return Unauthorized(new { message = "Usuario no encontrado." });

                // Buscar servicio
                var servicio = await _servicios.Find(s => s.Id == request.ServicioId).FirstOrDefaultAsync();
                if (servicio == null)
                    return NotFound(new { message = "Curso no encontrado." });

                // Aplicar estrategia
                IPrecioStrategy estrategia = new PrecioConDescuento();
                double total = estrategia.Calcular(servicio.PrecioBase);

                // Crear reserva
                var reserva = ReservaFactory.CrearReservaIndividual(servicio, userId, total);

                // Notificar
                reserva.AgregarObservador(new EmailNotificacion());
                reserva.AgregarObservador(new SmsNotificacion());
                reserva.Confirmar();

                // Guardar
                await _reservas.InsertOneAsync(reserva);

                // Crear notificación para el alumno
                var noti = new Notificacion { Mensaje = $"Te has registrado exitosamente al curso '{servicio.Titulo}'." };
                try { noti.Id = MongoDB.Bson.ObjectId.GenerateNewId().ToString(); } catch { }
                await _usuarioRepo.AddNotificationAsync(userId, noti);

                return Ok(new { message = "Registración a curso exitosa.", reservaId = reserva.Id, total = total });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error al crear la registración.", details = ex.Message });
            }
        }

        /// <summary>
        /// Obtener todos los servicios (cursos aprobados disponibles en tienda)
        /// </summary>
        [HttpGet("servicios")]
        public async Task<IActionResult> GetServicios()
        {
            try
            {
                var servicios = await _servicios.Find(Builders<Servicio>.Filter.Empty).ToListAsync();
                return Ok(new { servicios = servicios });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error al obtener servicios.", details = ex.Message });
            }
        }

        /// <summary>
        /// Obtener reservas del usuario autenticado (para mostrar en "Mis Cursos")
        /// </summary>
        [Authorize]
        [HttpGet("mis-reservas")]
        public async Task<IActionResult> GetMisReservas()
        {
            try
            {
                var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (string.IsNullOrEmpty(userId))
                    return Unauthorized(new { message = "Usuario no identificado." });

                var reservas = await _reservas
                    .Find(r => r.AlumnoId == userId)
                    .ToListAsync();

                return Ok(new { reservas = reservas });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error al obtener reservas.", details = ex.Message });
            }
        }
    }

    public class CrearReservaRequest
    {
        public string ServicioId { get; set; } = string.Empty;
        public string AlumnoId { get; set; } = string.Empty;
    }
}
