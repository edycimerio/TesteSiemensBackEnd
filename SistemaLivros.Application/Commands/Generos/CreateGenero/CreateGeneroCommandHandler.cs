using MediatR;
using SistemaLivros.Domain.Entities;
using SistemaLivros.Domain.Interfaces;
using System.Threading;
using System.Threading.Tasks;

namespace SistemaLivros.Application.Commands.Generos
{
    public class CreateGeneroCommandHandler : IRequestHandler<CreateGeneroCommand, int>
    {
        private readonly IGeneroRepository _generoRepository;

        public CreateGeneroCommandHandler(IGeneroRepository generoRepository)
        {
            _generoRepository = generoRepository;
        }

        public async Task<int> Handle(CreateGeneroCommand request, CancellationToken cancellationToken)
        {
            var genero = new Genero(request.Nome, request.Descricao);
            
            await _generoRepository.AddAsync(genero);
            
            return genero.Id;
        }
    }
}
