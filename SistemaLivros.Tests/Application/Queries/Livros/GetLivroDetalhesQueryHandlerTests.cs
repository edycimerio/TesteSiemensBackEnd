using AutoFixture;
using Moq;
using SistemaLivros.Application.DTOs;
using SistemaLivros.Application.Interfaces;
using SistemaLivros.Application.Queries.Livros;
using SistemaLivros.Application.Queries.Livros.GetLivroDetalhes;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace SistemaLivros.Tests.Application.Queries.Livros
{
    public class GetLivroDetalhesQueryHandlerTests
    {
        private readonly Mock<ILivroQueries> _livroQueriesMock;
        private readonly GetLivroDetalhesQueryHandler _handler;
        private readonly Fixture _fixture;

        public GetLivroDetalhesQueryHandlerTests()
        {
            _livroQueriesMock = new Mock<ILivroQueries>();
            _handler = new GetLivroDetalhesQueryHandler(_livroQueriesMock.Object);
            _fixture = new Fixture();
        }

        [Fact]
        public async Task RetornaLivroDetalhes()
        {
            // Arrange
            var livroId = 1;
            var query = new GetLivroDetalhesQuery(livroId);
            
            var livroDetalhesDto = new LivroDetalhesDto
            {
                Id = livroId,
                Titulo = "Cem Anos de Solidão",
                Ano = 1967
            };

            _livroQueriesMock.Setup(q => q.GetDetalhesAsync(livroId))
                .ReturnsAsync(livroDetalhesDto);
                
            // Act
            var result = await _handler.Handle(query, CancellationToken.None);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(livroId, result.Id);
            Assert.Equal(livroDetalhesDto.Titulo, result.Titulo);
            Assert.Equal(livroDetalhesDto.Ano, result.Ano);
            _livroQueriesMock.Verify(q => q.GetDetalhesAsync(livroId), Times.Once);
        }

        [Fact]
        public async Task RetornaNullQuandoLivroNaoExiste()
        {
            // Arrange
            var livroId = 99;
            var query = new GetLivroDetalhesQuery(livroId);
            
            _livroQueriesMock.Setup(q => q.GetDetalhesAsync(livroId))
                .ReturnsAsync((LivroDetalhesDto)null);
                
            // Act
            var result = await _handler.Handle(query, CancellationToken.None);

            // Assert
            Assert.Null(result);
            _livroQueriesMock.Verify(q => q.GetDetalhesAsync(livroId), Times.Once);
        }
    }
}
