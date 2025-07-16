namespace SistemaLivros.Domain.Entities
{
    public class LivroGenero : Entity
    {
        public int LivroId { get; private set; }
        public virtual Livro Livro { get; private set; }
        
        public int GeneroId { get; private set; }
        public virtual Genero Genero { get; private set; }
        
        protected LivroGenero() { } // Para o EF Core
        
        public LivroGenero(int livroId, int generoId)
        {
            LivroId = livroId;
            GeneroId = generoId;
        }
    }
}
