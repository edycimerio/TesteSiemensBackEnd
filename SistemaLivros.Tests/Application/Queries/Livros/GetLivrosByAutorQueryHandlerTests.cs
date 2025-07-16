using AutoFixture;
using Moq;
using SistemaLivros.Application.DTOs;
using SistemaLivros.Application.Interfaces;
using SistemaLivros.Application.Queries.Livros;
using SistemaLivros.Application.Queries.Livros.GetLivrosByAutor;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace SistemaLivros.Tests.Application.Queries.Livros
{
    public class GetLivrosByAutorQueryHandlerTests
    {
        private readonly Mock<ILivroQueries> _livroQueriesMock;
        private readonly GetLivrosByAutorQueryHandler _handler;
        private readonly Fixture _fixture;

        public GetLivrosByAutorQueryHandlerTests()
        {
            _livroQueriesMock = new Mock<ILivroQueries>();
            _handler = new GetLivrosByAutorQueryHandler(_livroQueriesMock.Object);
            _fixture = new Fixture();
        }

        [Fact]
        public async Task RetornaLivrosPorAutor()
        {
            // Arrange
            var autorId = 1;
            var query = new GetLivrosByAutorQuery(autorId);
            
            var livros = new List<LivroDto>
            {
                new LivroDto { Id = 1, Titulo = "Cem Anos de Solidão", Ano = 1967, AutorId = autorId, GeneroId = 1 },
                new LivroDto { Id = 2, Titulo = "O Amor nos Tempos do Cólera", Ano = 1985, AutorId = autorId, GeneroId = 2 }
            };

            _livroQueriesMock.Setup(q => q.GetByAutorIdAsync(autorId))
                .ReturnsAsync(livros);
                
            // Act
            var result = await _handler.Handle(query, CancellationToken.None);

            // Assert
            var resultList = result.ToList();
            Assert.NotNull(result);
            Assert.Equal(2, resultList.Count);
            Assert.Equal("Cem Anos de Solidão", resultList[0].Titulo);
            Assert.Equal("O Amor nos Tempos do Cólera", resultList[1].Titulo);
            Assert.All(resultList, livro => Assert.Equal(autorId, livro.AutorId));
            _livroQueriesMock.Verify(q => q.GetByAutorIdAsync(autorId), Times.Once);
        }

        [Fact]
        public async Task RetornaListaVaziaQuandoNaoHaLivrosParaAutor()
        {
            // Arrange
            var autorId = 99;
            var query = new GetLivrosByAutorQuery(autorId);
            
            _livroQueriesMock.Setup(q => q.GetByAutorIdAsync(autorId))
                .ReturnsAsync(new List<LivroDto>());
                
            // Act
            var result = await _handler.Handle(query, CancellationToken.None);

            // Assert
            Assert.NotNull(result);
            Assert.Empty(result);
            _livroQueriesMock.Verify(q => q.GetByAutorIdAsync(autorId), Times.Once);
        }
    }
}
