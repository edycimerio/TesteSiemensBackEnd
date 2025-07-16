using MediatR;

namespace SistemaLivros.Application.Commands.Livros
{
    public class DeleteLivroCommand : IRequest<bool>
    {
        public int Id { get; set; }

        public DeleteLivroCommand(int id)
        {
            Id = id;
        }
    }
}
