using MediatR;

namespace SistemaLivros.Application.Commands.Generos
{
    public class UpdateGeneroCommand : IRequest<bool>
    {
        public int Id { get; set; }
        public string Nome { get; set; }
        public string Descricao { get; set; }

        // Construtor sem parâmetros para o AutoMapper
        public UpdateGeneroCommand()
        {
        }

        public UpdateGeneroCommand(int id, string nome, string descricao)
        {
            Id = id;
            Nome = nome;
            Descricao = descricao;
        }
    }
}
