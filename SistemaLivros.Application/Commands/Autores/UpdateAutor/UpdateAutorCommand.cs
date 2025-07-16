using System;
using MediatR;

namespace SistemaLivros.Application.Commands.Autores
{
    public class UpdateAutorCommand : IRequest<bool>
    {
        public int Id { get; set; }
        public string Nome { get; set; }
        public string Biografia { get; set; }
        public DateTime DataNascimento { get; set; }

        // Construtor sem parâmetros para o AutoMapper
        public UpdateAutorCommand()
        {
        }

        public UpdateAutorCommand(int id, string nome, string biografia, DateTime dataNascimento)
        {
            Id = id;
            Nome = nome;
            Biografia = biografia;
            DataNascimento = dataNascimento;
        }
    }
}
