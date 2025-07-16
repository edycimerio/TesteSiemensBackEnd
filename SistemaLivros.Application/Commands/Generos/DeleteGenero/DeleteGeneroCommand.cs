using MediatR;

namespace SistemaLivros.Application.Commands.Generos
{
    public class DeleteGeneroCommand : IRequest<bool>
    {
        public int Id { get; set; }

        public DeleteGeneroCommand(int id)
        {
            Id = id;
        }
    }
}
