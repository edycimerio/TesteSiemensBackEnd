using MediatR;
using SistemaLivros.Domain.Interfaces;
using System.Threading;
using System.Threading.Tasks;

namespace SistemaLivros.Application.Commands.Autores
{
    public class DeleteAutorCommandHandler : IRequestHandler<DeleteAutorCommand, bool>
    {
        private readonly IAutorRepository _autorRepository;

        public DeleteAutorCommandHandler(IAutorRepository autorRepository)
        {
            _autorRepository = autorRepository;
        }

        public async Task<bool> Handle(DeleteAutorCommand request, CancellationToken cancellationToken)
        {
            var autor = await _autorRepository.GetByIdAsync(request.Id);
            
            if (autor == null)
                return false;
                
            await _autorRepository.RemoveAsync(request.Id);
            
            return true;
        }
    }
}
