using Microsoft.EntityFrameworkCore;
using SistemaLivros.Domain.Entities;
using SistemaLivros.Domain.Interfaces;
using SistemaLivros.Infrastructure.Data;
using System.Threading.Tasks;

namespace SistemaLivros.Infrastructure.Repositories
{
    public class AutorRepository : Repository<Autor>, IAutorRepository
    {
        public AutorRepository(AppDbContext context) : base(context)
        {
        }

        public async Task<Autor> GetByNomeAsync(string nome)
        {
            return await _dbSet.FirstOrDefaultAsync(a => a.Nome.ToLower() == nome.ToLower());
        }
    }
}
