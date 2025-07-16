using MediatR;
using SistemaLivros.Application.DTOs;

namespace SistemaLivros.Application.Queries.Livros.GetLivroById
{
    public class GetLivroByIdQuery : IRequest<LivroDto>
    {
        public int Id { get; set; }

        public GetLivroByIdQuery(int id)
        {
            Id = id;
        }
    }
}
