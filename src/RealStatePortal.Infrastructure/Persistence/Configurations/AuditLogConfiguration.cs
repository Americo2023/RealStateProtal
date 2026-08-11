using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RealStatePortal.Domain.Entities;

namespace RealStatePortal.Infrastructure.Persistence.Configurations;

public sealed class AuditLogConfiguration : IEntityTypeConfiguration<AuditLog>
{
    public void Configure(EntityTypeBuilder<AuditLog> builder)
    {
        builder.ToTable("AuditLogs");
        builder.HasKey(log => log.Id);
        builder.Property(log => log.EntityName).HasMaxLength(100).IsRequired();
        builder.Property(log => log.EntityId).IsRequired();
        builder.Property(log => log.Action).HasMaxLength(100).IsRequired();
        builder.Property(log => log.Details).HasMaxLength(10000).IsRequired();
        builder.Property(log => log.ChangedAt).IsRequired();
        builder.HasIndex(log => new { log.EntityName, log.EntityId, log.ChangedAt });
    }
}