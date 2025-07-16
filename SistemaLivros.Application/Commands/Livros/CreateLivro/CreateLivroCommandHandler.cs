using MediatR;
using SistemaLivros.Domain.Entities;
using SistemaLivros.Domain.Interfaces;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace SistemaLivros.Application.Commands.Livros
{
    public class CreateLivroCommandHandler : IRequestHandler<CreateLivroCommand, int>
    {
        private readonly ILivroRepository _livroRepository;
        private readonly IGeneroRepository _generoRepository;
        private readonly IAutorRepository _autorRepository;

        public CreateLivroCommandHandler(
            ILivroRepository livroRepository,
            IGeneroRepository generoRepository,
            IAutorRepository autorRepository)
        {
            _livroRepository = livroRepository;
            _generoRepository = generoRepository;
            _autorRepository = autorRepository;
        }

        public async Task<int> Handle(CreateLivroCommand request, CancellationToken cancellationToken)
        {
            // Verifica se a lista de gêneros não está vazia
            if (request.Generos == null || !request.Generos.Any())
                throw new System.Exception("Pelo menos um gênero deve ser informado.");

            // Verifica se o autor existe
            var autor = await _autorRepository.GetByIdAsync(request.AutorId);
            if (autor == null)
                throw new System.Exception($"Autor com ID {request.AutorId} não encontrado.");

            // Verifica se todos os gêneros existem
            foreach (var generoId in request.Generos.Distinct())
            {
                var genero = await _generoRepository.GetByIdAsync(generoId);
                if (genero == null)
                    throw new System.Exception($"Gênero com ID {generoId} não encontrado.");
            }

            // Cria o livro apenas com autor e informações básicas
            var livro = new Livro(request.Titulo, request.Ano, request.AutorId);
            
            // Salva o livro primeiro para obter um ID válido
            await _livroRepository.AddAsync(livro);

            // Adiciona todos os gêneros ao livro usando o ID do gênero
            foreach (var generoId in request.Generos.Distinct())
            {
                livro.AdicionarGenero(generoId);
            }
            
            // Atualiza o livro com os gêneros adicionados
            await _livroRepository.UpdateAsync(livro);
            
            return livro.Id;
        }
    }
}
