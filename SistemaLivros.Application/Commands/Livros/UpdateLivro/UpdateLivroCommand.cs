using MediatR;

namespace SistemaLivros.Application.Commands.Livros
{
    public class UpdateLivroCommand : IRequest<bool>
    {
        public int Id { get; set; }
        public string Titulo { get; set; }
        public int Ano { get; set; }
        public int GeneroId { get; set; }
        public int AutorId { get; set; }

        // Construtor sem parâmetros para o AutoMapper
        public UpdateLivroCommand()
        {
        }

        public UpdateLivroCommand(int id, string titulo, int ano, int generoId, int autorId)
        {
            Id = id;
            Titulo = titulo;
            Ano = ano;
            GeneroId = generoId;
            AutorId = autorId;
        }
    }
}
