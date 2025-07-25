using AutoMapper;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Moq;
using SistemaLivros.API.Controllers;
using SistemaLivros.API.Models.Request.Autores;
using SistemaLivros.API.Models.Response.Autores;
using SistemaLivros.API.Models.Response.Livros;
using SistemaLivros.Application.Commands.Autores;
using SistemaLivros.Application.Common;
using SistemaLivros.Application.DTOs;
using SistemaLivros.Application.Queries.Autores.GetAllAutores;
using SistemaLivros.Application.Queries.Autores.GetAutorById;
using SistemaLivros.Application.Queries.Autores.GetAutorDetalhes;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Xunit;
using FluentAssertions;

namespace SistemaLivros.Tests.API.Controllers
{
    public class AutoresControllerTests
    {
        private readonly Mock<IMediator> _mediatorMock;
        private readonly Mock<IMapper> _mapperMock;
        private readonly AutoresController _controller;

        public AutoresControllerTests()
        {
            _mediatorMock = new Mock<IMediator>();
            _mapperMock = new Mock<IMapper>();
            _controller = new AutoresController(_mediatorMock.Object, _mapperMock.Object);
        }

        [Fact]
        public async Task GetAllDeveRetornarListaDeAutoresPaginada()
        {
            // Arrange
            int pageNumber = 1;
            int pageSize = 10;
            
            var autores = new List<AutorDto>
            {
                new AutorDto { Id = 1, Nome = "J.R.R. Tolkien", Biografia = "Autor de O Senhor dos Anéis", DataNascimento = new DateTime(1892, 1, 3) },
                new AutorDto { Id = 2, Nome = "George R.R. Martin", Biografia = "Autor de Game of Thrones", DataNascimento = new DateTime(1948, 9, 20) }
            };

            var autoresResponse = new List<AutorResponse>
            {
                new AutorResponse { Id = 1, Nome = "J.R.R. Tolkien", Biografia = "Autor de O Senhor dos Anéis", DataNascimento = new DateTime(1892, 1, 3) },
                new AutorResponse { Id = 2, Nome = "George R.R. Martin", Biografia = "Autor de Game of Thrones", DataNascimento = new DateTime(1948, 9, 20) }
            };
            
            var pagedAutores = new PagedResult<AutorDto>
            {
                Items = autores,
                PageNumber = pageNumber,
                PageSize = pageSize,
                TotalCount = autores.Count,
                TotalPages = 1
            };
            
            var pagedAutoresResponse = new PagedResult<AutorResponse>
            {
                Items = autoresResponse,
                PageNumber = pageNumber,
                PageSize = pageSize,
                TotalCount = autoresResponse.Count,
                TotalPages = 1
            };

            _mediatorMock.Setup(m => m.Send(It.Is<GetAllAutoresQuery>(q => q.PageNumber == pageNumber && q.PageSize == pageSize), It.IsAny<CancellationToken>()))
                .ReturnsAsync(pagedAutores);

            _mapperMock.Setup(m => m.Map<PagedResult<AutorResponse>>(pagedAutores))
                .Returns(pagedAutoresResponse);

            // Act
            var result = await _controller.GetAll(pageNumber, pageSize);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var returnValue = Assert.IsType<PagedResult<AutorResponse>>(okResult.Value);
            Assert.Equal(2, returnValue.Items.Count);
            Assert.Equal(pageNumber, returnValue.PageNumber);
            Assert.Equal(pageSize, returnValue.PageSize);
            Assert.Equal(2, returnValue.TotalCount);
            Assert.Equal(1, returnValue.TotalPages);
        }

        [Fact]
        public async Task GetByIdQuandoAutorExisteDeveRetornarAutor()
        {
            // Arrange
            var autorDto = new AutorDto { Id = 1, Nome = "J.R.R. Tolkien", Biografia = "Autor de O Senhor dos Anéis" };
            var autorResponse = new AutorResponse { Id = 1, Nome = "J.R.R. Tolkien", Biografia = "Autor de O Senhor dos Anéis" };

            _mediatorMock.Setup(m => m.Send(It.IsAny<GetAutorByIdQuery>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(autorDto);

            _mapperMock.Setup(m => m.Map<AutorResponse>(autorDto))
                .Returns(autorResponse);

            // Act
            var result = await _controller.GetById(1);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var returnValue = Assert.IsType<AutorResponse>(okResult.Value);
            Assert.Equal(1, returnValue.Id);
            Assert.Equal("J.R.R. Tolkien", returnValue.Nome);
        }

        [Fact]
        public async Task GetByIdQuandoAutorNaoExisteDeveRetornarNotFound()
        {
            // Arrange
            _mediatorMock.Setup(m => m.Send(It.Is<GetAutorByIdQuery>(q => q.Id == 99), It.IsAny<CancellationToken>()))
                .ReturnsAsync((AutorDto)null);

            // Act
            var result = await _controller.GetById(99);

            // Assert
            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public async Task GetDetalhesQuandoAutorExisteDeveRetornarDetalhesDoAutor()
        {
            // Arrange
            var autorDetalhesDto = new AutorDetalhesDto 
            { 
                Id = 1, 
                Nome = "J.R.R. Tolkien", 
                Biografia = "Autor de O Senhor dos Anéis",
                Livros = new List<LivroDto>
                {
                    new LivroDto { Id = 1, Titulo = "O Senhor dos Anéis", Ano = 1954 },
                    new LivroDto { Id = 2, Titulo = "O Hobbit", Ano = 1937 }
                }
            };
            
            var autorDetalhesResponse = new AutorDetalhesResponse 
            { 
                Id = 1, 
                Nome = "J.R.R. Tolkien", 
                Biografia = "Autor de O Senhor dos Anéis",
                Livros = new List<LivroSimplificadoResponse>
                {
                    new LivroSimplificadoResponse { Id = 1, Titulo = "O Senhor dos Anéis", Ano = 1954 },
                    new LivroSimplificadoResponse { Id = 2, Titulo = "O Hobbit", Ano = 1937 }
                }
            };

            _mediatorMock.Setup(m => m.Send(It.Is<GetAutorDetalhesQuery>(q => q.Id == 1), It.IsAny<CancellationToken>()))
                .ReturnsAsync(autorDetalhesDto);

            _mapperMock.Setup(m => m.Map<AutorDetalhesResponse>(autorDetalhesDto))
                .Returns(autorDetalhesResponse);

            // Act
            var result = await _controller.GetDetalhes(1);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var returnValue = Assert.IsType<AutorDetalhesResponse>(okResult.Value);
            Assert.Equal(1, returnValue.Id);
            Assert.Equal("J.R.R. Tolkien", returnValue.Nome);
            Assert.Equal(2, returnValue.Livros.Count());
        }

        [Fact]
        public async Task GetDetalhesQuandoAutorNaoExisteDeveRetornarNotFound()
        {
            // Arrange
            _mediatorMock.Setup(m => m.Send(It.Is<GetAutorDetalhesQuery>(q => q.Id == 99), It.IsAny<CancellationToken>()))
                .ReturnsAsync((AutorDetalhesDto)null);

            // Act
            var result = await _controller.GetDetalhes(99);

            // Assert
            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public async Task CreateComDadosValidosDeveCriarAutorERetornarCreated()
        {
            // Arrange
            var request = new AutorRequest { Nome = "J.R.R. Tolkien", Biografia = "Autor de O Senhor dos Anéis" };
            var command = new CreateAutorCommand("J.R.R. Tolkien", "Autor de O Senhor dos Anéis", new DateTime(1892, 1, 3));
            var autorDto = new AutorDto { Id = 1, Nome = "J.R.R. Tolkien", Biografia = "Autor de O Senhor dos Anéis" };
            var autorResponse = new AutorResponse { Id = 1, Nome = "J.R.R. Tolkien", Biografia = "Autor de O Senhor dos Anéis" };

            _mapperMock.Setup(m => m.Map<CreateAutorCommand>(request))
                .Returns(command);

            _mediatorMock.Setup(m => m.Send(command, It.IsAny<CancellationToken>()))
                .ReturnsAsync(1);

            _mediatorMock.Setup(m => m.Send(It.IsAny<GetAutorByIdQuery>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(autorDto);

            _mapperMock.Setup(m => m.Map<AutorResponse>(autorDto))
                .Returns(autorResponse);

            // Act
            var result = await _controller.Create(request);

            // Assert
            var createdAtResult = Assert.IsType<CreatedAtActionResult>(result);
            Assert.Equal(nameof(AutoresController.GetById), createdAtResult.ActionName);
            Assert.Equal(1, createdAtResult.RouteValues["id"]);
            var returnValue = Assert.IsType<AutorResponse>(createdAtResult.Value);
            Assert.Equal(1, returnValue.Id);
            Assert.Equal("J.R.R. Tolkien", returnValue.Nome);
        }

        [Fact]
        public async Task UpdateQuandoAutorExisteDeveAtualizarERetornarNoContent()
        {
            // Arrange
            var id = 1;
            var request = new AutorRequest { Nome = "J.R.R. Tolkien Atualizado", Biografia = "Biografia atualizada", DataNascimento = new DateTime(1892, 1, 3) };
            var command = new UpdateAutorCommand(id, "J.R.R. Tolkien Atualizado", "Biografia atualizada", new DateTime(1892, 1, 3));

            _mapperMock.Setup(m => m.Map<UpdateAutorCommand>(request))
                .Returns(command);

            _mediatorMock.Setup(m => m.Send(It.Is<UpdateAutorCommand>(c => c.Id == id), It.IsAny<CancellationToken>()))
                .ReturnsAsync(true);

            // Act
            var result = await _controller.Update(id, request);

            // Assert
            Assert.IsType<NoContentResult>(result);
            _mediatorMock.Verify(m => m.Send(It.Is<UpdateAutorCommand>(c => c.Id == id), It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task UpdateQuandoAutorNaoExisteDeveRetornarNotFound()
        {
            // Arrange
            var id = 99;
            var request = new AutorRequest { Nome = "J.R.R. Tolkien", Biografia = "Autor de O Senhor dos Anéis" };
            var command = new UpdateAutorCommand(id, "J.R.R. Tolkien", "Autor de O Senhor dos Anéis", new DateTime(1892, 1, 3));

            _mapperMock.Setup(m => m.Map<UpdateAutorCommand>(request))
                .Returns(command);

            _mediatorMock.Setup(m => m.Send(It.Is<UpdateAutorCommand>(c => c.Id == id), It.IsAny<CancellationToken>()))
                .ReturnsAsync(false);

            // Act
            var result = await _controller.Update(id, request);

            // Assert
            Assert.IsType<NotFoundResult>(result);
            _mediatorMock.Verify(m => m.Send(It.Is<UpdateAutorCommand>(c => c.Id == id), It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task DeleteQuandoAutorExisteDeveRemoverERetornarNoContent()
        {
            // Arrange
            var id = 1;

            _mediatorMock.Setup(m => m.Send(It.Is<DeleteAutorCommand>(c => c.Id == id), It.IsAny<CancellationToken>()))
                .ReturnsAsync(true);

            // Act
            var result = await _controller.Delete(id);

            // Assert
            Assert.IsType<NoContentResult>(result);
            _mediatorMock.Verify(m => m.Send(It.Is<DeleteAutorCommand>(c => c.Id == id), It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task DeleteQuandoAutorNaoExisteDeveRetornarNotFound()
        {
            // Arrange
            var id = 99;

            _mediatorMock.Setup(m => m.Send(It.Is<DeleteAutorCommand>(c => c.Id == id), It.IsAny<CancellationToken>()))
                .ReturnsAsync(false);

            // Act
            var result = await _controller.Delete(id);

            // Assert
            Assert.IsType<NotFoundResult>(result);
            _mediatorMock.Verify(m => m.Send(It.Is<DeleteAutorCommand>(c => c.Id == id), It.IsAny<CancellationToken>()), Times.Once);
        }
    }
}
