using SistemaLivros.Application.DTOs;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SistemaLivros.Application.Interfaces
{
    public interface IAutorQueries
    {
        Task<IEnumerable<AutorDto>> GetAllAsync();
        Task<AutorDto> GetByIdAsync(int id);
        Task<AutorDetalhesDto> GetDetalhesAsync(int id);
    }
}
