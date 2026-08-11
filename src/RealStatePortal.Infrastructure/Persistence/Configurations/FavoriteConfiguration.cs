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
        builder.HasOne<Property>().WithMany().HasForeignKey(favorite => favorite.PropertyId).OnDelete(DeleteBehavior.Restrict);
        builder.HasOne<User>().WithMany().HasForeignKey(favorite => favorite.UserId).OnDelete(DeleteBehavior.Restrict);
    }
}