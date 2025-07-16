using AutoFixture;
using Moq;
using SistemaLivros.Application.Common;
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
            int pageNumber = 1;
            int pageSize = 10;
            var query = new GetLivrosByAutorQuery(autorId, pageNumber, pageSize);
            
            var livros = new List<LivroDto>
            {
                new LivroDto { 
                    Id = 1, 
                    Titulo = "Cem Anos de Solidão", 
                    Ano = 1967, 
                    Autor = new AutorDto { Id = autorId, Nome = "Gabriel García Márquez" },
                    Generos = new List<GeneroSimplificadoDto> { new GeneroSimplificadoDto { Id = 1, Nome = "Realismo Mágico" } }
                },
                new LivroDto { 
                    Id = 2, 
                    Titulo = "O Amor nos Tempos do Cólera", 
                    Ano = 1985, 
                    Autor = new AutorDto { Id = autorId, Nome = "Gabriel García Márquez" },
                    Generos = new List<GeneroSimplificadoDto> { new GeneroSimplificadoDto { Id = 2, Nome = "Romance" } }
                }
            };
            
            var pagedResult = new PagedResult<LivroDto>
            {
                Items = livros,
                PageNumber = pageNumber,
                PageSize = pageSize,
                TotalCount = livros.Count,
                TotalPages = 1
            };

            _livroQueriesMock.Setup(q => q.GetByAutorIdAsync(autorId, It.Is<PaginationParams>(p => p.PageNumber == pageNumber && p.PageSize == pageSize)))
                .ReturnsAsync(pagedResult);
                
            // Act
            var result = await _handler.Handle(query, CancellationToken.None);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(2, result.Items.Count);
            Assert.Equal("Cem Anos de Solidão", result.Items[0].Titulo);
            Assert.Equal("O Amor nos Tempos do Cólera", result.Items[1].Titulo);
            Assert.All(result.Items, livro => Assert.Equal(autorId, livro.Autor.Id));
            Assert.Equal(pageNumber, result.PageNumber);
            Assert.Equal(pageSize, result.PageSize);
            Assert.Equal(livros.Count, result.TotalCount);
            Assert.Equal(1, result.TotalPages);
            _livroQueriesMock.Verify(q => q.GetByAutorIdAsync(autorId, It.Is<PaginationParams>(p => p.PageNumber == pageNumber && p.PageSize == pageSize)), Times.Once);
        }

        [Fact]
        public async Task RetornaListaVaziaQuandoNaoHaLivrosParaAutor()
        {
            // Arrange
            var autorId = 99;
            int pageNumber = 1;
            int pageSize = 10;
            var query = new GetLivrosByAutorQuery(autorId, pageNumber, pageSize);
            
            var emptyPagedResult = new PagedResult<LivroDto>
            {
                Items = new List<LivroDto>(),
                PageNumber = pageNumber,
                PageSize = pageSize,
                TotalCount = 0,
                TotalPages = 0
            };
            
            _livroQueriesMock.Setup(q => q.GetByAutorIdAsync(autorId, It.Is<PaginationParams>(p => p.PageNumber == pageNumber && p.PageSize == pageSize)))
                .ReturnsAsync(emptyPagedResult);
                
            // Act
            var result = await _handler.Handle(query, CancellationToken.None);

            // Assert
            Assert.NotNull(result);
            Assert.Empty(result.Items);
            Assert.Equal(pageNumber, result.PageNumber);
            Assert.Equal(pageSize, result.PageSize);
            Assert.Equal(0, result.TotalCount);
            Assert.Equal(0, result.TotalPages);
            _livroQueriesMock.Verify(q => q.GetByAutorIdAsync(autorId, It.Is<PaginationParams>(p => p.PageNumber == pageNumber && p.PageSize == pageSize)), Times.Once);
        }
    }
}
