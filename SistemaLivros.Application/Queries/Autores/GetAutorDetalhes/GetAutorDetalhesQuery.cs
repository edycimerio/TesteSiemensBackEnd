using MediatR;
using SistemaLivros.Application.DTOs;

namespace SistemaLivros.Application.Queries.Autores.GetAutorDetalhes
{
    public class GetAutorDetalhesQuery : IRequest<AutorDetalhesDto>
    {
        public int Id { get; set; }

        public GetAutorDetalhesQuery(int id)
        {
            Id = id;
        }
    }
}
