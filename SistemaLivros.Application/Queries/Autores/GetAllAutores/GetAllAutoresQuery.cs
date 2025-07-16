using MediatR;
using SistemaLivros.Application.Common;
using SistemaLivros.Application.DTOs;

namespace SistemaLivros.Application.Queries.Autores.GetAllAutores
{
    public class GetAllAutoresQuery : IRequest<PagedResult<AutorDto>>
    {
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 10;
        
        public GetAllAutoresQuery(int pageNumber = 1, int pageSize = 10)
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
