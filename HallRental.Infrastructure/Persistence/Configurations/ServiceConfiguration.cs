using HallRental.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HallRental.Infrastructure.Persistence.Configurations;

public class ServiceConfiguration : IEntityTypeConfiguration<Service>
{
    public void Configure(EntityTypeBuilder<Service> builder)
    {
        builder.ToTable("Services");

        builder.HasKey(s => s.Id);
        // Otherwise a service added to a loaded hall is sent as UPDATE instead of INSERT
        builder.Property(s => s.Id).ValueGeneratedNever();
        builder.Property(s => s.Name).HasMaxLength(Service.MaxNameLength).IsRequired();

        // Backs up Hall.AddService; the default SQL Server collation is case-insensitive too
        builder.HasIndex("HallId", nameof(Service.Name)).IsUnique();
    }
}
