using MediatR;
using SistemaLivros.Application.DTOs;
using SistemaLivros.Application.Interfaces;
using System.Threading;
using System.Threading.Tasks;

namespace SistemaLivros.Application.Queries.Livros.GetLivroDetalhes
{
    public class GetLivroDetalhesQueryHandler : IRequestHandler<GetLivroDetalhesQuery, LivroDetalhesDto>
    {
        private readonly ILivroQueries _livroQueries;

        public GetLivroDetalhesQueryHandler(ILivroQueries livroQueries)
        {
            _livroQueries = livroQueries;
        }

        public async Task<LivroDetalhesDto> Handle(GetLivroDetalhesQuery request, CancellationToken cancellationToken)
        {
            return await _livroQueries.GetDetalhesAsync(request.Id);
        }
    }
}
