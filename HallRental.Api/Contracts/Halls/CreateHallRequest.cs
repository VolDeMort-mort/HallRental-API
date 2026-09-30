using HallRental.Application.Halls.AddHall;

namespace HallRental.Api.Contracts.Halls;

/// <summary>A new conference hall with the services it offers.</summary>
public sealed record CreateHallRequest
{
    /// <summary>Hall name.</summary>
    /// <example>Зал D</example>
    public required string Name { get; init; }

    /// <summary>Maximum number of people.</summary>
    /// <example>40</example>
    public required int Capacity { get; init; }

    /// <summary>Base rent per hour in UAH, before time-of-day discounts and markups.</summary>
    /// <example>1800</example>
    public required decimal PricePerHour { get; init; }

    /// <summary>Services the hall offers; may be empty.</summary>
    public required IReadOnlyList<ServiceRequest> Services { get; init; }

    public AddHallCommand ToCommand() => new(
        Name,
        Capacity,
        PricePerHour,
        Services.Select(s => new ServiceInput(s.Name, s.Price)).ToList());
}
