using AutoFixture;
using Moq;
using SistemaLivros.Application.DTOs;
using SistemaLivros.Application.Interfaces;
using SistemaLivros.Application.Queries.Generos;
using SistemaLivros.Application.Queries.Generos.GetGeneroDetalhes;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace SistemaLivros.Tests.Application.Queries.Generos
{
    public class GetGeneroDetalhesQueryHandlerTests
    {
        private readonly Mock<IGeneroQueries> _generoQueriesMock;
        private readonly GetGeneroDetalhesQueryHandler _handler;
        private readonly Fixture _fixture;

        public GetGeneroDetalhesQueryHandlerTests()
        {
            _generoQueriesMock = new Mock<IGeneroQueries>();
            _handler = new GetGeneroDetalhesQueryHandler(_generoQueriesMock.Object);
            _fixture = new Fixture();
        }

        [Fact]
        public async Task RetornaGeneroDetalhes()
        {
            // Arrange
            var generoId = 1;
            var query = new GetGeneroDetalhesQuery(generoId);
            
            var livros = new List<LivroDto>
            {
                new LivroDto { Id = 1, Titulo = "Livro 1", Ano = 2020 },
                new LivroDto { Id = 2, Titulo = "Livro 2", Ano = 2021 }
            };
            
            var generoDetalhesDto = new GeneroDetalhesDto
            {
                Id = generoId,
                Nome = "Ficção Científica",
                Descricao = "Livros de ficção científica",
                Livros = livros
            };

            _generoQueriesMock.Setup(q => q.GetDetalhesAsync(generoId))
                .ReturnsAsync(generoDetalhesDto);
                
            // Act
            var result = await _handler.Handle(query, CancellationToken.None);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(generoId, result.Id);
            Assert.Equal(generoDetalhesDto.Nome, result.Nome);
            Assert.Equal(generoDetalhesDto.Descricao, result.Descricao);
            Assert.Equal(2, result.Livros.Count);
            _generoQueriesMock.Verify(q => q.GetDetalhesAsync(generoId), Times.Once);
        }

        [Fact]
        public async Task RetornaNullQuandoGeneroNaoExiste()
        {
            // Arrange
            var generoId = 99;
            var query = new GetGeneroDetalhesQuery(generoId);
            
            _generoQueriesMock.Setup(q => q.GetDetalhesAsync(generoId))
                .ReturnsAsync((GeneroDetalhesDto)null);
                
            // Act
            var result = await _handler.Handle(query, CancellationToken.None);

            // Assert
            Assert.Null(result);
            _generoQueriesMock.Verify(q => q.GetDetalhesAsync(generoId), Times.Once);
        }
    }
}
