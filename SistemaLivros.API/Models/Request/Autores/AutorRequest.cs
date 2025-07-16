using System;
using System.ComponentModel.DataAnnotations;

namespace SistemaLivros.API.Models.Request.Autores
{
    public class AutorRequest
    {
        [Required(ErrorMessage = "O nome do autor é obrigatório")]
        [StringLength(100, ErrorMessage = "O nome do autor deve ter no máximo {1} caracteres")]
        public string Nome { get; set; }

        [StringLength(1000, ErrorMessage = "A biografia do autor deve ter no máximo {1} caracteres")]
        public string Biografia { get; set; }
        
        [DataType(DataType.Date)]
        public DateTime? DataNascimento { get; set; }
    }
}
