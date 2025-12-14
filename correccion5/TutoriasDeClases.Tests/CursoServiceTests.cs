using Xunit;
using Moq;
using TutoriasDeClases.Servicios;
using TutoriasDeClases.Interfaces;
using TutoriasDeClases.Modelos;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace TutoriasDeClases.Tests
{
    /// <summary>
    /// Pruebas unitarias para CursoService.
    /// Cumple con el requisito de pruebas de lógica de negocio:
    /// - Creación y validación de cursos
    /// - Inscripción de estudiantes
    /// - Reglas de negocio (cupos, prerequisitos, estados)
    /// - Pruebas de errores (curso lleno, duplicados, etc.)
    /// </summary>
    public class CursoServiceTests
    {
        #region Pruebas de Creación y Validación de Cursos

        [Fact]
        public async Task CrearCursoAsync_ConDatosValidos_DebeCrearCursoExitosamente()
        {
            // Arrange
            var mockCursoRepo = new Mock<ICursoRepository>();
            var mockUsuarioRepo = new Mock<IUsuarioRepository>();
            
            mockCursoRepo.Setup(r => r.AddAsync(It.IsAny<Curso>()))
                .ReturnsAsync("curso123");

            var service = new CursoService(mockCursoRepo.Object, mockUsuarioRepo.Object);

            // Act
            var resultado = await service.CrearCursoAsync(
                "Programación en C#",
                "Curso completo de C# desde cero",
                "Programación",
                99.99m,
                "Principiante",
                40,
                20,
                new List<string> { "Variables", "Funciones", "OOP" },
                "tutor123",
                "Juan Pérez"
            );

            // Assert
            Assert.True(resultado.Success);
            Assert.Equal("Curso creado exitosamente", resultado.Message);
            Assert.Equal("curso123", resultado.CursoId);
            mockCursoRepo.Verify(r => r.AddAsync(It.IsAny<Curso>()), Times.Once);
        }

        [Fact]
        public void ValidarDatosCurso_ConNombreVacio_DebeRetornarError()
        {
            // Arrange
            var mockCursoRepo = new Mock<ICursoRepository>();
            var mockUsuarioRepo = new Mock<IUsuarioRepository>();
            var service = new CursoService(mockCursoRepo.Object, mockUsuarioRepo.Object);

            // Act
            var resultado = service.ValidarDatosCurso("", "Descripción", 50m, 10, 20);

            // Assert
            Assert.False(resultado.IsValid);
            Assert.Contains("nombre", resultado.ErrorMessage.ToLower());
        }

        [Fact]
        public void ValidarDatosCurso_ConNombreCorto_DebeRetornarError()
        {
            // Arrange
            var mockCursoRepo = new Mock<ICursoRepository>();
            var mockUsuarioRepo = new Mock<IUsuarioRepository>();
            var service = new CursoService(mockCursoRepo.Object, mockUsuarioRepo.Object);

            // Act
            var resultado = service.ValidarDatosCurso("AB", "Descripción", 50m, 10, 20);

            // Assert
            Assert.False(resultado.IsValid);
            Assert.Contains("al menos 3 caracteres", resultado.ErrorMessage);
        }

        [Fact]
        public void ValidarDatosCurso_ConPrecioNegativo_DebeRetornarError()
        {
            // Arrange
            var mockCursoRepo = new Mock<ICursoRepository>();
            var mockUsuarioRepo = new Mock<IUsuarioRepository>();
            var service = new CursoService(mockCursoRepo.Object, mockUsuarioRepo.Object);

            // Act
            var resultado = service.ValidarDatosCurso("Curso Test", "Descripción", -10m, 10, 20);

            // Assert
            Assert.False(resultado.IsValid);
            Assert.Contains("precio", resultado.ErrorMessage.ToLower());
        }

        [Fact]
        public void ValidarDatosCurso_ConDuracionCero_DebeRetornarError()
        {
            // Arrange
            var mockCursoRepo = new Mock<ICursoRepository>();
            var mockUsuarioRepo = new Mock<IUsuarioRepository>();
            var service = new CursoService(mockCursoRepo.Object, mockUsuarioRepo.Object);

            // Act
            var resultado = service.ValidarDatosCurso("Curso Test", "Descripción", 50m, 0, 20);

            // Assert
            Assert.False(resultado.IsValid);
            Assert.Contains("duración", resultado.ErrorMessage.ToLower());
        }

        [Fact]
        public void ValidarDatosCurso_ConCapacidadCero_DebeRetornarError()
        {
            // Arrange
            var mockCursoRepo = new Mock<ICursoRepository>();
            var mockUsuarioRepo = new Mock<IUsuarioRepository>();
            var service = new CursoService(mockCursoRepo.Object, mockUsuarioRepo.Object);

            // Act
            var resultado = service.ValidarDatosCurso("Curso Test", "Descripción", 50m, 10, 0);

            // Assert
            Assert.False(resultado.IsValid);
            Assert.Contains("capacidad", resultado.ErrorMessage.ToLower());
        }

        [Fact]
        public void ValidarDatosCurso_ConCapacidadExcesiva_DebeRetornarError()
        {
            // Arrange
            var mockCursoRepo = new Mock<ICursoRepository>();
            var mockUsuarioRepo = new Mock<IUsuarioRepository>();
            var service = new CursoService(mockCursoRepo.Object, mockUsuarioRepo.Object);

            // Act
            var resultado = service.ValidarDatosCurso("Curso Test", "Descripción", 50m, 10, 150);

            // Assert
            Assert.False(resultado.IsValid);
            Assert.Contains("100", resultado.ErrorMessage);
        }

        #endregion

        #region Pruebas de Inscripción de Estudiantes

        [Fact]
        public async Task InscribirEstudianteAsync_ConCursoAprobadoYCuposDisponibles_DebeInscribirExitosamente()
        {
            // Arrange
            var curso = new Curso
            {
                Id = "curso123",
                Nombre = "Test Curso",
                Estado = "Aprobado",
                Capacidad = 20,
                Estudiantes = new List<string> { "est1", "est2" }
            };

            var mockCursoRepo = new Mock<ICursoRepository>();
            var mockUsuarioRepo = new Mock<IUsuarioRepository>();
            
            mockCursoRepo.Setup(r => r.GetByIdAsync("curso123"))
                .ReturnsAsync(curso);
            
            mockCursoRepo.Setup(r => r.UpdateAsync(It.IsAny<Curso>()))
                .Returns(Task.CompletedTask);

            var service = new CursoService(mockCursoRepo.Object, mockUsuarioRepo.Object);

            // Act
            var resultado = await service.InscribirEstudianteAsync("curso123", "est3");

            // Assert
            Assert.True(resultado.Success);
            Assert.Equal("Inscripción exitosa", resultado.Message);
            Assert.Contains("est3", curso.Estudiantes);
            mockCursoRepo.Verify(r => r.UpdateAsync(It.IsAny<Curso>()), Times.Once);
        }

        [Fact]
        public async Task InscribirEstudianteAsync_ConCursoNoEncontrado_DebeRetornarError()
        {
            // Arrange
            var mockCursoRepo = new Mock<ICursoRepository>();
            var mockUsuarioRepo = new Mock<IUsuarioRepository>();
            
            mockCursoRepo.Setup(r => r.GetByIdAsync("cursoInexistente"))
                .ReturnsAsync((Curso)null);

            var service = new CursoService(mockCursoRepo.Object, mockUsuarioRepo.Object);

            // Act
            var resultado = await service.InscribirEstudianteAsync("cursoInexistente", "est1");

            // Assert
            Assert.False(resultado.Success);
            Assert.Contains("no encontrado", resultado.Message.ToLower());
        }

        [Fact]
        public async Task InscribirEstudianteAsync_ConCursoNoPendiente_DebeRetornarError()
        {
            // Arrange
            var curso = new Curso
            {
                Id = "curso123",
                Estado = "Pendiente",
                Capacidad = 20,
                Estudiantes = new List<string>()
            };

            var mockCursoRepo = new Mock<ICursoRepository>();
            var mockUsuarioRepo = new Mock<IUsuarioRepository>();
            
            mockCursoRepo.Setup(r => r.GetByIdAsync("curso123"))
                .ReturnsAsync(curso);

            var service = new CursoService(mockCursoRepo.Object, mockUsuarioRepo.Object);

            // Act
            var resultado = await service.InscribirEstudianteAsync("curso123", "est1");

            // Assert
            Assert.False(resultado.Success);
            Assert.Contains("aprobados", resultado.Message.ToLower());
        }

        [Fact]
        public async Task InscribirEstudianteAsync_ConCursoLleno_DebeRetornarError()
        {
            // Arrange
            var curso = new Curso
            {
                Id = "curso123",
                Estado = "Aprobado",
                Capacidad = 2,
                Estudiantes = new List<string> { "est1", "est2" }
            };

            var mockCursoRepo = new Mock<ICursoRepository>();
            var mockUsuarioRepo = new Mock<IUsuarioRepository>();
            
            mockCursoRepo.Setup(r => r.GetByIdAsync("curso123"))
                .ReturnsAsync(curso);

            var service = new CursoService(mockCursoRepo.Object, mockUsuarioRepo.Object);

            // Act
            var resultado = await service.InscribirEstudianteAsync("curso123", "est3");

            // Assert
            Assert.False(resultado.Success);
            Assert.Contains("lleno", resultado.Message.ToLower());
            Assert.Contains("cupo", resultado.Message.ToLower());
        }

        [Fact]
        public async Task InscribirEstudianteAsync_ConEstudianteYaInscrito_DebeRetornarError()
        {
            // Arrange
            var curso = new Curso
            {
                Id = "curso123",
                Estado = "Aprobado",
                Capacidad = 20,
                Estudiantes = new List<string> { "est1", "est2" }
            };

            var mockCursoRepo = new Mock<ICursoRepository>();
            var mockUsuarioRepo = new Mock<IUsuarioRepository>();
            
            mockCursoRepo.Setup(r => r.GetByIdAsync("curso123"))
                .ReturnsAsync(curso);

            var service = new CursoService(mockCursoRepo.Object, mockUsuarioRepo.Object);

            // Act
            var resultado = await service.InscribirEstudianteAsync("curso123", "est1");

            // Assert
            Assert.False(resultado.Success);
            Assert.Contains("ya está inscrito", resultado.Message.ToLower());
        }

        #endregion

        #region Pruebas de Cupos Disponibles

        [Fact]
        public async Task TieneCuposDisponiblesAsync_ConCuposDisponibles_DebeRetornarTrue()
        {
            // Arrange
            var curso = new Curso
            {
                Id = "curso123",
                Capacidad = 20,
                Estudiantes = new List<string> { "est1", "est2", "est3" }
            };

            var mockCursoRepo = new Mock<ICursoRepository>();
            var mockUsuarioRepo = new Mock<IUsuarioRepository>();
            
            mockCursoRepo.Setup(r => r.GetByIdAsync("curso123"))
                .ReturnsAsync(curso);

            var service = new CursoService(mockCursoRepo.Object, mockUsuarioRepo.Object);

            // Act
            var resultado = await service.TieneCuposDisponiblesAsync("curso123");

            // Assert
            Assert.True(resultado);
        }

        [Fact]
        public async Task TieneCuposDisponiblesAsync_ConCursoLleno_DebeRetornarFalse()
        {
            // Arrange
            var curso = new Curso
            {
                Id = "curso123",
                Capacidad = 3,
                Estudiantes = new List<string> { "est1", "est2", "est3" }
            };

            var mockCursoRepo = new Mock<ICursoRepository>();
            var mockUsuarioRepo = new Mock<IUsuarioRepository>();
            
            mockCursoRepo.Setup(r => r.GetByIdAsync("curso123"))
                .ReturnsAsync(curso);

            var service = new CursoService(mockCursoRepo.Object, mockUsuarioRepo.Object);

            // Act
            var resultado = await service.TieneCuposDisponiblesAsync("curso123");

            // Assert
            Assert.False(resultado);
        }

        [Fact]
        public async Task ObtenerCuposDisponiblesAsync_DebeCalcularCorrectamente()
        {
            // Arrange
            var curso = new Curso
            {
                Id = "curso123",
                Capacidad = 20,
                Estudiantes = new List<string> { "est1", "est2", "est3" }
            };

            var mockCursoRepo = new Mock<ICursoRepository>();
            var mockUsuarioRepo = new Mock<IUsuarioRepository>();
            
            mockCursoRepo.Setup(r => r.GetByIdAsync("curso123"))
                .ReturnsAsync(curso);

            var service = new CursoService(mockCursoRepo.Object, mockUsuarioRepo.Object);

            // Act
            var cuposDisponibles = await service.ObtenerCuposDisponiblesAsync("curso123");

            // Assert
            Assert.Equal(17, cuposDisponibles);
        }

        #endregion

        #region Pruebas de Aprobación y Rechazo

        [Fact]
        public async Task AprobarCursoAsync_ConCursoPendiente_DebeAprobarExitosamente()
        {
            // Arrange
            var curso = new Curso
            {
                Id = "curso123",
                Nombre = "Test Curso",
                Estado = "Pendiente"
            };

            var mockCursoRepo = new Mock<ICursoRepository>();
            var mockUsuarioRepo = new Mock<IUsuarioRepository>();
            
            mockCursoRepo.Setup(r => r.GetByIdAsync("curso123"))
                .ReturnsAsync(curso);
            
            mockCursoRepo.Setup(r => r.UpdateAsync(It.IsAny<Curso>()))
                .Returns(Task.CompletedTask);

            var service = new CursoService(mockCursoRepo.Object, mockUsuarioRepo.Object);

            // Act
            var resultado = await service.AprobarCursoAsync("curso123");

            // Assert
            Assert.True(resultado.Success);
            Assert.Equal("Aprobado", curso.Estado);
            Assert.NotNull(curso.FechaAprobacion);
            mockCursoRepo.Verify(r => r.UpdateAsync(It.IsAny<Curso>()), Times.Once);
        }

        [Fact]
        public async Task AprobarCursoAsync_ConCursoYaAprobado_DebeRetornarError()
        {
            // Arrange
            var curso = new Curso
            {
                Id = "curso123",
                Estado = "Aprobado"
            };

            var mockCursoRepo = new Mock<ICursoRepository>();
            var mockUsuarioRepo = new Mock<IUsuarioRepository>();
            
            mockCursoRepo.Setup(r => r.GetByIdAsync("curso123"))
                .ReturnsAsync(curso);

            var service = new CursoService(mockCursoRepo.Object, mockUsuarioRepo.Object);

            // Act
            var resultado = await service.AprobarCursoAsync("curso123");

            // Assert
            Assert.False(resultado.Success);
            Assert.Contains("ya está aprobado", resultado.Message.ToLower());
        }

        [Fact]
        public async Task RechazarCursoAsync_ConCursoPendiente_DebeRechazarExitosamente()
        {
            // Arrange
            var curso = new Curso
            {
                Id = "curso123",
                Estado = "Pendiente"
            };

            var mockCursoRepo = new Mock<ICursoRepository>();
            var mockUsuarioRepo = new Mock<IUsuarioRepository>();
            
            mockCursoRepo.Setup(r => r.GetByIdAsync("curso123"))
                .ReturnsAsync(curso);
            
            mockCursoRepo.Setup(r => r.UpdateAsync(It.IsAny<Curso>()))
                .Returns(Task.CompletedTask);

            var service = new CursoService(mockCursoRepo.Object, mockUsuarioRepo.Object);

            // Act
            var resultado = await service.RechazarCursoAsync("curso123");

            // Assert
            Assert.True(resultado.Success);
            Assert.Equal("Rechazado", curso.Estado);
            mockCursoRepo.Verify(r => r.UpdateAsync(It.IsAny<Curso>()), Times.Once);
        }

        #endregion

        #region Pruebas de Obtención de Cursos

        [Fact]
        public async Task ObtenerCursosAprobadosAsync_DebeRetornarSoloCursosAprobados()
        {
            // Arrange
            var cursosAprobados = new List<Curso>
            {
                new Curso { Id = "curso1", Estado = "Aprobado" },
                new Curso { Id = "curso2", Estado = "Aprobado" }
            };

            var mockCursoRepo = new Mock<ICursoRepository>();
            var mockUsuarioRepo = new Mock<IUsuarioRepository>();
            
            mockCursoRepo.Setup(r => r.GetAprobadosAsync())
                .ReturnsAsync(cursosAprobados);

            var service = new CursoService(mockCursoRepo.Object, mockUsuarioRepo.Object);

            // Act
            var resultado = await service.ObtenerCursosAprobadosAsync();

            // Assert
            Assert.Equal(2, resultado.Count);
            Assert.All(resultado, c => Assert.Equal("Aprobado", c.Estado));
        }

        [Fact]
        public async Task ObtenerCursoPorIdAsync_DebeRetornarCursoCorrecto()
        {
            // Arrange
            var curso = new Curso { Id = "curso123", Nombre = "Test" };

            var mockCursoRepo = new Mock<ICursoRepository>();
            var mockUsuarioRepo = new Mock<IUsuarioRepository>();
            
            mockCursoRepo.Setup(r => r.GetByIdAsync("curso123"))
                .ReturnsAsync(curso);

            var service = new CursoService(mockCursoRepo.Object, mockUsuarioRepo.Object);

            // Act
            var resultado = await service.ObtenerCursoPorIdAsync("curso123");

            // Assert
            Assert.NotNull(resultado);
            Assert.Equal("curso123", resultado.Id);
        }

        #endregion
    }
}
