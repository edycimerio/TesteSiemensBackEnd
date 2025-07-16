using System.Collections.Generic;

namespace SistemaLivros.Domain.Entities
{
    public class Autor : Entity
    {
        public string Nome { get; private set; }
        public string Biografia { get; private set; }
        public virtual ICollection<Livro> Livros { get; private set; }
        
        protected Autor() { } // Para o EF Core
        
        public Autor(string nome, string biografia)
        {
            Nome = nome;
            Biografia = biografia;
            Livros = new List<Livro>();
        }
        
        public void Atualizar(string nome, string biografia)
        {
            Nome = nome;
            Biografia = biografia;
        }
    }
}
