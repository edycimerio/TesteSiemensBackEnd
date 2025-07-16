using SistemaLivros.Application.Common;
using SistemaLivros.Application.DTOs;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SistemaLivros.Application.Interfaces
{
    public interface ILivroQueries
    {
        Task<PagedResult<LivroDto>> GetAllAsync(PaginationParams paginationParams);
        Task<LivroDto> GetByIdAsync(int id);
        Task<LivroDetalhesDto> GetDetalhesAsync(int id);
        Task<PagedResult<LivroDto>> GetByGeneroIdAsync(int generoId, PaginationParams paginationParams);
        Task<PagedResult<LivroDto>> GetByAutorIdAsync(int autorId, PaginationParams paginationParams);
        Task<PagedResult<LivroDto>> SearchAsync(string termo, PaginationParams paginationParams);
    }
}
