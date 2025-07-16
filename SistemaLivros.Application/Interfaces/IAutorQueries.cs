using SistemaLivros.Application.Common;
using SistemaLivros.Application.DTOs;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SistemaLivros.Application.Interfaces
{
    public interface IAutorQueries
    {
        Task<PagedResult<AutorDto>> GetAllAsync(PaginationParams paginationParams);
        Task<AutorDto> GetByIdAsync(int id);
        Task<AutorDetalhesDto> GetDetalhesAsync(int id);
    }
}
