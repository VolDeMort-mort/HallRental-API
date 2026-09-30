using HallRental.Application.Halls.AddHall;

namespace HallRental.Api.Contracts.Halls;

public sealed record CreateHallRequest
{
    /// <example>Зал D</example>
    public required string Name { get; init; }

    /// <example>40</example>
    public required int Capacity { get; init; }

    /// <summary>Base rent per hour in UAH, before time-of-day discounts and markups.</summary>
    /// <example>1800</example>
    public required decimal PricePerHour { get; init; }

    public required IReadOnlyList<ServiceRequest> Services { get; init; }

    public AddHallCommand ToCommand() => new(
        Name,
        Capacity,
        PricePerHour,
        Services.Select(s => new ServiceInput(s.Name, s.Price)).ToList());
}
