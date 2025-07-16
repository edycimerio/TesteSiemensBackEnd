using AutoFixture;
using Moq;
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
            var query = new GetAllAutoresQuery();
            
            var autores = new List<AutorDto>
            {
                new AutorDto { Id = 1, Nome = "Gabriel García Márquez", Biografia = "Escritor colombiano" },
                new AutorDto { Id = 2, Nome = "J.K. Rowling", Biografia = "Escritora britânica" },
                new AutorDto { Id = 3, Nome = "George Orwell", Biografia = "Escritor britânico" }
            };

            _autorQueriesMock.Setup(q => q.GetAllAsync())
                .ReturnsAsync(autores);
                
            // Act
            var result = await _handler.Handle(query, CancellationToken.None);

            // Assert
            var resultList = result.ToList();
            Assert.NotNull(result);
            Assert.Equal(3, resultList.Count);
            Assert.Equal("Gabriel García Márquez", resultList[0].Nome);
            Assert.Equal("J.K. Rowling", resultList[1].Nome);
            Assert.Equal("George Orwell", resultList[2].Nome);
            _autorQueriesMock.Verify(q => q.GetAllAsync(), Times.Once);
        }

        [Fact]
        public async Task RetornaListaVaziaQuandoNaoHaAutores()
        {
            // Arrange
            var query = new GetAllAutoresQuery();
            
            _autorQueriesMock.Setup(q => q.GetAllAsync())
                .ReturnsAsync(new List<AutorDto>());
                
            // Act
            var result = await _handler.Handle(query, CancellationToken.None);

            // Assert
            Assert.NotNull(result);
            Assert.Empty(result);
            _autorQueriesMock.Verify(q => q.GetAllAsync(), Times.Once);
        }
    }
}
