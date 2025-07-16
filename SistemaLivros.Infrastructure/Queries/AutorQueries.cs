using Dapper;
using SistemaLivros.Application.DTOs;
using SistemaLivros.Application.Interfaces;
using SistemaLivros.Infrastructure.Data;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SistemaLivros.Infrastructure.Queries
{
    public class AutorQueries : IAutorQueries
    {
        private readonly DapperContext _context;

        public AutorQueries(DapperContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<AutorDto>> GetAllAsync()
        {
            const string sql = @"SELECT Id, Nome, Biografia, DataNascimento FROM Autores";
            
            using var connection = _context.CreateConnection();
            return await connection.QueryAsync<AutorDto>(sql);
        }

        public async Task<AutorDto> GetByIdAsync(int id)
        {
            const string sql = @"SELECT Id, Nome, Biografia, DataNascimento FROM Autores WHERE Id = @Id";
            
            using var connection = _context.CreateConnection();
            return await connection.QueryFirstOrDefaultAsync<AutorDto>(sql, new { Id = id });
        }

        public async Task<AutorDetalhesDto> GetDetalhesAsync(int id)
        {
            // 1. Buscar o autor básico
            const string autorSql = @"SELECT Id, Nome, Biografia, DataNascimento FROM Autores WHERE Id = @Id";
            
            using var connection = _context.CreateConnection();
            var autor = await connection.QueryFirstOrDefaultAsync<AutorDetalhesDto>(autorSql, new { Id = id });
            
            if (autor == null)
                return null;
                
            // 2. Buscar os livros associados a este autor
            const string livrosSql = @"
                SELECT l.Id, l.Titulo, l.Ano, l.GeneroId, g.Nome as GeneroNome,
                       l.AutorId, a.Nome as AutorNome
                FROM Livros l
                INNER JOIN Autores a ON l.AutorId = a.Id
                LEFT JOIN Generos g ON l.GeneroId = g.Id
                WHERE l.AutorId = @AutorId";
                
            var livros = await connection.QueryAsync<LivroDto>(livrosSql, new { AutorId = id });
            
            // 3. Associar os livros ao autor
            autor.Livros = livros.ToList();
            
            return autor;
        }
    }
}
