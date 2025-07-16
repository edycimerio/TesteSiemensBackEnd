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
            const string sql = @"SELECT Id, Nome, Biografia FROM Autores";
            
            using var connection = _context.CreateConnection();
            return await connection.QueryAsync<AutorDto>(sql);
        }

        public async Task<AutorDto> GetByIdAsync(int id)
        {
            const string sql = @"SELECT Id, Nome, Biografia FROM Autores WHERE Id = @Id";
            
            using var connection = _context.CreateConnection();
            return await connection.QueryFirstOrDefaultAsync<AutorDto>(sql, new { Id = id });
        }

        public async Task<AutorDetalhesDto> GetDetalhesAsync(int id)
        {
            const string sql = @"
                SELECT a.Id, a.Nome, a.Biografia,
                       l.Id as LivroId, l.Titulo, l.Ano, l.GeneroId, g.Nome as GeneroNome,
                       l.AutorId, a.Nome as AutorNome
                FROM Autores a
                LEFT JOIN Livros l ON a.Id = l.AutorId
                LEFT JOIN Generos g ON l.GeneroId = g.Id
                WHERE a.Id = @Id";
            
            using var connection = _context.CreateConnection();
            
            var autorDicionario = new Dictionary<int, AutorDetalhesDto>();
            
            var resultado = await connection.QueryAsync<AutorDetalhesDto, LivroDto, AutorDetalhesDto>(
                sql,
                (autor, livro) => {
                    if (!autorDicionario.TryGetValue(autor.Id, out var autorEntry))
                    {
                        autorEntry = autor;
                        autorEntry.Livros = new List<LivroDto>();
                        autorDicionario.Add(autor.Id, autorEntry);
                    }
                    
                    if (livro != null && livro.Id != 0)
                    {
                        autorEntry.Livros.Add(livro);
                    }
                    
                    return autorEntry;
                },
                new { Id = id },
                splitOn: "LivroId"
            );
            
            return autorDicionario.Values.FirstOrDefault();
        }
    }
}
