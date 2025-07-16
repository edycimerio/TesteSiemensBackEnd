using FluentValidation;
using SistemaLivros.API.Models.Request.Livros;
using SistemaLivros.Domain.Interfaces;
using System;
using System.Linq;

namespace SistemaLivros.API.Validators.Request.Livros
{
    public class LivroRequestValidator : AbstractValidator<LivroRequest>
    {
        private readonly IAutorRepository _autorRepository;
        private readonly IGeneroRepository _generoRepository;

        public LivroRequestValidator(IAutorRepository autorRepository, IGeneroRepository generoRepository)
        {
            _autorRepository = autorRepository;
            _generoRepository = generoRepository;

            RuleFor(x => x.Titulo)
                .NotEmpty().WithMessage("O título do livro é obrigatório")
                .MaximumLength(200).WithMessage("O título do livro deve ter no máximo {MaxLength} caracteres");

            RuleFor(x => x.Ano)
                .NotEmpty().WithMessage("O ano do livro é obrigatório")
                .InclusiveBetween(1000, DateTime.Now.Year).WithMessage($"O ano deve estar entre 1000 e {DateTime.Now.Year}");

            RuleFor(x => x.AutorId)
                .NotEmpty().WithMessage("O ID do autor é obrigatório")
                .MustAsync(async (autorId, cancellation) => 
                {
                    var autor = await _autorRepository.GetByIdAsync(autorId);
                    return autor != null;
                }).WithMessage("O autor informado não existe");

            RuleFor(x => x.Generos)
                .NotNull().WithMessage("A lista de gêneros não pode ser nula")
                .Must(generos => generos != null && generos.Count > 0)
                .WithMessage("Pelo menos um gênero deve ser informado")
                .ForEach(generoRule => generoRule
                    .MustAsync(async (generoId, cancellation) => 
                    {
                        var genero = await _generoRepository.GetByIdAsync(generoId);
                        return genero != null;
                    }).WithMessage((_, generoId) => $"O gênero com ID {generoId} não existe"));
        }
    }
}
