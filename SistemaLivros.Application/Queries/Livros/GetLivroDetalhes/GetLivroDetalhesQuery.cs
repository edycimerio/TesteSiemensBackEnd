using MediatR;
using SistemaLivros.Application.DTOs;

namespace SistemaLivros.Application.Queries.Livros.GetLivroDetalhes
{
    public class GetLivroDetalhesQuery : IRequest<LivroDetalhesDto>
    {
        public int Id { get; set; }

        public GetLivroDetalhesQuery(int id)
        {
            Id = id;
        }
    }
}
