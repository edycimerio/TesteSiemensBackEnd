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
    public class AutoresController : ControllerBase
    {
        private readonly IAutorRepository _autorRepository;
        private readonly IAutorQueries _autorQueries;

        public AutoresController(IAutorRepository autorRepository, IAutorQueries autorQueries)
        {
            _autorRepository = autorRepository;
            _autorQueries = autorQueries;
        }

        /// <summary>
        /// Obtém todos os autores
        /// </summary>
        /// <returns>Lista de autores</returns>
        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<AutorDto>), 200)]
        public async Task<IActionResult> GetAll()
        {
            var autores = await _autorQueries.GetAllAsync();
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
            var autor = await _autorQueries.GetByIdAsync(id);
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
            var autor = await _autorQueries.GetDetalhesAsync(id);
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

            var autor = new Autor(autorDto.Nome, autorDto.Biografia);
            await _autorRepository.AddAsync(autor);

            autorDto.Id = autor.Id;
            return CreatedAtAction(nameof(GetById), new { id = autor.Id }, autorDto);
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

            var autor = await _autorRepository.GetByIdAsync(id);
            if (autor == null)
                return NotFound();

            autor.Atualizar(autorDto.Nome, autorDto.Biografia);
            await _autorRepository.UpdateAsync(autor);

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
            var autor = await _autorRepository.GetByIdAsync(id);
            if (autor == null)
                return NotFound();

            await _autorRepository.RemoveAsync(id);
            return NoContent();
        }
    }
}
