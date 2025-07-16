using Dapper;
using SistemaLivros.Application.DTOs;
using SistemaLivros.Application.Interfaces;
using SistemaLivros.Infrastructure.Data;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SistemaLivros.Infrastructure.Queries
{
    public class LivroQueries : ILivroQueries
    {
        private readonly DapperContext _context;

        public LivroQueries(DapperContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<LivroDto>> GetAllAsync()
        {
            const string sql = @"
                SELECT l.Id, l.Titulo, l.Ano, 
                       l.GeneroId, g.Nome as GeneroNome,
                       l.AutorId, a.Nome as AutorNome
                FROM Livros l
                INNER JOIN Generos g ON l.GeneroId = g.Id
                INNER JOIN Autores a ON l.AutorId = a.Id";
            
            using var connection = _context.CreateConnection();
            return await connection.QueryAsync<LivroDto>(sql);
        }

        public async Task<LivroDto> GetByIdAsync(int id)
        {
            const string sql = @"
                SELECT l.Id, l.Titulo, l.Ano, 
                       l.GeneroId, g.Nome as GeneroNome,
                       l.AutorId, a.Nome as AutorNome
                FROM Livros l
                INNER JOIN Generos g ON l.GeneroId = g.Id
                INNER JOIN Autores a ON l.AutorId = a.Id
                WHERE l.Id = @Id";
            
            using var connection = _context.CreateConnection();
            return await connection.QueryFirstOrDefaultAsync<LivroDto>(sql, new { Id = id });
        }

        public async Task<IEnumerable<LivroDto>> GetByAutorIdAsync(int autorId)
        {
            const string sql = @"
                SELECT l.Id, l.Titulo, l.Ano, 
                       l.GeneroId, g.Nome as GeneroNome,
                       l.AutorId, a.Nome as AutorNome
                FROM Livros l
                INNER JOIN Generos g ON l.GeneroId = g.Id
                INNER JOIN Autores a ON l.AutorId = a.Id
                WHERE l.AutorId = @AutorId";
            
            using var connection = _context.CreateConnection();
            return await connection.QueryAsync<LivroDto>(sql, new { AutorId = autorId });
        }

        public async Task<IEnumerable<LivroDto>> GetByGeneroIdAsync(int generoId)
        {
            const string sql = @"
                SELECT l.Id, l.Titulo, l.Ano, 
                       l.GeneroId, g.Nome as GeneroNome,
                       l.AutorId, a.Nome as AutorNome
                FROM Livros l
                INNER JOIN Generos g ON l.GeneroId = g.Id
                INNER JOIN Autores a ON l.AutorId = a.Id
                WHERE l.GeneroId = @GeneroId";
            
            using var connection = _context.CreateConnection();
            return await connection.QueryAsync<LivroDto>(sql, new { GeneroId = generoId });
        }

        public async Task<IEnumerable<LivroDto>> SearchAsync(string termo)
        {
            const string sql = @"
                SELECT l.Id, l.Titulo, l.Ano, 
                       l.GeneroId, g.Nome as GeneroNome,
                       l.AutorId, a.Nome as AutorNome
                FROM Livros l
                INNER JOIN Generos g ON l.GeneroId = g.Id
                INNER JOIN Autores a ON l.AutorId = a.Id
                WHERE l.Titulo LIKE @Termo OR a.Nome LIKE @Termo OR g.Nome LIKE @Termo";
            
            using var connection = _context.CreateConnection();
            return await connection.QueryAsync<LivroDto>(sql, new { Termo = $"%{termo}%" });
        }
    }
}
