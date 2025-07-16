namespace SistemaLivros.Domain.Entities
{
    public class Livro : Entity
    {
        public string Titulo { get; private set; }
        public int Ano { get; private set; }
        
        public int GeneroId { get; private set; }
        public virtual Genero Genero { get; private set; }
        
        public int AutorId { get; private set; }
        public virtual Autor Autor { get; private set; }
        
        protected Livro() { } // Para o EF Core
        
        public Livro(string titulo, int ano, int generoId, int autorId)
        {
            Titulo = titulo;
            Ano = ano;
            GeneroId = generoId;
            AutorId = autorId;
        }
        
        public void Atualizar(string titulo, int ano, int generoId, int autorId)
        {
            Titulo = titulo;
            Ano = ano;
            GeneroId = generoId;
            AutorId = autorId;
        }
    }
}
