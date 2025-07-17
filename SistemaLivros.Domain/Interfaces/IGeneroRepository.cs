using SistemaLivros.Domain.Entities;
using System.Threading.Tasks;

namespace SistemaLivros.Domain.Interfaces
{
    public interface IGeneroRepository : IRepository<Genero>
    {
        Task<Genero> GetByNomeAsync(string nome);
        Task<int> CountLivrosByGeneroIdAsync(int generoId);
    }
}
