using MediatR;

namespace HallRental.Application.Reports.HallsReport;

public record GetHallsReportQuery(DateOnly From, DateOnly To) : IRequest<HallsReportDto>, IReportPeriod;
