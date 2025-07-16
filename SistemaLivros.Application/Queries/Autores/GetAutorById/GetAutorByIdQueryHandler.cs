using MediatR;
using SistemaLivros.Application.DTOs;
using SistemaLivros.Application.Interfaces;
using System.Threading;
using System.Threading.Tasks;

namespace SistemaLivros.Application.Queries.Autores.GetAutorById
{
    public class GetAutorByIdQueryHandler : IRequestHandler<GetAutorByIdQuery, AutorDto>
    {
        private readonly IAutorQueries _autorQueries;

        public GetAutorByIdQueryHandler(IAutorQueries autorQueries)
        {
            _autorQueries = autorQueries;
        }

        public async Task<AutorDto> Handle(GetAutorByIdQuery request, CancellationToken cancellationToken)
        {
            return await _autorQueries.GetByIdAsync(request.Id);
        }
    }
}
