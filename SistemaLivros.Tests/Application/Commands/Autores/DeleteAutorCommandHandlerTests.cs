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
        private readonly DeleteAutorCommandHandler _handler;
        private readonly Fixture _fixture;

        public DeleteAutorCommandHandlerTests()
        {
            _autorRepositoryMock = new Mock<IAutorRepository>();
            _handler = new DeleteAutorCommandHandler(_autorRepositoryMock.Object);
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
            
            _autorRepositoryMock.Setup(r => r.GetByIdAsync(autorId))
                .ReturnsAsync((Autor)null);
                
            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.False(result);
            _autorRepositoryMock.Verify(r => r.RemoveAsync(It.IsAny<int>()), Times.Never);
        }
    }
}
