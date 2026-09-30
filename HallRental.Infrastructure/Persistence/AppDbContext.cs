using HallRental.Application.Interfaces;
using HallRental.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace HallRental.Infrastructure.Persistence;

public class AppDbContext : DbContext, IUnitOfWork
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<Hall> Halls => Set<Hall>();
    public DbSet<Booking> Bookings => Set<Booking>();
    public DbSet<HoursPricing> HoursPricings => Set<HoursPricing>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Picks up every IEntityTypeConfiguration in Persistence/Configurations
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly);
    }

    protected override void ConfigureConventions(ModelConfigurationBuilder configurationBuilder)
    {
        // Every money field at once: without an explicit precision SQL Server may round values silently
        configurationBuilder.Properties<decimal>().HavePrecision(18, 2);
    }
}
