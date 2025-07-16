using System.Collections.Generic;

namespace SistemaLivros.Application.DTOs
{
    public class GeneroDetalhesDto
    {
        public int Id { get; set; }
        public string Nome { get; set; }
        public string Descricao { get; set; }
        public List<LivroDto> Livros { get; set; } = new List<LivroDto>();
    }
}
