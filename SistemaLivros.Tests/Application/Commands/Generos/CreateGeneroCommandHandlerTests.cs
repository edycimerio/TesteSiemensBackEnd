using AutoFixture;
using Moq;
using SistemaLivros.Application.Commands.Generos;
using SistemaLivros.Domain.Entities;
using SistemaLivros.Domain.Interfaces;
using System;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace SistemaLivros.Tests.Application.Commands.Generos
{
    public class CreateGeneroCommandHandlerTests
    {
        private readonly Mock<IGeneroRepository> _generoRepositoryMock;
        private readonly CreateGeneroCommandHandler _handler;
        private readonly Fixture _fixture;

        public CreateGeneroCommandHandlerTests()
        {
            _generoRepositoryMock = new Mock<IGeneroRepository>();
            _handler = new CreateGeneroCommandHandler(_generoRepositoryMock.Object);
            _fixture = new Fixture();
        }

        [Fact]
        public async Task CriaGeneroComSucesso()
        {
            // Arrange
            var command = new CreateGeneroCommand("Ficção Científica", "Livros de ficção científica");

            // Criar o gênero usando o construtor público
            var genero = new Genero(command.Nome, command.Descricao);
            
            // Usar reflection para definir o Id para teste
            typeof(Entity).GetProperty("Id").SetValue(genero, 1);

            _generoRepositoryMock.Setup(r => r.AddAsync(It.IsAny<Genero>()))
                .Callback<Genero>(g => typeof(Entity).GetProperty("Id").SetValue(g, 1));
                
            // Configurar o mock para que o teste possa verificar o ID retornado
            _generoRepositoryMock.Setup(r => r.GetByIdAsync(1))
                .ReturnsAsync(genero);

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.Equal(1, result);
            _generoRepositoryMock.Verify(r => r.AddAsync(It.Is<Genero>(g => 
                g.Nome == command.Nome && 
                g.Descricao == command.Descricao)), 
                Times.Once);
        }
    }
}
