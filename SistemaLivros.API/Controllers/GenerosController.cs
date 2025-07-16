using AutoMapper;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using SistemaLivros.API.Models.Request.Generos;
using SistemaLivros.API.Models.Response.Generos;
using SistemaLivros.Application.Commands.Generos;
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
        private readonly IMapper _mapper;

        public GenerosController(IMediator mediator, IMapper mapper)
        {
            _mediator = mediator;
            _mapper = mapper;
        }

        /// <summary>
        /// Obtém todos os gêneros
        /// </summary>
        /// <returns>Lista de gêneros</returns>
        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<GeneroResponse>), 200)]
        public async Task<IActionResult> GetAll()
        {
            var query = new GetAllGenerosQuery();
            var generos = await _mediator.Send(query);
            var response = _mapper.Map<IEnumerable<GeneroResponse>>(generos);
            return Ok(response);
        }

        /// <summary>
        /// Obtém um gênero pelo ID
        /// </summary>
        /// <param name="id">ID do gênero</param>
        /// <returns>Gênero encontrado</returns>
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(GeneroResponse), 200)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> GetById(int id)
        {
            var query = new GetGeneroByIdQuery(id);
            var genero = await _mediator.Send(query);
            if (genero == null)
                return NotFound();

            var response = _mapper.Map<GeneroResponse>(genero);
            return Ok(response);
        }

        /// <summary>
        /// Obtém detalhes de um gênero, incluindo seus livros
        /// </summary>
        /// <param name="id">ID do gênero</param>
        /// <returns>Detalhes do gênero</returns>
        [HttpGet("{id}/detalhes")]
        [ProducesResponseType(typeof(GeneroDetalhesResponse), 200)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> GetDetalhes(int id)
        {
            var query = new GetGeneroDetalhesQuery(id);
            var genero = await _mediator.Send(query);
            if (genero == null)
                return NotFound();

            var response = _mapper.Map<GeneroDetalhesResponse>(genero);
            return Ok(response);
        }

        /// <summary>
        /// Cria um novo gênero
        /// </summary>
        /// <param name="request">Dados do gênero</param>
        /// <returns>Gênero criado</returns>
        [HttpPost]
        [ProducesResponseType(typeof(GeneroResponse), 201)]
        [ProducesResponseType(400)]
        public async Task<IActionResult> Create([FromBody] GeneroRequest request)
        {
            var command = _mapper.Map<CreateGeneroCommand>(request);
            var id = await _mediator.Send(command);

            // Busca o gênero criado para retornar na resposta
            var query = new GetGeneroByIdQuery(id);
            var genero = await _mediator.Send(query);
            var response = _mapper.Map<GeneroResponse>(genero);

            return CreatedAtAction(nameof(GetById), new { id }, response);
        }

        /// <summary>
        /// Atualiza um gênero existente
        /// </summary>
        /// <param name="id">ID do gênero</param>
        /// <param name="request">Dados atualizados do gênero</param>
        /// <returns>Resultado da operação</returns>
        [HttpPut("{id}")]
        [ProducesResponseType(204)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> Update(int id, [FromBody] GeneroRequest request)
        {
            var command = _mapper.Map<UpdateGeneroCommand>(request);
            command.Id = id; // Define o ID do comando a partir da rota
            
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
