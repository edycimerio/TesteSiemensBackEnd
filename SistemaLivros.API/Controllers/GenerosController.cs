using MediatR;
using Microsoft.AspNetCore.Mvc;
using SistemaLivros.Application.Commands.Generos;

using SistemaLivros.Application.DTOs;
using SistemaLivros.Application.Queries.Generos.GetAllGeneros;
using SistemaLivros.Application.Queries.Generos.GetGeneroById;
using SistemaLivros.Application.Queries.Generos.GetGeneroDetalhes;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SistemaLivros.API.Controllers
{
    [ApiController]
    [Route("api/v1/[controller]")]
    public class GenerosController : ControllerBase
    {
        private readonly IMediator _mediator;

        public GenerosController(IMediator mediator)
        {
            _mediator = mediator;
        }

        /// <summary>
        /// Obtém todos os gêneros
        /// </summary>
        /// <returns>Lista de gêneros</returns>
        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<GeneroDto>), 200)]
        public async Task<IActionResult> GetAll()
        {
            var query = new GetAllGenerosQuery();
            var generos = await _mediator.Send(query);
            return Ok(generos);
        }

        /// <summary>
        /// Obtém um gênero pelo ID
        /// </summary>
        /// <param name="id">ID do gênero</param>
        /// <returns>Gênero encontrado</returns>
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(GeneroDto), 200)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> GetById(int id)
        {
            var query = new GetGeneroByIdQuery(id);
            var genero = await _mediator.Send(query);
            if (genero == null)
                return NotFound();

            return Ok(genero);
        }

        /// <summary>
        /// Obtém detalhes de um gênero, incluindo seus livros
        /// </summary>
        /// <param name="id">ID do gênero</param>
        /// <returns>Detalhes do gênero</returns>
        [HttpGet("{id}/detalhes")]
        [ProducesResponseType(typeof(GeneroDetalhesDto), 200)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> GetDetalhes(int id)
        {
            var query = new GetGeneroDetalhesQuery(id);
            var genero = await _mediator.Send(query);
            if (genero == null)
                return NotFound();

            return Ok(genero);
        }

        /// <summary>
        /// Cria um novo gênero
        /// </summary>
        /// <param name="generoDto">Dados do gênero</param>
        /// <returns>Gênero criado</returns>
        [HttpPost]
        [ProducesResponseType(typeof(GeneroDto), 201)]
        [ProducesResponseType(400)]
        public async Task<IActionResult> Create([FromBody] GeneroDto generoDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var command = new CreateGeneroCommand(generoDto.Nome, generoDto.Descricao);
            var id = await _mediator.Send(command);

            generoDto.Id = id;
            return CreatedAtAction(nameof(GetById), new { id }, generoDto);
        }

        /// <summary>
        /// Atualiza um gênero existente
        /// </summary>
        /// <param name="id">ID do gênero</param>
        /// <param name="generoDto">Dados atualizados do gênero</param>
        /// <returns>Resultado da operação</returns>
        [HttpPut("{id}")]
        [ProducesResponseType(204)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> Update(int id, [FromBody] GeneroDto generoDto)
        {
            if (id != generoDto.Id)
                return BadRequest("O ID do gênero na URL não corresponde ao ID no corpo da requisição.");

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var command = new UpdateGeneroCommand(id, generoDto.Nome, generoDto.Descricao);
            var result = await _mediator.Send(command);

            if (!result)
                return NotFound();

            return NoContent();
        }

        /// <summary>
        /// Remove um gênero
        /// </summary>
        /// <param name="id">ID do gênero</param>
        /// <returns>Resultado da operação</returns>
        [HttpDelete("{id}")]
        [ProducesResponseType(204)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> Delete(int id)
        {
            var command = new DeleteGeneroCommand(id);
            var result = await _mediator.Send(command);

            if (!result)
                return NotFound();

            return NoContent();
        }
    }
}
