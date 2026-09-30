using HallRental.Application.Interfaces;
using HallRental.Infrastructure.Persistence;
using HallRental.Infrastructure.Persistence.Queries;
using HallRental.Infrastructure.Persistence.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace HallRental.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("Default")
            ?? throw new InvalidOperationException("Connection string 'Default' is not configured");

        services.AddDbContext<AppDbContext>(options =>
            options.UseSqlServer(connectionString, sql => sql.EnableRetryOnFailure()));

        // The same scoped context as the repositories, so SaveChanges sees what they added
        services.AddScoped<IUnitOfWork>(sp => sp.GetRequiredService<AppDbContext>());
        services.AddScoped<IHallRepository, HallRepository>();
        services.AddScoped<IBookingRepository, BookingRepository>();
        services.AddScoped<IHoursPricingRepository, HoursPricingRepository>();
        services.AddScoped<IReportQueries, ReportQueries>();

        services.AddSingleton(TimeProvider.System);

        return services;
    }
}
