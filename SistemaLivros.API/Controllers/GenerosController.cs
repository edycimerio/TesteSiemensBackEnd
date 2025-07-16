using Microsoft.AspNetCore.Mvc;
using SistemaLivros.Application.DTOs;
using SistemaLivros.Application.Interfaces;
using SistemaLivros.Domain.Entities;
using SistemaLivros.Domain.Interfaces;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SistemaLivros.API.Controllers
{
    [ApiController]
    [Route("api/v1/[controller]")]
    public class GenerosController : ControllerBase
    {
        private readonly IGeneroRepository _generoRepository;
        private readonly IGeneroQueries _generoQueries;

        public GenerosController(IGeneroRepository generoRepository, IGeneroQueries generoQueries)
        {
            _generoRepository = generoRepository;
            _generoQueries = generoQueries;
        }

        /// <summary>
        /// Obtém todos os gêneros
        /// </summary>
        /// <returns>Lista de gêneros</returns>
        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<GeneroDto>), 200)]
        public async Task<IActionResult> GetAll()
        {
            var generos = await _generoQueries.GetAllAsync();
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
            var genero = await _generoQueries.GetByIdAsync(id);
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
            var genero = await _generoQueries.GetDetalhesAsync(id);
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

            var genero = new Genero(generoDto.Nome, generoDto.Descricao);
            await _generoRepository.AddAsync(genero);

            generoDto.Id = genero.Id;
            return CreatedAtAction(nameof(GetById), new { id = genero.Id }, generoDto);
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

            var genero = await _generoRepository.GetByIdAsync(id);
            if (genero == null)
                return NotFound();

            genero.Atualizar(generoDto.Nome, generoDto.Descricao);
            await _generoRepository.UpdateAsync(genero);

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
            var genero = await _generoRepository.GetByIdAsync(id);
            if (genero == null)
                return NotFound();

            await _generoRepository.RemoveAsync(id);
            return NoContent();
        }
    }
}
