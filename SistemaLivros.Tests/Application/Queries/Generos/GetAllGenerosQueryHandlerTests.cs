using AutoFixture;
using FluentAssertions;
using Moq;
using SistemaLivros.Application.Common;
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
            var pageNumber = 1;
            var pageSize = 10;
            var generos = new List<GeneroDto>
            {
                new GeneroDto { Id = 1, Nome = "Ficção Científica", Descricao = "Livros de ficção científica" },
                new GeneroDto { Id = 2, Nome = "Romance", Descricao = "Livros de romance" },
                new GeneroDto { Id = 3, Nome = "Terror", Descricao = "Livros de terror" }
            };

            var pagedResult = new PagedResult<GeneroDto>(generos, generos.Count, pageNumber, pageSize);

            _generoQueriesMock.Setup(q => q.GetAllAsync(It.IsAny<PaginationParams>()))
                .ReturnsAsync(pagedResult);

            var query = new GetAllGenerosQuery(pageNumber, pageSize);

            // Act
            var result = await _handler.Handle(query, CancellationToken.None);

            // Assert
            result.Should().NotBeNull();
            result.Items.Should().HaveCount(3);
            result.PageNumber.Should().Be(pageNumber);
            result.PageSize.Should().Be(pageSize);
            result.TotalPages.Should().Be(1);
            result.Items.Should().Contain(g => g.Id == 1 && g.Nome == "Ficção Científica");
            result.Items.Should().Contain(g => g.Id == 2 && g.Nome == "Romance");
            result.Items.Should().Contain(g => g.Id == 3 && g.Nome == "Terror");
            _generoQueriesMock.Verify(q => q.GetAllAsync(It.IsAny<PaginationParams>()), Times.Once);
        }
    }
}
