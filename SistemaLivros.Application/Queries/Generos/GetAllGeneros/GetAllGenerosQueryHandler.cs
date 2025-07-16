using MediatR;
using SistemaLivros.Application.Common;
using SistemaLivros.Application.DTOs;
using SistemaLivros.Application.Interfaces;
using System.Threading;
using System.Threading.Tasks;

namespace SistemaLivros.Application.Queries.Generos.GetAllGeneros
{
    public class GetAllGenerosQueryHandler : IRequestHandler<GetAllGenerosQuery, PagedResult<GeneroDto>>
    {
        private readonly IGeneroQueries _generoQueries;

        public GetAllGenerosQueryHandler(IGeneroQueries generoQueries)
        {
            _generoQueries = generoQueries;
        }

        public async Task<PagedResult<GeneroDto>> Handle(GetAllGenerosQuery request, CancellationToken cancellationToken)
        {
            var paginationParams = request.GetPaginationParams();
            return await _generoQueries.GetAllAsync(paginationParams);
        }
    }
}
