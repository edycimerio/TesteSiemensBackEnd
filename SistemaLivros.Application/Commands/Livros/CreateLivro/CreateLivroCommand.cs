using MediatR;

namespace SistemaLivros.Application.Commands.Livros
{
    public class CreateLivroCommand : IRequest<int>
    {
        public string Titulo { get; set; }
        public int Ano { get; set; }
        public int GeneroId { get; set; }
        public int AutorId { get; set; }

        public CreateLivroCommand(string titulo, int ano, int generoId, int autorId)
        {
            Titulo = titulo;
            Ano = ano;
            GeneroId = generoId;
            AutorId = autorId;
        }
    }
}
