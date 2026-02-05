using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Web_API.Domain.Entities;

namespace Web_API.Infrastructure.Data.Configurations;

public class RefreshTokenConfiguration : IEntityTypeConfiguration<RefreshToken>
{
    public void Configure(EntityTypeBuilder<RefreshToken> builder)
    {
        builder.HasKey(rt => rt.Id);

        builder.Property(rt => rt.Token)
               .IsRequired()
               .HasMaxLength(500);

        builder.Property(rt => rt.CreatedAt)
               .IsRequired();

        builder.Property(rt => rt.ExpiryDate)
               .IsRequired();

        builder.Property(rt => rt.IsRevoked)
               .IsRequired();

        builder.HasOne(rt => rt.Usuario)
               .WithMany(u => u.RefreshTokens)
               .HasForeignKey(rt => rt.UsuarioId)
               .OnDelete(DeleteBehavior.Cascade);

        builder.HasIndex(rt => rt.Token)
              .IsUnique();

        builder.HasIndex(rt => rt.UsuarioId);
    }
}

