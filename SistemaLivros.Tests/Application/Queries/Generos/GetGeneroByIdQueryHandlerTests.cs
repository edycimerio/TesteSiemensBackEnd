using AutoFixture;
using Moq;
using SistemaLivros.Application.DTOs;
using SistemaLivros.Application.Interfaces;
using SistemaLivros.Application.Queries.Generos;
using SistemaLivros.Application.Queries.Generos.GetGeneroById;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace SistemaLivros.Tests.Application.Queries.Generos
{
    public class GetGeneroByIdQueryHandlerTests
    {
        private readonly Mock<IGeneroQueries> _generoQueriesMock;
        private readonly GetGeneroByIdQueryHandler _handler;
        private readonly Fixture _fixture;

        public GetGeneroByIdQueryHandlerTests()
        {
            _generoQueriesMock = new Mock<IGeneroQueries>();
            _handler = new GetGeneroByIdQueryHandler(_generoQueriesMock.Object);
            _fixture = new Fixture();
        }

        [Fact]
        public async Task RetornaGeneroPorId()
        {
            // Arrange
            var generoId = 1;
            var query = new GetGeneroByIdQuery(generoId);
            
            var generoDto = new GeneroDto
            {
                Id = generoId,
                Nome = "Ficção Científica",
                Descricao = "Livros de ficção científica"
            };

            _generoQueriesMock.Setup(q => q.GetByIdAsync(generoId))
                .ReturnsAsync(generoDto);
                
            // Act
            var result = await _handler.Handle(query, CancellationToken.None);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(generoId, result.Id);
            Assert.Equal(generoDto.Nome, result.Nome);
            Assert.Equal(generoDto.Descricao, result.Descricao);
            _generoQueriesMock.Verify(q => q.GetByIdAsync(generoId), Times.Once);
        }

        [Fact]
        public async Task RetornaNullQuandoGeneroNaoExiste()
        {
            // Arrange
            var generoId = 99;
            var query = new GetGeneroByIdQuery(generoId);
            
            _generoQueriesMock.Setup(q => q.GetByIdAsync(generoId))
                .ReturnsAsync((GeneroDto)null);
                
            // Act
            var result = await _handler.Handle(query, CancellationToken.None);

            // Assert
            Assert.Null(result);
            _generoQueriesMock.Verify(q => q.GetByIdAsync(generoId), Times.Once);
        }
    }
}
