using MediatR;
using SistemaLivros.Domain.Interfaces;
using System.Threading;
using System.Threading.Tasks;

namespace SistemaLivros.Application.Commands.Generos
{
    public class UpdateGeneroCommandHandler : IRequestHandler<UpdateGeneroCommand, bool>
    {
        private readonly IGeneroRepository _generoRepository;

        public UpdateGeneroCommandHandler(IGeneroRepository generoRepository)
        {
            _generoRepository = generoRepository;
        }

        public async Task<bool> Handle(UpdateGeneroCommand request, CancellationToken cancellationToken)
        {
            var genero = await _generoRepository.GetByIdAsync(request.Id);
            
            if (genero == null)
                return false;
                
            genero.Atualizar(request.Nome, request.Descricao);
            
            await _generoRepository.UpdateAsync(genero);
            
            return true;
        }
    }
}
