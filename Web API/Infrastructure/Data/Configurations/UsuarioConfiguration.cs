using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Web_API.Domain.Entities;

namespace Web_API.Infrastructure.Data.Configurations;

public class UsuarioConfiguration : IEntityTypeConfiguration<Usuario>
{
    public void Configure(EntityTypeBuilder<Usuario> builder)
    {
        builder.HasKey(u => u.Id);
        builder.Property(u => u.Id)
            .ValueGeneratedOnAdd();

        builder.Property(u => u.Nome)
              .IsRequired()
              .HasMaxLength(100);

        builder.Property(u => u.Email)
              .IsRequired()
              .HasMaxLength(100);

        builder.HasIndex(u => u.Email)
              .IsUnique();

        builder.Property(u => u.SenhaHash)
              .IsRequired()
              .HasMaxLength(100);

        builder.Property(u => u.PerfilUsuario)
              .IsRequired();

        builder.Property(u => u.Ativo)
              .IsRequired();

        builder.Property(u => u.DataCriacao)
               .IsRequired();

        builder.Property(u => u.DataUltimoAcesso)
              .IsRequired(false);

        builder.HasMany(u => u.RefreshTokens)
               .WithOne(rt => rt.Usuario)
               .HasForeignKey(rt => rt.UsuarioId)
               .OnDelete(DeleteBehavior.Cascade);
    }
}
