using MediatR;
using SistemaLivros.Domain.Interfaces;
using System.Threading;
using System.Threading.Tasks;

namespace SistemaLivros.Application.Commands.Autores
{
    public class UpdateAutorCommandHandler : IRequestHandler<UpdateAutorCommand, bool>
    {
        private readonly IAutorRepository _autorRepository;

        public UpdateAutorCommandHandler(IAutorRepository autorRepository)
        {
            _autorRepository = autorRepository;
        }

        public async Task<bool> Handle(UpdateAutorCommand request, CancellationToken cancellationToken)
        {
            var autor = await _autorRepository.GetByIdAsync(request.Id);
            
            if (autor == null)
                return false;
                
            autor.Atualizar(request.Nome, request.Biografia, request.DataNascimento);
            
            await _autorRepository.UpdateAsync(autor);
            
            return true;
        }
    }
}
