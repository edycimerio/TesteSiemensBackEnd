using MediatR;

namespace SistemaLivros.Application.Commands.Autores
{
    public class UpdateAutorCommand : IRequest<bool>
    {
        public int Id { get; set; }
        public string Nome { get; set; }
        public string Biografia { get; set; }

        public UpdateAutorCommand(int id, string nome, string biografia)
        {
            Id = id;
            Nome = nome;
            Biografia = biografia;
        }
    }
}
