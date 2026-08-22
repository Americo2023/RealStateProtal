using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RealStatePortal.Domain.Entities;

namespace RealStatePortal.Infrastructure.Persistence.Configurations;

public sealed class UserRoleAssignmentConfiguration : IEntityTypeConfiguration<UserRoleAssignment>
{
    public void Configure(EntityTypeBuilder<UserRoleAssignment> builder)
    {
        builder.ToTable("UserRoles");
        builder.HasKey(role => role.Id);
        builder.Property(role => role.Role).HasConversion<string>().HasMaxLength(50).IsRequired();
        builder.HasIndex(role => new { role.UserId, role.Role }).IsUnique();
        builder.HasOne<User>().WithMany().HasForeignKey(role => role.UserId).OnDelete(DeleteBehavior.Cascade);
    }
}