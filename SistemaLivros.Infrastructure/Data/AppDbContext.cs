using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage;
using SistemaLivros.Domain.Entities;

namespace SistemaLivros.Infrastructure.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        public DbSet<Genero> Generos { get; set; }
        public DbSet<Autor> Autores { get; set; }
        public DbSet<Livro> Livros { get; set; }
        public DbSet<LivroGenero> LivroGeneros { get; set; }
        
        // Sobrescrever o método OnConfiguring para garantir que o SQLite seja configurado corretamente
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder.UseSqlite("Data Source=SistemaLivros.db");
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Configuração do Genero
            modelBuilder.Entity<Genero>().HasKey(g => g.Id);
            modelBuilder.Entity<Genero>().Property(g => g.Nome).IsRequired().HasMaxLength(100);
            modelBuilder.Entity<Genero>().Property(g => g.Descricao).HasMaxLength(500);

            // Configuração do Autor
            modelBuilder.Entity<Autor>().HasKey(a => a.Id);
            modelBuilder.Entity<Autor>().Property(a => a.Nome).IsRequired().HasMaxLength(100);
            modelBuilder.Entity<Autor>().Property(a => a.Biografia).HasMaxLength(1000);
            modelBuilder.Entity<Autor>()
                .HasMany(a => a.Livros)
                .WithOne(l => l.Autor)
                .HasForeignKey(l => l.AutorId)
                .OnDelete(DeleteBehavior.Restrict);

            // Configuração do Livro
            modelBuilder.Entity<Livro>().HasKey(l => l.Id);
            modelBuilder.Entity<Livro>().Property(l => l.Titulo).IsRequired().HasMaxLength(200);
            modelBuilder.Entity<Livro>().Property(l => l.Ano).IsRequired();
            
            // Configuração da entidade de relacionamento LivroGenero
            modelBuilder.Entity<LivroGenero>().HasKey(lg => lg.Id);
            modelBuilder.Entity<LivroGenero>().ToTable("LivroGeneros");
            
            // Configuração do relacionamento muitos-para-muitos entre Livro e Genero
            modelBuilder.Entity<LivroGenero>()
                .HasOne(lg => lg.Livro)
                .WithMany(l => l.LivroGeneros)
                .HasForeignKey(lg => lg.LivroId);
                
            modelBuilder.Entity<LivroGenero>()
                .HasOne(lg => lg.Genero)
                .WithMany(g => g.LivroGeneros)
                .HasForeignKey(lg => lg.GeneroId);
        }
    }
}
