using MediatR;

namespace HallRental.Application.Reports.ServicesReport;

public record GetServicesReportQuery(DateOnly From, DateOnly To) : IRequest<ServicesReportDto>, IReportPeriod;
