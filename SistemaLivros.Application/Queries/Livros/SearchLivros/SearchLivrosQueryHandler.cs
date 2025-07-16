using MediatR;
using SistemaLivros.Application.Common;
using SistemaLivros.Application.DTOs;
using SistemaLivros.Application.Interfaces;
using System.Threading;
using System.Threading.Tasks;

namespace SistemaLivros.Application.Queries.Livros.SearchLivros
{
    public class SearchLivrosQueryHandler : IRequestHandler<SearchLivrosQuery, PagedResult<LivroDto>>
    {
        private readonly ILivroQueries _livroQueries;

        public SearchLivrosQueryHandler(ILivroQueries livroQueries)
        {
            _livroQueries = livroQueries;
        }

        public async Task<PagedResult<LivroDto>> Handle(SearchLivrosQuery request, CancellationToken cancellationToken)
        {
            var paginationParams = request.GetPaginationParams();
            return await _livroQueries.SearchAsync(request.Termo, paginationParams);
        }
    }
}
