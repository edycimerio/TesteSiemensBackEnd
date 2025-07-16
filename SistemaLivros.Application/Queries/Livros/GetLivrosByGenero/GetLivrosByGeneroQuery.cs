using MediatR;
using SistemaLivros.Application.Common;
using SistemaLivros.Application.DTOs;

namespace SistemaLivros.Application.Queries.Livros.GetLivrosByGenero
{
    public class GetLivrosByGeneroQuery : IRequest<PagedResult<LivroDto>>
    {
        public int GeneroId { get; set; }
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 10;

        public GetLivrosByGeneroQuery(int generoId, int pageNumber = 1, int pageSize = 10)
        {
            GeneroId = generoId;
            PageNumber = pageNumber;
            PageSize = pageSize;
        }
        
        public PaginationParams GetPaginationParams()
        {
            return new PaginationParams(PageNumber, PageSize);
        }
    }
}
