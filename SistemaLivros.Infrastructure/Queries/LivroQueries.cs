using Dapper;
using SistemaLivros.Application.Common;
using SistemaLivros.Application.DTOs;
using SistemaLivros.Application.Interfaces;
using SistemaLivros.Infrastructure.Data;
using System;
using System.Collections.Generic;
using System.Linq;
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

        public async Task<PagedResult<LivroDto>> GetAllAsync(PaginationParams paginationParams)
    {
        // 1. Buscar o total de registros para paginação
        const string countSql = @"SELECT COUNT(*) FROM Livros";
        
        using var connection = _context.CreateConnection();
        var totalCount = await connection.ExecuteScalarAsync<int>(countSql);
        
        // 2. Buscar os livros básicos com paginação
        var sql = @"
        SELECT l.Id, l.Titulo, l.Ano
        FROM Livros l
        ORDER BY l.Id
        OFFSET @Offset ROWS FETCH NEXT @PageSize ROWS ONLY";
    
        var offset = (paginationParams.PageNumber - 1) * paginationParams.PageSize;
        var livros = await connection.QueryAsync<LivroDto>(sql, new { Offset = offset, PageSize = paginationParams.PageSize });
    
        // Converter para lista para poder modificar
        var livrosList = livros.ToList();
    
        // 3. Para cada livro, buscar o autor e os gêneros
        foreach (var livro in livrosList)
        {
            // 3.1 Buscar o autor do livro
            const string autorSql = @"
            SELECT a.Id, a.Nome, a.Biografia, a.DataNascimento
            FROM Autores a
            INNER JOIN Livros l ON l.AutorId = a.Id
            WHERE l.Id = @LivroId";
            
            var autor = await connection.QueryFirstOrDefaultAsync<AutorDto>(autorSql, new { LivroId = livro.Id });
            livro.Autor = autor;
            
            // 3.2 Buscar todos os gêneros associados a este livro
            const string generosSql = @"
            SELECT g.Id, g.Nome, g.Descricao
            FROM Generos g
            INNER JOIN LivroGeneros lg ON g.Id = lg.GeneroId
            WHERE lg.LivroId = @LivroId";
        
            var generos = await connection.QueryAsync<GeneroSimplificadoDto>(generosSql, new { LivroId = livro.Id });
            livro.Generos = generos.ToList();
        }
    
        // 4. Retornar resultado paginado
        return new PagedResult<LivroDto>(livrosList, totalCount, paginationParams.PageNumber, paginationParams.PageSize);
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

        public async Task<PagedResult<LivroDto>> GetByAutorIdAsync(int autorId, PaginationParams paginationParams)
        {
            // 1. Buscar o total de registros para paginação
            const string countSql = @"SELECT COUNT(*) FROM Livros WHERE AutorId = @AutorId";
            
            using var connection = _context.CreateConnection();
            var totalCount = await connection.ExecuteScalarAsync<int>(countSql, new { AutorId = autorId });
            
            // 2. Buscar os livros básicos do autor com paginação
            var sql = @"
            SELECT l.Id, l.Titulo, l.Ano
            FROM Livros l
            WHERE l.AutorId = @AutorId
            ORDER BY l.Id
            OFFSET @Offset ROWS FETCH NEXT @PageSize ROWS ONLY";
        
            var offset = (paginationParams.PageNumber - 1) * paginationParams.PageSize;
            var livros = await connection.QueryAsync<LivroDto>(sql, 
                new { AutorId = autorId, Offset = offset, PageSize = paginationParams.PageSize });
        
            // Converter para lista para poder modificar
            var livrosList = livros.ToList();
            
            // 3. Buscar o autor uma única vez
            const string autorSql = @"
            SELECT a.Id, a.Nome, a.Biografia, a.DataNascimento
            FROM Autores a
            WHERE a.Id = @AutorId";
            
            var autor = await connection.QueryFirstOrDefaultAsync<AutorDto>(autorSql, new { AutorId = autorId });
            
            // 4. Para cada livro, atribuir o autor e buscar os gêneros
            foreach (var livro in livrosList)
            {
                // 4.1 Atribuir o autor
                livro.Autor = autor;
                
                // 4.2 Buscar todos os gêneros associados a este livro
                const string generosSql = @"
                SELECT g.Id, g.Nome, g.Descricao
                FROM Generos g
                INNER JOIN LivroGeneros lg ON g.Id = lg.GeneroId
                WHERE lg.LivroId = @LivroId";
            
                var generos = await connection.QueryAsync<GeneroSimplificadoDto>(generosSql, new { LivroId = livro.Id });
                livro.Generos = generos.ToList();
            }
        
            // 5. Retornar resultado paginado
            return new PagedResult<LivroDto>(livrosList, totalCount, paginationParams.PageNumber, paginationParams.PageSize);
        }

        public async Task<PagedResult<LivroDto>> GetByGeneroIdAsync(int generoId, PaginationParams paginationParams)
        {
            // 1. Buscar o total de registros para paginação
            const string countSql = @"
            SELECT COUNT(DISTINCT l.Id) 
            FROM Livros l
            INNER JOIN LivroGeneros lg ON l.Id = lg.LivroId
            WHERE lg.GeneroId = @GeneroId";
            
            using var connection = _context.CreateConnection();
            var totalCount = await connection.ExecuteScalarAsync<int>(countSql, new { GeneroId = generoId });
            
            // 2. Buscar os livros básicos do gênero com paginação
            var sql = @"
            SELECT l.Id, l.Titulo, l.Ano
            FROM Livros l
            INNER JOIN LivroGeneros lg ON l.Id = lg.LivroId
            WHERE lg.GeneroId = @GeneroId
            ORDER BY l.Id
            OFFSET @Offset ROWS FETCH NEXT @PageSize ROWS ONLY";
        
            var offset = (paginationParams.PageNumber - 1) * paginationParams.PageSize;
            var livros = await connection.QueryAsync<LivroDto>(sql, 
                new { GeneroId = generoId, Offset = offset, PageSize = paginationParams.PageSize });
        
            // Converter para lista para poder modificar
            var livrosList = livros.ToList();
        
            // 3. Para cada livro, buscar o autor e os gêneros
            foreach (var livro in livrosList)
            {
                // 3.1 Buscar o autor do livro
                const string autorSql = @"
                SELECT a.Id, a.Nome, a.Biografia, a.DataNascimento
                FROM Autores a
                INNER JOIN Livros l ON l.AutorId = a.Id
                WHERE l.Id = @LivroId";
                
                var autor = await connection.QueryFirstOrDefaultAsync<AutorDto>(autorSql, new { LivroId = livro.Id });
                livro.Autor = autor;
                
                // 3.2 Buscar todos os gêneros associados a este livro
                const string generosSql = @"
                SELECT g.Id, g.Nome, g.Descricao
                FROM Generos g
                INNER JOIN LivroGeneros lg ON g.Id = lg.GeneroId
                WHERE lg.LivroId = @LivroId";
            
                var generos = await connection.QueryAsync<GeneroSimplificadoDto>(generosSql, new { LivroId = livro.Id });
                livro.Generos = generos.ToList();
            }
        
            // 4. Retornar resultado paginado
            return new PagedResult<LivroDto>(livrosList, totalCount, paginationParams.PageNumber, paginationParams.PageSize);
        }

        public async Task<PagedResult<LivroDto>> SearchAsync(string termo, PaginationParams paginationParams)
    {
        // 1. Buscar o total de registros para paginação
        var countSql = @"
        SELECT COUNT(DISTINCT l.Id)
        FROM Livros l
        INNER JOIN Autores a ON l.AutorId = a.Id
        LEFT JOIN LivroGeneros lg ON l.Id = lg.LivroId
        LEFT JOIN Generos g ON lg.GeneroId = g.Id
        WHERE l.Titulo LIKE @Termo OR a.Nome LIKE @Termo OR g.Nome LIKE @Termo";
        
        using var connection = _context.CreateConnection();
        var totalCount = await connection.ExecuteScalarAsync<int>(countSql, new { Termo = $"%{termo}%" });
        
        // 2. Buscar os livros básicos que correspondem ao termo de busca com paginação
        var sql = @"
        SELECT DISTINCT l.Id, l.Titulo, l.Ano
        FROM Livros l
        INNER JOIN Autores a ON l.AutorId = a.Id
        LEFT JOIN LivroGeneros lg ON l.Id = lg.LivroId
        LEFT JOIN Generos g ON lg.GeneroId = g.Id
        WHERE l.Titulo LIKE @Termo OR a.Nome LIKE @Termo OR g.Nome LIKE @Termo
        ORDER BY l.Id
        OFFSET @Offset ROWS FETCH NEXT @PageSize ROWS ONLY";
    
        var offset = (paginationParams.PageNumber - 1) * paginationParams.PageSize;
        var livros = await connection.QueryAsync<LivroDto>(sql, 
            new { Termo = $"%{termo}%", Offset = offset, PageSize = paginationParams.PageSize });
    
        // Converter para lista para poder modificar
        var livrosList = livros.ToList();
    
        // 3. Para cada livro, buscar o autor e os gêneros
        foreach (var livro in livrosList)
        {
            // 3.1 Buscar o autor do livro
            const string autorSql = @"
            SELECT a.Id, a.Nome, a.Biografia, a.DataNascimento
            FROM Autores a
            INNER JOIN Livros l ON l.AutorId = a.Id
            WHERE l.Id = @LivroId";
            
            var autor = await connection.QueryFirstOrDefaultAsync<AutorDto>(autorSql, new { LivroId = livro.Id });
            livro.Autor = autor;
            
            // 3.2 Buscar todos os gêneros associados a este livro
            const string generosSql = @"
            SELECT g.Id, g.Nome, g.Descricao
            FROM Generos g
            INNER JOIN LivroGeneros lg ON g.Id = lg.GeneroId
            WHERE lg.LivroId = @LivroId";
        
            var generos = await connection.QueryAsync<GeneroSimplificadoDto>(generosSql, new { LivroId = livro.Id });
            livro.Generos = generos.ToList();
        }
    
        // 4. Retornar resultado paginado
        return new PagedResult<LivroDto>(livrosList, totalCount, paginationParams.PageNumber, paginationParams.PageSize);
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
