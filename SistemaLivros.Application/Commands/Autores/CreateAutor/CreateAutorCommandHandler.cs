using MediatR;
using SistemaLivros.Domain.Entities;
using SistemaLivros.Domain.Interfaces;
using System.Threading;
using System.Threading.Tasks;

namespace SistemaLivros.Application.Commands.Autores
{
    public class CreateAutorCommandHandler : IRequestHandler<CreateAutorCommand, int>
    {
        private readonly IAutorRepository _autorRepository;

        public CreateAutorCommandHandler(IAutorRepository autorRepository)
        {
            _autorRepository = autorRepository;
        }

        public async Task<int> Handle(CreateAutorCommand request, CancellationToken cancellationToken)
        {
            var autor = new Autor(request.Nome, request.Biografia);
            
            await _autorRepository.AddAsync(autor);
            
            return autor.Id;
        }
    }
}
