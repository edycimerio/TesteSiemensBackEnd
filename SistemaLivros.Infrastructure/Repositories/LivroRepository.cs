using Microsoft.EntityFrameworkCore;
using SistemaLivros.Domain.Entities;
using SistemaLivros.Domain.Interfaces;
using SistemaLivros.Infrastructure.Data;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SistemaLivros.Infrastructure.Repositories
{
    public class LivroRepository : Repository<Livro>, ILivroRepository
    {
        public LivroRepository(AppDbContext context) : base(context)
        {
        }

        public async Task<IEnumerable<Livro>> GetByAutorIdAsync(int autorId)
        {
            return await _dbSet
                .Where(l => l.AutorId == autorId)
                .Include(l => l.Autor)
                .Include(l => l.LivroGeneros)
                    .ThenInclude(lg => lg.Genero)
                .ToListAsync();
        }

        public async Task<IEnumerable<Livro>> GetByGeneroIdAsync(int generoId)
        {
            return await _dbSet
                .Where(l => l.LivroGeneros.Any(lg => lg.GeneroId == generoId))
                .Include(l => l.Autor)
                .Include(l => l.LivroGeneros)
                    .ThenInclude(lg => lg.Genero)
                .ToListAsync();
        }

        public async Task<IEnumerable<Livro>> GetByTituloAsync(string titulo)
        {
            return await _dbSet
                .Where(l => l.Titulo.ToLower().Contains(titulo.ToLower()))
                .Include(l => l.Autor)
                .Include(l => l.LivroGeneros)
                    .ThenInclude(lg => lg.Genero)
                .ToListAsync();
        }

        public override async Task<Livro> GetByIdAsync(int id)
        {
            return await _dbSet
                .Include(l => l.Autor)
                .Include(l => l.LivroGeneros)
                    .ThenInclude(lg => lg.Genero)
                .FirstOrDefaultAsync(l => l.Id == id);
        }

        public override async Task<IEnumerable<Livro>> GetAllAsync()
        {
            return await _dbSet
                .Include(l => l.Autor)
                .Include(l => l.LivroGeneros)
                    .ThenInclude(lg => lg.Genero)
                .ToListAsync();
        }

        public async Task<int> CountLivrosByAutorIdAsync(int autorId)
        {
            return await _dbSet
                .CountAsync(l => l.AutorId == autorId);
        }
    }
}
