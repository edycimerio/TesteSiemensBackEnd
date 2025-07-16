using FluentAssertions;
using FluentValidation.TestHelper;
using SistemaLivros.API.Models.Request.Autores;
using SistemaLivros.API.Validators.Request.Autores;
using System;
using Xunit;

namespace SistemaLivros.Tests.API.Validators.Request.Autores
{
    public class AutorRequestValidatorTests
    {
        private readonly AutorRequestValidator _validator;

        public AutorRequestValidatorTests()
        {
            _validator = new AutorRequestValidator();
        }

        [Fact]
        public void ValidaNomeObrigatorio()
        {
            // Arrange
            var model = new AutorRequest { Nome = "", Biografia = "Uma biografia válida", DataNascimento = DateTime.Now.AddYears(-30) };

            // Act
            var result = _validator.TestValidate(model);

            // Assert
            result.ShouldHaveValidationErrorFor(x => x.Nome);
        }

        [Fact]
        public void ValidaTamanhoMaximoNome()
        {
            // Arrange
            var model = new AutorRequest { Nome = new string('A', 101), Biografia = "Uma biografia válida", DataNascimento = DateTime.Now.AddYears(-30) };

            // Act
            var result = _validator.TestValidate(model);

            // Assert
            result.ShouldHaveValidationErrorFor(x => x.Nome);
        }

        [Fact]
        public void ValidaTamanhoMaximoBiografia()
        {
            // Arrange
            var model = new AutorRequest { Nome = "Nome Válido", Biografia = new string('A', 1001), DataNascimento = DateTime.Now.AddYears(-30) };

            // Act
            var result = _validator.TestValidate(model);

            // Assert
            result.ShouldHaveValidationErrorFor(x => x.Biografia);
        }

        [Fact]
        public void ValidaDataNascimentoFutura()
        {
            // Arrange
            var model = new AutorRequest { Nome = "Nome Válido", Biografia = "Uma biografia válida", DataNascimento = DateTime.Now.AddDays(1) };

            // Act
            var result = _validator.TestValidate(model);

            // Assert
            result.ShouldHaveValidationErrorFor(x => x.DataNascimento);
        }
        
        [Fact]
        public void ValidaDataNascimentoObrigatoria()
        {
            // Arrange
            var model = new AutorRequest { Nome = "Nome Válido", Biografia = "Uma biografia válida", DataNascimento = default };

            // Act
            var result = _validator.TestValidate(model);

            // Assert
            result.ShouldHaveValidationErrorFor(x => x.DataNascimento);
        }

        [Fact]
        public void ValidaModeloValido()
        {
            // Arrange
            var model = new AutorRequest { Nome = "Nome Válido", Biografia = "Uma biografia válida", DataNascimento = DateTime.Now.AddYears(-30) };

            // Act
            var result = _validator.TestValidate(model);

            // Assert
            result.ShouldNotHaveAnyValidationErrors();
        }
    }
}
