namespace SistemaLivros.Application.DTOs
{
    public class LivroDto
    {
        public int Id { get; set; }
        public string Titulo { get; set; }
        public int Ano { get; set; }
        public int GeneroId { get; set; }
        public string GeneroNome { get; set; }
        public int AutorId { get; set; }
        public string AutorNome { get; set; }
    }
}
