using AutoFixture;
using Moq;
using SistemaLivros.Application.DTOs;
using SistemaLivros.Application.Interfaces;
using SistemaLivros.Application.Queries.Autores;
using SistemaLivros.Application.Queries.Autores.GetAutorById;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace SistemaLivros.Tests.Application.Queries.Autores
{
    public class GetAutorByIdQueryHandlerTests
    {
        private readonly Mock<IAutorQueries> _autorQueriesMock;
        private readonly GetAutorByIdQueryHandler _handler;
        private readonly Fixture _fixture;

        public GetAutorByIdQueryHandlerTests()
        {
            _autorQueriesMock = new Mock<IAutorQueries>();
            _handler = new GetAutorByIdQueryHandler(_autorQueriesMock.Object);
            _fixture = new Fixture();
        }

        [Fact]
        public async Task RetornaAutorPorId()
        {
            // Arrange
            var autorId = 1;
            var query = new GetAutorByIdQuery(autorId);
            
            var autorDto = new AutorDto
            {
                Id = autorId,
                Nome = "Gabriel García Márquez",
                Biografia = "Escritor colombiano"
            };

            _autorQueriesMock.Setup(q => q.GetByIdAsync(autorId))
                .ReturnsAsync(autorDto);
                
            // Act
            var result = await _handler.Handle(query, CancellationToken.None);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(autorId, result.Id);
            Assert.Equal(autorDto.Nome, result.Nome);
            Assert.Equal(autorDto.Biografia, result.Biografia);
            _autorQueriesMock.Verify(q => q.GetByIdAsync(autorId), Times.Once);
        }

        [Fact]
        public async Task RetornaNullQuandoAutorNaoExiste()
        {
            // Arrange
            var autorId = 99;
            var query = new GetAutorByIdQuery(autorId);
            
            _autorQueriesMock.Setup(q => q.GetByIdAsync(autorId))
                .ReturnsAsync((AutorDto)null);
                
            // Act
            var result = await _handler.Handle(query, CancellationToken.None);

            // Assert
            Assert.Null(result);
            _autorQueriesMock.Verify(q => q.GetByIdAsync(autorId), Times.Once);
        }
    }
}
