using MediatR;

namespace SistemaLivros.Application.Commands.Autores
{
    public class CreateAutorCommand : IRequest<int>
    {
        public string Nome { get; set; }
        public string Biografia { get; set; }

        public CreateAutorCommand(string nome, string biografia)
        {
            Nome = nome;
            Biografia = biografia;
        }
    }
}
