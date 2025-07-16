using AutoFixture;
using FluentAssertions;
using Moq;
using SistemaLivros.Application.Common;
using SistemaLivros.Application.DTOs;
using SistemaLivros.Application.Interfaces;
using SistemaLivros.Application.Queries.Autores;
using SistemaLivros.Application.Queries.Autores.GetAllAutores;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace SistemaLivros.Tests.Application.Queries.Autores
{
    public class GetAllAutoresQueryHandlerTests
    {
        private readonly Mock<IAutorQueries> _autorQueriesMock;
        private readonly GetAllAutoresQueryHandler _handler;
        private readonly Fixture _fixture;

        public GetAllAutoresQueryHandlerTests()
        {
            _autorQueriesMock = new Mock<IAutorQueries>();
            _handler = new GetAllAutoresQueryHandler(_autorQueriesMock.Object);
            _fixture = new Fixture();
        }

        [Fact]
        public async Task RetornaTodosAutores()
        {
            // Arrange
            var pageNumber = 1;
            var pageSize = 10;
            var query = new GetAllAutoresQuery(pageNumber, pageSize);
            
            var autores = new List<AutorDto>
            {
                new AutorDto { Id = 1, Nome = "Gabriel García Márquez", Biografia = "Escritor colombiano" },
                new AutorDto { Id = 2, Nome = "J.K. Rowling", Biografia = "Escritora britânica" },
                new AutorDto { Id = 3, Nome = "George Orwell", Biografia = "Escritor britânico" }
            };

            var pagedResult = new PagedResult<AutorDto>(autores, autores.Count, pageNumber, pageSize);

            _autorQueriesMock.Setup(q => q.GetAllAsync(It.IsAny<PaginationParams>()))
                .ReturnsAsync(pagedResult);
                
            // Act
            var result = await _handler.Handle(query, CancellationToken.None);

            // Assert
            result.Should().NotBeNull();
            result.Items.Should().HaveCount(3);
            result.PageNumber.Should().Be(pageNumber);
            result.PageSize.Should().Be(pageSize);
            result.TotalPages.Should().Be(1);
            result.Items.ElementAt(0).Nome.Should().Be("Gabriel García Márquez");
            result.Items.ElementAt(1).Nome.Should().Be("J.K. Rowling");
            result.Items.ElementAt(2).Nome.Should().Be("George Orwell");
            _autorQueriesMock.Verify(q => q.GetAllAsync(It.IsAny<PaginationParams>()), Times.Once);
        }

        [Fact]
        public async Task RetornaListaVaziaQuandoNaoHaAutores()
        {
            // Arrange
            var pageNumber = 1;
            var pageSize = 10;
            var query = new GetAllAutoresQuery(pageNumber, pageSize);
            
            var autores = new List<AutorDto>();
            var pagedResult = new PagedResult<AutorDto>(autores, 0, pageNumber, pageSize);
            
            _autorQueriesMock.Setup(q => q.GetAllAsync(It.IsAny<PaginationParams>()))
                .ReturnsAsync(pagedResult);
                
            // Act
            var result = await _handler.Handle(query, CancellationToken.None);

            // Assert
            result.Should().NotBeNull();
            result.Items.Should().BeEmpty();
            result.PageNumber.Should().Be(pageNumber);
            result.PageSize.Should().Be(pageSize);
            result.TotalPages.Should().Be(0);
            _autorQueriesMock.Verify(q => q.GetAllAsync(It.IsAny<PaginationParams>()), Times.Once);
        }
    }
}
