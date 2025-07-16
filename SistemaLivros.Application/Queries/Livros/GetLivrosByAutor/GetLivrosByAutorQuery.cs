using MediatR;
using SistemaLivros.Application.Common;
using SistemaLivros.Application.DTOs;

namespace SistemaLivros.Application.Queries.Livros.GetLivrosByAutor
{
    public class GetLivrosByAutorQuery : IRequest<PagedResult<LivroDto>>
    {
        public int AutorId { get; set; }
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 10;

        public GetLivrosByAutorQuery(int autorId, int pageNumber = 1, int pageSize = 10)
        {
            AutorId = autorId;
            PageNumber = pageNumber;
            PageSize = pageSize;
        }
        
        public PaginationParams GetPaginationParams()
        {
            return new PaginationParams(PageNumber, PageSize);
        }
    }
}
