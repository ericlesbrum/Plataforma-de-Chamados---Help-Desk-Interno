using Microsoft.EntityFrameworkCore;
using Web_API.Domain.Entities;
using Web_API.Domain.Enums;
using Web_API.Infrastructure.Data.Configurations;

namespace Web_API.Infrastructure.Data;

public class HelpDeskContext : DbContext
{
    public HelpDeskContext(DbContextOptions<HelpDeskContext> options) : base(options)
    { }
    public DbSet<Usuario> Usuarios { get; set; }
    public DbSet<RefreshToken> RefreshTokens { get; set; }
    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        builder.Entity<Usuario>(entity =>
        {
            builder.ApplyConfiguration(new UsuarioConfiguration());
            builder.ApplyConfiguration(new RefreshTokenConfiguration());

            //  HASH GERADO COM BCRYPT
            const string senhaHashPadrao =
            "$2a$12$IOflFcwlidPC85aJtkAaRe/HyhWXzSolT5cO/UMfdpN9kV5M1lHHq";

            // SEED DE USUÁRIOS PADRÃO
            builder.Entity<Usuario>().HasData(
                new Usuario
                {
                    Id = 1,
                    Nome = "Administrador Geral",
                    Email = "admin@sistema.com",
                    SenhaHash = senhaHashPadrao,
                    PerfilUsuario = PerfilUsuarioEnum.Administrador,
                    Ativo = true,
                    DataCriacao = new DateTime(2024, 01, 01)
                },
                new Usuario
                {
                    Id = 2,
                    Nome = "Gestor TI",
                    Email = "gestor.ti@sistema.com",
                    SenhaHash = senhaHashPadrao,
                    PerfilUsuario = PerfilUsuarioEnum.Administrador,
                    Ativo = true,
                    DataCriacao = new DateTime(2024, 01, 01)
                },
                new Usuario
                {
                    Id = 3,
                    Nome = "Analista de Suporte",
                    Email = "suporte@sistema.com",
                    SenhaHash = senhaHashPadrao,
                    PerfilUsuario = PerfilUsuarioEnum.Atendente,
                    Ativo = true,
                    DataCriacao = new DateTime(2024, 01, 01)
                },
                new Usuario
                {
                    Id = 4,
                    Nome = "Colaborador RH",
                    Email = "rh@sistema.com",
                    SenhaHash = senhaHashPadrao,
                    PerfilUsuario = PerfilUsuarioEnum.Atendente,
                    Ativo = true,
                    DataCriacao = new DateTime(2024, 01, 01)
                },
                new Usuario
                {
                    Id = 5,
                    Nome = "Usuário Teste",
                    Email = "teste@sistema.com",
                    SenhaHash = senhaHashPadrao,
                    PerfilUsuario = PerfilUsuarioEnum.Solicitante,
                    Ativo = true,
                    DataCriacao = new DateTime(2024, 01, 01)
                }
            );
        });
    }
}