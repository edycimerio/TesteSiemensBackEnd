using MediatR;
using SistemaLivros.Application.DTOs;
using SistemaLivros.Application.Interfaces;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace SistemaLivros.Application.Queries.Autores.GetAllAutores
{
    public class GetAllAutoresQueryHandler : IRequestHandler<GetAllAutoresQuery, IEnumerable<AutorDto>>
    {
        private readonly IAutorQueries _autorQueries;

        public GetAllAutoresQueryHandler(IAutorQueries autorQueries)
        {
            _autorQueries = autorQueries;
        }

        public async Task<IEnumerable<AutorDto>> Handle(GetAllAutoresQuery request, CancellationToken cancellationToken)
        {
            return await _autorQueries.GetAllAsync();
        }
    }
}
