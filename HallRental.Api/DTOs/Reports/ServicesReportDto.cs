namespace HallRental.Api.DTOs.Reports;

public record ServiceReportDto(string Name, int TimesBooked, decimal Revenue);

public record ServicesReportDto(DateOnly From, DateOnly To, IReadOnlyList<ServiceReportDto> Services, decimal TotalRevenue);
