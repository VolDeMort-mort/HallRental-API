using HallRental.Bll.Common.Shared.Constants;
using HallRental.Dal.SqlRepositories.Halls.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HallRental.Dal.SqlRepositories.Halls.Configurations;

internal class ServiceConfiguration : IEntityTypeConfiguration<ServiceEntity>
{
    public void Configure(EntityTypeBuilder<ServiceEntity> builder)
    {
        builder.ToTable("Services");

        builder.HasKey(s => s.Id);
        // Otherwise a service added to a loaded hall is sent as UPDATE instead of INSERT
        builder.Property(s => s.Id).ValueGeneratedNever();
        builder.Property(s => s.Name).HasMaxLength(EntityLimits.ServiceNameMaxLength).IsRequired();

        // Backs up the duplicate check in HallManager; the default SQL Server collation is case-insensitive too
        builder.HasIndex(s => new { s.HallId, s.Name }).IsUnique();
    }
}
