using AutoFixture;
using Moq;
using SistemaLivros.Application.DTOs;
using SistemaLivros.Application.Interfaces;
using SistemaLivros.Application.Queries.Autores;
using SistemaLivros.Application.Queries.Autores.GetAutorDetalhes;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace SistemaLivros.Tests.Application.Queries.Autores
{
    public class GetAutorDetalhesQueryHandlerTests
    {
        private readonly Mock<IAutorQueries> _autorQueriesMock;
        private readonly GetAutorDetalhesQueryHandler _handler;
        private readonly Fixture _fixture;

        public GetAutorDetalhesQueryHandlerTests()
        {
            _autorQueriesMock = new Mock<IAutorQueries>();
            _handler = new GetAutorDetalhesQueryHandler(_autorQueriesMock.Object);
            _fixture = new Fixture();
        }

        [Fact]
        public async Task RetornaAutorDetalhes()
        {
            // Arrange
            var autorId = 1;
            var query = new GetAutorDetalhesQuery(autorId);
            
            var livros = new List<LivroDto>
            {
                new LivroDto { Id = 1, Titulo = "Cem Anos de Solidão", Ano = 1967 },
                new LivroDto { Id = 2, Titulo = "O Amor nos Tempos do Cólera", Ano = 1985 }
            };
            
            var autorDetalhesDto = new AutorDetalhesDto
            {
                Id = autorId,
                Nome = "Gabriel García Márquez",
                Biografia = "Escritor colombiano",
                Livros = livros
            };

            _autorQueriesMock.Setup(q => q.GetDetalhesAsync(autorId))
                .ReturnsAsync(autorDetalhesDto);
                
            // Act
            var result = await _handler.Handle(query, CancellationToken.None);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(autorId, result.Id);
            Assert.Equal(autorDetalhesDto.Nome, result.Nome);
            Assert.Equal(autorDetalhesDto.Biografia, result.Biografia);
            Assert.Equal(2, result.Livros.Count);
            _autorQueriesMock.Verify(q => q.GetDetalhesAsync(autorId), Times.Once);
        }

        [Fact]
        public async Task RetornaNullQuandoAutorNaoExiste()
        {
            // Arrange
            var autorId = 99;
            var query = new GetAutorDetalhesQuery(autorId);
            
            _autorQueriesMock.Setup(q => q.GetDetalhesAsync(autorId))
                .ReturnsAsync((AutorDetalhesDto)null);
                
            // Act
            var result = await _handler.Handle(query, CancellationToken.None);

            // Assert
            Assert.Null(result);
            _autorQueriesMock.Verify(q => q.GetDetalhesAsync(autorId), Times.Once);
        }
    }
}
