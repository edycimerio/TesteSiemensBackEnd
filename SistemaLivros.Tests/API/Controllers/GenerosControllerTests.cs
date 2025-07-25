using AutoMapper;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Moq;
using SistemaLivros.API.Controllers;
using SistemaLivros.API.Models.Request.Generos;
using SistemaLivros.API.Models.Response.Generos;
using SistemaLivros.API.Models.Response.Livros;
using SistemaLivros.Application.Commands.Generos;
using SistemaLivros.Application.Common;
using SistemaLivros.Application.DTOs;
using SistemaLivros.Application.Queries.Generos.GetAllGeneros;
using SistemaLivros.Application.Queries.Generos.GetGeneroById;
using SistemaLivros.Application.Queries.Generos.GetGeneroDetalhes;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace SistemaLivros.Tests.API.Controllers
{
    public class GenerosControllerTests
    {
        private readonly Mock<IMediator> _mediatorMock;
        private readonly Mock<IMapper> _mapperMock;
        private readonly GenerosController _controller;

        public GenerosControllerTests()
        {
            _mediatorMock = new Mock<IMediator>();
            _mapperMock = new Mock<IMapper>();
            _controller = new GenerosController(_mediatorMock.Object, _mapperMock.Object);
        }

        [Fact]
        public async Task TestaGetAll()
        {
            // Arrange
            int pageNumber = 1;
            int pageSize = 10;
            
            var generos = new List<GeneroDto>
            {
                new GeneroDto { Id = 1, Nome = "Ficção Científica", Descricao = "Livros de ficção científica" },
                new GeneroDto { Id = 2, Nome = "Romance", Descricao = "Livros de romance" }
            };

            var generosResponse = new List<GeneroResponse>
            {
                new GeneroResponse { Id = 1, Nome = "Ficção Científica", Descricao = "Livros de ficção científica" },
                new GeneroResponse { Id = 2, Nome = "Romance", Descricao = "Livros de romance" }
            };
            
            var pagedGeneros = new PagedResult<GeneroDto>
            {
                Items = generos,
                PageNumber = pageNumber,
                PageSize = pageSize,
                TotalCount = generos.Count,
                TotalPages = 1
            };
            
            var pagedGenerosResponse = new PagedResult<GeneroResponse>
            {
                Items = generosResponse,
                PageNumber = pageNumber,
                PageSize = pageSize,
                TotalCount = generosResponse.Count,
                TotalPages = 1
            };

            _mediatorMock.Setup(m => m.Send(It.Is<GetAllGenerosQuery>(q => q.PageNumber == pageNumber && q.PageSize == pageSize), It.IsAny<CancellationToken>()))
                .ReturnsAsync(pagedGeneros);

            _mapperMock.Setup(m => m.Map<PagedResult<GeneroResponse>>(pagedGeneros))
                .Returns(pagedGenerosResponse);

            // Act
            var result = await _controller.GetAll(pageNumber, pageSize);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var returnValue = Assert.IsType<PagedResult<GeneroResponse>>(okResult.Value);
            Assert.Equal(2, returnValue.Items.Count);
            Assert.Equal(pageNumber, returnValue.PageNumber);
            Assert.Equal(pageSize, returnValue.PageSize);
            Assert.Equal(2, returnValue.TotalCount);
            Assert.Equal(1, returnValue.TotalPages);
        }

        [Fact]
        public async Task TestaGetByIdExistente()
        {
            // Arrange
            var generoDto = new GeneroDto { Id = 1, Nome = "Ficção Científica", Descricao = "Livros de ficção científica" };
            var generoResponse = new GeneroResponse { Id = 1, Nome = "Ficção Científica", Descricao = "Livros de ficção científica" };

            _mediatorMock.Setup(m => m.Send(It.IsAny<GetGeneroByIdQuery>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(generoDto);

            _mapperMock.Setup(m => m.Map<GeneroResponse>(generoDto))
                .Returns(generoResponse);

            // Act
            var result = await _controller.GetById(1);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var returnValue = Assert.IsType<GeneroResponse>(okResult.Value);
            Assert.Equal(1, returnValue.Id);
            Assert.Equal("Ficção Científica", returnValue.Nome);
        }

        [Fact]
        public async Task TestaGetByIdNaoExistente()
        {
            // Arrange
            _mediatorMock.Setup(m => m.Send(It.Is<GetGeneroByIdQuery>(q => q.Id == 99), It.IsAny<CancellationToken>()))
                .ReturnsAsync((GeneroDto)null);

            // Act
            var result = await _controller.GetById(99);

            // Assert
            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public async Task TestaCreate()
        {
            // Arrange
            var request = new GeneroRequest { Nome = "Ficção Científica", Descricao = "Livros de ficção científica" };
            var command = new CreateGeneroCommand("Ficção Científica", "Livros de ficção científica");
            var generoDto = new GeneroDto { Id = 1, Nome = "Ficção Científica", Descricao = "Livros de ficção científica" };
            var generoResponse = new GeneroResponse { Id = 1, Nome = "Ficção Científica", Descricao = "Livros de ficção científica" };

            _mapperMock.Setup(m => m.Map<CreateGeneroCommand>(request))
                .Returns(command);

            _mediatorMock.Setup(m => m.Send(command, It.IsAny<CancellationToken>()))
                .ReturnsAsync(1);

            _mediatorMock.Setup(m => m.Send(It.IsAny<GetGeneroByIdQuery>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(generoDto);

            _mapperMock.Setup(m => m.Map<GeneroResponse>(generoDto))
                .Returns(generoResponse);

            // Act
            var result = await _controller.Create(request);

            // Assert
            var createdAtResult = Assert.IsType<CreatedAtActionResult>(result);
            Assert.Equal(nameof(GenerosController.GetById), createdAtResult.ActionName);
            Assert.Equal(1, createdAtResult.RouteValues["id"]);
            var returnValue = Assert.IsType<GeneroResponse>(createdAtResult.Value);
            Assert.Equal(1, returnValue.Id);
            Assert.Equal("Ficção Científica", returnValue.Nome);
        }
        
        [Fact]
        public async Task TestaGetDetalhesExistente()
        {
            // Arrange
            var generoDetalhesDto = new GeneroDetalhesDto 
            { 
                Id = 1, 
                Nome = "Ficção Científica", 
                Descricao = "Livros de ficção científica",
                Livros = new List<LivroDto>
                {
                    new LivroDto { Id = 1, Titulo = "Duna", Ano = 1965 },
                    new LivroDto { Id = 2, Titulo = "Neuromancer", Ano = 1984 }
                }
            };
            
            var generoDetalhesResponse = new GeneroDetalhesResponse 
            { 
                Id = 1, 
                Nome = "Ficção Científica", 
                Descricao = "Livros de ficção científica",
                Livros = new List<LivroSimplificadoResponse>
                {
                    new LivroSimplificadoResponse { Id = 1, Titulo = "Duna", Ano = 1965 },
                    new LivroSimplificadoResponse { Id = 2, Titulo = "Neuromancer", Ano = 1984 }
                }
            };

            _mediatorMock.Setup(m => m.Send(It.Is<GetGeneroDetalhesQuery>(q => q.Id == 1), It.IsAny<CancellationToken>()))
                .ReturnsAsync(generoDetalhesDto);

            _mapperMock.Setup(m => m.Map<GeneroDetalhesResponse>(generoDetalhesDto))
                .Returns(generoDetalhesResponse);

            // Act
            var result = await _controller.GetDetalhes(1);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var returnValue = Assert.IsType<GeneroDetalhesResponse>(okResult.Value);
            Assert.Equal(1, returnValue.Id);
            Assert.Equal("Ficção Científica", returnValue.Nome);
            Assert.Equal(2, returnValue.Livros.Count());
        }

        [Fact]
        public async Task TestaGetDetalhesNaoExistente()
        {
            // Arrange
            _mediatorMock.Setup(m => m.Send(It.Is<GetGeneroDetalhesQuery>(q => q.Id == 99), It.IsAny<CancellationToken>()))
                .ReturnsAsync((GeneroDetalhesDto)null);

            // Act
            var result = await _controller.GetDetalhes(99);

            // Assert
            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public async Task TestaUpdateExistente()
        {
            // Arrange
            var id = 1;
            var request = new GeneroRequest { Nome = "Ficção Científica Atualizado", Descricao = "Descrição atualizada" };
            var command = new UpdateGeneroCommand(id, "Ficção Científica Atualizado", "Descrição atualizada");

            _mapperMock.Setup(m => m.Map<UpdateGeneroCommand>(request))
                .Returns(command);

            _mediatorMock.Setup(m => m.Send(It.Is<UpdateGeneroCommand>(c => c.Id == id), It.IsAny<CancellationToken>()))
                .ReturnsAsync(true);

            // Act
            var result = await _controller.Update(id, request);

            // Assert
            Assert.IsType<NoContentResult>(result);
            _mediatorMock.Verify(m => m.Send(It.Is<UpdateGeneroCommand>(c => c.Id == id), It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task TestaUpdateNaoExistente()
        {
            // Arrange
            var id = 99;
            var request = new GeneroRequest { Nome = "Ficção Científica", Descricao = "Livros de ficção científica" };
            var command = new UpdateGeneroCommand(id, "Ficção Científica", "Livros de ficção científica");

            _mapperMock.Setup(m => m.Map<UpdateGeneroCommand>(request))
                .Returns(command);

            _mediatorMock.Setup(m => m.Send(It.Is<UpdateGeneroCommand>(c => c.Id == id), It.IsAny<CancellationToken>()))
                .ReturnsAsync(false);

            // Act
            var result = await _controller.Update(id, request);

            // Assert
            Assert.IsType<NotFoundResult>(result);
            _mediatorMock.Verify(m => m.Send(It.Is<UpdateGeneroCommand>(c => c.Id == id), It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task TestaDeleteExistente()
        {
            // Arrange
            var id = 1;

            _mediatorMock.Setup(m => m.Send(It.Is<DeleteGeneroCommand>(c => c.Id == id), It.IsAny<CancellationToken>()))
                .ReturnsAsync(true);

            // Act
            var result = await _controller.Delete(id);

            // Assert
            Assert.IsType<NoContentResult>(result);
            _mediatorMock.Verify(m => m.Send(It.Is<DeleteGeneroCommand>(c => c.Id == id), It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task TestaDeleteNaoExistente()
        {
            // Arrange
            var id = 99;

            _mediatorMock.Setup(m => m.Send(It.Is<DeleteGeneroCommand>(c => c.Id == id), It.IsAny<CancellationToken>()))
                .ReturnsAsync(false);

            // Act
            var result = await _controller.Delete(id);

            // Assert
            Assert.IsType<NotFoundResult>(result);
            _mediatorMock.Verify(m => m.Send(It.Is<DeleteGeneroCommand>(c => c.Id == id), It.IsAny<CancellationToken>()), Times.Once);
        }
    }
}
