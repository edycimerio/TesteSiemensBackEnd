using SistemaLivros.API.Models.Response.Autores;
using SistemaLivros.API.Models.Response.Generos;
using System.Collections.Generic;

namespace SistemaLivros.API.Models.Response.Livros
{
    public class LivroResponse
    {
        public int Id { get; set; }
        public string Titulo { get; set; }
        public int Ano { get; set; }
        
        // Removidas propriedades obsoletas GeneroId e GeneroNome
        
        // Lista de gêneros associados ao livro
        public List<GeneroSimplificadoResponse> Generos { get; set; } = new List<GeneroSimplificadoResponse>();
        
        // Objeto autor completo em vez de propriedades separadas
        public AutorResponse Autor { get; set; }
    }
}
