using AutoFixture;
using Moq;
using SistemaLivros.Application.Commands.Generos.CreateGenero;
using SistemaLivros.Domain.Entities;
using SistemaLivros.Domain.Interfaces.Repositories;
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
            var command = new CreateGeneroCommand
            {
                Nome = "Ficção Científica",
                Descricao = "Livros de ficção científica"
            };

            var genero = new Genero
            {
                Id = 1,
                Nome = command.Nome,
                Descricao = command.Descricao
            };

            _generoRepositoryMock.Setup(r => r.AddAsync(It.IsAny<Genero>()))
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
