using MediatR;
using SistemaLivros.Application.DTOs;
using System.Collections.Generic;

namespace SistemaLivros.Application.Queries.Livros.GetLivrosByAutor
{
    public class GetLivrosByAutorQuery : IRequest<IEnumerable<LivroDto>>
    {
        public int AutorId { get; set; }

        public GetLivrosByAutorQuery(int autorId)
        {
            AutorId = autorId;
        }
    }
}
