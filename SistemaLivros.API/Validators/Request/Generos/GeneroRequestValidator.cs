using FluentValidation;
using SistemaLivros.API.Models.Request.Generos;

namespace SistemaLivros.API.Validators.Request.Generos
{
    public class GeneroRequestValidator : AbstractValidator<GeneroRequest>
    {
        public GeneroRequestValidator()
        {
            RuleFor(x => x.Nome)
                .NotEmpty().WithMessage("O nome do gênero é obrigatório")
                .MaximumLength(100).WithMessage("O nome do gênero deve ter no máximo {MaxLength} caracteres");

            RuleFor(x => x.Descricao)
                .MaximumLength(500).WithMessage("A descrição do gênero deve ter no máximo {MaxLength} caracteres")
                .When(x => !string.IsNullOrEmpty(x.Descricao));
        }
    }
}
