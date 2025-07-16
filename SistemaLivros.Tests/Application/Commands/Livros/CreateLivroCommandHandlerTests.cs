using AutoFixture;
using Moq;
using SistemaLivros.Application.Commands.Livros;
using SistemaLivros.Domain.Entities;
using SistemaLivros.Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace SistemaLivros.Tests.Application.Commands.Livros
{
    public class CreateLivroCommandHandlerTests
    {
        private readonly Mock<ILivroRepository> _livroRepositoryMock;
        private readonly Mock<IGeneroRepository> _generoRepositoryMock;
        private readonly Mock<IAutorRepository> _autorRepositoryMock;
        private readonly CreateLivroCommandHandler _handler;
        private readonly Fixture _fixture;

        public CreateLivroCommandHandlerTests()
        {
            _livroRepositoryMock = new Mock<ILivroRepository>();
            _generoRepositoryMock = new Mock<IGeneroRepository>();
            _autorRepositoryMock = new Mock<IAutorRepository>();
            _handler = new CreateLivroCommandHandler(
                _livroRepositoryMock.Object,
                _generoRepositoryMock.Object,
                _autorRepositoryMock.Object);
            _fixture = new Fixture();
        }

        [Fact]
        public async Task CriaLivroComSucesso()
        {
            // Arrange
            var generoId = 1;
            var autorId = 1;
            var generos = new List<int> { generoId };
            var command = new CreateLivroCommand("Cem Anos de Solidão", 1967, autorId, generos);

            // Criar as entidades usando os construtores públicos
            var genero = new Genero("Realismo Mágico", "Gênero literário");
            var autor = new Autor("Gabriel García Márquez", "Escritor colombiano", new DateTime(1927, 3, 6));
            
            // Usar reflection para definir os Ids para teste
            typeof(Entity).GetProperty("Id").SetValue(genero, generoId);
            typeof(Entity).GetProperty("Id").SetValue(autor, autorId);

            _generoRepositoryMock.Setup(r => r.GetByIdAsync(generoId))
                .ReturnsAsync(genero);
                
            _autorRepositoryMock.Setup(r => r.GetByIdAsync(autorId))
                .ReturnsAsync(autor);
                
            _livroRepositoryMock.Setup(r => r.AddAsync(It.IsAny<Livro>()))
                .Callback<Livro>(l => typeof(Entity).GetProperty("Id").SetValue(l, 1));

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.Equal(1, result);
            _livroRepositoryMock.Verify(r => r.AddAsync(It.Is<Livro>(l => 
                l.Titulo == command.Titulo && 
                l.Ano == command.Ano &&
                l.AutorId == command.AutorId)), 
                Times.Once);
        }

        [Fact]
        public async Task LancaExcecaoQuandoGeneroNaoExiste()
        {
            // Arrange
            var generoId = 99;
            var autorId = 2;
            var generos = new List<int> { generoId };
            var command = new CreateLivroCommand("Cem Anos de Solidão", 1967, autorId, generos);

            // Configuramos o mock para retornar um autor válido
            var autor = new Autor("Gabriel García Márquez", "Escritor colombiano", new DateTime(1927, 3, 6));
            typeof(Entity).GetProperty("Id").SetValue(autor, autorId);
            _autorRepositoryMock.Setup(r => r.GetByIdAsync(autorId))
                .ReturnsAsync(autor);

            // Configuramos o mock para retornar null quando o ID for 99
            _generoRepositoryMock.Setup(r => r.GetByIdAsync(99))
                .ReturnsAsync((Genero)null);

            // Act & Assert
            var exception = await Assert.ThrowsAsync<Exception>(() => 
                _handler.Handle(command, CancellationToken.None));
                
            Assert.Equal($"Gênero com ID {generoId} não encontrado.", exception.Message);
            _livroRepositoryMock.Verify(r => r.AddAsync(It.IsAny<Livro>()), Times.Never);
        }

        [Fact]
        public async Task LancaExcecaoQuandoAutorNaoExiste()
        {
            // Arrange
            var generoId = 1;
            var autorId = 99;
            var generos = new List<int> { generoId };
            var command = new CreateLivroCommand("Cem Anos de Solidão", 1967, autorId, generos);

            var genero = new Genero("Realismo Mágico", "Gênero literário");
            typeof(Entity).GetProperty("Id").SetValue(genero, generoId);

            // Configuramos o mock para retornar um gênero válido quando o ID for 1
            _generoRepositoryMock.Setup(r => r.GetByIdAsync(1))
                .ReturnsAsync(genero);
                
            // Configuramos o mock para retornar null quando o ID for 99
            _autorRepositoryMock.Setup(r => r.GetByIdAsync(99))
                .ReturnsAsync((Autor)null);

            // Act & Assert
            var exception = await Assert.ThrowsAsync<Exception>(() => 
                _handler.Handle(command, CancellationToken.None));
                
            Assert.Equal($"Autor com ID {autorId} não encontrado.", exception.Message);
            _livroRepositoryMock.Verify(r => r.AddAsync(It.IsAny<Livro>()), Times.Never);
        }
    }
}
