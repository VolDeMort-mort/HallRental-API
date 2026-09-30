using HallRental.Application.Halls.UpdateHall;

namespace HallRental.Api.Contracts.Halls;

/// <summary>Send every field; unchanged ones with their current values.</summary>
public sealed record UpdateHallRequest
{
    /// <example>Зал A</example>
    public required string Name { get; init; }

    /// <example>50</example>
    public required int Capacity { get; init; }

    /// <example>2500</example>
    public required decimal PricePerHour { get; init; }

    // The id comes from the route, so the body can't point the command at another hall
    public UpdateHallCommand ToCommand(Guid id) => new(id, Name, Capacity, PricePerHour);
}
