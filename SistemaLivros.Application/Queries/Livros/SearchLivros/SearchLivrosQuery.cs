using MediatR;
using SistemaLivros.Application.DTOs;
using System.Collections.Generic;

namespace SistemaLivros.Application.Queries.Livros.SearchLivros
{
    public class SearchLivrosQuery : IRequest<IEnumerable<LivroDto>>
    {
        public string Termo { get; set; }

        public SearchLivrosQuery(string termo)
        {
            Termo = termo;
        }
    }
}
