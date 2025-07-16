using AutoFixture;
using AutoMapper;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Moq;
using SistemaLivros.API.Controllers;
using SistemaLivros.API.Models.Request.Livros;
using SistemaLivros.API.Models.Response.Livros;
using SistemaLivros.Application.Commands.Livros;
using SistemaLivros.Application.DTOs;
using SistemaLivros.Application.Queries.Livros.GetAllLivros;
using SistemaLivros.Application.Queries.Livros.GetLivroById;
using SistemaLivros.Application.Queries.Livros.GetLivroDetalhes;
using SistemaLivros.Application.Queries.Livros.GetLivrosByAutor;
using SistemaLivros.Application.Queries.Livros.GetLivrosByGenero;
using SistemaLivros.Application.Queries.Livros.SearchLivros;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace SistemaLivros.Tests.API.Controllers
{
    public class LivrosControllerTests
    {
        private readonly Mock<IMediator> _mediatorMock;
        private readonly Mock<IMapper> _mapperMock;
        private readonly LivrosController _controller;
        private readonly Fixture _fixture;

        public LivrosControllerTests()
        {
            _mediatorMock = new Mock<IMediator>();
            _mapperMock = new Mock<IMapper>();
            _controller = new LivrosController(_mediatorMock.Object, _mapperMock.Object);
            _fixture = new Fixture();
        }

        [Fact]
        public async Task GetAll_RetornaListaDeLivros()
        {
            // Arrange
            var livrosDto = _fixture.CreateMany<LivroDto>(3).ToList();
            var livrosResponse = _fixture.CreateMany<LivroResponse>(3).ToList();

            _mediatorMock.Setup(m => m.Send(It.IsAny<GetAllLivrosQuery>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(livrosDto);

            _mapperMock.Setup(m => m.Map<IEnumerable<LivroResponse>>(livrosDto))
                .Returns(livrosResponse);

            // Act
            var result = await _controller.GetAll();

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var returnValue = Assert.IsAssignableFrom<IEnumerable<LivroResponse>>(okResult.Value);
            Assert.Equal(livrosResponse, returnValue);
            _mediatorMock.Verify(m => m.Send(It.IsAny<GetAllLivrosQuery>(), It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task GetById_QuandoLivroExiste_RetornaLivro()
        {
            // Arrange
            var id = 1;
            var livroDto = _fixture.Create<LivroDto>();
            var livroResponse = _fixture.Create<LivroResponse>();

            _mediatorMock.Setup(m => m.Send(It.Is<GetLivroByIdQuery>(q => q.Id == id), It.IsAny<CancellationToken>()))
                .ReturnsAsync(livroDto);

            _mapperMock.Setup(m => m.Map<LivroResponse>(livroDto))
                .Returns(livroResponse);

            // Act
            var result = await _controller.GetById(id);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var returnValue = Assert.IsType<LivroResponse>(okResult.Value);
            Assert.Equal(livroResponse, returnValue);
            _mediatorMock.Verify(m => m.Send(It.Is<GetLivroByIdQuery>(q => q.Id == id), It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task GetById_QuandoLivroNaoExiste_RetornaNotFound()
        {
            // Arrange
            var id = 99;

            _mediatorMock.Setup(m => m.Send(It.Is<GetLivroByIdQuery>(q => q.Id == id), It.IsAny<CancellationToken>()))
                .ReturnsAsync((LivroDto)null);

            // Act
            var result = await _controller.GetById(id);

            // Assert
            Assert.IsType<NotFoundResult>(result);
            _mediatorMock.Verify(m => m.Send(It.Is<GetLivroByIdQuery>(q => q.Id == id), It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task GetDetalhes_QuandoLivroExiste_RetornaDetalhes()
        {
            // Arrange
            var id = 1;
            var livroDetalhesDto = _fixture.Create<LivroDetalhesDto>();
            var livroDetalhesResponse = _fixture.Create<LivroDetalhesResponse>();

            _mediatorMock.Setup(m => m.Send(It.Is<GetLivroDetalhesQuery>(q => q.Id == id), It.IsAny<CancellationToken>()))
                .ReturnsAsync(livroDetalhesDto);

            _mapperMock.Setup(m => m.Map<LivroDetalhesResponse>(livroDetalhesDto))
                .Returns(livroDetalhesResponse);

            // Act
            var result = await _controller.GetDetalhes(id);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var returnValue = Assert.IsType<LivroDetalhesResponse>(okResult.Value);
            Assert.Equal(livroDetalhesResponse, returnValue);
            _mediatorMock.Verify(m => m.Send(It.Is<GetLivroDetalhesQuery>(q => q.Id == id), It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task GetDetalhes_QuandoLivroNaoExiste_RetornaNotFound()
        {
            // Arrange
            var id = 99;

            _mediatorMock.Setup(m => m.Send(It.Is<GetLivroDetalhesQuery>(q => q.Id == id), It.IsAny<CancellationToken>()))
                .ReturnsAsync((LivroDetalhesDto)null);

            // Act
            var result = await _controller.GetDetalhes(id);

            // Assert
            Assert.IsType<NotFoundResult>(result);
            _mediatorMock.Verify(m => m.Send(It.Is<GetLivroDetalhesQuery>(q => q.Id == id), It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task GetByGenero_RetornaLivrosPorGenero()
        {
            // Arrange
            var generoId = 1;
            var livrosDto = _fixture.CreateMany<LivroDto>(2).ToList();
            var livrosResponse = _fixture.CreateMany<LivroResponse>(2).ToList();

            _mediatorMock.Setup(m => m.Send(It.Is<GetLivrosByGeneroQuery>(q => q.GeneroId == generoId), It.IsAny<CancellationToken>()))
                .ReturnsAsync(livrosDto);

            _mapperMock.Setup(m => m.Map<IEnumerable<LivroResponse>>(livrosDto))
                .Returns(livrosResponse);

            // Act
            var result = await _controller.GetByGenero(generoId);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var returnValue = Assert.IsAssignableFrom<IEnumerable<LivroResponse>>(okResult.Value);
            Assert.Equal(livrosResponse, returnValue);
            _mediatorMock.Verify(m => m.Send(It.Is<GetLivrosByGeneroQuery>(q => q.GeneroId == generoId), It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task GetByAutor_RetornaLivrosPorAutor()
        {
            // Arrange
            var autorId = 1;
            var livrosDto = _fixture.CreateMany<LivroDto>(2).ToList();
            var livrosResponse = _fixture.CreateMany<LivroResponse>(2).ToList();

            _mediatorMock.Setup(m => m.Send(It.Is<GetLivrosByAutorQuery>(q => q.AutorId == autorId), It.IsAny<CancellationToken>()))
                .ReturnsAsync(livrosDto);

            _mapperMock.Setup(m => m.Map<IEnumerable<LivroResponse>>(livrosDto))
                .Returns(livrosResponse);

            // Act
            var result = await _controller.GetByAutor(autorId);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var returnValue = Assert.IsAssignableFrom<IEnumerable<LivroResponse>>(okResult.Value);
            Assert.Equal(livrosResponse, returnValue);
            _mediatorMock.Verify(m => m.Send(It.Is<GetLivrosByAutorQuery>(q => q.AutorId == autorId), It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task Search_RetornaLivrosPorTermo()
        {
            // Arrange
            var termo = "fantasia";
            var livrosDto = _fixture.CreateMany<LivroDto>(2).ToList();
            var livrosResponse = _fixture.CreateMany<LivroResponse>(2).ToList();

            _mediatorMock.Setup(m => m.Send(It.Is<SearchLivrosQuery>(q => q.Termo == termo), It.IsAny<CancellationToken>()))
                .ReturnsAsync(livrosDto);

            _mapperMock.Setup(m => m.Map<IEnumerable<LivroResponse>>(livrosDto))
                .Returns(livrosResponse);

            // Act
            var result = await _controller.Search(termo);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var returnValue = Assert.IsAssignableFrom<IEnumerable<LivroResponse>>(okResult.Value);
            Assert.Equal(livrosResponse, returnValue);
            _mediatorMock.Verify(m => m.Send(It.Is<SearchLivrosQuery>(q => q.Termo == termo), It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task Create_QuandoSucesso_RetornaCreated()
        {
            // Arrange
            var request = _fixture.Create<LivroRequest>();
            var command = _fixture.Create<CreateLivroCommand>();
            var livroId = 1;
            var livroDto = _fixture.Create<LivroDto>();
            var livroResponse = _fixture.Create<LivroResponse>();

            _mapperMock.Setup(m => m.Map<CreateLivroCommand>(request))
                .Returns(command);

            _mediatorMock.Setup(m => m.Send(command, It.IsAny<CancellationToken>()))
                .ReturnsAsync(livroId);

            _mediatorMock.Setup(m => m.Send(It.Is<GetLivroByIdQuery>(q => q.Id == livroId), It.IsAny<CancellationToken>()))
                .ReturnsAsync(livroDto);

            _mapperMock.Setup(m => m.Map<LivroResponse>(livroDto))
                .Returns(livroResponse);

            // Act
            var result = await _controller.Create(request);

            // Assert
            var createdAtResult = Assert.IsType<CreatedAtActionResult>(result);
            Assert.Equal(nameof(LivrosController.GetById), createdAtResult.ActionName);
            Assert.Equal(livroId, createdAtResult.RouteValues["id"]);
            Assert.Equal(livroResponse, createdAtResult.Value);
            _mediatorMock.Verify(m => m.Send(command, It.IsAny<CancellationToken>()), Times.Once);
            _mediatorMock.Verify(m => m.Send(It.Is<GetLivroByIdQuery>(q => q.Id == livroId), It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task Create_QuandoErro_RetornaBadRequest()
        {
            // Arrange
            var request = _fixture.Create<LivroRequest>();
            var command = _fixture.Create<CreateLivroCommand>();
            var errorMessage = "Autor não encontrado";

            _mapperMock.Setup(m => m.Map<CreateLivroCommand>(request))
                .Returns(command);

            _mediatorMock.Setup(m => m.Send(command, It.IsAny<CancellationToken>()))
                .ThrowsAsync(new Exception(errorMessage));

            // Act
            var result = await _controller.Create(request);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal(errorMessage, badRequestResult.Value);
            _mediatorMock.Verify(m => m.Send(command, It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task Update_QuandoSucesso_RetornaNoContent()
        {
            // Arrange
            var id = 1;
            var request = _fixture.Create<LivroRequest>();
            var command = _fixture.Create<UpdateLivroCommand>();

            _mapperMock.Setup(m => m.Map<UpdateLivroCommand>(request))
                .Returns(command);

            _mediatorMock.Setup(m => m.Send(It.Is<UpdateLivroCommand>(c => c.Id == id), It.IsAny<CancellationToken>()))
                .ReturnsAsync(true);

            // Act
            var result = await _controller.Update(id, request);

            // Assert
            Assert.IsType<NoContentResult>(result);
            _mediatorMock.Verify(m => m.Send(It.Is<UpdateLivroCommand>(c => c.Id == id), It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task Update_QuandoLivroNaoExiste_RetornaNotFound()
        {
            // Arrange
            var id = 99;
            var request = _fixture.Create<LivroRequest>();
            var command = _fixture.Create<UpdateLivroCommand>();

            _mapperMock.Setup(m => m.Map<UpdateLivroCommand>(request))
                .Returns(command);

            _mediatorMock.Setup(m => m.Send(It.Is<UpdateLivroCommand>(c => c.Id == id), It.IsAny<CancellationToken>()))
                .ReturnsAsync(false);

            // Act
            var result = await _controller.Update(id, request);

            // Assert
            Assert.IsType<NotFoundResult>(result);
            _mediatorMock.Verify(m => m.Send(It.Is<UpdateLivroCommand>(c => c.Id == id), It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task Update_QuandoErro_RetornaBadRequest()
        {
            // Arrange
            var id = 1;
            var request = _fixture.Create<LivroRequest>();
            var command = _fixture.Create<UpdateLivroCommand>();
            var errorMessage = "Gênero não encontrado";

            _mapperMock.Setup(m => m.Map<UpdateLivroCommand>(request))
                .Returns(command);

            _mediatorMock.Setup(m => m.Send(It.Is<UpdateLivroCommand>(c => c.Id == id), It.IsAny<CancellationToken>()))
                .ThrowsAsync(new Exception(errorMessage));

            // Act
            var result = await _controller.Update(id, request);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal(errorMessage, badRequestResult.Value);
            _mediatorMock.Verify(m => m.Send(It.Is<UpdateLivroCommand>(c => c.Id == id), It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task Delete_QuandoSucesso_RetornaNoContent()
        {
            // Arrange
            var id = 1;

            _mediatorMock.Setup(m => m.Send(It.Is<DeleteLivroCommand>(c => c.Id == id), It.IsAny<CancellationToken>()))
                .ReturnsAsync(true);

            // Act
            var result = await _controller.Delete(id);

            // Assert
            Assert.IsType<NoContentResult>(result);
            _mediatorMock.Verify(m => m.Send(It.Is<DeleteLivroCommand>(c => c.Id == id), It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task Delete_QuandoLivroNaoExiste_RetornaNotFound()
        {
            // Arrange
            var id = 99;

            _mediatorMock.Setup(m => m.Send(It.Is<DeleteLivroCommand>(c => c.Id == id), It.IsAny<CancellationToken>()))
                .ReturnsAsync(false);

            // Act
            var result = await _controller.Delete(id);

            // Assert
            Assert.IsType<NotFoundResult>(result);
            _mediatorMock.Verify(m => m.Send(It.Is<DeleteLivroCommand>(c => c.Id == id), It.IsAny<CancellationToken>()), Times.Once);
        }
    }
}
