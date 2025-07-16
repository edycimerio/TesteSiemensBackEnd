using MediatR;
using System.Collections.Generic;

namespace SistemaLivros.Application.Commands.Livros
{
    public class UpdateLivroCommand : IRequest<bool>
    {
        public int Id { get; set; }
        public string Titulo { get; set; }
        public int Ano { get; set; }
        public int AutorId { get; set; }
        public List<int> Generos { get; set; } = new List<int>();

        // Construtor sem parâmetros para o AutoMapper
        public UpdateLivroCommand()
        {
        }

        public UpdateLivroCommand(int id, string titulo, int ano, int autorId, List<int> generos = null)
        {
            Id = id;
            Titulo = titulo;
            Ano = ano;
            AutorId = autorId;
            Generos = generos ?? new List<int>();
        }
    }
}
