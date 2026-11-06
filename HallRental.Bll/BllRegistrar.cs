using FluentValidation;
using HallRental.Bll.Bookings;
using HallRental.Bll.Common.Bookings;
using HallRental.Bll.Common.Halls;
using HallRental.Bll.Common.Pricing;
using HallRental.Bll.Common.Reports;
using HallRental.Bll.Common.Shared;
using HallRental.Bll.Halls;
using HallRental.Bll.Pricing;
using HallRental.Bll.Reports;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace HallRental.Bll;

public class BllRegistrar : IDependencyRegistrar
{
    public void Register(IServiceCollection services, IConfiguration configuration)
    {
        services.AddScoped<IHallManager, HallManager>();
        services.AddScoped<IBookingManager, BookingManager>();
        services.AddScoped<IReportManager, ReportManager>();
        services.AddScoped<IPricingManager, PricingManager>();

        services.AddValidatorsFromAssembly(typeof(BllRegistrar).Assembly, includeInternalTypes: true);

        // Without this, messages follow the machine's language: Ukrainian on a dev PC, English on a Linux server
        ValidatorOptions.Global.LanguageManager.Enabled = false;
    }
}
