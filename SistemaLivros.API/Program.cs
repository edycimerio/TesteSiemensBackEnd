using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Models;
using SistemaLivros.Infrastructure.Data;
using SistemaLivros.IoC;
using System;
using System.IO;
using System.Reflection;

var builder = WebApplication.CreateBuilder(args);

// Configuração da conexão com o banco de dados
builder.Services.AddInfrastructure(builder.Configuration);
builder.Services.AddApplication();

// Adicionar controllers
builder.Services.AddControllers();

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
    
    // Inicialização do banco de dados em ambiente de desenvolvimento
    using (var scope = app.Services.CreateScope())
    {
        var services = scope.ServiceProvider;
        var logger = services.GetRequiredService<ILogger<Program>>();
        try
        {
            // Inicializa o banco de dados e aplica as migrações
            DatabaseInitializer.InitializeAsync(services, logger).Wait();
            DatabaseInitializer.SeedDataAsync(services, logger).Wait();
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Ocorreu um erro durante a inicialização do banco de dados.");
        }
    }
}
else
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
