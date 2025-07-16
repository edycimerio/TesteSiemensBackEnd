using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SistemaLivros.Application.Interfaces;
using SistemaLivros.Domain.Interfaces;
using SistemaLivros.Infrastructure.Data;
using SistemaLivros.Infrastructure.Queries;
using SistemaLivros.Infrastructure.Repositories;
using System.Reflection;

namespace SistemaLivros.IoC
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
        {
            // Configuração do DbContext
            services.AddDbContext<AppDbContext>(options =>
                options.UseSqlite(configuration.GetConnectionString("DefaultConnection")));

            // Configuração do DapperContext
            services.AddSingleton(provider => 
                new DapperContext(configuration.GetConnectionString("DefaultConnection")));

            // Registro dos Repositórios
            services.AddScoped(typeof(IRepository<>), typeof(Repository<>));
            services.AddScoped<IGeneroRepository, GeneroRepository>();
            services.AddScoped<IAutorRepository, AutorRepository>();
            services.AddScoped<ILivroRepository, LivroRepository>();

            // Registro das Queries
            services.AddScoped<IGeneroQueries, GeneroQueries>();
            services.AddScoped<IAutorQueries, AutorQueries>();
            services.AddScoped<ILivroQueries, LivroQueries>();

            return services;
        }

        public static IServiceCollection AddApplication(this IServiceCollection services)
        {
            // Registrar MediatR
            services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(Assembly.Load("SistemaLivros.Application")));

            return services;
        }
    }
}
