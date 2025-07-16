using FluentAssertions;
using FluentValidation.TestHelper;
using Moq;
using SistemaLivros.API.Models.Request.Livros;
using SistemaLivros.API.Validators.Request.Livros;
using SistemaLivros.Domain.Entities;
using SistemaLivros.Domain.Interfaces;
using System;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace SistemaLivros.Tests.API.Validators.Request.Livros
{
    public class LivroRequestValidatorTests
    {
        private readonly LivroRequestValidator _validator;
        private readonly Mock<IAutorRepository> _autorRepositoryMock;
        private readonly Mock<IGeneroRepository> _generoRepositoryMock;

        public LivroRequestValidatorTests()
        {
            _autorRepositoryMock = new Mock<IAutorRepository>();
            _generoRepositoryMock = new Mock<IGeneroRepository>();
            _validator = new LivroRequestValidator(_autorRepositoryMock.Object, _generoRepositoryMock.Object);
            
            // Configuração padrão para os mocks
            var autor = new Autor("Nome do Autor", "Biografia", new DateTime(1980, 1, 1));
            var genero = new Genero("Nome do Gênero", "Descrição");
            
            _autorRepositoryMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(autor);
            _generoRepositoryMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(genero);
        }

        [Fact]
        public async Task ValidaTituloObrigatorio()
        {
            // Arrange
            var model = new LivroRequest { Titulo = "", Ano = 2020, AutorId = 1, GeneroId = 1 };

            // Act
            var result = await _validator.TestValidateAsync(model);

            // Assert
            result.ShouldHaveValidationErrorFor(x => x.Titulo);
        }

        [Fact]
        public async Task ValidaTamanhoMaximoTitulo()
        {
            // Arrange
            var model = new LivroRequest { Titulo = new string('A', 201), Ano = 2020, AutorId = 1, GeneroId = 1 };

            // Act
            var result = await _validator.TestValidateAsync(model);

            // Assert
            result.ShouldHaveValidationErrorFor(x => x.Titulo);
        }

        [Fact]
        public async Task ValidaAnoMinimo()
        {
            // Arrange
            var model = new LivroRequest { Titulo = "Título Válido", Ano = 999, AutorId = 1, GeneroId = 1 };

            // Act
            var result = await _validator.TestValidateAsync(model);

            // Assert
            result.ShouldHaveValidationErrorFor(x => x.Ano);
        }

        [Fact]
        public async Task ValidaAnoMaximo()
        {
            // Arrange
            var model = new LivroRequest { Titulo = "Título Válido", Ano = 3000, AutorId = 1, GeneroId = 1 };

            // Act
            var result = await _validator.TestValidateAsync(model);

            // Assert
            result.ShouldHaveValidationErrorFor(x => x.Ano);
        }

        [Fact]
        public async Task ValidaAutorInexistente()
        {
            // Arrange
            _autorRepositoryMock.Setup(r => r.GetByIdAsync(99)).ReturnsAsync((Autor)null);
            var model = new LivroRequest { Titulo = "Título Válido", Ano = 2020, AutorId = 99, GeneroId = 1 };

            // Act
            var result = await _validator.TestValidateAsync(model);

            // Assert
            result.ShouldHaveValidationErrorFor(x => x.AutorId);
        }

        [Fact]
        public async Task ValidaGeneroInexistente()
        {
            // Arrange
            _generoRepositoryMock.Setup(r => r.GetByIdAsync(99)).ReturnsAsync((Genero)null);
            var model = new LivroRequest { Titulo = "Título Válido", Ano = 2020, AutorId = 1, GeneroId = 99 };

            // Act
            var result = await _validator.TestValidateAsync(model);

            // Assert
            result.ShouldHaveValidationErrorFor(x => x.GeneroId);
        }

        [Fact]
        public async Task ValidaModeloValido()
        {
            // Arrange
            var model = new LivroRequest { Titulo = "Título Válido", Ano = 2020, AutorId = 1, GeneroId = 1 };

            // Act
            var result = await _validator.TestValidateAsync(model);

            // Assert
            result.ShouldNotHaveAnyValidationErrors();
        }
    }
}
