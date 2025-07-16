using MediatR;
using SistemaLivros.Application.DTOs;
using SistemaLivros.Application.Interfaces;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace SistemaLivros.Application.Queries.Livros.GetLivrosByAutor
{
    public class GetLivrosByAutorQueryHandler : IRequestHandler<GetLivrosByAutorQuery, IEnumerable<LivroDto>>
    {
        private readonly ILivroQueries _livroQueries;

        public GetLivrosByAutorQueryHandler(ILivroQueries livroQueries)
        {
            _livroQueries = livroQueries;
        }

        public async Task<IEnumerable<LivroDto>> Handle(GetLivrosByAutorQuery request, CancellationToken cancellationToken)
        {
            return await _livroQueries.GetByAutorIdAsync(request.AutorId);
        }
    }
}
