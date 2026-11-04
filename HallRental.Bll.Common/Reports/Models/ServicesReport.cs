namespace HallRental.Bll.Common.Reports.Models;

public record ServicesReport(DateOnly From, DateOnly To, IReadOnlyList<ServiceReport> Services, decimal TotalRevenue);