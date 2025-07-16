using MediatR;
using SistemaLivros.Application.DTOs;
using System.Collections.Generic;

namespace SistemaLivros.Application.Queries.Autores.GetAllAutores
{
    public class GetAllAutoresQuery : IRequest<IEnumerable<AutorDto>>
    {
        // Não precisa de parâmetros, pois é uma consulta para todos os autores
    }
}
