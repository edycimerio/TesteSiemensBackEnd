# Sistema de Gerenciamento de Livros - Backend

API RESTful para gerenciamento de livros, autores e gêneros literários.

## Tecnologias

- .NET 8.0
- Entity Framework Core
- SQLite
- MediatR (CQRS)
- Dapper
- AutoMapper
- xUnit e Moq (Testes)

## Arquitetura

O projeto segue os princípios de Domain-Driven Design (DDD) e Command Query Responsibility Segregation (CQRS):

- **Domain**: Entidades e regras de negócio
- **Application**: Commands, Queries e Handlers
- **Infrastructure**: Repositórios, Configurações e Acesso a Dados
- **API**: Controllers e Endpoints

## Entidades

- **Autor**: Representa um escritor com nome, biografia e data de nascimento
- **Gênero**: Categorias literárias com nome e descrição
- **Livro**: Obra literária com título, ano, autor e gêneros associados

## Regras de Exclusão

- **Autor**: Não pode ser excluído se possuir livros associados
- **Gênero**: Não pode ser excluído se possuir livros associados
- **Livro**: Pode ser excluído livremente, removendo automaticamente as associações com gêneros

## Endpoints

### Autores
- `GET /api/autores` - Lista todos os autores
- `GET /api/autores/{id}` - Obtém autor por ID
- `GET /api/autores/{id}/detalhes` - Obtém autor com livros associados
- `POST /api/autores` - Cria novo autor
- `PUT /api/autores/{id}` - Atualiza autor existente
- `DELETE /api/autores/{id}` - Remove autor (se não possuir livros)

### Gêneros
- `GET /api/generos` - Lista todos os gêneros
- `GET /api/generos/{id}` - Obtém gênero por ID
- `GET /api/generos/{id}/detalhes` - Obtém gênero com livros associados
- `POST /api/generos` - Cria novo gênero
- `PUT /api/generos/{id}` - Atualiza gênero existente
- `DELETE /api/generos/{id}` - Remove gênero (se não possuir livros)

### Livros
- `GET /api/livros` - Lista todos os livros
- `GET /api/livros/{id}` - Obtém livro por ID
- `GET /api/livros/{id}/detalhes` - Obtém livro com autor e gêneros
- `GET /api/livros/autor/{autorId}` - Lista livros por autor
- `GET /api/livros/genero/{generoId}` - Lista livros por gênero
- `POST /api/livros` - Cria novo livro
- `PUT /api/livros/{id}` - Atualiza livro existente
- `DELETE /api/livros/{id}` - Remove livro

## Como Executar

1. Clone o repositório
2. Navegue até a pasta do projeto
3. Execute `dotnet restore`
4. Execute `dotnet build`
5. Execute `dotnet run --project SistemaLivros.API`

## Testes

Execute `dotnet test` para rodar os testes unitários.
