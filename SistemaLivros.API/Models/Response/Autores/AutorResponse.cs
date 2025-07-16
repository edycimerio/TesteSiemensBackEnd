using System;

namespace SistemaLivros.API.Models.Response.Autores
{
    public class AutorResponse
    {
        public int Id { get; set; }
        public string Nome { get; set; }
        public string Biografia { get; set; }
        public DateTime? DataNascimento { get; set; }
        public DateTime DataCadastro { get; set; }
    }
}
