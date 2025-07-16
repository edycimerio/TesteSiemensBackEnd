using MediatR;
using System.Collections.Generic;

namespace SistemaLivros.Application.Commands.Livros
{
    public class CreateLivroCommand : IRequest<int>
    {
        public string Titulo { get; set; }
        public int Ano { get; set; }
        public int AutorId { get; set; }
        public List<int> Generos { get; set; } = new List<int>();

        public CreateLivroCommand(string titulo, int ano, int autorId, List<int> generos = null)
        {
            Titulo = titulo;
            Ano = ano;
            AutorId = autorId;
            Generos = generos ?? new List<int>();
        }
    }
}
