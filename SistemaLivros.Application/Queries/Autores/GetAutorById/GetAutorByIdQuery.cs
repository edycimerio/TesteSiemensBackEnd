using MediatR;
using SistemaLivros.Application.DTOs;

namespace SistemaLivros.Application.Queries.Autores.GetAutorById
{
    public class GetAutorByIdQuery : IRequest<AutorDto>
    {
        public int Id { get; set; }

        public GetAutorByIdQuery(int id)
        {
            Id = id;
        }
    }
}
