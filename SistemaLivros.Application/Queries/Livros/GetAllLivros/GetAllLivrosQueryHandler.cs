using MediatR;
using SistemaLivros.Application.Common;
using SistemaLivros.Application.DTOs;
using SistemaLivros.Application.Interfaces;
using System.Threading;
using System.Threading.Tasks;

namespace SistemaLivros.Application.Queries.Livros.GetAllLivros
{
    public class GetAllLivrosQueryHandler : IRequestHandler<GetAllLivrosQuery, PagedResult<LivroDto>>
    {
        private readonly ILivroQueries _livroQueries;

        public GetAllLivrosQueryHandler(ILivroQueries livroQueries)
        {
            _livroQueries = livroQueries;
        }

        public async Task<PagedResult<LivroDto>> Handle(GetAllLivrosQuery request, CancellationToken cancellationToken)
        {
            var paginationParams = request.GetPaginationParams();
            return await _livroQueries.GetAllAsync(paginationParams);
        }
    }
}
