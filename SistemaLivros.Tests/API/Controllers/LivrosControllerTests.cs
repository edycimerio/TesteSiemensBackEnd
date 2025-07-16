using AutoFixture;
using AutoMapper;
using FluentValidation;
using FluentValidation.Results;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Moq;
using SistemaLivros.API.Controllers;
using SistemaLivros.API.Models.Request.Livros;
using SistemaLivros.API.Models.Response.Livros;
using SistemaLivros.Application.Commands.Livros;
using SistemaLivros.Application.Common;
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
        private readonly Mock<IValidator<LivroRequest>> _validatorMock;
        private readonly LivrosController _controller;
        private readonly Fixture _fixture;

        public LivrosControllerTests()
        {
            _mediatorMock = new Mock<IMediator>();
            _mapperMock = new Mock<IMapper>();
            _validatorMock = new Mock<IValidator<LivroRequest>>();
            _controller = new LivrosController(_mediatorMock.Object, _mapperMock.Object);
            _fixture = new Fixture();
            
            // Configuração padrão para o validador retornar sucesso
            _validatorMock.Setup(v => v.ValidateAsync(It.IsAny<LivroRequest>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new ValidationResult());
        }

        [Fact]
        public async Task GetAll_RetornaListaDeLivrosPaginada()
        {
            // Arrange
            int pageNumber = 1;
            int pageSize = 10;
            var livrosDto = _fixture.CreateMany<LivroDto>(3).ToList();
            var livrosResponse = _fixture.CreateMany<LivroResponse>(3).ToList();
            
            var pagedLivrosDto = new PagedResult<LivroDto>
            {
                Items = livrosDto,
                PageNumber = pageNumber,
                PageSize = pageSize,
                TotalCount = livrosDto.Count,
                TotalPages = 1
            };
            
            var pagedLivrosResponse = new PagedResult<LivroResponse>
            {
                Items = livrosResponse,
                PageNumber = pageNumber,
                PageSize = pageSize,
                TotalCount = livrosResponse.Count,
                TotalPages = 1
            };

            _mediatorMock.Setup(m => m.Send(It.Is<GetAllLivrosQuery>(q => q.PageNumber == pageNumber && q.PageSize == pageSize), It.IsAny<CancellationToken>()))
                .ReturnsAsync(pagedLivrosDto);

            _mapperMock.Setup(m => m.Map<PagedResult<LivroResponse>>(pagedLivrosDto))
                .Returns(pagedLivrosResponse);

            // Act
            var result = await _controller.GetAll(pageNumber, pageSize);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var returnValue = Assert.IsType<PagedResult<LivroResponse>>(okResult.Value);
            Assert.Equal(pagedLivrosResponse.Items.Count, returnValue.Items.Count);
            Assert.Equal(pageNumber, returnValue.PageNumber);
            Assert.Equal(pageSize, returnValue.PageSize);
            Assert.Equal(livrosDto.Count, returnValue.TotalCount);
            Assert.Equal(1, returnValue.TotalPages);
            _mediatorMock.Verify(m => m.Send(It.Is<GetAllLivrosQuery>(q => q.PageNumber == pageNumber && q.PageSize == pageSize), It.IsAny<CancellationToken>()), Times.Once);
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
        public async Task GetByGenero_RetornaLivrosPorGeneroPaginados()
        {
            // Arrange
            var generoId = 1;
            int pageNumber = 1;
            int pageSize = 10;
            var livrosDto = _fixture.CreateMany<LivroDto>(2).ToList();
            var livrosResponse = _fixture.CreateMany<LivroResponse>(2).ToList();
            
            var pagedLivrosDto = new PagedResult<LivroDto>
            {
                Items = livrosDto,
                PageNumber = pageNumber,
                PageSize = pageSize,
                TotalCount = livrosDto.Count,
                TotalPages = 1
            };
            
            var pagedLivrosResponse = new PagedResult<LivroResponse>
            {
                Items = livrosResponse,
                PageNumber = pageNumber,
                PageSize = pageSize,
                TotalCount = livrosResponse.Count,
                TotalPages = 1
            };

            _mediatorMock.Setup(m => m.Send(It.Is<GetLivrosByGeneroQuery>(q => q.GeneroId == generoId && q.PageNumber == pageNumber && q.PageSize == pageSize), It.IsAny<CancellationToken>()))
                .ReturnsAsync(pagedLivrosDto);

            _mapperMock.Setup(m => m.Map<PagedResult<LivroResponse>>(pagedLivrosDto))
                .Returns(pagedLivrosResponse);

            // Act
            var result = await _controller.GetByGenero(generoId, pageNumber, pageSize);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var returnValue = Assert.IsType<PagedResult<LivroResponse>>(okResult.Value);
            Assert.Equal(pagedLivrosResponse.Items.Count, returnValue.Items.Count);
            Assert.Equal(pageNumber, returnValue.PageNumber);
            Assert.Equal(pageSize, returnValue.PageSize);
            Assert.Equal(livrosDto.Count, returnValue.TotalCount);
            Assert.Equal(1, returnValue.TotalPages);
            _mediatorMock.Verify(m => m.Send(It.Is<GetLivrosByGeneroQuery>(q => q.GeneroId == generoId && q.PageNumber == pageNumber && q.PageSize == pageSize), It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task GetByAutor_RetornaLivrosPorAutorPaginados()
        {
            // Arrange
            var autorId = 1;
            int pageNumber = 1;
            int pageSize = 10;
            var livrosDto = _fixture.CreateMany<LivroDto>(2).ToList();
            var livrosResponse = _fixture.CreateMany<LivroResponse>(2).ToList();
            
            var pagedLivrosDto = new PagedResult<LivroDto>
            {
                Items = livrosDto,
                PageNumber = pageNumber,
                PageSize = pageSize,
                TotalCount = livrosDto.Count,
                TotalPages = 1
            };
            
            var pagedLivrosResponse = new PagedResult<LivroResponse>
            {
                Items = livrosResponse,
                PageNumber = pageNumber,
                PageSize = pageSize,
                TotalCount = livrosResponse.Count,
                TotalPages = 1
            };

            _mediatorMock.Setup(m => m.Send(It.Is<GetLivrosByAutorQuery>(q => q.AutorId == autorId && q.PageNumber == pageNumber && q.PageSize == pageSize), It.IsAny<CancellationToken>()))
                .ReturnsAsync(pagedLivrosDto);

            _mapperMock.Setup(m => m.Map<PagedResult<LivroResponse>>(pagedLivrosDto))
                .Returns(pagedLivrosResponse);

            // Act
            var result = await _controller.GetByAutor(autorId, pageNumber, pageSize);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var returnValue = Assert.IsType<PagedResult<LivroResponse>>(okResult.Value);
            Assert.Equal(pagedLivrosResponse.Items.Count, returnValue.Items.Count);
            Assert.Equal(pageNumber, returnValue.PageNumber);
            Assert.Equal(pageSize, returnValue.PageSize);
            Assert.Equal(livrosDto.Count, returnValue.TotalCount);
            Assert.Equal(1, returnValue.TotalPages);
            _mediatorMock.Verify(m => m.Send(It.Is<GetLivrosByAutorQuery>(q => q.AutorId == autorId && q.PageNumber == pageNumber && q.PageSize == pageSize), It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task Search_RetornaLivrosPorTermo()
        {
            // Arrange
            var termo = "fantasia";
            var pageNumber = 1;
            var pageSize = 10;
            var livrosDto = _fixture.CreateMany<LivroDto>(2).ToList();
            var pagedResult = new PagedResult<LivroDto>(livrosDto, pageNumber, pageSize, livrosDto.Count);
            var livrosResponse = _fixture.CreateMany<LivroResponse>(2).ToList();
            var pagedResponse = new PagedResult<LivroResponse>(livrosResponse, pageNumber, pageSize, livrosResponse.Count);

            _mediatorMock.Setup(m => m.Send(It.IsAny<SearchLivrosQuery>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(pagedResult);

            _mapperMock.Setup(m => m.Map<PagedResult<LivroResponse>>(pagedResult))
                .Returns(pagedResponse);

            // Act
            var result = await _controller.Search(termo, pageNumber, pageSize);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var returnValue = Assert.IsAssignableFrom<PagedResult<LivroResponse>>(okResult.Value);
            Assert.Equal(pagedResponse, returnValue);
            _mediatorMock.Verify(m => m.Send(It.Is<SearchLivrosQuery>(q => q.Termo == termo), It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task Create_QuandoSucesso_RetornaCreated()
        {
            // Arrange
            var request = _fixture.Create<LivroRequest>();
            var command = _fixture.Create<CreateLivroCommand>();
            var id = 1;
            var livroDto = _fixture.Create<LivroDto>();
            var livroResponse = _fixture.Create<LivroResponse>();

            _mapperMock.Setup(m => m.Map<CreateLivroCommand>(request))
                .Returns(command);

            _mediatorMock.Setup(m => m.Send(command, It.IsAny<CancellationToken>()))
                .ReturnsAsync(id);

            _mediatorMock.Setup(m => m.Send(It.Is<GetLivroByIdQuery>(q => q.Id == id), It.IsAny<CancellationToken>()))
                .ReturnsAsync(livroDto);

            _mapperMock.Setup(m => m.Map<LivroResponse>(livroDto))
                .Returns(livroResponse);
                
            // Configurar validador para retornar sucesso
            _validatorMock.Setup(v => v.ValidateAsync(request, It.IsAny<CancellationToken>()))
                .ReturnsAsync(new ValidationResult());

            // Act
            var result = await _controller.Create(request, _validatorMock.Object);

            // Assert
            var createdAtActionResult = Assert.IsType<CreatedAtActionResult>(result);
            Assert.Equal(nameof(LivrosController.GetById), createdAtActionResult.ActionName);
            Assert.Equal(id, createdAtActionResult.RouteValues["id"]);
            Assert.Equal(livroResponse, createdAtActionResult.Value);
            _mediatorMock.Verify(m => m.Send(command, It.IsAny<CancellationToken>()), Times.Once);
            _validatorMock.Verify(v => v.ValidateAsync(request, It.IsAny<CancellationToken>()), Times.Once);
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
                
            // Configurar validador para retornar sucesso
            _validatorMock.Setup(v => v.ValidateAsync(request, It.IsAny<CancellationToken>()))
                .ReturnsAsync(new ValidationResult());

            // Act
            var result = await _controller.Create(request, _validatorMock.Object);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal(errorMessage, badRequestResult.Value);
            _mediatorMock.Verify(m => m.Send(command, It.IsAny<CancellationToken>()), Times.Once);
            _validatorMock.Verify(v => v.ValidateAsync(request, It.IsAny<CancellationToken>()), Times.Once);
        }
        
        [Fact]
        public async Task Create_QuandoValidacaoFalha_RetornaBadRequest()
        {
            // Arrange
            var request = _fixture.Create<LivroRequest>();
            var validationFailures = new List<ValidationFailure>
            {
                new ValidationFailure("Titulo", "O título é obrigatório"),
                new ValidationFailure("AutorId", "O autor é obrigatório")
            };
            
            // Configurar validador para retornar falha
            _validatorMock.Setup(v => v.ValidateAsync(request, It.IsAny<CancellationToken>()))
                .ReturnsAsync(new ValidationResult(validationFailures));

            // Act
            var result = await _controller.Create(request, _validatorMock.Object);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            var errors = Assert.IsAssignableFrom<IEnumerable<object>>(badRequestResult.Value);
            Assert.Equal(2, errors.Count());
            _validatorMock.Verify(v => v.ValidateAsync(request, It.IsAny<CancellationToken>()), Times.Once);
            // Verificar que o mediator não foi chamado, pois a validação falhou
            _mediatorMock.Verify(m => m.Send(It.IsAny<CreateLivroCommand>(), It.IsAny<CancellationToken>()), Times.Never);
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
                
            // Configurar validador para retornar sucesso
            _validatorMock.Setup(v => v.ValidateAsync(request, It.IsAny<CancellationToken>()))
                .ReturnsAsync(new ValidationResult());

            // Act
            var result = await _controller.Update(id, request, _validatorMock.Object);

            // Assert
            Assert.IsType<NoContentResult>(result);
            _mediatorMock.Verify(m => m.Send(It.Is<UpdateLivroCommand>(c => c.Id == id), It.IsAny<CancellationToken>()), Times.Once);
            _validatorMock.Verify(v => v.ValidateAsync(request, It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task Update_QuandoLivroNaoEncontrado_RetornaNotFound()
        {
            // Arrange
            var id = 1;
            var request = _fixture.Create<LivroRequest>();
            var command = _fixture.Create<UpdateLivroCommand>();

            _mapperMock.Setup(m => m.Map<UpdateLivroCommand>(request))
                .Returns(command);

            _mediatorMock.Setup(m => m.Send(It.Is<UpdateLivroCommand>(c => c.Id == id), It.IsAny<CancellationToken>()))
                .ReturnsAsync(false);
                
            // Configurar validador para retornar sucesso
            _validatorMock.Setup(v => v.ValidateAsync(request, It.IsAny<CancellationToken>()))
                .ReturnsAsync(new ValidationResult());

            // Act
            var result = await _controller.Update(id, request, _validatorMock.Object);

            // Assert
            Assert.IsType<NotFoundResult>(result);
            _mediatorMock.Verify(m => m.Send(It.Is<UpdateLivroCommand>(c => c.Id == id), It.IsAny<CancellationToken>()), Times.Once);
            _validatorMock.Verify(v => v.ValidateAsync(request, It.IsAny<CancellationToken>()), Times.Once);
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
                
            // Configurar validador para retornar sucesso
            _validatorMock.Setup(v => v.ValidateAsync(request, It.IsAny<CancellationToken>()))
                .ReturnsAsync(new ValidationResult());

            // Act
            var result = await _controller.Update(id, request, _validatorMock.Object);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal(errorMessage, badRequestResult.Value);
            _mediatorMock.Verify(m => m.Send(It.Is<UpdateLivroCommand>(c => c.Id == id), It.IsAny<CancellationToken>()), Times.Once);
            _validatorMock.Verify(v => v.ValidateAsync(request, It.IsAny<CancellationToken>()), Times.Once);
        }
        
        [Fact]
        public async Task Update_QuandoValidacaoFalha_RetornaBadRequest()
        {
            // Arrange
            var id = 1;
            var request = _fixture.Create<LivroRequest>();
            var validationFailures = new List<ValidationFailure>
            {
                new ValidationFailure("Titulo", "O título é obrigatório"),
                new ValidationFailure("AutorId", "O autor é obrigatório")
            };
            
            // Configurar validador para retornar falha
            _validatorMock.Setup(v => v.ValidateAsync(request, It.IsAny<CancellationToken>()))
                .ReturnsAsync(new ValidationResult(validationFailures));

            // Act
            var result = await _controller.Update(id, request, _validatorMock.Object);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            var errors = Assert.IsAssignableFrom<IEnumerable<object>>(badRequestResult.Value);
            Assert.Equal(2, errors.Count());
            _validatorMock.Verify(v => v.ValidateAsync(request, It.IsAny<CancellationToken>()), Times.Once);
            // Verificar que o mediator não foi chamado, pois a validação falhou
            _mediatorMock.Verify(m => m.Send(It.IsAny<UpdateLivroCommand>(), It.IsAny<CancellationToken>()), Times.Never);
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
