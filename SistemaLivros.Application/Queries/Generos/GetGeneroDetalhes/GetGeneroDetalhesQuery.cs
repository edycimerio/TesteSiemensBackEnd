using MediatR;
using SistemaLivros.Application.DTOs;

namespace SistemaLivros.Application.Queries.Generos.GetGeneroDetalhes
{
    public class GetGeneroDetalhesQuery : IRequest<GeneroDetalhesDto>
    {
        public int Id { get; set; }

        public GetGeneroDetalhesQuery(int id)
        {
            Id = id;
        }
    }
}
