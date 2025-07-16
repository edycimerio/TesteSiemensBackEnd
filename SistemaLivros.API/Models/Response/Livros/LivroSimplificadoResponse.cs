using System;

namespace SistemaLivros.API.Models.Response.Livros
{
    public class LivroSimplificadoResponse
    {
        public int Id { get; set; }
        public string Titulo { get; set; }
        public int Ano { get; set; }
    }
}
