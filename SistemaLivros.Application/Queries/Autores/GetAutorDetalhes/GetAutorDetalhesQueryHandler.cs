using MediatR;
using SistemaLivros.Application.DTOs;
using SistemaLivros.Application.Interfaces;
using System.Threading;
using System.Threading.Tasks;

namespace SistemaLivros.Application.Queries.Autores.GetAutorDetalhes
{
    public class GetAutorDetalhesQueryHandler : IRequestHandler<GetAutorDetalhesQuery, AutorDetalhesDto>
    {
        private readonly IAutorQueries _autorQueries;

        public GetAutorDetalhesQueryHandler(IAutorQueries autorQueries)
        {
            _autorQueries = autorQueries;
        }

        public async Task<AutorDetalhesDto> Handle(GetAutorDetalhesQuery request, CancellationToken cancellationToken)
        {
            return await _autorQueries.GetDetalhesAsync(request.Id);
        }
    }
}
