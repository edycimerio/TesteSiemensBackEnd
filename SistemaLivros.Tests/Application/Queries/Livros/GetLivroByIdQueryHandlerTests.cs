using AutoFixture;
using Moq;
using SistemaLivros.Application.DTOs;
using SistemaLivros.Application.Interfaces;
using SistemaLivros.Application.Queries.Livros;
using SistemaLivros.Application.Queries.Livros.GetLivroById;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace SistemaLivros.Tests.Application.Queries.Livros
{
    public class GetLivroByIdQueryHandlerTests
    {
        private readonly Mock<ILivroQueries> _livroQueriesMock;
        private readonly GetLivroByIdQueryHandler _handler;
        private readonly Fixture _fixture;

        public GetLivroByIdQueryHandlerTests()
        {
            _livroQueriesMock = new Mock<ILivroQueries>();
            _handler = new GetLivroByIdQueryHandler(_livroQueriesMock.Object);
            _fixture = new Fixture();
        }

        [Fact]
        public async Task RetornaLivroPorId()
        {
            // Arrange
            var livroId = 1;
            var query = new GetLivroByIdQuery(livroId);
            
            var livroDto = new LivroDto
            {
                Id = livroId,
                Titulo = "Cem Anos de Solidão",
                Ano = 1967,
                AutorId = 1,
                GeneroId = 2
            };

            _livroQueriesMock.Setup(q => q.GetByIdAsync(livroId))
                .ReturnsAsync(livroDto);
                
            // Act
            var result = await _handler.Handle(query, CancellationToken.None);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(livroId, result.Id);
            Assert.Equal(livroDto.Titulo, result.Titulo);
            Assert.Equal(livroDto.Ano, result.Ano);
            Assert.Equal(livroDto.AutorId, result.AutorId);
            Assert.Equal(livroDto.GeneroId, result.GeneroId);
            _livroQueriesMock.Verify(q => q.GetByIdAsync(livroId), Times.Once);
        }

        [Fact]
        public async Task RetornaNullQuandoLivroNaoExiste()
        {
            // Arrange
            var livroId = 99;
            var query = new GetLivroByIdQuery(livroId);
            
            _livroQueriesMock.Setup(q => q.GetByIdAsync(livroId))
                .ReturnsAsync((LivroDto)null);
                
            // Act
            var result = await _handler.Handle(query, CancellationToken.None);

            // Assert
            Assert.Null(result);
            _livroQueriesMock.Verify(q => q.GetByIdAsync(livroId), Times.Once);
        }
    }
}
