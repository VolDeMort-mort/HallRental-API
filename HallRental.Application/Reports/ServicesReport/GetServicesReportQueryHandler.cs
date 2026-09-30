using MediatR;
using HallRental.Application.Interfaces;

namespace HallRental.Application.Reports.ServicesReport;

public class GetServicesReportQueryHandler : IRequestHandler<GetServicesReportQuery, ServicesReportDto>
{
    private readonly IReportQueries _reports;

    public GetServicesReportQueryHandler(IReportQueries reports)
    {
        _reports = reports;
    }

    public async Task<ServicesReportDto> Handle(GetServicesReportQuery request, CancellationToken cancellationToken)
    {
        var services = await _reports.GetServiceRowsAsync(
            request.From.ToDateTime(TimeOnly.MinValue),
            request.To.ToDateTime(TimeOnly.MinValue),
            cancellationToken);

        return new ServicesReportDto(request.From, request.To, services, services.Sum(s => s.Revenue));
    }
}
