using System.ComponentModel.DataAnnotations;

namespace SistemaLivros.API.Models.Request.Generos
{
    public class GeneroRequest
    {
        [Required(ErrorMessage = "O nome do gênero é obrigatório")]
        [StringLength(100, ErrorMessage = "O nome do gênero deve ter no máximo {1} caracteres")]
        public string Nome { get; set; }

        [StringLength(500, ErrorMessage = "A descrição do gênero deve ter no máximo {1} caracteres")]
        public string Descricao { get; set; }
    }
}
