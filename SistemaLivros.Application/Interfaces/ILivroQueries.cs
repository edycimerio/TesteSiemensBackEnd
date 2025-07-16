using SistemaLivros.Application.DTOs;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SistemaLivros.Application.Interfaces
{
    public interface ILivroQueries
    {
        Task<IEnumerable<LivroDto>> GetAllAsync();
        Task<LivroDto> GetByIdAsync(int id);
        Task<LivroDetalhesDto> GetDetalhesAsync(int id);
        Task<IEnumerable<LivroDto>> GetByGeneroIdAsync(int generoId);
        Task<IEnumerable<LivroDto>> GetByAutorIdAsync(int autorId);
        Task<IEnumerable<LivroDto>> SearchAsync(string termo);
    }
}
