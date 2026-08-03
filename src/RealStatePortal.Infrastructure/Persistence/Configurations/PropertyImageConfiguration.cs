using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RealStatePortal.Domain.Entities;

namespace RealStatePortal.Infrastructure.Persistence.Configurations;

public sealed class PropertyImageConfiguration : IEntityTypeConfiguration<PropertyImage>
{
    public void Configure(EntityTypeBuilder<PropertyImage> builder)
    {
        builder.ToTable("PropertyImages");
        builder.HasKey(image => image.Id);
        builder.Property(image => image.Url).HasMaxLength(1000).IsRequired();
        builder.Property(image => image.AltText).HasMaxLength(250).IsRequired();
        builder.Property(image => image.SortOrder).IsRequired();
        builder.Property(image => image.IsPrimary).IsRequired();
        builder.HasIndex("PropertyId", nameof(PropertyImage.SortOrder)).IsUnique();
        builder.HasIndex("PropertyId", nameof(PropertyImage.IsPrimary))
            .HasFilter("[IsPrimary] = 1")
            .IsUnique();
    }
}