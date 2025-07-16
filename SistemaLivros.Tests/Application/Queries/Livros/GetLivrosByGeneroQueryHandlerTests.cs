using AutoFixture;
using Moq;
using SistemaLivros.Application.DTOs;
using SistemaLivros.Application.Interfaces;
using SistemaLivros.Application.Queries.Livros;
using SistemaLivros.Application.Queries.Livros.GetLivrosByGenero;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace SistemaLivros.Tests.Application.Queries.Livros
{
    public class GetLivrosByGeneroQueryHandlerTests
    {
        private readonly Mock<ILivroQueries> _livroQueriesMock;
        private readonly GetLivrosByGeneroQueryHandler _handler;
        private readonly Fixture _fixture;

        public GetLivrosByGeneroQueryHandlerTests()
        {
            _livroQueriesMock = new Mock<ILivroQueries>();
            _handler = new GetLivrosByGeneroQueryHandler(_livroQueriesMock.Object);
            _fixture = new Fixture();
        }

        [Fact]
        public async Task RetornaLivrosPorGenero()
        {
            // Arrange
            var generoId = 1;
            var query = new GetLivrosByGeneroQuery(generoId);
            
            var livros = new List<LivroDto>
            {
                new LivroDto { 
                    Id = 1, 
                    Titulo = "Cem Anos de Solidão", 
                    Ano = 1967, 
                    Autor = new AutorDto { Id = 1, Nome = "Gabriel García Márquez" },
                    Generos = new List<GeneroSimplificadoDto> { new GeneroSimplificadoDto { Id = generoId, Nome = "Realismo Mágico" } }
                },
                new LivroDto { 
                    Id = 2, 
                    Titulo = "O Amor nos Tempos do Cólera", 
                    Ano = 1985, 
                    Autor = new AutorDto { Id = 1, Nome = "Gabriel García Márquez" },
                    Generos = new List<GeneroSimplificadoDto> { new GeneroSimplificadoDto { Id = generoId, Nome = "Realismo Mágico" } }
                }
            };

            _livroQueriesMock.Setup(q => q.GetByGeneroIdAsync(generoId))
                .ReturnsAsync(livros);
                
            // Act
            var result = await _handler.Handle(query, CancellationToken.None);

            // Assert
            var resultList = result.ToList();
            Assert.NotNull(result);
            Assert.Equal(2, resultList.Count);
            Assert.Equal("Cem Anos de Solidão", resultList[0].Titulo);
            Assert.Equal("O Amor nos Tempos do Cólera", resultList[1].Titulo);
            Assert.All(resultList, livro => Assert.Contains(livro.Generos, g => g.Id == generoId));
            _livroQueriesMock.Verify(q => q.GetByGeneroIdAsync(generoId), Times.Once);
        }

        [Fact]
        public async Task RetornaListaVaziaQuandoNaoHaLivrosParaGenero()
        {
            // Arrange
            var generoId = 99;
            var query = new GetLivrosByGeneroQuery(generoId);
            
            _livroQueriesMock.Setup(q => q.GetByGeneroIdAsync(generoId))
                .ReturnsAsync(new List<LivroDto>());
                
            // Act
            var result = await _handler.Handle(query, CancellationToken.None);

            // Assert
            Assert.NotNull(result);
            Assert.Empty(result);
            _livroQueriesMock.Verify(q => q.GetByGeneroIdAsync(generoId), Times.Once);
        }
    }
}
