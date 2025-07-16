using System.Collections.Generic;

namespace SistemaLivros.Domain.Entities
{
    public class Genero : Entity
    {
        public string Nome { get; private set; }
        public string Descricao { get; private set; }
        
        // Coleção para relacionamento muitos-para-muitos
        public virtual ICollection<LivroGenero> LivroGeneros { get; private set; }
        
        protected Genero() { } // Para o EF Core
        
        public Genero(string nome, string descricao)
        {
            Nome = nome;
            Descricao = descricao;
            LivroGeneros = new List<LivroGenero>();
        }
        
        public void Atualizar(string nome, string descricao)
        {
            Nome = nome;
            Descricao = descricao;
        }
    }
}
