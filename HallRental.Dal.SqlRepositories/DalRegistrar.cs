using HallRental.Bll.Common.Bookings;
using HallRental.Bll.Common.Halls;
using HallRental.Bll.Common.Pricing;
using HallRental.Bll.Common.Reports;
using HallRental.Bll.Common.Shared;
using HallRental.Dal.SqlRepositories.Bookings;
using HallRental.Dal.SqlRepositories.Halls;
using HallRental.Dal.SqlRepositories.Mapping;
using HallRental.Dal.SqlRepositories.Pricing;
using HallRental.Dal.SqlRepositories.Reports;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace HallRental.Dal.SqlRepositories;

public class DalRegistrar : IDependencyRegistrar
{
    public void Register(IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("Default")
            ?? throw new InvalidOperationException("Connection string 'Default' is not configured");

        services.AddScoped(_ => new SqlUnitOfWork(connectionString));

        // The same scoped unit of work as the repositories, so SaveChanges commits what they wrote
        services.AddScoped<IUnitOfWork>(sp => sp.GetRequiredService<SqlUnitOfWork>());
        services.AddScoped<IHallRepository, SqlHallRepository>();
        services.AddScoped<IBookingRepository, SqlBookingRepository>();
        services.AddScoped<IHoursPricingRepository, SqlHoursPricingRepository>();
        services.AddScoped<IReportRepository, SqlReportRepository>();
        services.AddScoped<IDatabaseInitializer, DbInitializer>();

        services.AddAutoMapper(cfg => cfg.LicenseKey = configuration["AutoMapper:LicenseKey"], typeof(DalMappingProfile));

        services.AddSingleton(TimeProvider.System);
    }
}
