namespace HallRental.Bll.Common.Reports.Models;

public record ServiceReport(string Name, int TimesBooked, decimal Revenue);

public record ServicesReport(DateOnly From, DateOnly To, IReadOnlyList<ServiceReport> Services, decimal TotalRevenue);
