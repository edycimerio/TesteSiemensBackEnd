using AutoMapper;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using SistemaLivros.API.Models.Request.Autores;
using SistemaLivros.API.Models.Response.Autores;
using SistemaLivros.Application.Commands.Autores;
using SistemaLivros.Application.Common;
using SistemaLivros.Application.Queries.Autores.GetAllAutores;
using SistemaLivros.Application.Queries.Autores.GetAutorById;
using SistemaLivros.Application.Queries.Autores.GetAutorDetalhes;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SistemaLivros.API.Controllers
{
    [ApiController]
    [Route("api/v1/[controller]")]
    public class AutoresController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly IMapper _mapper;

        public AutoresController(IMediator mediator, IMapper mapper)
        {
            _mediator = mediator;
            _mapper = mapper;
        }

        /// <summary>
        /// Obtém todos os autores com paginação
        /// </summary>
        /// <param name="pageNumber">Número da página (padrão: 1)</param>
        /// <param name="pageSize">Tamanho da página (padrão: 10)</param>
        /// <returns>Lista paginada de autores</returns>
        [HttpGet]
        [ProducesResponseType(typeof(PagedResult<AutorResponse>), 200)]
        public async Task<IActionResult> GetAll([FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10)
        {
            var query = new GetAllAutoresQuery(pageNumber, pageSize);
            var autores = await _mediator.Send(query);
            var response = _mapper.Map<PagedResult<AutorResponse>>(autores);
            return Ok(response);
        }

        /// <summary>
        /// Obtém um autor pelo ID
        /// </summary>
        /// <param name="id">ID do autor</param>
        /// <returns>Autor encontrado</returns>
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(AutorResponse), 200)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> GetById(int id)
        {
            var query = new GetAutorByIdQuery(id);
            var autor = await _mediator.Send(query);
            if (autor == null)
                return NotFound();

            var response = _mapper.Map<AutorResponse>(autor);
            return Ok(response);
        }

        /// <summary>
        /// Obtém detalhes de um autor, incluindo seus livros
        /// </summary>
        /// <param name="id">ID do autor</param>
        /// <returns>Detalhes do autor</returns>
        [HttpGet("{id}/detalhes")]
        [ProducesResponseType(typeof(AutorDetalhesResponse), 200)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> GetDetalhes(int id)
        {
            var query = new GetAutorDetalhesQuery(id);
            var autor = await _mediator.Send(query);
            if (autor == null)
                return NotFound();

            var response = _mapper.Map<AutorDetalhesResponse>(autor);
            return Ok(response);
        }

        /// <summary>
        /// Cria um novo autor
        /// </summary>
        /// <param name="request">Dados do autor</param>
        /// <returns>Autor criado</returns>
        [HttpPost]
        [ProducesResponseType(typeof(AutorResponse), 201)]
        [ProducesResponseType(400)]
        public async Task<IActionResult> Create([FromBody] AutorRequest request)
        {
            var command = _mapper.Map<CreateAutorCommand>(request);
            var id = await _mediator.Send(command);

            // Busca o autor criado para retornar na resposta
            var query = new GetAutorByIdQuery(id);
            var autor = await _mediator.Send(query);
            var response = _mapper.Map<AutorResponse>(autor);

            return CreatedAtAction(nameof(GetById), new { id }, response);
        }

        /// <summary>
        /// Atualiza um autor existente
        /// </summary>
        /// <param name="id">ID do autor</param>
        /// <param name="request">Dados atualizados do autor</param>
        /// <returns>Resultado da operação</returns>
        [HttpPut("{id}")]
        [ProducesResponseType(204)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> Update(int id, [FromBody] AutorRequest request)
        {
            var command = _mapper.Map<UpdateAutorCommand>(request);
            command.Id = id; // Define o ID do comando a partir da rota
            
            var result = await _mediator.Send(command);

            if (!result)
                return NotFound();

            return NoContent();
        }

        /// <summary>
        /// Remove um autor
        /// </summary>
        /// <param name="id">ID do autor</param>
        /// <returns>Resultado da operação</returns>
        [HttpDelete("{id}")]
        [ProducesResponseType(204)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> Delete(int id)
        {
            var command = new DeleteAutorCommand(id);
            var result = await _mediator.Send(command);

            if (!result)
                return NotFound();

            return NoContent();
        }
    }
}
