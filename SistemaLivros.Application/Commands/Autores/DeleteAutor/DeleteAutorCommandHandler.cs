using MediatR;
using SistemaLivros.Domain.Interfaces;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace SistemaLivros.Application.Commands.Autores
{
    public class DeleteAutorCommandHandler : IRequestHandler<DeleteAutorCommand, bool>
    {
        private readonly IAutorRepository _autorRepository;
        private readonly ILivroRepository _livroRepository;

        public DeleteAutorCommandHandler(IAutorRepository autorRepository, ILivroRepository livroRepository)
        {
            _autorRepository = autorRepository;
            _livroRepository = livroRepository;
        }

        public async Task<bool> Handle(DeleteAutorCommand request, CancellationToken cancellationToken)
        {
            // Verificar se existem livros associados ao autor
            var livrosCount = await _livroRepository.CountLivrosByAutorIdAsync(request.Id);
            if (livrosCount > 0)
            {
                throw new InvalidOperationException($"Não é possível excluir o autor pois existem {livrosCount} livros associados a ele.");
            }

            var autor = await _autorRepository.GetByIdAsync(request.Id);
            if (autor == null)
                return false;
                
            await _autorRepository.RemoveAsync(request.Id);
            
            return true;
        }
    }
}
