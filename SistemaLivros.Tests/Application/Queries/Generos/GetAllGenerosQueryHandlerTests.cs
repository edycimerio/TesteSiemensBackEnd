using AutoFixture;
using Moq;
using SistemaLivros.Application.DTOs;
using SistemaLivros.Application.Interfaces;
using SistemaLivros.Application.Queries.Generos.GetAllGeneros;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace SistemaLivros.Tests.Application.Queries.Generos
{
    public class GetAllGenerosQueryHandlerTests
    {
        private readonly Mock<IGeneroQueries> _generoQueriesMock;
        private readonly GetAllGenerosQueryHandler _handler;
        private readonly Fixture _fixture;

        public GetAllGenerosQueryHandlerTests()
        {
            _generoQueriesMock = new Mock<IGeneroQueries>();
            _handler = new GetAllGenerosQueryHandler(_generoQueriesMock.Object);
            _fixture = new Fixture();
        }

        [Fact]
        public async Task RetornaTodosGeneros()
        {
            // Arrange
            var generos = new List<GeneroDto>
            {
                new GeneroDto { Id = 1, Nome = "Ficção Científica", Descricao = "Livros de ficção científica" },
                new GeneroDto { Id = 2, Nome = "Romance", Descricao = "Livros de romance" },
                new GeneroDto { Id = 3, Nome = "Terror", Descricao = "Livros de terror" }
            };

            _generoQueriesMock.Setup(q => q.GetAllAsync())
                .ReturnsAsync(generos);

            var query = new GetAllGenerosQuery();

            // Act
            var result = await _handler.Handle(query, CancellationToken.None);

            // Assert
            var resultList = result.ToList();
            Assert.Equal(3, resultList.Count);
            Assert.Contains(result, g => g.Id == 1 && g.Nome == "Ficção Científica");
            Assert.Contains(result, g => g.Id == 2 && g.Nome == "Romance");
            Assert.Contains(result, g => g.Id == 3 && g.Nome == "Terror");
            _generoQueriesMock.Verify(q => q.GetAllAsync(), Times.Once);
        }
    }
}
