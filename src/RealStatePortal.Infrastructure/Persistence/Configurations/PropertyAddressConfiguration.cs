using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RealStatePortal.Domain.Entities;

namespace RealStatePortal.Infrastructure.Persistence.Configurations;

public sealed class PropertyAddressConfiguration : IEntityTypeConfiguration<PropertyAddress>
{
    public void Configure(EntityTypeBuilder<PropertyAddress> builder)
    {
        builder.ToTable("PropertyAddresses");
        builder.HasKey(address => address.Id);
        builder.Property(address => address.Street).HasMaxLength(200).IsRequired();
        builder.Property(address => address.StreetNumber).HasMaxLength(30).IsRequired();
        builder.Property(address => address.PostalCode).HasMaxLength(20).IsRequired();
        builder.Property(address => address.City).HasMaxLength(100).IsRequired();
        builder.Property(address => address.Region).HasMaxLength(100).IsRequired();
        builder.Property(address => address.Country).HasMaxLength(100).IsRequired();

        builder.OwnsOne(address => address.Coordinates, coordinates =>
        {
            coordinates.Property(value => value.Latitude).HasColumnName("Latitude").HasPrecision(9, 6);
            coordinates.Property(value => value.Longitude).HasColumnName("Longitude").HasPrecision(9, 6);
        });
    }
}