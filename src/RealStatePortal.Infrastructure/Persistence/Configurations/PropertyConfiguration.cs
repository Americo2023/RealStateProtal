using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RealStatePortal.Domain.Entities;

namespace RealStatePortal.Infrastructure.Persistence.Configurations;

public sealed class PropertyConfiguration : IEntityTypeConfiguration<Property>
{
    public void Configure(EntityTypeBuilder<Property> builder)
    {
        builder.ToTable("Properties");
        builder.HasKey(property => property.Id);
        builder.Property(property => property.ReferenceNumber).HasMaxLength(50).IsRequired();
        builder.HasIndex(property => property.ReferenceNumber).IsUnique();
        builder.Property(property => property.Title).HasMaxLength(200).IsRequired();
        builder.Property(property => property.Description).HasMaxLength(5000).IsRequired();
        builder.Property(property => property.Status).HasConversion<string>().HasMaxLength(20).IsRequired();
        builder.Property(property => property.PropertyType).HasConversion<string>().HasMaxLength(30).IsRequired();
        builder.Property(property => property.EnergyClass).HasConversion<string>().HasMaxLength(20).IsRequired();
        builder.Property(property => property.LivingArea).HasPrecision(18, 2);
        builder.Property(property => property.TotalArea).HasPrecision(18, 2);
        builder.Property(property => property.CreatedAt).IsRequired();
        builder.Property(property => property.UpdatedAt).IsRequired();
        builder.Property(property => property.BrokerId).IsRequired();
        builder.HasIndex(property => new { property.Status, property.CreatedAt });
        builder.HasIndex(property => property.BrokerId);

        builder.OwnsOne(property => property.Price, price =>
        {
            price.Property(value => value.Amount).HasColumnName("Price").HasPrecision(18, 2).IsRequired();
            price.Property(value => value.Currency).HasColumnName("Currency").HasMaxLength(3).IsRequired();
        });

        builder.HasOne(property => property.Address)
            .WithOne()
            .HasForeignKey<PropertyAddress>("PropertyId")
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(property => property.Images)
            .WithOne()
            .HasForeignKey("PropertyId")
            .OnDelete(DeleteBehavior.Cascade);
        builder.Navigation(property => property.Images).UsePropertyAccessMode(PropertyAccessMode.Field);
    }
}