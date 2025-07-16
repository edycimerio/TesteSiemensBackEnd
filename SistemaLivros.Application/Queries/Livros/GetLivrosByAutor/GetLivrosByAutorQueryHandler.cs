using MediatR;
using SistemaLivros.Application.Common;
using SistemaLivros.Application.DTOs;
using SistemaLivros.Application.Interfaces;
using System.Threading;
using System.Threading.Tasks;

namespace SistemaLivros.Application.Queries.Livros.GetLivrosByAutor
{
    public class GetLivrosByAutorQueryHandler : IRequestHandler<GetLivrosByAutorQuery, PagedResult<LivroDto>>
    {
        private readonly ILivroQueries _livroQueries;

        public GetLivrosByAutorQueryHandler(ILivroQueries livroQueries)
        {
            _livroQueries = livroQueries;
        }

        public async Task<PagedResult<LivroDto>> Handle(GetLivrosByAutorQuery request, CancellationToken cancellationToken)
        {
            var paginationParams = request.GetPaginationParams();
            return await _livroQueries.GetByAutorIdAsync(request.AutorId, paginationParams);
        }
    }
}
