using MediatR;
using SistemaLivros.Domain.Interfaces;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace SistemaLivros.Application.Commands.Generos
{
    public class DeleteGeneroCommandHandler : IRequestHandler<DeleteGeneroCommand, bool>
    {
        private readonly IGeneroRepository _generoRepository;

        public DeleteGeneroCommandHandler(IGeneroRepository generoRepository)
        {
            _generoRepository = generoRepository;
        }

        public async Task<bool> Handle(DeleteGeneroCommand request, CancellationToken cancellationToken)
        {
            // Verificar se existem associações na tabela LivroGeneros
            var associacoesCount = await _generoRepository.CountLivrosByGeneroIdAsync(request.Id);
            if (associacoesCount > 0)
            {
                throw new InvalidOperationException($"Não é possível excluir o gênero pois existem {associacoesCount} livros associados a ele.");
            }

            var genero = await _generoRepository.GetByIdAsync(request.Id);
            if (genero == null)
                return false;
                
            await _generoRepository.RemoveAsync(request.Id);
            
            return true;
        }
    }
}
