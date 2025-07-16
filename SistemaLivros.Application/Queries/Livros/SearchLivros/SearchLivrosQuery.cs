using MediatR;
using SistemaLivros.Application.Common;
using SistemaLivros.Application.DTOs;

namespace SistemaLivros.Application.Queries.Livros.SearchLivros
{
    public class SearchLivrosQuery : IRequest<PagedResult<LivroDto>>
    {
        public string Termo { get; set; }
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 10;

        public SearchLivrosQuery(string termo, int pageNumber = 1, int pageSize = 10)
        {
            Termo = termo;
            PageNumber = pageNumber;
            PageSize = pageSize;
        }
        
        public PaginationParams GetPaginationParams()
        {
            return new PaginationParams(PageNumber, PageSize);
        }
    }
}
