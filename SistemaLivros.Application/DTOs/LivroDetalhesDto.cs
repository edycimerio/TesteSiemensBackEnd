using System;

namespace SistemaLivros.Application.DTOs
{
    public class LivroDetalhesDto
    {
        public int Id { get; set; }
        public string Titulo { get; set; }
        public int Ano { get; set; }
        
        // Informações do autor
        public int AutorId { get; set; }
        public string AutorNome { get; set; }
        public string AutorBiografia { get; set; }
        
        // Informações do gênero
        public int GeneroId { get; set; }
        public string GeneroNome { get; set; }
        public string GeneroDescricao { get; set; }
        
        // Data de cadastro
        public DateTime DataCadastro { get; set; }
    }
}
