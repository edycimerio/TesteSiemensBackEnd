using AutoMapper;
using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using SistemaLivros.API.Models.Request.Livros;
using SistemaLivros.API.Models.Response.Livros;
using SistemaLivros.API.Validators.Request.Livros;
using SistemaLivros.Application.Commands.Livros;
using SistemaLivros.Application.Common;
using SistemaLivros.Application.Queries.Livros.GetAllLivros;
using SistemaLivros.Application.Queries.Livros.GetLivroById;
using SistemaLivros.Application.Queries.Livros.GetLivroDetalhes;
using SistemaLivros.Application.Queries.Livros.GetLivrosByAutor;
using SistemaLivros.Application.Queries.Livros.GetLivrosByGenero;
using SistemaLivros.Application.Queries.Livros.SearchLivros;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SistemaLivros.API.Controllers
{
    [ApiController]
    [Route("api/v1/[controller]")]
    public class LivrosController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly IMapper _mapper;

        public LivrosController(IMediator mediator, IMapper mapper)
        {
            _mediator = mediator;
            _mapper = mapper;
        }

        /// <summary>
        /// Obtém todos os livros com paginação
        /// </summary>
        /// <param name="pageNumber">Número da página (padrão: 1)</param>
        /// <param name="pageSize">Tamanho da página (padrão: 10)</param>
        /// <returns>Lista paginada de livros</returns>
        [HttpGet]
        [ProducesResponseType(typeof(PagedResult<LivroResponse>), 200)]
        public async Task<IActionResult> GetAll([FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10)
        {
            var query = new GetAllLivrosQuery(pageNumber, pageSize);
            var livros = await _mediator.Send(query);
            var response = _mapper.Map<PagedResult<LivroResponse>>(livros);
            return Ok(response);
        }

        /// <summary>
        /// Obtém um livro pelo ID
        /// </summary>
        /// <param name="id">ID do livro</param>
        /// <returns>Livro encontrado</returns>
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(LivroResponse), 200)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> GetById(int id)
        {
            var query = new GetLivroByIdQuery(id);
            var livro = await _mediator.Send(query);
            if (livro == null)
                return NotFound();

            var response = _mapper.Map<LivroResponse>(livro);
            return Ok(response);
        }

        /// <summary>
        /// Obtém detalhes completos de um livro pelo ID
        /// </summary>
        /// <param name="id">ID do livro</param>
        /// <returns>Detalhes completos do livro</returns>
        [HttpGet("{id}/detalhes")]
        [ProducesResponseType(typeof(LivroDetalhesResponse), 200)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> GetDetalhes(int id)
        {
            var query = new GetLivroDetalhesQuery(id);
            var livroDetalhes = await _mediator.Send(query);
            if (livroDetalhes == null)
                return NotFound();

            var response = _mapper.Map<LivroDetalhesResponse>(livroDetalhes);
            return Ok(response);
        }

        /// <summary>
        /// Obtém livros por gênero com paginação
        /// </summary>
        /// <param name="generoId">ID do gênero</param>
        /// <param name="pageNumber">Número da página (padrão: 1)</param>
        /// <param name="pageSize">Tamanho da página (padrão: 10)</param>
        /// <returns>Lista paginada de livros do gênero</returns>
        [HttpGet("porGenero/{generoId}")]
        [ProducesResponseType(typeof(PagedResult<LivroResponse>), 200)]
        public async Task<IActionResult> GetByGenero(int generoId, [FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10)
        {
            var query = new GetLivrosByGeneroQuery(generoId, pageNumber, pageSize);
            var livros = await _mediator.Send(query);
            var response = _mapper.Map<PagedResult<LivroResponse>>(livros);
            return Ok(response);
        }

        /// <summary>
        /// Obtém livros por autor com paginação
        /// </summary>
        /// <param name="autorId">ID do autor</param>
        /// <param name="pageNumber">Número da página (padrão: 1)</param>
        /// <param name="pageSize">Tamanho da página (padrão: 10)</param>
        /// <returns>Lista paginada de livros do autor</returns>
        [HttpGet("porAutor/{autorId}")]
        [ProducesResponseType(typeof(PagedResult<LivroResponse>), 200)]
        public async Task<IActionResult> GetByAutor(int autorId, [FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10)
        {
            var query = new GetLivrosByAutorQuery(autorId, pageNumber, pageSize);
            var livros = await _mediator.Send(query);
            var response = _mapper.Map<PagedResult<LivroResponse>>(livros);
            return Ok(response);
        }

        /// <summary>
        /// Pesquisa livros por termo com paginação
        /// </summary>
        /// <param name="termo">Termo de pesquisa</param>
        /// <param name="pageNumber">Número da página (padrão: 1)</param>
        /// <param name="pageSize">Tamanho da página (padrão: 10)</param>
        /// <returns>Lista paginada de livros encontrados</returns>
        [HttpGet("pesquisa")]
        [ProducesResponseType(typeof(PagedResult<LivroResponse>), 200)]
        public async Task<IActionResult> Search([FromQuery] string termo, [FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10)
        {
            var query = new SearchLivrosQuery(termo, pageNumber, pageSize);
            var livros = await _mediator.Send(query);
            var response = _mapper.Map<PagedResult<LivroResponse>>(livros);
            return Ok(response);
        }

        /// <summary>
        /// Cria um novo livro
        /// </summary>
        /// <param name="request">Dados do livro</param>
        /// <returns>Livro criado</returns>
        [HttpPost]
        [ProducesResponseType(typeof(LivroResponse), 201)]
        [ProducesResponseType(400)]
        public async Task<IActionResult> Create([FromBody] LivroRequest request, [FromServices] IValidator<LivroRequest> validator)
        {
            // Validação manual assíncrona
            var validationResult = await validator.ValidateAsync(request);
            if (!validationResult.IsValid)
            {
                return BadRequest(validationResult.Errors.Select(e => new { Property = e.PropertyName, Error = e.ErrorMessage }));
            }

            var command = _mapper.Map<CreateLivroCommand>(request);

            try
            {
                var id = await _mediator.Send(command);
                
                // Busca o livro criado para retornar na resposta
                var query = new GetLivroByIdQuery(id);
                var livro = await _mediator.Send(query);
                var response = _mapper.Map<LivroResponse>(livro);
                
                return CreatedAtAction(nameof(GetById), new { id }, response);
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
        /// <param name="request">Dados atualizados do livro</param>
        /// <returns>Resultado da operação</returns>
        [HttpPut("{id}")]
        [ProducesResponseType(204)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> Update(int id, [FromBody] LivroRequest request, [FromServices] IValidator<LivroRequest> validator)
        {
            // Validação manual assíncrona
            var validationResult = await validator.ValidateAsync(request);
            if (!validationResult.IsValid)
            {
                return BadRequest(validationResult.Errors.Select(e => new { Property = e.PropertyName, Error = e.ErrorMessage }));
            }

            var command = _mapper.Map<UpdateLivroCommand>(request);
            command.Id = id; // Define o ID do comando a partir da rota

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
