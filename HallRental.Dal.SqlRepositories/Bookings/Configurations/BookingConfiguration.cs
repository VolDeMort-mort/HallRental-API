using HallRental.Bll.Common.Shared.Constants;
using HallRental.Dal.SqlRepositories.Bookings.Entities;
using HallRental.Dal.SqlRepositories.Halls.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HallRental.Dal.SqlRepositories.Bookings.Configurations;

internal class BookingConfiguration : IEntityTypeConfiguration<BookingEntity>
{
    public void Configure(EntityTypeBuilder<BookingEntity> builder)
    {
        builder.ToTable("Bookings");

        builder.HasKey(b => b.Id);
        builder.Property(b => b.Id).ValueGeneratedNever();
        builder.Property(b => b.ClientId).HasMaxLength(EntityLimits.ClientIdMaxLength).IsRequired();
        builder.HasIndex(b => b.ClientId);

        // Bookings are the history for reports, so a hall with bookings can't be removed from the database
        builder.HasOne<HallEntity>()
            .WithMany()
            .HasForeignKey(b => b.HallId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasIndex(b => b.Start);

        builder.OwnsMany(b => b.Services, service =>
        {
            service.ToTable("BookedServices");
            service.WithOwner().HasForeignKey("BookingId");
            service.Property(s => s.Name).HasMaxLength(EntityLimits.ServiceNameMaxLength).IsRequired();
        });

        builder.HasIndex(b => b.HallId);
    }
}
