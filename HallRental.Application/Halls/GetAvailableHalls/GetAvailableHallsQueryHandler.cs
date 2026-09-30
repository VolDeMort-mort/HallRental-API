using MediatR;
using HallRental.Application.Interfaces;
using HallRental.Domain.ValueObjects;

namespace HallRental.Application.Halls.GetAvailableHalls;

public class GetAvailableHallsQueryHandler: IRequestHandler<GetAvailableHallsQuery, IReadOnlyList<HallDto>>
{
    private readonly IHallRepository _halls;

    public GetAvailableHallsQueryHandler(IHallRepository halls)
    {
        _halls = halls;
    }

    public async Task<IReadOnlyList<HallDto>> Handle(GetAvailableHallsQuery request, CancellationToken cancellationToken)
    {
        var period = new RentalPeriod(request.Start, request.End);
        var halls = await _halls.GetAvailableAsync(request.MinCapacity, period, cancellationToken);

        return halls
            .Select(h => new HallDto(
                h.Id,
                h.Name,
                h.Capacity,
                h.PricePerHour,
                h.Services.Select(s => new ServiceDto(s.Id, s.Name, s.Price)).ToList()))
            .ToList();
    }
}
