using System;
using System.Collections.Generic;
using SistemaLivros.API.Models.Response.Autores;
using SistemaLivros.API.Models.Response.Generos;

namespace SistemaLivros.API.Models.Response.Livros
{
    public class LivroDetalhesResponse
    {
        public int Id { get; set; }
        public string Titulo { get; set; }
        public int Ano { get; set; }
        
        // Objeto autor completo
        public AutorResponse Autor { get; set; }
        
        // Lista de gêneros associados ao livro
        public List<GeneroSimplificadoResponse> Generos { get; set; } = new List<GeneroSimplificadoResponse>();
    }
}
