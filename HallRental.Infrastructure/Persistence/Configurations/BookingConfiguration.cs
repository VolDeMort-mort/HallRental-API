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
        builder.Property(b => b.ClientId).HasMaxLength(Booking.MaxClientIdLength).IsRequired();
        builder.HasIndex(b => b.ClientId);

        // Bookings are the history for reports, so a hall with bookings can't be removed from the database
        builder.HasOne<Hall>()
            .WithMany()
            .HasForeignKey(b => b.HallId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.OwnsOne(b => b.Period, period =>
        {
            period.Property(p => p.Start).HasColumnName("Start");
            period.Property(p => p.End).HasColumnName("End");
            period.HasIndex(p => p.Start);
        });
        builder.Navigation(b => b.Period).IsRequired();

        builder.OwnsMany(b => b.Services, service =>
        {
            service.ToTable("BookedServices");
            service.WithOwner().HasForeignKey("BookingId");
            service.Property(s => s.Name).HasMaxLength(Service.MaxNameLength).IsRequired();
        });
        builder.Navigation(b => b.Services).UsePropertyAccessMode(PropertyAccessMode.Field);

        builder.Ignore(b => b.TotalPrice);

        builder.HasIndex(b => b.HallId);
    }
}
