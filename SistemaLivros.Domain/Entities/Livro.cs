using System.Collections.Generic;
using System.Linq;

namespace SistemaLivros.Domain.Entities
{
    public class Livro : Entity
    {
        public string Titulo { get; private set; }
        public int Ano { get; private set; }
        
        // Coleção de gêneros através da entidade de relacionamento
        public virtual ICollection<LivroGenero> LivroGeneros { get; private set; } = new List<LivroGenero>();
        
        public int AutorId { get; private set; }
        public virtual Autor Autor { get; private set; }
        
        protected Livro() { } // Para o EF Core
        
        public Livro(string titulo, int ano, int autorId)
        {
            Titulo = titulo;
            Ano = ano;
            AutorId = autorId;
            LivroGeneros = new List<LivroGenero>();
        }
        
        public void Atualizar(string titulo, int ano, int autorId)
        {
            Titulo = titulo;
            Ano = ano;
            AutorId = autorId;
        }
        
        // Método para adicionar um gênero ao livro usando o ID
        public void AdicionarGenero(int generoId)
        {
            // Verifica se o livro já tem um ID válido
            if (Id <= 0)
            {
                throw new System.InvalidOperationException("O livro precisa ser salvo antes de adicionar gêneros.");
            }
            
            var livroGenero = new LivroGenero(Id, generoId);
            LivroGeneros.Add(livroGenero);
        }
        
        // Método para adicionar um gênero ao livro usando o objeto Genero
        public void AdicionarGenero(Genero genero)
        {
            // Verifica se o livro já tem um ID válido
            if (Id <= 0)
            {
                throw new System.InvalidOperationException("O livro precisa ser salvo antes de adicionar gêneros.");
            }
            
            var livroGenero = new LivroGenero(Id, genero.Id);
            LivroGeneros.Add(livroGenero);
        }
        
        // Método para remover um gênero do livro
        public void RemoverGenero(int generoId)
        {
            var livroGenero = LivroGeneros.FirstOrDefault(lg => lg.GeneroId == generoId);
            if (livroGenero != null)
            {
                LivroGeneros.Remove(livroGenero);
            }
        }
        
        // Método para limpar todos os gêneros do livro
        public void LimparGeneros()
        {
            LivroGeneros.Clear();
        }
    }
}
