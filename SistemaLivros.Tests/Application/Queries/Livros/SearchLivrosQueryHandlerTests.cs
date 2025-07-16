using AutoFixture;
using Moq;
using SistemaLivros.Application.DTOs;
using SistemaLivros.Application.Interfaces;
using SistemaLivros.Application.Queries.Livros;
using SistemaLivros.Application.Queries.Livros.SearchLivros;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace SistemaLivros.Tests.Application.Queries.Livros
{
    public class SearchLivrosQueryHandlerTests
    {
        private readonly Mock<ILivroQueries> _livroQueriesMock;
        private readonly SearchLivrosQueryHandler _handler;
        private readonly Fixture _fixture;

        public SearchLivrosQueryHandlerTests()
        {
            _livroQueriesMock = new Mock<ILivroQueries>();
            _handler = new SearchLivrosQueryHandler(_livroQueriesMock.Object);
            _fixture = new Fixture();
        }

        [Fact]
        public async Task RetornaLivrosPorTermoDeBusca()
        {
            // Arrange
            var termo = "anos";
            var query = new SearchLivrosQuery(termo);
            
            var livros = new List<LivroDto>
            {
                new LivroDto { Id = 1, Titulo = "Cem Anos de Solidão", Ano = 1967, AutorId = 1, GeneroId = 1 },
                new LivroDto { Id = 3, Titulo = "Vinte Anos Depois", Ano = 1845, AutorId = 2, GeneroId = 3 }
            };

            _livroQueriesMock.Setup(q => q.SearchAsync(termo))
                .ReturnsAsync(livros);
                
            // Act
            var result = await _handler.Handle(query, CancellationToken.None);

            // Assert
            var resultList = result.ToList();
            Assert.NotNull(result);
            Assert.Equal(2, resultList.Count);
            Assert.Contains(resultList, l => l.Titulo == "Cem Anos de Solidão");
            Assert.Contains(resultList, l => l.Titulo == "Vinte Anos Depois");
            _livroQueriesMock.Verify(q => q.SearchAsync(termo), Times.Once);
        }

        [Fact]
        public async Task RetornaListaVaziaQuandoNaoHaResultados()
        {
            // Arrange
            var termo = "termoinexistente";
            var query = new SearchLivrosQuery(termo);
            
            _livroQueriesMock.Setup(q => q.SearchAsync(termo))
                .ReturnsAsync(new List<LivroDto>());
                
            // Act
            var result = await _handler.Handle(query, CancellationToken.None);

            // Assert
            Assert.NotNull(result);
            Assert.Empty(result);
            _livroQueriesMock.Verify(q => q.SearchAsync(termo), Times.Once);
        }
    }
}
