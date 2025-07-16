using AutoFixture;
using Moq;
using SistemaLivros.Application.Commands.Livros;
using SistemaLivros.Domain.Entities;
using SistemaLivros.Domain.Interfaces;
using System;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace SistemaLivros.Tests.Application.Commands.Livros
{
    public class DeleteLivroCommandHandlerTests
    {
        private readonly Mock<ILivroRepository> _livroRepositoryMock;
        private readonly DeleteLivroCommandHandler _handler;
        private readonly Fixture _fixture;

        public DeleteLivroCommandHandlerTests()
        {
            _livroRepositoryMock = new Mock<ILivroRepository>();
            _handler = new DeleteLivroCommandHandler(_livroRepositoryMock.Object);
            _fixture = new Fixture();
        }

        [Fact]
        public async Task RemoveLivroComSucesso()
        {
            // Arrange
            var livroId = 1;
            var command = new DeleteLivroCommand(livroId);

            // Criar o livro usando o construtor público
            var livro = new Livro("Cem Anos de Solidão", 1967, 1);

            // Usar reflection para definir o Id para teste
            typeof(Entity).GetProperty("Id").SetValue(livro, livroId);

            _livroRepositoryMock.Setup(r => r.GetByIdAsync(livroId))
                .ReturnsAsync(livro);

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.True(result);
            _livroRepositoryMock.Verify(r => r.RemoveAsync(livroId), Times.Once);
        }

        [Fact]
        public async Task RetornaFalsoQuandoLivroNaoExiste()
        {
            // Arrange
            var livroId = 99;
            var command = new DeleteLivroCommand(livroId);

            _livroRepositoryMock.Setup(r => r.GetByIdAsync(livroId))
                .ReturnsAsync((Livro)null);

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.False(result);
            _livroRepositoryMock.Verify(r => r.RemoveAsync(It.IsAny<int>()), Times.Never);
        }
    }
}