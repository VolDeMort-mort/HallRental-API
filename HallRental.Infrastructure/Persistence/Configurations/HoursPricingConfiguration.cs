using HallRental.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HallRental.Infrastructure.Persistence.Configurations;

public class HoursPricingConfiguration : IEntityTypeConfiguration<HoursPricing>
{
    public void Configure(EntityTypeBuilder<HoursPricing> builder)
    {
        builder.HasKey(z => z.Id);
        builder.Property(z => z.Id).ValueGeneratedNever();
        builder.Property(z => z.Name).HasMaxLength(HoursPricing.MaxNameLength).IsRequired();

        // TimeOnly maps to the SQL Server "time" type out of the box in EF Core 8
    }
}
