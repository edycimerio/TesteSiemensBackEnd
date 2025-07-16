using MediatR;
using SistemaLivros.Application.DTOs;
using SistemaLivros.Application.Interfaces;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace SistemaLivros.Application.Queries.Livros.GetLivrosByGenero
{
    public class GetLivrosByGeneroQueryHandler : IRequestHandler<GetLivrosByGeneroQuery, IEnumerable<LivroDto>>
    {
        private readonly ILivroQueries _livroQueries;

        public GetLivrosByGeneroQueryHandler(ILivroQueries livroQueries)
        {
            _livroQueries = livroQueries;
        }

        public async Task<IEnumerable<LivroDto>> Handle(GetLivrosByGeneroQuery request, CancellationToken cancellationToken)
        {
            return await _livroQueries.GetByGeneroIdAsync(request.GeneroId);
        }
    }
}
