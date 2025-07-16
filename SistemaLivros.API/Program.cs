using FluentValidation;
using FluentValidation.AspNetCore;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Models;
using SistemaLivros.API.Mappings;
using SistemaLivros.API.Validators.Request.Autores;
using SistemaLivros.API.Validators.Request.Generos;
using SistemaLivros.API.Validators.Request.Livros;
using SistemaLivros.Infrastructure.Data;
using SistemaLivros.IoC;
using System;
using System.IO;
using System.Reflection;

var builder = WebApplication.CreateBuilder(args);

// Configuração da conexão com o banco de dados
builder.Services.AddInfrastructure(builder.Configuration);
builder.Services.AddApplication();

// Adicionar controllers com FluentValidation
builder.Services.AddControllers()
    .AddFluentValidation(fv => {
        fv.RegisterValidatorsFromAssemblyContaining<GeneroRequestValidator>();
        fv.DisableDataAnnotationsValidation = true; // Desabilita validações com DataAnnotations
    });

// Configuração do AutoMapper
builder.Services.AddAutoMapper(typeof(MappingProfile));

// Configuração do Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "Sistema de Livros API",
        Version = "v1",
        Description = "API para gerenciamento de livros, autores e gêneros",
        Contact = new OpenApiContact
        {
            Name = "Administrador",
            Email = "admin@sistemaslivros.com"
        }
    });
    
    // Configuração para incluir comentários XML na documentação do Swagger
    var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
    if (File.Exists(xmlPath))
    {
        c.IncludeXmlComments(xmlPath);
    }
});

// Configuração do CORS
builder.Services.AddCors(options =>
{
    // Política permissiva para desenvolvimento
    options.AddPolicy("Development",
        builder => builder
            .AllowAnyOrigin()
            .AllowAnyMethod()
            .AllowAnyHeader());
            
    // Política mais restritiva para produção
    options.AddPolicy("Production",
        builder => builder
            .WithOrigins(
                "https://sistemaslivros.com",
                "https://app.sistemaslivros.com",
                "http://localhost:3000",
                "http://localhost:4200")
            .AllowAnyMethod()
            .AllowAnyHeader()
            .WithExposedHeaders("X-Pagination")
            .SetIsOriginAllowedToAllowWildcardSubdomains());
});

var app = builder.Build();

// Configuração do pipeline HTTP
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
    app.UseSwagger();
    app.UseSwaggerUI(c => 
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "Sistema de Livros API v1");
        c.RoutePrefix = string.Empty; // Para definir a raiz como página do Swagger
    });
}

// Inicialização do banco de dados (sempre executado, independente do ambiente)
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    var logger = services.GetRequiredService<ILogger<Program>>();
    try
    {
        // Inicializa o banco de dados e aplica as migrações
        var dbContext = services.GetRequiredService<AppDbContext>();
        
        // Remover o banco de dados existente para garantir uma criação limpa
        dbContext.Database.EnsureDeleted();
        
        // Criar o banco de dados com o esquema atual
        dbContext.Database.EnsureCreated();
        
        logger.LogInformation("Banco de dados recriado com sucesso.");
        
        // Inserir dados iniciais se necessário
        DatabaseInitializer.SeedDataAsync(services, logger).Wait();
        
        logger.LogInformation("Dados iniciais inseridos com sucesso.");
    }
    catch (Exception ex)
    {
        logger.LogError(ex, "Ocorreu um erro durante a inicialização do banco de dados: {Message}", ex.Message);
        logger.LogError(ex.StackTrace);
        if (ex.InnerException != null)
        {
            logger.LogError("Inner Exception: {Message}", ex.InnerException.Message);
            logger.LogError(ex.InnerException.StackTrace);
        }
    }
}

// Configuração para ambiente de produção
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseRouting();

// Aplicar política de CORS baseada no ambiente
if (app.Environment.IsDevelopment())
{
    app.UseCors("Development");
}
else
{
    app.UseCors("Production");
}

app.UseAuthorization();

app.MapControllers();

app.Run();
