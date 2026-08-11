using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RealStatePortal.Domain.Entities;

namespace RealStatePortal.Infrastructure.Persistence.Configurations;

public sealed class ContactInquiryConfiguration : IEntityTypeConfiguration<ContactInquiry>
{
    public void Configure(EntityTypeBuilder<ContactInquiry> builder)
    {
        builder.ToTable("ContactInquiries");
        builder.HasKey(inquiry => inquiry.Id);
        builder.Property(inquiry => inquiry.PropertyId).IsRequired();
        builder.Property(inquiry => inquiry.VisitorName).HasMaxLength(200).IsRequired();
        builder.Property(inquiry => inquiry.VisitorEmail).HasMaxLength(320).IsRequired();
        builder.Property(inquiry => inquiry.VisitorPhone).HasMaxLength(40);
        builder.Property(inquiry => inquiry.Message).HasMaxLength(5000).IsRequired();
        builder.Property(inquiry => inquiry.CreatedAt).IsRequired();
        builder.HasIndex(inquiry => new { inquiry.PropertyId, inquiry.CreatedAt });
        builder.HasOne<Property>().WithMany().HasForeignKey(inquiry => inquiry.PropertyId).OnDelete(DeleteBehavior.Restrict);
    }
}