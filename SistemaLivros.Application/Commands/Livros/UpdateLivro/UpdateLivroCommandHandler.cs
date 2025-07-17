using MediatR;
using SistemaLivros.Domain.Interfaces;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace SistemaLivros.Application.Commands.Livros
{
    public class UpdateLivroCommandHandler : IRequestHandler<UpdateLivroCommand, bool>
    {
        private readonly ILivroRepository _livroRepository;
        private readonly IGeneroRepository _generoRepository;
        private readonly IAutorRepository _autorRepository;

        public UpdateLivroCommandHandler(
            ILivroRepository livroRepository,
            IGeneroRepository generoRepository,
            IAutorRepository autorRepository)
        {
            _livroRepository = livroRepository;
            _generoRepository = generoRepository;
            _autorRepository = autorRepository;
        }

        public async Task<bool> Handle(UpdateLivroCommand request, CancellationToken cancellationToken)
        {
            // Verifica se a lista de gêneros não está vazia
            if (request.Generos == null || !request.Generos.Any())
                throw new System.Exception("Pelo menos um gênero deve ser informado.");

            // Verifica se o livro existe
            var livro = await _livroRepository.GetByIdAsync(request.Id);
            if (livro == null)
                return false;

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

            // Atualiza o livro com as informações básicas
            livro.Atualizar(request.Titulo, request.Ano, request.AutorId);
            
            // Remove todos os gêneros da lista usando o ID
            foreach (var generoId in request.Generos.Distinct())
            {
                await _generoRepository.RemoveAsync(generoId);                
            }

            // Adiciona todos os gêneros da lista usando o ID
            foreach (var generoId in request.Generos.Distinct())
            {
                livro.AdicionarGenero(generoId);
            }
            
            await _livroRepository.UpdateAsync(livro);
            
            return true;
        }
    }
}
