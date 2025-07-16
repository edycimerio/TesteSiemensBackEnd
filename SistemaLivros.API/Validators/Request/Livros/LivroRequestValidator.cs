using FluentValidation;
using SistemaLivros.API.Models.Request.Livros;
using SistemaLivros.Domain.Interfaces;
using System;

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
                .NotEmpty().WithMessage("O ID do autor é obrigatório");
                // Removida validação assíncrona que causava erro

            RuleFor(x => x.GeneroId)
                .NotEmpty().WithMessage("O ID do gênero é obrigatório");
                // Removida validação assíncrona que causava erro
        }
    }
}
