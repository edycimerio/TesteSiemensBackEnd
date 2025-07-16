using MediatR;
using SistemaLivros.Application.DTOs;
using SistemaLivros.Application.Interfaces;
using System.Threading;
using System.Threading.Tasks;

namespace SistemaLivros.Application.Queries.Livros.GetLivroById
{
    public class GetLivroByIdQueryHandler : IRequestHandler<GetLivroByIdQuery, LivroDto>
    {
        private readonly ILivroQueries _livroQueries;

        public GetLivroByIdQueryHandler(ILivroQueries livroQueries)
        {
            _livroQueries = livroQueries;
        }

        public async Task<LivroDto> Handle(GetLivroByIdQuery request, CancellationToken cancellationToken)
        {
            return await _livroQueries.GetByIdAsync(request.Id);
        }
    }
}
