using System.Collections.Generic;

namespace SistemaLivros.Domain.Entities
{
    public class Genero : Entity
    {
        public string Nome { get; private set; }
        public string Descricao { get; private set; }
        public virtual ICollection<Livro> Livros { get; private set; }
        
        protected Genero() { } // Para o EF Core
        
        public Genero(string nome, string descricao)
        {
            Nome = nome;
            Descricao = descricao;
            Livros = new List<Livro>();
        }
        
        public void Atualizar(string nome, string descricao)
        {
            Nome = nome;
            Descricao = descricao;
        }
    }
}
