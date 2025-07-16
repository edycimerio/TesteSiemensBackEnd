using MediatR;
using SistemaLivros.Application.DTOs;
using System.Collections.Generic;

namespace SistemaLivros.Application.Queries.Livros.GetAllLivros
{
    public class GetAllLivrosQuery : IRequest<IEnumerable<LivroDto>>
    {
        // Não precisa de parâmetros, pois é uma consulta para todos os livros
    }
}
