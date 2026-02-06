using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Web_API.Domain.Entities;

namespace Web_API.Infrastructure.Data.Configurations;

public class SecurityEventConfiguration : IEntityTypeConfiguration<SecurityEvent>
{
    public void Configure(EntityTypeBuilder<SecurityEvent> builder)
    {
        builder.ToTable("SecurityEvents");

        builder.HasKey(se => se.Id);

        builder.Property(se => se.EventType)
            .IsRequired()
            .HasConversion<int>();

        builder.Property(se => se.IpAddress)
            .HasMaxLength(45);

        builder.Property(se => se.UserAgent)
            .HasMaxLength(255);

        builder.Property(se => se.CreatedAt)
            .IsRequired();

        builder.HasIndex(se => se.UsuarioId);

        builder.HasOne<Usuario>()
            .WithMany()
            .HasForeignKey(se => se.UsuarioId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}