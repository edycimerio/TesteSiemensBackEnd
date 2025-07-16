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
    public class LivrosController : ControllerBase
    {
        private readonly ILivroRepository _livroRepository;
        private readonly ILivroQueries _livroQueries;
        private readonly IGeneroRepository _generoRepository;
        private readonly IAutorRepository _autorRepository;

        public LivrosController(
            ILivroRepository livroRepository, 
            ILivroQueries livroQueries,
            IGeneroRepository generoRepository,
            IAutorRepository autorRepository)
        {
            _livroRepository = livroRepository;
            _livroQueries = livroQueries;
            _generoRepository = generoRepository;
            _autorRepository = autorRepository;
        }

        /// <summary>
        /// Obtém todos os livros
        /// </summary>
        /// <returns>Lista de livros</returns>
        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<LivroDto>), 200)]
        public async Task<IActionResult> GetAll()
        {
            var livros = await _livroQueries.GetAllAsync();
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
            var livro = await _livroQueries.GetByIdAsync(id);
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
            var livros = await _livroQueries.GetByGeneroIdAsync(generoId);
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
            var livros = await _livroQueries.GetByAutorIdAsync(autorId);
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
            var livros = await _livroQueries.SearchAsync(termo);
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

            // Verifica se o gênero existe
            var genero = await _generoRepository.GetByIdAsync(livroDto.GeneroId);
            if (genero == null)
                return BadRequest($"Gênero com ID {livroDto.GeneroId} não encontrado.");

            // Verifica se o autor existe
            var autor = await _autorRepository.GetByIdAsync(livroDto.AutorId);
            if (autor == null)
                return BadRequest($"Autor com ID {livroDto.AutorId} não encontrado.");

            var livro = new Livro(livroDto.Titulo, livroDto.Ano, livroDto.GeneroId, livroDto.AutorId);
            await _livroRepository.AddAsync(livro);

            livroDto.Id = livro.Id;
            return CreatedAtAction(nameof(GetById), new { id = livro.Id }, livroDto);
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

            // Verifica se o livro existe
            var livro = await _livroRepository.GetByIdAsync(id);
            if (livro == null)
                return NotFound();

            // Verifica se o gênero existe
            var genero = await _generoRepository.GetByIdAsync(livroDto.GeneroId);
            if (genero == null)
                return BadRequest($"Gênero com ID {livroDto.GeneroId} não encontrado.");

            // Verifica se o autor existe
            var autor = await _autorRepository.GetByIdAsync(livroDto.AutorId);
            if (autor == null)
                return BadRequest($"Autor com ID {livroDto.AutorId} não encontrado.");

            livro.Atualizar(livroDto.Titulo, livroDto.Ano, livroDto.GeneroId, livroDto.AutorId);
            await _livroRepository.UpdateAsync(livro);

            return NoContent();
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
            var livro = await _livroRepository.GetByIdAsync(id);
            if (livro == null)
                return NotFound();

            await _livroRepository.RemoveAsync(id);
            return NoContent();
        }
    }
}
