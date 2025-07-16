using MediatR;
using SistemaLivros.Application.Common;
using SistemaLivros.Application.DTOs;
using SistemaLivros.Application.Interfaces;
using System.Threading;
using System.Threading.Tasks;

namespace SistemaLivros.Application.Queries.Livros.GetLivrosByGenero
{
    public class GetLivrosByGeneroQueryHandler : IRequestHandler<GetLivrosByGeneroQuery, PagedResult<LivroDto>>
    {
        private readonly ILivroQueries _livroQueries;

        public GetLivrosByGeneroQueryHandler(ILivroQueries livroQueries)
        {
            _livroQueries = livroQueries;
        }

        public async Task<PagedResult<LivroDto>> Handle(GetLivrosByGeneroQuery request, CancellationToken cancellationToken)
        {
            var paginationParams = request.GetPaginationParams();
            return await _livroQueries.GetByGeneroIdAsync(request.GeneroId, paginationParams);
        }
    }
}
