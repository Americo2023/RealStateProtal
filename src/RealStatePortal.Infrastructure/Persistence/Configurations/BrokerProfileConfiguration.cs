using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RealStatePortal.Domain.Entities;

namespace RealStatePortal.Infrastructure.Persistence.Configurations;

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
        builder.Property(profile => profile.Phone).HasMaxLength(40).IsRequired();
        builder.Property(profile => profile.Bio).HasMaxLength(2000).IsRequired();
        builder.HasOne<User>().WithOne().HasForeignKey<BrokerProfile>(profile => profile.UserId).OnDelete(DeleteBehavior.Cascade);
    }
}