using System;
using MediatR;

namespace SistemaLivros.Application.Commands.Autores
{
    public class CreateAutorCommand : IRequest<int>
    {
        public string Nome { get; set; }
        public string Biografia { get; set; }
        public DateTime DataNascimento { get; set; }

        public CreateAutorCommand(string nome, string biografia, DateTime dataNascimento)
        {
            Nome = nome;
            Biografia = biografia;
            DataNascimento = dataNascimento;
        }
    }
}
