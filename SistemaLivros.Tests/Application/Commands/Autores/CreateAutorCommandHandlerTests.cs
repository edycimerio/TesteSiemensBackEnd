using AutoFixture;
using Moq;
using SistemaLivros.Application.Commands.Autores;
using SistemaLivros.Domain.Entities;
using SistemaLivros.Domain.Interfaces;
using System;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace SistemaLivros.Tests.Application.Commands.Autores
{
    public class CreateAutorCommandHandlerTests
    {
        private readonly Mock<IAutorRepository> _autorRepositoryMock;
        private readonly CreateAutorCommandHandler _handler;
        private readonly Fixture _fixture;

        public CreateAutorCommandHandlerTests()
        {
            _autorRepositoryMock = new Mock<IAutorRepository>();
            _handler = new CreateAutorCommandHandler(_autorRepositoryMock.Object);
            _fixture = new Fixture();
        }

        [Fact]
        public async Task CriaAutorComSucesso()
        {
            // Arrange
            var dataNascimento = new DateTime(1927, 3, 6);
            var command = new CreateAutorCommand("Gabriel García Márquez", "Escritor colombiano", dataNascimento);

            // Criar o autor usando o construtor público
            var autor = new Autor(command.Nome, command.Biografia, command.DataNascimento);
            
            // Usar reflection para definir o Id para teste
            typeof(Entity).GetProperty("Id").SetValue(autor, 1);

            _autorRepositoryMock.Setup(r => r.AddAsync(It.IsAny<Autor>()))
                .Callback<Autor>(a => typeof(Entity).GetProperty("Id").SetValue(a, 1));
                
            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.Equal(1, result);
            _autorRepositoryMock.Verify(r => r.AddAsync(It.Is<Autor>(a => 
                a.Nome == command.Nome && 
                a.Biografia == command.Biografia &&
                a.DataNascimento == command.DataNascimento)), 
                Times.Once);
        }
    }
}
