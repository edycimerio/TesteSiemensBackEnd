using Dapper;
using SistemaLivros.Application.DTOs;
using SistemaLivros.Application.Interfaces;
using SistemaLivros.Infrastructure.Data;
using System;
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

        public async Task<LivroDetalhesDto> GetDetalhesAsync(int id)
        {
            // 1. Buscar o livro básico
            const string livroSql = @"SELECT l.Id, l.Titulo, l.Ano, l.GeneroId, l.AutorId 
                            FROM Livros l WHERE l.Id = @Id";
            
            using var connection = _context.CreateConnection();
            var livro = await connection.QueryFirstOrDefaultAsync<LivroDetalhesDto>(livroSql, new { Id = id });
            
            if (livro == null)
                return null;
                
            // 2. Buscar detalhes do autor associado
            const string autorSql = @"SELECT a.Id, a.Nome as AutorNome, a.Biografia as AutorBiografia 
                                    FROM Autores a WHERE a.Id = @AutorId";
                
            var autorDetalhes = await connection.QueryFirstOrDefaultAsync<dynamic>(autorSql, new { AutorId = livro.AutorId });
            
            // 3. Buscar detalhes do gênero associado
            const string generoSql = @"SELECT g.Id, g.Nome as GeneroNome, g.Descricao as GeneroDescricao 
                                     FROM Generos g WHERE g.Id = @GeneroId";
                
            var generoDetalhes = await connection.QueryFirstOrDefaultAsync<dynamic>(generoSql, new { GeneroId = livro.GeneroId });
            
            // 4. Preencher os detalhes do livro com as informações do autor e gênero
            if (autorDetalhes != null)
            {
                livro.AutorNome = autorDetalhes.AutorNome;
                livro.AutorBiografia = autorDetalhes.AutorBiografia;
            }
            
            if (generoDetalhes != null)
            {
                livro.GeneroNome = generoDetalhes.GeneroNome;
                livro.GeneroDescricao = generoDetalhes.GeneroDescricao;
            }
            
            return livro;
        }
    }
}
