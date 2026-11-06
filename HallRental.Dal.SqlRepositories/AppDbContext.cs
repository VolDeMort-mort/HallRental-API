using HallRental.Bll.Common.Shared;
using HallRental.Dal.SqlRepositories.Bookings.Entities;
using HallRental.Dal.SqlRepositories.Halls.Entities;
using HallRental.Dal.SqlRepositories.Pricing.Entities;
using Microsoft.EntityFrameworkCore;

namespace HallRental.Dal.SqlRepositories;

public class AppDbContext : DbContext, IUnitOfWork
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    internal DbSet<HallEntity> Halls => Set<HallEntity>();
    internal DbSet<ServiceEntity> Services => Set<ServiceEntity>();
    internal DbSet<BookingEntity> Bookings => Set<BookingEntity>();
    internal DbSet<HoursPricingEntity> HoursPricings => Set<HoursPricingEntity>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly);
    }

    protected override void ConfigureConventions(ModelConfigurationBuilder configurationBuilder)
    {
        // Without an explicit precision SQL Server may round money silently
        configurationBuilder.Properties<decimal>().HavePrecision(18, 2);
    }
}
