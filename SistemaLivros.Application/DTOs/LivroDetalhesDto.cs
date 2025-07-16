using System;
using System.Collections.Generic;

namespace SistemaLivros.Application.DTOs
{
    public class LivroDetalhesDto
    {
        public int Id { get; set; }
        public string Titulo { get; set; }
        public int Ano { get; set; }
        
        // Objeto autor completo
        public AutorDto Autor { get; set; }
        
        // Coleção de gêneros associados ao livro
        public List<GeneroSimplificadoDto> Generos { get; set; } = new List<GeneroSimplificadoDto>();
    }
}
