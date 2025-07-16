using MediatR;

namespace SistemaLivros.Application.Commands.Autores
{
    public class DeleteAutorCommand : IRequest<bool>
    {
        public int Id { get; set; }

        public DeleteAutorCommand(int id)
        {
            Id = id;
        }
    }
}
