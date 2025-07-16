using MediatR;
using SistemaLivros.Application.Common;
using SistemaLivros.Application.DTOs;
using SistemaLivros.Application.Interfaces;
using System.Threading;
using System.Threading.Tasks;

namespace SistemaLivros.Application.Queries.Autores.GetAllAutores
{
    public class GetAllAutoresQueryHandler : IRequestHandler<GetAllAutoresQuery, PagedResult<AutorDto>>
    {
        private readonly IAutorQueries _autorQueries;

        public GetAllAutoresQueryHandler(IAutorQueries autorQueries)
        {
            _autorQueries = autorQueries;
        }

        public async Task<PagedResult<AutorDto>> Handle(GetAllAutoresQuery request, CancellationToken cancellationToken)
        {
            var paginationParams = request.GetPaginationParams();
            return await _autorQueries.GetAllAsync(paginationParams);
        }
    }
}
