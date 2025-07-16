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
    public class UpdateLivroCommandHandlerTests
    {
        private readonly Mock<ILivroRepository> _livroRepositoryMock;
        private readonly Mock<IGeneroRepository> _generoRepositoryMock;
        private readonly Mock<IAutorRepository> _autorRepositoryMock;
        private readonly UpdateLivroCommandHandler _handler;
        private readonly Fixture _fixture;

        public UpdateLivroCommandHandlerTests()
        {
            _livroRepositoryMock = new Mock<ILivroRepository>();
            _generoRepositoryMock = new Mock<IGeneroRepository>();
            _autorRepositoryMock = new Mock<IAutorRepository>();
            _handler = new UpdateLivroCommandHandler(
                _livroRepositoryMock.Object,
                _generoRepositoryMock.Object,
                _autorRepositoryMock.Object);
            _fixture = new Fixture();
        }

        [Fact]
        public async Task AtualizaLivroComSucesso()
        {
            // Arrange
            var livroId = 1;
            var generoId = 1;
            var autorId = 1;
            var generos = new List<int> { generoId };
            var command = new UpdateLivroCommand(livroId, "Cem Anos de Solidão - Edição Atualizada", 1967, autorId, generos);

            // Criar as entidades usando os construtores públicos
            var livro = new Livro("Cem Anos de Solidão", 1967, 1);
            var genero = new Genero("Realismo Mágico", "Gênero literário");
            var autor = new Autor("Gabriel García Márquez", "Escritor colombiano", new DateTime(1927, 3, 6));

            // Usar reflection para definir os Ids para teste
            typeof(Entity).GetProperty("Id").SetValue(livro, livroId);
            typeof(Entity).GetProperty("Id").SetValue(genero, generoId);
            typeof(Entity).GetProperty("Id").SetValue(autor, autorId);

            // Adicionar o gênero ao livro
            livro.AdicionarGenero(generoId);

            _livroRepositoryMock.Setup(r => r.GetByIdAsync(livroId))
                .ReturnsAsync(livro);

            _generoRepositoryMock.Setup(r => r.GetByIdAsync(generoId))
                .ReturnsAsync(genero);

            _autorRepositoryMock.Setup(r => r.GetByIdAsync(autorId))
                .ReturnsAsync(autor);

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.True(result);
            _livroRepositoryMock.Verify(r => r.UpdateAsync(It.Is<Livro>(l =>
                l.Id == livroId &&
                l.Titulo == command.Titulo &&
                l.Ano == command.Ano &&
                l.AutorId == command.AutorId)),
                Times.Once);
        }

        [Fact]
        public async Task RetornaFalsoQuandoLivroNaoExiste()
        {
            // Arrange
            var livroId = 99;
            var generoId = 2;
            var autorId = 3;
            var generos = new List<int> { generoId };
            var command = new UpdateLivroCommand(livroId, "Cem Anos de Solidão - Edição Atualizada", 1967, autorId, generos);

            _livroRepositoryMock.Setup(r => r.GetByIdAsync(livroId))
                .ReturnsAsync((Livro)null);

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.False(result);
            _livroRepositoryMock.Verify(r => r.UpdateAsync(It.IsAny<Livro>()), Times.Never);
        }

        [Fact]
        public async Task LancaExcecaoQuandoGeneroNaoExiste()
        {
            // Arrange
            var livroId = 1;
            var generoId = 99;
            var autorId = 3;
            var generos = new List<int> { generoId };
            var command = new UpdateLivroCommand(livroId, "Cem Anos de Solidão - Edição Atualizada", 1967, autorId, generos);

            var livro = new Livro("Cem Anos de Solidão", 1967, 1);
            var autor = new Autor("Gabriel García Márquez", "Escritor colombiano", new DateTime(1980, 1, 1));

            typeof(Entity).GetProperty("Id").SetValue(livro, livroId);
            typeof(Entity).GetProperty("Id").SetValue(autor, autorId);

            // Configuramos o mock para retornar um livro válido quando o ID for 1
            _livroRepositoryMock.Setup(r => r.GetByIdAsync(1))
                .ReturnsAsync(livro);

            // Configuramos o mock para retornar um autor válido
            _autorRepositoryMock.Setup(r => r.GetByIdAsync(autorId))
                .ReturnsAsync(autor);

            // Configuramos o mock para retornar null quando o ID for 99
            _generoRepositoryMock.Setup(r => r.GetByIdAsync(99))
                .ReturnsAsync((Genero)null);

            // Act & Assert
            var exception = await Assert.ThrowsAsync<Exception>(() =>
                _handler.Handle(command, CancellationToken.None));

            Assert.Equal($"Gênero com ID {generoId} não encontrado.", exception.Message);
            _livroRepositoryMock.Verify(r => r.UpdateAsync(It.IsAny<Livro>()), Times.Never);
        }

        [Fact]
        public async Task LancaExcecaoQuandoAutorNaoExiste()
        {
            // Arrange
            var livroId = 1;
            var generoId = 2;
            var autorId = 99;
            var generos = new List<int> { generoId };
            var command = new UpdateLivroCommand(livroId, "Cem Anos de Solidão - Edição Atualizada", 1967, autorId, generos);

            var livro = new Livro("Cem Anos de Solidão", 1967, 1);
            var genero = new Genero("Realismo Mágico", "Gênero literário");

            typeof(Entity).GetProperty("Id").SetValue(livro, livroId);
            typeof(Entity).GetProperty("Id").SetValue(genero, generoId);

            // Configuramos o mock para retornar um livro válido quando o ID for 1
            _livroRepositoryMock.Setup(r => r.GetByIdAsync(1))
                .ReturnsAsync(livro);

            // Configuramos o mock para retornar um gênero válido quando o ID for 2
            _generoRepositoryMock.Setup(r => r.GetByIdAsync(2))
                .ReturnsAsync(genero);

            // Configuramos o mock para retornar null quando o ID for 99
            _autorRepositoryMock.Setup(r => r.GetByIdAsync(99))
                .ReturnsAsync((Autor)null);

            // Act & Assert
            var exception = await Assert.ThrowsAsync<Exception>(() =>
                _handler.Handle(command, CancellationToken.None));

            Assert.Equal($"Autor com ID {autorId} não encontrado.", exception.Message);
            _livroRepositoryMock.Verify(r => r.UpdateAsync(It.IsAny<Livro>()), Times.Never);
        }
    }
}