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
        // 1. Buscar os livros básicos
        const string sql = @"
        SELECT l.Id, l.Titulo, l.Ano
        FROM Livros l";
    
        using var connection = _context.CreateConnection();
        var livros = await connection.QueryAsync<LivroDto>(sql);
    
        // Converter para lista para poder modificar
        var livrosList = livros.ToList();
    
        // 2. Para cada livro, buscar o autor e os gêneros
        foreach (var livro in livrosList)
        {
            // 2.1 Buscar o autor do livro
            const string autorSql = @"
            SELECT a.Id, a.Nome, a.Biografia, a.DataNascimento
            FROM Autores a
            INNER JOIN Livros l ON l.AutorId = a.Id
            WHERE l.Id = @LivroId";
            
            var autor = await connection.QueryFirstOrDefaultAsync<AutorDto>(autorSql, new { LivroId = livro.Id });
            livro.Autor = autor;
            
            // 2.2 Buscar todos os gêneros associados a este livro
            const string generosSql = @"
            SELECT g.Id, g.Nome, g.Descricao
            FROM Generos g
            INNER JOIN LivroGeneros lg ON g.Id = lg.GeneroId
            WHERE lg.LivroId = @LivroId";
        
            var generos = await connection.QueryAsync<GeneroSimplificadoDto>(generosSql, new { LivroId = livro.Id });
            livro.Generos = generos.ToList();
        }
    
        return livrosList;
    }

        public async Task<LivroDto> GetByIdAsync(int id)
        {
            // 1. Buscar o livro básico
            const string sql = @"
            SELECT l.Id, l.Titulo, l.Ano
            FROM Livros l
            WHERE l.Id = @Id";
            
            using var connection = _context.CreateConnection();
            var livro = await connection.QueryFirstOrDefaultAsync<LivroDto>(sql, new { Id = id });
            
            if (livro != null)
            {
                // 2. Buscar o autor do livro
                const string autorSql = @"
                SELECT a.Id, a.Nome, a.Biografia, a.DataNascimento
                FROM Autores a
                INNER JOIN Livros l ON l.AutorId = a.Id
                WHERE l.Id = @LivroId";
                
                var autor = await connection.QueryFirstOrDefaultAsync<AutorDto>(autorSql, new { LivroId = id });
                livro.Autor = autor;
                
                // 3. Buscar todos os gêneros associados a este livro
                const string generosSql = @"
                SELECT g.Id, g.Nome, g.Descricao
                FROM Generos g
                INNER JOIN LivroGeneros lg ON g.Id = lg.GeneroId
                WHERE lg.LivroId = @LivroId";
                
                var generos = await connection.QueryAsync<GeneroSimplificadoDto>(generosSql, new { LivroId = id });
                livro.Generos = generos.ToList();
            }
            
            return livro;
        }

        public async Task<IEnumerable<LivroDto>> GetByAutorIdAsync(int autorId)
        {
            // 1. Buscar os livros básicos do autor
            const string sql = @"
            SELECT l.Id, l.Titulo, l.Ano
            FROM Livros l
            WHERE l.AutorId = @AutorId";
        
            using var connection = _context.CreateConnection();
            var livros = await connection.QueryAsync<LivroDto>(sql, new { AutorId = autorId });
        
            // Converter para lista para poder modificar
            var livrosList = livros.ToList();
            
            // 2. Buscar o autor uma única vez
            const string autorSql = @"
            SELECT a.Id, a.Nome, a.Biografia, a.DataNascimento
            FROM Autores a
            WHERE a.Id = @AutorId";
            
            var autor = await connection.QueryFirstOrDefaultAsync<AutorDto>(autorSql, new { AutorId = autorId });
        
            // 3. Para cada livro, buscar seus gêneros e associar o autor
            foreach (var livro in livrosList)
            {
                // Associar o mesmo autor a todos os livros
                livro.Autor = autor;
                
                // Buscar todos os gêneros associados a este livro
                const string generosSql = @"
                SELECT g.Id, g.Nome, g.Descricao
                FROM Generos g
                INNER JOIN LivroGeneros lg ON g.Id = lg.GeneroId
                WHERE lg.LivroId = @LivroId";
            
                var generos = await connection.QueryAsync<GeneroSimplificadoDto>(generosSql, new { LivroId = livro.Id });
                livro.Generos = generos.ToList();
            }
        
            return livrosList;
        }

        public async Task<IEnumerable<LivroDto>> GetByGeneroIdAsync(int generoId)
        {
            // 1. Buscar os livros básicos que têm o gênero especificado
            const string sql = @"
            SELECT DISTINCT l.Id, l.Titulo, l.Ano
            FROM Livros l
            INNER JOIN LivroGeneros lg ON l.Id = lg.LivroId
            WHERE lg.GeneroId = @GeneroId";
        
            using var connection = _context.CreateConnection();
            var livros = await connection.QueryAsync<LivroDto>(sql, new { GeneroId = generoId });
        
            // Converter para lista para poder modificar
            var livrosList = livros.ToList();
        
            // 2. Para cada livro, buscar o autor e os gêneros
            foreach (var livro in livrosList)
            {
                // 2.1 Buscar o autor do livro
                const string autorSql = @"
                SELECT a.Id, a.Nome, a.Biografia, a.DataNascimento
                FROM Autores a
                INNER JOIN Livros l ON l.AutorId = a.Id
                WHERE l.Id = @LivroId";
            
                var autor = await connection.QueryFirstOrDefaultAsync<AutorDto>(autorSql, new { LivroId = livro.Id });
                livro.Autor = autor;
            
                // 2.2 Buscar todos os gêneros associados a este livro
                const string generosSql = @"
                SELECT g.Id, g.Nome, g.Descricao
                FROM Generos g
                INNER JOIN LivroGeneros lg ON g.Id = lg.GeneroId
                WHERE lg.LivroId = @LivroId";
        
                var generos = await connection.QueryAsync<GeneroSimplificadoDto>(generosSql, new { LivroId = livro.Id });
                livro.Generos = generos.ToList();
            }
        
            return livrosList;
        }

        public async Task<IEnumerable<LivroDto>> SearchAsync(string termo)
    {
        // 1. Buscar os livros básicos que correspondem ao termo de busca
        const string sql = @"
        SELECT DISTINCT l.Id, l.Titulo, l.Ano
        FROM Livros l
        INNER JOIN Autores a ON l.AutorId = a.Id
        LEFT JOIN LivroGeneros lg ON l.Id = lg.LivroId
        LEFT JOIN Generos g ON lg.GeneroId = g.Id
        WHERE l.Titulo LIKE @Termo OR a.Nome LIKE @Termo OR g.Nome LIKE @Termo";
    
        using var connection = _context.CreateConnection();
        var livros = await connection.QueryAsync<LivroDto>(sql, new { Termo = $"%{termo}%" });
    
        // Converter para lista para poder modificar
        var livrosList = livros.ToList();
    
        // 2. Para cada livro, buscar o autor e os gêneros
        foreach (var livro in livrosList)
        {
            // 2.1 Buscar o autor do livro
            const string autorSql = @"
            SELECT a.Id, a.Nome, a.Biografia, a.DataNascimento
            FROM Autores a
            INNER JOIN Livros l ON l.AutorId = a.Id
            WHERE l.Id = @LivroId";
            
            var autor = await connection.QueryFirstOrDefaultAsync<AutorDto>(autorSql, new { LivroId = livro.Id });
            livro.Autor = autor;
            
            // 2.2 Buscar todos os gêneros associados a este livro
            const string generosSql = @"
            SELECT g.Id, g.Nome, g.Descricao
            FROM Generos g
            INNER JOIN LivroGeneros lg ON g.Id = lg.GeneroId
            WHERE lg.LivroId = @LivroId";
        
            var generos = await connection.QueryAsync<GeneroSimplificadoDto>(generosSql, new { LivroId = livro.Id });
            livro.Generos = generos.ToList();
        }
    
        return livrosList;
    }

        public async Task<LivroDetalhesDto> GetDetalhesAsync(int id)
        {
            // 1. Buscar o livro básico
            const string livroSql = @"SELECT l.Id, l.Titulo, l.Ano 
                            FROM Livros l WHERE l.Id = @Id";
            
            using var connection = _context.CreateConnection();
            var livro = await connection.QueryFirstOrDefaultAsync<LivroDetalhesDto>(livroSql, new { Id = id });
            
            if (livro == null)
                return null;
                
            // 2. Buscar detalhes do autor associado
            const string autorSql = @"SELECT a.Id, a.Nome, a.Biografia, a.DataNascimento 
                            FROM Autores a 
                            INNER JOIN Livros l ON l.AutorId = a.Id
                            WHERE l.Id = @LivroId";
            
            var autor = await connection.QueryFirstOrDefaultAsync<AutorDto>(autorSql, new { LivroId = id });
            
            // 3. Buscar gêneros associados ao livro
            const string generosSql = @"SELECT g.Id, g.Nome, g.Descricao
                             FROM Generos g
                             INNER JOIN LivroGeneros lg ON g.Id = lg.GeneroId
                             WHERE lg.LivroId = @LivroId";
            
            var generos = await connection.QueryAsync<GeneroSimplificadoDto>(generosSql, new { LivroId = id });
            
            // 4. Montar o objeto de retorno
            livro.Autor = autor;
            livro.Generos = generos.ToList();
            
            return livro;
        }
    }
}
