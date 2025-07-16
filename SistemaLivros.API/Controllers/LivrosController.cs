using MediatR;
using Microsoft.AspNetCore.Mvc;
using SistemaLivros.Application.Commands.Livros;
using SistemaLivros.Application.DTOs;
using SistemaLivros.Application.Queries.Livros.GetAllLivros;
using SistemaLivros.Application.Queries.Livros.GetLivroById;
using SistemaLivros.Application.Queries.Livros.GetLivroDetalhes;
using SistemaLivros.Application.Queries.Livros.GetLivrosByAutor;
using SistemaLivros.Application.Queries.Livros.GetLivrosByGenero;
using SistemaLivros.Application.Queries.Livros.SearchLivros;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SistemaLivros.API.Controllers
{
    [ApiController]
    [Route("api/v1/[controller]")]
    public class LivrosController : ControllerBase
    {
        private readonly IMediator _mediator;

        public LivrosController(IMediator mediator)
        {
            _mediator = mediator;
        }

        /// <summary>
        /// Obtém todos os livros
        /// </summary>
        /// <returns>Lista de livros</returns>
        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<LivroDto>), 200)]
        public async Task<IActionResult> GetAll()
        {
            var query = new GetAllLivrosQuery();
            var livros = await _mediator.Send(query);
            return Ok(livros);
        }

        /// <summary>
        /// Obtém um livro pelo ID
        /// </summary>
        /// <param name="id">ID do livro</param>
        /// <returns>Livro encontrado</returns>
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(LivroDto), 200)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> GetById(int id)
        {
            var query = new GetLivroByIdQuery(id);
            var livro = await _mediator.Send(query);
            if (livro == null)
                return NotFound();

            return Ok(livro);
        }

        /// <summary>
        /// Obtém livros por gênero
        /// </summary>
        /// <param name="generoId">ID do gênero</param>
        /// <returns>Lista de livros do gênero</returns>
        [HttpGet("porGenero/{generoId}")]
        [ProducesResponseType(typeof(IEnumerable<LivroDto>), 200)]
        public async Task<IActionResult> GetByGenero(int generoId)
        {
            var query = new GetLivrosByGeneroQuery(generoId);
            var livros = await _mediator.Send(query);
            return Ok(livros);
        }

        /// <summary>
        /// Obtém livros por autor
        /// </summary>
        /// <param name="autorId">ID do autor</param>
        /// <returns>Lista de livros do autor</returns>
        [HttpGet("porAutor/{autorId}")]
        [ProducesResponseType(typeof(IEnumerable<LivroDto>), 200)]
        public async Task<IActionResult> GetByAutor(int autorId)
        {
            var query = new GetLivrosByAutorQuery(autorId);
            var livros = await _mediator.Send(query);
            return Ok(livros);
        }

        /// <summary>
        /// Pesquisa livros por termo
        /// </summary>
        /// <param name="termo">Termo de pesquisa</param>
        /// <returns>Lista de livros encontrados</returns>
        [HttpGet("pesquisa")]
        [ProducesResponseType(typeof(IEnumerable<LivroDto>), 200)]
        public async Task<IActionResult> Search([FromQuery] string termo)
        {
            var query = new SearchLivrosQuery(termo);
            var livros = await _mediator.Send(query);
            return Ok(livros);
        }

        /// <summary>
        /// Cria um novo livro
        /// </summary>
        /// <param name="livroDto">Dados do livro</param>
        /// <returns>Livro criado</returns>
        [HttpPost]
        [ProducesResponseType(typeof(LivroDto), 201)]
        [ProducesResponseType(400)]
        public async Task<IActionResult> Create([FromBody] LivroDto livroDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var command = new CreateLivroCommand(
                livroDto.Titulo,
                livroDto.Ano,
                livroDto.GeneroId,
                livroDto.AutorId);

            try
            {
                var id = await _mediator.Send(command);
                livroDto.Id = id;
                return CreatedAtAction(nameof(GetById), new { id }, livroDto);
            }
            catch (System.Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        /// <summary>
        /// Atualiza um livro existente
        /// </summary>
        /// <param name="id">ID do livro</param>
        /// <param name="livroDto">Dados atualizados do livro</param>
        /// <returns>Resultado da operação</returns>
        [HttpPut("{id}")]
        [ProducesResponseType(204)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> Update(int id, [FromBody] LivroDto livroDto)
        {
            if (id != livroDto.Id)
                return BadRequest("O ID do livro na URL não corresponde ao ID no corpo da requisição.");

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var command = new UpdateLivroCommand(
                id,
                livroDto.Titulo,
                livroDto.Ano,
                livroDto.GeneroId,
                livroDto.AutorId);

            try
            {
                var result = await _mediator.Send(command);
                if (!result)
                    return NotFound();

                return NoContent();
            }
            catch (System.Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        /// <summary>
        /// Remove um livro
        /// </summary>
        /// <param name="id">ID do livro</param>
        /// <returns>Resultado da operação</returns>
        [HttpDelete("{id}")]
        [ProducesResponseType(204)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> Delete(int id)
        {
            var command = new DeleteLivroCommand(id);
            var result = await _mediator.Send(command);

            if (!result)
                return NotFound();

            return NoContent();
        }
    }
}
