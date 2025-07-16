using FluentValidation;
using SistemaLivros.API.Models.Request.Autores;
using System;

namespace SistemaLivros.API.Validators.Request.Autores
{
    public class AutorRequestValidator : AbstractValidator<AutorRequest>
    {
        public AutorRequestValidator()
        {
            RuleFor(x => x.Nome)
                .NotEmpty().WithMessage("O nome do autor é obrigatório")
                .MaximumLength(100).WithMessage("O nome do autor deve ter no máximo {MaxLength} caracteres");

            RuleFor(x => x.Biografia)
                .MaximumLength(1000).WithMessage("A biografia do autor deve ter no máximo {MaxLength} caracteres")
                .When(x => !string.IsNullOrEmpty(x.Biografia));

            RuleFor(x => x.DataNascimento)
                .LessThanOrEqualTo(DateTime.Today).WithMessage("A data de nascimento não pode ser no futuro")
                .When(x => x.DataNascimento.HasValue);
        }
    }
}
