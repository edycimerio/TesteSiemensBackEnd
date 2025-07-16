using SistemaLivros.Domain.Entities;
using System.Threading.Tasks;

namespace SistemaLivros.Domain.Interfaces
{
    public interface IAutorRepository : IRepository<Autor>
    {
        Task<Autor> GetByNomeAsync(string nome);
    }
}
