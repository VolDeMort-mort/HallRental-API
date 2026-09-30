namespace HallRental.Application.Reports.ServicesReport;

/// <summary>How often a service was ordered and how much it earned.</summary>
/// <param name="Name">Service name; the same service in different halls is counted together.</param>
/// <param name="TimesBooked">Number of bookings that included the service.</param>
/// <param name="Revenue">Earned in UAH at the prices of the booking moment.</param>
public record ServiceReportDto(string Name, int TimesBooked, decimal Revenue);

/// <summary>Popularity of the services: which ones are worth developing and which ones nobody orders.</summary>
/// <param name="From">First day of the period.</param>
/// <param name="To">Day after the period.</param>
/// <param name="Services">Most booked first.</param>
/// <param name="TotalRevenue">Revenue of all services.</param>
public record ServicesReportDto(DateOnly From, DateOnly To, IReadOnlyList<ServiceReportDto> Services, decimal TotalRevenue);
