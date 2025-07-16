using MediatR;
using SistemaLivros.Application.DTOs;
using SistemaLivros.Application.Interfaces;
using System.Threading;
using System.Threading.Tasks;

namespace SistemaLivros.Application.Queries.Generos.GetGeneroById
{
    public class GetGeneroByIdQueryHandler : IRequestHandler<GetGeneroByIdQuery, GeneroDto>
    {
        private readonly IGeneroQueries _generoQueries;

        public GetGeneroByIdQueryHandler(IGeneroQueries generoQueries)
        {
            _generoQueries = generoQueries;
        }

        public async Task<GeneroDto> Handle(GetGeneroByIdQuery request, CancellationToken cancellationToken)
        {
            return await _generoQueries.GetByIdAsync(request.Id);
        }
    }
}
