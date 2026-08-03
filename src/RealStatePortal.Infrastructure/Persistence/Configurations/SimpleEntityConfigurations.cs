using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RealStatePortal.Domain.Entities;

namespace RealStatePortal.Infrastructure.Persistence.Configurations;

public sealed class FavoriteConfiguration : IEntityTypeConfiguration<Favorite>
{
    public void Configure(EntityTypeBuilder<Favorite> builder)
    {
        builder.ToTable("Favorites");
        builder.HasKey(favorite => favorite.Id);
        builder.Property(favorite => favorite.UserId).IsRequired();
        builder.Property(favorite => favorite.PropertyId).IsRequired();
        builder.Property(favorite => favorite.CreatedAt).IsRequired();
        builder.HasIndex(favorite => new { favorite.UserId, favorite.PropertyId }).IsUnique();
        builder.HasOne<Property>().WithMany().HasForeignKey(favorite => favorite.PropertyId).OnDelete(DeleteBehavior.Cascade);
        builder.HasOne<User>().WithMany().HasForeignKey(favorite => favorite.UserId).OnDelete(DeleteBehavior.Restrict);
    }
}

public sealed class UserConfiguration : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder.ToTable("Users");
        builder.HasKey(user => user.Id);
        builder.Property(user => user.Auth0UserId).HasMaxLength(150).IsRequired();
        builder.HasIndex(user => user.Auth0UserId).IsUnique();
        builder.Property(user => user.Email).HasMaxLength(320).IsRequired();
        builder.Property(user => user.FirstName).HasMaxLength(100).IsRequired();
        builder.Property(user => user.LastName).HasMaxLength(100).IsRequired();
        builder.Property(user => user.Role).HasConversion<string>().HasMaxLength(30).IsRequired();
        builder.Property(user => user.CreatedAt).IsRequired();
        builder.Property(user => user.UpdatedAt).IsRequired();
    }
}

public sealed class BrokerProfileConfiguration : IEntityTypeConfiguration<BrokerProfile>
{
    public void Configure(EntityTypeBuilder<BrokerProfile> builder)
    {
        builder.ToTable("BrokerProfiles");
        builder.HasKey(profile => profile.Id);
        builder.Property(profile => profile.UserId).IsRequired();
        builder.HasIndex(profile => profile.UserId).IsUnique();
        builder.Property(profile => profile.FullName).HasMaxLength(200).IsRequired();
        builder.Property(profile => profile.Email).HasMaxLength(320).IsRequired();
        builder.Property(profile => profile.Phone).HasMaxLength(50);
        builder.Property(profile => profile.Bio).HasMaxLength(2000);
        builder.HasOne<User>().WithOne().HasForeignKey<BrokerProfile>(profile => profile.UserId).OnDelete(DeleteBehavior.Cascade);
    }
}

public sealed class ContactInquiryConfiguration : IEntityTypeConfiguration<ContactInquiry>
{
    public void Configure(EntityTypeBuilder<ContactInquiry> builder)
    {
        builder.ToTable("ContactInquiries");
        builder.HasKey(inquiry => inquiry.Id);
        builder.Property(inquiry => inquiry.PropertyId).IsRequired();
        builder.Property(inquiry => inquiry.VisitorName).HasMaxLength(200).IsRequired();
        builder.Property(inquiry => inquiry.VisitorEmail).HasMaxLength(320).IsRequired();
        builder.Property(inquiry => inquiry.VisitorPhone).HasMaxLength(50);
        builder.Property(inquiry => inquiry.Message).HasMaxLength(5000).IsRequired();
        builder.Property(inquiry => inquiry.CreatedAt).IsRequired();
        builder.HasIndex(inquiry => inquiry.PropertyId);
        builder.HasOne<Property>().WithMany().HasForeignKey(inquiry => inquiry.PropertyId).OnDelete(DeleteBehavior.Restrict);
    }
}

public sealed class AuditLogConfiguration : IEntityTypeConfiguration<AuditLog>
{
    public void Configure(EntityTypeBuilder<AuditLog> builder)
    {
        builder.ToTable("AuditLogs");
        builder.HasKey(log => log.Id);
        builder.Property(log => log.EntityName).HasMaxLength(100).IsRequired();
        builder.Property(log => log.EntityId).IsRequired();
        builder.Property(log => log.Action).HasMaxLength(100).IsRequired();
        builder.Property(log => log.Details).HasMaxLength(5000);
        builder.Property(log => log.ChangedAt).IsRequired();
        builder.HasIndex(log => new { log.EntityName, log.EntityId, log.ChangedAt });
    }
}