using HallRental.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HallRental.Infrastructure.Persistence.Configurations;

public class HallConfiguration : IEntityTypeConfiguration<Hall>
{
    public void Configure(EntityTypeBuilder<Hall> builder)
    {
        builder.HasKey(h => h.Id);
        // Ids come from the domain (Guid.NewGuid()), not from the database
        builder.Property(h => h.Id).ValueGeneratedNever();
        builder.Property(h => h.Name).HasMaxLength(Hall.MaxNameLength).IsRequired();

        // Soft delete: every query sees only active halls, without a Where in each repository method
        builder.HasQueryFilter(h => h.IsActive);

        // A service always belongs to a hall, so HallId is NOT NULL
        builder.HasMany(h => h.Services)
            .WithOne()
            .HasForeignKey("HallId")
            .IsRequired()
            .OnDelete(DeleteBehavior.Cascade);

        // Services is a read-only wrapper, so EF has to fill the private _services list
        builder.Navigation(h => h.Services).UsePropertyAccessMode(PropertyAccessMode.Field);
    }
}
