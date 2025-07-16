using AutoFixture;
using Moq;
using SistemaLivros.Application.Common;
using SistemaLivros.Application.DTOs;
using SistemaLivros.Application.Interfaces;
using SistemaLivros.Application.Queries.Livros;
using SistemaLivros.Application.Queries.Livros.GetAllLivros;
using System;
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
        public async Task RetornaTodosLivrosPaginados()
        {
            // Arrange
            int pageNumber = 1;
            int pageSize = 2;
            var query = new GetAllLivrosQuery(pageNumber, pageSize);
            
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

            var pagedResult = new PagedResult<LivroDto>
            {
                Items = livros.Take(pageSize).ToList(),
                PageNumber = pageNumber,
                PageSize = pageSize,
                TotalCount = livros.Count,
                TotalPages = (int)Math.Ceiling(livros.Count / (double)pageSize)
            };

            _livroQueriesMock.Setup(q => q.GetAllAsync(It.Is<PaginationParams>(p => p.PageNumber == pageNumber && p.PageSize == pageSize)))
                .ReturnsAsync(pagedResult);
                
            // Act
            var result = await _handler.Handle(query, CancellationToken.None);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(pageSize, result.Items.Count);
            Assert.Equal(pageNumber, result.PageNumber);
            Assert.Equal(pageSize, result.PageSize);
            Assert.Equal(livros.Count, result.TotalCount);
            Assert.Equal(2, result.TotalPages); // 3 itens com pageSize 2 = 2 páginas
            Assert.Equal("Cem Anos de Solidão", result.Items[0].Titulo);
            Assert.Equal(1949, result.Items[1].Ano);
            _livroQueriesMock.Verify(q => q.GetAllAsync(It.IsAny<PaginationParams>()), Times.Once);
        }

        [Fact]
        public async Task RetornaListaVaziaQuandoNaoHaLivros()
        {
            // Arrange
            int pageNumber = 1;
            int pageSize = 10;
            var query = new GetAllLivrosQuery(pageNumber, pageSize);
            
            var emptyPagedResult = new PagedResult<LivroDto>
            {
                Items = new List<LivroDto>(),
                PageNumber = pageNumber,
                PageSize = pageSize,
                TotalCount = 0,
                TotalPages = 0
            };
            
            _livroQueriesMock.Setup(q => q.GetAllAsync(It.Is<PaginationParams>(p => p.PageNumber == pageNumber && p.PageSize == pageSize)))
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
            _livroQueriesMock.Verify(q => q.GetAllAsync(It.IsAny<PaginationParams>()), Times.Once);
        }
    }
}
