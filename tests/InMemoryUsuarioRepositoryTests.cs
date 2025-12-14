using Xunit;
using TutoriasDeClases.Interfaces;
using TutoriasDeClases.Repositories;
using TutoriasDeClases.Modelos;
using System;
using System.Threading.Tasks;
using BCrypt.Net;

namespace TutoriasDeClases.Tests
{
    public class InMemoryUsuarioRepositoryTests
    {
        private IUsuarioRepository CreateRepository()
        {
            return new InMemoryUsuarioRepository();
        }

        [Fact]
        public async Task AddAsync_ShouldAddUserSuccessfully()
        {
            // Arrange
            var repo = CreateRepository();
            var usuario = new Usuario
            {
                Nombre = "Test",
                Apellido = "User",
                Correo = "test@example.com",
                Rol = "Estudiante",
                Edad = 20,
                Especializacion = "Programación",
                ContrasenaHash = "password123"
            };

            // Act
            var id = await repo.AddAsync(usuario);

            // Assert
            Assert.NotNull(id);
            Assert.NotEmpty(id);
        }

        [Fact]
        public async Task FindByEmailAsync_ShouldReturnUserIfExists()
        {
            // Arrange
            var repo = CreateRepository();
            var email = "findme@example.com";
            var usuario = new Usuario
            {
                Nombre = "John",
                Apellido = "Doe",
                Correo = email,
                Rol = "Tutor",
                Edad = 30,
                Especializacion = "Matemáticas",
                ContrasenaHash = "hashedpass"
            };
            await repo.AddAsync(usuario);

            // Act
            var found = await repo.FindByEmailAsync(email);

            // Assert
            Assert.NotNull(found);
            Assert.Equal(email, found.Correo);
            Assert.Equal("John", found.Nombre);
        }

        [Fact]
        public async Task FindByEmailAsync_ShouldReturnNullIfUserDoesNotExist()
        {
            // Arrange
            var repo = CreateRepository();

            // Act
            var found = await repo.FindByEmailAsync("nonexistent@example.com");

            // Assert
            Assert.Null(found);
        }

        [Fact]
        public async Task ValidateCredentialsAsync_ShouldReturnUserForValidCredentials()
        {
            // Arrange
            var repo = CreateRepository();
            var email = "valid@example.com";
            var password = "correctpassword";
            var usuario = new Usuario
            {
                Nombre = "Valid",
                Apellido = "User",
                Correo = email,
                Rol = "Estudiante",
                Edad = 25,
                Especializacion = "Programación",
                ContrasenaHash = password
            };
            await repo.AddAsync(usuario);

            // Act
            var result = await repo.ValidateCredentialsAsync(email, password);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(email, result.Correo);
        }

        [Fact]
        public async Task ValidateCredentialsAsync_ShouldReturnNullForInvalidPassword()
        {
            // Arrange
            var repo = CreateRepository();
            var email = "invalid@example.com";
            var usuario = new Usuario
            {
                Nombre = "Invalid",
                Apellido = "User",
                Correo = email,
                Rol = "Estudiante",
                Edad = 22,
                Especializacion = "Diseño",
                ContrasenaHash = "actualpassword"
            };
            await repo.AddAsync(usuario);

            // Act
            var result = await repo.ValidateCredentialsAsync(email, "wrongpassword");

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public async Task ValidateCredentialsAsync_ShouldReturnNullForNonexistentUser()
        {
            // Arrange
            var repo = CreateRepository();

            // Act
            var result = await repo.ValidateCredentialsAsync("doesnotexist@example.com", "anypassword");

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public async Task AddAsync_ShouldHashPasswordAutomatically()
        {
            // Arrange
            var repo = CreateRepository();
            var plainPassword = "myplainpassword";
            var usuario = new Usuario
            {
                Nombre = "Hashed",
                Apellido = "User",
                Correo = "hash@example.com",
                Rol = "Tutor",
                Edad = 35,
                Especializacion = "Historia",
                ContrasenaHash = plainPassword
            };

            // Act
            var id = await repo.AddAsync(usuario);
            var found = await repo.FindByEmailAsync("hash@example.com");

            // Assert
            Assert.NotNull(found);
            Assert.NotEqual(plainPassword, found?.ContrasenaHash);
            var isValid = await repo.ValidateCredentialsAsync("hash@example.com", plainPassword);
            Assert.NotNull(isValid);
        }
    }
}
