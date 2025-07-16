using System;
using System.Collections.Generic;
using SistemaLivros.API.Models.Response.Livros;

namespace SistemaLivros.API.Models.Response.Generos
{
    public class GeneroDetalhesResponse
    {
        public int Id { get; set; }
        public string Nome { get; set; }
        public string Descricao { get; set; }
        public DateTime DataCadastro { get; set; }
        public IEnumerable<LivroSimplificadoResponse> Livros { get; set; }
    }
}
