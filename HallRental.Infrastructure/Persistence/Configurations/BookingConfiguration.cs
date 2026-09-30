using HallRental.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HallRental.Infrastructure.Persistence.Configurations;

public class BookingConfiguration : IEntityTypeConfiguration<Booking>
{
    public void Configure(EntityTypeBuilder<Booking> builder)
    {
        builder.HasKey(b => b.Id);
        builder.Property(b => b.Id).ValueGeneratedNever();

        // Bookings are the history for reports: a hall with bookings can't be removed from the database
        builder.HasOne<Hall>()
            .WithMany()
            .HasForeignKey(b => b.HallId)
            .OnDelete(DeleteBehavior.Restrict);

        // The period is stored in the booking's own table as two columns
        builder.OwnsOne(b => b.Period, period =>
        {
            period.Property(p => p.Start).HasColumnName("Start");
            period.Property(p => p.End).HasColumnName("End");
        });
        builder.Navigation(b => b.Period).IsRequired();

        // Copies of the picked services with their prices at the booking moment
        builder.OwnsMany(b => b.Services, service =>
        {
            service.ToTable("BookedServices");
            service.WithOwner().HasForeignKey("BookingId");
            service.Property(s => s.Name).HasMaxLength(Service.MaxNameLength).IsRequired();
        });
        builder.Navigation(b => b.Services).UsePropertyAccessMode(PropertyAccessMode.Field);

        // Calculated as RentPrice + ServicesPrice, nothing to store
        builder.Ignore(b => b.TotalPrice);

        // Overlap checks and the search filter bookings by hall
        builder.HasIndex(b => b.HallId);
    }
}
