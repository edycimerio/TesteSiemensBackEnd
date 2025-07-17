using Microsoft.EntityFrameworkCore;
using SistemaLivros.Domain.Entities;
using SistemaLivros.Domain.Interfaces;
using SistemaLivros.Infrastructure.Data;
using System.Linq;
using System.Threading.Tasks;

namespace SistemaLivros.Infrastructure.Repositories
{
    public class GeneroRepository : Repository<Genero>, IGeneroRepository
    {
        public GeneroRepository(AppDbContext context) : base(context)
        {
        }

        public async Task<Genero> GetByNomeAsync(string nome)
        {
            return await _dbSet.FirstOrDefaultAsync(g => g.Nome.ToLower() == nome.ToLower());
        }

        public async Task<int> CountLivrosByGeneroIdAsync(int generoId)
        {
            return await _context.LivroGeneros.CountAsync(lg => lg.GeneroId == generoId);
        }
    }
}
