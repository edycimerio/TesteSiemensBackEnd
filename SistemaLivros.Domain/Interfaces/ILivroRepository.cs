using SistemaLivros.Domain.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SistemaLivros.Domain.Interfaces
{
    public interface ILivroRepository : IRepository<Livro>
    {
        Task<IEnumerable<Livro>> GetByGeneroIdAsync(int generoId);
        Task<IEnumerable<Livro>> GetByAutorIdAsync(int autorId);
        Task<IEnumerable<Livro>> GetByTituloAsync(string titulo);
    }
}
