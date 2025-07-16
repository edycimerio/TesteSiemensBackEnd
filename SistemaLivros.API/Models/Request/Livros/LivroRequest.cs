using System.ComponentModel.DataAnnotations;

namespace SistemaLivros.API.Models.Request.Livros
{
    public class LivroRequest
    {
        [Required(ErrorMessage = "O título do livro é obrigatório")]
        [StringLength(200, ErrorMessage = "O título do livro deve ter no máximo {1} caracteres")]
        public string Titulo { get; set; }

        [Required(ErrorMessage = "O ano do livro é obrigatório")]
        [Range(1000, 9999, ErrorMessage = "O ano deve estar entre {1} e {2}")]
        public int Ano { get; set; }

        [Required(ErrorMessage = "O ID do autor é obrigatório")]
        public int AutorId { get; set; }

        [Required(ErrorMessage = "O ID do gênero é obrigatório")]
        public int GeneroId { get; set; }
    }
}
