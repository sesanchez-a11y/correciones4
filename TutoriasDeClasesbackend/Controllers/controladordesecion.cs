using Microsoft.AspNetCore.Mvc;
using TutoriasDeClases.Modelos;
using TutoriasDeClases.Interfaces;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using System.IdentityModel.Tokens.Jwt;


namespace TutoriasDeClases.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ControladorDeSesion : ControllerBase // ← Cambié el nombre de la clase para coincidir con el archivo
    {
        private readonly IUsuarioRepository _usuarioRepo;
        private readonly IConfiguration _config;

        public ControladorDeSesion(IUsuarioRepository usuarioRepo, IConfiguration config)
        {
            _usuarioRepo = usuarioRepo;
            _config = config;
            Console.WriteLine($"✓ ControladorDeSesion inicializado. Repositorio: {_usuarioRepo.GetType().Name}");
        }

        // Test endpoint
        [HttpGet("test")]
        public IActionResult Test()
        {
            Console.WriteLine("GET /api/ControladorDeSesion/test llamado");
            return Ok(new { message = "ControladorDeSesion respondiendo", repo = _usuarioRepo.GetType().Name });
        }

    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegistroModel model)
    {
        if (string.IsNullOrEmpty(model.Email) || string.IsNullOrEmpty(model.Contrasena))
            return BadRequest(new { message = "El correo y la contraseña son requeridos." });

        try
        {
            // Verificar si ya existe el usuario (a través del repositorio)
            var existingUser = await _usuarioRepo.FindByEmailAsync(model.Email);
            if (existingUser != null)
                return Conflict(new { message = "El correo ya está registrado." });

            // Crear nuevo usuario (puedes usar Estudiante o Tutor según el rol)
            Usuario newUser;
            if (model.Rol.ToLower() == "tutor")
            {
                newUser = new Tutor();
            }
            else
            {
                newUser = new Estudiante();
            }

            newUser.Nombre = model.Nombre;
            newUser.Apellido = model.Apellido;
            newUser.Correo = model.Email;
            newUser.Edad = model.Edad;
            newUser.Especializacion = model.Especializacion;
            newUser.Rol = model.Rol;
            newUser.ContrasenaHash = model.Contrasena; // ⚠️ En producción, HASHEA LA CONTRASEÑA aquí

            var userId = await _usuarioRepo.AddAsync(newUser);

            // Recuperar el usuario guardado para devolver datos públicos
            var saved = await _usuarioRepo.FindByEmailAsync(newUser.Correo);
            if (saved == null)
                return StatusCode(500, new { message = "Error interno: usuario guardado no encontrado." });

            var publicUser = new {
                id = saved.Id,
                nombre = saved.Nombre,
                apellido = saved.Apellido,
                correo = saved.Correo,
                edad = saved.Edad,
                especializacion = saved.Especializacion,
                rol = saved.Rol
            };

            return Ok(new { message = "Registro exitoso.", user = publicUser });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "Error interno al guardar el usuario.", details = ex.Message });
        }
    }

        // Endpoint para iniciar sesión
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginModel model)
        {
            // Aceptamos tanto `Password` como `Contrasena` para compatibilidad con el frontend en español
            var pwd = !string.IsNullOrEmpty(model.Password) ? model.Password : model.Contrasena;
            if (string.IsNullOrEmpty(model.Email) || string.IsNullOrEmpty(pwd))
                return BadRequest("Email y contraseña son obligatorios.");

            var user = await _usuarioRepo.ValidateCredentialsAsync(model.Email, pwd);
            if (user == null)
                return Unauthorized(new { message = "Credenciales inválidas." });

            var publicUser = new {
                id = user.Id,
                nombre = user.Nombre,
                apellido = user.Apellido,
                correo = user.Correo,
                edad = user.Edad,
                especializacion = user.Especializacion,
                rol = user.Rol
            };

            // Generar JWT real
            var jwtSection = _config.GetSection("Jwt");
            var jwtKey = jwtSection.GetValue<string>("Key") ?? "cambiar_esta_clave_por_una_mas_segura_en_produccion_!";
            var jwtIssuer = jwtSection.GetValue<string>("Issuer") ?? "EduMentor";
            var jwtAudience = jwtSection.GetValue<string>("Audience") ?? "EduMentorUsers";

            var claims = new[] {
                new Claim(ClaimTypes.NameIdentifier, user.Id ?? string.Empty),
                new Claim(ClaimTypes.Email, user.Correo ?? string.Empty),
                new Claim(ClaimTypes.Name, user.Nombre ?? string.Empty),
                new Claim(ClaimTypes.Role, user.Rol ?? string.Empty)
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
            var token = new JwtSecurityToken(
                issuer: jwtIssuer,
                audience: jwtAudience,
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(jwtSection.GetValue<int>("ExpiryMinutes", 120)),
                signingCredentials: creds
            );

            var tokenString = new JwtSecurityTokenHandler().WriteToken(token);

            return Ok(new { message = "Inicio de sesión exitoso.", token = tokenString, user = publicUser });
        }

        [Authorize]
        [HttpGet("me")]
        public async Task<IActionResult> Me()
        {
            var email = User.FindFirst(ClaimTypes.Email)?.Value;
            if (string.IsNullOrEmpty(email)) return Unauthorized();

            var user = await _usuarioRepo.FindByEmailAsync(email);
            if (user == null) return NotFound();

            var publicUser = new {
                id = user.Id,
                nombre = user.Nombre,
                apellido = user.Apellido,
                correo = user.Correo,
                edad = user.Edad,
                especializacion = user.Especializacion,
                rol = user.Rol
            };

            return Ok(publicUser);
        }

        // Endpoint para cerrar sesión (logout)
        [HttpPost("logout")]
        public IActionResult Logout()
        {
            // Aquí podrías invalidar un token o borrar una cookie
            return Ok(new { message = "Sesión cerrada correctamente." });
        }
    }

    public class LoginModel
    {
        // Se soportan ambas propiedades para compatibilidad: `Password` (inglés) y `Contrasena` (español)
        public string Email { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public string Contrasena { get; set; } = string.Empty;
    }
}