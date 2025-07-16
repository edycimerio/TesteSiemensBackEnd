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
            const string sql = @"
                SELECT g.Id, g.Nome, g.Descricao,
                       l.Id as LivroId, l.Titulo, l.Ano, l.GeneroId, g.Nome as GeneroNome,
                       l.AutorId, a.Nome as AutorNome
                FROM Generos g
                LEFT JOIN Livros l ON g.Id = l.GeneroId
                LEFT JOIN Autores a ON l.AutorId = a.Id
                WHERE g.Id = @Id";
            
            using var connection = _context.CreateConnection();
            
            var generoDicionario = new Dictionary<int, GeneroDetalhesDto>();
            
            var resultado = await connection.QueryAsync<GeneroDetalhesDto, LivroDto, GeneroDetalhesDto>(
                sql,
                (genero, livro) => {
                    if (!generoDicionario.TryGetValue(genero.Id, out var generoEntry))
                    {
                        generoEntry = genero;
                        generoEntry.Livros = new List<LivroDto>();
                        generoDicionario.Add(genero.Id, generoEntry);
                    }
                    
                    if (livro != null && livro.Id != 0)
                    {
                        generoEntry.Livros.Add(livro);
                    }
                    
                    return generoEntry;
                },
                new { Id = id },
                splitOn: "LivroId"
            );
            
            return generoDicionario.Values.FirstOrDefault();
        }
    }
}
