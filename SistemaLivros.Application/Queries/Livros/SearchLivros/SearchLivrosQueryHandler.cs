using MediatR;
using SistemaLivros.Application.DTOs;
using SistemaLivros.Application.Interfaces;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace SistemaLivros.Application.Queries.Livros.SearchLivros
{
    public class SearchLivrosQueryHandler : IRequestHandler<SearchLivrosQuery, IEnumerable<LivroDto>>
    {
        private readonly ILivroQueries _livroQueries;

        public SearchLivrosQueryHandler(ILivroQueries livroQueries)
        {
            _livroQueries = livroQueries;
        }

        public async Task<IEnumerable<LivroDto>> Handle(SearchLivrosQuery request, CancellationToken cancellationToken)
        {
            return await _livroQueries.SearchAsync(request.Termo);
        }
    }
}
