using System;
using System.Collections.Generic;
using SistemaLivros.API.Models.Response.Livros;

namespace SistemaLivros.API.Models.Response.Autores
{
    public class AutorDetalhesResponse
    {
        public int Id { get; set; }
        public string Nome { get; set; }
        public string Biografia { get; set; }
        public DateTime? DataNascimento { get; set; }
        // Removido DataCadastro pois não existe na tabela
        public IEnumerable<LivroSimplificadoResponse> Livros { get; set; }
    }
}
