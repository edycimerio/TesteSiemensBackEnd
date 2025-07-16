using MediatR;
using SistemaLivros.Application.DTOs;
using System.Collections.Generic;

namespace SistemaLivros.Application.Queries.Livros.GetLivrosByGenero
{
    public class GetLivrosByGeneroQuery : IRequest<IEnumerable<LivroDto>>
    {
        public int GeneroId { get; set; }

        public GetLivrosByGeneroQuery(int generoId)
        {
            GeneroId = generoId;
        }
    }
}
