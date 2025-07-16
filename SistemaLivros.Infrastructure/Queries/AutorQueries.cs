using Dapper;
using SistemaLivros.Application.Common;
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

        public async Task<PagedResult<AutorDto>> GetAllAsync(PaginationParams paginationParams)
        {
            // 1. Buscar o total de registros para paginação
            const string countSql = @"SELECT COUNT(*) FROM Autores";
            
            using var connection = _context.CreateConnection();
            var totalCount = await connection.ExecuteScalarAsync<int>(countSql);
            
            // 2. Buscar os autores com paginação
            var sql = @"
                SELECT Id, Nome, Biografia, DataNascimento 
                FROM Autores
                ORDER BY Id
                OFFSET @Offset ROWS FETCH NEXT @PageSize ROWS ONLY";
            
            var offset = (paginationParams.PageNumber - 1) * paginationParams.PageSize;
            var autores = await connection.QueryAsync<AutorDto>(sql, 
                new { Offset = offset, PageSize = paginationParams.PageSize });
            
            // 3. Retornar resultado paginado
            return new PagedResult<AutorDto>(autores.ToList(), totalCount, paginationParams.PageNumber, paginationParams.PageSize);
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
                SELECT l.Id, l.Titulo, l.Ano, l.AutorId, a.Nome as AutorNome
                FROM Livros l
                INNER JOIN Autores a ON l.AutorId = a.Id
                WHERE l.AutorId = @AutorId";
                
            var livros = await connection.QueryAsync<LivroDto>(livrosSql, new { AutorId = id });
            
            // Converter para lista para poder modificar
            var livrosList = livros.ToList();
            
            // Para cada livro, buscar todos os seus gêneros
            foreach (var livro in livrosList)
            {
                // Buscar todos os gêneros associados a este livro
                const string generosSql = @"
                    SELECT g.Id, g.Nome, g.Descricao
                    FROM Generos g
                    INNER JOIN LivroGeneros lg ON g.Id = lg.GeneroId
                    WHERE lg.LivroId = @LivroId";
                
                var generos = await connection.QueryAsync<GeneroSimplificadoDto>(generosSql, new { LivroId = livro.Id });
                livro.Generos = generos.ToList();
                
                // Não precisamos mais definir o GeneroNome, pois foi removido do DTO
            }
            
            // 3. Associar os livros ao autor
            autor.Livros = livrosList;
            
            return autor;
        }
    }
}
