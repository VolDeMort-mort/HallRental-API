namespace HallRental.Application.Reports.ServicesReport;

/// <summary>The same service in different halls is counted together, at the prices of the booking moment.</summary>
public record ServiceReportDto(string Name, int TimesBooked, decimal Revenue);

public record ServicesReportDto(DateOnly From, DateOnly To, IReadOnlyList<ServiceReportDto> Services, decimal TotalRevenue);
