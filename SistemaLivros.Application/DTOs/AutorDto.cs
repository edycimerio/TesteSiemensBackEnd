using System;

namespace SistemaLivros.Application.DTOs
{
    public class AutorDto
    {
        public int Id { get; set; }
        public string Nome { get; set; }
        public string Biografia { get; set; }
        public DateTime DataNascimento { get; set; }
    }
}
