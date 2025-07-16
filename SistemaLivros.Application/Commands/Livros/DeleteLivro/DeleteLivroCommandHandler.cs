using MediatR;
using SistemaLivros.Domain.Interfaces;
using System.Threading;
using System.Threading.Tasks;

namespace SistemaLivros.Application.Commands.Livros
{
    public class DeleteLivroCommandHandler : IRequestHandler<DeleteLivroCommand, bool>
    {
        private readonly ILivroRepository _livroRepository;

        public DeleteLivroCommandHandler(ILivroRepository livroRepository)
        {
            _livroRepository = livroRepository;
        }

        public async Task<bool> Handle(DeleteLivroCommand request, CancellationToken cancellationToken)
        {
            var livro = await _livroRepository.GetByIdAsync(request.Id);
            
            if (livro == null)
                return false;
                
            await _livroRepository.RemoveAsync(request.Id);
            
            return true;
        }
    }
}
