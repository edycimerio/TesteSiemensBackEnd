using MediatR;
using SistemaLivros.Application.DTOs;
using SistemaLivros.Application.Interfaces;
using System.Threading;
using System.Threading.Tasks;

namespace SistemaLivros.Application.Queries.Generos.GetGeneroDetalhes
{
    public class GetGeneroDetalhesQueryHandler : IRequestHandler<GetGeneroDetalhesQuery, GeneroDetalhesDto>
    {
        private readonly IGeneroQueries _generoQueries;

        public GetGeneroDetalhesQueryHandler(IGeneroQueries generoQueries)
        {
            _generoQueries = generoQueries;
        }

        public async Task<GeneroDetalhesDto> Handle(GetGeneroDetalhesQuery request, CancellationToken cancellationToken)
        {
            return await _generoQueries.GetDetalhesAsync(request.Id);
        }
    }
}
