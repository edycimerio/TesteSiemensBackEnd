using MediatR;
using SistemaLivros.Application.DTOs;
using SistemaLivros.Application.Interfaces;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace SistemaLivros.Application.Queries.Generos.GetAllGeneros
{
    public class GetAllGenerosQueryHandler : IRequestHandler<GetAllGenerosQuery, IEnumerable<GeneroDto>>
    {
        private readonly IGeneroQueries _generoQueries;

        public GetAllGenerosQueryHandler(IGeneroQueries generoQueries)
        {
            _generoQueries = generoQueries;
        }

        public async Task<IEnumerable<GeneroDto>> Handle(GetAllGenerosQuery request, CancellationToken cancellationToken)
        {
            return await _generoQueries.GetAllAsync();
        }
    }
}
