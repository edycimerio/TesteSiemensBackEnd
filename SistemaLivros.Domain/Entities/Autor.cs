using System;
using System.Collections.Generic;

namespace SistemaLivros.Domain.Entities
{
    public class Autor : Entity
    {
        public string Nome { get; private set; }
        public string Biografia { get; private set; }
        public DateTime DataNascimento { get; private set; }
        public virtual ICollection<Livro> Livros { get; private set; }
        
        protected Autor() { } // Para o EF Core
        
        public Autor(string nome, string biografia, DateTime dataNascimento)
        {
            Nome = nome;
            Biografia = biografia;
            DataNascimento = dataNascimento;
            Livros = new List<Livro>();
        }
        
        public void Atualizar(string nome, string biografia, DateTime dataNascimento)
        {
            Nome = nome;
            Biografia = biografia;
            DataNascimento = dataNascimento;
        }
    }
}
