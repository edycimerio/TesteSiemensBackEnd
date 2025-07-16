using AutoFixture;
using Moq;
using SistemaLivros.Application.DTOs;
using SistemaLivros.Application.Interfaces;
using SistemaLivros.Application.Queries.Livros;
using SistemaLivros.Application.Queries.Livros.GetAllLivros;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace SistemaLivros.Tests.Application.Queries.Livros
{
    public class GetAllLivrosQueryHandlerTests
    {
        private readonly Mock<ILivroQueries> _livroQueriesMock;
        private readonly GetAllLivrosQueryHandler _handler;
        private readonly Fixture _fixture;

        public GetAllLivrosQueryHandlerTests()
        {
            _livroQueriesMock = new Mock<ILivroQueries>();
            _handler = new GetAllLivrosQueryHandler(_livroQueriesMock.Object);
            _fixture = new Fixture();
        }

        [Fact]
        public async Task RetornaTodosLivros()
        {
            // Arrange
            var query = new GetAllLivrosQuery();
            
            var livros = new List<LivroDto>
            {
                new LivroDto { 
                    Id = 1, 
                    Titulo = "Cem Anos de Solidão", 
                    Ano = 1967, 
                    Autor = new AutorDto { Id = 1, Nome = "Gabriel García Márquez" },
                    Generos = new List<GeneroSimplificadoDto> { new GeneroSimplificadoDto { Id = 1, Nome = "Realismo Mágico" } }
                },
                new LivroDto { 
                    Id = 2, 
                    Titulo = "1984", 
                    Ano = 1949, 
                    Autor = new AutorDto { Id = 2, Nome = "George Orwell" },
                    Generos = new List<GeneroSimplificadoDto> { new GeneroSimplificadoDto { Id = 2, Nome = "Ficção Distópica" } }
                },
                new LivroDto { 
                    Id = 3, 
                    Titulo = "O Senhor dos Anéis", 
                    Ano = 1954, 
                    Autor = new AutorDto { Id = 3, Nome = "J.R.R. Tolkien" },
                    Generos = new List<GeneroSimplificadoDto> { new GeneroSimplificadoDto { Id = 3, Nome = "Fantasia" } }
                }
            };

            _livroQueriesMock.Setup(q => q.GetAllAsync())
                .ReturnsAsync(livros);
                
            // Act
            var result = await _handler.Handle(query, CancellationToken.None);

            // Assert
            var resultList = result.ToList();
            Assert.NotNull(result);
            Assert.Equal(3, resultList.Count);
            Assert.Equal("Cem Anos de Solidão", resultList[0].Titulo);
            Assert.Equal(1949, resultList[1].Ano);
            Assert.Equal(3, resultList[2].Id);
            _livroQueriesMock.Verify(q => q.GetAllAsync(), Times.Once);
        }

        [Fact]
        public async Task RetornaListaVaziaQuandoNaoHaLivros()
        {
            // Arrange
            var query = new GetAllLivrosQuery();
            
            _livroQueriesMock.Setup(q => q.GetAllAsync())
                .ReturnsAsync(new List<LivroDto>());
                
            // Act
            var result = await _handler.Handle(query, CancellationToken.None);

            // Assert
            Assert.NotNull(result);
            Assert.Empty(result);
            _livroQueriesMock.Verify(q => q.GetAllAsync(), Times.Once);
        }
    }
}
