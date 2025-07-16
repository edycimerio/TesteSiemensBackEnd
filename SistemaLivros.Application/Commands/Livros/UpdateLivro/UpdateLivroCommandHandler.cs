using MediatR;
using SistemaLivros.Domain.Interfaces;
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
            // Verifica se o livro existe
            var livro = await _livroRepository.GetByIdAsync(request.Id);
            if (livro == null)
                return false;

            // Verifica se o gênero existe
            var genero = await _generoRepository.GetByIdAsync(request.GeneroId);
            if (genero == null)
                throw new System.Exception($"Gênero com ID {request.GeneroId} não encontrado.");

            // Verifica se o autor existe
            var autor = await _autorRepository.GetByIdAsync(request.AutorId);
            if (autor == null)
                throw new System.Exception($"Autor com ID {request.AutorId} não encontrado.");

            livro.Atualizar(request.Titulo, request.Ano, request.GeneroId, request.AutorId);
            
            await _livroRepository.UpdateAsync(livro);
            
            return true;
        }
    }
}
