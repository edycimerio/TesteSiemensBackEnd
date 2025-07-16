using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;

namespace SistemaLivros.Infrastructure.Data
{
    public static class DatabaseInitializer
    {
        public static async Task InitializeAsync(IServiceProvider serviceProvider, ILogger logger)
        {
            try
            {
                using var scope = serviceProvider.CreateScope();
                var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
                
                logger.LogInformation("Iniciando migração do banco de dados...");
                await dbContext.Database.MigrateAsync();
                logger.LogInformation("Migração do banco de dados concluída com sucesso.");
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Ocorreu um erro durante a inicialização do banco de dados.");
                throw;
            }
        }

        public static async Task SeedDataAsync(IServiceProvider serviceProvider, ILogger logger)
        {
            try
            {
                using var scope = serviceProvider.CreateScope();
                var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
                
                logger.LogInformation("Verificando dados iniciais...");
                
                // Verifica se já existem gêneros no banco
                if (!await dbContext.Generos.AnyAsync())
                {
                    logger.LogInformation("Inserindo dados iniciais de gêneros...");
                    dbContext.Generos.AddRange(
                        new Domain.Entities.Genero("Romance", "Livros de ficção com foco em relacionamentos e emoções"),
                        new Domain.Entities.Genero("Ficção Científica", "Livros baseados em avanços científicos e tecnológicos"),
                        new Domain.Entities.Genero("Fantasia", "Livros com elementos mágicos e sobrenaturais"),
                        new Domain.Entities.Genero("Biografia", "Livros sobre a vida de pessoas reais"),
                        new Domain.Entities.Genero("História", "Livros sobre eventos históricos")
                    );
                    await dbContext.SaveChangesAsync();
                }

                // Verifica se já existem autores no banco
                if (!await dbContext.Autores.AnyAsync())
                {
                    logger.LogInformation("Inserindo dados iniciais de autores...");
                    dbContext.Autores.AddRange(
                        new Domain.Entities.Autor("J.K. Rowling", "Autora britânica conhecida pela série Harry Potter", new DateTime(1965, 7, 31)),
                        new Domain.Entities.Autor("George R.R. Martin", "Autor americano conhecido pela série As Crônicas de Gelo e Fogo", new DateTime(1948, 9, 20)),
                        new Domain.Entities.Autor("Agatha Christie", "Autora britânica de romances policiais", new DateTime(1890, 9, 15)),
                        new Domain.Entities.Autor("Machado de Assis", "Escritor brasileiro, considerado um dos maiores nomes da literatura", new DateTime(1839, 6, 21)),
                        new Domain.Entities.Autor("Clarice Lispector", "Escritora brasileira reconhecida por suas obras de ficção", new DateTime(1920, 12, 10))
                    );
                    await dbContext.SaveChangesAsync();
                }
                
                logger.LogInformation("Dados iniciais verificados e inseridos com sucesso.");
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Ocorreu um erro durante a inserção dos dados iniciais.");
                throw;
            }
        }
    }
}
