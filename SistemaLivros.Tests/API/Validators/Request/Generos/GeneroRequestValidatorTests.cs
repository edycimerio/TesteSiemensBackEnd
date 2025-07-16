using FluentAssertions;
using FluentValidation.TestHelper;
using SistemaLivros.API.Models.Request.Generos;
using SistemaLivros.API.Validators.Request.Generos;
using Xunit;

namespace SistemaLivros.Tests.API.Validators.Request.Generos
{
    public class GeneroRequestValidatorTests
    {
        private readonly GeneroRequestValidator _validator;

        public GeneroRequestValidatorTests()
        {
            _validator = new GeneroRequestValidator();
        }

        [Fact]
        public void ValidaNomeObrigatorio()
        {
            // Arrange
            var model = new GeneroRequest { Nome = "", Descricao = "Uma descrição válida" };

            // Act
            var result = _validator.TestValidate(model);

            // Assert
            result.ShouldHaveValidationErrorFor(x => x.Nome);
        }

        [Fact]
        public void ValidaTamanhoMaximoNome()
        {
            // Arrange
            var model = new GeneroRequest { Nome = new string('A', 101), Descricao = "Uma descrição válida" };

            // Act
            var result = _validator.TestValidate(model);

            // Assert
            result.ShouldHaveValidationErrorFor(x => x.Nome);
        }

        [Fact]
        public void ValidaTamanhoMaximoDescricao()
        {
            // Arrange
            var model = new GeneroRequest { Nome = "Nome Válido", Descricao = new string('A', 501) };

            // Act
            var result = _validator.TestValidate(model);

            // Assert
            result.ShouldHaveValidationErrorFor(x => x.Descricao);
        }

        [Fact]
        public void ValidaModeloValido()
        {
            // Arrange
            var model = new GeneroRequest { Nome = "Nome Válido", Descricao = "Uma descrição válida" };

            // Act
            var result = _validator.TestValidate(model);

            // Assert
            result.ShouldNotHaveAnyValidationErrors();
        }
    }
}
