using MediatR;
using SistemaLivros.Application.Common;
using SistemaLivros.Application.DTOs;

namespace SistemaLivros.Application.Queries.Generos.GetAllGeneros
{
    public class GetAllGenerosQuery : IRequest<PagedResult<GeneroDto>>
    {
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 10;
        
        public GetAllGenerosQuery(int pageNumber = 1, int pageSize = 10)
        {
            PageNumber = pageNumber;
            PageSize = pageSize;
        }
        
        public PaginationParams GetPaginationParams()
        {
            return new PaginationParams(PageNumber, PageSize);
        }
    }
}
