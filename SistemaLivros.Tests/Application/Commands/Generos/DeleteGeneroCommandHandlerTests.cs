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
    public class DeleteGeneroCommandHandlerTests
    {
        private readonly Mock<IGeneroRepository> _generoRepositoryMock;
        private readonly DeleteGeneroCommandHandler _handler;
        private readonly Fixture _fixture;

        public DeleteGeneroCommandHandlerTests()
        {
            _generoRepositoryMock = new Mock<IGeneroRepository>();
            _handler = new DeleteGeneroCommandHandler(_generoRepositoryMock.Object);
            _fixture = new Fixture();
        }

        [Fact]
        public async Task RemoveGeneroComSucesso()
        {
            // Arrange
            var generoId = 1;
            var command = new DeleteGeneroCommand(generoId);
            
            // Criar o gênero usando o construtor público
            var genero = new Genero("Ficção Científica", "Descrição do gênero");
            
            // Usar reflection para definir o Id para teste
            typeof(Entity).GetProperty("Id").SetValue(genero, generoId);

            // Configurar mock para retornar 0 associações com livros
            _generoRepositoryMock.Setup(r => r.CountLivrosByGeneroIdAsync(generoId))
                .ReturnsAsync(0);

            _generoRepositoryMock.Setup(r => r.GetByIdAsync(generoId))
                .ReturnsAsync(genero);
                
            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.True(result);
            _generoRepositoryMock.Verify(r => r.RemoveAsync(generoId), Times.Once);
        }

        [Fact]
        public async Task RetornaFalsoQuandoGeneroNaoExiste()
        {
            // Arrange
            var generoId = 99;
            var command = new DeleteGeneroCommand(generoId);
            
            // Configurar mock para retornar 0 associações com livros
            _generoRepositoryMock.Setup(r => r.CountLivrosByGeneroIdAsync(generoId))
                .ReturnsAsync(0);
            
            _generoRepositoryMock.Setup(r => r.GetByIdAsync(generoId))
                .ReturnsAsync((Genero)null);
                
            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.False(result);
            _generoRepositoryMock.Verify(r => r.RemoveAsync(It.IsAny<int>()), Times.Never);
        }
        
        [Fact]
        public async Task LancaExcecaoQuandoGeneroTemLivrosAssociados()
        {
            // Arrange
            var generoId = 1;
            var command = new DeleteGeneroCommand(generoId);
            
            // Configurar mock para retornar associações com livros
            _generoRepositoryMock.Setup(r => r.CountLivrosByGeneroIdAsync(generoId))
                .ReturnsAsync(3); // Simula 3 livros associados ao gênero
                
            // Act & Assert
            var exception = await Assert.ThrowsAsync<InvalidOperationException>(
                async () => await _handler.Handle(command, CancellationToken.None));
                
            // Verifica a mensagem de erro
            Assert.Contains("Não é possível excluir o gênero", exception.Message);
            Assert.Contains("3 livros", exception.Message);
            
            // Verifica que o método de remoção nunca foi chamado
            _generoRepositoryMock.Verify(r => r.RemoveAsync(It.IsAny<int>()), Times.Never);
        }
    }
}
