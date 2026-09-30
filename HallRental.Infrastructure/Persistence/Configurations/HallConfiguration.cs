using HallRental.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HallRental.Infrastructure.Persistence.Configurations;

public class HallConfiguration : IEntityTypeConfiguration<Hall>
{
    public void Configure(EntityTypeBuilder<Hall> builder)
    {
        builder.HasKey(h => h.Id);
        builder.Property(h => h.Id).ValueGeneratedNever();
        builder.Property(h => h.Name).HasMaxLength(Hall.MaxNameLength).IsRequired();

        // Soft delete
        builder.HasQueryFilter(h => h.IsActive);

        builder.HasMany(h => h.Services)
            .WithOne()
            .HasForeignKey("HallId")
            .IsRequired()
            .OnDelete(DeleteBehavior.Cascade);

        // Services is a read-only wrapper, EF has to fill the private _services list
        builder.Navigation(h => h.Services).UsePropertyAccessMode(PropertyAccessMode.Field);
    }
}
