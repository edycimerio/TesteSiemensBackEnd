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
    public class GeneroQueries : IGeneroQueries
    {
        private readonly DapperContext _context;

        public GeneroQueries(DapperContext context)
        {
            _context = context;
        }

        public async Task<PagedResult<GeneroDto>> GetAllAsync(PaginationParams paginationParams)
        {
            // 1. Buscar o total de registros para paginação
            const string countSql = @"SELECT COUNT(*) FROM Generos";
            
            using var connection = _context.CreateConnection();
            var totalCount = await connection.ExecuteScalarAsync<int>(countSql);
            
            // 2. Buscar os gêneros com paginação
            var sql = @"
                SELECT Id, Nome, Descricao 
                FROM Generos
                ORDER BY Id
                LIMIT @PageSize OFFSET @Offset";
            
            var offset = (paginationParams.PageNumber - 1) * paginationParams.PageSize;
            var generos = await connection.QueryAsync<GeneroDto>(sql, 
                new { Offset = offset, PageSize = paginationParams.PageSize });
            
            // 3. Retornar resultado paginado
            return new PagedResult<GeneroDto>(generos.ToList(), totalCount, paginationParams.PageNumber, paginationParams.PageSize);
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
                
            // Busca os livros associados a este gênero através da tabela de relacionamento LivroGenero
            const string livrosSql = @"
                SELECT l.Id, l.Titulo, l.Ano, l.AutorId, a.Nome as AutorNome
                FROM Livros l
                INNER JOIN LivroGeneros lg ON l.Id = lg.LivroId
                LEFT JOIN Autores a ON l.AutorId = a.Id
                WHERE lg.GeneroId = @GeneroId";
                
            var livros = await connection.QueryAsync<LivroDto>(livrosSql, new { GeneroId = id });
            
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
            
            // Associa os livros ao gênero
            genero.Livros = livrosList;
            
            return genero;
        }
    }
}
