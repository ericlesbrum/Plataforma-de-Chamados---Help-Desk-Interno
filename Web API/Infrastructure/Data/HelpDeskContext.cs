using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System.Reflection.Emit;
using Web_API.Domain.Entities;
using Web_API.Domain.Enums;

namespace Web_API.Infrastructure.Data
{
    public class HelpDeskContext : IdentityDbContext<IdentityUser>
    {
        public HelpDeskContext(DbContextOptions<HelpDeskContext> options) : base(options)
        {
        }

        public DbSet<Usuario> Usuarios { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.Entity<Usuario>(entity =>
            {
                // Configurações básicas
                entity.HasKey(u => u.Id);

                entity.Property(u => u.Nome)
                      .IsRequired()
                      .HasMaxLength(100);

                entity.Property(u => u.Email)
                      .IsRequired()
                      .HasMaxLength(100);

                entity.HasIndex(u => u.Email)
                      .IsUnique();

                entity.Property(u => u.SenhaHash)
                      .IsRequired()
                      .HasMaxLength(100);

                entity.Property(u => u.PerfilUsuario)
                      .IsRequired();

                entity.Property(u => u.Ativo)
                      .IsRequired();


                //  HASH GERADO COM BCRYPT
                const string senhaHashPadrao =
                "$2a$12$M7rtSZx83024qcjy4o1W0.AVJYXaPNwLrbObO43.uY7zY/coxi5Pm";

                // SEED DE USUÁRIOS PADRÃO
                builder.Entity<Usuario>().HasData(
                    new Usuario
                    {
                        Id = 1,
                        Nome = "Administrador Geral",
                        Email = "admin@sistema.com",
                        SenhaHash = senhaHashPadrao,
                        PerfilUsuario = PerfilUsuarioEnum.Administrador,
                        Ativo = true
                    },
                    new Usuario
                    {
                        Id = 2,
                        Nome = "Gestor TI",
                        Email = "gestor.ti@sistema.com",
                        SenhaHash = senhaHashPadrao,
                        PerfilUsuario = PerfilUsuarioEnum.Administrador,
                        Ativo = true
                    },
                    new Usuario
                    {
                        Id = 3,
                        Nome = "Analista de Suporte",
                        Email = "suporte@sistema.com",
                        SenhaHash = senhaHashPadrao,
                        PerfilUsuario = PerfilUsuarioEnum.Atendente,
                        Ativo = true
                    },
                    new Usuario
                    {
                        Id = 4,
                        Nome = "Colaborador RH",
                        Email = "rh@sistema.com",
                        SenhaHash = senhaHashPadrao,
                        PerfilUsuario = PerfilUsuarioEnum.Atendente,
                        Ativo = true
                    },
                    new Usuario
                    {
                        Id = 5,
                        Nome = "Usuário Teste",
                        Email = "teste@sistema.com",
                        SenhaHash = senhaHashPadrao,
                        PerfilUsuario = PerfilUsuarioEnum.Solicitante,
                        Ativo = true
                    }
                );
            });
        }
    }
}
