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
    public class UpdateAutorCommandHandlerTests
    {
        private readonly Mock<IAutorRepository> _autorRepositoryMock;
        private readonly UpdateAutorCommandHandler _handler;
        private readonly Fixture _fixture;

        public UpdateAutorCommandHandlerTests()
        {
            _autorRepositoryMock = new Mock<IAutorRepository>();
            _handler = new UpdateAutorCommandHandler(_autorRepositoryMock.Object);
            _fixture = new Fixture();
        }

        [Fact]
        public async Task AtualizaAutorComSucesso()
        {
            // Arrange
            var autorId = 1;
            var dataNascimento = new DateTime(1927, 3, 6);
            var command = new UpdateAutorCommand(autorId, "Gabriel García Márquez Atualizado", "Biografia atualizada", dataNascimento);
            
            // Criar o autor usando o construtor público
            var autor = new Autor("Gabriel García Márquez", "Biografia original", new DateTime(1927, 3, 6));
            
            // Usar reflection para definir o Id para teste
            typeof(Entity).GetProperty("Id").SetValue(autor, autorId);

            _autorRepositoryMock.Setup(r => r.GetByIdAsync(autorId))
                .ReturnsAsync(autor);
                
            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.True(result);
            _autorRepositoryMock.Verify(r => r.UpdateAsync(It.Is<Autor>(a => 
                a.Id == autorId && 
                a.Nome == command.Nome && 
                a.Biografia == command.Biografia &&
                a.DataNascimento == command.DataNascimento)), 
                Times.Once);
        }

        [Fact]
        public async Task RetornaFalsoQuandoAutorNaoExiste()
        {
            // Arrange
            var autorId = 99;
            var dataNascimento = new DateTime(1927, 3, 6);
            var command = new UpdateAutorCommand(autorId, "Gabriel García Márquez Atualizado", "Biografia atualizada", dataNascimento);
            
            _autorRepositoryMock.Setup(r => r.GetByIdAsync(autorId))
                .ReturnsAsync((Autor)null);
                
            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.False(result);
            _autorRepositoryMock.Verify(r => r.UpdateAsync(It.IsAny<Autor>()), Times.Never);
        }
    }
}
