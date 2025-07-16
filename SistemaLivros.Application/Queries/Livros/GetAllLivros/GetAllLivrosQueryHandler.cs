using MediatR;
using SistemaLivros.Application.DTOs;
using SistemaLivros.Application.Interfaces;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace SistemaLivros.Application.Queries.Livros.GetAllLivros
{
    public class GetAllLivrosQueryHandler : IRequestHandler<GetAllLivrosQuery, IEnumerable<LivroDto>>
    {
        private readonly ILivroQueries _livroQueries;

        public GetAllLivrosQueryHandler(ILivroQueries livroQueries)
        {
            _livroQueries = livroQueries;
        }

        public async Task<IEnumerable<LivroDto>> Handle(GetAllLivrosQuery request, CancellationToken cancellationToken)
        {
            return await _livroQueries.GetAllAsync();
        }
    }
}
