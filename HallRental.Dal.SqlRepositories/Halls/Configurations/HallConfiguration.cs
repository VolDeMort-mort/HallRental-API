using HallRental.Bll.Common.Shared.Constants;
using HallRental.Dal.SqlRepositories.Halls.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HallRental.Dal.SqlRepositories.Halls.Configurations;

internal class HallConfiguration : IEntityTypeConfiguration<HallEntity>
{
    public void Configure(EntityTypeBuilder<HallEntity> builder)
    {
        builder.ToTable("Halls");

        builder.HasKey(h => h.Id);
        builder.Property(h => h.Id).ValueGeneratedNever();
        builder.Property(h => h.Name).HasMaxLength(EntityLimits.HallNameMaxLength).IsRequired();

        // Soft delete
        builder.HasQueryFilter(h => h.IsActive);

        builder.HasMany(h => h.Services)
            .WithOne()
            .HasForeignKey(s => s.HallId)
            .IsRequired()
            .OnDelete(DeleteBehavior.Cascade);
    }
}
