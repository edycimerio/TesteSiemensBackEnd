using MediatR;

namespace SistemaLivros.Application.Commands.Generos
{
    public class CreateGeneroCommand : IRequest<int>
    {
        public string Nome { get; set; }
        public string Descricao { get; set; }

        public CreateGeneroCommand(string nome, string descricao)
        {
            Nome = nome;
            Descricao = descricao;
        }
    }
}
