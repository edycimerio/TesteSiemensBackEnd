using MediatR;
using SistemaLivros.Application.DTOs;
using System.Collections.Generic;

namespace SistemaLivros.Application.Queries.Generos.GetAllGeneros
{
    public class GetAllGenerosQuery : IRequest<IEnumerable<GeneroDto>>
    {
        // Não precisa de parâmetros, pois é uma consulta para todos os gêneros
    }
}
