using Dapper;
using SistemaLivros.Application.DTOs;
using SistemaLivros.Application.Interfaces;
using SistemaLivros.Infrastructure.Data;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SistemaLivros.Infrastructure.Queries
{
    public class GeneroQueries : IGeneroQueries
    {
        private readonly DapperContext _context;

        public GeneroQueries(DapperContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<GeneroDto>> GetAllAsync()
        {
            const string sql = @"SELECT Id, Nome, Descricao FROM Generos";
            
            using var connection = _context.CreateConnection();
            return await connection.QueryAsync<GeneroDto>(sql);
        }

        public async Task<GeneroDto> GetByIdAsync(int id)
        {
            const string sql = @"SELECT Id, Nome, Descricao FROM Generos WHERE Id = @Id";
            
            using var connection = _context.CreateConnection();
            return await connection.QueryFirstOrDefaultAsync<GeneroDto>(sql, new { Id = id });
        }

        public async Task<GeneroDetalhesDto> GetDetalhesAsync(int id)
        {
            // Primeiro, busca o gênero básico
            const string generoSql = @"SELECT Id, Nome, Descricao FROM Generos WHERE Id = @Id";
            
            using var connection = _context.CreateConnection();
            var genero = await connection.QueryFirstOrDefaultAsync<GeneroDetalhesDto>(generoSql, new { Id = id });
            
            if (genero == null)
                return null;
                
            // Depois, busca os livros associados a este gênero
            const string livrosSql = @"
                SELECT l.Id, l.Titulo, l.Ano, l.GeneroId, g.Nome as GeneroNome,
                       l.AutorId, a.Nome as AutorNome
                FROM Livros l
                INNER JOIN Generos g ON l.GeneroId = g.Id
                LEFT JOIN Autores a ON l.AutorId = a.Id
                WHERE l.GeneroId = @GeneroId";
                
            var livros = await connection.QueryAsync<LivroDto>(livrosSql, new { GeneroId = id });
            
            // Associa os livros ao gênero
            genero.Livros = livros.ToList();
            
            return genero;
        }
    }
}
