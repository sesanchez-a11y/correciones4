using Xunit;
using Moq;
using TutoriasDeClases.Controllers;
using TutoriasDeClases.Interfaces;
using TutoriasDeClases.Modelos;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace TutoriasDeClases.Tests
{
    public class ControladorDeSesionTests
    {
        private IConfiguration CreateMockConfiguration()
        {
            var inMemorySettings = new Dictionary<string, string>
            {
                { "Jwt:Key", "test_key_that_is_long_enough_for_hs256_algorithm_testing" },
                { "Jwt:Issuer", "TestIssuer" },
                { "Jwt:Audience", "TestAudience" },
                { "Jwt:ExpiryMinutes", "60" }
            };

            var config = new ConfigurationBuilder()
                .AddInMemoryCollection(inMemorySettings)
                .Build();

            return config;
        }

        [Fact]
        public async Task Register_ShouldReturnOkWithUserData_WhenRegistrationSucceeds()
        {
            // Arrange
            var usuario = new Usuario
            {
                Id = "user123",
                Nombre = "New",
                Apellido = "User",
                Correo = "newuser@example.com",
                Rol = "Estudiante",
                Edad = 20,
                Especializacion = "Programación",
                ContrasenaHash = "hash"
            };

            var mockRepo = new Mock<IUsuarioRepository>();
            var sequence = mockRepo.SetupSequence(r => r.FindByEmailAsync("newuser@example.com"));
            sequence.ReturnsAsync((Usuario)null);  // First call - user doesn't exist yet
            sequence.ReturnsAsync(usuario);        // Second call - after Add, user exists
            
            mockRepo.Setup(r => r.AddAsync(It.IsAny<Usuario>()))
                .ReturnsAsync("user123");

            var config = CreateMockConfiguration();
            var controller = new ControladorDeSesion(mockRepo.Object, config);

            var registerModel = new RegistroModel
            {
                Email = "newuser@example.com",
                Nombre = "New",
                Apellido = "User",
                Rol = "Estudiante",
                Edad = 20,
                Especializacion = "Programación",
                Contrasena = "SecurePass123"
            };

            // Act
            var result = await controller.Register(registerModel);

            // Assert
            Assert.NotNull(result);
            Assert.IsType<OkObjectResult>(result);
        }

        [Fact]
        public async Task Register_ShouldReturnConflict_WhenEmailAlreadyExists()
        {
            // Arrange
            var existingUser = new Usuario
            {
                Id = "existing123",
                Correo = "taken@example.com",
                Nombre = "Existing",
                Apellido = "User",
                Rol = "Estudiante",
                Edad = 25,
                Especializacion = "Diseño",
                ContrasenaHash = "hash"
            };

            var mockRepo = new Mock<IUsuarioRepository>();
            mockRepo.Setup(r => r.FindByEmailAsync("taken@example.com"))
                .ReturnsAsync(existingUser);

            var config = CreateMockConfiguration();
            var controller = new ControladorDeSesion(mockRepo.Object, config);

            var registerModel = new RegistroModel
            {
                Email = "taken@example.com",
                Nombre = "Another",
                Apellido = "User",
                Rol = "Tutor",
                Edad = 30,
                Especializacion = "Matemáticas",
                Contrasena = "Password123"
            };

            // Act
            var result = await controller.Register(registerModel);

            // Assert
            var conflictResult = Assert.IsType<ConflictObjectResult>(result);
            Assert.NotNull(conflictResult.Value);
        }

        [Fact]
        public async Task Login_ShouldReturnTokenAndUserData_WhenCredentialsAreValid()
        {
            // Arrange
            var usuario = new Usuario
            {
                Id = "user123",
                Correo = "test@example.com",
                Nombre = "Test",
                Apellido = "User",
                Rol = "Estudiante",
                Edad = 22,
                Especializacion = "Programación",
                ContrasenaHash = "somehash"
            };

            var mockRepo = new Mock<IUsuarioRepository>();
            mockRepo.Setup(r => r.ValidateCredentialsAsync("test@example.com", "password123"))
                .ReturnsAsync(usuario);

            var config = CreateMockConfiguration();
            var controller = new ControladorDeSesion(mockRepo.Object, config);

            var loginModel = new LoginModel
            {
                Email = "test@example.com",
                Password = "password123"
            };

            // Act
            var result = await controller.Login(loginModel);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.NotNull(okResult.Value);
            // Verify ValidateCredentialsAsync was called
            mockRepo.Verify(r => r.ValidateCredentialsAsync("test@example.com", "password123"), Times.Once);
        }

        [Fact]
        public async Task Login_ShouldReturnUnauthorized_WhenCredentialsAreInvalid()
        {
            // Arrange
            var mockRepo = new Mock<IUsuarioRepository>();
            mockRepo.Setup(r => r.ValidateCredentialsAsync(It.IsAny<string>(), It.IsAny<string>()))
                .ReturnsAsync((Usuario)null);

            var config = CreateMockConfiguration();
            var controller = new ControladorDeSesion(mockRepo.Object, config);

            var loginModel = new LoginModel
            {
                Email = "invalid@example.com",
                Password = "wrongpassword"
            };

            // Act
            var result = await controller.Login(loginModel);

            // Assert
            var unauthorizedResult = Assert.IsType<UnauthorizedObjectResult>(result);
            Assert.NotNull(unauthorizedResult.Value);
        }
    }
}
