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
    public class UpdateGeneroCommandHandlerTests
    {
        private readonly Mock<IGeneroRepository> _generoRepositoryMock;
        private readonly UpdateGeneroCommandHandler _handler;
        private readonly Fixture _fixture;

        public UpdateGeneroCommandHandlerTests()
        {
            _generoRepositoryMock = new Mock<IGeneroRepository>();
            _handler = new UpdateGeneroCommandHandler(_generoRepositoryMock.Object);
            _fixture = new Fixture();
        }

        [Fact]
        public async Task AtualizaGeneroComSucesso()
        {
            // Arrange
            var generoId = 1;
            var command = new UpdateGeneroCommand(generoId, "Ficção Científica Atualizada", "Descrição atualizada");
            
            // Criar o gênero usando o construtor público
            var genero = new Genero("Ficção Científica", "Descrição original");
            
            // Usar reflection para definir o Id para teste
            typeof(Entity).GetProperty("Id").SetValue(genero, generoId);

            _generoRepositoryMock.Setup(r => r.GetByIdAsync(generoId))
                .ReturnsAsync(genero);
                
            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.True(result);
            _generoRepositoryMock.Verify(r => r.UpdateAsync(It.Is<Genero>(g => 
                g.Id == generoId && 
                g.Nome == command.Nome && 
                g.Descricao == command.Descricao)), 
                Times.Once);
        }

        [Fact]
        public async Task RetornaFalsoQuandoGeneroNaoExiste()
        {
            // Arrange
            var generoId = 99;
            var command = new UpdateGeneroCommand(generoId, "Ficção Científica Atualizada", "Descrição atualizada");
            
            _generoRepositoryMock.Setup(r => r.GetByIdAsync(generoId))
                .ReturnsAsync((Genero)null);
                
            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.False(result);
            _generoRepositoryMock.Verify(r => r.UpdateAsync(It.IsAny<Genero>()), Times.Never);
        }
    }
}
