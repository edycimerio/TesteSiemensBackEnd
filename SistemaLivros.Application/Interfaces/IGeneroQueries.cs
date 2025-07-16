using SistemaLivros.Application.DTOs;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SistemaLivros.Application.Interfaces
{
    public interface IGeneroQueries
    {
        Task<IEnumerable<GeneroDto>> GetAllAsync();
        Task<GeneroDto> GetByIdAsync(int id);
        Task<GeneroDetalhesDto> GetDetalhesAsync(int id);
    }
}
