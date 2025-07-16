using System;

namespace SistemaLivros.API.Models.Response.Livros
{
    public class LivroResponse
    {
        public int Id { get; set; }
        public string Titulo { get; set; }
        public int Ano { get; set; }
        public int GeneroId { get; set; }
        public string GeneroNome { get; set; }
        public int AutorId { get; set; }
        public string AutorNome { get; set; }
        public DateTime DataCadastro { get; set; }
    }
}
