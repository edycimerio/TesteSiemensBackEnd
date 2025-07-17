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
    public class DeleteAutorCommandHandlerTests
    {
        private readonly Mock<IAutorRepository> _autorRepositoryMock;
        private readonly Mock<ILivroRepository> _livroRepositoryMock;
        private readonly DeleteAutorCommandHandler _handler;
        private readonly Fixture _fixture;

        public DeleteAutorCommandHandlerTests()
        {
            _autorRepositoryMock = new Mock<IAutorRepository>();
            _livroRepositoryMock = new Mock<ILivroRepository>();
            _handler = new DeleteAutorCommandHandler(_autorRepositoryMock.Object, _livroRepositoryMock.Object);
            _fixture = new Fixture();
        }

        [Fact]
        public async Task RemoveAutorComSucesso()
        {
            // Arrange
            var autorId = 1;
            var command = new DeleteAutorCommand(autorId);

            // Criar o autor usando o construtor público
            var autor = new Autor("Gabriel García Márquez", "Escritor colombiano", new DateTime(1927, 3, 6));

            // Usar reflection para definir o Id para teste
            typeof(Entity).GetProperty("Id").SetValue(autor, autorId);

            // Configurar mock para retornar 0 livros associados
            _livroRepositoryMock.Setup(r => r.CountLivrosByAutorIdAsync(autorId))
                .ReturnsAsync(0);

            _autorRepositoryMock.Setup(r => r.GetByIdAsync(autorId))
                .ReturnsAsync(autor);

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.True(result);
            _autorRepositoryMock.Verify(r => r.RemoveAsync(autorId), Times.Once);
        }

        [Fact]
        public async Task RetornaFalsoQuandoAutorNaoExiste()
        {
            // Arrange
            var autorId = 99;
            var command = new DeleteAutorCommand(autorId);

            // Configurar mock para retornar 0 livros associados
            _livroRepositoryMock.Setup(r => r.CountLivrosByAutorIdAsync(autorId))
                .ReturnsAsync(0);

            _autorRepositoryMock.Setup(r => r.GetByIdAsync(autorId))
                .ReturnsAsync((Autor)null);

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.False(result);
            _autorRepositoryMock.Verify(r => r.RemoveAsync(It.IsAny<int>()), Times.Never);
        }

        [Fact]
        public async Task LancaExcecaoQuandoAutorTemLivrosAssociados()
        {
            // Arrange
            var autorId = 1;
            var command = new DeleteAutorCommand(autorId);
            
            // Configurar mock para retornar livros associados
            _livroRepositoryMock.Setup(r => r.CountLivrosByAutorIdAsync(autorId))
                .ReturnsAsync(2); // Simula 2 livros associados
                
            // Act & Assert
            var exception = await Assert.ThrowsAsync<InvalidOperationException>(
                async () => await _handler.Handle(command, CancellationToken.None));
                
            // Verifica a mensagem de erro
            Assert.Contains("Não é possível excluir o autor", exception.Message);
            Assert.Contains("2 livros", exception.Message);
            
            // Verifica que o método de remoção nunca foi chamado
            _autorRepositoryMock.Verify(r => r.RemoveAsync(It.IsAny<int>()), Times.Never);
        }
    }
}
