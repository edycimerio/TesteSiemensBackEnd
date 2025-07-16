using MediatR;
using Microsoft.AspNetCore.Mvc;
using SistemaLivros.Application.Commands.Autores;
using SistemaLivros.Application.DTOs;
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

        public AutoresController(IMediator mediator)
        {
            _mediator = mediator;
        }

        /// <summary>
        /// Obtém todos os autores
        /// </summary>
        /// <returns>Lista de autores</returns>
        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<AutorDto>), 200)]
        public async Task<IActionResult> GetAll()
        {
            var query = new GetAllAutoresQuery();
            var autores = await _mediator.Send(query);
            return Ok(autores);
        }

        /// <summary>
        /// Obtém um autor pelo ID
        /// </summary>
        /// <param name="id">ID do autor</param>
        /// <returns>Autor encontrado</returns>
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(AutorDto), 200)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> GetById(int id)
        {
            var query = new GetAutorByIdQuery(id);
            var autor = await _mediator.Send(query);
            if (autor == null)
                return NotFound();

            return Ok(autor);
        }

        /// <summary>
        /// Obtém detalhes de um autor, incluindo seus livros
        /// </summary>
        /// <param name="id">ID do autor</param>
        /// <returns>Detalhes do autor</returns>
        [HttpGet("{id}/detalhes")]
        [ProducesResponseType(typeof(AutorDetalhesDto), 200)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> GetDetalhes(int id)
        {
            var query = new GetAutorDetalhesQuery(id);
            var autor = await _mediator.Send(query);
            if (autor == null)
                return NotFound();

            return Ok(autor);
        }

        /// <summary>
        /// Cria um novo autor
        /// </summary>
        /// <param name="autorDto">Dados do autor</param>
        /// <returns>Autor criado</returns>
        [HttpPost]
        [ProducesResponseType(typeof(AutorDto), 201)]
        [ProducesResponseType(400)]
        public async Task<IActionResult> Create([FromBody] AutorDto autorDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var command = new CreateAutorCommand(autorDto.Nome, autorDto.Biografia);
            var id = await _mediator.Send(command);

            autorDto.Id = id;
            return CreatedAtAction(nameof(GetById), new { id }, autorDto);
        }

        /// <summary>
        /// Atualiza um autor existente
        /// </summary>
        /// <param name="id">ID do autor</param>
        /// <param name="autorDto">Dados atualizados do autor</param>
        /// <returns>Resultado da operação</returns>
        [HttpPut("{id}")]
        [ProducesResponseType(204)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> Update(int id, [FromBody] AutorDto autorDto)
        {
            if (id != autorDto.Id)
                return BadRequest("O ID do autor na URL não corresponde ao ID no corpo da requisição.");

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var command = new UpdateAutorCommand(id, autorDto.Nome, autorDto.Biografia);
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
