using HallRental.Bll.Common.Shared.Constants;
using HallRental.Dal.SqlRepositories.Pricing.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HallRental.Dal.SqlRepositories.Pricing.Configurations;

internal class HoursPricingConfiguration : IEntityTypeConfiguration<HoursPricingEntity>
{
    public void Configure(EntityTypeBuilder<HoursPricingEntity> builder)
    {
        builder.ToTable("HoursPricings");

        builder.HasKey(z => z.Id);
        builder.Property(z => z.Id).ValueGeneratedNever();
        builder.Property(z => z.Name).HasMaxLength(EntityLimits.HoursPricingNameMaxLength).IsRequired();
    }
}
