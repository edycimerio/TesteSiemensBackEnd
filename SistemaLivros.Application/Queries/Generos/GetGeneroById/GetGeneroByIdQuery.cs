using MediatR;
using SistemaLivros.Application.DTOs;

namespace SistemaLivros.Application.Queries.Generos.GetGeneroById
{
    public class GetGeneroByIdQuery : IRequest<GeneroDto>
    {
        public int Id { get; set; }

        public GetGeneroByIdQuery(int id)
        {
            Id = id;
        }
    }
}
